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
    public bool IsMortgaged { get; set; } = false;  // Đang thế chấp bảo lãnh ngân hàng (FlagMortageBank / RM_ReqMortgage)
    public string? MortgageBankCode { get; set; }   // Mã ngân hàng nhận thế chấp (VCB, VPB, TCB, BIDV, CTG...)
    public DateTime? MortgageDate { get; set; }     // Ngày bắt đầu thế chấp ngân hàng
    public DateTime? RedeemDate { get; set; }       // Ngày giải chấp / rút thế chấp (RD_ReqRedeem)
    public bool IsPaid { get; set; } = false;       // Đã hoàn tất thanh toán tiền xe cho OEM (Pmt_Payment / Finished)
    public decimal PaidAmount { get; set; } = 0;    // Tổng số tiền đã thanh toán cho xe (VNĐ)
    public DateTime? PaidAt { get; set; }           // Thời điểm hoàn tất thanh toán tiền xe
    public string? StorageCode { get; set; }        // vị trí ô đỗ / kho bãi nội bộ OEM (StorageCodeCurrent)
    public string? PackingListNo { get; set; }      // Mã Packing List xuất xưởng / cập cảng (CT_PackingList)
    public string? DeclarationNo { get; set; }      // Số tờ khai hải quan thông quan (CT_Declaration / CT_TKHQ)
    public DateTime? TaxPaymentDate { get; set; }   // Ngày hoàn tất nộp thuế hải quan (ContractTKHQUpdate_TaxPaymentDate)
    public bool IsCustomsCleared { get; set; } = false; // Đã hoàn tất thủ tục thông quan hải quan
    public DateTime? CustomsClearanceDate { get; set; } // Ngày hoàn tất thông quan hải quan
    public DateTime? LastStorageMtnDate { get; set; } // Ngày bảo dưỡng lưu kho gần nhất (VIN_MaintainPeriod.MtnLastDate)
    public DateTime? NextStorageMtnDate { get; set; } // Hạn bảo dưỡng lưu kho tiếp theo (VIN_MaintainPeriod.MtnNextDate)
    public int StorageMtnTimes { get; set; } = 0;     // Số lần bảo dưỡng lưu kho đã thực hiện (VIN_MaintainPeriod.MtnTimes)
    public string? SOCode { get; set; }             // Đơn đặt hàng SO được phân bổ (Ord_SalesOrder)
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

/// <summary>Yêu cầu kiểm tra tiền bàn giao xe PDI (BizHTC.WH.DlrPDIRequest / Dlr_PDIRequest): quy trình kiểm tra chất lượng trước khi bàn giao xe hoặc giao cho khách hàng cuối.</summary>
public sealed class PdiRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PdiReqNo { get; set; } = "";             // Mã phiếu yêu cầu PDI (PDI...)
    public string DealerCode { get; set; } = "";          // Đại lý tạo yêu cầu PDI
    public string? InspectorName { get; set; }            // Kỹ thuật viên / Chuyên viên PDI phụ trách
    public string? ApprovedBy { get; set; }               // Người phê duyệt / Quản đốc xưởng
    public string Status { get; set; } = "Pending";       // Pending → Approved → InProgress → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú điều hành / yêu cầu PDI
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Chi tiết xe trong phiếu kiểm tra PDI (Dlr_PDIRequestDtl): danh sách hạng mục kiểm tra chất lượng tiền bàn giao (ắc quy, lốp, mức dầu/nước, điện tử, ngoại thất, nội thất, chẩn đoán OBD, số RO xưởng).</summary>
public sealed class PdiRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long PdiRequestId { get; set; }
    public string PdiReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? DlrContractNo { get; set; }            // Hợp đồng bán xe liên quan
    public string? RoNo { get; set; }                     // Số Repair Order / Lệnh dịch vụ xưởng nếu có phát sinh sửa chữa
    public string RoStatus { get; set; } = "NORE";        // NORE (Chưa tạo), CRE (Mới tạo), FNS (Hoàn thành)
    public double? BatteryVoltage { get; set; } = 12.6;   // Điện áp bình ắc quy (V)
    public bool TirePressureOk { get; set; } = true;      // Đạt chuẩn áp suất lốp
    public bool FluidsOk { get; set; } = true;            // Đạt chuẩn mức dầu mỡ, nước làm mát, nước rửa kính
    public bool ElectronicsOk { get; set; } = true;       // Đạt chuẩn hệ thống điện tử, đèn, còi, màn hình AVN
    public bool ExteriorOk { get; set; } = true;          // Đạt chuẩn ngoại thất, thân vỏ, kính không trầy xước
    public bool InteriorCleanOk { get; set; } = true;     // Đạt chuẩn vệ sinh nội thất & lột bỏ nilon bảo vệ
    public bool DiagnosticScanOk { get; set; } = true;    // Quét chẩn đoán OBD/ECU không có mã lỗi (No DTC)
    public string PdiResult { get; set; } = "Pending";    // Pending → Passed / Failed
    public string Status { get; set; } = "Pending";       // Pending → Approved → Inspected → Completed (hoặc Rejected / Cancelled)
    public DateTime? InspectedAt { get; set; }            // Thời điểm hoàn tất kiểm tra xe này
    public string? InspectedBy { get; set; }              // KTV thực hiện kiểm tra xe này
    public string? DefectNotes { get; set; }              // Ghi chú khiếm khuyết kỹ thuật nếu không đạt
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu thế chấp xe ô tô vào ngân hàng (BizHTC.GiaiChap.RM_ReqMortgage / RM_ReqMortgage): đưa lô xe vào danh mục tài sản bảo đảm / vay bảo lãnh hạn mức tín dụng ngân hàng.</summary>
public sealed class MortgageRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ReqMortgageNo { get; set; } = "";        // Mã yêu cầu thế chấp (RM...)
    public string BankCode { get; set; } = "";             // Mã ngân hàng nhận thế chấp (VCB, VPB, TCB, BIDV, CTG...)
    public DateTime? MortgageDate { get; set; }            // Ngày hiệu lực thế chấp
    public string Status { get; set; } = "Pending";        // Pending → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú hợp đồng tín dụng / gói bảo lãnh
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>Chi tiết xe trong yêu cầu thế chấp (RM_ReqMortgageDtl): danh sách VIN và định giá giá trị thế chấp.</summary>
public sealed class MortgageRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long MortgageRequestId { get; set; }
    public string ReqMortgageNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public decimal MortgageAmount { get; set; } = 0;       // Giá trị định giá thế chấp / mức giải ngân bảo lãnh
    public string Status { get; set; } = "Pending";        // Pending → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu giải chấp xe ngân hàng (BizHTC.GiaiChap.RD_ReqRedeem / RD_ReqRedeem): yêu cầu rút xe / giải chấp tài sản bảo lãnh ngân hàng để giao xe cho khách hàng hoặc xuất kho bán lẻ.</summary>
