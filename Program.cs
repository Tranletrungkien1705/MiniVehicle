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
