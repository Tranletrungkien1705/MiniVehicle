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
            ModelYear = r.ProductionYearActual, Status = VehicleStatus.InStock
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
