using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniVehicle.Data;
using MiniVehicle.Models;
using MiniVehicle.Services;
using Serilog;

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
FleetObs.ConfigureLogger("minivehicle");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.WebHost.UseUrls($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=minivehicle.db";
builder.Services.AddDbContext<AppDbContext>(o =>
{
    if (DbUtil.IsPostgres(conn)) o.UseNpgsql(DbUtil.ToNpgsql(conn));
    else o.UseSqlite(conn);
});
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var ssoAuthority = Environment.GetEnvironmentVariable("SSO_AUTHORITY") ?? "https://minisso.onrender.com";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.Authority = ssoAuthority;
    o.RequireHttpsMetadata = ssoAuthority.StartsWith("https");
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = ssoAuthority,
        ValidateAudience = false, ValidateLifetime = true, NameClaimType = "name", RoleClaimType = "role"
    };
});
builder.Services.AddAuthorization();
builder.Services.AddFleetObs();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await Seeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());

app.UseFleetObs();
FleetObs.ReportLicense(ssoAuthority, "minivehicle");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/whoami", (ClaimsPrincipal u) => Results.Ok(new
{
    app = "minivehicle",
    sub = u.FindFirst("sub")?.Value, name = u.Identity?.Name ?? u.FindFirst("name")?.Value,
    email = u.FindFirst("email")?.Value, tenant = u.FindFirst("tenant")?.Value,
    roles = u.FindAll("role").Select(c => c.Value)
})).RequireAuthorization();

