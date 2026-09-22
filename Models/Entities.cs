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
    public bool IsTestCar { get; set; } = false;    // Đang phục vụ chương trình chạy thử / lái thử (FlagTestCar)
    public string? StorageCode { get; set; }        // vị trí ô đỗ / kho bãi nội bộ OEM (StorageCodeCurrent)
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

/// <summary>Biên bản giao nhận bàn giao xe (BizHTC.Storage.DlvMinutes / Sto_DlvMinutes): kiểm tra tình trạng ngoại thất, phụ kiện, giấy tờ, km ODO khi bàn giao xe.</summary>
public sealed class DeliveryMinutes
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DlvMnNo { get; set; } = "";             // Số biên bản (DMN...)
    public string Vin { get; set; } = "";                 // Số khung bàn giao
    public string DealerCode { get; set; } = "";          // Đại lý nhận xe
    public string? DoNo { get; set; }                     // Lệnh giao xe liên quan (nếu có)
    public string? TransporterCode { get; set; }          // Đơn vị vận chuyển / nhà xe
    public string? DriverName { get; set; }               // Lái xe chuyên dụng chở xe
    public string? DriverPhone { get; set; }              // SĐT lái xe
    public string? TruckPlateNo { get; set; }             // Biển số xe lồng chở xe
    public string? FromStorage { get; set; }              // Kho xuất phát
    public string? ToStorage { get; set; }                // Kho/Điểm đến
    public int OdoKm { get; set; }                        // Số km ODO lúc giao nhận
    public string? ExteriorCondition { get; set; }        // Tình trạng ngoại thất, thân vỏ, sơn, kính, đèn (OK / Xước...)
    public string? InteriorCondition { get; set; }        // Tình trạng nội thất, ghế, táp-lô, điều hòa (OK / ...)
    public bool HasSpareWheel { get; set; } = true;       // Có lốp sơ cua
    public bool HasToolKit { get; set; } = true;          // Có bộ đồ nghề kích lốp
    public int KeyCount { get; set; } = 2;                // Số lượng chìa khóa (thường là 2 chìa)
    public bool HasGuarantyBooklet { get; set; } = true;  // Sổ bảo hành
    public bool HasUserManual { get; set; } = true;       // Sách hướng dẫn sử dụng
    public bool HasOriginalCertificate { get; set; } = true; // Giấy chứng nhận chất lượng XK
    public string? DeliveredBy { get; set; }              // Đại diện bên giao (thủ kho / lái xe)
    public string? ReceivedBy { get; set; }               // Đại diện bên nhận (KTV / Cố vấn ĐL)
    public string Status { get; set; } = "Draft";         // Draft → Inspected → Confirmed (hoặc Rejected)
    public string? Remark { get; set; }                   // Ghi chú chi tiết khi giao nhận
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? HandoverDate { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

/// <summary>Lệnh thu hồi / nhập trả xe về kho (BizHTC.Storage.CarRetrieve / Sto_CarRetrieve): thu hồi xe từ đại lý về lại kho trung tâm OEM.</summary>
public sealed class CarRetrieve
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string RetrieveNo { get; set; } = "";        // Mã lệnh thu hồi (RET...)
    public string DealerCode { get; set; } = "";        // Đại lý bị thu hồi / trả xe
    public string? ToStorage { get; set; }              // Kho tiếp nhận xe thu hồi
    public string? Reason { get; set; }                 // Lý do thu hồi (quá hạn thanh toán, hoàn kho, hủy phân bổ...)
    public string Status { get; set; } = "Requested";   // Requested → Approved → InTransit → Received (hoặc Rejected / Cancelled)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}

/// <summary>Chi tiết xe trong lệnh thu hồi (Sto_CarRetrieveDetail): danh sách VIN thu hồi.</summary>
public sealed class CarRetrieveLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CarRetrieveId { get; set; }
    public string RetrieveNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? StorageCode { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu / Kế hoạch vận chuyển xe ô tô (BizHTC.Car.TransportReq / Car_TransportReq): điều phối nhà xe lồng / vận tải chở ô tô từ kho OEM đến đại lý.</summary>
public sealed class TransportRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TransportReqNo { get; set; } = "";        // Mã lệnh vận chuyển (TR... / CTR...)
    public string DealerCode { get; set; } = "";            // Đại lý nhận xe
    public string? TransporterCode { get; set; }          // Đơn vị / Nhà xe vận chuyển (NYK, Traco...)
    public string? TransportContractNo { get; set; }      // Số hợp đồng vận chuyển
    public string? TruckPlateNo { get; set; }             // Biển số xe tải / xe lồng chuyên dụng
    public string? DriverName { get; set; }               // Tên lái xe lồng
    public string? DriverPhone { get; set; }              // SĐT lái xe
    public string? FromStorage { get; set; }              // Kho bãi xuất phát
    public string? ToStorage { get; set; }                // Kho / Điểm hạ tải đại lý
    public DateTime? EstimatedDeparture { get; set; }     // Ngày dự kiến xuất bến
    public DateTime? EstimatedArrival { get; set; }       // Ngày dự kiến đến nơi
    public string Status { get; set; } = "Pending";       // Pending → Approved → InTransit → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú điều vận
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DispatchedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Chi tiết xe trong lệnh vận chuyển (Car_TransportReqDetail): danh sách VIN và liên kết lệnh giao xe.</summary>
public sealed class TransportRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TransportRequestId { get; set; }
    public string TransportReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? DeliveryOrderNo { get; set; }          // Liên kết Lệnh giao xe (nếu có)
    public string? StorageCode { get; set; }              // Kho xuất phát của xe
    public string Status { get; set; } = "Pending";       // Pending → Approved → InTransit → Delivered
    public string? Remark { get; set; }
}

/// <summary>Lệnh tái sắp xếp / đảo chuyển vị trí bãi đỗ ô tô nội bộ OEM (BizHTC.Storage.StorageRearrange / Sto_StorageRearrange): quản lý di dời xe giữa các bãi/ô đỗ trong kho trung tâm OEM.</summary>
public sealed class StorageRearrange
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string StorageRearrangeNo { get; set; } = "";      // Mã lệnh tái sắp xếp (SRR...)
    public string? Reason { get; set; }                       // Lý do sắp xếp / quy hoạch lại kho bãi
    public string? Remark { get; set; }                       // Ghi chú điều hành
    public string Status { get; set; } = "Requested";         // Requested → Approved → InProgress → Completed (hoặc Rejected / Cancelled)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Chi tiết xe trong lệnh tái sắp xếp kho bãi (Sto_StorageRearrangeDetail): danh sách VIN, vị trí cũ và vị trí mới.</summary>
public sealed class StorageRearrangeLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long StorageRearrangeId { get; set; }
    public string StorageRearrangeNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? StorageCodeFrom { get; set; }              // Vị trí/bãi đỗ cũ của xe
    public string StorageCodeTo { get; set; } = "";           // Vị trí/bãi đỗ mới chuyển đến
    public DateTime? RearrangeStartDate { get; set; }         // Thời điểm bắt đầu di dời xe
    public DateTime? RearrangeEndDate { get; set; }           // Thời điểm hoàn tất di dời vào vị trí mới
    public string Status { get; set; } = "Pending";           // Pending → Approved → Moving → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                       // Ghi chú tình trạng vị trí ô đỗ
}