public sealed class RedeemRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string RedeemReqNo { get; set; } = "";          // Mã yêu cầu giải chấp (RD...)
    public string DealerCode { get; set; } = "";           // Đại lý yêu cầu giải chấp
    public string BankCode { get; set; } = "";             // Ngân hàng giải chấp
    public string? ReqMortgageNo { get; set; }             // Liên kết mã yêu cầu thế chấp trước đó (nếu có)
    public string? Reason { get; set; }                    // Lý do giải chấp (Khách hàng thanh toán 100%, Giải ngân bán lẻ, Đảo tài sản...)
    public string Status { get; set; } = "Pending";        // Pending → Approved → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú điều hành
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Chi tiết xe trong yêu cầu giải chấp (RD_ReqRedeemDtl): danh sách VIN và loại chứng từ giấy tờ giải phóng (Hóa đơn, COC, Đăng kiểm...).</summary>
public sealed class RedeemRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long RedeemRequestId { get; set; }
    public string RedeemReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string ReleaseDocType { get; set; } = "All";    // All, COC, Invoice, QualityCert, Registration
    public string Status { get; set; } = "Pending";        // Pending → Approved → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Đơn đặt hàng xe ô tô của Đại lý (BizHTC.Order.Ord_SalesOrder / SalesOrder): kế hoạch đặt xe theo tháng/quý của đại lý gửi lên hãng xe OEM.</summary>
public sealed class SalesOrder
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SOCode { get; set; } = "";             // Mã đơn đặt hàng (SO...)
    public string SOType { get; set; } = "Normal";       // Loại đơn: Normal (Kế hoạch tháng), Urgent (Giao khẩn), Special (Dự án/Lô lớn), Display (Trưng bày showroom)
    public string DealerCode { get; set; } = "";         // Đại lý đặt mua xe
    public string? SPCode { get; set; }                  // Mã chính sách bán hàng / Sales Policy
    public string? OrderMonth { get; set; }              // Tháng đặt hàng (yyyy-MM)
    public string? ProductionMonth { get; set; }         // Tháng kế hoạch sản xuất (yyyy-MM)
    public string? ExpectedMonth { get; set; }           // Tháng dự kiến giao xe (yyyy-MM)
    public int TotalOrderQty { get; set; } = 0;          // Tổng số lượng xe đặt
    public int TotalApprovedQty { get; set; } = 0;       // Tổng số lượng xe duyệt cấp
    public int TotalAllocatedQty { get; set; } = 0;      // Tổng số lượng xe đã gán/phân bổ VIN thực tế
    public decimal TotalAmount { get; set; } = 0;        // Tổng giá trị đơn hàng
    public string Status { get; set; } = "Draft";        // Draft → Submitted → Approved1 → Approved2 / Approved (hoặc Rejected / Cancelled)
    public string? CreatedBy { get; set; }
    public string? ApprovedBy1 { get; set; }             // Người duyệt cấp 1 (Kế hoạch kinh doanh/sản xuất)
    public DateTime? ApprovedAt1 { get; set; }
    public string? ApprovedBy2 { get; set; }             // Người duyệt cấp 2 (Ban Giám Đốc/Tài chính)
    public DateTime? ApprovedAt2 { get; set; }
    public string? Remark { get; set; }                  // Ghi chú đơn đặt hàng
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Chi tiết dòng xe trong đơn đặt hàng (BizHTC.Order.Ord_SalesOrderDetail / SalesOrderLine): dòng model, phiên bản spec, màu sắc, số lượng và đơn giá.</summary>
public sealed class SalesOrderLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesOrderId { get; set; }
    public string SOCode { get; set; } = "";
    public string Model { get; set; } = "";              // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra, Custin, Palisade...)
    public string? SpecCode { get; set; }                // Phiên bản xe (1.6T, 2.0 AT Tiêu chuẩn, 2.0 AT Đặc biệt, Hybrid...)
    public string? Color { get; set; }                   // Màu xe (Trắng, Đen, Đỏ, Bạc, Xanh, Vàng cát...)
    public int OrderQty { get; set; } = 1;               // Số lượng xe đại lý đặt
    public int ApprovedQty { get; set; } = 0;            // Số lượng xe hãng duyệt
    public int AllocatedQty { get; set; } = 0;           // Số lượng VIN thực tế đã gán
    public decimal UnitPrice { get; set; } = 0;          // Đơn giá xe (VNĐ)
    public decimal TotalAmount { get; set; } = 0;        // Thành tiền (VNĐ)
    public string Status { get; set; } = "Pending";      // Pending → Approved → PartiallyAllocated → FullyAllocated (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Giao dịch bán lẻ xe ô tô của Đại lý cho Khách hàng (BizHTC.DealerSales / DLS_Deal): hợp đồng bán lẻ tại showroom đại lý, thông tin khách hàng, tư vấn bán hàng TVBH, phương thức trả góp ngân hàng và bàn giao kích hoạt Sổ Bảo Hành Online.</summary>
