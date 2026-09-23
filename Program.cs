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

// ---- Hợp đồng phụ kiện xe ô tô của Đại lý (HCare.idocNet Dlr_ContractMstPart / Dlr_ContractMstPartDtl) ----
app.MapPost("/api/accessory-contracts", async (CreateAccessoryContractDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode." });
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách phụ kiện Items trong phụ lục hợp đồng." });
    try { return Results.Ok(await svc.CreateAccessoryContractAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/accessory-contracts", async (IVehicleService svc, string? status, string? dealer, string? dlrContractPartNo, string? customer, string? partCode) =>
    Results.Ok(await svc.ListAccessoryContractsAsync(status, dealer, dlrContractPartNo, customer, partCode))).RequireAuthorization();

app.MapGet("/api/accessory-contracts/{dlrContractPartNo}", async (string dlrContractPartNo, IVehicleService svc) =>
{
    var r = await svc.GetAccessoryContractAsync(dlrContractPartNo);
    return r is null ? Results.NotFound(new { dlrContractPartNo, error = "Không tìm thấy phụ lục hợp đồng phụ kiện." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/accessory-contracts/{dlrContractPartNo}/{action}", async (string dlrContractPartNo, string action, AccessoryContractTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|cancel" });
    var r = await svc.AccessoryContractTransitionAsync(dlrContractPartNo, action, dto);
    return r is null ? Results.NotFound(new { dlrContractPartNo, error = "Không thấy phụ lục hợp đồng phụ kiện hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/accessory-contracts/{dlrContractPartNo}/lines/{partCode}/update", async (string dlrContractPartNo, string partCode, UpdateAccessoryContractLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateAccessoryContractLineAsync(dlrContractPartNo, partCode, dto);
    return r is null ? Results.NotFound(new { dlrContractPartNo, partCode, error = "Không tìm thấy dòng phụ kiện hoặc phụ lục đã duyệt/hủy." }) : Results.Ok(r);
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

// ===== Hóa đơn chiếu lệ Proforma Invoice (BizHTC.Order.PerformanceInvoice / Ord_PI / ProformaInvoice) =====

app.MapPost("/api/proforma-invoices", async (CreateProformaInvoiceDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.OrderMonth))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và tháng đặt hàng OrderMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateProformaInvoiceAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/proforma-invoices", async (IVehicleService svc, string? status, string? dealer, string? orderMonth, string? productionMonth, string? refNo, string? model, string? vin) =>
    Results.Ok(await svc.ListProformaInvoicesAsync(status, dealer, orderMonth, productionMonth, refNo, model, vin))).RequireAuthorization();

app.MapGet("/api/proforma-invoices/summary", async (IVehicleService svc, string? dealerCode, string? orderMonth) =>
    Results.Ok(await svc.GetProformaInvoiceSummaryAsync(dealerCode, orderMonth))).RequireAuthorization();

app.MapGet("/api/reports/proforma-invoices/summary", async (IVehicleService svc, string? dealerCode, string? orderMonth) =>
    Results.Ok(await svc.GetProformaInvoiceSummaryAsync(dealerCode, orderMonth))).RequireAuthorization();

app.MapGet("/api/proforma-invoices/{refNo}", async (string refNo, IVehicleService svc) =>
{
    var r = await svc.GetProformaInvoiceAsync(refNo);
    return r is null ? Results.NotFound(new { refNo, error = "Không tìm thấy Proforma Invoice." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/proforma-invoices/{refNo}", async (string refNo, UpdateProformaInvoiceHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProformaInvoiceHeaderAsync(refNo, dto);
        return r is null ? Results.NotFound(new { refNo, error = "Không tìm thấy Proforma Invoice." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/proforma-invoices/{refNo}/update", async (string refNo, UpdateProformaInvoiceHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProformaInvoiceHeaderAsync(refNo, dto);
        return r is null ? Results.NotFound(new { refNo, error = "Không tìm thấy Proforma Invoice." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/proforma-invoices/{refNo}/{action}", async (string refNo, string action, ProformaInvoiceTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "execute" or "in-execution" or "inexecution" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|execute|complete|reject|cancel" });
    try
    {
        var r = await svc.ProformaInvoiceTransitionAsync(refNo, action, dto);
        return r is null ? Results.NotFound(new { refNo, error = "Không tìm thấy Proforma Invoice hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/proforma-invoices/{refNo}/lines/{lineId:long}/allocate-vin", async (string refNo, long lineId, AllocateVinToPiLineDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần số khung Vin để phân bổ vào dòng Proforma Invoice." });
    try
    {
        var r = await svc.AllocateVinToPiLineAsync(refNo, lineId, dto);
        return r is null ? Results.NotFound(new { refNo, lineId, error = "Không tìm thấy Proforma Invoice hoặc dòng chi tiết." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/proforma-invoices/{refNo}/lines", async (string refNo, List<ProformaInvoiceItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items dòng xe để thêm vào Proforma Invoice." });
    try
    {
        var r = await svc.AddProformaInvoiceLinesAsync(refNo, items);
        return r is null ? Results.NotFound(new { refNo, error = "Không tìm thấy Proforma Invoice." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/proforma-invoices/{refNo}/lines/{lineId:long}/update", async (string refNo, long lineId, UpdateProformaInvoiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProformaInvoiceLineAsync(refNo, lineId, dto);
        return r is null ? Results.NotFound(new { refNo, lineId, error = "Không tìm thấy dòng chi tiết Proforma Invoice." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/proforma-invoices/{refNo}/lines/{lineId:long}", async (string refNo, long lineId, UpdateProformaInvoiceLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateProformaInvoiceLineAsync(refNo, lineId, dto);
        return r is null ? Results.NotFound(new { refNo, lineId, error = "Không tìm thấy dòng chi tiết Proforma Invoice." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/proforma-invoices/{refNo}/lines/{lineId:long}", async (string refNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveProformaInvoiceLineAsync(refNo, lineId);
        return r is null ? Results.NotFound(new { refNo, lineId, error = "Không tìm thấy dòng chi tiết Proforma Invoice hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/proforma-invoice-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePiInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pi-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePiInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pi-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePiHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Bảng kê & Quyết toán chi phí kiểm tra PDI xe cho Đại lý & Kho bãi (BizHTC.Payment.Pmt_PaymentPDI / PdiPayment) =====

app.MapPost("/api/pdi-payments", async (CreatePdiPaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.PeriodMonth))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và kỳ quyết toán PeriodMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreatePdiPaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/pdi-payments", async (IVehicleService svc, string? status, string? dealer, string? storage, string? periodMonth, string? pmtPdiNo, string? vin) =>
    Results.Ok(await svc.ListPdiPaymentsAsync(status, dealer, storage, periodMonth, pmtPdiNo, vin))).RequireAuthorization();

app.MapGet("/api/pdi-payments/summary", async (IVehicleService svc, string? dealerCode, string? periodMonth, string? storageCode) =>
    Results.Ok(await svc.GetPdiPaymentSummaryAsync(dealerCode, periodMonth, storageCode))).RequireAuthorization();

app.MapGet("/api/reports/pdi-payments/summary", async (IVehicleService svc, string? dealerCode, string? periodMonth, string? storageCode) =>
    Results.Ok(await svc.GetPdiPaymentSummaryAsync(dealerCode, periodMonth, storageCode))).RequireAuthorization();

app.MapGet("/api/pdi-payments/{pmtPdiNo}", async (string pmtPdiNo, IVehicleService svc) =>
{
    var r = await svc.GetPdiPaymentAsync(pmtPdiNo);
    return r is null ? Results.NotFound(new { pmtPdiNo, error = "Không tìm thấy bảng kê quyết toán PDI." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/pdi-payments/{pmtPdiNo}", async (string pmtPdiNo, UpdatePdiPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdatePdiPaymentHeaderAsync(pmtPdiNo, dto);
        return r is null ? Results.NotFound(new { pmtPdiNo, error = "Không tìm thấy bảng kê quyết toán PDI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/pdi-payments/{pmtPdiNo}/update", async (string pmtPdiNo, UpdatePdiPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdatePdiPaymentHeaderAsync(pmtPdiNo, dto);
        return r is null ? Results.NotFound(new { pmtPdiNo, error = "Không tìm thấy bảng kê quyết toán PDI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/pdi-payments/{pmtPdiNo}/{action}", async (string pmtPdiNo, string action, PdiPaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve1" or "approve-tech" or "tech-approve" or "approve2" or "approve-finance" or "finance-approve" or "approve" or "tcms-sign" or "tcmssign" or "sign-tcms" or "htv-sign" or "htvsign" or "sign-htv" or "settle" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|tcms-sign|htv-sign|settle|reject|cancel" });
    try
    {
        var r = await svc.PdiPaymentTransitionAsync(pmtPdiNo, action, dto);
        return r is null ? Results.NotFound(new { pmtPdiNo, error = "Không tìm thấy bảng kê quyết toán PDI hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/pdi-payments/{pmtPdiNo}/lines", async (string pmtPdiNo, List<PdiPaymentItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe items để thêm vào bảng kê quyết toán PDI." });
    try
    {
        var r = await svc.AddPdiPaymentLinesAsync(pmtPdiNo, items);
        return r is null ? Results.NotFound(new { pmtPdiNo, error = "Không tìm thấy bảng kê quyết toán PDI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/pdi-payments/{pmtPdiNo}/lines/{vin}/update", async (string pmtPdiNo, string vin, UpdatePdiPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdatePdiPaymentLineAsync(pmtPdiNo, vin, dto);
        return r is null ? Results.NotFound(new { pmtPdiNo, vin, error = "Không tìm thấy dòng xe trong bảng kê quyết toán PDI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/pdi-payments/{pmtPdiNo}/lines/{vin}", async (string pmtPdiNo, string vin, UpdatePdiPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdatePdiPaymentLineAsync(pmtPdiNo, vin, dto);
        return r is null ? Results.NotFound(new { pmtPdiNo, vin, error = "Không tìm thấy dòng xe trong bảng kê quyết toán PDI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/pdi-payments/{pmtPdiNo}/lines/{vin}", async (string pmtPdiNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemovePdiPaymentLineAsync(pmtPdiNo, vin);
        return r is null ? Results.NotFound(new { pmtPdiNo, vin, error = "Không tìm thấy dòng xe trong bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pdi-payment-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePdiPaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pdi-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePdiPaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pdi-payments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePdiPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/pdi-payment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePdiPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Chính sách hỗ trợ bán lẻ xe ô tô cho Đại lý & Quyết toán hỗ trợ theo VIN (BizHTC.DealerSales / SPL_SalesPolicyMst & SPL_SPSupportRetail) =====

app.MapPost("/api/sales-policies", async (CreateSalesPolicyDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.SPNo))
        return Results.BadRequest(new { error = "Cần số hiệu văn bản chính sách SPNo." });
    try { return Results.Ok(await svc.CreateSalesPolicyAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/sales-policies", async (IVehicleService svc, string? status, string? spsrType, string? q) =>
    Results.Ok(await svc.ListSalesPoliciesAsync(status, spsrType, q))).RequireAuthorization();

app.MapGet("/api/sales-policies/summary", async (IVehicleService svc, string? spsrCode, string? dealerCode) =>
    Results.Ok(await svc.GetSalesPolicySummaryAsync(spsrCode, dealerCode))).RequireAuthorization();

app.MapGet("/api/reports/sales-policies/summary", async (IVehicleService svc, string? spsrCode, string? dealerCode) =>
    Results.Ok(await svc.GetSalesPolicySummaryAsync(spsrCode, dealerCode))).RequireAuthorization();

app.MapGet("/api/sales-policies/{spsrCode}", async (string spsrCode, IVehicleService svc) =>
{
    var r = await svc.GetSalesPolicyAsync(spsrCode);
    return r is null ? Results.NotFound(new { spsrCode, error = "Không tìm thấy chính sách hỗ trợ bán hàng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/sales-policies/{spsrCode}", async (string spsrCode, UpdateSalesPolicyHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicyHeaderAsync(spsrCode, dto);
        return r is null ? Results.NotFound(new { spsrCode, error = "Không tìm thấy chính sách hỗ trợ bán hàng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policies/{spsrCode}/update", async (string spsrCode, UpdateSalesPolicyHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicyHeaderAsync(spsrCode, dto);
        return r is null ? Results.NotFound(new { spsrCode, error = "Không tìm thấy chính sách hỗ trợ bán hàng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policies/{spsrCode}/{action}", async (string spsrCode, string action, SalesPolicyTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "activate" or "active" or "suspend" or "expire" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|activate|suspend|expire|cancel" });
    try
    {
        var r = await svc.SalesPolicyTransitionAsync(spsrCode, action, dto);
        return r is null ? Results.NotFound(new { spsrCode, error = "Không tìm thấy chính sách hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policies/{spsrCode}/lines", async (string spsrCode, List<SalesPolicyLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách các dòng model xe áp dụng chính sách." });
    try
    {
        var r = await svc.AddSalesPolicyLinesAsync(spsrCode, items);
        return r is null ? Results.NotFound(new { spsrCode, error = "Không tìm thấy chính sách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policies/{spsrCode}/lines/{lineId:long}/update", async (string spsrCode, long lineId, UpdateSalesPolicyLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicyLineAsync(spsrCode, lineId, dto);
        return r is null ? Results.NotFound(new { spsrCode, lineId, error = "Không tìm thấy dòng chi tiết chính sách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/sales-policies/{spsrCode}/lines/{lineId:long}", async (string spsrCode, long lineId, UpdateSalesPolicyLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicyLineAsync(spsrCode, lineId, dto);
        return r is null ? Results.NotFound(new { spsrCode, lineId, error = "Không tìm thấy dòng chi tiết chính sách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-policies/{spsrCode}/lines/{lineId:long}", async (string spsrCode, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesPolicyLineAsync(spsrCode, lineId);
        return r is null ? Results.NotFound(new { spsrCode, lineId, error = "Không tìm thấy dòng chi tiết chính sách hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policy-supports", async (CreateSalesPolicySupportDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.SPSRCode) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã chính sách SPSRCode và số khung xe VIN." });
    try { return Results.Ok(await svc.CreateSalesPolicySupportAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policy-supports/batch-assign", async (BatchAssignPolicySupportDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.SPSRCode) || dto.Vins is null || dto.Vins.Count == 0)
        return Results.BadRequest(new { error = "Cần mã chính sách SPSRCode và danh sách số khung xe VINs." });
    try { return Results.Ok(await svc.BatchAssignPolicySupportVinsAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/sales-policy-supports", async (IVehicleService svc, string? status, string? dealer, string? spsrCode, string? vin, string? supportNo) =>
    Results.Ok(await svc.ListSalesPolicySupportsAsync(status, dealer, spsrCode, vin, supportNo))).RequireAuthorization();

app.MapGet("/api/sales-policy-supports/{supportNo}", async (string supportNo, IVehicleService svc) =>
{
    var r = await svc.GetSalesPolicySupportAsync(supportNo);
    return r is null ? Results.NotFound(new { supportNo, error = "Không tìm thấy hồ sơ hỗ trợ bán lẻ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/sales-policy-supports/{supportNo}", async (string supportNo, UpdateSalesPolicySupportDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicySupportAsync(supportNo, dto);
        return r is null ? Results.NotFound(new { supportNo, error = "Không tìm thấy hồ sơ hỗ trợ bán lẻ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policy-supports/{supportNo}/update", async (string supportNo, UpdateSalesPolicySupportDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesPolicySupportAsync(supportNo, dto);
        return r is null ? Results.NotFound(new { supportNo, error = "Không tìm thấy hồ sơ hỗ trợ bán lẻ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-policy-supports/{supportNo}/{action}", async (string supportNo, string action, SalesPolicySupportTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "settle" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|settle|reject|cancel" });
    try
    {
        var r = await svc.SalesPolicySupportTransitionAsync(supportNo, action, dto);
        return r is null ? Results.NotFound(new { supportNo, error = "Không tìm thấy hồ sơ hỗ trợ bán lẻ hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-policy-supports/{supportNo}", async (string supportNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesPolicySupportAsync(supportNo);
        return r is null ? Results.NotFound(new { supportNo, error = "Không tìm thấy hồ sơ hỗ trợ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/policy-supports", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePolicySupportInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/policy-support-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePolicySupportInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/policy-support-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehiclePolicySupportHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Quản lý Thiết bị Định vị GPS & Giám sát Vị trí Xe (BizHTC.StorageFG / Sto_StoBalanceGPS, StoF_GPSIn, StoF_GPSOut, GPSF_GPSClaim) ----

app.MapPost("/api/gps/devices", async (RegisterGpsDeviceDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.GpsCode))
        return Results.BadRequest(new { error = "Cần mã thiết bị GpsCode." });
    try { return Results.Ok(await svc.RegisterGpsDeviceAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps/devices", async (IVehicleService svc, string? status, string? provider, string? storageCode, string? q) =>
    Results.Ok(await svc.ListGpsDevicesAsync(status, provider, storageCode, q))).RequireAuthorization();

app.MapGet("/api/gps/devices/{gpsCode}", async (string gpsCode, IVehicleService svc) =>
{
    var r = await svc.GetGpsDeviceAsync(gpsCode);
    return r is null ? Results.NotFound(new { gpsCode, error = "Không tìm thấy thiết bị định vị GPS." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/gps/devices/{gpsCode}", async (string gpsCode, UpdateGpsDeviceDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsDeviceAsync(gpsCode, dto);
        return r is null ? Results.NotFound(new { gpsCode, error = "Không tìm thấy thiết bị định vị GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/devices/{gpsCode}", async (string gpsCode, IVehicleService svc) =>
{
    try
    {
        var r = await svc.DeleteGpsDeviceAsync(gpsCode);
        return r is null ? Results.NotFound(new { gpsCode, error = "Không tìm thấy thiết bị định vị GPS hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/devices/{gpsCode}/location", async (string gpsCode, UpdateGpsLocationDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsLocationAsync(gpsCode, dto);
        return r is null ? Results.NotFound(new { gpsCode, error = "Không tìm thấy thiết bị định vị GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/installations", async (CreateGpsInstallationDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateGpsInstallationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps/installations", async (IVehicleService svc, string? status, string? gpsInType, string? vin, string? gpsCode) =>
    Results.Ok(await svc.ListGpsInstallationsAsync(status, gpsInType, vin, gpsCode))).RequireAuthorization();

app.MapGet("/api/gps/installations/{gpsInNo}", async (string gpsInNo, IVehicleService svc) =>
{
    var r = await svc.GetGpsInstallationAsync(gpsInNo);
    return r is null ? Results.NotFound(new { gpsInNo, error = "Không tìm thấy phiếu lắp đặt GPS." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/gps/installations/{gpsInNo}", async (string gpsInNo, UpdateGpsInstallationHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsInstallationHeaderAsync(gpsInNo, dto);
        return r is null ? Results.NotFound(new { gpsInNo, error = "Không tìm thấy phiếu lắp đặt GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/installations/{gpsInNo}/{action}", async (string gpsInNo, string action, GpsInstallationTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "install" or "complete" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|install|complete|cancel" });
    try
    {
        var r = await svc.GpsInstallationTransitionAsync(gpsInNo, action, dto);
        return r is null ? Results.NotFound(new { gpsInNo, error = "Không tìm thấy phiếu lắp đặt GPS hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/installations/{gpsInNo}/lines", async (string gpsInNo, List<GpsInstallationItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe & thiết bị items." });
    try
    {
        var r = await svc.AddGpsInstallationLinesAsync(gpsInNo, items);
        return r is null ? Results.NotFound(new { gpsInNo, error = "Không tìm thấy phiếu lắp đặt GPS hoặc không thể thêm dòng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/gps/installations/{gpsInNo}/lines/{lineId:long}", async (string gpsInNo, long lineId, UpdateGpsInstallationLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsInstallationLineAsync(gpsInNo, lineId, dto);
        return r is null ? Results.NotFound(new { gpsInNo, lineId, error = "Không tìm thấy dòng chi tiết lắp đặt." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/installations/{gpsInNo}/lines/{lineId:long}", async (string gpsInNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsInstallationLineAsync(gpsInNo, lineId);
        return r is null ? Results.NotFound(new { gpsInNo, lineId, error = "Không tìm thấy dòng chi tiết lắp đặt hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/installations/{gpsInNo}", async (string gpsInNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsInstallationAsync(gpsInNo);
        return r is null ? Results.NotFound(new { gpsInNo, error = "Không tìm thấy phiếu lắp đặt hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/uninstallations", async (CreateGpsUninstallationDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateGpsUninstallationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps/uninstallations", async (IVehicleService svc, string? status, string? reason, string? vin, string? gpsCode) =>
    Results.Ok(await svc.ListGpsUninstallationsAsync(status, reason, vin, gpsCode))).RequireAuthorization();

app.MapGet("/api/gps/uninstallations/{gpsOutNo}", async (string gpsOutNo, IVehicleService svc) =>
{
    var r = await svc.GetGpsUninstallationAsync(gpsOutNo);
    return r is null ? Results.NotFound(new { gpsOutNo, error = "Không tìm thấy phiếu tháo gỡ GPS." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/gps/uninstallations/{gpsOutNo}", async (string gpsOutNo, UpdateGpsUninstallationHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsUninstallationHeaderAsync(gpsOutNo, dto);
        return r is null ? Results.NotFound(new { gpsOutNo, error = "Không tìm thấy phiếu tháo gỡ GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/uninstallations/{gpsOutNo}/{action}", async (string gpsOutNo, string action, GpsUninstallationTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "complete" or "finish" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|complete|finish|cancel" });
    try
    {
        var r = await svc.GpsUninstallationTransitionAsync(gpsOutNo, action, dto);
        return r is null ? Results.NotFound(new { gpsOutNo, error = "Không tìm thấy phiếu tháo gỡ GPS hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/uninstallations/{gpsOutNo}/lines", async (string gpsOutNo, List<GpsUninstallationItemInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách xe items." });
    try
    {
        var r = await svc.AddGpsUninstallationLinesAsync(gpsOutNo, items);
        return r is null ? Results.NotFound(new { gpsOutNo, error = "Không tìm thấy phiếu tháo gỡ GPS hoặc không thể thêm dòng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/gps/uninstallations/{gpsOutNo}/lines/{lineId:long}", async (string gpsOutNo, long lineId, UpdateGpsUninstallationLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsUninstallationLineAsync(gpsOutNo, lineId, dto);
        return r is null ? Results.NotFound(new { gpsOutNo, lineId, error = "Không tìm thấy dòng chi tiết tháo gỡ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/uninstallations/{gpsOutNo}/lines/{lineId:long}", async (string gpsOutNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsUninstallationLineAsync(gpsOutNo, lineId);
        return r is null ? Results.NotFound(new { gpsOutNo, lineId, error = "Không tìm thấy dòng chi tiết tháo gỡ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/uninstallations/{gpsOutNo}", async (string gpsOutNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsUninstallationAsync(gpsOutNo);
        return r is null ? Results.NotFound(new { gpsOutNo, error = "Không tìm thấy phiếu tháo gỡ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/claims", async (CreateGpsClaimDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.GpsCode))
        return Results.BadRequest(new { error = "Cần mã thiết bị GpsCode." });
    try { return Results.Ok(await svc.CreateGpsClaimAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps/claims", async (IVehicleService svc, string? status, string? vendor, string? faultType, string? gpsCode, string? vin) =>
    Results.Ok(await svc.ListGpsClaimsAsync(status, vendor, faultType, gpsCode, vin))).RequireAuthorization();

app.MapGet("/api/gps/claims/{gpsClaimNo}", async (string gpsClaimNo, IVehicleService svc) =>
{
    var r = await svc.GetGpsClaimAsync(gpsClaimNo);
    return r is null ? Results.NotFound(new { gpsClaimNo, error = "Không tìm thấy phiếu yêu cầu bảo hành GPS." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/gps/claims/{gpsClaimNo}", async (string gpsClaimNo, UpdateGpsClaimDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsClaimAsync(gpsClaimNo, dto);
        return r is null ? Results.NotFound(new { gpsClaimNo, error = "Không tìm thấy phiếu yêu cầu bảo hành GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps/claims/{gpsClaimNo}/{action}", async (string gpsClaimNo, string action, GpsClaimTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "send_to_vendor" or "send" or "repair" or "replace" or "receive" or "settle" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|send_to_vendor|repair|replace|settle|reject|cancel" });
    try
    {
        var r = await svc.GpsClaimTransitionAsync(gpsClaimNo, action, dto);
        return r is null ? Results.NotFound(new { gpsClaimNo, error = "Không tìm thấy phiếu yêu cầu bảo hành GPS hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps/claims/{gpsClaimNo}", async (string gpsClaimNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsClaimAsync(gpsClaimNo);
        return r is null ? Results.NotFound(new { gpsClaimNo, error = "Không tìm thấy phiếu bảo hành hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps/summary", async (IVehicleService svc, string? provider, string? storageCode) =>
    Results.Ok(await svc.GetGpsFleetSummaryAsync(provider, storageCode))).RequireAuthorization();

app.MapGet("/api/gps/vehicles/{vin}", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVinGpsLocationAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/gps/vehicles/{vin}/history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGpsHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/gps", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVinGpsLocationAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/gps-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGpsHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Quản lý Khoang sửa chữa xưởng dịch vụ & Điều phối xe (BizCarSv / Ser_Cavity & Ser_CavityDispatch / FrmCavityCreate, FrmCavitySearch, FrmShowCavityStatus) =====

app.MapPost("/api/cavities", async (CreateServiceCavityDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.CavityNo) || string.IsNullOrWhiteSpace(dto.CavityName) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần mã khoang CavityNo, tên khoang CavityName và mã đại lý DealerCode." });
    try { return Results.Ok(await svc.CreateCavityAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/cavities", async (IVehicleService svc, string? status, string? cavityType, string? dealer, string? q) =>
    Results.Ok(await svc.ListCavitiesAsync(status, cavityType, dealer, q))).RequireAuthorization();

app.MapGet("/api/cavities/summary", async (IVehicleService svc, string? dealerCode) =>
    Results.Ok(await svc.GetCavitySummaryAsync(dealerCode))).RequireAuthorization();

app.MapGet("/api/reports/cavities/summary", async (IVehicleService svc, string? dealerCode) =>
    Results.Ok(await svc.GetCavitySummaryAsync(dealerCode))).RequireAuthorization();

app.MapGet("/api/cavities/board", async (IVehicleService svc, string? dealerCode) =>
    Results.Ok(await svc.GetCavityDispatchBoardAsync(dealerCode))).RequireAuthorization();

app.MapGet("/api/cavities/dispatch-board", async (IVehicleService svc, string? dealerCode) =>
    Results.Ok(await svc.GetCavityDispatchBoardAsync(dealerCode))).RequireAuthorization();

app.MapGet("/api/cavities/{cavityNo}", async (string cavityNo, IVehicleService svc) =>
{
    var r = await svc.GetCavityAsync(cavityNo);
    return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa / cầu nâng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/cavities/{cavityNo}", async (string cavityNo, UpdateServiceCavityDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCavityAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa / cầu nâng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/cavities/{cavityNo}/update", async (string cavityNo, UpdateServiceCavityDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCavityAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa / cầu nâng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/cavities/{cavityNo}", async (string cavityNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.DeleteCavityAsync(cavityNo);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/cavities/{cavityNo}/dispatch", async (string cavityNo, DispatchVehicleToCavityDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần cung cấp số khung VIN xe để điều phối vào khoang." });
    try
    {
        var r = await svc.DispatchVehicleToCavityAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/cavities/{cavityNo}/transfer", async (string cavityNo, TransferCavityDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.TargetCavityNo))
        return Results.BadRequest(new { error = "Cần chỉ định mã khoang đích TargetCavityNo." });
    try
    {
        var r = await svc.TransferCavityAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa nguồn." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/cavities/{cavityNo}/release", async (string cavityNo, ReleaseCavityDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ReleaseCavityAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/cavities/{cavityNo}/maintenance", async (string cavityNo, SetCavityMaintenanceDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.SetCavityMaintenanceAsync(cavityNo, dto);
        return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/cavities/{cavityNo}/history", async (string cavityNo, IVehicleService svc) =>
{
    var r = await svc.GetCavityHistoryAsync(cavityNo);
    return r is null ? Results.NotFound(new { cavityNo, error = "Không tìm thấy khoang sửa chữa." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/cavity-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCavityInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/cavity-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCavityHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/cavities", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleCavityHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Bảng kê & Quyết toán chi phí Lưu kho bãi xe tồn OEM (BizHTC.Payment / Pmt_PaymentStorage & StoragePayment / FrmQuanLyThanhToanLuuKho) =====

app.MapPost("/api/storage-payments", async (CreateStoragePaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PmtMonth))
        return Results.BadRequest(new { error = "Cần tháng/kỳ quyết toán PmtMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateStoragePaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/storage-payments", async (IVehicleService svc, string? status, string? storageCode, string? pmtMonth, string? storageProvider, string? pmtStorageNo, string? vin) =>
    Results.Ok(await svc.ListStoragePaymentsAsync(status, storageCode, pmtMonth, storageProvider, pmtStorageNo, vin))).RequireAuthorization();

app.MapGet("/api/storage-payments/summary", async (IVehicleService svc, string? storageCode, string? pmtMonth) =>
    Results.Ok(await svc.GetStoragePaymentSummaryAsync(storageCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/reports/storage-payments/summary", async (IVehicleService svc, string? storageCode, string? pmtMonth) =>
    Results.Ok(await svc.GetStoragePaymentSummaryAsync(storageCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/storage-payments/{paymentStorageNo}", async (string paymentStorageNo, IVehicleService svc) =>
{
    var r = await svc.GetStoragePaymentAsync(paymentStorageNo);
    return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê quyết toán lưu kho." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/storage-payments/{paymentStorageNo}", async (string paymentStorageNo, UpdateStoragePaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateStoragePaymentHeaderAsync(paymentStorageNo, dto);
        return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê quyết toán lưu kho." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/storage-payments/{paymentStorageNo}/update", async (string paymentStorageNo, UpdateStoragePaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateStoragePaymentHeaderAsync(paymentStorageNo, dto);
        return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê quyết toán lưu kho." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/storage-payments/{paymentStorageNo}/{action}", async (string paymentStorageNo, string action, StoragePaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve1" or "approve-step1" or "approve2" or "approve" or "tcms-sign" or "tcmssign" or "sign-tcms" or "htv-sign" or "htvsign" or "sign-htv" or "settle" or "pay" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|tcms-sign|htv-sign|settle|reject|cancel" });
    try
    {
        var r = await svc.StoragePaymentTransitionAsync(paymentStorageNo, action, dto);
        return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/storage-payments/{paymentStorageNo}/lines/{vin}/update", async (string paymentStorageNo, string vin, UpdateStoragePaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateStoragePaymentLineAsync(paymentStorageNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentStorageNo, vin, error = "Không tìm thấy dòng xe trong bảng kê lưu kho." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/storage-payments/{paymentStorageNo}/lines/{vin}", async (string paymentStorageNo, string vin, UpdateStoragePaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateStoragePaymentLineAsync(paymentStorageNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentStorageNo, vin, error = "Không tìm thấy dòng xe trong bảng kê lưu kho." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/storage-payments/{paymentStorageNo}/lines", async (string paymentStorageNo, List<StoragePaymentLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bảng kê." });
    try
    {
        var r = await svc.AddStoragePaymentLinesAsync(paymentStorageNo, items);
        return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê hoặc không thể thêm xe." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/storage-payments/{paymentStorageNo}/lines/{vin}", async (string paymentStorageNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveStoragePaymentLineAsync(paymentStorageNo, vin);
        return r is null ? Results.NotFound(new { paymentStorageNo, vin, error = "Không tìm thấy dòng xe trong bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/storage-payments/{paymentStorageNo}", async (string paymentStorageNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveStoragePaymentAsync(paymentStorageNo);
        return r is null ? Results.NotFound(new { paymentStorageNo, error = "Không tìm thấy bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/storage-payment-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleStoragePaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/storage-payments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleStoragePaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/storage-payment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleStoragePaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Bảng kê & Quyết toán chi phí Màn hình AVN & Thẻ bản đồ định vị trên xe ô tô (BizHTC.Payment / Pmt_PaymentAVN & AvnPayment / FrmQuanLyThanhToanAVN, FrmTaoThanhToanAVN) =====

app.MapPost("/api/avn-payments", async (CreateAvnPaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PmtMonth))
        return Results.BadRequest(new { error = "Cần tháng/kỳ quyết toán PmtMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateAvnPaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/avn-payments", async (IVehicleService svc, string? status, string? supplierCode, string? pmtMonth, string? paymentAVNNo, string? vin) =>
    Results.Ok(await svc.ListAvnPaymentsAsync(status, supplierCode, pmtMonth, paymentAVNNo, vin))).RequireAuthorization();

app.MapGet("/api/avn-payments/summary", async (IVehicleService svc, string? supplierCode, string? pmtMonth) =>
    Results.Ok(await svc.GetAvnPaymentSummaryAsync(supplierCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/reports/avn-payments/summary", async (IVehicleService svc, string? supplierCode, string? pmtMonth) =>
    Results.Ok(await svc.GetAvnPaymentSummaryAsync(supplierCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/avn-payments/{paymentAVNNo}", async (string paymentAVNNo, IVehicleService svc) =>
{
    var r = await svc.GetAvnPaymentAsync(paymentAVNNo);
    return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê quyết toán AVN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/avn-payments/{paymentAVNNo}", async (string paymentAVNNo, UpdateAvnPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateAvnPaymentHeaderAsync(paymentAVNNo, dto);
        return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê quyết toán AVN." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/avn-payments/{paymentAVNNo}/update", async (string paymentAVNNo, UpdateAvnPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateAvnPaymentHeaderAsync(paymentAVNNo, dto);
        return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê quyết toán AVN." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/avn-payments/{paymentAVNNo}/{action}", async (string paymentAVNNo, string action, AvnPaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve1" or "approve-step1" or "approve2" or "approve" or "supplier-sign" or "suppliersign" or "sign-supplier" or "htv-sign" or "htvsign" or "sign-htv" or "settle" or "pay" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|supplier-sign|htv-sign|settle|reject|cancel" });
    try
    {
        var r = await svc.AvnPaymentTransitionAsync(paymentAVNNo, action, dto);
        return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/avn-payments/{paymentAVNNo}/lines/{vin}/update", async (string paymentAVNNo, string vin, UpdateAvnPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateAvnPaymentLineAsync(paymentAVNNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentAVNNo, vin, error = "Không tìm thấy dòng xe trong bảng kê AVN." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/avn-payments/{paymentAVNNo}/lines/{vin}", async (string paymentAVNNo, string vin, UpdateAvnPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateAvnPaymentLineAsync(paymentAVNNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentAVNNo, vin, error = "Không tìm thấy dòng xe trong bảng kê AVN." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/avn-payments/{paymentAVNNo}/lines", async (string paymentAVNNo, List<AvnPaymentLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bảng kê." });
    try
    {
        var r = await svc.AddAvnPaymentLinesAsync(paymentAVNNo, items);
        return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê hoặc không thể thêm xe." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/avn-payments/{paymentAVNNo}/lines/{vin}", async (string paymentAVNNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveAvnPaymentLineAsync(paymentAVNNo, vin);
        return r is null ? Results.NotFound(new { paymentAVNNo, vin, error = "Không tìm thấy dòng xe trong bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/avn-payments/{paymentAVNNo}", async (string paymentAVNNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveAvnPaymentAsync(paymentAVNNo);
        return r is null ? Results.NotFound(new { paymentAVNNo, error = "Không tìm thấy bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/avn-payment-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAvnPaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/avn-payments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAvnPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/avn-payment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAvnPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Đăng ký & Nhật ký Khách hàng Lái thử xe (BizHTC.RetailContract / DLR_DriveTest, Mst_CarDriverTest / FrmMngTestDriver, FrmNewTestDriver) =====

app.MapPost("/api/test-drives", async (CreateCustomerTestDriveDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.PhoneNo) || string.IsNullOrWhiteSpace(dto.DriverLicenseNo) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode, họ tên khách hàng FullName, số điện thoại PhoneNo, số GPLX DriverLicenseNo và số khung VIN." });
    try { return Results.Ok(await svc.CreateCustomerTestDriveAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/test-drives", async (IVehicleService svc, string? status, string? dealer, string? driveTestType, string? purchaseIntent, string? model, string? vin, string? driveTestCode, string? phoneNo) =>
    Results.Ok(await svc.ListCustomerTestDrivesAsync(status, dealer, driveTestType, purchaseIntent, model, vin, driveTestCode, phoneNo))).RequireAuthorization();

app.MapGet("/api/test-drives/summary", async (IVehicleService svc, string? dealer, string? driveTestType) =>
    Results.Ok(await svc.GetTestDriveSummaryAsync(dealer, driveTestType))).RequireAuthorization();

app.MapGet("/api/reports/test-drives/summary", async (IVehicleService svc, string? dealer, string? driveTestType) =>
    Results.Ok(await svc.GetTestDriveSummaryAsync(dealer, driveTestType))).RequireAuthorization();

app.MapGet("/api/test-drives/available-cars", async (IVehicleService svc, string? dealer, string? model) =>
    Results.Ok(await svc.GetAvailableTestDriveVehiclesAsync(dealer, model))).RequireAuthorization();

app.MapGet("/api/test-drives/{driveTestCode}", async (string driveTestCode, IVehicleService svc) =>
{
    var r = await svc.GetCustomerTestDriveAsync(driveTestCode);
    return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu đăng ký lái thử." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/test-drives/{driveTestCode}", async (string driveTestCode, UpdateCustomerTestDriveDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCustomerTestDriveAsync(driveTestCode, dto);
        return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu đăng ký lái thử." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/test-drives/{driveTestCode}/update", async (string driveTestCode, UpdateCustomerTestDriveDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCustomerTestDriveAsync(driveTestCode, dto);
        return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu đăng ký lái thử." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/test-drives/{driveTestCode}/feedback", async (string driveTestCode, RecordTestDriveFeedbackDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RecordCustomerTestDriveFeedbackAsync(driveTestCode, dto);
        return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu đăng ký lái thử." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/test-drives/{driveTestCode}/{action}", async (string driveTestCode, string action, CustomerTestDriveTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("schedule" or "approve" or "start" or "in-progress" or "inprogress" or "complete" or "finish" or "noshow" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = schedule|approve|start|complete|noshow|reject|cancel" });
    try
    {
        var r = await svc.CustomerTestDriveTransitionAsync(driveTestCode, action, dto);
        return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu đăng ký lái thử hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/test-drives/{driveTestCode}", async (string driveTestCode, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveCustomerTestDriveAsync(driveTestCode);
        return r is null ? Results.NotFound(new { driveTestCode, error = "Không tìm thấy phiếu lái thử hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/test-drives", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTestDriveHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/test-drive-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTestDriveInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/test-drive-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTestDriveHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Kế hoạch Điều độ Vận tải & Phân bổ Xe ô tô OEM (BizHTC.Storage / Sto_TranspPlan / TransportPlan) ----
app.MapPost("/api/transport-plans", async (CreateTransportPlanDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateTransportPlanAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/transport-plans", async (IVehicleService svc, string? status, string? month, string? storageCode, string? tpType, string? planNo, string? vin, string? dealerCode, string? transporterCode) =>
    Results.Ok(await svc.ListTransportPlansAsync(status, month, storageCode, tpType, planNo, vin, dealerCode, transporterCode))).RequireAuthorization();

app.MapGet("/api/transport-plans/summary", async (IVehicleService svc, string? planMonth, string? storageCode) =>
    Results.Ok(await svc.GetTransportPlanSummaryAsync(planMonth, storageCode))).RequireAuthorization();

app.MapGet("/api/transport-plans/{planNo}", async (string planNo, IVehicleService svc) =>
{
    var r = await svc.GetTransportPlanAsync(planNo);
    return r is null ? Results.NotFound(new { planNo, error = "Không tìm thấy kế hoạch điều độ vận tải." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/transport-plans/{planNo}", async (string planNo, UpdateTransportPlanDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportPlanHeaderAsync(planNo, dto);
        return r is null ? Results.NotFound(new { planNo, error = "Không tìm thấy kế hoạch điều độ hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/lines", async (string planNo, AddTransportPlanLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.AddTransportPlanLineAsync(planNo, dto);
        return r is null ? Results.NotFound(new { planNo, error = "Không tìm thấy kế hoạch điều độ hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/transport-plans/{planNo}/lines/{lineId:long}/kehoach", async (string planNo, long lineId, UpdateTransportPlanLineByKeHoachDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportPlanLineByKeHoachAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/transport-plans/{planNo}/lines/{lineId:long}/banhang", async (string planNo, long lineId, UpdateTransportPlanLineByBanHangDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportPlanLineByBanHangAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/transport-plans/{planNo}/lines/{lineId:long}/logistic", async (string planNo, long lineId, UpdateTransportPlanLineByLogisticDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportPlanLineByLogisticAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/lines/{lineId:long}/map-vin", async (string planNo, long lineId, MapVinRealDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.MapVinRealAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/map-vins", async (string planNo, MapVinRealBatchDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.MapVinRealBatchAsync(planNo, dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/lines/{lineId:long}/unmap-vin", async (string planNo, long lineId, UnmapVinRealDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UnmapVinRealAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/lines/{lineId:long}/transporter-approve", async (string planNo, long lineId, TransporterApproveLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.TransporterApproveLineAsync(planNo, lineId, dto);
        return r is null ? Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng xe trong kế hoạch." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-plans/{planNo}/{action}", async (string planNo, string action, TransportPlanTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "execute" or "complete" or "finish" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|execute|complete|reject|cancel" });
    try
    {
        var r = await svc.TransportPlanTransitionAsync(planNo, action, dto);
        return r is null ? Results.NotFound(new { planNo, error = "Không tìm thấy kế hoạch điều độ hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/transport-plans/{planNo}/lines/{lineId:long}", async (string planNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var ok = await svc.RemoveTransportPlanLineAsync(planNo, lineId);
        return ok ? Results.Ok(new { success = true, planNo, lineId }) : Results.NotFound(new { planNo, lineId, error = "Không tìm thấy dòng hoặc kế hoạch không ở trạng thái cho phép xóa." });
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/transport-plans/{planNo}", async (string planNo, IVehicleService svc) =>
{
    try
    {
        var ok = await svc.RemoveTransportPlanAsync(planNo);
        return ok ? Results.Ok(new { success = true, planNo }) : Results.NotFound(new { planNo, error = "Không tìm thấy kế hoạch hoặc kế hoạch không ở trạng thái cho phép xóa." });
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/transport-plan-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTransportPlanInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/transport-plan-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTransportPlanHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Bảng kê & Quyết toán chi phí Mua sắm/Thuê thiết bị định vị GPS & Dịch vụ SIM 4G data viễn thông theo lô xe VIN (BizHTC.Payment / Pmt_PaymentGPS & GpsPayment / FrmQuanLyThanhToanGPS, FrmTaoThanhToanGPS) =====

app.MapPost("/api/gps-payments", async (CreateGpsPaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PmtMonth))
        return Results.BadRequest(new { error = "Cần tháng/kỳ quyết toán PmtMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateGpsPaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/gps-payments", async (IVehicleService svc, string? status, string? supplierCode, string? pmtMonth, string? paymentGPSNo, string? vin) =>
    Results.Ok(await svc.ListGpsPaymentsAsync(status, supplierCode, pmtMonth, paymentGPSNo, vin))).RequireAuthorization();

app.MapGet("/api/gps-payments/summary", async (IVehicleService svc, string? supplierCode, string? pmtMonth) =>
    Results.Ok(await svc.GetGpsPaymentSummaryAsync(supplierCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/reports/gps-payments/summary", async (IVehicleService svc, string? supplierCode, string? pmtMonth) =>
    Results.Ok(await svc.GetGpsPaymentSummaryAsync(supplierCode, pmtMonth))).RequireAuthorization();

app.MapGet("/api/gps-payments/{paymentGPSNo}", async (string paymentGPSNo, IVehicleService svc) =>
{
    var r = await svc.GetGpsPaymentAsync(paymentGPSNo);
    return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê quyết toán GPS." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/gps-payments/{paymentGPSNo}", async (string paymentGPSNo, UpdateGpsPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsPaymentHeaderAsync(paymentGPSNo, dto);
        return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê quyết toán GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps-payments/{paymentGPSNo}/update", async (string paymentGPSNo, UpdateGpsPaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsPaymentHeaderAsync(paymentGPSNo, dto);
        return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê quyết toán GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps-payments/{paymentGPSNo}/{action}", async (string paymentGPSNo, string action, GpsPaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve1" or "approve-step1" or "approve2" or "approve" or "tcms-sign" or "tcmssign" or "sign-tcms" or "htv-sign" or "htvsign" or "sign-htv" or "settle" or "pay" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|tcms-sign|htv-sign|settle|reject|cancel" });
    try
    {
        var r = await svc.GpsPaymentTransitionAsync(paymentGPSNo, action, dto);
        return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps-payments/{paymentGPSNo}/lines/{vin}/update", async (string paymentGPSNo, string vin, UpdateGpsPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsPaymentLineAsync(paymentGPSNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentGPSNo, vin, error = "Không tìm thấy dòng xe trong bảng kê GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/gps-payments/{paymentGPSNo}/lines/{vin}", async (string paymentGPSNo, string vin, UpdateGpsPaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateGpsPaymentLineAsync(paymentGPSNo, vin, dto);
        return r is null ? Results.NotFound(new { paymentGPSNo, vin, error = "Không tìm thấy dòng xe trong bảng kê GPS." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/gps-payments/{paymentGPSNo}/lines", async (string paymentGPSNo, List<GpsPaymentLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bảng kê." });
    try
    {
        var r = await svc.AddGpsPaymentLinesAsync(paymentGPSNo, items);
        return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê hoặc không thể thêm xe." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps-payments/{paymentGPSNo}/lines/{vin}", async (string paymentGPSNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsPaymentLineAsync(paymentGPSNo, vin);
        return r is null ? Results.NotFound(new { paymentGPSNo, vin, error = "Không tìm thấy dòng xe trong bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/gps-payments/{paymentGPSNo}", async (string paymentGPSNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveGpsPaymentAsync(paymentGPSNo);
        return r is null ? Results.NotFound(new { paymentGPSNo, error = "Không tìm thấy bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/gps-payment-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGpsPaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/gps-payments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGpsPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/gps-payment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleGpsPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Bảng kê & Quyết toán chi phí Vận tải & Bảo hiểm xe ô tô vận chuyển theo lô VIN (BizHTC.Payment / Pmt_TransportIns & TransportInsurancePayment) =====

app.MapPost("/api/transport-insurance-payments", async (CreateTransportInsurancePaymentDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PmtMonth))
        return Results.BadRequest(new { error = "Cần tháng/kỳ quyết toán PmtMonth (YYYY-MM)." });
    try { return Results.Ok(await svc.CreateTransportInsurancePaymentAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/transport-insurance-payments", async (IVehicleService svc, string? status, string? transporterCode, string? insuranceCompanyCode, string? pmtMonth, string? transportInsNo, string? vin) =>
    Results.Ok(await svc.ListTransportInsurancePaymentsAsync(status, transporterCode, insuranceCompanyCode, pmtMonth, transportInsNo, vin))).RequireAuthorization();

app.MapGet("/api/transport-insurance-payments/summary", async (IVehicleService svc, string? transporterCode, string? pmtMonth, string? insuranceCompanyCode) =>
    Results.Ok(await svc.GetTransportInsurancePaymentSummaryAsync(transporterCode, pmtMonth, insuranceCompanyCode))).RequireAuthorization();

app.MapGet("/api/reports/transport-insurance-payments/summary", async (IVehicleService svc, string? transporterCode, string? pmtMonth, string? insuranceCompanyCode) =>
    Results.Ok(await svc.GetTransportInsurancePaymentSummaryAsync(transporterCode, pmtMonth, insuranceCompanyCode))).RequireAuthorization();

app.MapGet("/api/transport-insurance-payments/{transportInsNo}", async (string transportInsNo, IVehicleService svc) =>
{
    var r = await svc.GetTransportInsurancePaymentAsync(transportInsNo);
    return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê quyết toán vận tải & bảo hiểm." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/transport-insurance-payments/{transportInsNo}", async (string transportInsNo, UpdateTransportInsurancePaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportInsurancePaymentHeaderAsync(transportInsNo, dto);
        return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê quyết toán vận tải & bảo hiểm." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-insurance-payments/{transportInsNo}/update", async (string transportInsNo, UpdateTransportInsurancePaymentHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportInsurancePaymentHeaderAsync(transportInsNo, dto);
        return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê quyết toán vận tải & bảo hiểm." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-insurance-payments/{transportInsNo}/{action}", async (string transportInsNo, string action, TransportInsurancePaymentTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "request" or "approve1" or "approve-step1" or "approve2" or "approve" or "transporter-sign" or "transportersign" or "sign-transporter" or "htv-sign" or "htvsign" or "sign-htv" or "settle" or "pay" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve1|approve2|transporter-sign|htv-sign|settle|reject|cancel" });
    try
    {
        var r = await svc.TransportInsurancePaymentTransitionAsync(transportInsNo, action, dto);
        return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-insurance-payments/{transportInsNo}/lines/{vin}/update", async (string transportInsNo, string vin, UpdateTransportInsurancePaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportInsurancePaymentLineAsync(transportInsNo, vin, dto);
        return r is null ? Results.NotFound(new { transportInsNo, vin, error = "Không tìm thấy dòng xe trong bảng kê." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/transport-insurance-payments/{transportInsNo}/lines/{vin}", async (string transportInsNo, string vin, UpdateTransportInsurancePaymentLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTransportInsurancePaymentLineAsync(transportInsNo, vin, dto);
        return r is null ? Results.NotFound(new { transportInsNo, vin, error = "Không tìm thấy dòng xe trong bảng kê." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/transport-insurance-payments/{transportInsNo}/lines", async (string transportInsNo, List<TransportInsurancePaymentLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách items xe để thêm vào bảng kê." });
    try
    {
        var r = await svc.AddTransportInsurancePaymentLinesAsync(transportInsNo, items);
        return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê hoặc không thể thêm xe." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/transport-insurance-payments/{transportInsNo}/lines/{vin}", async (string transportInsNo, string vin, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveTransportInsurancePaymentLineAsync(transportInsNo, vin);
        return r is null ? Results.NotFound(new { transportInsNo, vin, error = "Không tìm thấy dòng xe trong bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/transport-insurance-payments/{transportInsNo}", async (string transportInsNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveTransportInsurancePaymentAsync(transportInsNo);
        return r is null ? Results.NotFound(new { transportInsNo, error = "Không tìm thấy bảng kê hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/transport-insurance-payment-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTranspInsPaymentInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/transport-insurance-payments", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTranspInsPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/transport-insurance-payment-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleTranspInsPaymentHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Quản lý Định mức Tồn kho An toàn & Cân đối Tồn kho Đại lý OEM (BizHTC.MasterData & BizHTC.StorageFG / Mst_DealerInventoryThreshold, Mst_MinInventory, St_MinInvBalance / FrmMstSalesInventoryThreshold, FrmSt_MinInvBalance) ----
app.MapPost("/api/inventory-thresholds", async (CreateDealerInventoryThresholdDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.Model))
        return Results.BadRequest(new { error = "Cần mã đại lý DealerCode và dòng xe Model." });
    try { return Results.Ok(await svc.CreateDealerInventoryThresholdAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/inventory-thresholds/batch", async (BatchCreateDealerInventoryThresholdDto dto, IVehicleService svc) =>
{
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách định mức tồn kho Items." });
    try { return Results.Ok(await svc.BatchCreateDealerInventoryThresholdsAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/inventory-thresholds", async (IVehicleService svc, string? status, string? dealer, string? model, int? month, int? year, string? thresholdNo) =>
    Results.Ok(await svc.ListDealerInventoryThresholdsAsync(status, dealer, model, month, year, thresholdNo))).RequireAuthorization();

app.MapGet("/api/inventory-thresholds/summary", async (IVehicleService svc, int? month, int? year, string? region) =>
    Results.Ok(await svc.GetDealerInventoryThresholdSummaryAsync(month, year, region))).RequireAuthorization();

app.MapGet("/api/reports/inventory-thresholds/summary", async (IVehicleService svc, int? month, int? year, string? region) =>
    Results.Ok(await svc.GetDealerInventoryThresholdSummaryAsync(month, year, region))).RequireAuthorization();

app.MapGet("/api/inventory-thresholds/health-report", async (IVehicleService svc, string? dealer, string? model, string? region) =>
    Results.Ok(await svc.GetDealerStockHealthReportAsync(dealer, model, region))).RequireAuthorization();

app.MapGet("/api/inventory-thresholds/rebalance-suggestions", async (IVehicleService svc, string? model) =>
    Results.Ok(await svc.GetStockRebalanceSuggestionsAsync(model))).RequireAuthorization();

app.MapPost("/api/inventory-thresholds/audit", async (RunInventoryAuditDto? dto, IVehicleService svc) =>
    Results.Ok(await svc.RunInventoryAuditAsync(dto))).RequireAuthorization();

app.MapGet("/api/inventory-thresholds/audit/records", async (IVehicleService svc, string? dealer, string? model, string? healthStatus, string? auditNo) =>
    Results.Ok(await svc.ListInventoryAuditRecordsAsync(dealer, model, healthStatus, auditNo))).RequireAuthorization();

app.MapGet("/api/inventory-thresholds/{thresholdNo}", async (string thresholdNo, IVehicleService svc) =>
{
    var r = await svc.GetDealerInventoryThresholdAsync(thresholdNo);
    return r is null ? Results.NotFound(new { thresholdNo, error = "Không tìm thấy định mức tồn kho." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/inventory-thresholds/{thresholdNo}", async (string thresholdNo, UpdateDealerInventoryThresholdDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDealerInventoryThresholdAsync(thresholdNo, dto);
        return r is null ? Results.NotFound(new { thresholdNo, error = "Không tìm thấy định mức tồn kho hoặc định mức không ở trạng thái Draft." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/inventory-thresholds/{thresholdNo}/update", async (string thresholdNo, UpdateDealerInventoryThresholdDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDealerInventoryThresholdAsync(thresholdNo, dto);
        return r is null ? Results.NotFound(new { thresholdNo, error = "Không tìm thấy định mức tồn kho hoặc định mức không ở trạng thái Draft." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/inventory-thresholds/{thresholdNo}/{action}", async (string thresholdNo, string action, DealerInventoryThresholdTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "activate" or "approve" or "suspend" or "resume" or "expire" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|activate|approve|suspend|resume|expire|cancel" });
    try
    {
        var r = await svc.DealerInventoryThresholdTransitionAsync(thresholdNo, action, dto);
        return r is null ? Results.NotFound(new { thresholdNo, error = "Không tìm thấy định mức tồn kho hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/inventory-thresholds/{thresholdNo}", async (string thresholdNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveDealerInventoryThresholdAsync(thresholdNo);
        return r is null ? Results.NotFound(new { thresholdNo, error = "Không tìm thấy định mức tồn kho hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/inventory-threshold-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleInventoryThresholdInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/inventory-thresholds", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleInventoryThresholdHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/inventory-threshold-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleInventoryThresholdHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Đào tạo, Sát hạch & Cấp Chứng chỉ Chuẩn hóa Nhân sự Đại lý (BizHTC.MasterData / Mst_Training & Mst_SalesManCertificate / TrainingCourse & StaffCertificate) =====

app.MapPost("/api/trainings", async (CreateTrainingCourseDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.CourseName))
        return Results.BadRequest(new { error = "Cần tên khóa đào tạo CourseName." });
    try { return Results.Ok(await svc.CreateTrainingCourseAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/trainings", async (IVehicleService svc, string? status, string? trainingType, string? level, string? trainingCode, string? q) =>
    Results.Ok(await svc.ListTrainingCoursesAsync(status, trainingType, level, trainingCode, q))).RequireAuthorization();

app.MapGet("/api/trainings/summary", async (IVehicleService svc, int? year, string? trainingType) =>
    Results.Ok(await svc.GetTrainingSummaryAsync(year, trainingType))).RequireAuthorization();

app.MapGet("/api/reports/trainings/summary", async (IVehicleService svc, int? year, string? trainingType) =>
    Results.Ok(await svc.GetTrainingSummaryAsync(year, trainingType))).RequireAuthorization();

app.MapGet("/api/trainings/dealer-matrix", async (IVehicleService svc, string? dealerCode) =>
    Results.Ok(await svc.GetDealerTrainingMatrixAsync(dealerCode))).RequireAuthorization();

app.MapGet("/api/trainings/staff/{staffCode}", async (string staffCode, IVehicleService svc) =>
{
    var r = await svc.GetStaffTrainingProfileAsync(staffCode);
    return r is null ? Results.NotFound(new { staffCode, error = "Không tìm thấy hồ sơ đào tạo và chứng chỉ của nhân viên." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/trainings/{trainingCode}", async (string trainingCode, IVehicleService svc) =>
{
    var r = await svc.GetTrainingCourseAsync(trainingCode);
    return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/trainings/{trainingCode}", async (string trainingCode, UpdateTrainingCourseHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTrainingCourseHeaderAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/update", async (string trainingCode, UpdateTrainingCourseHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateTrainingCourseHeaderAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/{action}", async (string trainingCode, string action, TrainingCourseTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "schedule" or "approve" or "start" or "in-progress" or "inprogress" or "complete" or "finish" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|schedule|approve|start|complete|cancel" });
    try
    {
        var r = await svc.TrainingCourseTransitionAsync(trainingCode, action, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/enroll", async (string trainingCode, EnrollStaffDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.StaffCode) || string.IsNullOrWhiteSpace(dto.StaffName))
        return Results.BadRequest(new { error = "Cần mã nhân viên StaffCode và họ tên StaffName." });
    try
    {
        var r = await svc.EnrollStaffAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/batch-enroll", async (string trainingCode, BatchEnrollStaffDto dto, IVehicleService svc) =>
{
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách học viên trong Items." });
    try
    {
        var r = await svc.BatchEnrollStaffAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/trainings/{trainingCode}/enrollments/{enrollmentNo}", async (string trainingCode, string enrollmentNo, UpdateEnrollmentDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateEnrollmentAsync(trainingCode, enrollmentNo, dto);
    return r is null ? Results.NotFound(new { trainingCode, enrollmentNo, error = "Không tìm thấy lượt ghi danh của học viên." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/enrollments/{enrollmentNo}/grade", async (string trainingCode, string enrollmentNo, GradeEnrollmentDto dto, IVehicleService svc) =>
{
    var r = await svc.GradeEnrollmentAsync(trainingCode, enrollmentNo, dto);
    return r is null ? Results.NotFound(new { trainingCode, enrollmentNo, error = "Không tìm thấy lượt ghi danh của học viên." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/batch-grade", async (string trainingCode, BatchGradeEnrollmentDto dto, IVehicleService svc) =>
{
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách học viên chấm điểm trong Items." });
    try
    {
        var r = await svc.BatchGradeEnrollmentsAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/trainings/{trainingCode}/enrollments/{enrollmentNo}", async (string trainingCode, string enrollmentNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveEnrollmentAsync(trainingCode, enrollmentNo);
        return r is null ? Results.NotFound(new { trainingCode, enrollmentNo, error = "Không tìm thấy lượt ghi danh của học viên hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/trainings/{trainingCode}/auto-issue-certificates", async (string trainingCode, AutoIssueCertificatesDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.AutoIssueCertificatesAsync(trainingCode, dto);
        return r is null ? Results.NotFound(new { trainingCode, error = "Không tìm thấy khóa đào tạo." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/certificates", async (IVehicleService svc, string? status, string? dealer, string? certType, string? staffCode, string? certNo) =>
    Results.Ok(await svc.ListStaffCertificatesAsync(status, dealer, certType, staffCode, certNo))).RequireAuthorization();

app.MapGet("/api/certificates/{certNo}", async (string certNo, IVehicleService svc) =>
{
    var r = await svc.GetStaffCertificateAsync(certNo);
    return r is null ? Results.NotFound(new { certNo, error = "Không tìm thấy chứng chỉ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/certificates", async (CreateStaffCertificateDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.StaffCode) || string.IsNullOrWhiteSpace(dto.StaffName))
        return Results.BadRequest(new { error = "Cần mã nhân viên StaffCode và họ tên StaffName." });
    try { return Results.Ok(await svc.CreateStaffCertificateAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/certificates/{certNo}", async (string certNo, UpdateStaffCertificateDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateStaffCertificateAsync(certNo, dto);
    return r is null ? Results.NotFound(new { certNo, error = "Không tìm thấy chứng chỉ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/certificates/{certNo}/update", async (string certNo, UpdateStaffCertificateDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateStaffCertificateAsync(certNo, dto);
    return r is null ? Results.NotFound(new { certNo, error = "Không tìm thấy chứng chỉ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/certificates/{certNo}/{action}", async (string certNo, string action, StaffCertificateTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("activate" or "renew" or "suspend" or "expire" or "revoke"))
        return Results.BadRequest(new { error = "action = activate|renew|suspend|expire|revoke" });
    try
    {
        var r = await svc.StaffCertificateTransitionAsync(certNo, action, dto);
        return r is null ? Results.NotFound(new { certNo, error = "Không tìm thấy chứng chỉ hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

// ===== Quản lý Gói Dịch Vụ & Thẻ Bảo Dưỡng Trọn Gói Xe Ô Tô (BizCarSv.ServicePackage / Ser_ServicePackage) =====

app.MapPost("/api/service-packages", async (CreateServicePackageDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PackageName))
        return Results.BadRequest(new { error = "Cần tên gói dịch vụ PackageName." });
    try { return Results.Ok(await svc.CreateServicePackageAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/service-packages", async (IVehicleService svc, string? status, string? dealer, string? packageType, string? model, string? packageNo) =>
    Results.Ok(await svc.ListServicePackagesAsync(status, dealer, packageType, model, packageNo))).RequireAuthorization();

app.MapGet("/api/service-packages/summary", async (IVehicleService svc, string? dealerCode, string? packageType) =>
    Results.Ok(await svc.GetServicePackageSummaryAsync(dealerCode, packageType))).RequireAuthorization();

app.MapGet("/api/reports/service-packages/summary", async (IVehicleService svc, string? dealerCode, string? packageType) =>
    Results.Ok(await svc.GetServicePackageSummaryAsync(dealerCode, packageType))).RequireAuthorization();

app.MapGet("/api/service-packages/{packageNo}", async (string packageNo, IVehicleService svc) =>
{
    var r = await svc.GetServicePackageAsync(packageNo);
    return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/service-packages/{packageNo}", async (string packageNo, UpdateServicePackageHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageHeaderAsync(packageNo, dto);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/update", async (string packageNo, UpdateServicePackageHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageHeaderAsync(packageNo, dto);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/{action}", async (string packageNo, string action, ServicePackageTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "activate" or "active" or "suspend" or "resume" or "reactivate" or "archive" or "draft"))
        return Results.BadRequest(new { error = "action = approve|activate|suspend|resume|archive|draft" });
    try
    {
        var r = await svc.ServicePackageTransitionAsync(packageNo, action, dto);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-packages/{packageNo}", async (string packageNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServicePackageAsync(packageNo);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/labor-lines", async (string packageNo, List<ServicePackageLaborLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách hạng mục công việc labor lines." });
    try
    {
        var r = await svc.AddServicePackageLaborLinesAsync(packageNo, items);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/labor-lines/{lineId:long}/update", async (string packageNo, long lineId, UpdateServicePackageLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageLaborLineAsync(packageNo, lineId, dto);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng công việc trong gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/service-packages/{packageNo}/labor-lines/{lineId:long}", async (string packageNo, long lineId, UpdateServicePackageLaborLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageLaborLineAsync(packageNo, lineId, dto);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng công việc trong gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-packages/{packageNo}/labor-lines/{lineId:long}", async (string packageNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServicePackageLaborLineAsync(packageNo, lineId);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng công việc." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/part-lines", async (string packageNo, List<ServicePackagePartLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách phụ tùng part lines." });
    try
    {
        var r = await svc.AddServicePackagePartLinesAsync(packageNo, items);
        return r is null ? Results.NotFound(new { packageNo, error = "Không tìm thấy gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/{packageNo}/part-lines/{lineId:long}/update", async (string packageNo, long lineId, UpdateServicePackagePartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackagePartLineAsync(packageNo, lineId, dto);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng phụ tùng trong gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/service-packages/{packageNo}/part-lines/{lineId:long}", async (string packageNo, long lineId, UpdateServicePackagePartLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackagePartLineAsync(packageNo, lineId, dto);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng phụ tùng trong gói dịch vụ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-packages/{packageNo}/part-lines/{lineId:long}", async (string packageNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServicePackagePartLineAsync(packageNo, lineId);
        return r is null ? Results.NotFound(new { packageNo, lineId, error = "Không tìm thấy dòng phụ tùng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/subscriptions", async (SubscribeServicePackageDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PackageNo) || string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần mã gói PackageNo và số khung VIN của xe." });
    try { return Results.Ok(await svc.SubscribeServicePackageAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/service-packages/subscriptions", async (IVehicleService svc, string? status, string? dealer, string? packageNo, string? vin, string? cardNo, string? subNo) =>
    Results.Ok(await svc.ListServicePackageSubscriptionsAsync(status, dealer, packageNo, vin, cardNo, subNo))).RequireAuthorization();

app.MapGet("/api/service-packages/subscriptions/{subNo}", async (string subNo, IVehicleService svc) =>
{
    var r = await svc.GetServicePackageSubscriptionAsync(subNo);
    return r is null ? Results.NotFound(new { subNo, error = "Không tìm thấy hợp đồng thẻ bảo dưỡng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/service-packages/subscriptions/{subNo}", async (string subNo, UpdateServicePackageSubscriptionDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageSubscriptionAsync(subNo, dto);
        return r is null ? Results.NotFound(new { subNo, error = "Không tìm thấy hợp đồng thẻ bảo dưỡng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/subscriptions/{subNo}/update", async (string subNo, UpdateServicePackageSubscriptionDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateServicePackageSubscriptionAsync(subNo, dto);
        return r is null ? Results.NotFound(new { subNo, error = "Không tìm thấy hợp đồng thẻ bảo dưỡng." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/subscriptions/{subNo}/{action}", async (string subNo, string action, ServicePackageSubscriptionTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("activate" or "active" or "suspend" or "resume" or "reactivate" or "renew" or "extend" or "cancel"))
        return Results.BadRequest(new { error = "action = activate|suspend|resume|renew|cancel" });
    try
    {
        var r = await svc.ServicePackageSubscriptionTransitionAsync(subNo, action, dto);
        return r is null ? Results.NotFound(new { subNo, error = "Không tìm thấy hợp đồng thẻ hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/service-packages/subscriptions/{subNo}", async (string subNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveServicePackageSubscriptionAsync(subNo);
        return r is null ? Results.NotFound(new { subNo, error = "Không tìm thấy hợp đồng thẻ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/usages", async (RecordServicePackageUsageDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.SubscriptionNo) && string.IsNullOrWhiteSpace(dto.PackageCardNo))
        return Results.BadRequest(new { error = "Cần mã hợp đồng SubscriptionNo hoặc mã thẻ PackageCardNo." });
    try { return Results.Ok(await svc.RecordServicePackageUsageAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/service-packages/usages", async (IVehicleService svc, string? status, string? dealer, string? packageNo, string? vin, string? cardNo, string? usageNo, string? roNo) =>
    Results.Ok(await svc.ListServicePackageUsagesAsync(status, dealer, packageNo, vin, cardNo, usageNo, roNo))).RequireAuthorization();

app.MapGet("/api/service-packages/usages/{usageNo}", async (string usageNo, IVehicleService svc) =>
{
    var r = await svc.GetServicePackageUsageAsync(usageNo);
    return r is null ? Results.NotFound(new { usageNo, error = "Không tìm thấy lượt sử dụng gói dịch vụ." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/service-packages/usages/{usageNo}/{action}", async (string usageNo, string action, ServicePackageUsageTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("confirm" or "complete" or "cancel"))
        return Results.BadRequest(new { error = "action = confirm|complete|cancel" });
    try
    {
        var r = await svc.ServicePackageUsageTransitionAsync(usageNo, action, dto);
        return r is null ? Results.NotFound(new { usageNo, error = "Không tìm thấy lượt sử dụng hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/service-packages/usages/{usageNo}/feedback", async (string usageNo, RecordServicePackageUsageFeedbackDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RecordServicePackageUsageFeedbackAsync(usageNo, dto);
        return r is null ? Results.NotFound(new { usageNo, error = "Không tìm thấy lượt sử dụng gói." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/service-packages", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleServicePackageHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/service-package-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleServicePackageInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/service-package-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleServicePackageHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Cấu hình Điều kiện & Tự động Phân bổ Sinh Lệnh Giao Xe DO Tự Động (BizHTC.Car / Car_ConditionForDOAuto, Mst_DOATCondition, Car_DeliveryOrderAuto) =====

app.MapPost("/api/auto-delivery-conditions", async (CreateDOAutoConditionDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.ConditionName))
        return Results.BadRequest(new { error = "Cần tên cấu hình điều kiện ConditionName." });
    try { return Results.Ok(await svc.CreateDOAutoConditionAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/auto-delivery-conditions", async (IVehicleService svc, string? status, string? priorityRule, string? conditionCode, string? q) =>
    Results.Ok(await svc.ListDOAutoConditionsAsync(status, priorityRule, conditionCode, q))).RequireAuthorization();

app.MapGet("/api/auto-delivery-conditions/{conditionCode}", async (string conditionCode, IVehicleService svc) =>
{
    var r = await svc.GetDOAutoConditionAsync(conditionCode);
    return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện giao xe tự động." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/auto-delivery-conditions/{conditionCode}", async (string conditionCode, UpdateDOAutoConditionHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionHeaderAsync(conditionCode, dto);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/update", async (string conditionCode, UpdateDOAutoConditionHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionHeaderAsync(conditionCode, dto);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/{action}", async (string conditionCode, string action, DOAutoConditionTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve" or "activate" or "active" or "suspend" or "resume" or "reactivate" or "expire" or "draft" or "cancel"))
        return Results.BadRequest(new { error = "action = approve|activate|suspend|resume|expire|draft|cancel" });
    try
    {
        var r = await svc.DOAutoConditionTransitionAsync(conditionCode, action, dto);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/auto-delivery-conditions/{conditionCode}", async (string conditionCode, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveDOAutoConditionAsync(conditionCode);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/model-lines", async (string conditionCode, List<DOAutoConditionLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách model dòng xe áp dụng." });
    try
    {
        var r = await svc.AddDOAutoConditionLinesAsync(conditionCode, items);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/model-lines/{lineId:long}/update", async (string conditionCode, long lineId, UpdateDOAutoConditionLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionLineAsync(conditionCode, lineId, dto);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng model." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/auto-delivery-conditions/{conditionCode}/model-lines/{lineId:long}", async (string conditionCode, long lineId, UpdateDOAutoConditionLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionLineAsync(conditionCode, lineId, dto);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng model." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/auto-delivery-conditions/{conditionCode}/model-lines/{lineId:long}", async (string conditionCode, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveDOAutoConditionLineAsync(conditionCode, lineId);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng model." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/dealer-lines", async (string conditionCode, List<DOAutoConditionDealerLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách đại lý áp dụng." });
    try
    {
        var r = await svc.AddDOAutoConditionDealerLinesAsync(conditionCode, items);
        return r is null ? Results.NotFound(new { conditionCode, error = "Không tìm thấy cấu hình điều kiện." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-delivery-conditions/{conditionCode}/dealer-lines/{lineId:long}/update", async (string conditionCode, long lineId, UpdateDOAutoConditionDealerLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionDealerLineAsync(conditionCode, lineId, dto);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng đại lý." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/auto-delivery-conditions/{conditionCode}/dealer-lines/{lineId:long}", async (string conditionCode, long lineId, UpdateDOAutoConditionDealerLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDOAutoConditionDealerLineAsync(conditionCode, lineId, dto);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng đại lý." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/auto-delivery-conditions/{conditionCode}/dealer-lines/{lineId:long}", async (string conditionCode, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveDOAutoConditionDealerLineAsync(conditionCode, lineId);
        return r is null ? Results.NotFound(new { conditionCode, lineId, error = "Không tìm thấy dòng đại lý." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

// ---- Quản lý Đợt Chạy Phân Bổ & Sinh Lệnh Giao Xe DO Tự Động ----

app.MapPost("/api/auto-deliveries/simulate", async (SimulateAutoDeliveryAllocationDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.ConditionCode))
        return Results.BadRequest(new { error = "Cần mã cấu hình điều kiện ConditionCode để chạy mô phỏng." });
    try { return Results.Ok(await svc.SimulateAutoDeliveryAllocationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-deliveries/execute", async (ExecuteAutoDeliveryAllocationDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.ConditionCode))
        return Results.BadRequest(new { error = "Cần mã cấu hình điều kiện ConditionCode để thực thi phân bổ." });
    try { return Results.Ok(await svc.ExecuteAutoDeliveryAllocationAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/auto-deliveries/batches", async (IVehicleService svc, string? status, string? conditionCode, string? batchNo, string? executionMode) =>
    Results.Ok(await svc.ListAutoDeliveryBatchesAsync(status, conditionCode, batchNo, executionMode))).RequireAuthorization();

app.MapGet("/api/auto-deliveries/summary", async (IVehicleService svc, string? conditionCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetAutoDeliveryOrderSummaryAsync(conditionCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/reports/auto-deliveries/summary", async (IVehicleService svc, string? conditionCode, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetAutoDeliveryOrderSummaryAsync(conditionCode, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/auto-deliveries/batches/{batchNo}", async (string batchNo, IVehicleService svc) =>
{
    var r = await svc.GetAutoDeliveryBatchAsync(batchNo);
    return r is null ? Results.NotFound(new { batchNo, error = "Không tìm thấy đợt chạy giao xe tự động." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/auto-deliveries/batches/{batchNo}/{action}", async (string batchNo, string action, AutoDeliveryBatchTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("confirm" or "cancel"))
        return Results.BadRequest(new { error = "action = confirm|cancel" });
    try
    {
        var r = await svc.AutoDeliveryBatchTransitionAsync(batchNo, action, dto);
        return r is null ? Results.NotFound(new { batchNo, error = "Không tìm thấy đợt chạy hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/auto-deliveries/batches/{batchNo}/rollback", async (string batchNo, AutoDeliveryBatchTransitionDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RollbackAutoDeliveryBatchAsync(batchNo, dto);
        return r is null ? Results.NotFound(new { batchNo, error = "Không tìm thấy đợt chạy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/auto-deliveries/batches/{batchNo}", async (string batchNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveAutoDeliveryBatchAsync(batchNo);
        return r is null ? Results.NotFound(new { batchNo, error = "Không tìm thấy đợt chạy hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/auto-do-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAutoDoInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/auto-do-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleAutoDoHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Khảo sát Chỉ số Hài lòng Bán hàng SSI (BizHTC.DealerSales / DLS_VINSurvey, RptSSI_ICIC, DlsVINSurvey_Update) =====

app.MapPost("/api/ssi-surveys", async (CreateSalesSatisfactionSurveyDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.Vin))
        return Results.BadRequest(new { error = "Cần số khung xe VIN để lập phiếu khảo sát SSI." });
    try { return Results.Ok(await svc.CreateSalesSatisfactionSurveyAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/ssi-surveys", async (IVehicleService svc, string? status, string? dealer, string? model, string? npsCategory, bool? hasComplaint, string? surveyNo, string? vin, string? q) =>
    Results.Ok(await svc.ListSalesSatisfactionSurveysAsync(status, dealer, model, npsCategory, hasComplaint, surveyNo, vin, q))).RequireAuthorization();

app.MapGet("/api/ssi-surveys/summary", async (IVehicleService svc, string? dealer, string? model, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetSalesSatisfactionSummaryAsync(dealer, model, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/reports/ssi/summary", async (IVehicleService svc, string? dealer, string? model, DateTime? fromDate, DateTime? toDate) =>
    Results.Ok(await svc.GetSalesSatisfactionSummaryAsync(dealer, model, fromDate, toDate))).RequireAuthorization();

app.MapGet("/api/ssi-surveys/{surveyNo}", async (string surveyNo, IVehicleService svc) =>
{
    var r = await svc.GetSalesSatisfactionSurveyAsync(surveyNo);
    return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/ssi-surveys/{surveyNo}", async (string surveyNo, UpdateSalesSatisfactionSurveyDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesSatisfactionSurveyHeaderAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/update", async (string surveyNo, UpdateSalesSatisfactionSurveyDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesSatisfactionSurveyHeaderAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/complete", async (string surveyNo, CompleteSalesSatisfactionSurveyDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CompleteSalesSatisfactionSurveyAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/contact-attempt", async (string surveyNo, RecordSsiContactAttemptDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RecordSsiContactAttemptAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/escalate", async (string surveyNo, EscalateSsiSurveyDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.ComplaintCategory) || string.IsNullOrWhiteSpace(dto.ComplaintDetails))
        return Results.BadRequest(new { error = "Cần phân loại khiếu nại ComplaintCategory và nội dung ComplaintDetails." });
    try
    {
        var r = await svc.EscalateSsiSurveyAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/resolve-complaint", async (string surveyNo, ResolveSsiComplaintDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.RemedyAction))
        return Results.BadRequest(new { error = "Cần nội dung phương án xử lý RemedyAction." });
    try
    {
        var r = await svc.ResolveSsiComplaintAsync(surveyNo, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/{action}", async (string surveyNo, string action, SalesSatisfactionSurveyTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("in-progress" or "inprogress" or "start" or "complete" or "finish" or "unreachable" or "escalate" or "cancel"))
        return Results.BadRequest(new { error = "action = in-progress|complete|unreachable|escalate|cancel" });
    try
    {
        var r = await svc.SalesSatisfactionSurveyTransitionAsync(surveyNo, action, dto);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/ssi-surveys/{surveyNo}", async (string surveyNo, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesSatisfactionSurveyAsync(surveyNo);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/questions", async (string surveyNo, List<SsiQuestionInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách câu hỏi items để thêm vào phiếu." });
    try
    {
        var r = await svc.AddSsiQuestionLinesAsync(surveyNo, items);
        return r is null ? Results.NotFound(new { surveyNo, error = "Không tìm thấy phiếu khảo sát SSI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/ssi-surveys/{surveyNo}/questions/{lineId:long}/update", async (string surveyNo, long lineId, UpdateSsiQuestionLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSsiQuestionLineAsync(surveyNo, lineId, dto);
        return r is null ? Results.NotFound(new { surveyNo, lineId, error = "Không tìm thấy câu hỏi khảo sát." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/ssi-surveys/{surveyNo}/questions/{lineId:long}", async (string surveyNo, long lineId, UpdateSsiQuestionLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSsiQuestionLineAsync(surveyNo, lineId, dto);
        return r is null ? Results.NotFound(new { surveyNo, lineId, error = "Không tìm thấy câu hỏi khảo sát." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/ssi-surveys/{surveyNo}/questions/{lineId:long}", async (string surveyNo, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSsiQuestionLineAsync(surveyNo, lineId);
        return r is null ? Results.NotFound(new { surveyNo, lineId, error = "Không tìm thấy câu hỏi hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/ssi-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSsiInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/ssi-surveys", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSsiHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/ssi-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSsiHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Quản lý Khách hàng Tham quan Showroom & Phễu Bán hàng (BizHTC.RetailContract / DLR_CtmVisit, FrmCusVisit / CustomerVisit) =====

app.MapPost("/api/customer-visits", async (CreateCustomerVisitDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CreateCustomerVisitAsync(dto);
        return Results.Created($"/api/customer-visits/{(r as dynamic)?.visitCode}", r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/customer-visits", async (string? status, string? dealer, string? model, string? phone, string? consultant, string? leadSource, DateTime? fromDate, DateTime? toDate, IVehicleService svc) =>
{
    var r = await svc.ListCustomerVisitsAsync(status, dealer, model, phone, consultant, leadSource, fromDate, toDate);
    return Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/customer-visits/summary", async (string? dealer, int? year, int? month, IVehicleService svc) =>
{
    var r = await svc.GetCustomerVisitSummaryAsync(dealer, year, month);
    return Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/customer-visits/funnel", async (string? dealer, int? year, int? month, IVehicleService svc) =>
{
    var r = await svc.GetShowroomFunnelAnalyticsAsync(dealer, year, month);
    return Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/customer-visits/by-phone/{phone}", async (string phone, IVehicleService svc) =>
{
    var r = await svc.GetCustomerVisitByPhoneAsync(phone);
    return r is null ? Results.NotFound(new { phone, error = "Không tìm thấy lượt khách nào với số điện thoại này." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/customer-visits/{visitCode}", async (string visitCode, IVehicleService svc) =>
{
    var r = await svc.GetCustomerVisitAsync(visitCode);
    return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy mã lượt khách." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/customer-visits/{visitCode}", async (string visitCode, UpdateCustomerVisitDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateCustomerVisitAsync(visitCode, dto);
        return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy hoặc không thể cập nhật lượt khách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-visits/{visitCode}/transition", async (string visitCode, string action, CustomerVisitTransitionDto? dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.CustomerVisitTransitionAsync(visitCode, action, dto);
        return r is null ? Results.BadRequest(new { visitCode, action, error = "Chuyển trạng thái không hợp lệ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-visits/{visitCode}/follow-up", async (string visitCode, RecordVisitFollowUpDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RecordVisitFollowUpAsync(visitCode, dto);
        return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy lượt khách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-visits/{visitCode}/convert-to-test-drive", async (string visitCode, ConvertToTestDriveDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ConvertVisitToTestDriveAsync(visitCode, dto);
        return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy lượt khách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-visits/{visitCode}/convert-to-deal", async (string visitCode, ConvertToDealDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ConvertVisitToDealAsync(visitCode, dto);
        return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy lượt khách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/customer-visits/{visitCode}/actions", async (string visitCode, List<CustomerVisitActionInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách hành động items." });
    try
    {
        var r = await svc.AddVisitActionLogsAsync(visitCode, items);
        return r is null ? Results.NotFound(new { visitCode, error = "Không tìm thấy lượt khách." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/customer-visits/{visitCode}/actions/{lineId:long}", async (string visitCode, long lineId, UpdateVisitActionLogDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateVisitActionLogAsync(visitCode, lineId, dto);
        return r is null ? Results.NotFound(new { visitCode, lineId, error = "Không tìm thấy hành động tương tác." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/customer-visits/{visitCode}/actions/{lineId:long}", async (string visitCode, long lineId, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveVisitActionLogAsync(visitCode, lineId);
        return r is null ? Results.NotFound(new { visitCode, lineId, error = "Không tìm thấy hành động tương tác." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/visit-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleVisitInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/visits", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleVisitHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Quản lý Đề nghị & Quyết toán Chi phí Hỗ trợ Marketing Đại lý OEM (BizHTC.Marketing / MKT_MarketingFee, MKT_MarketingFeeDetail, Mst_MarketingActivity) =====

app.MapGet("/api/marketing-activity-types", async (IVehicleService svc, bool? activeOnly) =>
    Results.Ok(await svc.ListMarketingActivityTypesAsync(activeOnly))).RequireAuthorization();

app.MapPost("/api/marketing-activity-types", async (CreateMarketingActivityTypeDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateMarketingActivityTypeAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/marketing-activities", async (IVehicleService svc, string? typeCode, bool? activeOnly, string? q) =>
    Results.Ok(await svc.ListMarketingActivitiesAsync(typeCode, activeOnly, q))).RequireAuthorization();

app.MapGet("/api/marketing-activities/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetMarketingActivityAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hoạt động marketing." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/marketing-activities", async (CreateMarketingActivityDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateMarketingActivityAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/marketing-activities/{code}", async (string code, UpdateMarketingActivityDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingActivityAsync(code, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hoạt động marketing." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-activities/{code}/update", async (string code, UpdateMarketingActivityDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingActivityAsync(code, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hoạt động marketing." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees", async (CreateMarketingFeeDto dto, IVehicleService svc) =>
{
    try { return Results.Ok(await svc.CreateMarketingFeeAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/marketing-fees", async (IVehicleService svc, string? status, string? dealer, string? month, string? feeCode, string? q) =>
    Results.Ok(await svc.ListMarketingFeesAsync(status, dealer, month, feeCode, q))).RequireAuthorization();

app.MapGet("/api/marketing-fees/summary", async (IVehicleService svc, string? campaignMonth, string? dealerCode) =>
    Results.Ok(await svc.GetMarketingFeeSummaryAsync(campaignMonth, dealerCode))).RequireAuthorization();

app.MapGet("/api/reports/marketing-fees/summary", async (IVehicleService svc, string? campaignMonth, string? dealerCode) =>
    Results.Ok(await svc.GetMarketingFeeSummaryAsync(campaignMonth, dealerCode))).RequireAuthorization();

app.MapGet("/api/marketing-fees/{code}", async (string code, IVehicleService svc) =>
{
    var r = await svc.GetMarketingFeeAsync(code);
    return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ quyết toán marketing." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/marketing-fees/{code}", async (string code, UpdateMarketingFeeHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingFeeHeaderAsync(code, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ hoặc hồ sơ đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/update", async (string code, UpdateMarketingFeeHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingFeeHeaderAsync(code, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ hoặc hồ sơ đã hoàn tất/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/marketing-fees/{code}", async (string code, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveMarketingFeeAsync(code);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/marketing-fees/{code}/lines/{lineIndex:int}", async (string code, int lineIndex, UpdateMarketingFeeDetailDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingFeeDetailAsync(code, lineIndex, dto);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/lines/{lineIndex:int}/update", async (string code, int lineIndex, UpdateMarketingFeeDetailDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateMarketingFeeDetailAsync(code, lineIndex, dto);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/marketing-fees/{code}/lines/{lineIndex:int}/htc-limit", async (string code, int lineIndex, UpdateHTCLimitPriceDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateDetailHTCLimitAsync(code, lineIndex, dto);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/lines/{lineIndex:int}/approve", async (string code, int lineIndex, decimal? approvedQty, decimal? approvedAmount, string? actor, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ApproveMarketingFeeDetailAsync(code, lineIndex, approvedQty, approvedAmount, actor);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động hoặc hồ sơ đã chốt." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/lines/{lineIndex:int}/reject", async (string code, int lineIndex, string reason, string? actor, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RejectMarketingFeeDetailAsync(code, lineIndex, reason, actor);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động hoặc hồ sơ đã chốt." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/lines/{lineIndex:int}/attachments", async (string code, int lineIndex, AddMarketingFeeAttachDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.FileName))
        return Results.BadRequest(new { error = "Cần tên tệp tin FileName." });
    try
    {
        var r = await svc.AddMarketingFeeAttachAsync(code, lineIndex, dto);
        return r is null ? Results.NotFound(new { code, lineIndex, error = "Không tìm thấy dòng hoạt động." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/attachments/{attachCode}/review", async (string code, string attachCode, ReviewMarketingFeeAttachDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.ReviewMarketingFeeAttachAsync(code, attachCode, dto);
        return r is null ? Results.NotFound(new { code, attachCode, error = "Không tìm thấy chứng từ đính kèm." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/marketing-fees/{code}/{action}", async (string code, string action, MarketingFeeTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "pending" or "approve" or "finish" or "settle" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|finish|settle|reject|cancel" });
    try
    {
        var r = await svc.MarketingFeeTransitionAsync(code, action, dto);
        return r is null ? Results.NotFound(new { code, error = "Không tìm thấy hồ sơ hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/marketing-fee-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleMarketingFeeInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/marketing-fee-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleMarketingFeeHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/marketing-fees", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleMarketingFeeHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ===== Quản lý Chỉ tiêu Bán hàng & KPI Doanh số Xe Ô tô Đại lý & TVBH (BizHTC.MasterData & DMS.NP.Biz / SP_KPIMonth & Mst_SMKPI) =====

app.MapGet("/api/sales-kpi/indicators", async (IVehicleService svc, string? category, bool? activeOnly) =>
    Results.Ok(await svc.ListSalesKpiIndicatorsAsync(category, activeOnly))).RequireAuthorization();

app.MapGet("/api/sales-kpi/indicators/{kpiCode}", async (string kpiCode, IVehicleService svc) =>
{
    var r = await svc.GetSalesKpiIndicatorAsync(kpiCode);
    return r is null ? Results.NotFound(new { kpiCode, error = "Không tìm thấy chỉ số KPI." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/sales-kpi/indicators", async (CreateSalesKpiIndicatorDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.KPICode) || string.IsNullOrWhiteSpace(dto.KPIName))
        return Results.BadRequest(new { error = "Cần mã chỉ số KPICode và tên chỉ số KPIName." });
    try { return Results.Ok(await svc.CreateSalesKpiIndicatorAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/sales-kpi/indicators/{kpiCode}", async (string kpiCode, UpdateSalesKpiIndicatorDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesKpiIndicatorAsync(kpiCode, dto);
        return r is null ? Results.NotFound(new { kpiCode, error = "Không tìm thấy chỉ số KPI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-kpi/indicators/{kpiCode}", async (string kpiCode, IVehicleService svc) =>
{
    try
    {
        var r = await svc.DeleteSalesKpiIndicatorAsync(kpiCode);
        return r is null ? Results.NotFound(new { kpiCode, error = "Không tìm thấy chỉ số KPI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis", async (CreateSalesTargetKpiDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.PeriodMonth) || string.IsNullOrWhiteSpace(dto.DealerCode))
        return Results.BadRequest(new { error = "Cần tháng áp dụng PeriodMonth (YYYY-MM) và mã đại lý DealerCode." });
    try { return Results.Ok(await svc.CreateSalesTargetKpiAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/sales-target-kpis", async (IVehicleService svc, string? status, string? dealer, string? userCode, string? month, string? quarter, int? year, string? q) =>
    Results.Ok(await svc.ListSalesTargetKpisAsync(status, dealer, userCode, month, quarter, year, q))).RequireAuthorization();

app.MapGet("/api/sales-target-kpis/summary", async (IVehicleService svc, string? periodMonth, string? dealerCode, int? periodYear) =>
    Results.Ok(await svc.GetSalesKpiSummaryAsync(periodMonth, dealerCode, periodYear))).RequireAuthorization();

app.MapGet("/api/reports/sales-target-kpis/summary", async (IVehicleService svc, string? periodMonth, string? dealerCode, int? periodYear) =>
    Results.Ok(await svc.GetSalesKpiSummaryAsync(periodMonth, dealerCode, periodYear))).RequireAuthorization();

app.MapGet("/api/sales-target-kpis/leaderboard", async (IVehicleService svc, string? periodMonth, string? dealerCode) =>
    Results.Ok(await svc.GetSalesLeaderboardAsync(periodMonth, dealerCode))).RequireAuthorization();

app.MapGet("/api/sales-target-kpis/{targetCode}", async (string targetCode, IVehicleService svc) =>
{
    var r = await svc.GetSalesTargetKpiAsync(targetCode);
    return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu KPI." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPut("/api/sales-target-kpis/{targetCode}", async (string targetCode, UpdateSalesTargetKpiHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesTargetKpiHeaderAsync(targetCode, dto);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/update", async (string targetCode, UpdateSalesTargetKpiHeaderDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesTargetKpiHeaderAsync(targetCode, dto);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu hoặc kế hoạch đã chốt/hủy." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/{action}", async (string targetCode, string action, SalesTargetKpiTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("submit" or "approve" or "evaluate" or "cancel"))
        return Results.BadRequest(new { error = "action = submit|approve|evaluate|cancel" });
    try
    {
        var r = await svc.SalesTargetKpiTransitionAsync(targetCode, action, dto);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu hoặc sai trạng thái cho action." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/evaluate", async (string targetCode, EvaluateSalesTargetKpiDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.EvaluateSalesTargetKpiAsync(targetCode, dto);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu KPI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/sync-actuals", async (string targetCode, string? actor, IVehicleService svc) =>
{
    try
    {
        var r = await svc.SyncSalesTargetKpiActualsAsync(targetCode, actor);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu KPI." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-target-kpis/{targetCode}", async (string targetCode, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesTargetKpiAsync(targetCode);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu hoặc không thể xóa." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/lines", async (string targetCode, List<SalesTargetKpiLineInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách dòng model xe." });
    try
    {
        var r = await svc.AddSalesTargetKpiLinesAsync(targetCode, items);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPut("/api/sales-target-kpis/{targetCode}/lines/{lineIndex:int}", async (string targetCode, int lineIndex, UpdateSalesTargetKpiLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesTargetKpiLineAsync(targetCode, lineIndex, dto);
        return r is null ? Results.NotFound(new { targetCode, lineIndex, error = "Không tìm thấy dòng chỉ tiêu." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/lines/{lineIndex:int}/update", async (string targetCode, int lineIndex, UpdateSalesTargetKpiLineDto dto, IVehicleService svc) =>
{
    try
    {
        var r = await svc.UpdateSalesTargetKpiLineAsync(targetCode, lineIndex, dto);
        return r is null ? Results.NotFound(new { targetCode, lineIndex, error = "Không tìm thấy dòng chỉ tiêu." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-target-kpis/{targetCode}/lines/{lineIndex:int}", async (string targetCode, int lineIndex, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesTargetKpiLineAsync(targetCode, lineIndex);
        return r is null ? Results.NotFound(new { targetCode, lineIndex, error = "Không tìm thấy dòng chỉ tiêu." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/sales-target-kpis/{targetCode}/daily-logs", async (string targetCode, List<SalesKpiDailyLogInputDto> items, IVehicleService svc) =>
{
    if (items is null || items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách nhật ký tiến độ bán hàng." });
    try
    {
        var r = await svc.AddSalesKpiDailyLogsAsync(targetCode, items);
        return r is null ? Results.NotFound(new { targetCode, error = "Không tìm thấy kế hoạch chỉ tiêu." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapDelete("/api/sales-target-kpis/{targetCode}/daily-logs/{lineIndex:int}", async (string targetCode, int lineIndex, IVehicleService svc) =>
{
    try
    {
        var r = await svc.RemoveSalesKpiDailyLogAsync(targetCode, lineIndex);
        return r is null ? Results.NotFound(new { targetCode, lineIndex, error = "Không tìm thấy nhật ký tiến độ." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/sales-kpi-info", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSalesKpiInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/sales-kpis", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSalesKpiHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/sales-kpi-history", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSalesKpiHistoryAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Đề nghị giao tài liệu xe theo lô (BizHTC.WH.Car_DocReqList / Car_DocReqDtl) ----
app.MapPost("/api/doc-request-lists", async (CreateDocRequestListDto dto, IVehicleService svc) =>
{
    if (dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần danh sách VIN trong phiếu đề nghị giao tài liệu." });
    try { return Results.Ok(await svc.CreateDocRequestListAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/doc-request-lists", async (IVehicleService svc, string? status, string? dealer, string? vin, string? typeCRR) =>
    Results.Ok(await svc.ListDocRequestListsAsync(status, dealer, vin, typeCRR))).RequireAuthorization();

app.MapGet("/api/doc-request-lists/{drListCode}", async (string drListCode, IVehicleService svc) =>
{
    var r = await svc.GetDocRequestListAsync(drListCode);
    return r is null ? Results.NotFound(new { drListCode, error = "Không tìm thấy phiếu đề nghị giao tài liệu." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/doc-request-lists/{drListCode}/{action}", async (string drListCode, string action, DocRequestListTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve1" or "approve" or "cancel"))
        return Results.BadRequest(new { error = "action = approve1|approve|cancel" });
    try
    {
        var r = await svc.DocRequestListTransitionAsync(drListCode, action, dto);
        return r is null ? Results.NotFound(new { drListCode, error = "Không tìm thấy phiếu đề nghị hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapPost("/api/doc-request-lists/{drListCode}/lines/{vin}/{action}", async (string drListCode, string vin, string action, DocRequestListTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("approve2" or "approve" or "finish" or "complete" or "reject" or "cancel"))
        return Results.BadRequest(new { error = "action = approve2|approve|finish|complete|reject|cancel" });
    try
    {
        var r = await svc.DocRequestListLineTransitionAsync(drListCode, vin, action, dto);
        return r is null ? Results.NotFound(new { drListCode, vin, error = "Không tìm thấy phiếu/dòng hoặc sai trạng thái." }) : Results.Ok(r);
    }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/doc-request-lists", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleDocRequestListInfoAsync(vin);
    return r is null ? Results.NotFound(new { vin, error = "Không tìm thấy số khung VIN." }) : Results.Ok(r);
}).RequireAuthorization();

// ---- Tiến trình bán hàng / Phễu bán hàng khách hàng (HCare.idocNet SP_SalesProcess) ----
app.MapPost("/api/sales-processes", async (CreateSalesProcessDto dto, IVehicleService svc) =>
{
    if (string.IsNullOrWhiteSpace(dto.CarModelType) || dto.Items is null || dto.Items.Count == 0)
        return Results.BadRequest(new { error = "Cần CarModelType và danh sách Items dòng xe quan tâm." });
    try { return Results.Ok(await svc.CreateSalesProcessAsync(dto)); }
    catch (InvalidOperationException ex) { return Results.BadRequest(new { error = ex.Message }); }
}).RequireAuthorization();

app.MapGet("/api/sales-processes", async (IVehicleService svc, string? status, string? dealer, string? customer, string? salesId, string? model, string? userCodeOwner) =>
    Results.Ok(await svc.ListSalesProcessesAsync(status, dealer, customer, salesId, model, userCodeOwner))).RequireAuthorization();

app.MapGet("/api/sales-processes/summary", async (IVehicleService svc, string? dealer) =>
    Results.Ok(await svc.GetSalesProcessSummaryAsync(dealer))).RequireAuthorization();

app.MapGet("/api/sales-processes/{salesId}", async (string salesId, IVehicleService svc) =>
{
    var r = await svc.GetSalesProcessAsync(salesId);
    return r is null ? Results.NotFound(new { salesId, error = "Không tìm thấy tiến trình bán hàng." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/sales-processes/{salesId}/{action}", async (string salesId, string action, SalesProcessTransitionDto? dto, IVehicleService svc) =>
{
    if (action is not ("advance" or "movestatus" or "approvelevel" or "approve" or "cancel"))
        return Results.BadRequest(new { error = "action = advance|movestatus|approvelevel|approve|cancel" });
    var r = await svc.SalesProcessTransitionAsync(salesId, action, dto);
    return r is null ? Results.NotFound(new { salesId, error = "Không thấy tiến trình hoặc sai trạng thái cho action." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapPost("/api/sales-processes/{salesId}/lines/{modelCode}/update", async (string salesId, string modelCode, UpdateSalesProcessLineDto dto, IVehicleService svc) =>
{
    var r = await svc.UpdateSalesProcessLineAsync(salesId, modelCode, dto);
    return r is null ? Results.NotFound(new { salesId, modelCode, error = "Không tìm thấy dòng xe hoặc tiến trình đã hủy." }) : Results.Ok(r);
}).RequireAuthorization();

app.MapGet("/api/vehicles/{vin}/sales-processes", async (string vin, IVehicleService svc) =>
{
    var r = await svc.GetVehicleSalesProcessInfoAsync(vin);
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