/// <summary>Đăng ký xe lái thử / mượn xe chạy thử (BizHTC.Car.Car_TestCar / TestCar): quản lý điều phối và bàn giao xe cho đại lý hoặc sự kiện lái thử (Roadshow / Test Drive).</summary>
public sealed class TestCarRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TestCarCode { get; set; } = "";             // Mã lệnh / phiếu đăng ký xe lái thử (TC...)
    public string DealerCode { get; set; } = "";              // Đại lý đăng ký mượn / nhận xe lái thử
    public string? EventName { get; set; }                    // Tên sự kiện / Chương trình trải nghiệm lái thử
    public string? Purpose { get; set; }                      // Mục đích (Trưng bày, Lái thử khách hàng, Test Drive Event...)
    public DateTime? StartDate { get; set; }                  // Ngày bắt đầu đợt lái thử
    public DateTime? EndDate { get; set; }                    // Ngày dự kiến kết thúc / bàn giao lại
    public string Status { get; set; } = "Pending";           // Pending → Approved → InUse → Finished (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                       // Ghi chú điều hành
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}

/// <summary>Chi tiết xe trong lệnh đăng ký lái thử (Car_TestCarDtl): danh sách VIN, chỉ số ODO xuất phát - kết thúc, tình trạng xe.</summary>
public sealed class TestCarLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TestCarRequestId { get; set; }
    public string TestCarCode { get; set; } = "";
    public string Vin { get; set; } = "";
    public int OdoStart { get; set; } = 0;                    // ODO lúc bắt đầu đợt lái thử
    public int? OdoEnd { get; set; }                          // ODO lúc kết thúc hoàn trả xe
    public string? ConditionStart { get; set; }               // Tình trạng ngoại thất / nội thất lúc nhận xe
    public string? ConditionEnd { get; set; }                 // Tình trạng xe lúc hoàn trả
    public DateTime? HandoverDate { get; set; }               // Ngày bàn giao xe thực tế
    public DateTime? ReturnDate { get; set; }                 // Ngày trả lại xe thực tế
    public string Status { get; set; } = "Pending";           // Pending → Approved → InUse → Finished (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
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
