using Microsoft.EntityFrameworkCore;
using MiniVehicle.Data;
using MiniVehicle.Models;

namespace MiniVehicle.Services;

public record RegisterVehicleDto(string Vin, string Model, string? EngineNo, string? Color, int? ModelYear, int? WarrantyMonths);
public record CreateDoDto(string DealerCode, List<string> Vins, string? DoNo);
public record DeliverDto(string? OwnerName, string? OwnerPhone, string? PlateNo);
public record CreateRecallDto(string Code, string Title, string? Model, string? Reason, string? Remedy, List<string>? Vins);
public record RecallDoneDto(string Vin, string? DoneBy);
public record TransferDto(string NewOwnerName, string? NewOwnerPhone, string? NewPlateNo);
public record RegisterPlateDto(string PlateNo);

public interface IVehicleService
{
    Task<object> RegisterAsync(RegisterVehicleDto dto);
    Task<object> ListAsync(string? status, string? model, string? dealer);
    Task<object?> AllocateAsync(string vin, string dealerCode);
    Task<object> CreateDeliveryOrderAsync(CreateDoDto dto);
    Task<object?> DeliverAsync(string doNo, DeliverDto dto);
    Task<object?> HistoryAsync(string vin);
    Task<object> StatsAsync();
    Task<object?> PublicLookupAsync(string vin);
    Task<object> CreateRecallAsync(CreateRecallDto dto);
    Task<object> ListRecallsAsync();
    Task<object?> RecallAffectedAsync(string code);
    Task<object?> MarkRecallDoneAsync(string code, RecallDoneDto dto);
    Task<object?> TransferAsync(string vin, TransferDto dto);
    Task<object?> RegisterPlateAsync(string vin, string plateNo);
}

public sealed class VehicleService(AppDbContext db, ITenantContext tenant) : IVehicleService
{
    private Guid Org => tenant.OrgId;

    private void Log(string vin, string kind, string? note = null)
        => db.Events.Add(new VehicleEvent { OrgId = Org, Vin = vin, Kind = kind, Note = note });

    public async Task<object> RegisterAsync(RegisterVehicleDto dto)
    {
        var vin = dto.Vin.Trim().ToUpperInvariant();
        if (await db.Vehicles.AnyAsync(v => v.OrgId == Org && v.Vin == vin))
            throw new InvalidOperationException($"VIN {vin} đã tồn tại.");
        var v = new Vehicle
        {
            OrgId = Org, Vin = vin, Model = dto.Model.Trim(), EngineNo = dto.EngineNo, Color = dto.Color,
            ModelYear = dto.ModelYear, WarrantyMonths = dto.WarrantyMonths is > 0 ? dto.WarrantyMonths!.Value : 36,
            Status = VehicleStatus.InStock
        };
        db.Vehicles.Add(v);
        Log(vin, "Created", $"Model={v.Model}");
        await db.SaveChangesAsync();
        return new { v.Id, v.Vin, v.Model, status = v.Status.ToString() };
    }

    public async Task<object> ListAsync(string? status, string? model, string? dealer)
    {
        var q = db.Vehicles.Where(v => v.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<VehicleStatus>(status, true, out var st))
            q = q.Where(v => v.Status == st);
        if (!string.IsNullOrWhiteSpace(model)) q = q.Where(v => v.Model.Contains(model));
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(v => v.DealerCode == dealer);
        var items = await q.OrderByDescending(v => v.Id).Take(500).Select(v => new
        {
            v.Vin, v.Model, v.Color, v.ModelYear, status = v.Status.ToString(),
            v.DealerCode, v.OwnerName, v.PlateNo, v.DeliveredAt, v.WarrantyEnd
        }).ToListAsync();
        return new { count = items.Count, items };
    }