public sealed class DealerDeal
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DealNo { get; set; } = "";             // Mã giao dịch bán lẻ (DEAL...)
    public string? DealNoUser { get; set; }            // Số hợp đồng bán lẻ của đại lý (HDBL...)
    public string DealerCode { get; set; } = "";         // Đại lý bán xe
    public string? CustomerCode { get; set; }          // Mã khách hàng
    public string CustomerName { get; set; } = "";       // Tên khách hàng / bên mua xe
    public string CustomerPhone { get; set; } = "";      // SĐT khách hàng
    public string CustomerType { get; set; } = "Individual"; // Individual (Cá nhân), Corporate (Doanh nghiệp), Fleet (Dự án/Lô)
    public string? IdNo { get; set; }                  // Số CCCD/Hộ chiếu hoặc MST doanh nghiệp
    public string? Address { get; set; }               // Địa chỉ khách hàng
    public string? SalesManCode { get; set; }          // Mã tư vấn bán hàng (TVBH)
    public string? SalesManName { get; set; }          // Tên tư vấn bán hàng
    public string SalesType { get; set; } = "Retail";  // Kiểu bán: Retail (Bán lẻ), Fleet (Bán dự án), Wholesale (Bán buôn), Staff (Nội bộ)
    public string PaymentType { get; set; } = "Cash";  // Phương thức: Cash (Tiền mặt/Chuyển khoản), BankLoan (Trả góp ngân hàng)
    public string? BankCode { get; set; }              // Ngân hàng tài trợ vay (VCB, TCB, VPB, BIDV, MB...)
    public decimal LoanAmount { get; set; } = 0;       // Số tiền vay trả góp
    public decimal TotalAmount { get; set; } = 0;      // Tổng giá niêm yết các xe
    public decimal DiscountAmount { get; set; } = 0;   // Tổng giảm giá / chiết khấu
    public decimal FinalAmount { get; set; } = 0;      // Tổng tiền thực tế = TotalAmount - DiscountAmount
    public decimal DepositAmount { get; set; } = 0;    // Số tiền đặt cọc
    public DateTime? DealDate { get; set; }            // Ngày giao dịch / ký hợp đồng
    public string Status { get; set; } = "Draft";      // Draft → Submitted → Approved → Delivered (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                // Ghi chú giao dịch
    public string? CreatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }         // Ngày hoàn tất bàn giao xe cho khách
}

/// <summary>Chi tiết xe trong giao dịch bán lẻ (BizHTC.DealerSales / DLS_DealDetail): thông tin xe VIN, giá bán, biển số xe đăng ký, ODO lúc bàn giao và số sổ bảo hành điện tử (SBH Online).</summary>
public sealed class DealerDealLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DealerDealId { get; set; }
    public string DealNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Model { get; set; } = "";
    public string? Color { get; set; }
    public decimal UnitPrice { get; set; } = 0;        // Giá niêm yết xe
    public decimal Discount { get; set; } = 0;         // Giảm giá cho xe
    public decimal Price { get; set; } = 0;            // Giá bán thực tế = UnitPrice - Discount
    public string? PlateNo { get; set; }              // Biển số xe đăng ký (30K-123.45)
    public string? SBHOnlineNo { get; set; }          // Số Sổ bảo hành điện tử online (SBH-...)
    public int DeliveryOdoKm { get; set; } = 10;      // Số km ODO lúc bàn giao xe cho khách
    public DateTime? WarrantyStartDate { get; set; }   // Ngày bắt đầu bảo hành
    public int WarrantyMonths { get; set; } = 36;      // Thời hạn bảo hành (tháng)
    public DateTime? DeliveryDate { get; set; }        // Ngày bàn giao xe thực tế cho khách
    public string Status { get; set; } = "Pending";    // Pending → Approved → Delivered (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Bảo lãnh thanh toán mua xe ô tô của Ngân hàng cho Đại lý (BizHTC.Payment / Pmt_Guarantee): cấp hạn mức bảo lãnh thanh toán cho đại lý nhận xe từ hãng OEM.</summary>
public sealed class PaymentGuarantee
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GuaranteeNo { get; set; } = "";        // Mã chứng thư bảo lãnh (GRT...)
    public string BankGuaranteeNo { get; set; } = "";    // Số thư bảo lãnh ngân hàng (BL-VCB-...)
    public string BankCode { get; set; } = "";           // Mã ngân hàng phát hành (VCB, VPB, TCB, BIDV, CTG, MB...)
    public string BankName { get; set; } = "";           // Tên ngân hàng
    public string DealerCode { get; set; } = "";         // Đại lý thụ hưởng bảo lãnh mua xe
    public DateTime DateOpen { get; set; } = DateTime.Now; // Ngày mở / hiệu lực bảo lãnh
    public DateTime DateExpired { get; set; }            // Ngày hết hạn hiệu lực bảo lãnh
    public int Term { get; set; } = 30;                  // Thời hạn quy định (ngày)
    public int TermActual { get; set; } = 30;            // Thời hạn thực tế (ngày)
    public decimal TotalAmount { get; set; } = 0;        // Tổng hạn mức bảo lãnh (VNĐ)
    public int TotalVehicleCount { get; set; } = 0;      // Tổng số lượng xe được bảo lãnh
    public string Status { get; set; } = "Pending";      // Pending → Approved → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                  // Ghi chú hợp đồng tín dụng/bảo lãnh
    public string? CreatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? SettledAt { get; set; }             // Ngày tất toán giải phóng hoàn toàn bảo lãnh
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong bảo lãnh thanh toán (BizHTC.Payment / Pmt_GuaranteeDetail): danh sách VIN được bảo lãnh, tỷ lệ và giá trị bảo lãnh, thời hạn cảnh báo đáo hạn.</summary>
public sealed class PaymentGuaranteeLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long PaymentGuaranteeId { get; set; }
    public string GuaranteeNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public decimal GuaranteePercent { get; set; } = 100; // Tỷ lệ bảo lãnh (%)
    public decimal GuaranteeValue { get; set; } = 0;     // Giá trị bảo lãnh cho xe này (VNĐ)
    public DateTime? DateStart { get; set; }             // Ngày bắt đầu tính bảo lãnh cho xe
    public DateTime? DateWarning { get; set; }           // Ngày cảnh báo hạn bảo lãnh
    public DateTime? DateExpired { get; set; }           // Ngày hết hạn bảo lãnh xe này
    public string Status { get; set; } = "Pending";      // Pending → Approved → Settled (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Hợp đồng mua bán xe ô tô giữa Hãng OEM và Đại lý phân phối (BizHTC.Contract.DealerContract / CT_DealerContract): hợp đồng bán buôn xe ô tô, cam kết thời hạn giao hàng, hạn mức thanh toán và danh mục VIN xe giao dịch.</summary>
