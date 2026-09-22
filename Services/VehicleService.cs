using Microsoft.EntityFrameworkCore;
using MiniVehicle.Data;
using MiniVehicle.Models;

namespace MiniVehicle.Services;

public record RegisterVehicleDto(string Vin, string Model, string? EngineNo, string? Color, int? ModelYear, int? WarrantyMonths, string? StorageCode = null);
public record CreateDoDto(string DealerCode, List<string> Vins, string? DoNo);
public record DeliverDto(string? OwnerName, string? OwnerPhone, string? PlateNo);
public record CreateRecallDto(string Code, string Title, string? Model, string? Reason, string? Remedy, List<string>? Vins);
public record RecallDoneDto(string Vin, string? DoneBy);
public record TransferDto(string NewOwnerName, string? NewOwnerPhone, string? NewPlateNo);
public record RegisterPlateDto(string PlateNo);
public record CreateClaimDto(string Vin, string DealerCode, string Issue, decimal PartsCost, decimal LaborCost);
public record ClaimDecisionDto(string? Note);
public record CreateDocReqDto(string Vin, string DealerCode, string? DocType, string? Note);
public record ShipDocDto(string? TrackingNo);
public record CreateTransferDto(string Vin, string ToDealer, string? Note);
public record CreateDeliveryMinutesDto(string Vin, string DealerCode, string? DoNo, string? TransporterCode, string? DriverName, string? DriverPhone, string? TruckPlateNo, string? FromStorage, string? ToStorage, string? DeliveredBy);
public record InspectDeliveryMinutesDto(int? OdoKm, string? ExteriorCondition, string? InteriorCondition, bool? HasSpareWheel, bool? HasToolKit, int? KeyCount, bool? HasGuarantyBooklet, bool? HasUserManual, bool? HasOriginalCertificate, string? ReceivedBy, string? Remark);
public record ConfirmDeliveryMinutesDto(string? ConfirmedBy, string? Remark);
public record RejectDeliveryMinutesDto(string? Reason);
public record CreateCarRetrieveDto(string DealerCode, List<string> Vins, string? ToStorage, string? Reason, string? RetrieveNo);
public record CarRetrieveTransitionDto(string? Note);
public record CreateTransportRequestDto(string DealerCode, List<string> Vins, string? TransporterCode, string? TransportContractNo, string? TruckPlateNo, string? DriverName, string? DriverPhone, string? FromStorage, string? ToStorage, string? DeliveryOrderNo, DateTime? EstimatedDeparture, DateTime? EstimatedArrival, string? Remark, string? TransportReqNo);
public record TransportRequestTransitionDto(string? Note, string? TruckPlateNo, string? DriverName, string? DriverPhone);
public record StorageRearrangeItemInputDto(string Vin, string StorageCodeTo, string? StorageCodeFrom = null, string? Remark = null);
public record CreateStorageRearrangeDto(List<StorageRearrangeItemInputDto> Items, string? Reason = null, string? Remark = null, string? StorageRearrangeNo = null);
public record StorageRearrangeTransitionDto(string? Note);
public record CompleteStorageRearrangeLineDto(string? RearrangeEndDate = null, string? Remark = null);

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
    Task<object> CreateClaimAsync(CreateClaimDto dto);
    Task<object> ListClaimsAsync(string? status, string? vin, string? dealer);
    Task<object?> DecideClaimAsync(string claimNo, bool approve, string? note);
    Task<object?> SettleClaimAsync(string claimNo);
    Task<object> CreateDocReqAsync(CreateDocReqDto dto);
    Task<object> ListDocReqAsync(string? status, string? dealer, string? vin);
    Task<object?> DocReqTransitionAsync(string code, string action, string? trackingNo);
    Task<object> CreateTransferAsync(CreateTransferDto dto);
    Task<object> ListTransfersAsync(string? status);
    Task<object?> TransferTransitionAsync(string code, string action);
    Task<object> CreateDeliveryMinutesAsync(CreateDeliveryMinutesDto dto);
    Task<object> ListDeliveryMinutesAsync(string? status, string? dealer, string? vin);
    Task<object?> GetDeliveryMinutesAsync(string dlvMnNo);
    Task<object?> InspectDeliveryMinutesAsync(string dlvMnNo, InspectDeliveryMinutesDto dto);
    Task<object?> ConfirmDeliveryMinutesAsync(string dlvMnNo, ConfirmDeliveryMinutesDto dto);
    Task<object?> RejectDeliveryMinutesAsync(string dlvMnNo, string? reason);
    Task<object> CreateCarRetrieveAsync(CreateCarRetrieveDto dto);
    Task<object> ListCarRetrievesAsync(string? status, string? dealer, string? vin);
    Task<object?> GetCarRetrieveAsync(string retrieveNo);
    Task<object?> CarRetrieveTransitionAsync(string retrieveNo, string action, string? note);
    Task<object> CreateTransportRequestAsync(CreateTransportRequestDto dto);
    Task<object> ListTransportRequestsAsync(string? status, string? dealer, string? transporter, string? vin);
    Task<object?> GetTransportRequestAsync(string transportReqNo);
    Task<object?> TransportRequestTransitionAsync(string transportReqNo, string action, TransportRequestTransitionDto? dto);
    Task<object> CreateStorageRearrangeAsync(CreateStorageRearrangeDto dto);
    Task<object> ListStorageRearrangesAsync(string? status, string? vin, string? storageCodeTo);
    Task<object?> GetStorageRearrangeAsync(string storageRearrangeNo);
    Task<object?> StorageRearrangeTransitionAsync(string storageRearrangeNo, string action, StorageRearrangeTransitionDto? dto);
    Task<object?> CompleteStorageRearrangeLineAsync(string storageRearrangeNo, string vin, CompleteStorageRearrangeLineDto? dto);
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
            StorageCode = dto.StorageCode?.Trim(),
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
            v.StorageCode, v.DealerCode, v.OwnerName, v.PlateNo, v.DeliveredAt, v.WarrantyEnd
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

    // ===== Yêu cầu bảo hành (GrtClaim) =====
    public async Task<object> CreateClaimAsync(CreateClaimDto dto)
    {
        var vin = dto.Vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin)
            ?? throw new InvalidOperationException($"Không thấy xe {vin}.");
        if (v.Status != VehicleStatus.Delivered)
            throw new InvalidOperationException("Xe chưa giao — không thể mở yêu cầu bảo hành.");
        if (!(v.WarrantyEnd.HasValue && v.WarrantyEnd.Value.Date >= DateTime.Now.Date))
            throw new InvalidOperationException("Xe đã HẾT bảo hành — không đủ điều kiện.");
        var claimNo = "WC" + DateTime.Now.ToString("yyMMddHHmmss");
        var c = new WarrantyClaim
        {
            OrgId = Org, ClaimNo = claimNo, Vin = vin, DealerCode = dto.DealerCode.Trim(),
            Issue = dto.Issue.Trim(), PartsCost = dto.PartsCost, LaborCost = dto.LaborCost, Status = "Submitted"
        };
        db.Claims.Add(c);
        Log(vin, "WarrantyClaim", $"{claimNo} {dto.Issue}");
        await db.SaveChangesAsync();
        return new { c.ClaimNo, c.Vin, c.DealerCode, total = c.PartsCost + c.LaborCost, status = c.Status };
    }

    public async Task<object> ListClaimsAsync(string? status, string? vin, string? dealer)
    {
        var q = db.Claims.Where(c => c.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(c => c.Status == status);
        if (!string.IsNullOrWhiteSpace(vin)) { var vv = vin.Trim().ToUpperInvariant(); q = q.Where(c => c.Vin == vv); }
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(c => c.DealerCode == dealer);
        var items = await q.OrderByDescending(c => c.Id).Take(500).Select(c => new
        {
            c.ClaimNo, c.Vin, c.DealerCode, c.Issue, c.PartsCost, c.LaborCost,
            total = c.PartsCost + c.LaborCost, c.Status, c.CreatedAt, c.DecidedAt, c.SettledAt, c.DecisionNote
        }).ToListAsync();
        return new { count = items.Count, totalApprovedValue = items.Where(i => i.Status is "Approved" or "Settled").Sum(i => i.total), items };
    }

    public async Task<object?> DecideClaimAsync(string claimNo, bool approve, string? note)
    {
        claimNo = claimNo.Trim().ToUpperInvariant();
        var c = await db.Claims.FirstOrDefaultAsync(x => x.OrgId == Org && x.ClaimNo == claimNo);
        if (c is null || c.Status != "Submitted") return null;   // chỉ quyết trên claim đang chờ
        c.Status = approve ? "Approved" : "Rejected";
        c.DecisionNote = note; c.DecidedAt = DateTime.Now;
        Log(c.Vin, "WarrantyClaim" + c.Status, claimNo);
        await db.SaveChangesAsync();
        return new { c.ClaimNo, c.Vin, status = c.Status, c.DecisionNote };
    }

    public async Task<object?> SettleClaimAsync(string claimNo)
    {
        claimNo = claimNo.Trim().ToUpperInvariant();
        var c = await db.Claims.FirstOrDefaultAsync(x => x.OrgId == Org && x.ClaimNo == claimNo);
        if (c is null || c.Status != "Approved") return null;    // chỉ quyết toán claim đã duyệt
        c.Status = "Settled"; c.SettledAt = DateTime.Now;
        Log(c.Vin, "WarrantyClaimSettled", $"{claimNo} {c.PartsCost + c.LaborCost}");
        await db.SaveChangesAsync();
        return new { c.ClaimNo, c.Vin, status = c.Status, total = c.PartsCost + c.LaborCost, c.SettledAt };
    }

    // ===== Chuyển kho / điều chuyển xe =====
    public async Task<object> CreateTransferAsync(CreateTransferDto dto)
    {
        var vin = dto.Vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin)
            ?? throw new InvalidOperationException($"Không thấy xe {vin}.");
        if (v.Status == VehicleStatus.Delivered)
            throw new InvalidOperationException("Xe đã giao — không điều chuyển được.");
        var code = "TF" + DateTime.Now.ToString("yyMMddHHmmss");
        var t = new StockTransfer { OrgId = Org, Code = code, Vin = vin, FromDealer = v.DealerCode, ToDealer = dto.ToDealer.Trim(), Note = dto.Note, Status = "Requested" };
        db.Transfers.Add(t);
        Log(vin, "TransferRequest", $"{code} {t.FromDealer}->{t.ToDealer}");
        await db.SaveChangesAsync();
        return new { t.Code, t.Vin, t.FromDealer, t.ToDealer, status = t.Status };
    }

    public async Task<object> ListTransfersAsync(string? status)
    {
        var q = db.Transfers.Where(t => t.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(t => t.Status == status);
        var items = await q.OrderByDescending(t => t.Id).Take(500).Select(t => new
        {
            t.Code, t.Vin, t.FromDealer, t.ToDealer, t.Status, t.CreatedAt, t.ApprovedAt, t.ReceivedAt
        }).ToListAsync();
        return new { count = items.Count, items };
    }

    // approve (Requested→Approved→InTransit), receive (InTransit→Received: cập nhật DealerCode xe), reject
    public async Task<object?> TransferTransitionAsync(string code, string action)
    {
        code = code.Trim().ToUpperInvariant();
        var t = await db.Transfers.FirstOrDefaultAsync(x => x.OrgId == Org && x.Code == code);
        if (t is null) return null;
        var now = DateTime.Now;
        switch (action)
        {
            case "approve": if (t.Status != "Requested") return null; t.Status = "InTransit"; t.ApprovedAt = now; break;
            case "reject": if (t.Status != "Requested") return null; t.Status = "Rejected"; t.ApprovedAt = now; break;
            case "receive":
                if (t.Status != "InTransit") return null;
                t.Status = "Received"; t.ReceivedAt = now;
                var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == t.Vin);
                if (v != null && v.Status != VehicleStatus.Delivered) { v.DealerCode = t.ToDealer; if (v.Status == VehicleStatus.InStock) v.Status = VehicleStatus.Allocated; }
                break;
            default: return null;
        }
        Log(t.Vin, "Transfer" + t.Status, code);
        await db.SaveChangesAsync();
        return new { t.Code, t.Vin, status = t.Status, t.ToDealer };
    }

    // ===== Đề nghị giao tài liệu (CarDocReq/ĐNGT) =====
    public async Task<object> CreateDocReqAsync(CreateDocReqDto dto)
    {
        var vin = dto.Vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin)
            ?? throw new InvalidOperationException($"Không thấy xe {vin}.");
        if (v.Status != VehicleStatus.Delivered)
            throw new InvalidOperationException("Xe chưa giao — chưa thể xin hồ sơ.");
        var code = "DR" + DateTime.Now.ToString("yyMMddHHmmss");
        var r = new DocRequest
        {
            OrgId = Org, Code = code, Vin = vin, DealerCode = dto.DealerCode.Trim(),
            DocType = string.IsNullOrWhiteSpace(dto.DocType) ? "Registration" : dto.DocType!.Trim(),
            Note = dto.Note, Status = "Requested"
        };
        db.DocRequests.Add(r);
        Log(vin, "DocRequest", $"{code} {r.DocType}");
        await db.SaveChangesAsync();
        return new { r.Code, r.Vin, r.DocType, r.DealerCode, status = r.Status };
    }

    public async Task<object> ListDocReqAsync(string? status, string? dealer, string? vin)
    {
        var q = db.DocRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(r => r.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(vin)) { var vv = vin.Trim().ToUpperInvariant(); q = q.Where(r => r.Vin == vv); }
        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.Code, r.Vin, r.DealerCode, r.DocType, r.Status, r.TrackingNo, r.CreatedAt, r.ApprovedAt, r.ShippedAt, r.ReceivedAt
        }).ToListAsync();
        return new { count = items.Count, items };
    }

    // State machine: approve (Requested→Approved), reject (Requested→Rejected), ship (Approved→Shipped), receive (Shipped→Received)
    public async Task<object?> DocReqTransitionAsync(string code, string action, string? trackingNo)
    {
        code = code.Trim().ToUpperInvariant();
        var r = await db.DocRequests.FirstOrDefaultAsync(x => x.OrgId == Org && x.Code == code);
        if (r is null) return null;
        var now = DateTime.Now;
        switch (action)
        {
            case "approve": if (r.Status != "Requested") return null; r.Status = "Approved"; r.ApprovedAt = now; break;
            case "reject": if (r.Status != "Requested") return null; r.Status = "Rejected"; r.ApprovedAt = now; break;
            case "ship": if (r.Status != "Approved") return null; r.Status = "Shipped"; r.ShippedAt = now; r.TrackingNo = trackingNo; break;
            case "receive": if (r.Status != "Shipped") return null; r.Status = "Received"; r.ReceivedAt = now; break;
            default: return null;
        }
        Log(r.Vin, "DocRequest" + r.Status, code);
        await db.SaveChangesAsync();
        return new { r.Code, r.Vin, status = r.Status, r.TrackingNo };
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

    // ===== Biên bản giao nhận xe (BizHTC.Storage.DlvMinutes / Sto_DlvMinutes) =====
    public async Task<object> CreateDeliveryMinutesAsync(CreateDeliveryMinutesDto dto)
    {
        var vin = dto.Vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin)
            ?? throw new InvalidOperationException($"Không thấy xe {vin}.");

        var dlvMnNo = "DMN" + DateTime.Now.ToString("yyMMddHHmmss");
        var m = new DeliveryMinutes
        {
            OrgId = Org,
            DlvMnNo = dlvMnNo,
            Vin = vin,
            DealerCode = dto.DealerCode.Trim(),
            DoNo = dto.DoNo?.Trim(),
            TransporterCode = dto.TransporterCode?.Trim(),
            DriverName = dto.DriverName?.Trim(),
            DriverPhone = dto.DriverPhone?.Trim(),
            TruckPlateNo = dto.TruckPlateNo?.Trim(),
            FromStorage = dto.FromStorage?.Trim(),
            ToStorage = dto.ToStorage?.Trim(),
            DeliveredBy = dto.DeliveredBy?.Trim(),
            Status = "Draft",
            CreatedAt = DateTime.Now
        };
        db.DeliveryMinutes.Add(m);
        Log(vin, "DeliveryMinutesCreated", $"{dlvMnNo} ĐL:{m.DealerCode} Nhà xe:{m.TransporterCode ?? "N/A"}");
        await db.SaveChangesAsync();
        return new { m.DlvMnNo, m.Vin, m.DealerCode, m.TransporterCode, m.TruckPlateNo, status = m.Status };
    }

    public async Task<object> ListDeliveryMinutesAsync(string? status, string? dealer, string? vin)
    {
        var q = db.DeliveryMinutes.Where(m => m.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(m => m.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(m => m.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(vin)) { var vv = vin.Trim().ToUpperInvariant(); q = q.Where(m => m.Vin == vv); }
        var items = await q.OrderByDescending(m => m.Id).Take(500).Select(m => new
        {
            m.DlvMnNo, m.Vin, m.DealerCode, m.DoNo, m.TransporterCode, m.DriverName, m.TruckPlateNo,
            m.OdoKm, m.Status, m.CreatedAt, m.HandoverDate, m.ConfirmedAt, m.DeliveredBy, m.ReceivedBy
        }).ToListAsync();
        return new { count = items.Count, items };
    }

    public async Task<object?> GetDeliveryMinutesAsync(string dlvMnNo)
    {
        dlvMnNo = dlvMnNo.Trim().ToUpperInvariant();
        var m = await db.DeliveryMinutes.FirstOrDefaultAsync(x => x.OrgId == Org && x.DlvMnNo == dlvMnNo);
        if (m is null) return null;
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == m.Vin);
        return new
        {
            m.DlvMnNo, m.Vin,
            vehicle = v == null ? null : new { v.Model, v.Color, v.EngineNo, v.ModelYear, status = v.Status.ToString(), v.OwnerName, v.PlateNo },
            m.DealerCode, m.DoNo, m.TransporterCode, m.DriverName, m.DriverPhone, m.TruckPlateNo,
            m.FromStorage, m.ToStorage, m.OdoKm,
            m.ExteriorCondition, m.InteriorCondition,
            m.HasSpareWheel, m.HasToolKit, m.KeyCount,
            m.HasGuarantyBooklet, m.HasUserManual, m.HasOriginalCertificate,
            m.DeliveredBy, m.ReceivedBy, m.Status, m.Remark,
            m.CreatedAt, m.HandoverDate, m.ConfirmedAt
        };
    }

    public async Task<object?> InspectDeliveryMinutesAsync(string dlvMnNo, InspectDeliveryMinutesDto dto)
    {
        dlvMnNo = dlvMnNo.Trim().ToUpperInvariant();
        var m = await db.DeliveryMinutes.FirstOrDefaultAsync(x => x.OrgId == Org && x.DlvMnNo == dlvMnNo);
        if (m is null || m.Status is "Confirmed" or "Rejected") return null;

        if (dto.OdoKm.HasValue) m.OdoKm = dto.OdoKm.Value;
        if (dto.ExteriorCondition != null) m.ExteriorCondition = dto.ExteriorCondition;
        if (dto.InteriorCondition != null) m.InteriorCondition = dto.InteriorCondition;
        if (dto.HasSpareWheel.HasValue) m.HasSpareWheel = dto.HasSpareWheel.Value;
        if (dto.HasToolKit.HasValue) m.HasToolKit = dto.HasToolKit.Value;
        if (dto.KeyCount.HasValue) m.KeyCount = dto.KeyCount.Value;
        if (dto.HasGuarantyBooklet.HasValue) m.HasGuarantyBooklet = dto.HasGuarantyBooklet.Value;
        if (dto.HasUserManual.HasValue) m.HasUserManual = dto.HasUserManual.Value;
        if (dto.HasOriginalCertificate.HasValue) m.HasOriginalCertificate = dto.HasOriginalCertificate.Value;
        if (!string.IsNullOrWhiteSpace(dto.ReceivedBy)) m.ReceivedBy = dto.ReceivedBy.Trim();
        if (dto.Remark != null) m.Remark = dto.Remark;

        m.Status = "Inspected";
        Log(m.Vin, "DeliveryMinutesInspected", $"{dlvMnNo} ODO={m.OdoKm}km Ngoại thất={m.ExteriorCondition ?? "OK"}");
        await db.SaveChangesAsync();
        return new { m.DlvMnNo, m.Vin, m.Status, m.OdoKm, m.ExteriorCondition, m.ReceivedBy };
    }

    public async Task<object?> ConfirmDeliveryMinutesAsync(string dlvMnNo, ConfirmDeliveryMinutesDto dto)
    {
        dlvMnNo = dlvMnNo.Trim().ToUpperInvariant();
        var m = await db.DeliveryMinutes.FirstOrDefaultAsync(x => x.OrgId == Org && x.DlvMnNo == dlvMnNo);
        if (m is null || m.Status is "Confirmed" or "Rejected") return null;

        var now = DateTime.Now;
        m.Status = "Confirmed";
        m.ConfirmedAt = now;
        m.HandoverDate ??= now;
        if (!string.IsNullOrWhiteSpace(dto.ConfirmedBy) && string.IsNullOrWhiteSpace(m.ReceivedBy))
            m.ReceivedBy = dto.ConfirmedBy.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Remark))
            m.Remark = string.IsNullOrWhiteSpace(m.Remark) ? dto.Remark : $"{m.Remark} | {dto.Remark}";

        Log(m.Vin, "DeliveryMinutesConfirmed", $"{dlvMnNo} Ký nhận: {m.ReceivedBy ?? dto.ConfirmedBy}");
        await db.SaveChangesAsync();
        return new { m.DlvMnNo, m.Vin, m.DealerCode, status = m.Status, m.ConfirmedAt, m.ReceivedBy };
    }

    public async Task<object?> RejectDeliveryMinutesAsync(string dlvMnNo, string? reason)
    {
        dlvMnNo = dlvMnNo.Trim().ToUpperInvariant();
        var m = await db.DeliveryMinutes.FirstOrDefaultAsync(x => x.OrgId == Org && x.DlvMnNo == dlvMnNo);
        if (m is null || m.Status is "Confirmed" or "Rejected") return null;

        m.Status = "Rejected";
        if (!string.IsNullOrWhiteSpace(reason))
            m.Remark = string.IsNullOrWhiteSpace(m.Remark) ? reason : $"{m.Remark} | Từ chối: {reason}";

        Log(m.Vin, "DeliveryMinutesRejected", $"{dlvMnNo} Lý do: {reason ?? "Không đạt tiêu chuẩn bàn giao"}");
        await db.SaveChangesAsync();
        return new { m.DlvMnNo, m.Vin, status = m.Status, m.Remark };
    }

    // ===== Lệnh thu hồi xe về kho / Đại lý trả xe (BizHTC.Storage.CarRetrieve / Sto_CarRetrieve) =====
    public async Task<object> CreateCarRetrieveAsync(CreateCarRetrieveDto dto)
    {
        if (dto.Vins is null || dto.Vins.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 VIN để tạo lệnh thu hồi.");

        var dealer = dto.DealerCode.Trim();
        var vins = dto.Vins.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Kiểm tra xe không được ở trạng thái đã giao cho khách lẻ (Delivered)
        var invalidDelivered = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (invalidDelivered.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng cuối (Delivered) không thể thu hồi kho: " + string.Join(", ", invalidDelivered));

        var retrieveNo = string.IsNullOrWhiteSpace(dto.RetrieveNo) ? "RET" + DateTime.Now.ToString("yyMMddHHmmss") : dto.RetrieveNo!.Trim().ToUpperInvariant();
        if (await db.CarRetrieves.AnyAsync(r => r.OrgId == Org && r.RetrieveNo == retrieveNo))
            throw new InvalidOperationException($"Mã lệnh thu hồi {retrieveNo} đã tồn tại.");

        var ret = new CarRetrieve
        {
            OrgId = Org,
            RetrieveNo = retrieveNo,
            DealerCode = dealer,
            ToStorage = dto.ToStorage?.Trim(),
            Reason = dto.Reason?.Trim(),
            Status = "Requested",
            CreatedAt = DateTime.Now
        };
        db.CarRetrieves.Add(ret);
        await db.SaveChangesAsync();

        foreach (var v in vehicles)
        {
            db.CarRetrieveLines.Add(new CarRetrieveLine
            {
                OrgId = Org,
                CarRetrieveId = ret.Id,
                RetrieveNo = retrieveNo,
                Vin = v.Vin,
                StorageCode = dto.ToStorage?.Trim(),
                Remark = dto.Reason?.Trim()
            });
            Log(v.Vin, "RetrieveRequested", $"{retrieveNo} ĐL:{dealer} Lý do:{dto.Reason ?? "N/A"}");
        }
        await db.SaveChangesAsync();

        return new { ret.RetrieveNo, ret.DealerCode, ret.ToStorage, ret.Status, totalVins = vins.Count, vins };
    }

    public async Task<object> ListCarRetrievesAsync(string? status, string? dealer, string? vin)
    {
        var q = db.CarRetrieves.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(r => r.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.CarRetrieveLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.RetrieveNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.RetrieveNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.RetrieveNo,
            r.DealerCode,
            r.ToStorage,
            r.Reason,
            r.Status,
            r.CreatedAt,
            r.ApprovedAt,
            r.ReceivedAt,
            vinCount = db.CarRetrieveLines.Count(l => l.OrgId == Org && l.CarRetrieveId == r.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetCarRetrieveAsync(string retrieveNo)
    {
        retrieveNo = retrieveNo.Trim().ToUpperInvariant();
        var ret = await db.CarRetrieves.FirstOrDefaultAsync(r => r.OrgId == Org && r.RetrieveNo == retrieveNo);
        if (ret is null) return null;

        var lines = await db.CarRetrieveLines.Where(l => l.OrgId == Org && l.CarRetrieveId == ret.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.StorageCode,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.DealerCode } : null
        }).ToList();

        return new
        {
            ret.RetrieveNo,
            ret.DealerCode,
            ret.ToStorage,
            ret.Reason,
            ret.Status,
            ret.CreatedAt,
            ret.ApprovedAt,
            ret.ReceivedAt,
            vins = details
        };
    }

    public async Task<object?> CarRetrieveTransitionAsync(string retrieveNo, string action, string? note)
    {
        retrieveNo = retrieveNo.Trim().ToUpperInvariant();
        var ret = await db.CarRetrieves.FirstOrDefaultAsync(r => r.OrgId == Org && r.RetrieveNo == retrieveNo);
        if (ret is null) return null;

        var now = DateTime.Now;
        var lines = await db.CarRetrieveLines.Where(l => l.OrgId == Org && l.CarRetrieveId == ret.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (ret.Status != "Requested") return null;
                ret.Status = "Approved";
                ret.ApprovedAt = now;
                foreach (var v in vehicles) Log(v.Vin, "RetrieveApproved", retrieveNo);
                break;

            case "ship":
            case "dispatch":
                if (ret.Status != "Approved") return null;
                ret.Status = "InTransit";
                foreach (var v in vehicles) Log(v.Vin, "RetrieveInTransit", retrieveNo);
                break;

            case "receive":
                if (ret.Status is not ("Approved" or "InTransit")) return null;
                ret.Status = "Received";
                ret.ReceivedAt = now;
                foreach (var v in vehicles)
                {
                    v.Status = VehicleStatus.InStock;
                    v.DealerCode = null; // giải phóng khỏi đại lý, trở về kho trung tâm
                    Log(v.Vin, "Retrieved", $"{retrieveNo} Trả về kho: {ret.ToStorage ?? "Kho trung tâm"} Note: {note ?? ret.Reason ?? ""}".Trim());
                }
                break;

            case "reject":
                if (ret.Status != "Requested") return null;
                ret.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(note)) ret.Reason = $"{ret.Reason} | Từ chối: {note}";
                foreach (var v in vehicles) Log(v.Vin, "RetrieveRejected", $"{retrieveNo} Lý do: {note ?? "N/A"}");
                break;

            case "cancel":
                if (ret.Status is not ("Requested" or "Approved")) return null;
                ret.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(note)) ret.Reason = $"{ret.Reason} | Hủy: {note}";
                foreach (var v in vehicles) Log(v.Vin, "RetrieveCancelled", retrieveNo);
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { ret.RetrieveNo, ret.DealerCode, status = ret.Status, ret.ApprovedAt, ret.ReceivedAt };
    }

    // ===== Yêu cầu / Kế hoạch vận chuyển xe (BizHTC.Car.TransportReq / Car_TransportReq) =====
    public async Task<object> CreateTransportRequestAsync(CreateTransportRequestDto dto)
    {
        if (dto.Vins is null || dto.Vins.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 VIN để tạo yêu cầu vận chuyển.");

        var dealer = dto.DealerCode.Trim();
        var vins = dto.Vins.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Không cho phép vận chuyển xe đã giao cho khách cuối (Delivered)
        var invalidDelivered = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (invalidDelivered.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng cuối (Delivered) không thể lập lệnh vận chuyển: " + string.Join(", ", invalidDelivered));

        var reqNo = string.IsNullOrWhiteSpace(dto.TransportReqNo) ? "TR" + DateTime.Now.ToString("yyMMddHHmmss") : dto.TransportReqNo!.Trim().ToUpperInvariant();
        if (await db.TransportRequests.AnyAsync(r => r.OrgId == Org && r.TransportReqNo == reqNo))
            throw new InvalidOperationException($"Mã lệnh vận chuyển {reqNo} đã tồn tại.");

        var tr = new TransportRequest
        {
            OrgId = Org,
            TransportReqNo = reqNo,
            DealerCode = dealer,
            TransporterCode = dto.TransporterCode?.Trim(),
            TransportContractNo = dto.TransportContractNo?.Trim(),
            TruckPlateNo = dto.TruckPlateNo?.Trim(),
            DriverName = dto.DriverName?.Trim(),
            DriverPhone = dto.DriverPhone?.Trim(),
            FromStorage = dto.FromStorage?.Trim(),
            ToStorage = dto.ToStorage?.Trim(),
            EstimatedDeparture = dto.EstimatedDeparture,
            EstimatedArrival = dto.EstimatedArrival,
            Status = "Pending",
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.TransportRequests.Add(tr);
        await db.SaveChangesAsync();

        foreach (var v in vehicles)
        {
            db.TransportRequestLines.Add(new TransportRequestLine
            {
                OrgId = Org,
                TransportRequestId = tr.Id,
                TransportReqNo = reqNo,
                Vin = v.Vin,
                DeliveryOrderNo = dto.DeliveryOrderNo?.Trim(),
                StorageCode = dto.FromStorage?.Trim(),
                Status = "Pending",
                Remark = dto.Remark?.Trim()
            });
            Log(v.Vin, "TransportReqCreated", $"{reqNo} ĐL:{dealer} Nhà xe:{tr.TransporterCode ?? "N/A"} Xe tải:{tr.TruckPlateNo ?? "N/A"}");
        }
        await db.SaveChangesAsync();

        return new
        {
            tr.TransportReqNo,
            tr.DealerCode,
            tr.TransporterCode,
            tr.TruckPlateNo,
            tr.Status,
            totalVins = vins.Count,
            vins
        };
    }

    public async Task<object> ListTransportRequestsAsync(string? status, string? dealer, string? transporter, string? vin)
    {
        var q = db.TransportRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(r => r.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(transporter)) q = q.Where(r => r.TransporterCode == transporter);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.TransportRequestLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.TransportReqNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.TransportReqNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.TransportReqNo,
            r.DealerCode,
            r.TransporterCode,
            r.TransportContractNo,
            r.TruckPlateNo,
            r.DriverName,
            r.DriverPhone,
            r.FromStorage,
            r.ToStorage,
            r.EstimatedDeparture,
            r.EstimatedArrival,
            r.Status,
            r.Remark,
            r.CreatedAt,
            r.ApprovedAt,
            r.DispatchedAt,
            r.CompletedAt,
            vinCount = db.TransportRequestLines.Count(l => l.OrgId == Org && l.TransportRequestId == r.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetTransportRequestAsync(string transportReqNo)
    {
        transportReqNo = transportReqNo.Trim().ToUpperInvariant();
        var tr = await db.TransportRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.TransportReqNo == transportReqNo);
        if (tr is null) return null;

        var lines = await db.TransportRequestLines.Where(l => l.OrgId == Org && l.TransportRequestId == tr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.DeliveryOrderNo,
            l.StorageCode,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.DealerCode } : null
        }).ToList();

        return new
        {
            tr.TransportReqNo,
            tr.DealerCode,
            tr.TransporterCode,
            tr.TransportContractNo,
            tr.TruckPlateNo,
            tr.DriverName,
            tr.DriverPhone,
            tr.FromStorage,
            tr.ToStorage,
            tr.EstimatedDeparture,
            tr.EstimatedArrival,
            tr.Status,
            tr.Remark,
            tr.CreatedAt,
            tr.ApprovedAt,
            tr.DispatchedAt,
            tr.CompletedAt,
            vins = details
        };
    }

    public async Task<object?> TransportRequestTransitionAsync(string transportReqNo, string action, TransportRequestTransitionDto? dto)
    {
        transportReqNo = transportReqNo.Trim().ToUpperInvariant();
        var tr = await db.TransportRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.TransportReqNo == transportReqNo);
        if (tr is null) return null;

        var now = DateTime.Now;
        var lines = await db.TransportRequestLines.Where(l => l.OrgId == Org && l.TransportRequestId == tr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (tr.Status != "Pending") return null;
                tr.Status = "Approved";
                tr.ApprovedAt = now;
                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "TransportReqApproved", transportReqNo);
                break;

            case "dispatch":
            case "ship":
                if (tr.Status != "Approved") return null;
                tr.Status = "InTransit";
                tr.DispatchedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.TruckPlateNo)) tr.TruckPlateNo = dto.TruckPlateNo.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.DriverName)) tr.DriverName = dto.DriverName.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.DriverPhone)) tr.DriverPhone = dto.DriverPhone.Trim();
                foreach (var l in lines) l.Status = "InTransit";
                foreach (var v in vehicles)
                {
                    if (v.Status == VehicleStatus.Allocated || v.Status == VehicleStatus.InStock)
                        v.Status = VehicleStatus.OnDelivery;
                    Log(v.Vin, "TransportReqInTransit", $"{transportReqNo} Xe:{tr.TruckPlateNo ?? "N/A"}");
                }
                break;

            case "complete":
            case "receive":
            case "deliver":
                if (tr.Status is not ("Approved" or "InTransit")) return null;
                tr.Status = "Completed";
                tr.CompletedAt = now;
                foreach (var l in lines) l.Status = "Delivered";
                foreach (var v in vehicles) Log(v.Vin, "TransportReqCompleted", $"{transportReqNo} Hạ tải tại: {tr.ToStorage ?? tr.DealerCode}");
                break;

            case "reject":
                if (tr.Status != "Pending") return null;
                tr.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tr.Remark = string.IsNullOrWhiteSpace(tr.Remark) ? dto.Note : $"{tr.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "TransportReqRejected", $"{transportReqNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (tr.Status is not ("Pending" or "Approved")) return null;
                tr.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tr.Remark = string.IsNullOrWhiteSpace(tr.Remark) ? dto.Note : $"{tr.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "TransportReqCancelled", $"{transportReqNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { tr.TransportReqNo, tr.DealerCode, status = tr.Status, tr.ApprovedAt, tr.DispatchedAt, tr.CompletedAt };
    }

    // ===== Lệnh tái sắp xếp / Đảo chuyển kho bãi nội bộ OEM (BizHTC.Storage.StorageRearrange / Sto_StorageRearrange) =====
    public async Task<object> CreateStorageRearrangeAsync(CreateStorageRearrangeDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe (VIN và vị trí đích StorageCodeTo) để tạo lệnh tái sắp xếp kho.");

        var distinctItems = dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin) && !string.IsNullOrWhiteSpace(i.StorageCodeTo))
            .GroupBy(i => i.Vin.Trim().ToUpperInvariant())
            .Select(g => g.First())
            .ToList();

        if (distinctItems.Count == 0)
            throw new InvalidOperationException("Danh sách xe không hợp lệ (cần VIN và StorageCodeTo).");

        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Kiểm tra xe không được ở trạng thái đã giao cho khách lẻ (Delivered)
        var invalidDelivered = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (invalidDelivered.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách (Delivered) không thể tái sắp xếp trong kho bãi: " + string.Join(", ", invalidDelivered));

        var reqNo = string.IsNullOrWhiteSpace(dto.StorageRearrangeNo)
            ? "SRR" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.StorageRearrangeNo.Trim().ToUpperInvariant();

        if (await db.StorageRearranges.AnyAsync(r => r.OrgId == Org && r.StorageRearrangeNo == reqNo))
            throw new InvalidOperationException($"Mã lệnh tái sắp xếp kho {reqNo} đã tồn tại.");

        var vMap = vehicles.ToDictionary(v => v.Vin);
        var srr = new StorageRearrange
        {
            OrgId = Org,
            StorageRearrangeNo = reqNo,
            Reason = dto.Reason?.Trim(),
            Remark = dto.Remark?.Trim(),
            Status = "Requested",
            CreatedAt = DateTime.Now
        };
        db.StorageRearranges.Add(srr);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var targetStorage = item.StorageCodeTo.Trim().ToUpperInvariant();
            var currentStorage = item.StorageCodeFrom?.Trim() ?? (vMap.TryGetValue(vin, out var v) ? v.StorageCode : null) ?? "YARD-DEFAULT";

            db.StorageRearrangeLines.Add(new StorageRearrangeLine
            {
                OrgId = Org,
                StorageRearrangeId = srr.Id,
                StorageRearrangeNo = reqNo,
                Vin = vin,
                StorageCodeFrom = currentStorage,
                StorageCodeTo = targetStorage,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "StorageRearrangeRequested", $"{reqNo} Chuyển bãi: {currentStorage} -> {targetStorage}");
        }
        await db.SaveChangesAsync();

        return new
        {
            srr.StorageRearrangeNo,
            srr.Reason,
            srr.Remark,
            srr.Status,
            totalVins = distinctItems.Count,
            items = distinctItems.Select(i => new { i.Vin, i.StorageCodeTo })
        };
    }

    public async Task<object> ListStorageRearrangesAsync(string? status, string? vin, string? storageCodeTo)
    {
        var q = db.StorageRearranges.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.StorageRearrangeLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.StorageRearrangeNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.StorageRearrangeNo));
        }
        if (!string.IsNullOrWhiteSpace(storageCodeTo))
        {
            var st = storageCodeTo.Trim().ToUpperInvariant();
            var matchedNos = await db.StorageRearrangeLines.Where(l => l.OrgId == Org && l.StorageCodeTo == st).Select(l => l.StorageRearrangeNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.StorageRearrangeNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.StorageRearrangeNo,
            r.Reason,
            r.Remark,
            r.Status,
            r.CreatedAt,
            r.ApprovedAt,
            r.CompletedAt,
            vinCount = db.StorageRearrangeLines.Count(l => l.OrgId == Org && l.StorageRearrangeId == r.Id),
            completedCount = db.StorageRearrangeLines.Count(l => l.OrgId == Org && l.StorageRearrangeId == r.Id && l.Status == "Completed")
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetStorageRearrangeAsync(string storageRearrangeNo)
    {
        storageRearrangeNo = storageRearrangeNo.Trim().ToUpperInvariant();
        var srr = await db.StorageRearranges.FirstOrDefaultAsync(r => r.OrgId == Org && r.StorageRearrangeNo == storageRearrangeNo);
        if (srr is null) return null;

        var lines = await db.StorageRearrangeLines.Where(l => l.OrgId == Org && l.StorageRearrangeId == srr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.StorageCodeFrom,
            l.StorageCodeTo,
            l.RearrangeStartDate,
            l.RearrangeEndDate,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), currentStorage = v.StorageCode, v.DealerCode } : null
        }).ToList();

        return new
        {
            srr.StorageRearrangeNo,
            srr.Reason,
            srr.Remark,
            srr.Status,
            srr.CreatedAt,
            srr.ApprovedAt,
            srr.CompletedAt,
            vins = details
        };
    }

    public async Task<object?> StorageRearrangeTransitionAsync(string storageRearrangeNo, string action, StorageRearrangeTransitionDto? dto)
    {
        storageRearrangeNo = storageRearrangeNo.Trim().ToUpperInvariant();
        var srr = await db.StorageRearranges.FirstOrDefaultAsync(r => r.OrgId == Org && r.StorageRearrangeNo == storageRearrangeNo);
        if (srr is null) return null;

        var now = DateTime.Now;
        var lines = await db.StorageRearrangeLines.Where(l => l.OrgId == Org && l.StorageRearrangeId == srr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();
        var vMap = vehicles.ToDictionary(v => v.Vin);

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (srr.Status != "Requested") return null;
                srr.Status = "Approved";
                srr.ApprovedAt = now;
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "StorageRearrangeApproved", storageRearrangeNo);
                break;

            case "start":
            case "move":
                if (srr.Status is not ("Approved" or "Requested")) return null;
                srr.Status = "InProgress";
                srr.ApprovedAt ??= now;
                foreach (var l in lines)
                {
                    if (l.Status is "Pending" or "Approved")
                    {
                        l.Status = "Moving";
                        l.RearrangeStartDate ??= now;
                    }
                }
                foreach (var v in vehicles) Log(v.Vin, "StorageRearrangeMoving", storageRearrangeNo);
                break;

            case "complete":
                if (srr.Status is not ("Approved" or "InProgress")) return null;
                srr.Status = "Completed";
                srr.CompletedAt = now;
                foreach (var l in lines)
                {
                    if (l.Status != "Completed")
                    {
                        l.Status = "Completed";
                        l.RearrangeStartDate ??= now;
                        l.RearrangeEndDate = now;
                        if (vMap.TryGetValue(l.Vin, out var v))
                        {
                            v.StorageCode = l.StorageCodeTo;
                            Log(v.Vin, "StorageRearrangeCompleted", $"{storageRearrangeNo} Đã chuyển đến ô/bãi: {l.StorageCodeTo}");
                        }
                    }
                }
                break;

            case "reject":
                if (srr.Status != "Requested") return null;
                srr.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    srr.Remark = string.IsNullOrWhiteSpace(srr.Remark) ? dto.Note : $"{srr.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "StorageRearrangeRejected", $"{storageRearrangeNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (srr.Status is not ("Requested" or "Approved" or "InProgress")) return null;
                srr.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    srr.Remark = string.IsNullOrWhiteSpace(srr.Remark) ? dto.Note : $"{srr.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) if (l.Status != "Completed") l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "StorageRearrangeCancelled", $"{storageRearrangeNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { srr.StorageRearrangeNo, status = srr.Status, srr.ApprovedAt, srr.CompletedAt };
    }

    public async Task<object?> CompleteStorageRearrangeLineAsync(string storageRearrangeNo, string vin, CompleteStorageRearrangeLineDto? dto)
    {
        storageRearrangeNo = storageRearrangeNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var srr = await db.StorageRearranges.FirstOrDefaultAsync(r => r.OrgId == Org && r.StorageRearrangeNo == storageRearrangeNo);
        if (srr is null || srr.Status is "Cancelled" or "Rejected") return null;

        var line = await db.StorageRearrangeLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.StorageRearrangeId == srr.Id && l.Vin == vin);
        if (line is null) return null;

        var now = DateTime.Now;
        DateTime endDate = now;
        if (!string.IsNullOrWhiteSpace(dto?.RearrangeEndDate) && DateTime.TryParse(dto.RearrangeEndDate, out var parsedDate))
            endDate = parsedDate;

        line.Status = "Completed";
        line.RearrangeStartDate ??= now;
        line.RearrangeEndDate = endDate;
        if (!string.IsNullOrWhiteSpace(dto?.Remark))
            line.Remark = string.IsNullOrWhiteSpace(line.Remark) ? dto.Remark : $"{line.Remark} | {dto.Remark}";

        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == Org && v.Vin == vin);
        if (vehicle != null)
        {
            vehicle.StorageCode = line.StorageCodeTo;
            Log(vin, "StorageRearrangeLineCompleted", $"{storageRearrangeNo} Đã di dời thành công vào bãi/slot: {line.StorageCodeTo}");
        }

        // Cập nhật trạng thái tổng thể của lệnh
        var allLines = await db.StorageRearrangeLines.Where(l => l.OrgId == Org && l.StorageRearrangeId == srr.Id).ToListAsync();
        if (allLines.All(l => l.Status == "Completed"))
        {
            srr.Status = "Completed";
            srr.CompletedAt = now;
        }
        else if (srr.Status == "Approved" || srr.Status == "Requested")
        {
            srr.Status = "InProgress";
        }

        await db.SaveChangesAsync();
        return new
        {
            srr.StorageRearrangeNo,
            line.Vin,
            line.StorageCodeFrom,
            line.StorageCodeTo,
            line.Status,
            line.RearrangeEndDate,
            headerStatus = srr.Status
        };
    }
}
