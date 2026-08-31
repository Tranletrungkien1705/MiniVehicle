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