public sealed class DealerContract
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ContractNo { get; set; } = "";             // Mã hợp đồng OEM (CTR...)
    public string? ContractNoUser { get; set; }            // Số hợp đồng nội bộ đại lý (HĐMB-...)
    public string DealerCode { get; set; } = "";           // Mã đại lý ký kết mua xe
    public string? SOCode { get; set; }                    // Mã đơn đặt hàng xe liên quan (Ord_SalesOrder)
    public string ContractType { get; set; } = "Wholesale"; // Wholesale (Bán buôn tiêu chuẩn), Project (Dự án/Lô), Spot (Đột xuất), Principle (Nguyên tắc)
    public DateTime ContractDate { get; set; } = DateTime.Now; // Ngày ký kết hợp đồng
    public DateTime? DeliveryDeadline { get; set; }        // Hạn chót hoàn tất bàn giao toàn bộ xe
    public int PaymentTermDays { get; set; } = 30;         // Thời hạn thanh toán hợp đồng (ngày)
    public int TotalQuantity { get; set; } = 0;            // Tổng số lượng xe trong hợp đồng
    public decimal TotalAmount { get; set; } = 0;          // Tổng giá trị xe niêm yết (VNĐ)
    public decimal DiscountAmount { get; set; } = 0;       // Tổng chiết khấu thương mại / khuyến mãi
    public decimal FinalAmount { get; set; } = 0;          // Tổng giá trị thực tế sau chiết khấu = TotalAmount - DiscountAmount
    public decimal DepositAmount { get; set; } = 0;        // Tiền đặt cọc hợp đồng
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved → Completed (hoặc Rejected / Cancelled)
    public string? CreatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Remark { get; set; }                    // Điều khoản / ghi chú hợp đồng
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong hợp đồng mua bán đại lý (BizHTC.Contract / CT_DealerContractDetail): thông tin xe VIN, model, màu sắc, đơn giá xuất buôn, chiết khấu và giá bán thực tế.</summary>
public sealed class DealerContractLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DealerContractId { get; set; }
    public string ContractNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Model { get; set; } = "";
    public string? SpecCode { get; set; }
    public string? Color { get; set; }
    public decimal UnitPrice { get; set; } = 0;            // Giá niêm yết xuất buôn
    public decimal Discount { get; set; } = 0;             // Chiết khấu dòng xe
    public decimal ActualPrice { get; set; } = 0;          // Giá bán thực tế = UnitPrice - Discount
    public string Status { get; set; } = "Pending";        // Pending → Approved → Delivered (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu & Quyết toán Chiết khấu thanh toán mua xe ô tô cho Đại lý (BizHTC.PaymentDiscount / Req_PaymentDiscount): tính toán chiết khấu trả sớm theo các đợt thanh toán (Phase 1/2/3), hỗ trợ lãi suất ngân hàng và luồng ký số điện tử giữa Đại lý (DlrSign) và Hãng OEM (HTCSign).</summary>
public sealed class PaymentDiscount
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PaymentDiscountNo { get; set; } = "";     // Mã đề nghị chiết khấu (yyyyMMdd-xxx/DNCK/DealerCode)
    public string DealerCode { get; set; } = "";           // Mã đại lý đề nghị chiết khấu thanh toán
    public DateTime? DateEndFrom { get; set; }             // Kỳ thanh toán từ ngày
    public DateTime? DateEndTo { get; set; }               // Kỳ thanh toán đến ngày
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe được chiết khấu
    public decimal TotalPaymentAmount { get; set; } = 0;   // Tổng số tiền thanh toán (VNĐ)
    public decimal TotalDiscountAmount { get; set; } = 0;  // Tổng số tiền chiết khấu được hưởng (VNĐ)
    public decimal DiscountPercent { get; set; } = 0;      // Tỷ lệ % chiết khấu bình quân / chính sách
    public decimal PenaltyPercent { get; set; } = 0;       // Tỷ lệ % phạt quá hạn (nếu có)
    public string? FilePath { get; set; }                  // Đường dẫn file văn bản đề nghị / phụ lục ký số
    public string PmtDctStatus { get; set; } = "Draft";    // Draft/NotSign → Approved → Signed (hoặc Rejected / Cancelled)
    public string DlrSignStatus { get; set; } = "Pending"; // Pending → Signed / Approved1
    public string HTCSignStatus { get; set; } = "Pending"; // Pending → Approved → Signed / Approved2
    public string? Remark { get; set; }                    // Ghi chú đề nghị chiết khấu
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? HTCApprBy { get; set; }                 // Người sơ duyệt cấp OEM
    public DateTime? HTCApprAt { get; set; }
    public string? DlrSignBy { get; set; }                 // Người đại diện đại lý ký số
    public DateTime? DlrSignAt { get; set; }
    public string? HTCSignBy { get; set; }                 // Lãnh đạo OEM ký số phê duyệt quyết toán
    public DateTime? HTCSignAt { get; set; }
    public string? RejectBy { get; set; }                  // Người từ chối
    public DateTime? RejectAt { get; set; }
    public string? CancelBy { get; set; }                  // Người hủy
    public DateTime? CancelAt { get; set; }
}