    public async Task<object?> AllocateAsync(string vin, string dealerCode)
    {
        vin = vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null || v.Status != VehicleStatus.InStock) return null;
        v.Status = VehicleStatus.Allocated; v.DealerCode = dealerCode.Trim();
        Log(vin, "Allocated", $"Dealer={v.DealerCode}");
        await db.SaveChangesAsync();
        return new { v.Vin, status = v.Status.ToString(), v.DealerCode };
    }

    public async Task<object> CreateDeliveryOrderAsync(CreateDoDto dto)
    {
        var dealer = dto.DealerCode.Trim();
        var vins = dto.Vins.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();
        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0) throw new InvalidOperationException("VIN không tồn tại: " + string.Join(", ", missing));
        var notReady = vehicles.Where(v => v.Status is VehicleStatus.OnDelivery or VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (notReady.Count > 0) throw new InvalidOperationException("VIN đã trên đường/đã giao: " + string.Join(", ", notReady));

        var doNo = string.IsNullOrWhiteSpace(dto.DoNo) ? "DO" + DateTime.Now.ToString("yyMMddHHmmss") : dto.DoNo!.Trim();
        var order = new DeliveryOrder { OrgId = Org, DoNo = doNo, DealerCode = dealer, Status = "Open" };
        db.DeliveryOrders.Add(order);
        await db.SaveChangesAsync();   // lấy Id
        foreach (var v in vehicles)
        {
            db.DeliveryOrderLines.Add(new DeliveryOrderLine { OrgId = Org, DeliveryOrderId = order.Id, Vin = v.Vin });
            v.Status = VehicleStatus.OnDelivery; v.DealerCode = dealer;
            Log(v.Vin, "DeliveryOrder", $"DO={doNo} Dealer={dealer}");
        }
        await db.SaveChangesAsync();
        return new { order.DoNo, dealer, count = vehicles.Count, vins = vehicles.Select(v => v.Vin), status = order.Status };
    }

    public async Task<object?> DeliverAsync(string doNo, DeliverDto dto)
    {
        doNo = doNo.Trim();
        var order = await db.DeliveryOrders.FirstOrDefaultAsync(o => o.OrgId == Org && o.DoNo == doNo && o.Status == "Open");
        if (order is null) return null;
        var lineVins = await db.DeliveryOrderLines.Where(l => l.OrgId == Org && l.DeliveryOrderId == order.Id).Select(l => l.Vin).ToListAsync();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && lineVins.Contains(v.Vin)).ToListAsync();
        var now = DateTime.Now;
        foreach (var v in vehicles)
        {
            v.Status = VehicleStatus.Delivered; v.DeliveredAt = now;
            v.WarrantyStart = now; v.WarrantyEnd = now.AddMonths(v.WarrantyMonths);
            if (vehicles.Count == 1)   // giao lẻ 1 xe → gán chủ xe nếu có
            {
                v.OwnerName = dto.OwnerName ?? v.OwnerName;
                v.OwnerPhone = dto.OwnerPhone ?? v.OwnerPhone;
                v.PlateNo = dto.PlateNo ?? v.PlateNo;
            }
            Log(v.Vin, "Delivered", $"DO={doNo} BH đến {v.WarrantyEnd:yyyy-MM-dd}");
        }
        order.Status = "Delivered"; order.DeliveredAt = now;
        await db.SaveChangesAsync();
        return new { order.DoNo, order.DealerCode, delivered = vehicles.Count, deliveredAt = now, warrantyMonths = vehicles.FirstOrDefault()?.WarrantyMonths };
    }

    public async Task<object?> HistoryAsync(string vin)
    {
        vin = vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null) return null;
        var events = await db.Events.Where(e => e.OrgId == Org && e.Vin == vin).OrderBy(e => e.Id)
            .Select(e => new { e.Kind, e.Note, e.At }).ToListAsync();
        return new
        {
            v.Vin, v.Model, status = v.Status.ToString(), v.DealerCode, v.OwnerName, v.PlateNo,
            v.DeliveredAt, v.WarrantyStart, v.WarrantyEnd, history = events
        };
    }

    public async Task<object> StatsAsync()
    {
        var q = db.Vehicles.Where(v => v.OrgId == Org);
        var byStatus = await q.GroupBy(v => v.Status).Select(g => new { status = g.Key, count = g.Count() }).ToListAsync();
        return new
        {
            total = await q.CountAsync(),
            inStock = byStatus.FirstOrDefault(x => x.status == VehicleStatus.InStock)?.count ?? 0,
            allocated = byStatus.FirstOrDefault(x => x.status == VehicleStatus.Allocated)?.count ?? 0,
            onDelivery = byStatus.FirstOrDefault(x => x.status == VehicleStatus.OnDelivery)?.count ?? 0,
            delivered = byStatus.FirstOrDefault(x => x.status == VehicleStatus.Delivered)?.count ?? 0,
            deliveryOrders = await db.DeliveryOrders.CountAsync(o => o.OrgId == Org)
        };
    }

    // Công khai: KHÔNG lọc theo tenant (tra bất kỳ VIN nào trong hệ thống).
    public async Task<object?> PublicLookupAsync(string vin)
    {
        vin = (vin ?? "").Trim().ToUpperInvariant();
        var v = await db.Vehicles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Vin == vin);
        if (v is null) return null;
        var active = v.WarrantyEnd.HasValue && v.WarrantyEnd.Value.Date >= DateTime.Now.Date;
        // Triệu hồi còn mở của xe này (join campaign)
        var openRecalls = await (from vr in db.VehicleRecalls.IgnoreQueryFilters()
                                 join c in db.Recalls.IgnoreQueryFilters() on vr.CampaignId equals c.Id
                                 where vr.Vin == vin && vr.Status == "Open"
                                 select new { c.Code, c.Title, c.Remedy }).ToListAsync();
        return new
        {
            found = true, v.Vin, v.Model, v.Color, v.ModelYear, status = v.Status.ToString(),
            delivered = v.Status == VehicleStatus.Delivered, v.DeliveredAt,
            warrantyActive = active,
            warrantyEnd = v.WarrantyEnd?.ToString("yyyy-MM-dd"),
            daysLeft = active ? (int)(v.WarrantyEnd!.Value.Date - DateTime.Now.Date).TotalDays : 0,
            hasOpenRecall = openRecalls.Count > 0,
            openRecalls
        };
    }

    // Đổi chủ (sang tên): chỉ xe đã giao. Bảo hành theo xe nên GIỮ NGUYÊN khi đổi chủ.
    public async Task<object?> TransferAsync(string vin, TransferDto dto)
    {
        vin = vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null || v.Status != VehicleStatus.Delivered) return null;
        var old = v.OwnerName;
        v.OwnerName = dto.NewOwnerName.Trim();
        v.OwnerPhone = dto.NewOwnerPhone ?? v.OwnerPhone;
        if (!string.IsNullOrWhiteSpace(dto.NewPlateNo)) v.PlateNo = dto.NewPlateNo.Trim();
        Log(vin, "Transfer", $"{old} -> {v.OwnerName}");
        await db.SaveChangesAsync();
        return new { v.Vin, previousOwner = old, newOwner = v.OwnerName, v.PlateNo };
    }

    // Đăng ký biển số (sau giao/đăng kiểm)
    public async Task<object?> RegisterPlateAsync(string vin, string plateNo)
    {
        vin = vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null || v.Status != VehicleStatus.Delivered) return null;
        v.PlateNo = plateNo.Trim();
        Log(vin, "PlateRegistered", v.PlateNo);
        await db.SaveChangesAsync();
        return new { v.Vin, v.PlateNo, v.OwnerName };
    }

    // ===== Triệu hồi (recall) =====
    public async Task<object> CreateRecallAsync(CreateRecallDto dto)
    {
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await db.Recalls.AnyAsync(r => r.OrgId == Org && r.Code == code))
            throw new InvalidOperationException($"Mã triệu hồi {code} đã tồn tại.");
        var c = new RecallCampaign
        {
            OrgId = Org, Code = code, Title = dto.Title.Trim(), Model = dto.Model,
            Reason = dto.Reason, Remedy = dto.Remedy, Status = "Open"
        };
        db.Recalls.Add(c);
        await db.SaveChangesAsync();

        // Gắn xe bị ảnh hưởng: theo model (nếu có) và/hoặc danh sách VIN chỉ định.
        var affected = new List<Vehicle>();
        if (!string.IsNullOrWhiteSpace(dto.Model))
            affected.AddRange(await db.Vehicles.Where(v => v.OrgId == Org && v.Model == dto.Model).ToListAsync());
        if (dto.Vins is { Count: > 0 })
        {
            var vins = dto.Vins.Select(s => s.Trim().ToUpperInvariant()).ToList();
            affected.AddRange(await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync());
        }
        var uniq = affected.GroupBy(v => v.Vin).Select(g => g.First()).ToList();
        foreach (var v in uniq)
        {
            db.VehicleRecalls.Add(new VehicleRecall { OrgId = Org, CampaignId = c.Id, Vin = v.Vin, Status = "Open" });
            Log(v.Vin, "Recall", $"Campaign={code}");
        }
        await db.SaveChangesAsync();
        return new { c.Code, c.Title, c.Model, affected = uniq.Count };
    }

    public async Task<object> ListRecallsAsync()
    {
        var items = await db.Recalls.Where(r => r.OrgId == Org).OrderByDescending(r => r.Id).Select(r => new
        {
            r.Code, r.Title, r.Model, r.Reason, r.Remedy, r.Status, r.CreatedAt,
            affected = db.VehicleRecalls.Count(x => x.OrgId == Org && x.CampaignId == r.Id),
            done = db.VehicleRecalls.Count(x => x.OrgId == Org && x.CampaignId == r.Id && x.Status == "Done")
        }).ToListAsync();
        return new { count = items.Count, items };
    }

    public async Task<object?> RecallAffectedAsync(string code)
    {
        code = code.Trim().ToUpperInvariant();
        var c = await db.Recalls.FirstOrDefaultAsync(r => r.OrgId == Org && r.Code == code);
        if (c is null) return null;
        var lines = await db.VehicleRecalls.Where(x => x.OrgId == Org && x.CampaignId == c.Id)
            .Select(x => new { x.Vin, x.Status, x.DoneAt, x.DoneBy }).ToListAsync();
        return new { c.Code, c.Title, c.Status, affected = lines.Count, done = lines.Count(l => l.Status == "Done"), vehicles = lines };
    }

    public async Task<object?> MarkRecallDoneAsync(string code, RecallDoneDto dto)
    {
        code = code.Trim().ToUpperInvariant();
        var vin = dto.Vin.Trim().ToUpperInvariant();
        var c = await db.Recalls.FirstOrDefaultAsync(r => r.OrgId == Org && r.Code == code);
        if (c is null) return null;
        var vr = await db.VehicleRecalls.FirstOrDefaultAsync(x => x.OrgId == Org && x.CampaignId == c.Id && x.Vin == vin);
        if (vr is null) return null;
        if (vr.Status != "Done")
        {
            vr.Status = "Done"; vr.DoneAt = DateTime.Now; vr.DoneBy = dto.DoneBy;
            Log(vin, "RecallDone", $"Campaign={code} By={dto.DoneBy}");
            await db.SaveChangesAsync();   // lưu trước rồi mới đếm để đóng campaign chính xác
            var remaining = await db.VehicleRecalls.CountAsync(x => x.OrgId == Org && x.CampaignId == c.Id && x.Status == "Open");
            if (remaining == 0 && c.Status != "Closed") { c.Status = "Closed"; await db.SaveChangesAsync(); }
        }
        return new { code, vin, status = vr.Status, campaignStatus = c.Status };
    }
}