// Multi-tenant (OEM/NPP): org = header X-Api-Key / cookie org_key. Tra cứu VIN công khai KHÔNG lọc tenant.
app.Use(async (ctx, next) =>
{
    var key = ctx.Request.Headers["X-Api-Key"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(key)) ctx.Request.Cookies.TryGetValue(TenantContext.CookieName, out key);
    if (!string.IsNullOrWhiteSpace(key))
    {
        using var lookup = app.Services.CreateScope();
        var ldb = lookup.ServiceProvider.GetRequiredService<AppDbContext>();
        var org = await ldb.Orgs.FirstOrDefaultAsync(o => o.ApiKey == key);
        if (org != null) ctx.RequestServices.GetRequiredService<ITenantContext>().OrgId = org.Id;
    }
    await next();
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/healthz", () => "ok");

// ===== Sổ đăng ký xe theo VIN (chuyển đổi BizHTC.Car) =====

// Nhập xe vào kho (tạo hồ sơ VIN)
app.MapPost("/api/vehicles", async (RegisterVehicleDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin) || string.IsNullOrWhiteSpace(dto.Model))
        return Results.BadRequest(new { error = "Cần Vin và Model." });
    try { return Results.Ok(await svc.RegisterAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.Conflict(new { error = ex.Message }); }
}).RequireAuthorization();

// Danh sách / tìm kiếm xe theo trạng thái, model, đại lý
app.MapGet("/api/vehicles", async (IVehicleService svc, string? status, string? model, string? dealer) =>
    Results.Ok(await svc.ListAsync(status, model, dealer))).RequireAuthorization();

// Import hàng loạt data thật từ Car_VIN (SQL nguồn 2010.HTC) — dedupe theo VIN, bỏ qua VIN đã tồn tại (không throw như RegisterAsync).
app.MapPost("/api/import/vehicles", async (List<ImportVehicleRowDto> rows, AppDbContext db, ITenantContext tenant) =>
{
    if (rows is null || rows.Count == 0) return Results.BadRequest(new { error = "Không có dữ liệu import." });
    int added = 0, skipped = 0;
    foreach (var r in rows)
    {
        if (string.IsNullOrWhiteSpace(r.VIN) || string.IsNullOrWhiteSpace(r.ModelCode)) { skipped++; continue; }
        var vin = r.VIN.Trim().ToUpperInvariant();
        if (await db.Vehicles.AnyAsync(v => v.OrgId == tenant.OrgId && v.Vin == vin)) { skipped++; continue; }
        db.Vehicles.Add(new Vehicle
        {
            OrgId = tenant.OrgId, Vin = vin, Model = r.ModelCode.Trim(), EngineNo = r.EngineNo, Color = r.ColorCode,
            ModelYear = r.ProductionYearActual, StorageCode = r.StorageCodeCurrent, Status = VehicleStatus.InStock
        });
        added++;
    }
    await db.SaveChangesAsync();
    return Results.Ok(new { added, skipped, total = rows.Count });
}).RequireAuthorization();

// Phân bổ xe cho đại lý (InStock → Allocated)
app.MapPost("/api/vehicles/{vin}/allocate", async (string vin, AllocateDto dto, IVehicleService svc) =>
{
    var r = await svc.AllocateAsync(vin, dto.DealerCode);
    return r is null ? Results.NotFound(new { vin, error = "Không thấy xe hoặc không ở trạng thái kho." }) : Results.Ok(r);
}).RequireAuthorization();

// Tạo lệnh giao xe (nhiều VIN → 1 đại lý). Xe chuyển OnDelivery.
app.MapPost("/api/delivery-orders", async (CreateDoDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Vins is null || dto.Vins.Count == 0)
        return Results.BadRequest(new { error = "Cần DealerCode và danh sách Vins." });
    try { return Results.Ok(await svc.CreateDeliveryOrderAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

// Giao xe (chốt lệnh): OnDelivery → Delivered, kích hoạt bảo hành + gán chủ xe.
app.MapPost("/api/delivery-orders/{doNo}/deliver", async (string doNo, DeliverDto dto, IVehicleService svc) =>
{
    var r = await svc.DeliverAsync(doNo, dto);
    return r is null ? Results.NotFound(new { doNo, error = "Không thấy lệnh hoặc đã giao." }) : Results.Ok(r);
}).RequireAuthorization();

// Lịch sử vòng đời 1 xe
app.MapGet("/api/vehicles/{vin}/history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.HistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin }) : Results.Ok(r);
}).RequireAuthorization();

// Thống kê vòng đời
app.MapGet("/api/stats", async (IVehicleService svc) => Results.Ok(await svc.StatsAsync())).RequireAuthorization();

// ---- Đổi chủ + đăng ký biển số (sau giao) ----
app.MapPost("/api/vehicles/{vin}/transfer", async (string vin, TransferDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.NewOwnerName)) return Results.BadRequest(new { error = "Cần NewOwnerName." });
    var r = await svc.TransferAsync(vin, dto);
    return r is null ? Results.NotFound(new { vin, error = "Không thấy xe hoặc xe chưa giao." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/vehicles/{vin}/register-plate", async (string vin, RegisterPlateDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PlateNo)) return Results.BadRequest(new { error = "Cần PlateNo." });
    var r = await svc.RegisterPlateAsync(vin, dto.PlateNo);
    return r is null ? Results.NotFound(new { vin, error = "Không thấy xe hoặc xe chưa giao." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu bảo hành (GrtClaim) ----
app.MapPost("/api/claims", async (CreateClaimDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin) || string.IsNullOrWhiteSpace(dto.Issue))
        return Results.BadRequest(new { error = "Cần Vin và Issue." });
    try { return Results.Ok(await svc.CreateClaimAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/claims", async (IVehicleService svc, string? status, string? vin, string? dealer) =>
    Results.Ok(await svc.ListClaimsAsync(status, vin, dealer))).RequireAuthorization();

app.MapPost("/api/claims/{claimNo}/approve", async (string claimNo, ClaimDecisionDto dto, IVehicleService svc) =>
{
    var r = await svc.DecideClaimAsync(claimNo, true, dto.Note);
    return r is null ? Results.NotFound(new { claimNo, error = "Không thấy claim đang chờ duyệt." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/claims/{claimNo}/reject", async (string claimNo, ClaimDecisionDto dto, IVehicleService svc) =>
{
    var r = await svc.DecideClaimAsync(claimNo, false, dto.Note);
    return r is null ? Results.NotFound(new { claimNo, error = "Không thấy claim đang chờ duyệt." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/claims/{claimNo}/settle", async (string claimNo, IVehicleService svc) =>
{
    var r = await svc.SettleClaimAsync(claimNo);
    return r is null ? Results.NotFound(new { claimNo, error = "Claim chưa duyệt hoặc không tồn tại." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Triệu hồi (recall) ----
app.MapPost("/api/recalls", async (CreateRecallDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Code) || string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { error = "Cần Code và Title." });
    try { return Results.Ok(await svc.CreateRecallAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.Conflict(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/recalls", async (IVehicleService svc) => Results.Ok(await svc.ListRecallsAsync())).RequireAuthorization();

app.MapGet("/api/recalls/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.RecallAffectedAsync(code);
    return r is null ? Results.NotFound(new { code }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/recalls/{code}/done", async (string code, RecallDoneDto dto, IVehicleService svc) =>
{
    var r = await svc.MarkRecallDoneAsync(code, dto);
    return r is null ? Results.NotFound(new { code, dto.Vin, error = "Không thấy campaign/xe trong campaign." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Chuyển kho / điều chuyển xe ----
app.MapPost("/api/transfers", async (CreateTransferDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin) || string.IsNullOrWhiteSpace(dto.ToDealer))
        return Results.BadRequest(new { error = "Cần Vin và ToDealer." });
    try { return Results.Ok(await svc.CreateTransferAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/transfers", async (IVehicleService svc, string? status) =>
    Results.Ok(await svc.ListTransfersAsync(status))).RequireAuthorization();

app.MapPost("/api/transfers/{code}/{action}", async (string code, string action, IVehicleService svc) =>
{
    if (action is not ("approve" or "reject" or "receive"))
        return Results.BadRequest(new { error = "action = approve|reject|receive" });
    var r = await svc.TransferTransitionAsync(code, action);
    return r is null ? Results.NotFound(new { code, error = "Không thấy hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị giao tài liệu xe (CarDocReq/ĐNGT) ----
app.MapPost("/api/docreqs", async (CreateDocReqDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần Vin và DealerCode." });
    try { return Results.Ok(await svc.CreateDocReqAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/docreqs", async (IVehicleService svc, string? status, string? dealer, string? vin) =>
    Results.Ok(await svc.ListDocReqAsync(status, dealer, vin))).RequireAuthorization();

app.MapPost("/api/docreqs/{code}/{action}", async (string code, string action, ShipDocDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "reject" or "ship" or "receive"))
        return Results.BadRequest(new { error = "action = approve|reject|ship|receive" });
    var r = await svc.DocReqTransitionAsync(code, action, dto?.TrackingNo);
    return r is null ? Results.NotFound(new { code, error = "Không thấy hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Biên bản giao nhận xe (Sto_DlvMinutes / DeliveryMinutes) ----
app.MapPost("/api/delivery-minutes", async (CreateDeliveryMinutesDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần Vin và DealerCode." });
    try { return Results.Ok(await svc.CreateDeliveryMinutesAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/delivery-minutes", async (IVehicleService svc, string? status, string? dealer, string? vin) =>
    Results.Ok(await svc.ListDeliveryMinutesAsync(status, dealer, vin))).RequireAuthorization();

app.MapGet("/api/delivery-minutes/{dlvMnNo}", async (string dlvMnNo, IVehicleService svc) =>
{
    var r = await svc.GetDeliveryMinutesAsync(dlvMnNo);
    return r is null ? Results.NotFound(new { dlvMnNo, error = "Không tìm thấy biên bản." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/delivery-minutes/{dlvMnNo}/inspect", async (string dlvMnNo, InspectDeliveryMinutesDto dto, IVehicleService svc) =>
{
    var r = await svc.InspectDeliveryMinutesAsync(dlvMnNo, dto);
    return r is null ? Results.NotFound(new { dlvMnNo, error = "Không thấy biên bản hoặc biên bản đã chốt/từ chối." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/delivery-minutes/{dlvMnNo}/confirm", async (string dlvMnNo, ConfirmDeliveryMinutesDto dto, IVehicleService svc) =>
{
    var r = await svc.ConfirmDeliveryMinutesAsync(dlvMnNo, dto);
    return r is null ? Results.NotFound(new { dlvMnNo, error = "Không thấy biên bản hoặc biên bản đã chốt/từ chối." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/delivery-minutes/{dlvMnNo}/reject", async (string dlvMnNo, RejectDeliveryMinutesDto dto, IVehicleService svc) =>
{
    var r = await svc.RejectDeliveryMinutesAsync(dlvMnNo, dto.Reason);
    return r is null ? Results.NotFound(new { dlvMnNo, error = "Không thấy biên bản hoặc biên bản đã chốt/từ chối." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Lệnh thu hồi xe về kho (Sto_CarRetrieve / CarRetrieve) ----
app.MapPost("/api/retrieves", async (CreateCarRetrieveDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Vins is null || dto.Vins.Count == 0)
        return Results.BadRequest(new { error = "Cần DealerCode và danh sách Vins." });
    try { return Results.Ok(await svc.CreateCarRetrieveAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/retrieves", async (IVehicleService svc, string? status, string? dealer, string? vin) =>
    Results.Ok(await svc.ListCarRetrievesAsync(status, dealer, vin))).RequireAuthorization();

app.MapGet("/api/retrieves/{retrieveNo}", async (string retrieveNo, IVehicleService svc) =>
{
    var r = await svc.GetCarRetrieveAsync(retrieveNo);
    return r is null ? Results.NotFound(new { retrieveNo, error = "Không tìm thấy lệnh thu hồi." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/retrieves/{retrieveNo}/{action}", async (string retrieveNo, string action, CarRetrieveTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "ship" or "dispatch" or "receive" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|ship|dispatch|receive|reject|cancel" });
    var r = await svc.CarRetrieveTransitionAsync(retrieveNo, action, dto?.Note);
    return r is null ? Results.NotFound(new { retrieveNo, error = "Không thấy lệnh thu hồi hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu / Kế hoạch vận chuyển xe (BizHTC.Car.TransportReq / Car_TransportReq) ----
app.MapPost("/api/transport-requests", async (CreateTransportRequestDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Vins is null || dto.Vins.Count == 0)
        return Results.BadRequest(new { error = "Cần DealerCode và danh sách Vins." });
    try { return Results.Ok(await svc.CreateTransportRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/transport-requests", async (IVehicleService svc, string? status, string? dealer, string? transporter, string? vin) =>
    Results.Ok(await svc.ListTransportRequestsAsync(status, dealer, transporter, vin))).RequireAuthorization();

app.MapGet("/api/transport-requests/{transportReqNo}", async (string transportReqNo, IVehicleService svc) =>
{
    var r = await svc.GetTransportRequestAsync(transportReqNo);
    return r is null ? Results.NotFound(new { transportReqNo, error = "Không tìm thấy yêu cầu vận chuyển." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/transport-requests/{transportReqNo}/{action}", async (string transportReqNo, string action, TransportRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "dispatch" or "ship" or "complete" or "receive" or "deliver" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|dispatch|ship|complete|receive|deliver|reject|cancel" });
    var r = await svc.TransportRequestTransitionAsync(transportReqNo, action, dto);
    return r is null ? Results.NotFound(new { transportReqNo, error = "Không thấy yêu cầu vận chuyển hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Lệnh tái sắp xếp kho bãi nội bộ OEM (BizHTC.Storage.StorageRearrange / Sto_StorageRearrange) ----
app.MapPost("/api/rearranges", async (CreateStorageRearrangeDto dto, IVehicleService svc) =>
{
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách Items (VIN và StorageCodeTo)." });
    try { return Results.Ok(await svc.CreateStorageRearrangeAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/rearranges", async (IVehicleService svc, string? status, string? vin, string? storageCodeTo) =>
    Results.Ok(await svc.ListStorageRearrangesAsync(status, vin, storageCodeTo))).RequireAuthorization();

app.MapGet("/api/rearranges/{storageRearrangeNo}", async (string storageRearrangeNo, IVehicleService svc) =>
{
    var r = await svc.GetStorageRearrangeAsync(storageRearrangeNo);
    return r is null ? Results.NotFound(new { storageRearrangeNo, error = "Không tìm thấy lệnh tái sắp xếp kho." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/rearranges/{storageRearrangeNo}/{action}", async (string storageRearrangeNo, string action, StorageRearrangeTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "start" or "move" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|start|move|complete|reject|cancel" });
    var r = await svc.StorageRearrangeTransitionAsync(storageRearrangeNo, action, dto);
    return r is null ? Results.NotFound(new { storageRearrangeNo, error = "Không thấy lệnh tái sắp xếp hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/rearranges/{storageRearrangeNo}/lines/{vin}/complete", async (string storageRearrangeNo, string vin, CompleteStorageRearrangeLineDto? dto, IVehicleService svc) =>
{
    var r = await svc.CompleteStorageRearrangeLineAsync(storageRearrangeNo, vin, dto);
    return r is null ? Results.NotFound(new { storageRearrangeNo, vin, error = "Không tìm thấy dòng chi tiết hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đăng ký / Quản lý xe lái thử & chạy thử (BizHTC.Car.Car_TestCar / TestCar) ----
app.MapPost("/api/test-cars", async (CreateTestCarDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Vins is null || dto.Vins.Count == 0)
        return Results.BadRequest(new { error = "Cần DealerCode và danh sách Vins." });
    try { return Results.Ok(await svc.CreateTestCarAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/test-cars", async (IVehicleService svc, string? status, string? dealer, string? vin) =>
    Results.Ok(await svc.ListTestCarsAsync(status, dealer, vin))).RequireAuthorization();

app.MapGet("/api/test-cars/{testCarCode}", async (string testCarCode, IVehicleService svc) =>
{
    var r = await svc.GetTestCarAsync(testCarCode);
    return r is null ? Results.NotFound(new { testCarCode, error = "Không tìm thấy phiếu đăng ký xe lái thử." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/test-cars/{testCarCode}/{action}", async (string testCarCode, string action, TestCarTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "start" or "handover" or "inuse" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|start|handover|inuse|finish|complete|reject|cancel" });
    var r = await svc.TestCarTransitionAsync(testCarCode, action, dto);
    return r is null ? Results.NotFound(new { testCarCode, error = "Không thấy phiếu lái thử hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/test-cars/{testCarCode}/lines/{vin}/finish", async (string testCarCode, string vin, FinishTestCarLineDto? dto, IVehicleService svc) =>
{
    var r = await svc.FinishTestCarLineAsync(testCarCode, vin, dto);
    return r is null ? Results.NotFound(new { testCarCode, vin, error = "Không tìm thấy dòng chi tiết hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Kiểm tra chất lượng tiền bàn giao xe PDI (BizHTC.WH.DlrPDIRequest / Dlr_PDIRequest) ----
app.MapPost("/api/pdi-requests", async (CreatePdiRequestDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần DealerCode để tạo yêu cầu PDI." });
    try { return Results.Ok(await svc.CreatePdiRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/pdi-requests", async (IVehicleService svc, string? status, string? dealer, string? vin) =>
    Results.Ok(await svc.ListPdiRequestsAsync(status, dealer, vin))).RequireAuthorization();

app.MapGet("/api/pdi-requests/{pdiReqNo}", async (string pdiReqNo, IVehicleService svc) =>
{
    var r = await svc.GetPdiRequestAsync(pdiReqNo);
    return r is null ? Results.NotFound(new { pdiReqNo, error = "Không tìm thấy phiếu yêu cầu PDI." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/pdi-requests/{pdiReqNo}/{action}", async (string pdiReqNo, string action, PdiRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "start" or "inspect" or "complete" or "pass" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|start|inspect|complete|pass|reject|cancel" });
    var r = await svc.PdiRequestTransitionAsync(pdiReqNo, action, dto);
    return r is null ? Results.NotFound(new { pdiReqNo, error = "Không thấy phiếu PDI hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/pdi-requests/{pdiReqNo}/lines/{vin}/inspect", async (string pdiReqNo, string vin, InspectPdiLineDto dto, IVehicleService svc) =>
{
    var r = await svc.InspectPdiLineAsync(pdiReqNo, vin, dto);
    return r is null ? Results.NotFound(new { pdiReqNo, vin, error = "Không tìm thấy dòng chi tiết PDI hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Thế chấp xe ngân hàng (BizHTC.GiaiChap.RM_ReqMortgage / RM_ReqMortgage) ----
app.MapPost("/api/mortgages", async (CreateMortgageRequestDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.BankCode))
        return Results.BadRequest(new { error = "Cần mã Ngân hàng thế chấp (BankCode)." });
    try { return Results.Ok(await svc.CreateMortgageRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/mortgages", async (IVehicleService svc, string? status, string? bank, string? vin) =>
    Results.Ok(await svc.ListMortgageRequestsAsync(status, bank, vin))).RequireAuthorization();

app.MapGet("/api/mortgages/{reqMortgageNo}", async (string reqMortgageNo, IVehicleService svc) =>
{
    var r = await svc.GetMortgageRequestAsync(reqMortgageNo);
    return r is null ? Results.NotFound(new { reqMortgageNo, error = "Không tìm thấy yêu cầu thế chấp." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/mortgages/{reqMortgageNo}/{action}", async (string reqMortgageNo, string action, MortgageRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|reject|cancel" });
    var r = await svc.MortgageRequestTransitionAsync(reqMortgageNo, action, dto);
    return r is null ? Results.NotFound(new { reqMortgageNo, error = "Không thấy yêu cầu thế chấp hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Giải chấp xe ngân hàng (BizHTC.GiaiChap.RD_ReqRedeem / RD_ReqRedeem) ----
app.MapPost("/api/redeems", async (CreateRedeemRequestDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.BankCode))
        return Results.BadRequest(new { error = "Cần DealerCode và BankCode." });
    try { return Results.Ok(await svc.CreateRedeemRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/redeems", async (IVehicleService svc, string? status, string? dealer, string? bank, string? vin) =>
    Results.Ok(await svc.ListRedeemRequestsAsync(status, dealer, bank, vin))).RequireAuthorization();

app.MapGet("/api/redeems/{redeemReqNo}", async (string redeemReqNo, IVehicleService svc) =>
{
    var r = await svc.GetRedeemRequestAsync(redeemReqNo);
    return r is null ? Results.NotFound(new { redeemReqNo, error = "Không tìm thấy yêu cầu giải chấp." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/redeems/{redeemReqNo}/{action}", async (string redeemReqNo, string action, RedeemRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|complete|reject|cancel" });
    var r = await svc.RedeemRequestTransitionAsync(redeemReqNo, action, dto);
    return r is null ? Results.NotFound(new { redeemReqNo, error = "Không thấy yêu cầu giải chấp hoặc sai trạng thái." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đơn đặt hàng xe ô tô của Đại lý (BizHTC.Order.Ord_SalesOrder / SalesOrder) ----
app.MapPost("/api/sales-orders", async (CreateSalesOrderDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần DealerCode và danh sách Items." });
    try { return Results.Ok(await svc.CreateSalesOrderAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/sales-orders", async (IVehicleService svc, string? status, string? dealer, string? orderMonth, string? model) =>
    Results.Ok(await svc.ListSalesOrdersAsync(status, dealer, orderMonth, model))).RequireAuthorization();

app.MapGet("/api/sales-orders/{soCode}", async (string soCode, IVehicleService svc) =>
{
    var r = await svc.GetSalesOrderAsync(soCode);
    return r is null ? Results.NotFound(new { soCode, error = "Không tìm thấy đơn đặt hàng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/sales-orders/{soCode}/{action}", async (string soCode, string action, SalesOrderTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve1" or "approve2" or "approve" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|approve|reject|cancel" });
    var r = await svc.SalesOrderTransitionAsync(soCode, action, dto);
    return r is null ? Results.NotFound(new { soCode, error = "Không thấy đơn đặt hàng hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/sales-orders/{soCode}/allocate-vin", async (string soCode, AllocateSoVinDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần Vin để phân bổ." });
    try
    {
        var r = await svc.AllocateSoVinAsync(soCode, dto);
        return r is null ? Results.NotFound(new { soCode, error = "Không tìm thấy đơn hàng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-orders/{soCode}/deallocate-vin/{vin}", async (string soCode, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.DeallocateSoVinAsync(soCode, vin);
        return r is null ? Results.NotFound(new { soCode, error = "Không tìm thấy đơn hàng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

// ---- Giao dịch bán lẻ ô tô của Đại lý & Kích hoạt Sổ bảo hành điện tử (BizHTC.DealerSales / DLS_Deal) ----
app.MapPost("/api/dealer-deals", async (CreateDealerDealDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.CustomerName) || string.IsNullOrWhiteSpace(dto.CustomerPhone))
        return Results.BadRequest(new { error = "Cần DealerCode, CustomerName và CustomerPhone." });
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe Items trong giao dịch bán lẻ." });
    try { return Results.Ok(await svc.CreateDealerDealAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/dealer-deals", async (IVehicleService svc, string? status, string? dealer, string? customer, string? salesMan, string? paymentType, string? vin) =>
    Results.Ok(await svc.ListDealerDealsAsync(status, dealer, customer, salesMan, paymentType, vin))).RequireAuthorization();

app.MapGet("/api/dealer-deals/{dealNo}", async (string dealNo, IVehicleService svc) =>
{
    var r = await svc.GetDealerDealAsync(dealNo);
    return r is null ? Results.NotFound(new { dealNo, error = "Không tìm thấy giao dịch bán lẻ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/dealer-deals/{dealNo}/{action}", async (string dealNo, string action, DealerDealTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "deliver" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|deliver|reject|cancel" });
    var r = await svc.DealerDealTransitionAsync(dealNo, action, dto);
    return r is null ? Results.NotFound(new { dealNo, error = "Không thấy giao dịch bán lẻ hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/dealer-deals/{dealNo}/lines/{vin}/update-delivery", async (string dealNo, string vin, UpdateDealerDealLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateDealerDealLineAsync(dealNo, vin, dto);
    return r is null ? Results.NotFound(new { dealNo, vin, error = "Không tìm thấy dòng xe trong giao dịch." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Bảo lãnh thanh toán ngân hàng mua xe ô tô cho Đại lý (BizHTC.Payment / Pmt_Guarantee) ----
app.MapPost("/api/guarantees", async (CreatePaymentGuaranteeDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.BankGuaranteeNo) || string.IsNullOrWhiteSpace(dto.BankCode) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần BankGuaranteeNo, BankCode và DealerCode." });
    if (dto.TotalAmount <= 0)
        return Results.BadRequest(new { error = "Tổng hạn mức TotalAmount phải > 0." });
    try { return Results.Ok(await svc.CreatePaymentGuaranteeAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/guarantees", async (IVehicleService svc, string? status, string? dealer, string? bank, string? vin) =>
    Results.Ok(await svc.ListPaymentGuaranteesAsync(status, dealer, bank, vin))).RequireAuthorization();

app.MapGet("/api/guarantees/{guaranteeNo}", async (string guaranteeNo, IVehicleService svc) =>
{
    var r = await svc.GetPaymentGuaranteeAsync(guaranteeNo);
    return r is null ? Results.NotFound(new { guaranteeNo, error = "Không tìm thấy chứng thư bảo lãnh ngân hàng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantees/{guaranteeNo}/{action}", async (string guaranteeNo, string action, PaymentGuaranteeTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|settle|reject|cancel" });
    var r = await svc.PaymentGuaranteeTransitionAsync(guaranteeNo, action, dto);
    return r is null ? Results.NotFound(new { guaranteeNo, error = "Không thấy chứng thư bảo lãnh hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantees/{guaranteeNo}/lines/{vin}/cancel", async (string guaranteeNo, string vin, CancelGuaranteeLineDto? dto, IVehicleService svc) =>
{
    var r = await svc.CancelPaymentGuaranteeLineAsync(guaranteeNo, vin, dto?.Reason);
    return r is null ? Results.NotFound(new { guaranteeNo, vin, error = "Không tìm thấy dòng xe trong chứng thư bảo lãnh." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/guarantees/{guaranteeNo}", async (string guaranteeNo, UpdatePaymentGuaranteeDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdatePaymentGuaranteeAsync(guaranteeNo, dto);
    return r is null ? Results.NotFound(new { guaranteeNo, error = "Không thấy chứng thư bảo lãnh hoặc bảo lãnh đã đóng." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Hợp đồng mua bán xe ô tô giữa Hãng OEM và Đại lý phân phối (BizHTC.Contract.DealerContract / CT_DealerContract) ----
app.MapPost("/api/contracts", async (CreateDealerContractDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode." });
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe Items trong hợp đồng mua bán." });
    try { return Results.Ok(await svc.CreateDealerContractAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/contracts", async (IVehicleService svc, string? status, string? dealer, string? contractNo, string? soCode, string? vin) =>
    Results.Ok(await svc.ListDealerContractsAsync(status, dealer, contractNo, soCode, vin))).RequireAuthorization();

app.MapGet("/api/contracts/{contractNo}", async (string contractNo, IVehicleService svc) =>
{
    var r = await svc.GetDealerContractAsync(contractNo);
    return r is null ? Results.NotFound(new { contractNo, error = "Không tìm thấy hợp đồng mua bán xe." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/contracts/{contractNo}/{action}", async (string contractNo, string action, DealerContractTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|complete|reject|cancel" });
    var r = await svc.DealerContractTransitionAsync(contractNo, action, dto);
    return r is null ? Results.NotFound(new { contractNo, error = "Không thấy hợp đồng mua bán hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/contracts/{contractNo}/lines/{vin}/update", async (string contractNo, string vin, UpdateDealerContractLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateDealerContractLineAsync(contractNo, vin, dto);
    return r is null ? Results.NotFound(new { contractNo, vin, error = "Không tìm thấy dòng xe trong hợp đồng hoặc hợp đồng đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu & Quyết toán Chiết khấu thanh toán mua xe ô tô cho Đại lý (BizHTC.PaymentDiscount / Req_PaymentDiscount) ----
app.MapPost("/api/payment-discounts", async (CreatePaymentDiscountDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode." });
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe Items yêu cầu chiết khấu thanh toán." });
    try { return Results.Ok(await svc.CreatePaymentDiscountAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/payment-discounts", async (IVehicleService svc, string? status, string? dealer, string? paymentDiscountNo, string? vin) =>
    Results.Ok(await svc.ListPaymentDiscountsAsync(status, dealer, paymentDiscountNo, vin))).RequireAuthorization();

app.MapGet("/api/payment-discounts/{paymentDiscountNo}", async (string paymentDiscountNo, IVehicleService svc) =>
{
    var r = await svc.GetPaymentDiscountAsync(paymentDiscountNo);
    return r is null ? Results.NotFound(new { paymentDiscountNo, error = "Không tìm thấy đề nghị chiết khấu thanh toán." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/payment-discounts/{paymentDiscountNo}/{action}", async (string paymentDiscountNo, string action, PaymentDiscountTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "dlr-sign" or "dlrsign" or "sign-dlr" or "htc-sign" or "htcsign" or "sign-htc" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|dlr-sign|htc-sign|reject|cancel" });
    var r = await svc.PaymentDiscountTransitionAsync(paymentDiscountNo, action, dto);
    return r is null ? Results.NotFound(new { paymentDiscountNo, error = "Không thấy đề nghị chiết khấu hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/payment-discounts/{paymentDiscountNo}/lines/{vin}/update", async (string paymentDiscountNo, string vin, UpdatePaymentDiscountLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdatePaymentDiscountLineAsync(paymentDiscountNo, vin, dto);
    return r is null ? Results.NotFound(new { paymentDiscountNo, vin, error = "Không tìm thấy dòng xe trong đề nghị chiết khấu hoặc phiếu đã ký/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu & Quản lý Bảo hiểm lô xe vận chuyển & lưu kho (BizHTC.WH.Ins_InsuranceReq / Ins_InsuranceReq) ----
app.MapPost("/api/insurance-requests", async (CreateInsuranceRequestDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.InsCompanyCode))
        return Results.BadRequest(new { error = "Cần mã hãng bảo hiểm InsCompanyCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong yêu cầu bảo hiểm." });
    try { return Results.Ok(await svc.CreateInsuranceRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/insurance-requests", async (IVehicleService svc, string? status, string? insCompanyCode, string? insTypeCode, string? vin) =>
    Results.Ok(await svc.ListInsuranceRequestsAsync(status, insCompanyCode, insTypeCode, vin))).RequireAuthorization();

app.MapGet("/api/insurance-requests/{insReqNo}", async (string insReqNo, IVehicleService svc) =>
{
    var r = await svc.GetInsuranceRequestAsync(insReqNo);
    return r is null ? Results.NotFound(new { insReqNo, error = "Không tìm thấy yêu cầu bảo hiểm." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/insurance-requests/{insReqNo}/{action}", async (string insReqNo, string action, InsuranceRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|complete|reject|cancel" });
    var r = await svc.InsuranceRequestTransitionAsync(insReqNo, action, dto);
    return r is null ? Results.NotFound(new { insReqNo, error = "Không thấy yêu cầu bảo hiểm hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/insurance-requests/{insReqNo}/lines/{vin}/update", async (string insReqNo, string vin, UpdateInsuranceRequestLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateInsuranceRequestLineAsync(insReqNo, vin, dto);
    return r is null ? Results.NotFound(new { insReqNo, vin, error = "Không tìm thấy dòng xe trong yêu cầu bảo hiểm hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/insurance-requests/{insReqNo}/lines", async (string insReqNo, List<InsuranceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào yêu cầu bảo hiểm." });
    var r = await svc.AddInsuranceRequestLinesAsync(insReqNo, items);
    return r is null ? Results.NotFound(new { insReqNo, error = "Không tìm thấy yêu cầu bảo hiểm hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapDelete("/api/insurance-requests/{insReqNo}/lines/{vin}", async (string insReqNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveInsuranceRequestLineAsync(insReqNo, vin);
    return r is null ? Results.NotFound(new { insReqNo, vin, error = "Không tìm thấy dòng xe trong yêu cầu bảo hiểm hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Biên bản giao nhận & nghiệm thu vận chuyển xe ô tô (BizHTC.Car.Car_TransportMinutes / TransportMinutes) ----
app.MapPost("/api/transport-minutes", async (CreateTransportMinutesDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.TransporterCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và nhà xe TransporterCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong biên bản vận chuyển." });
    try { return Results.Ok(await svc.CreateTransportMinutesAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/transport-minutes", async (IVehicleService svc, string? status, string? dealer, string? transporter, string? vin) =>
    Results.Ok(await svc.ListTransportMinutesAsync(status, dealer, transporter, vin))).RequireAuthorization();

app.MapGet("/api/transport-minutes/{transportMinutesNo}", async (string transportMinutesNo, IVehicleService svc) =>
{
    var r = await svc.GetTransportMinutesAsync(transportMinutesNo);
    return r is null ? Results.NotFound(new { transportMinutesNo, error = "Không tìm thấy biên bản vận chuyển." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/transport-minutes/{transportMinutesNo}/{action}", async (string transportMinutesNo, string action, TransportMinutesTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "dl-appr" or "dlappr" or "sign-dlr" or "htc-appr1" or "htcappr1" or "logistics-appr" or "htc-appr2" or "htcappr2" or "approve" or "complete" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|dl-appr|htc-appr1|approve|reject|cancel" });
    var r = await svc.TransportMinutesTransitionAsync(transportMinutesNo, action, dto);
    return r is null ? Results.NotFound(new { transportMinutesNo, error = "Không thấy biên bản vận chuyển hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/transport-minutes/{transportMinutesNo}/lines/{vin}/update", async (string transportMinutesNo, string vin, UpdateTransportMinutesLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateTransportMinutesLineAsync(transportMinutesNo, vin, dto);
    return r is null ? Results.NotFound(new { transportMinutesNo, vin, error = "Không tìm thấy dòng xe trong biên bản vận chuyển hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/transport-minutes/{transportMinutesNo}/lines", async (string transportMinutesNo, List<TransportMinutesItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào biên bản vận chuyển." });
    var r = await svc.AddTransportMinutesLinesAsync(transportMinutesNo, items);
    return r is null ? Results.NotFound(new { transportMinutesNo, error = "Không tìm thấy biên bản vận chuyển hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapDelete("/api/transport-minutes/{transportMinutesNo}/lines/{vin}", async (string transportMinutesNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveTransportMinutesLineAsync(transportMinutesNo, vin);
    return r is null ? Results.NotFound(new { transportMinutesNo, vin, error = "Không tìm thấy dòng xe trong biên bản vận chuyển hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Chứng từ / Phiếu thanh toán tiền mua xe ô tô cho Đại lý (BizHTC.Payment.Pmt_Payment / DealerPayment) ----
app.MapPost("/api/payments", async (CreateDealerPaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode nộp tiền mua xe." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong phiếu thanh toán." });
    try { return Results.Ok(await svc.CreateDealerPaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/payments", async (IVehicleService svc, string? status, string? dealer, string? paymentType, string? paymentNo, string? vin) =>
    Results.Ok(await svc.ListDealerPaymentsAsync(status, dealer, paymentType, paymentNo, vin))).RequireAuthorization();

app.MapGet("/api/payments/{paymentNo}", async (string paymentNo, IVehicleService svc) =>
{
    var r = await svc.GetDealerPaymentAsync(paymentNo);
    return r is null ? Results.NotFound(new { paymentNo, error = "Không tìm thấy phiếu thanh toán tiền xe." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/payments/{paymentNo}/{action}", async (string paymentNo, string action, DealerPaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "confirm" or "complete" or "finish" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|confirm|reject|cancel" });
    var r = await svc.DealerPaymentTransitionAsync(paymentNo, action, dto);
    return r is null ? Results.NotFound(new { paymentNo, error = "Không thấy phiếu thanh toán hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/payments/{paymentNo}/lines/{vin}/update", async (string paymentNo, string vin, UpdateDealerPaymentLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateDealerPaymentLineAsync(paymentNo, vin, dto);
    return r is null ? Results.NotFound(new { paymentNo, vin, error = "Không tìm thấy dòng xe trong phiếu thanh toán hoặc chứng từ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/payments/{paymentNo}/lines", async (string paymentNo, List<DealerPaymentItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào phiếu thanh toán." });
    var r = await svc.AddDealerPaymentLinesAsync(paymentNo, items);
    return r is null ? Results.NotFound(new { paymentNo, error = "Không tìm thấy phiếu thanh toán hoặc chứng từ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapDelete("/api/payments/{paymentNo}/lines/{vin}", async (string paymentNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveDealerPaymentLineAsync(paymentNo, vin);
    return r is null ? Results.NotFound(new { paymentNo, vin, error = "Không tìm thấy dòng xe trong phiếu thanh toán hoặc chứng từ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Bảo dưỡng định kỳ xe tồn kho OEM (BizHTC.StorageFG.VIN_MaintainPeriod & StoF_Maintain / StorageMaintenance) ----
app.MapPost("/api/storage-maintenances", async (CreateStorageMaintenanceDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.StorageCode))
        return Results.BadRequest(new { error = "Cần mã kho/bãi bốc xếp StorageCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong phiếu bảo dưỡng kho." });
    try { return Results.Ok(await svc.CreateStorageMaintenanceAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/storage-maintenances", async (IVehicleService svc, string? status, string? storageCode, string? mtnType, string? mtnNo, string? vin) =>
    Results.Ok(await svc.ListStorageMaintenancesAsync(status, storageCode, mtnType, mtnNo, vin))).RequireAuthorization();

app.MapGet("/api/storage-maintenances/due-vehicles", async (IVehicleService svc, string? storageCode, int? dueWithinDays) =>
    Results.Ok(await svc.GetDueMaintenanceVehiclesAsync(storageCode, dueWithinDays ?? 7))).RequireAuthorization();

app.MapGet("/api/storage-maintenances/{mtnNo}", async (string mtnNo, IVehicleService svc) =>
{
    var r = await svc.GetStorageMaintenanceAsync(mtnNo);
    return r is null ? Results.NotFound(new { mtnNo, error = "Không tìm thấy phiếu bảo dưỡng kho." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/storage-maintenances/{mtnNo}/{action}", async (string mtnNo, string action, StorageMaintenanceTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve" or "start" or "in-progress" or "inprogress" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|start|complete|reject|cancel" });
    var r = await svc.StorageMaintenanceTransitionAsync(mtnNo, action, dto);
    return r is null ? Results.NotFound(new { mtnNo, error = "Không thấy phiếu bảo dưỡng hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/storage-maintenances/{mtnNo}/lines/{vin}/inspect", async (string mtnNo, string vin, InspectStorageMaintenanceLineDto dto, IVehicleService svc) =>
{
    var r = await svc.InspectStorageMaintenanceLineAsync(mtnNo, vin, dto);
    return r is null ? Results.NotFound(new { mtnNo, vin, error = "Không tìm thấy dòng xe trong phiếu bảo dưỡng hoặc phiếu đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/storage-maintenances/{mtnNo}/lines/{vin}/update", async (string mtnNo, string vin, UpdateStorageMaintenanceLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateStorageMaintenanceLineAsync(mtnNo, vin, dto);
    return r is null ? Results.NotFound(new { mtnNo, vin, error = "Không tìm thấy dòng xe trong phiếu bảo dưỡng hoặc phiếu đã hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/storage-maintenances/{mtnNo}/lines", async (string mtnNo, List<StorageMaintenanceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào phiếu bảo dưỡng kho." });
    var r = await svc.AddStorageMaintenanceLinesAsync(mtnNo, items);
    return r is null ? Results.NotFound(new { mtnNo, error = "Không tìm thấy phiếu bảo dưỡng hoặc phiếu đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapDelete("/api/storage-maintenances/{mtnNo}/lines/{vin}", async (string mtnNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveStorageMaintenanceLineAsync(mtnNo, vin);
    return r is null ? Results.NotFound(new { mtnNo, vin, error = "Không tìm thấy dòng xe trong phiếu bảo dưỡng hoặc phiếu đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/maintenance-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleMaintenanceHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Packing List xuất xưởng nhà máy & Vận đơn nhập khẩu CBU/CKD (BizHTC.Contract.ContractPackingList / CT_PackingList) ----
app.MapPost("/api/packing-lists", async (CreatePackingListDto dto, IVehicleService svc) =>
{
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong Packing List." });
    try { return Results.Ok(await svc.CreatePackingListAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/packing-lists", async (IVehicleService svc, string? status, string? portCode, string? contractNo, string? vesselName, string? vin) =>
    Results.Ok(await svc.ListPackingListsAsync(status, portCode, contractNo, vesselName, vin))).RequireAuthorization();

app.MapGet("/api/packing-lists/{packingListNo}", async (string packingListNo, IVehicleService svc) =>
{
    var r = await svc.GetPackingListAsync(packingListNo);
    return r is null ? Results.NotFound(new { packingListNo, error = "Không tìm thấy Packing List." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/packing-lists/{packingListNo}/{action}", async (string packingListNo, string action, PackingListTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve" or "confirm" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|reject|cancel" });
    var r = await svc.PackingListTransitionAsync(packingListNo, action, dto);
    return r is null ? Results.NotFound(new { packingListNo, error = "Không thấy Packing List hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/packing-lists/{packingListNo}/lines/{vin}/update", async (string packingListNo, string vin, UpdatePackingListLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdatePackingListLineAsync(packingListNo, vin, dto);
    return r is null ? Results.NotFound(new { packingListNo, vin, error = "Không tìm thấy dòng xe trong Packing List hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/packing-lists/{packingListNo}/lines", async (string packingListNo, List<PackingListItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào Packing List." });
    try
    {
        var r = await svc.AddPackingListLinesAsync(packingListNo, items);
        return r is null ? Results.NotFound(new { packingListNo, error = "Không tìm thấy Packing List hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/packing-lists/{packingListNo}/lines/{vin}", async (string packingListNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemovePackingListLineAsync(packingListNo, vin);
    return r is null ? Results.NotFound(new { packingListNo, vin, error = "Không tìm thấy dòng xe trong Packing List hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Tờ khai Hải quan nhập khẩu CBU/CKD & Nộp thuế thông quan xe (BizHTC.Contract.ContractDeclaration & CT_TKHQ / CT_Declaration) ----
app.MapPost("/api/customs-declarations", async (CreateCustomsDeclarationDto dto, IVehicleService svc) =>
{
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong Tờ khai hải quan." });
    try { return Results.Ok(await svc.CreateCustomsDeclarationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/customs-declarations", async (IVehicleService svc, string? status, string? portCode, string? contractNo, string? declarationType, string? declarationNo, string? vin) =>
    Results.Ok(await svc.ListCustomsDeclarationsAsync(status, portCode, contractNo, declarationType, declarationNo, vin))).RequireAuthorization();

app.MapGet("/api/customs-declarations/{declarationNo}", async (string declarationNo, IVehicleService svc) =>
{
    var r = await svc.GetCustomsDeclarationAsync(declarationNo);
    return r is null ? Results.NotFound(new { declarationNo, error = "Không tìm thấy Tờ khai hải quan." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/customs-declarations/{declarationNo}/{action}", async (string declarationNo, string action, CustomsDeclarationTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "register" or "pay-tax" or "paytax" or "clear" or "clearance" or "approve" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|pay-tax|clear|reject|cancel" });
    var r = await svc.CustomsDeclarationTransitionAsync(declarationNo, action, dto);
    return r is null ? Results.NotFound(new { declarationNo, error = "Không thấy Tờ khai hải quan hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/customs-declarations/{declarationNo}/lines/{vin}/update", async (string declarationNo, string vin, UpdateCustomsDeclarationLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateCustomsDeclarationLineAsync(declarationNo, vin, dto);
    return r is null ? Results.NotFound(new { declarationNo, vin, error = "Không tìm thấy dòng xe trong Tờ khai hải quan hoặc tờ khai đã thông quan/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/customs-declarations/{declarationNo}/lines", async (string declarationNo, List<CustomsDeclarationItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào Tờ khai hải quan." });
    try
    {
        var r = await svc.AddCustomsDeclarationLinesAsync(declarationNo, items);
        return r is null ? Results.NotFound(new { declarationNo, error = "Không tìm thấy Tờ khai hải quan hoặc tờ khai đã thông quan/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/customs-declarations/{declarationNo}/lines/{vin}", async (string declarationNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveCustomsDeclarationLineAsync(declarationNo, vin);
    return r is null ? Results.NotFound(new { declarationNo, vin, error = "Không tìm thấy dòng xe trong Tờ khai hải quan hoặc tờ khai đã thông quan/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/customs-declarations/{declarationNo}/update-tax-payment", async (string declarationNo, UpdateCustomsDeclarationTaxPaymentDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateCustomsDeclarationTaxPaymentAsync(declarationNo, dto);
    return r is null ? Results.NotFound(new { declarationNo, error = "Không tìm thấy Tờ khai hải quan hoặc tờ khai đã bị hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu & Nghiệm thu đóng thùng xe thương mại / xe tải (BizHTC.Storage.Sto_CBReq / CarBoxRequest) ----
app.MapPost("/api/car-box-requests", async (CreateCarBoxRequestDto dto, IVehicleService svc) =>
{
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong yêu cầu đóng thùng." });
    try { return Results.Ok(await svc.CreateCarBoxRequestAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/car-box-requests", async (IVehicleService svc, string? status, string? dealer, string? loaiThung, string? bodyBuilder, string? vin) =>
    Results.Ok(await svc.ListCarBoxRequestsAsync(status, dealer, loaiThung, bodyBuilder, vin))).RequireAuthorization();

app.MapGet("/api/car-box-requests/{cbReqNo}", async (string cbReqNo, IVehicleService svc) =>
{
    var r = await svc.GetCarBoxRequestAsync(cbReqNo);
    return r is null ? Results.NotFound(new { cbReqNo, error = "Không tìm thấy yêu cầu đóng thùng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-box-requests/{cbReqNo}/{action}", async (string cbReqNo, string action, CarBoxRequestTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "start" or "in-progress" or "inprogress" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|start|complete|reject|cancel" });
    var r = await svc.CarBoxRequestTransitionAsync(cbReqNo, action, dto);
    return r is null ? Results.NotFound(new { cbReqNo, error = "Không thấy yêu cầu đóng thùng hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-box-requests/{cbReqNo}/lines/{vin}/inspect", async (string cbReqNo, string vin, InspectCarBoxLineDto dto, IVehicleService svc) =>
{
    var r = await svc.InspectCarBoxLineAsync(cbReqNo, vin, dto);
    return r is null ? Results.NotFound(new { cbReqNo, vin, error = "Không tìm thấy dòng xe trong yêu cầu đóng thùng hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-box-requests/{cbReqNo}/lines/{vin}/update", async (string cbReqNo, string vin, UpdateCarBoxRequestLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateCarBoxRequestLineAsync(cbReqNo, vin, dto);
    return r is null ? Results.NotFound(new { cbReqNo, vin, error = "Không tìm thấy dòng xe trong yêu cầu đóng thùng hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-box-requests/{cbReqNo}/lines", async (string cbReqNo, List<CarBoxItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào yêu cầu đóng thùng." });
    try
    {
        var r = await svc.AddCarBoxRequestLinesAsync(cbReqNo, items);
        return r is null ? Results.NotFound(new { cbReqNo, error = "Không tìm thấy yêu cầu đóng thùng hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/car-box-requests/{cbReqNo}/lines/{vin}", async (string cbReqNo, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveCarBoxRequestLineAsync(cbReqNo, vin);
    return r is null ? Results.NotFound(new { cbReqNo, vin, error = "Không tìm thấy dòng xe trong yêu cầu đóng thùng hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Bảng kê / Đợt xuất hóa đơn GTGT xe ô tô cho Đại lý (BizHTC.Car.Car_InvoiceList / CarInvoice) ----
app.MapPost("/api/car-invoices", async (CreateCarInvoiceDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode nhận hóa đơn." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong bảng kê hóa đơn." });
    try { return Results.Ok(await svc.CreateCarInvoiceAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/car-invoices", async (IVehicleService svc, string? status, string? dealer, string? invoiceListCode, string? invoiceNo, string? vin) =>
    Results.Ok(await svc.ListCarInvoicesAsync(status, dealer, invoiceListCode, invoiceNo, vin))).RequireAuthorization();

app.MapGet("/api/car-invoices/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetCarInvoiceAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy bảng kê hóa đơn." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-invoices/{code}/{action}", async (string code, string action, CarInvoiceTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("issue" or "approve" or "cancel"))
        return Results.BadRequest(new { error = "action = issue|cancel" });
    var r = await svc.CarInvoiceTransitionAsync(code, action, dto);
    return r is null ? Results.NotFound(new { code, error = "Không thấy bảng kê hóa đơn hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-invoices/{code}/lines/{vin}/update", async (string code, string vin, UpdateCarInvoiceLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateCarInvoiceLineAsync(code, vin, dto);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong bảng kê hóa đơn hoặc hóa đơn đã phát hành/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/car-invoices/{code}/lines", async (string code, List<CarInvoiceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bảng kê hóa đơn." });
    try
    {
        var r = await svc.AddCarInvoiceLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy bảng kê hóa đơn hoặc hóa đơn đã phát hành/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/car-invoices/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveCarInvoiceLineAsync(code, vin);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong bảng kê hóa đơn hoặc hóa đơn đã phát hành/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/invoice-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleInvoiceInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị & Quyết định gia hạn bảo lãnh thanh toán ngân hàng cho Đại lý (BizHTC.PaymentGrtExt / Pmt_GrtClaimExt) ----
app.MapPost("/api/guarantee-extensions", async (CreateGuaranteeExtensionDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode đề nghị gia hạn bảo lãnh." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong đề nghị gia hạn bảo lãnh." });
    try { return Results.Ok(await svc.CreateGuaranteeExtensionAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/guarantee-extensions", async (IVehicleService svc, string? status, string? dealer, string? bank, string? guaranteeNo, string? grtClaimExtNo, string? vin) =>
    Results.Ok(await svc.ListGuaranteeExtensionsAsync(status, dealer, bank, guaranteeNo, grtClaimExtNo, vin))).RequireAuthorization();

app.MapGet("/api/guarantee-extensions/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetGuaranteeExtensionAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị gia hạn bảo lãnh." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantee-extensions/{code}/{action}", async (string code, string action, GuaranteeExtensionTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "sign" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|sign|complete|reject|cancel" });
    var r = await svc.GuaranteeExtensionTransitionAsync(code, action, dto);
    return r is null ? Results.NotFound(new { code, error = "Không thấy đề nghị gia hạn bảo lãnh hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantee-extensions/{code}/lines/{vin}/update", async (string code, string vin, UpdateGuaranteeExtensionLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateGuaranteeExtensionLineAsync(code, vin, dto);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị gia hạn hoặc hồ sơ đã hoàn tất/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantee-extensions/{code}/lines", async (string code, List<GuaranteeExtensionItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào đề nghị gia hạn." });
    try
    {
        var r = await svc.AddGuaranteeExtensionLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị gia hạn hoặc hồ sơ đã hoàn tất/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/guarantee-extensions/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveGuaranteeExtensionLineAsync(code, vin);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị gia hạn hoặc hồ sơ đã hoàn tất/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/guarantee-extension-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGuaranteeExtensionInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị & Quyết định Hủy / Rút dòng xe Hợp đồng mua bán xe Đại lý (BizHTC.Contract.Dlr_ContractCancel / ContractCancel) ----
app.MapPost("/api/contract-cancels", async (CreateContractCancelDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode đề nghị hủy hợp đồng." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong đề nghị hủy hợp đồng." });
    try { return Results.Ok(await svc.CreateContractCancelAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/contract-cancels", async (IVehicleService svc, string? status, string? dealer, string? dlrContractNo, string? contractCNo, string? vin) =>
    Results.Ok(await svc.ListContractCancelsAsync(status, dealer, dlrContractNo, contractCNo, vin))).RequireAuthorization();

app.MapGet("/api/contract-cancels/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetContractCancelAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị hủy hợp đồng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/contract-cancels/{code}/{action}", async (string code, string action, ContractCancelTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|reject|cancel" });
    var r = await svc.ContractCancelTransitionAsync(code, action, dto);
    return r is null ? Results.NotFound(new { code, error = "Không thấy đề nghị hủy hợp đồng hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/contract-cancels/{code}/lines/{vin}/update", async (string code, string vin, UpdateContractCancelLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateContractCancelLineAsync(code, vin, dto);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị hủy hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/contract-cancels/{code}/lines", async (string code, List<ContractCancelItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào đề nghị hủy hợp đồng." });
    try
    {
        var r = await svc.AddContractCancelLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị hủy hợp đồng hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/contract-cancels/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveContractCancelLineAsync(code, vin);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị hủy hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/contract-cancel-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleContractCancelInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị & Quản lý Thay đổi màu sơn xe ô tô (BizHTC.WH & BizHTC.Car.Car_ColorChange / CarColorChange) ----
app.MapPost("/api/color-changes", async (CreateCarColorChangeDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode đề nghị đổi màu xe." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong đề nghị đổi màu xe." });
    try { return Results.Ok(await svc.CreateCarColorChangeAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/color-changes", async (IVehicleService svc, string? status, string? dealer, string? changeNo, string? vin) =>
    Results.Ok(await svc.ListCarColorChangesAsync(status, dealer, changeNo, vin))).RequireAuthorization();

app.MapGet("/api/color-changes/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetCarColorChangeAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị đổi màu xe." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/color-changes/{code}/{action}", async (string code, string action, CarColorChangeTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|reject|cancel" });
    var r = await svc.CarColorChangeTransitionAsync(code, action, dto);
    return r is null ? Results.NotFound(new { code, error = "Không thấy đề nghị đổi màu xe hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/color-changes/{code}/lines/{vin}/update", async (string code, string vin, UpdateCarColorChangeLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCarColorChangeLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị đổi màu hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/color-changes/{code}/lines", async (string code, List<CarColorChangeItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào đề nghị đổi màu." });
    try
    {
        var r = await svc.AddCarColorChangeLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy đề nghị đổi màu hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/color-changes/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveCarColorChangeLineAsync(code, vin);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong đề nghị đổi màu hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/color-changes/history/{vin}", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleColorChangeHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/color-changes/vehicle-info/{vin}", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleColorChangeInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Biên bản bàn giao hóa đơn & hồ sơ chứng từ xe cho Ngân hàng (BizHTC.Car.Car_BankBillMinutes / BankBillMinutes) ----
app.MapPost("/api/bank-bill-minutes", async (CreateBankBillMinutesDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.BankCode) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã ngân hàng BankCode và đại lý DealerCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong biên bản bàn giao hồ sơ ngân hàng." });
    try { return Results.Ok(await svc.CreateBankBillMinutesAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/bank-bill-minutes", async (IVehicleService svc, string? status, string? bank, string? dealer, string? guaranteeNo, string? bankBillMnNo, string? vin) =>
    Results.Ok(await svc.ListBankBillMinutesAsync(status, bank, dealer, guaranteeNo, bankBillMnNo, vin))).RequireAuthorization();

app.MapGet("/api/bank-bill-minutes/pending-vehicles", async (IVehicleService svc, string? bank, string? dealer) =>
    Results.Ok(await svc.GetPendingVehiclesForBankBillAsync(bank, dealer))).RequireAuthorization();

app.MapGet("/api/bank-bill-minutes/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetBankBillMinutesAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy biên bản bàn giao hồ sơ ngân hàng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/bank-bill-minutes/{code}/{action}", async (string code, string action, BankBillMinutesTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "handover" or "complete" or "sign" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|handover|reject|cancel" });
    var r = await svc.BankBillMinutesTransitionAsync(code, action, dto);
    return r is null ? Results.NotFound(new { code, error = "Không thấy biên bản bàn giao hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/bank-bill-minutes/{code}/lines/{vin}/update", async (string code, string vin, UpdateBankBillMinutesLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateBankBillMinutesLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong biên bản hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/bank-bill-minutes/{code}/lines", async (string code, List<BankBillMinutesItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào biên bản bàn giao." });
    try
    {
        var r = await svc.AddBankBillMinutesLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy biên bản bàn giao hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/bank-bill-minutes/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    var r = await svc.RemoveBankBillMinutesLineAsync(code, vin);
    return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong biên bản hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/bank-bill-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleBankBillInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Yêu cầu & Hồ sơ Đòi tiền / Khiếu nại bảo lãnh thanh toán ngân hàng (BizHTC.Payment.Pmt_GrtClaim / GuaranteeClaim) ----
app.MapPost("/api/guarantee-claims", async (CreateGuaranteeClaimDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.BankCode) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã ngân hàng BankCode và đại lý DealerCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong hồ sơ đòi tiền bảo lãnh." });
    try { return Results.Ok(await svc.CreateGuaranteeClaimAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/guarantee-claims", async (IVehicleService svc, string? status, string? bank, string? dealer, string? guaranteeNo, string? claimNo, string? vin) =>
    Results.Ok(await svc.ListGuaranteeClaimsAsync(status, bank, dealer, guaranteeNo, claimNo, vin))).RequireAuthorization();

app.MapGet("/api/guarantee-claims/overdue-vehicles", async (IVehicleService svc, string? bank, string? dealer, int? overdueDays) =>
    Results.Ok(await svc.GetOverdueGuaranteedVehiclesAsync(bank, dealer, overdueDays))).RequireAuthorization();

app.MapGet("/api/guarantee-claims/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetGuaranteeClaimAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ đòi tiền bảo lãnh." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/guarantee-claims/{code}/{action}", async (string code, string action, GuaranteeClaimTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "claim" or "settle" or "disburse" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|claim|settle|disburse|reject|cancel" });
    try
    {
        var r = await svc.GuaranteeClaimTransitionAsync(code, action, dto);
        return r is null ? Results.NotFound(new { code, error = "Không thấy hồ sơ đòi bảo lãnh hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/guarantee-claims/{code}/lines/{vin}/update", async (string code, string vin, UpdateGuaranteeClaimLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGuaranteeClaimLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong hồ sơ hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/guarantee-claims/{code}/lines", async (string code, List<GuaranteeClaimItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào hồ sơ đòi bảo lãnh." });
    try
    {
        var r = await svc.AddGuaranteeClaimLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ đòi bảo lãnh hoặc hồ sơ đã chốt/hủy/xe đã tồn tại." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/guarantee-claims/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGuaranteeClaimLineAsync(code, vin);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong hồ sơ hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/guarantee-claim-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGuaranteeClaimInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Hợp đồng mua bán xe / bộ linh kiện ngoại thương CBU/CKD (BizHTC.Contract.ContractOversea / CT_ContractOversea) ----
app.MapPost("/api/contract-overseas", async (CreateContractOverseaDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.SupplierCode))
        return Results.BadRequest(new { error = "Cần mã nhà cung cấp / đối tác quốc tế SupplierCode." });
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe/linh kiện Items trong hợp đồng ngoại thương." });
    try { return Results.Ok(await svc.CreateContractOverseaAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/contract-overseas", async (IVehicleService svc, string? status, string? supplier, string? incoterms, string? currency, string? orderMonth, string? contractNo, string? vin) =>
    Results.Ok(await svc.ListContractOverseasAsync(status, supplier, incoterms, currency, orderMonth, contractNo, vin))).RequireAuthorization();

app.MapGet("/api/contract-overseas/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetContractOverseaSummaryAsync())).RequireAuthorization();

app.MapGet("/api/contract-overseas/{contractNo}", async (string contractNo, IVehicleService svc) =>
{
    var r = await svc.GetContractOverseaAsync(contractNo);
    return r is null ? Results.NotFound(new { contractNo, error = "Không tìm thấy hợp đồng ngoại thương." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/contract-overseas/{contractNo}", async (string contractNo, UpdateContractOverseaHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateContractOverseaHeaderAsync(contractNo, dto);
        return r is null ? Results.NotFound(new { contractNo, error = "Không tìm thấy hợp đồng ngoại thương hoặc hợp đồng đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/contract-overseas/{contractNo}/{action}", async (string contractNo, string action, ContractOverseaTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "execute" or "in-execution" or "inexecution" or "start" or "inprogress" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|execute|complete|reject|cancel" });
    try
    {
        var r = await svc.ContractOverseaTransitionAsync(contractNo, action, dto);
        return r is null ? Results.NotFound(new { contractNo, error = "Không thấy hợp đồng ngoại thương hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/contract-overseas/{contractNo}/lines/{lineId:long}/update", async (string contractNo, long lineId, UpdateContractOverseaLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateContractOverseaLineAsync(contractNo, lineId, dto);
        return r is null ? Results.NotFound(new { contractNo, lineId, error = "Không tìm thấy dòng xe trong hợp đồng hoặc hợp đồng đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/contract-overseas/{contractNo}/lines/{lineId:long}", async (string contractNo, long lineId, UpdateContractOverseaLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateContractOverseaLineAsync(contractNo, lineId, dto);
        return r is null ? Results.NotFound(new { contractNo, lineId, error = "Không tìm thấy dòng xe trong hợp đồng hoặc hợp đồng đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/contract-overseas/{contractNo}/lines", async (string contractNo, List<ContractOverseaItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào hợp đồng ngoại thương." });
    try
    {
        var r = await svc.AddContractOverseaLinesAsync(contractNo, items);
        return r is null ? Results.NotFound(new { contractNo, error = "Không tìm thấy hợp đồng ngoại thương hoặc hợp đồng đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/contract-overseas/{contractNo}/lines/{lineId:long}", async (string contractNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveContractOverseaLineAsync(contractNo, lineId);
        return r is null ? Results.NotFound(new { contractNo, lineId, error = "Không tìm thấy dòng xe trong hợp đồng hoặc hợp đồng đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/contract-oversea-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleContractOverseaInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Thư tín dụng L/C thanh toán quốc tế nhập khẩu xe CBU/CKD (BizHTC.Contract.ContractLC / CT_LC) ----
app.MapPost("/api/letters-of-credit", async (CreateLetterOfCreditDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.ContractNo) || string.IsNullOrWhiteSpace(dto.BankCode))
        return Results.BadRequest(new { error = "Cần mã hợp đồng ngoại thương ContractNo và mã ngân hàng BankCode." });
    try { return Results.Ok(await svc.CreateLetterOfCreditAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/letters-of-credit", async (IVehicleService svc, string? status, string? bank, string? contractNo, string? currency, string? paymentTerm, string? lcNo, string? vin) =>
    Results.Ok(await svc.ListLettersOfCreditAsync(status, bank, contractNo, currency, paymentTerm, lcNo, vin))).RequireAuthorization();

app.MapGet("/api/letters-of-credit/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetLetterOfCreditSummaryAsync())).RequireAuthorization();

app.MapGet("/api/letters-of-credit/{lcNo}", async (string lcNo, IVehicleService svc) =>
{
    var r = await svc.GetLetterOfCreditAsync(lcNo);
    return r is null ? Results.NotFound(new { lcNo, error = "Không tìm thấy thư tín dụng L/C." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/letters-of-credit/{lcNo}", async (string lcNo, UpdateLetterOfCreditHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateLetterOfCreditHeaderAsync(lcNo, dto);
        return r is null ? Results.NotFound(new { lcNo, error = "Không tìm thấy thư tín dụng L/C hoặc L/C đã tất toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/letters-of-credit/{lcNo}/{action}", async (string lcNo, string action, LetterOfCreditTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "issue" or "approve" or "utilize" or "pay" or "disburse" or "settle" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|issue|utilize|settle|reject|cancel" });
    try
    {
        var r = await svc.LetterOfCreditTransitionAsync(lcNo, action, dto);
        return r is null ? Results.NotFound(new { lcNo, error = "Không thấy thư tín dụng L/C hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/letters-of-credit/{lcNo}/lines/{lineId:long}/update", async (string lcNo, long lineId, UpdateLetterOfCreditLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateLetterOfCreditLineAsync(lcNo, lineId, dto);
        return r is null ? Results.NotFound(new { lcNo, lineId, error = "Không tìm thấy dòng xe trong L/C hoặc L/C đã tất toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/letters-of-credit/{lcNo}/lines/{lineId:long}", async (string lcNo, long lineId, UpdateLetterOfCreditLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateLetterOfCreditLineAsync(lcNo, lineId, dto);
        return r is null ? Results.NotFound(new { lcNo, lineId, error = "Không tìm thấy dòng xe trong L/C hoặc L/C đã tất toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/letters-of-credit/{lcNo}/lines", async (string lcNo, List<LetterOfCreditItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào L/C." });
    try
    {
        var r = await svc.AddLetterOfCreditLinesAsync(lcNo, items);
        return r is null ? Results.NotFound(new { lcNo, error = "Không tìm thấy thư tín dụng L/C hoặc L/C đã tất toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/letters-of-credit/{lcNo}/lines/{lineId:long}", async (string lcNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveLetterOfCreditLineAsync(lcNo, lineId);
        return r is null ? Results.NotFound(new { lcNo, lineId, error = "Không tìm thấy dòng xe trong L/C hoặc L/C đã tất toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/lc-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleLetterOfCreditInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Lệnh sửa chữa & Dịch vụ xưởng đại lý ủy quyền (BizHTC.Car / DMS.CarService / SerROService / Ser_RO) ----
app.MapPost("/api/repair-orders", async (CreateRepairOrderDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và số khung Vin để lập lệnh sửa chữa." });
    try { return Results.Ok(await svc.CreateRepairOrderAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/repair-orders", async (IVehicleService svc, string? status, string? dealer, string? roType, string? vin, string? plateNo, string? roNo) =>
    Results.Ok(await svc.ListRepairOrdersAsync(status, dealer, roType, vin, plateNo, roNo))).RequireAuthorization();

app.MapGet("/api/repair-orders/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetRepairOrderSummaryAsync())).RequireAuthorization();

app.MapGet("/api/repair-orders/{roNo}", async (string roNo, IVehicleService svc) =>
{
    var r = await svc.GetRepairOrderAsync(roNo);
    return r is null ? Results.NotFound(new { roNo, error = "Không tìm thấy lệnh sửa chữa." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/repair-orders/{roNo}", async (string roNo, UpdateRepairOrderHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateRepairOrderHeaderAsync(roNo, dto);
        return r is null ? Results.NotFound(new { roNo, error = "Không tìm thấy lệnh sửa chữa hoặc lệnh đã thanh toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/repair-orders/{roNo}/{action}", async (string roNo, string action, RepairOrderTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "start" or "in-garage" or "ingarage" or "in-progress" or "inprogress" or "repair" or "repaired" or "pass-qc" or "qc-pass" or "deliver" or "delivered" or "complete" or "finish" or "pay" or "paid" or "settle" or "cancel" or "reject"))
        return Results.BadRequest(new { error = "action = submit|start|repair|deliver|pay|cancel" });
    try
    {
        var r = await svc.RepairOrderTransitionAsync(roNo, action, dto);
        return r is null ? Results.NotFound(new { roNo, error = "Không thấy lệnh sửa chữa hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/repair-orders/{roNo}/service-lines/{lineId:long}/update", async (string roNo, long lineId, UpdateRepairOrderServiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateRepairOrderServiceLineAsync(roNo, lineId, dto);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/repair-orders/{roNo}/service-lines/{lineId:long}", async (string roNo, long lineId, UpdateRepairOrderServiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateRepairOrderServiceLineAsync(roNo, lineId, dto);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/repair-orders/{roNo}/service-lines", async (string roNo, List<RepairOrderServiceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items hạng mục dịch vụ để thêm vào lệnh sửa chữa." });
    try
    {
        var r = await svc.AddRepairOrderServiceLinesAsync(roNo, items);
        return r is null ? Results.NotFound(new { roNo, error = "Không tìm thấy lệnh sửa chữa hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/repair-orders/{roNo}/service-lines/{lineId:long}", async (string roNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveRepairOrderServiceLineAsync(roNo, lineId);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/repair-orders/{roNo}/part-lines/{lineId:long}/update", async (string roNo, long lineId, UpdateRepairOrderPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateRepairOrderPartLineAsync(roNo, lineId, dto);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/repair-orders/{roNo}/part-lines/{lineId:long}", async (string roNo, long lineId, UpdateRepairOrderPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateRepairOrderPartLineAsync(roNo, lineId, dto);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/repair-orders/{roNo}/part-lines", async (string roNo, List<RepairOrderPartItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items phụ tùng để thêm vào lệnh sửa chữa." });
    try
    {
        var r = await svc.AddRepairOrderPartLinesAsync(roNo, items);
        return r is null ? Results.NotFound(new { roNo, error = "Không tìm thấy lệnh sửa chữa hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/repair-orders/{roNo}/part-lines/{lineId:long}", async (string roNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveRepairOrderPartLineAsync(roNo, lineId);
        return r is null ? Results.NotFound(new { roNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lệnh đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/repair-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleRepairOrderHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/ro-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleRepairOrderHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Lịch hẹn Dịch vụ & Tiếp nhận xe xưởng (BizCarSv.Appointment / Ser_App) ----
app.MapPost("/api/service-appointments", async (CreateServiceAppointmentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và số khung Vin để đặt lịch hẹn." });
    try { return Results.Ok(await svc.CreateServiceAppointmentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/service-appointments", async (IVehicleService svc, string? status, string? dealer, string? serviceType, string? date, string? vin, string? plateNo, string? appNo) =>
    Results.Ok(await svc.ListServiceAppointmentsAsync(status, dealer, serviceType, date, vin, plateNo, appNo))).RequireAuthorization();

app.MapGet("/api/service-appointments/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetServiceAppointmentSummaryAsync())).RequireAuthorization();

app.MapGet("/api/service-appointments/{appNo}", async (string appNo, IVehicleService svc) =>
{
    var r = await svc.GetServiceAppointmentAsync(appNo);
    return r is null ? Results.NotFound(new { appNo, error = "Không tìm thấy lịch hẹn dịch vụ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/service-appointments/{appNo}", async (string appNo, UpdateServiceAppointmentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceAppointmentHeaderAsync(appNo, dto);
        return r is null ? Results.NotFound(new { appNo, error = "Không tìm thấy lịch hẹn hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/{action}", async (string appNo, string action, ServiceAppointmentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("confirm" or "checkin" or "check-in" or "arrived" or "inservice" or "in-service" or "start" or "complete" or "finish" or "noshow" or "no-show" or "cancel" or "reject"))
        return Results.BadRequest(new { error = "action = confirm|checkin|inservice|complete|noshow|cancel" });
    try
    {
        var r = await svc.ServiceAppointmentTransitionAsync(appNo, action, dto);
        return r is null ? Results.NotFound(new { appNo, error = "Không thấy lịch hẹn hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/create-ro", async (string appNo, CreateRoFromAppointmentDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CreateRoFromAppointmentAsync(appNo, dto);
        return r is null ? Results.NotFound(new { appNo, error = "Không tìm thấy lịch hẹn dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/service-lines/{lineId:long}/update", async (string appNo, long lineId, UpdateServiceAppointmentServiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceAppointmentServiceLineAsync(appNo, lineId, dto);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/service-appointments/{appNo}/service-lines/{lineId:long}", async (string appNo, long lineId, UpdateServiceAppointmentServiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceAppointmentServiceLineAsync(appNo, lineId, dto);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/service-lines", async (string appNo, List<ServiceAppointmentServiceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items hạng mục dịch vụ để thêm vào lịch hẹn." });
    try
    {
        var r = await svc.AddServiceAppointmentServiceLinesAsync(appNo, items);
        return r is null ? Results.NotFound(new { appNo, error = "Không tìm thấy lịch hẹn hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-appointments/{appNo}/service-lines/{lineId:long}", async (string appNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServiceAppointmentServiceLineAsync(appNo, lineId);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng dịch vụ hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/part-lines/{lineId:long}/update", async (string appNo, long lineId, UpdateServiceAppointmentPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceAppointmentPartLineAsync(appNo, lineId, dto);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/service-appointments/{appNo}/part-lines/{lineId:long}", async (string appNo, long lineId, UpdateServiceAppointmentPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceAppointmentPartLineAsync(appNo, lineId, dto);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-appointments/{appNo}/part-lines", async (string appNo, List<ServiceAppointmentPartItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items phụ tùng để thêm vào lịch hẹn." });
    try
    {
        var r = await svc.AddServiceAppointmentPartLinesAsync(appNo, items);
        return r is null ? Results.NotFound(new { appNo, error = "Không tìm thấy lịch hẹn hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-appointments/{appNo}/part-lines/{lineId:long}", async (string appNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServiceAppointmentPartLineAsync(appNo, lineId);
        return r is null ? Results.NotFound(new { appNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc lịch hẹn đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/appointments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAppointmentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/appointment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAppointmentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Bản tin kỹ thuật & Hướng dẫn kỹ thuật dịch vụ xe ô tô (BizCarSv.Bulletin / Blt_Bulletin / TechnicalBulletin) ----
app.MapPost("/api/technical-bulletins", async (CreateTechnicalBulletinDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { error = "Cần tiêu đề bản tin kỹ thuật Title." });
    try { return Results.Ok(await svc.CreateTechnicalBulletinAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/technical-bulletins", async (IVehicleService svc, string? status, string? category, string? severity, string? model, string? bulletinNo, string? vin) =>
    Results.Ok(await svc.ListTechnicalBulletinsAsync(status, category, severity, model, bulletinNo, vin))).RequireAuthorization();

app.MapGet("/api/technical-bulletins/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetTechnicalBulletinSummaryAsync())).RequireAuthorization();

app.MapGet("/api/technical-bulletins/{bulletinNo}", async (string bulletinNo, IVehicleService svc) =>
{
    var r = await svc.GetTechnicalBulletinAsync(bulletinNo);
    return r is null ? Results.NotFound(new { bulletinNo, error = "Không tìm thấy bản tin kỹ thuật." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/technical-bulletins/{bulletinNo}", async (string bulletinNo, UpdateTechnicalBulletinHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTechnicalBulletinHeaderAsync(bulletinNo, dto);
        return r is null ? Results.NotFound(new { bulletinNo, error = "Không tìm thấy bản tin kỹ thuật hoặc bản tin đã đóng/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/technical-bulletins/{bulletinNo}/{action}", async (string bulletinNo, string action, TechnicalBulletinTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("publish" or "submit" or "suspend" or "resume" or "archive" or "close" or "cancel"))
        return Results.BadRequest(new { error = "action = publish|suspend|archive|cancel" });
    try
    {
        var r = await svc.TechnicalBulletinTransitionAsync(bulletinNo, action, dto);
        return r is null ? Results.NotFound(new { bulletinNo, error = "Không thấy bản tin kỹ thuật hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/technical-bulletins/{bulletinNo}/lines/{vin}/update", async (string bulletinNo, string vin, UpdateTechnicalBulletinLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTechnicalBulletinLineAsync(bulletinNo, vin, dto);
        return r is null ? Results.NotFound(new { bulletinNo, vin, error = "Không tìm thấy dòng xe trong bản tin hoặc bản tin đã đóng/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/technical-bulletins/{bulletinNo}/lines/{vin}/complete", async (string bulletinNo, string vin, CompleteBulletinLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CompleteTechnicalBulletinLineAsync(bulletinNo, vin, dto);
        return r is null ? Results.NotFound(new { bulletinNo, vin, error = "Không tìm thấy dòng xe trong bản tin hoặc bản tin đã đóng/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/technical-bulletins/{bulletinNo}/lines", async (string bulletinNo, List<TechnicalBulletinItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bản tin kỹ thuật." });
    try
    {
        var r = await svc.AddTechnicalBulletinLinesAsync(bulletinNo, items);
        return r is null ? Results.NotFound(new { bulletinNo, error = "Không tìm thấy bản tin kỹ thuật hoặc bản tin đã đóng/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/technical-bulletins/{bulletinNo}/lines/{vin}", async (string bulletinNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveTechnicalBulletinLineAsync(bulletinNo, vin);
        return r is null ? Results.NotFound(new { bulletinNo, vin, error = "Không tìm thấy dòng xe trong bản tin hoặc bản tin đã đóng/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/bulletins", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleBulletinHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/bulletin-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleBulletinHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị & Lệnh giao dịch giải ngân ngân hàng mua xe ô tô cho Đại lý (BizHTC.VietinBank & BizHTC.MBBank / RQ_BankingTransactions / BankDisbursement) ----
app.MapPost("/api/bank-disbursements", async (CreateBankDisbursementDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.BankCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và mã ngân hàng BankCode." });
    if ((dto.Items is null || dto.Items.Count == 0) && (dto.Vins is null || dto.Vins.Count == 0))
        return Results.BadRequest(new { error = "Cần danh sách xe Items hoặc Vins trong đề nghị giải ngân." });
    try { return Results.Ok(await svc.CreateBankDisbursementAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/bank-disbursements", async (IVehicleService svc, string? status, string? bank, string? dealer, string? transNo, string? vin) =>
    Results.Ok(await svc.ListBankDisbursementsAsync(status, bank, dealer, transNo, vin))).RequireAuthorization();

app.MapGet("/api/bank-disbursements/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetBankDisbursementSummaryAsync())).RequireAuthorization();

app.MapGet("/api/bank-disbursements/{transNo}", async (string transNo, IVehicleService svc) =>
{
    var r = await svc.GetBankDisbursementAsync(transNo);
    return r is null ? Results.NotFound(new { transNo, error = "Không tìm thấy hồ sơ đề nghị giải ngân." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/bank-disbursements/{transNo}", async (string transNo, UpdateBankDisbursementHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateBankDisbursementHeaderAsync(transNo, dto);
        return r is null ? Results.NotFound(new { transNo, error = "Không tìm thấy hồ sơ đề nghị giải ngân hoặc hồ sơ đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/bank-disbursements/{transNo}/{action}", async (string transNo, string action, BankDisbursementTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve" or "push-to-bank" or "pushtobank" or "pushbank" or "disburse" or "complete" or "finish" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|push-to-bank|disburse|reject|cancel" });
    try
    {
        var r = await svc.BankDisbursementTransitionAsync(transNo, action, dto);
        return r is null ? Results.NotFound(new { transNo, error = "Không thấy đề nghị giải ngân hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/bank-disbursements/{transNo}/lines/{vin}/update", async (string transNo, string vin, UpdateBankDisbursementLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateBankDisbursementLineAsync(transNo, vin, dto);
        return r is null ? Results.NotFound(new { transNo, vin, error = "Không tìm thấy dòng xe trong đề nghị giải ngân hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/bank-disbursements/{transNo}/lines/{vin}", async (string transNo, string vin, UpdateBankDisbursementLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateBankDisbursementLineAsync(transNo, vin, dto);
        return r is null ? Results.NotFound(new { transNo, vin, error = "Không tìm thấy dòng xe trong đề nghị giải ngân hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/bank-disbursements/{transNo}/lines", async (string transNo, List<BankDisbursementItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào đề nghị giải ngân." });
    try
    {
        var r = await svc.AddBankDisbursementLinesAsync(transNo, items);
        return r is null ? Results.NotFound(new { transNo, error = "Không tìm thấy đề nghị giải ngân hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/bank-disbursements/{transNo}/lines/{vin}", async (string transNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveBankDisbursementLineAsync(transNo, vin);
        return r is null ? Results.NotFound(new { transNo, vin, error = "Không tìm thấy dòng xe trong đề nghị giải ngân hoặc hồ sơ đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/disbursement-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleDisbursementInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/disbursement-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleDisbursementHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Chiến dịch Dịch vụ & Khuyến mãi Hậu mãi xe ô tô (BizCarSv.CampaignMarketing / Ser_CampaignMarketing / ServiceCampaign) ----
app.MapPost("/api/service-campaigns", async (CreateServiceCampaignDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.CampaignName))
        return Results.BadRequest(new { error = "Cần tên chiến dịch khuyến mãi CampaignName." });
    try { return Results.Ok(await svc.CreateServiceCampaignAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/service-campaigns", async (IVehicleService svc, string? status, string? dealer, string? campaignType, string? camMarketingNo, string? vin) =>
    Results.Ok(await svc.ListServiceCampaignsAsync(status, dealer, campaignType, camMarketingNo, vin))).RequireAuthorization();

app.MapGet("/api/service-campaigns/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetServiceCampaignSummaryAsync())).RequireAuthorization();

app.MapGet("/api/service-campaigns/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetServiceCampaignAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy chiến dịch khuyến mãi dịch vụ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/service-campaigns/{code}", async (string code, UpdateServiceCampaignHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceCampaignHeaderAsync(code, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy chiến dịch khuyến mãi hoặc chiến dịch đã kết thúc/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-campaigns/{code}/{action}", async (string code, string action, ServiceCampaignTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve" or "activate" or "active" or "start" or "in-progress" or "inprogress" or "complete" or "finish" or "suspend" or "resume" or "cancel" or "reject"))
        return Results.BadRequest(new { error = "action = submit|approve|start|complete|suspend|resume|cancel" });
    try
    {
        var r = await svc.ServiceCampaignTransitionAsync(code, action, dto);
        return r is null ? Results.NotFound(new { code, error = "Không thấy chiến dịch khuyến mãi hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-campaigns/{code}/lines/{vin}/attend", async (string code, string vin, AttendServiceCampaignLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.AttendServiceCampaignLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong chiến dịch hoặc chiến dịch đã hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-campaigns/{code}/lines/{vin}/update", async (string code, string vin, UpdateServiceCampaignLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceCampaignLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong chiến dịch hoặc chiến dịch đã hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/service-campaigns/{code}/lines/{vin}", async (string code, string vin, UpdateServiceCampaignLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServiceCampaignLineAsync(code, vin, dto);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong chiến dịch hoặc chiến dịch đã hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-campaigns/{code}/lines", async (string code, List<ServiceCampaignItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào chiến dịch khuyến mãi." });
    try
    {
        var r = await svc.AddServiceCampaignLinesAsync(code, items);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy chiến dịch khuyến mãi hoặc chiến dịch đã kết thúc/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-campaigns/{code}/lines/{vin}", async (string code, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServiceCampaignLineAsync(code, vin);
        return r is null ? Results.NotFound(new { code, vin, error = "Không tìm thấy dòng xe trong chiến dịch hoặc chiến dịch đã kết thúc/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/campaign-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCampaignInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/campaigns", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCampaignHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/campaign-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCampaignHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Báo cáo & Quyết toán Bảo hành xe ô tô OEM / Đại lý ủy quyền (BizCarSv.WarrantyReport / Ser_ROWarrantyReport / WarrantyReport) ----
app.MapPost("/api/warranty-reports", async (CreateWarrantyReportDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và số khung Vin để lập báo cáo bảo hành." });
    try { return Results.Ok(await svc.CreateWarrantyReportAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/warranty-reports", async (IVehicleService svc, string? status, string? dealer, string? vin, string? plateNo, string? rowNo, string? warrantyType, string? causeCode, string? naturalCode) =>
    Results.Ok(await svc.ListWarrantyReportsAsync(status, dealer, vin, plateNo, rowNo, warrantyType, causeCode, naturalCode))).RequireAuthorization();

app.MapGet("/api/warranty-reports/summary", async (IVehicleService svc) =>
    Results.Ok(await svc.GetWarrantyReportSummaryAsync())).RequireAuthorization();

app.MapGet("/api/warranty-reports/{rowNo}", async (string rowNo, IVehicleService svc) =>
{
    var r = await svc.GetWarrantyReportAsync(rowNo);
    return r is null ? Results.NotFound(new { rowNo, error = "Không tìm thấy hồ sơ báo cáo bảo hành." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/warranty-reports/{rowNo}", async (string rowNo, UpdateWarrantyReportHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateWarrantyReportHeaderAsync(rowNo, dto);
        return r is null ? Results.NotFound(new { rowNo, error = "Không tìm thấy hồ sơ bảo hành hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/warranty-reports/{rowNo}/{action}", async (string rowNo, string action, WarrantyReportTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "confirm" or "approve" or "settle" or "pay" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|confirm|approve|settle|reject|cancel" });
    try
    {
        var r = await svc.WarrantyReportTransitionAsync(rowNo, action, dto);
        return r is null ? Results.NotFound(new { rowNo, error = "Không thấy hồ sơ bảo hành hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/warranty-reports/{rowNo}/labor-lines", async (string rowNo, List<WarrantyReportLaborItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items hạng mục công việc bảo hành." });
    try
    {
        var r = await svc.AddWarrantyReportLaborLinesAsync(rowNo, items);
        return r is null ? Results.NotFound(new { rowNo, error = "Không tìm thấy hồ sơ bảo hành hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/warranty-reports/{rowNo}/labor-lines/{lineId:long}/update", async (string rowNo, long lineId, UpdateWarrantyReportLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateWarrantyReportLaborLineAsync(rowNo, lineId, dto);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng công việc hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/warranty-reports/{rowNo}/labor-lines/{lineId:long}", async (string rowNo, long lineId, UpdateWarrantyReportLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateWarrantyReportLaborLineAsync(rowNo, lineId, dto);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng công việc hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/warranty-reports/{rowNo}/labor-lines/{lineId:long}", async (string rowNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveWarrantyReportLaborLineAsync(rowNo, lineId);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng công việc hoặc hồ sơ đã quyết toán/phê duyệt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/warranty-reports/{rowNo}/part-lines", async (string rowNo, List<WarrantyReportPartItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items phụ tùng bảo hành." });
    try
    {
        var r = await svc.AddWarrantyReportPartLinesAsync(rowNo, items);
        return r is null ? Results.NotFound(new { rowNo, error = "Không tìm thấy hồ sơ bảo hành hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/warranty-reports/{rowNo}/part-lines/{lineId:long}/update", async (string rowNo, long lineId, UpdateWarrantyReportPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateWarrantyReportPartLineAsync(rowNo, lineId, dto);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/warranty-reports/{rowNo}/part-lines/{lineId:long}", async (string rowNo, long lineId, UpdateWarrantyReportPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateWarrantyReportPartLineAsync(rowNo, lineId, dto);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc hồ sơ đã quyết toán/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/warranty-reports/{rowNo}/part-lines/{lineId:long}", async (string rowNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveWarrantyReportPartLineAsync(rowNo, lineId);
        return r is null ? Results.NotFound(new { rowNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc hồ sơ đã quyết toán/phê duyệt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/warranty-reports", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleWarrantyReportHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/warranty-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleWarrantyReportHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Báo giá Dịch vụ & Phụ tùng xưởng sửa chữa xe ô tô (BizCarSv.Inventory.Quote / Ser_Quotation / ServiceQuotation) =====

app.MapPost("/api/quotations", async (CreateQuotationDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần cung cấp số khung VIN để tạo báo giá." });
    try { return Results.Ok(await svc.CreateQuotationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/quotations", async (IVehicleService svc, string? status, string? dealer, string? vin, string? customer, string? quoteType) =>
    Results.Ok(await svc.ListQuotationsAsync(status, dealer, vin, customer, quoteType))).RequireAuthorization();

app.MapGet("/api/quotations/summary-report", async (IVehicleService svc, string? dealerCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetQuotationSummaryAsync(dealerCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/reports/quotations/summary", async (IVehicleService svc, string? dealerCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetQuotationSummaryAsync(dealerCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/quotations/{quoteNo}", async (string quoteNo, IVehicleService svc) =>
{
    var r = await svc.GetQuotationAsync(quoteNo);
    return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/quotations/{quoteNo}", async (string quoteNo, UpdateQuotationDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/update", async (string quoteNo, UpdateQuotationDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/send", async (string quoteNo, SendQuotationDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.SendQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/customer-approve", async (string quoteNo, CustomerApproveQuotationDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CustomerApproveQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/convert-to-ro", async (string quoteNo, ConvertQuotationToRoDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ConvertQuotationToRepairOrderAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/reject", async (string quoteNo, RejectQuotationDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RejectQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/cancel", async (string quoteNo, CancelQuotationDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CancelQuotationAsync(quoteNo, dto);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/labor-lines", async (string quoteNo, List<ServiceQuotationLaborItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách hạng mục công việc." });
    try
    {
        var r = await svc.AddQuotationLaborLinesAsync(quoteNo, items);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/labor-lines/{lineId:long}/update", async (string quoteNo, long lineId, UpdateQuotationLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationLaborLineAsync(quoteNo, lineId, dto);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng công việc hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/quotations/{quoteNo}/labor-lines/{lineId:long}", async (string quoteNo, long lineId, UpdateQuotationLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationLaborLineAsync(quoteNo, lineId, dto);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng công việc hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/quotations/{quoteNo}/labor-lines/{lineId:long}", async (string quoteNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveQuotationLaborLineAsync(quoteNo, lineId);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng công việc hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/part-lines", async (string quoteNo, List<ServiceQuotationPartItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách phụ tùng." });
    try
    {
        var r = await svc.AddQuotationPartLinesAsync(quoteNo, items);
        return r is null ? Results.NotFound(new { quoteNo, error = "Không tìm thấy báo giá hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/quotations/{quoteNo}/part-lines/{lineId:long}/update", async (string quoteNo, long lineId, UpdateQuotationPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationPartLineAsync(quoteNo, lineId, dto);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/quotations/{quoteNo}/part-lines/{lineId:long}", async (string quoteNo, long lineId, UpdateQuotationPartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateQuotationPartLineAsync(quoteNo, lineId, dto);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/quotations/{quoteNo}/part-lines/{lineId:long}", async (string quoteNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveQuotationPartLineAsync(quoteNo, lineId);
        return r is null ? Results.NotFound(new { quoteNo, lineId, error = "Không tìm thấy dòng phụ tùng hoặc báo giá đã khóa/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/quotations", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleQuotationHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/quotation-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleQuotationHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Chăm sóc khách hàng & Khảo sát CSI sau dịch vụ/bán xe (BizCarSv.Customer / Ser_CustomerCare, Ser_CustomerCare24h, Ser_CustomerCare72h, Ser_CustomerCareMaintance, Ser_CustomerCareBth) =====

app.MapPost("/api/customer-cares", async (CreateCustomerCareDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần cung cấp số khung VIN để tạo phiếu chăm sóc khách hàng." });
    try { return Results.Ok(await svc.CreateCustomerCareAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/customer-cares", async (IVehicleService svc, string? status, string? dealer, string? vin, string? careType, string? contactMethod, decimal? minScore, string? careNo) =>
    Results.Ok(await svc.ListCustomerCaresAsync(status, dealer, vin, careType, contactMethod, minScore, careNo))).RequireAuthorization();

app.MapGet("/api/customer-cares/summary", async (IVehicleService svc, string? dealerCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetCustomerCareSummaryAsync(dealerCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/reports/customer-cares/summary", async (IVehicleService svc, string? dealerCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetCustomerCareSummaryAsync(dealerCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/customer-cares/{careNo}", async (string careNo, IVehicleService svc) =>
{
    var r = await svc.GetCustomerCareAsync(careNo);
    return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/customer-cares/{careNo}", async (string careNo, UpdateCustomerCareDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCustomerCareAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-cares/{careNo}/update", async (string careNo, UpdateCustomerCareDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCustomerCareAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-cares/{careNo}/contact-attempt", async (string careNo, RecordContactAttemptDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RecordContactAttemptAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-cares/{careNo}/complete", async (string careNo, CompleteCustomerCareDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CompleteCustomerCareAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-cares/{careNo}/escalate", async (string careNo, EscalateCustomerCareDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.EscalateCustomerCareAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-cares/{careNo}/cancel", async (string careNo, CancelCustomerCareDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CancelCustomerCareAsync(careNo, dto);
        return r is null ? Results.NotFound(new { careNo, error = "Không tìm thấy phiếu CSKH." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/customer-cares", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCareHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/customer-care-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCareHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/care-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCareHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Lệnh sản xuất & Kế hoạch sản xuất ô tô tại Nhà máy OEM (BizHTC.WorkOrder & BizHTC.MMSIntergration / MnfPl_Order / ProductionOrder) =====

app.MapPost("/api/production-orders", async (CreateProductionOrderDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.OrdMonth))
        return Results.BadRequest(new { error = "Cần cung cấp tháng sản xuất kế hoạch OrdMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateProductionOrderAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/production-orders", async (IVehicleService svc, string? status, string? plantCode, string? ordMonth, string? ordType, string? model, string? orderNo) =>
    Results.Ok(await svc.ListProductionOrdersAsync(status, plantCode, ordMonth, ordType, model, orderNo))).RequireAuthorization();

app.MapGet("/api/production-orders/summary", async (IVehicleService svc, string? plantCode, string? ordMonth) =>
    Results.Ok(await svc.GetProductionSummaryAsync(plantCode, ordMonth))).RequireAuthorization();

app.MapGet("/api/reports/production/summary", async (IVehicleService svc, string? plantCode, string? ordMonth) =>
    Results.Ok(await svc.GetProductionSummaryAsync(plantCode, ordMonth))).RequireAuthorization();

app.MapGet("/api/production-orders/{orderNo}", async (string orderNo, IVehicleService svc) =>
{
    var r = await svc.GetProductionOrderAsync(orderNo);
    return r is null ? Results.NotFound(new { orderNo, error = "Không tìm thấy lệnh sản xuất." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/production-orders/{orderNo}", async (string orderNo, UpdateProductionOrderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProductionOrderAsync(orderNo, dto);
        return r is null ? Results.NotFound(new { orderNo, error = "Không tìm thấy lệnh sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/production-orders/{orderNo}/update", async (string orderNo, UpdateProductionOrderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProductionOrderAsync(orderNo, dto);
        return r is null ? Results.NotFound(new { orderNo, error = "Không tìm thấy lệnh sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/production-orders/{orderNo}/{action}", async (string orderNo, string action, ProductionOrderTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "schedule" or "start" or "in-production" or "inprogress" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|schedule|start|cancel" });
    try
    {
        var r = await svc.ProductionOrderTransitionAsync(orderNo, action, dto);
        return r is null ? Results.NotFound(new { orderNo, error = "Không tìm thấy lệnh sản xuất hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/production-orders/{orderNo}/lines/{lineId:long}/produce-vin", async (string orderNo, long lineId, ProduceVinDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ProduceVinAsync(orderNo, lineId, dto);
        return r is null ? Results.NotFound(new { orderNo, lineId, error = "Không tìm thấy dòng sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/production-orders/{orderNo}/lines", async (string orderNo, List<ProductionOrderItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách dòng sản phẩm để thêm vào đơn hàng." });
    try
    {
        var r = await svc.AddProductionOrderLinesAsync(orderNo, items);
        return r is null ? Results.NotFound(new { orderNo, error = "Không tìm thấy lệnh sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/production-orders/{orderNo}/lines/{lineId:long}/update", async (string orderNo, long lineId, UpdateProductionOrderLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProductionOrderLineAsync(orderNo, lineId, dto);
        return r is null ? Results.NotFound(new { orderNo, lineId, error = "Không tìm thấy dòng sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/production-orders/{orderNo}/lines/{lineId:long}", async (string orderNo, long lineId, UpdateProductionOrderLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProductionOrderLineAsync(orderNo, lineId, dto);
        return r is null ? Results.NotFound(new { orderNo, lineId, error = "Không tìm thấy dòng sản xuất." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/production-orders/{orderNo}/lines/{lineId:long}", async (string orderNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveProductionOrderLineAsync(orderNo, lineId);
        return r is null ? Results.NotFound(new { orderNo, lineId, error = "Không tìm thấy dòng sản xuất hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/production-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleProductionInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/production-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleProductionHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Công khai (không cần auth): tra cứu VIN + bảo hành (cho app/đại lý/khách) ----
app.MapGet("/api/lookup", async (string vin, IVehicleService svc) =>
{
    var r = await svc.PublicLookupAsync(vin);
    return r is null ? Results.NotFound(new { vin, found = false }) : Results.Ok(r);
});

app.MapPost("/api/orgs/register", async (RegisterOrgDto dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest(new { error = "Cần Name." });
    var org = new Org { Name = dto.Name.Trim(), ApiKey = "veh_" + Guid.NewGuid().ToString("N") };
    db.Orgs.Add(org); await db.SaveChangesAsync();
    return Results.Ok(new { orgId = org.Id, apiKey = org.ApiKey });
});

app.Run();

record ImportVehicleRowDto(string? VIN, string? ModelCode, string? SpecCode, string? ColorCode, string? EngineNo, int? ProductionYearActual, string? StorageCodeCurrent);
record RegisterOrgDto(string Name);
record AllocateDto(string DealerCode);