/// <summary>Chi tiết dòng xe trong yêu cầu chiết khấu thanh toán (BizHTC.PaymentDiscount / Req_PaymentDiscountDtl): thông tin xe VIN, bảo lãnh ngân hàng, đơn giá và chi tiết chiết khấu qua 3 đợt thanh toán.</summary>
public sealed class PaymentDiscountLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long PaymentDiscountId { get; set; }
    public string PaymentDiscountNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? GuaranteeNo { get; set; }               // Liên kết chứng thư bảo lãnh ngân hàng (Pmt_Guarantee)
    public decimal UnitPrice { get; set; } = 0;            // Đơn giá xe (VNĐ)

    // Đợt thanh toán 1 (Phase 1)
    public DateTime? PaymentEndDatePhase1 { get; set; }
    public decimal AmountPhase1 { get; set; } = 0;
    public int DiscountDateNumberPhase1 { get; set; } = 0; // Số ngày thanh toán sớm đợt 1
    public decimal DiscountPercentPhase1 { get; set; } = 0; // % chiết khấu đợt 1
    public decimal DiscountPricePhase1 { get; set; } = 0;  // Tiền chiết khấu đợt 1 (VNĐ)

    // Đợt thanh toán 2 (Phase 2)
    public DateTime? PaymentEndDatePhase2 { get; set; }
    public decimal AmountPhase2 { get; set; } = 0;
    public int DiscountDateNumberPhase2 { get; set; } = 0; // Số ngày thanh toán sớm đợt 2
    public decimal DiscountPercentPhase2 { get; set; } = 0; // % chiết khấu đợt 2
    public decimal DiscountPricePhase2 { get; set; } = 0;  // Tiền chiết khấu đợt 2 (VNĐ)

    // Đợt thanh toán 3 (Phase 3)
    public DateTime? PaymentEndDatePhase3 { get; set; }
    public decimal AmountPhase3 { get; set; } = 0;
    public int DiscountDateNumberPhase3 { get; set; } = 0; // Số ngày thanh toán sớm đợt 3
    public decimal DiscountPercentPhase3 { get; set; } = 0; // % chiết khấu đợt 3
    public decimal DiscountPricePhase3 { get; set; } = 0;  // Tiền chiết khấu đợt 3 (VNĐ)

    public decimal TotalAmount { get; set; } = 0;          // Tổng tiền thanh toán cho xe này
    public decimal TotalDiscountPrice { get; set; } = 0;   // Tổng tiền chiết khấu được hưởng cho xe này
    public DateTime? PG_DateEnd { get; set; }              // Ngày hết hạn bảo lãnh ngân hàng
    public string Status { get; set; } = "Pending";        // Pending → Approved → Signed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu & Hợp đồng Bảo hiểm lô xe vận chuyển & lưu kho (BizHTC.WH.Ins_InsuranceReq / Ins_InsuranceReq): quản lý mua/tham gia bảo hiểm vật chất, bảo hiểm xe lồng vận chuyển, lưu kho bãi OEM và cấp GCN bảo hiểm điện tử.</summary>
public sealed class InsuranceRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string InsReqNo { get; set; } = "";             // Mã yêu cầu bảo hiểm (INS...)
    public string InsCompanyCode { get; set; } = "";       // Mã công ty bảo hiểm (PVI, BAOVIET, PJICO, PTI, BMI, BIC, MIC...)
    public string? InsCompanyName { get; set; }            // Tên hãng bảo hiểm
    public string InsTypeCode { get; set; } = "CARGO";     // Loại hình BH: CARGO (Vận chuyển xe lồng đường bộ), STORAGE (Lưu kho bãi OEM), COMPREHENSIVE (Vật chất thân vỏ toàn diện), TRANSIT (Nội bộ/PDI)
    public string? PolicyNo { get; set; }                  // Số hợp đồng / Giấy chứng nhận bảo hiểm khung
    public DateTime EffectiveDate { get; set; } = DateTime.Now; // Ngày bắt đầu hiệu lực bảo hiểm
    public DateTime? ExpireDate { get; set; }              // Ngày hết hạn bảo hiểm
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe tham gia bảo hiểm
    public decimal TotalInsuredValue { get; set; } = 0;    // Tổng giá trị định giá các xe (VNĐ)
    public decimal PremiumRate { get; set; } = 0.15m;      // Tỷ lệ phí bảo hiểm (%) (VD: 0.15% = 0.15)
    public decimal TotalPremiumAmount { get; set; } = 0;   // Tổng phí bảo hiểm phải nộp (VNĐ)
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú hợp đồng bảo hiểm
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }             // Ngày xuất đủ GCNBH / tất toán phí bảo hiểm
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong yêu cầu bảo hiểm (BizHTC.WH.Ins_InsuranceReqDtl / Ins_InsuranceReqDtl): danh sách VIN, định giá xe, phí bảo hiểm, kho xuất phát/đích và số GCN bảo hiểm điện tử cấp cho xe.</summary>
public sealed class InsuranceRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long InsuranceRequestId { get; set; }
    public string InsReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public decimal InsuredValue { get; set; } = 0;         // Giá trị định giá bảo hiểm của xe (VNĐ)
    public decimal PremiumRate { get; set; } = 0.15m;      // Tỷ lệ phí bảo hiểm (%)
    public decimal PremiumAmount { get; set; } = 0;        // Phí bảo hiểm xe này (VNĐ)
    public int InsuranceDays { get; set; } = 30;           // Thời hạn bảo hiểm (ngày)
    public string? FromStorage { get; set; }               // Kho xuất phát (nếu bảo hiểm vận chuyển)
    public string? ToStorage { get; set; }                 // Kho / Điểm đến
    public string? CertificateNo { get; set; }             // Số GCN bảo hiểm xe điện tử (GCN-...)
    public string Status { get; set; } = "Pending";        // Pending → Approved → Completed (hoặc Cancelled / Rejected)
    public string? Remark { get; set; }
}

