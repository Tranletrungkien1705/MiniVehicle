namespace MiniVehicle.Models;

/// <summary>Tổ chức (hãng/nhà phân phối). Mỗi org 1 ApiKey.</summary>
public sealed class Org
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Vòng đời xe (chuyển đổi từ BizHTC.Car_Car): kho → phân bổ ĐL → lệnh giao → đã giao (kích hoạt BH).</summary>
public enum VehicleStatus { InStock = 0, Allocated = 1, OnDelivery = 2, Delivered = 3 }

/// <summary>Hồ sơ xe theo VIN (BizHTC.Car_Car): master + trạng thái vòng đời + chủ xe + bảo hành.</summary>
public sealed class Vehicle
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Vin { get; set; } = "";           // số khung (duy nhất theo org)
    public string Model { get; set; } = "";
    public string? EngineNo { get; set; }           // số máy
    public string? Color { get; set; }
    public int? ModelYear { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.InStock;
    public string? DealerCode { get; set; }         // đại lý được phân bổ/giao
    public string? OwnerName { get; set; }
    public string? OwnerPhone { get; set; }
    public string? PlateNo { get; set; }            // biển số (sau đăng ký)
    public DateTime? DeliveredAt { get; set; }
    public DateTime? WarrantyStart { get; set; }
    public DateTime? WarrantyEnd { get; set; }
    public int WarrantyMonths { get; set; } = 36;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Lệnh giao xe (BizHTC.Car_DeliveryOrder): 1 phiếu giao nhiều VIN cho 1 đại lý.</summary>
public sealed class DeliveryOrder
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DoNo { get; set; } = "";
    public string DealerCode { get; set; } = "";
    public string Status { get; set; } = "Open";    // Open → Delivered
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DeliveredAt { get; set; }
}

/// <summary>Dòng lệnh giao (BizHTC.Car_DeliveryOrderDetail): 1 VIN trong 1 lệnh.</summary>
public sealed class DeliveryOrderLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DeliveryOrderId { get; set; }
    public string Vin { get; set; } = "";
}

/// <summary>Chiến dịch triệu hồi (recall) — theo model, kèm lý do/hành động khắc phục.</summary>
public sealed class RecallCampaign
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Model { get; set; }          // lọc xe bị ảnh hưởng theo model (rỗng = chỉ định VIN thủ công)
    public string? Reason { get; set; }
    public string? Remedy { get; set; }          // hành động khắc phục
    public string Status { get; set; } = "Open"; // Open → Closed
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Xe bị ảnh hưởng bởi 1 chiến dịch triệu hồi + tiến độ khắc phục.</summary>
public sealed class VehicleRecall
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CampaignId { get; set; }
    public string Vin { get; set; } = "";
    public string Status { get; set; } = "Open"; // Open → Done
    public DateTime? DoneAt { get; set; }
    public string? DoneBy { get; set; }          // đại lý thực hiện
}

/// <summary>Yêu cầu bảo hành (GrtClaim): đại lý mở cho xe còn BH → hãng duyệt/từ chối → quyết toán.</summary>
public sealed class WarrantyClaim
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ClaimNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string DealerCode { get; set; } = "";
    public string Issue { get; set; } = "";
    public decimal PartsCost { get; set; }
    public decimal LaborCost { get; set; }
    public string Status { get; set; } = "Submitted";  // Submitted → Approved/Rejected → Settled
    public string? DecisionNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DecidedAt { get; set; }
    public DateTime? SettledAt { get; set; }
}

/// <summary>Đề nghị giao tài liệu xe (CarDocReq/ĐNGT): ĐL xin hồ sơ (đăng ký/hóa đơn/COC/BH) sau giao → HQ duyệt → giao → nhận.</summary>
public sealed class DocRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Code { get; set; } = "";
    public string Vin { get; set; } = "";
    public string DealerCode { get; set; } = "";
    public string DocType { get; set; } = "Registration";  // Registration/Invoice/COC/Warranty
    public string Status { get; set; } = "Requested";       // Requested → Approved → Shipped → Received (hoặc Rejected)
    public string? Note { get; set; }
    public string? TrackingNo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}

/// <summary>Điều chuyển xe giữa đại lý/kho (chuyển kho DMS): xe CHƯA giao mới được chuyển.</summary>
public sealed class StockTransfer
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Code { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? FromDealer { get; set; }
    public string ToDealer { get; set; } = "";
    public string Status { get; set; } = "Requested";   // Requested → Approved → InTransit → Received (hoặc Rejected)
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}

/// <summary>Mốc lịch sử vòng đời xe (audit) — thay cho việc dò log rời.</summary>
public sealed class VehicleEvent
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Vin { get; set; } = "";
    public string Kind { get; set; } = "";          // Created/Allocated/DeliveryOrder/Delivered/Recall...
    public string? Note { get; set; }
    public DateTime At { get; set; } = DateTime.Now;
}