/// <summary>Biên bản giao nhận & nghiệm thu vận chuyển xe ô tô đường bộ (BizHTC.Car.Car_TransportMinutes / TransportMinutes): quản lý giao nhận và quyết toán cước vận tải xe lồng giữa Hãng OEM, Đại lý và Đơn vị vận chuyển.</summary>
public sealed class TransportMinutes
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TransportMinutesNo { get; set; } = "";     // Mã biên bản vận chuyển (TM...)
    public string DealerCode { get; set; } = "";           // Đại lý nhận xe
    public string TransporterCode { get; set; } = "";      // Đơn vị / Nhà xe vận chuyển (NYK, Traco, Vinafco...)
    public string? TransporterName { get; set; }           // Tên đơn vị vận chuyển
    public string? TruckPlateNo { get; set; }              // Biển số xe lồng chở ô tô
    public string? DriverName { get; set; }                // Tên lái xe lồng
    public string? DriverPhone { get; set; }               // SĐT lái xe
    public string? TransportReqNo { get; set; }            // Mã kế hoạch vận chuyển (Car_TransportReq)
    public string? DeliveryOrderNo { get; set; }           // Lệnh giao xe liên kết (nếu có)
    public DateTime TransportMinutesDate { get; set; } = DateTime.Now; // Ngày lập biên bản bàn giao
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe bàn giao
    public decimal TotalFreightAmount { get; set; } = 0;   // Tổng cước phí vận chuyển (VNĐ)
    public decimal TotalSurchargeAmount { get; set; } = 0; // Tổng phụ phí phát sinh (cầu đường/bến bãi) (VNĐ)
    public decimal TotalAmount { get; set; } = 0;          // Tổng tiền quyết toán cước = TotalFreightAmount + TotalSurchargeAmount
    public string? FilePath { get; set; }                  // Đường dẫn/link ảnh chụp/biên bản giấy ký nhận
    public string Status { get; set; } = "Draft";          // Draft → Pending → DLAppr → HTCAppr1 → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú điều hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? DLApprBy { get; set; }                  // Người đại diện đại lý ký nhận
    public DateTime? DLApprAt { get; set; }
    public string? DLApprNote { get; set; }                // Ghi chú nghiệm thu của đại lý
    public string? HTCAppr1By { get; set; }                // Bộ phận Điều vận/Logistics OEM xác nhận
    public DateTime? HTCAppr1At { get; set; }
    public string? HTCAppr2By { get; set; }                // Lãnh đạo / Kế toán OEM duyệt quyết toán
    public DateTime? HTCAppr2At { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong biên bản vận chuyển (BizHTC.Car.Car_TransportMinutesDetail / TransportMinutesLine): danh sách VIN, chỉ số ODO xuất phát/đích, cước phí, phụ phí và tình trạng xe khi hạ tải.</summary>
public sealed class TransportMinutesLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TransportMinutesId { get; set; }
    public string TransportMinutesNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? DeliveryOrderNo { get; set; }
    public string? TransportReqNo { get; set; }
    public string? FromStorage { get; set; }               // Bãi bốc xe lên xe lồng
    public string? ToStorage { get; set; }                 // Bãi/Showroom hạ xe
    public int OdoDeparture { get; set; } = 0;             // ODO khi xếp xe lên lồng (km)
    public int OdoArrival { get; set; } = 0;               // ODO khi bàn giao hạ xe tại đại lý (km)
    public decimal FreightAmount { get; set; } = 0;        // Cước vận chuyển xe này (VNĐ)
    public decimal Surcharge { get; set; } = 0;            // Phụ phí phát sinh xe này (VNĐ)
    public decimal TotalAmount { get; set; } = 0;          // Tổng cước xe = FreightAmount + Surcharge
    public string CargoCondition { get; set; } = "Good";   // Tình trạng xe: Good (Nguyên vẹn), Scratched (Trầy xước), Dented (Móp), Dirty (Bụi bẩn)
    public bool IsInspectionPassed { get; set; } = true;   // Kết quả nghiệm thu đạt yêu cầu
    public string Status { get; set; } = "Pending";        // Pending → DLAppr → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chứng từ / Phiếu thanh toán tiền mua xe ô tô của Đại lý cho Hãng OEM (BizHTC.Payment.Pmt_Payment / DealerPayment): quản lý thanh toán tiền mua xe qua ủy nhiệm chi UNC ngân hàng, bù trừ công nợ, duyệt hạch toán kế toán ERP và giải phóng bảo lãnh.</summary>
public sealed class DealerPayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PaymentNo { get; set; } = "";             // Mã phiếu thanh toán (PMT...)
    public string DealerCode { get; set; } = "";           // Mã đại lý thanh toán tiền xe
    public string PaymentType { get; set; } = "Payment";   // Loại: Payment (Thanh toán UNC ngân hàng), Clearing (Bù trừ công nợ/chiết khấu), Adjust (Điều chỉnh tiền xe)
    public string? BankNameSend { get; set; }              // Ngân hàng chuyển tiền của đại lý (VCB, TCB, VPB, BIDV, MB...)
    public string? BankNameReceive { get; set; }           // Ngân hàng thụ hưởng của Hãng OEM (VCB SGD, BIDV...)
    public string? BankPaymentNo { get; set; }             // Số ủy nhiệm chi UNC / Số điện chuyển tiền ngân hàng
    public string? AccountingRecordNo { get; set; }        // Số chứng từ hạch toán kế toán ERP / Phiếu thu (PT...)
    public DateTime? PaymentEndDate { get; set; }          // Ngày thực tế tiền về tài khoản OEM / chốt kế toán
    public decimal TotalAmount { get; set; } = 0;          // Tổng số tiền thanh toán (VNĐ)
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe được phân bổ thanh toán
    public string Status { get; set; } = "Draft";          // Draft → Pending → Approved → Confirmed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Diễn giải / ghi chú thanh toán
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Kế toán công nợ OEM kiểm tra sơ duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? ConfirmBy { get; set; }                 // Kế toán trưởng / Thủ quỹ xác nhận tiền về & ghi sổ ERP
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong chứng từ thanh toán (BizHTC.Payment.Pmt_PaymentDetail / DealerPaymentLine): danh sách VIN, số tiền thanh toán cho từng xe và liên kết chứng thư bảo lãnh ngân hàng (Pmt_Guarantee).</summary>
public sealed class DealerPaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DealerPaymentId { get; set; }
    public string PaymentNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh ngân hàng liên quan (nếu xe này có bảo lãnh)
    public decimal Amount { get; set; } = 0;               // Số tiền thanh toán phân bổ cho xe này (VNĐ)
    public string Status { get; set; } = "Pending";        // Pending → Approved → Confirmed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Phiếu / Kế hoạch bảo dưỡng định kỳ xe tồn kho OEM (BizHTC.StorageFG.VIN_MaintainPeriod &amp; StoF_Maintain / StorageMaintenance): quản lý kiểm tra bảo dưỡng kỹ thuật định kỳ (ắc quy, lốp, động cơ, chất lỏng, vệ sinh) cho các xe đang lưu giữ tại bãi đỗ/kho trung tâm nhà máy.</summary>
public sealed class StorageMaintenance
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MtnNo { get; set; } = "";             // Mã phiếu bảo dưỡng kho (MTN...)
    public string StorageCode { get; set; } = "";       // Bãi đỗ / Kho lưu xe thực hiện bảo dưỡng (YARD-A1, KHO_NBD...)
    public string MtnType { get; set; } = "Periodic";   // Periodic (Định kỳ 30 ngày), Extended (Tăng cường), BatteryTire (Ắc quy & Lốp), PreDelivery (Tiền xuất bãi)
    public DateTime PlanDate { get; set; } = DateTime.Now; // Ngày kế hoạch thực hiện bảo dưỡng
    public int TotalVehicleCount { get; set; } = 0;     // Tổng số lượng xe trong đợt bảo dưỡng
    public int PassedVehicleCount { get; set; } = 0;    // Số lượng xe kiểm tra đạt chuẩn
    public int FailedVehicleCount { get; set; } = 0;    // Số lượng xe không đạt / có khiếm khuyết
    public string Status { get; set; } = "Draft";       // Draft → Pending → InProgress → Completed (hoặc Rejected / Cancelled)
    public string? TechnicianCode { get; set; }         // Mã KTV phụ trách
    public string? TechnicianName { get; set; }         // Tên KTV phụ trách
    public string? SupervisorCode { get; set; }         // Mã Quản đốc / Giám sát kho bãi
    public string? SupervisorName { get; set; }         // Tên Quản đốc / Giám sát kho bãi
    public string? Remark { get; set; }                 // Ghi chú đợt bảo dưỡng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }             // Người phê duyệt kế hoạch
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }          // Thời điểm nghiệm thu hoàn tất toàn bộ đợt bảo dưỡng
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong phiếu bảo dưỡng kho OEM (BizHTC.StorageFG.VIN_MaintainPeriodHist &amp; StoF_MaintainMain / StorageMaintenanceLine): danh sách VIN, đo điện áp ắc quy, nổ máy kiểm tra động cơ, áp suất lốp, mức dầu nước, vệ sinh thân vỏ và hạn bảo dưỡng kế tiếp.</summary>
public sealed class StorageMaintenanceLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long StorageMaintenanceId { get; set; }
    public string MtnNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? StorageCode { get; set; }            // Vị trí bãi / ô đỗ của xe
    public int MtnTimes { get; set; } = 0;              // Số lần bảo dưỡng lũy kế của xe này
    public double? BatteryVoltage { get; set; } = 12.6; // Điện áp bình ắc quy đo được (V, chuẩn >= 12.4V)
    public bool ChargeBatteryOk { get; set; } = true;   // Ắc quy đủ điện / Đã sạc bổ sung
    public bool EngineStartCheckOk { get; set; } = true;// Nổ máy động cơ 15 phút, bơm dầu bôi trơn hoạt động tốt
    public bool TirePressureCheckOk { get; set; } = true;// Áp suất lốp đạt tiêu chuẩn kỹ thuật (2.2 - 2.5 bar)
    public bool TireRotationOk { get; set; } = true;    // Đã di chuyển dịch chuyển bánh xe chống méo lốp
    public bool FluidLevelsCheckOk { get; set; } = true;// Mức dung dịch dầu máy, nước làm mát, dầu phanh đạt chuẩn
    public bool ElectricalSystemsOk { get; set; } = true;// Hệ thống điện, đèn, còi, gạt mưa hoạt động tốt
    public bool BodyCleanOk { get; set; } = true;       // Vệ sinh sạch sẽ bề mặt sơn và thân vỏ xe
    public string InspectionResult { get; set; } = "Pending"; // Pending → Passed / Failed
    public DateTime? MtnDate { get; set; }              // Thời điểm thực hiện kiểm tra xe này
    public DateTime? NextMtnDate { get; set; }          // Hạn bảo dưỡng định kỳ kế tiếp (= MtnDate + 30 ngày)
    public string? Technician { get; set; }             // KTV thực hiện kiểm tra xe này
    public string? DefectNotes { get; set; }            // Ghi chú sự cố / khiếm khuyết kỹ thuật nếu Failed
    public string Status { get; set; } = "Pending";     // Pending → InProgress → Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Packing List xuất xưởng nhà máy & Vận đơn nhập khẩu CBU/CKD (BizHTC.Contract.ContractPackingList / CT_PackingList): quản lý vận đơn đóng gói lô xe xuất xưởng từ nhà máy hoặc tàu biển cập cảng, liên kết hợp đồng/LC và tự động sinh nhập kho xe VIN khi phê duyệt.</summary>
public sealed class PackingList
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PackingListNo { get; set; } = "";         // Mã Packing List (PL...)
    public string? ContractNo { get; set; }                // Hợp đồng ngoại thương / đơn hàng sản xuất OEM
    public string? LCNo { get; set; }                      // Số thư tín dụng L/C ngân hàng
    public string PortCode { get; set; } = "NHA_MAY_NINH_BINH"; // Cảng cập bến / Bãi xuất xưởng (CANG_HAI_PHONG, NHA_MAY_NINH_BINH, CANG_CAT_LAI...)
    public string? VesselName { get; set; }                // Tên tàu biển chở hàng / Đoàn xe HTMV
    public string? VoyageNo { get; set; }                  // Số chuyến tàu / Lô xuất xưởng
    public DateTime? ShippingDateStart { get; set; }       // Ngày xuất xưởng / Rời cảng
    public DateTime? ShippingDateEndExpected { get; set; } // Ngày dự kiến cập cảng / Về kho OEM
    public DateTime? ShippingDateEnd { get; set; }         // Ngày cập cảng / Nhập bãi thực tế
    public int TotalQuantity { get; set; } = 0;            // Tổng số lượng xe trong lô
    public decimal TotalAmount { get; set; } = 0;          // Tổng giá trị lô xe (VNĐ)
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved (hoặc Cancelled)
    public string? Remark { get; set; }                    // Ghi chú vận đơn lô xe
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Người duyệt kế hoạch / Thủ kho tiếp nhận
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết dòng xe trong Packing List (BizHTC.Contract.CT_PackingListDetail / PackingListLine): thông tin xe VIN, phiên bản Spec, số máy EngineNo, màu sắc, năm sản xuất, mã chìa khóa và đơn giá xuất xưởng.</summary>
public sealed class PackingListLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long PackingListId { get; set; }
    public string PackingListNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Model { get; set; } = "";                // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra, Custin...)
    public string? SpecCode { get; set; }                  // Phiên bản xe (1.6T, 2.0 AT Tiêu chuẩn, 2.0 AT Đặc biệt, Hybrid...)
    public string? EngineNo { get; set; }                  // Số máy động cơ
    public string? Color { get; set; }                     // Màu sắc xe
    public int? ModelYear { get; set; } = 2026;            // Năm sản xuất / Model Year
    public string? KeyNo { get; set; }                     // Mã chìa khóa xuất xưởng
    public DateTime? ProductionDate { get; set; }          // Ngày xuất xưởng nhà máy
    public decimal UnitPrice { get; set; } = 0;            // Đơn giá xuất xưởng xe (VNĐ)
    public string Status { get; set; } = "Pending";        // Pending → Approved (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Tờ khai Hải quan nhập khẩu CBU/CKD & Nộp thuế thông quan xe (BizHTC.Contract.ContractDeclaration & CT_TKHQ / CT_Declaration): quản lý tờ khai hải quan, số tờ khai, chi cục hải quan cửa khẩu, tính thuế nhập khẩu, thuế TTĐB, thuế VAT, ngày nộp thuế và thông quan giải phóng xe.</summary>
public sealed class CustomsDeclaration
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DeclarationNo { get; set; } = "";             // Số tờ khai hải quan (TKHQ...)
    public string PortCode { get; set; } = "HQ_HAI_PHONG";      // Chi cục Hải quan cửa khẩu (HQ_HAI_PHONG, HQ_CAT_LAI, HQ_CAI_MEP, HQ_NOI_BAI, NHA_MAY_NINH_BINH...)
    public string? PortName { get; set; }                       // Tên Chi cục Hải quan cửa khẩu
    public string? ContractNo { get; set; }                     // Số hợp đồng ngoại thương liên kết (CT_ContractOversea)
    public string? LCNo { get; set; }                           // Thư tín dụng L/C (CT_LC)
    public string? BillOfLadingNo { get; set; }                 // Số vận đơn đường biển / hàng hải B/L
    public string DeclarationType { get; set; } = "CBU";        // CBU (Xe nguyên chiếc), CKD (Linh kiện lắp ráp xe), TEMPORARY (Tạm nhập tái xuất), SPAREPARTS (Phụ tùng)
    public DateTime OpenDate { get; set; } = DateTime.Now;      // Ngày đăng ký mở tờ khai hải quan
    public DateTime? TaxPaymentDate { get; set; }               // Ngày hoàn thành nộp thuế vào NSNN
    public DateTime? ClearanceDate { get; set; }                // Ngày thực tế thông quan giải phóng hàng hóa
    public string? CustomsOfficer { get; set; }                 // Cán bộ / Công chức hải quan tiếp nhận
    public string? DeclarantName { get; set; }                  // Người khai hải quan / Đại lý thủ tục hải quan
    public int TotalVehicleCount { get; set; } = 0;             // Tổng số lượng xe trong tờ khai
    public decimal TotalTaxValue { get; set; } = 0;             // Tổng trị giá tính thuế (CIF/FOB, VNĐ)
    public decimal ImportTaxAmount { get; set; } = 0;           // Tổng tiền thuế nhập khẩu (VNĐ)
    public decimal ExciseTaxAmount { get; set; } = 0;           // Tổng tiền thuế tiêu thụ đặc biệt (VNĐ)
    public decimal VatAmount { get; set; } = 0;                 // Tổng tiền thuế VAT (VNĐ)
    public decimal TotalTaxAmount { get; set; } = 0;            // Tổng tiền thuế phải nộp = ImportTax + ExciseTax + VatAmount (VNĐ)
    public string Status { get; set; } = "Draft";               // Draft → Registered → TaxPaid → Cleared (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                         // Ghi chú tờ khai hải quan
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ClearedBy { get; set; }
    public DateTime? ClearedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết dòng xe trong Tờ khai hải quan (BizHTC.Contract.CT_TKHQ & CT_DeclarationDetail / CustomsDeclarationLine): thông tin xe VIN, trị giá tính thuế CIF/FOB, thuế suất & tiền thuế nhập khẩu, thuế TTĐB, thuế VAT và ngày nộp thuế của từng xe.</summary>
public sealed class CustomsDeclarationLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CustomsDeclarationId { get; set; }
    public string DeclarationNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Model { get; set; } = "";                     // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra...)
    public string? SpecCode { get; set; }                       // Phiên bản xe
    public string? EngineNo { get; set; }                       // Số máy động cơ
    public string? Color { get; set; }                          // Màu sắc xe
    public int? ModelYear { get; set; } = 2026;                 // Năm sản xuất / Model Year
    public string? PackingListNo { get; set; }                  // Mã Packing List xuất xưởng liên kết (nếu có)
    public decimal TaxValue { get; set; } = 0;                  // Trị giá tính thuế xe (VNĐ)
    public decimal ImportTaxRate { get; set; } = 50;            // Thuế suất nhập khẩu (%) (VD: 50% = 50)
    public decimal ImportTax { get; set; } = 0;                 // Tiền thuế nhập khẩu = TaxValue * ImportTaxRate%
    public decimal ExciseTaxRate { get; set; } = 35;            // Thuế suất TTĐB (%) (VD: 35% = 35)
    public decimal ExciseTax { get; set; } = 0;                 // Tiền thuế TTĐB = (TaxValue + ImportTax) * ExciseTaxRate%
    public decimal VatRate { get; set; } = 10;                  // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal VatTax { get; set; } = 0;                    // Tiền thuế VAT = (TaxValue + ImportTax + ExciseTax) * VatRate%
    public decimal TotalTax { get; set; } = 0;                  // Tổng tiền thuế của xe = ImportTax + ExciseTax + VatTax
    public DateTime? TaxPaymentDate { get; set; }               // Ngày nộp thuế của xe này
    public DateTime? ClearanceDate { get; set; }                // Ngày thông quan xe này
    public string Status { get; set; } = "Pending";             // Pending → Registered → TaxPaid → Cleared (hoặc Rejected / Cancelled)
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
