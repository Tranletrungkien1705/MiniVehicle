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
    public string TypeCB { get; set; } = "0";       // Tình trạng đóng thùng xe thương mại: "0" - Chassis chưa đóng thùng, "1" - Đã đóng thùng (BizHTC.Car.TypeCB)
    public string? LoaiThung { get; set; }          // Loại thùng hiện tại (ThungBat, ThungKin, ThungLanh, ThungLung, ThungComposite, ThungChuyenDung)
    public string? CBReqNo { get; set; }            // Mã yêu cầu đóng thùng gần nhất (Sto_CBReq)
    public string? ContractNoOversea { get; set; }  // Mã hợp đồng mua bán ngoại thương CBU/CKD (CT_ContractOversea)
    public string? LCNo { get; set; }               // Mã Thư tín dụng L/C ngân hàng thanh toán nhập khẩu (CT_LC / LetterOfCredit)
    public bool IsInvoiced { get; set; } = false;   // Đã xuất hóa đơn GTGT bán xe cho đại lý (Car_InvoiceList)
    public string? InvoiceNo { get; set; }          // Số hóa đơn GTGT điện tử (HD26-...)
    public DateTime? InvoiceDate { get; set; }      // Ngày xuất hóa đơn GTGT
    public string? InvoiceListCode { get; set; }    // Mã bảng kê / đợt xuất hóa đơn liên quan (IVL...)
    public bool IsBankBillHandedOver { get; set; } = false; // Đã bàn giao hồ sơ gốc và hóa đơn cho Ngân hàng (Car_BankBillMinutes)
    public string? BankBillMnNo { get; set; }       // Mã biên bản bàn giao hồ sơ ngân hàng gần nhất
    public DateTime? BankBillHandoverDate { get; set; } // Ngày bàn giao hồ sơ xe cho ngân hàng
    public string? LastRoNo { get; set; }           // Mã lệnh sửa chữa xưởng dịch vụ gần nhất (Ser_RO / RepairOrder)
    public DateTime? LastRoDate { get; set; }       // Ngày thực hiện lệnh sửa chữa dịch vụ xưởng gần nhất
    public int? LastOdoKm { get; set; }             // Chỉ số ODO gần nhất ghi nhận tại xưởng dịch vụ
    public string? LastAppointmentNo { get; set; }  // Mã lịch hẹn dịch vụ gần nhất (Ser_App / ServiceAppointment)
    public DateTime? LastAppointmentDate { get; set; } // Ngày hẹn làm dịch vụ gần nhất
    public string? LastBulletinNo { get; set; }     // Mã bản tin kỹ thuật TSB gần nhất áp dụng (Blt_Bulletin / TechnicalBulletin)
    public DateTime? LastBulletinDate { get; set; } // Ngày thực hiện hoàn tất bản tin kỹ thuật gần nhất
    public string? LastDisbursementNo { get; set; } // Mã giao dịch giải ngân ngân hàng gần nhất (RQ_BankingTransactions / BankDisbursement)
    public DateTime? LastDisbursementDate { get; set; } // Ngày ngân hàng giải ngân gần nhất
    public string? LastCampaignNo { get; set; }     // Mã chiến dịch dịch vụ / CSKH gần nhất tham gia (Ser_CampaignMarketing / ServiceCampaign)
    public DateTime? LastCampaignDate { get; set; } // Ngày tham gia chiến dịch dịch vụ gần nhất
    public string? LastWarrantyReportNo { get; set; } // Mã báo cáo bảo hành gần nhất (Ser_ROWarrantyReport / WarrantyReport)
    public DateTime? LastWarrantyReportDate { get; set; } // Ngày báo cáo bảo hành gần nhất
    public int WarrantyClaimCount { get; set; } = 0;   // Tổng số lần xe đã phát sinh yêu cầu bảo hành chính hãng
    public string? LastQuoteNo { get; set; }           // Mã báo giá dịch vụ & phụ tùng gần nhất (Ser_Quotation / ServiceQuotation)
    public DateTime? LastQuoteDate { get; set; }       // Ngày lập báo giá dịch vụ gần nhất
    public int QuotationCount { get; set; } = 0;       // Tổng số lần xe đã lập báo giá dịch vụ & phụ tùng
    public string? LastCareNo { get; set; }           // Mã phiếu CSKH gần nhất (Ser_CustomerCare / CustomerCare)
    public DateTime? LastCareDate { get; set; }       // Ngày thực hiện CSKH gần nhất
    public string? LastCareType { get; set; }         // Loại hình chăm sóc gần nhất (FollowUp24h, FollowUp72h, MaintenanceReminder, Birthday, SeasonalCare)
    public decimal? LastCsiScore { get; set; }        // Điểm đánh giá hài lòng CSI gần nhất (1-5 sao)
    public int CareCount { get; set; } = 0;           // Tổng số lần đã thực hiện CSKH
    public string? LastWorkOrderNo { get; set; }      // Mã Lệnh sản xuất / Đơn đặt hàng sản xuất nhà máy đã sinh ra xe (MnfPl_Order / WorkOrder)
    public DateTime? ManufacturedDate { get; set; }   // Ngày hoàn tất xuất xưởng KCS tại nhà máy OEM
    public string? PlantCode { get; set; }            // Nhà máy sản xuất lắp ráp xe (HTMV_NINHBINH_1, HTMV_NINHBINH_2, TCV_PLANT)
    public string? LastPiNo { get; set; }             // Mã số Proforma Invoice gần nhất (Ord_PI / PerformanceInvoice)
    public DateTime? LastPiDate { get; set; }         // Ngày ban hành Proforma Invoice
    public int PiCount { get; set; } = 0;             // Tổng số lần lập PI liên quan đến xe
    public bool IsPdiPaid { get; set; } = false;      // Đã quyết toán/thanh toán chi phí kiểm tra PDI (BizHTC.Payment.Pmt_PaymentPDI)
    public decimal PdiPaidAmount { get; set; } = 0;   // Tổng tiền PDI đã thanh toán cho xe (VNĐ)
    public string? LastPdiPaymentNo { get; set; }     // Mã bảng kê quyết toán PDI gần nhất (Pmt_PaymentPDI)
    public DateTime? LastPdiPaymentDate { get; set; } // Ngày quyết toán PDI gần nhất
    public int PdiPaymentCount { get; set; } = 0;     // Tổng số lần phát sinh quyết toán PDI
    public bool IsPolicySupported { get; set; } = false; // Đã duyệt hưởng chính sách hỗ trợ bán lẻ (BizHTC.DealerSales.SPL_SPSupportRetail)
    public decimal PolicySupportAmount { get; set; } = 0; // Tổng tiền hỗ trợ bán lẻ đã duyệt chi (VNĐ)
    public string? LastPolicyCode { get; set; }       // Mã chính sách hỗ trợ bán lẻ gần nhất (SPSRCode)
    public DateTime? LastPolicyDate { get; set; }     // Ngày hưởng chính sách hỗ trợ gần nhất
    public int PolicySupportCount { get; set; } = 0;   // Số lần đã được duyệt hưởng chính sách hỗ trợ bán lẻ
    public bool IsGpsInstalled { get; set; } = false; // Đang gắn thiết bị định vị GPS giám sát (BizHTC.StorageFG.Sto_StoBalanceGPS)
    public string? GpsCode { get; set; }             // Mã thiết bị GPS đang gắn trên xe (GPSDvNo / GPSCode)
    public DateTime? GpsInstallDate { get; set; }    // Ngày lắp đặt thiết bị GPS gần nhất
    public DateTime? GpsUninstallDate { get; set; }  // Ngày tháo gỡ thiết bị GPS gần nhất
    public decimal? LastGpsLatitude { get; set; }    // Tọa độ vĩ độ GPS hiện tại
    public decimal? LastGpsLongitude { get; set; }   // Tọa độ kinh độ GPS hiện tại
    public string? LastGpsAddress { get; set; }      // Địa chỉ / vị trí GPS ghi nhận gần nhất
    public decimal? LastGpsSpeed { get; set; }       // Vận tốc di chuyển gần nhất (km/h)
    public decimal? LastGpsBatteryVolt { get; set; } // Điện áp nguồn thiết bị GPS / ắc quy xe (V)
    public DateTime? LastGpsSignalTime { get; set; } // Thời điểm cập nhật tín hiệu GPS gần nhất
    public int GpsDeviceCount { get; set; } = 0;     // Tổng số lần xe đã từng gắn / đổi thiết bị định vị
    public string? LastCavityNo { get; set; }       // Mã khoang/cầu sửa chữa xưởng dịch vụ gần nhất xe vào (BizCarSv.Ser_Cavity / ServiceCavity)
    public string? LastCavityName { get; set; }     // Tên khoang/cầu sửa chữa gần nhất
    public DateTime? LastCavityDate { get; set; }   // Thời điểm vào khoang sửa chữa gần nhất
    public int CavityVisitCount { get; set; } = 0;   // Tổng số lượt xe đã vào khoang cầu làm dịch vụ
    public bool IsStoragePaid { get; set; } = false; // Đã thanh toán / quyết toán chi phí lưu kho bãi OEM (BizHTC.Payment.Pmt_PaymentStorage)
    public decimal StoragePaidAmount { get; set; } = 0; // Tổng tiền lưu kho đã thanh toán của xe (VNĐ)
    public string? LastStoragePaymentNo { get; set; } // Mã bảng kê quyết toán lưu kho gần nhất (PaymentStorageNo)
    public DateTime? LastStoragePaymentDate { get; set; } // Ngày quyết toán chi phí lưu kho gần nhất
    public int StoragePaymentCount { get; set; } = 0; // Số lần xe phát sinh trong bảng kê quyết toán lưu kho
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
    public string? LastGrtExtNo { get; set; }            // Mã đơn đề nghị/quyết định gia hạn bảo lãnh gần nhất (Pmt_GrtClaimExt)
    public int ExtensionTimes { get; set; } = 0;         // Số lần xe đã được gia hạn thời hạn bảo lãnh
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

/// <summary>Yêu cầu & Lệnh đóng thùng xe thương mại / xe tải (BizHTC.Storage.Sto_CBReq / CarBoxRequest): quản lý quy trình chuyển đổi đóng thùng xe chassis sắt xi sang thùng mui bạt, thùng kín, thùng đông lạnh, thùng lửng, thùng composite, thùng chuyên dụng.</summary>
public sealed class CarBoxRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CBReqNo { get; set; } = "";             // Mã yêu cầu đóng thùng (CBR...)
    public string? DealerCode { get; set; }               // Đại lý / Bộ phận kinh doanh xe thương mại yêu cầu
    public string? BodyBuilder { get; set; }              // Cơ sở / Nhà xưởng đóng thùng ủy quyền (Body Builder)
    public DateTime RequestDate { get; set; } = DateTime.Now; // Ngày lập đề nghị đóng thùng
    public DateTime? ExpectedStartDate { get; set; }     // Ngày dự kiến bắt đầu thi công
    public DateTime? ExpectedEndDate { get; set; }       // Ngày dự kiến hoàn tất đóng thùng
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng xe đóng thùng trong đợt
    public decimal TotalAmount { get; set; } = 0;         // Tổng chi phí gia công đóng thùng (VNĐ)
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved → InProgress → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú yêu cầu kỹ thuật đóng thùng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Kỹ sư trưởng / Quản đốc xưởng đóng thùng duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? CompletedBy { get; set; }              // KTV nghiệm thu xuất xưởng đóng thùng
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong yêu cầu đóng thùng (BizHTC.Storage.Sto_CBReqDetail / CarBoxRequestLine): thông tin xe VIN, loại thùng, quy cách kích thước lọt lòng, tải trọng cho phép, chi phí đóng thùng, nghiệm thu kỹ thuật và phiếu xuất xưởng thùng xe.</summary>
public sealed class CarBoxRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CarBoxRequestId { get; set; }
    public string CBReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? StorageCodeFrom { get; set; }          // Vị trí bãi xe sắt xi xuất phát (YARD-CHASSIS...)
    public string StorageCodeTo { get; set; } = "";       // Xưởng / Bãi đóng thùng tiếp nhận (BODY-SHOP-01...)
    public string LoaiThung { get; set; } = "ThungBat";   // Loại thùng: ThungBat (Thùng mui bạt), ThungKin (Thùng kín), ThungLanh (Thùng đông lạnh), ThungLung (Thùng lửng), ThungComposite (Thùng composite), ThungChuyenDung (Thùng chuyên dụng)
    public string? TenLoaiThung { get; set; }             // Tên diễn giải loại thùng
    public double? BoxLengthMm { get; set; }              // Chiều dài lọt lòng thùng (mm)
    public double? BoxWidthMm { get; set; }               // Chiều rộng lọt lòng thùng (mm)
    public double? BoxHeightMm { get; set; }              // Chiều cao lọt lòng thùng (mm)
    public double? PayloadKg { get; set; }                // Tải trọng chở hàng cho phép (kg)
    public decimal BodyPrice { get; set; } = 0;           // Chi phí gia công đóng thùng (VNĐ)
    public string? BodyBuilder { get; set; }              // Cơ sở / Nhà xưởng đóng thùng cho xe này
    public string? InspectionNo { get; set; }             // Số phiếu kiểm tra xuất xưởng / GCN chất lượng thùng xe
    public string InspectionResult { get; set; } = "Pending"; // Pending → Passed / Failed
    public DateTime? InspectionDate { get; set; }         // Ngày nghiệm thu kỹ thuật
    public string? InspectorName { get; set; }            // Kỹ sư kiểm định / Nghiệm thu
    public string? DefectNotes { get; set; }              // Ghi chú khiếm khuyết kỹ thuật nếu chưa đạt
    public string Status { get; set; } = "Pending";       // Pending → Approved → InProgress → Completed (hoặc Rejected / Cancelled)
    public DateTime? CompletedDate { get; set; }          // Ngày hoàn tất đóng thùng cho xe này
    public string? Remark { get; set; }
}

/// <summary>Bảng kê / Đợt xuất hóa đơn GTGT xe ô tô cho Đại lý (BizHTC.Car.Car_InvoiceList / CarInvoice): quản lý phát hành hóa đơn GTGT điện tử bán buôn xe ô tô từ OEM cho đại lý, đối soát giá trị tính thuế, tiền thuế VAT và cập nhật trạng thái hóa đơn trên hồ sơ số khung VIN.</summary>
public sealed class CarInvoice
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string InvoiceListCode { get; set; } = "";     // Mã bảng kê hóa đơn (IVL...)
    public string DealerCode { get; set; } = "";          // Mã đại lý nhận hóa đơn GTGT
    public string InvoiceType { get; set; } = "VAT";      // Loại hóa đơn: VAT (Hóa đơn GTGT điện tử), Commercial (Hóa đơn thương mại), Export (Hóa đơn xuất khẩu)
    public DateTime InvoiceDate { get; set; } = DateTime.Now; // Ngày lập hóa đơn
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng xe xuất hóa đơn trong đợt
    public decimal TotalTaxValue { get; set; } = 0;       // Tổng trị giá xe trước thuế (VNĐ)
    public decimal VatRate { get; set; } = 10;            // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;      // Tổng tiền thuế VAT (VNĐ)
    public decimal TotalAmount { get; set; } = 0;         // Tổng tiền thanh toán đã bao gồm VAT = TotalTaxValue + TotalVatAmount
    public string Status { get; set; } = "Draft";         // Draft → Issued (hoặc Cancelled)
    public string? Remark { get; set; }                   // Diễn giải / ghi chú bảng kê hóa đơn
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? IssuedBy { get; set; }                 // Kế toán viên / Người ký phát hành hóa đơn điện tử
    public DateTime? IssuedAt { get; set; }               // Thời điểm phát hành hóa đơn chính thức
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong bảng kê hóa đơn GTGT (BizHTC.Car.Car_InvoiceListDetail / CarInvoiceLine): số khung VIN, số hóa đơn VAT, ngày hóa đơn, đại lý thụ hưởng, đơn giá tính thuế, thuế VAT và tổng tiền.</summary>
public sealed class CarInvoiceLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CarInvoiceId { get; set; }
    public string InvoiceListCode { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string InvoiceDealerCode { get; set; } = "";   // Mã đại lý ghi trên hóa đơn
    public string InvoiceNo { get; set; } = "";           // Số hóa đơn GTGT điện tử (HD26-0001001)
    public DateTime? InvoiceDate { get; set; }            // Ngày lập hóa đơn cho xe này
    public decimal TaxValue { get; set; } = 0;            // Đơn giá xe trước thuế GTGT (VNĐ)
    public decimal VatRate { get; set; } = 10;            // Thuế suất VAT (%)
    public decimal VatAmount { get; set; } = 0;           // Tiền thuế VAT = TaxValue * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;         // Tổng tiền xe bao gồm thuế = TaxValue + VatAmount
    public string Status { get; set; } = "Pending";       // Pending → Issued (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Đề nghị & Quyết định gia hạn bảo lãnh thanh toán ngân hàng mua xe ô tô cho Đại lý (BizHTC.PaymentGrtExt / Pmt_GrtClaimExt): quản lý đề nghị gia hạn thời hạn bảo lãnh khi xe lưu bãi đại lý sắp đến hạn nộp tiền.</summary>
public sealed class GuaranteeExtension
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GrtClaimExtNo { get; set; } = "";        // Mã đề nghị / Quyết định gia hạn bảo lãnh (GEXT...)
    public string DealerCode { get; set; } = "";           // Đại lý đề nghị gia hạn
    public string? BankCode { get; set; }                  // Ngân hàng phát hành chứng thư bảo lãnh (VCB, TCB, VPB, BIDV...)
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh ngân hàng gốc liên quan (Pmt_Guarantee)
    public int ExtensionDays { get; set; } = 30;           // Số ngày xin gia hạn thêm (15, 30, 45, 60 ngày...)
    public decimal FeeRate { get; set; } = 0;              // Tỷ lệ phí gia hạn (%) (nếu có)
    public decimal TotalFeeAmount { get; set; } = 0;       // Tổng phí gia hạn bảo lãnh (VNĐ)
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe xin gia hạn trong đợt
    public decimal TotalGuaranteeAmount { get; set; } = 0; // Tổng giá trị bảo lãnh các xe xin gia hạn (VNĐ)
    public string? FileSigned { get; set; }                // Đường dẫn / tệp văn bản thỏa thuận gia hạn ký số điện tử
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Lý do đề nghị & diễn giải gia hạn
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo kinh doanh / Kế toán OEM sơ duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? SignedBy { get; set; }                  // Người ký số xác nhận hoàn tất thỏa thuận gia hạn
    public DateTime? SignedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong đề nghị gia hạn bảo lãnh (BizHTC.PaymentGrtExt / Pmt_GrtClaimExtDtl): danh sách VIN, hạn bảo lãnh cũ, hạn bảo lãnh mới, số ngày gia hạn và phí gia hạn từng xe.</summary>
public sealed class GuaranteeExtensionLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long GuaranteeExtensionId { get; set; }
    public string GrtClaimExtNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh gốc (Pmt_Guarantee)
    public DateTime? CurrentDateExpired { get; set; }      // Hạn bảo lãnh hiện tại trước khi gia hạn
    public DateTime? NewDateExpired { get; set; }          // Hạn bảo lãnh mới sau gia hạn (= CurrentDateExpired + ExtensionDays)
    public int ExtensionDays { get; set; } = 30;           // Số ngày gia hạn cho xe này
    public decimal GuaranteeValue { get; set; } = 0;       // Giá trị bảo lãnh xe này (VNĐ)
    public decimal FeeRate { get; set; } = 0;              // % phí gia hạn cho xe này
    public decimal ExtensionFee { get; set; } = 0;         // Tiền phí gia hạn xe này (VNĐ)
    public string Status { get; set; } = "Pending";        // Pending → Submitted → Approved → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Đề nghị & Quyết định Hủy / Điều chỉnh rút dòng xe Hợp đồng mua bán xe Đại lý (BizHTC.Contract.Dlr_ContractCancel / ContractCancel): quản lý đề nghị hủy toàn bộ hợp đồng hoặc rút bớt/hủy dòng xe từ đại lý gửi lên hãng OEM.</summary>
public sealed class ContractCancel
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ContractCNo { get; set; } = "";             // Mã phiếu đề nghị hủy (CCN...)
    public string DealerCode { get; set; } = "";               // Mã đại lý đề nghị hủy
    public string? DlrContractNo { get; set; }                 // Mã hợp đồng mua bán gốc liên quan (CTR...)
    public string CancelType { get; set; } = "Partial";        // Full (Hủy toàn bộ hợp đồng), Partial (Hủy rút từng dòng xe), VinCancel (Hủy chỉ định VIN)
    public string? CancelReason { get; set; }                  // Lý do hủy (Khách hủy cọc, Đổi mẫu xe, Điều chuyển vốn...)
    public int TotalCancelQty { get; set; } = 0;              // Tổng số lượng xe xin hủy/rút
    public decimal TotalCancelAmount { get; set; } = 0;       // Tổng giá trị xe xin hủy (VNĐ)
    public decimal DepositRefundAmount { get; set; } = 0;     // Tiền cọc đề nghị hoàn trả lại cho đại lý (VNĐ)
    public string Status { get; set; } = "Draft";             // Draft → Submitted → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                       // Ghi chú điều hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                   // Người duyệt cấp OEM
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }                   // Người từ chối
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }                  // Người hủy đề nghị
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết dòng xe trong đề nghị hủy hợp đồng (BizHTC.Contract.Dlr_ContractCancelDtl / ContractCancelLine): danh sách VIN / model, phân loại cập nhật hợp đồng, số lượng hủy, đơn giá và tiền hoàn trả.</summary>
public sealed class ContractCancelLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ContractCancelId { get; set; }
    public string ContractCNo { get; set; } = "";
    public string DlrContractNo { get; set; } = "";           // Mã hợp đồng liên quan
    public string? Vin { get; set; }                          // Số khung VIN cụ thể (nếu có)
    public string Model { get; set; } = "";                   // Dòng xe (SantaFe, Tucson, Accent...)
    public string? SpecCode { get; set; }                     // Phiên bản xe
    public string? Color { get; set; }                        // Màu sắc
    public string ContractUpdateType { get; set; } = "CANCEL_VIN"; // Phân loại: CANCEL_VIN (Rút xe VIN), REDUCE_QTY (Giảm số lượng), CUSTOMER_CANCEL (Khách hủy cọc), MODEL_CHANGE (Đổi phiên bản), OTHER (Khác)
    public int CancelQty { get; set; } = 1;                   // Số lượng xe xin hủy
    public decimal UnitPrice { get; set; } = 0;               // Đơn giá xe xuất buôn (VNĐ)
    public decimal RefundAmount { get; set; } = 0;            // Tiền hoàn trả cho xe này (VNĐ) = CancelQty * UnitPrice
    public string Status { get; set; } = "Pending";           // Pending → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Đề nghị & Lệnh thay đổi màu sơn xe ô tô (BizHTC.WH &amp; BizHTC.Car.Car_ColorChange / CarColorChange): quản lý đề nghị đổi màu sơn xe ô tô từ đại lý hoặc kế hoạch OEM trước khi xuất kho / bàn giao xe.</summary>
public sealed class CarColorChange
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ChangeNo { get; set; } = "";             // Mã đề nghị đổi màu xe (CCC...)
    public string DealerCode { get; set; } = "";           // Đại lý đề nghị đổi màu hoặc "OEM"
    public string ChangeType { get; set; } = "DealerRequest"; // DealerRequest (Đại lý yêu cầu), OEMPlan (Kế hoạch sản xuất/bãi OEM), CustomerRequest (Khách hàng đổi ý)
    public string? Reason { get; set; }                    // Lý do đổi màu (Đổi theo hợp đồng bán lẻ, Khách chọn màu phong thủy, Điều chuyển tồn kho...)
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe đổi màu
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú điều hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo kinh doanh / Kỹ thuật phê duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết xe trong đề nghị đổi màu sơn (BizHTC.WH.Rpt_CarColorChangeHistory / CarColorChangeLine): danh sách VIN, thông tin màu cũ, màu mới, mã màu và trạng thái cập nhật.</summary>
public sealed class CarColorChangeLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CarColorChangeId { get; set; }
    public string ChangeNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string Model { get; set; } = "";                // Dòng xe
    public string? SpecCode { get; set; }                  // Phiên bản xe
    public string OldColor { get; set; } = "";             // Màu sơn cũ của xe
    public string NewColor { get; set; } = "";             // Màu sơn mới yêu cầu thay đổi
    public string? OldColorCode { get; set; }              // Mã màu cũ (NWAC, T2X, R4R...)
    public string? NewColorCode { get; set; }              // Mã màu mới (WW2, SAW, R2P...)
    public string? OldColorName { get; set; }              // Tên chi tiết màu cũ (Trắng ngọc trai / Đen...)
    public string? NewColorName { get; set; }              // Tên chi tiết màu mới (Đỏ đô / Xanh lục bảo...)
    public string Status { get; set; } = "Pending";        // Pending → Submitted → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Biên bản bàn giao hóa đơn & hồ sơ chứng từ xe ô tô cho Ngân hàng (BizHTC.Car.Car_BankBillMinutes / BankBillMinutes): bàn giao hồ sơ gốc, hóa đơn GTGT, giấy chứng nhận chất lượng cho Ngân hàng bảo lãnh / tài trợ tín dụng mua xe ô tô cho Đại lý.</summary>
public sealed class BankBillMinutes
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string BankBillMnNo { get; set; } = "";         // Mã biên bản bàn giao (BBM...)
    public string BankCode { get; set; } = "";             // Mã ngân hàng tiếp nhận chứng từ (VCB, TCB, VPB, BIDV, CTG, MB...)
    public string? BankName { get; set; }                  // Tên ngân hàng
    public string DealerCode { get; set; } = "";           // Mã đại lý mua xe thụ hưởng
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh ngân hàng liên kết (Pmt_Guarantee)
    public DateTime BankBillDate { get; set; } = DateTime.Now; // Ngày lập biên bản bàn giao hồ sơ
    public DateTime? BankBillReceiveDate { get; set; }     // Ngày đại diện ngân hàng ký xác nhận nhận đủ hồ sơ
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe bàn giao hồ sơ trong đợt
    public decimal TotalAmount { get; set; } = 0;          // Tổng trị giá các xe bàn giao hồ sơ (VNĐ)
    public string? BankOfficer { get; set; }               // Cán bộ / Chuyên viên đại diện Ngân hàng nhận hồ sơ
    public string? HTCOfficer { get; set; }                // Cán bộ Kế toán / Pháp chế OEM bàn giao hồ sơ
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved / HandedOver (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Diễn giải / ghi chú biên bản bàn giao
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo OEM phê duyệt / bàn giao
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong Biên bản bàn giao chứng từ ngân hàng (BizHTC.Car.Car_BankBillMinutesDtl / BankBillMinutesLine): danh sách VIN, thông tin hóa đơn GTGT, chứng thư bảo lãnh, kiểm tra bàn giao Hóa đơn gốc, Phiếu kiểm tra chất lượng XK (COC), Phiếu kiểm định PDI và Sổ bảo hành.</summary>
public sealed class BankBillMinutesLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long BankBillMinutesId { get; set; }
    public string BankBillMnNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public string? InvoiceDealerCode { get; set; }         // Đại lý ghi trên hóa đơn GTGT
    public string? InvoiceNo { get; set; }                 // Số hóa đơn GTGT điện tử (HD26-...)
    public DateTime? InvoiceDate { get; set; }             // Ngày hóa đơn GTGT
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh ngân hàng (nếu có)
    public decimal CarPrice { get; set; } = 0;             // Trị giá xuất hóa đơn / giá bán xe (VNĐ)
    public decimal GuaranteeValue { get; set; } = 0;       // Giá trị bảo lãnh thanh toán (VNĐ)
    public bool HasOriginalInvoice { get; set; } = true;   // Bàn giao Hóa đơn GTGT bản gốc / chuyển đổi điện tử
    public bool HasQualityCert { get; set; } = true;       // Bàn giao Giấy chứng nhận chất lượng xuất xưởng (COC)
    public bool HasInspectionCert { get; set; } = true;    // Bàn giao Phiếu kiểm tra chất lượng / PDI xuất xưởng
    public bool HasWarrantyBooklet { get; set; } = true;   // Bàn giao Sổ bảo hành tiêu chuẩn xe ô tô
    public string Status { get; set; } = "Pending";        // Pending → HandedOver (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Yêu cầu & Hồ sơ Đòi tiền / Khiếu nại bảo lãnh thanh toán ngân hàng mua xe ô tô cho Đại lý (BizHTC.Payment.Pmt_GrtClaim / GuaranteeClaim): phát hành lệnh yêu cầu ngân hàng giải ngân thực thi nghĩa vụ bảo lãnh khi đại lý phát sinh nợ quá hạn hoặc rủi ro thanh toán.</summary>
public sealed class GuaranteeClaim
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ClaimNo { get; set; } = "";             // Mã hồ sơ đòi tiền bảo lãnh (CLM...)
    public string DealerCode { get; set; } = "";          // Mã đại lý nợ quá hạn bị đòi bảo lãnh
    public string BankCode { get; set; } = "";            // Mã ngân hàng bảo lãnh (VCB, TCB, VPB, BIDV, CTG, MB...)
    public string? BankName { get; set; }                 // Tên ngân hàng
    public string? GuaranteeNo { get; set; }              // Mã chứng thư bảo lãnh ngân hàng liên quan (Pmt_Guarantee)
    public DateTime ClaimDate { get; set; } = DateTime.Now; // Ngày lập hồ sơ đòi tiền bảo lãnh
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng xe yêu cầu đòi bảo lãnh
    public decimal TotalClaimAmount { get; set; } = 0;    // Tổng số tiền yêu cầu ngân hàng giải ngân trả (VNĐ)
    public string ClaimReason { get; set; } = "OverduePayment"; // Lý do đòi bảo lãnh: OverduePayment (Nợ quá hạn), DealerDefault (Đại lý mất khả năng thanh toán), CreditRisk (Rủi ro tín dụng), ContractBreach (Vi phạm hợp đồng)
    public string? FileSigned { get; set; }               // Văn bản đòi tiền bảo lãnh có chữ ký số OEM
    public string? BankRefNo { get; set; }                // Số chứng từ / Ủy nhiệm chi ngân hàng giải ngân
    public DateTime? DisbursementDate { get; set; }       // Ngày ngân hàng giải ngân tiền bảo lãnh
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Claimed → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú giải trình hồ sơ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Lãnh đạo tài chính duyệt phát hành văn bản đòi bảo lãnh
    public DateTime? ApprovedAt { get; set; }
    public string? SettledBy { get; set; }                // Kế toán OEM xác nhận ngân hàng đã chuyển tiền
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }               // Người từ chối hồ sơ
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }              // Người hủy hồ sơ
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong hồ sơ đòi bảo lãnh ngân hàng (BizHTC.Payment.Pmt_GrtClaimDetail / GuaranteeClaimLine): danh sách VIN, chứng thư bảo lãnh, giá trị bảo lãnh, số tiền đòi, hạn thanh toán và số ngày nợ quá hạn.</summary>
public sealed class GuaranteeClaimLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long GuaranteeClaimId { get; set; }
    public string ClaimNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public string? GuaranteeNo { get; set; }              // Mã chứng thư bảo lãnh ngân hàng gốc
    public decimal GuaranteeValue { get; set; } = 0;      // Giá trị bảo lãnh ban đầu của xe (VNĐ)
    public decimal ClaimAmount { get; set; } = 0;         // Số tiền đòi ngân hàng thanh toán cho xe này (VNĐ)
    public DateTime? DueDate { get; set; }                // Hạn thanh toán bảo lãnh theo hợp đồng
    public int OverdueDays { get; set; } = 0;             // Số ngày quá hạn thanh toán
    public string Status { get; set; } = "Pending";       // Pending → Claimed → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Hợp đồng mua bán xe / bộ linh kiện ngoại thương CBU/CKD (BizHTC.Contract.ContractOversea / CT_ContractOversea): quản lý hợp đồng nhập khẩu xe nguyên chiếc CBU hoặc bộ linh kiện CKD giữa Hãng OEM và Nhà sản xuất quốc tế (HMC Hàn Quốc, Ấn Độ...).</summary>
public sealed class ContractOversea
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ContractNo { get; set; } = "";             // Mã hợp đồng ngoại thương (CTO...)
    public string? ContractNoUser { get; set; }            // Số hợp đồng nội bộ / tham chiếu
    public string SupplierCode { get; set; } = "";         // Mã nhà cung cấp / Hãng sản xuất ngoại (HMC, HMI, HMEC, MOBIS...)
    public string? SupplierName { get; set; }              // Tên nhà cung cấp / Hãng xe quốc tế
    public string IncotermsCode { get; set; } = "CIF_HAI_PHONG"; // Incoterms (CIF_HAI_PHONG, FOB_BUSAN, FOB_ULSAN, CFR_CAT_LAI, CIF_CAI_MEP, EXW...)
    public string Currency { get; set; } = "USD";          // Đồng tiền thanh toán quốc tế (USD, EUR, KRW, JPY, VND)
    public decimal ExchangeRate { get; set; } = 25450m;    // Tỷ giá quy đổi ngoại tệ sang VNĐ
    public string PaymentTerm { get; set; } = "LC";        // Phương thức thanh toán quốc tế: LC (Letter of Credit), TT (Telegraphic Transfer), DP, DA, OA
    public string DeparturePort { get; set; } = "BUSAN";   // Cảng xuất bến / bốc hàng (BUSAN, ULSAN, CHENNAI, INCHEON...)
    public string ArrivalPort { get; set; } = "CANG_HAI_PHONG"; // Cảng cập bến / đích (CANG_HAI_PHONG, CANG_CAT_LAI, CANG_CAI_MEP, NHA_MAY_NINH_BINH...)
    public string? OrderMonth { get; set; }                // Tháng đặt hàng (yyyy-MM)
    public string? ProductionMonth { get; set; }           // Tháng kế hoạch sản xuất (yyyy-MM)
    public string? ExpectedDeliveryMonth { get; set; }     // Tháng dự kiến cập cảng / giao nhận (yyyy-MM)
    public DateTime ContractDate { get; set; } = DateTime.Now; // Ngày ký kết hợp đồng ngoại thương
    public DateTime? DeliveryDeadline { get; set; }        // Hạn chót giao hàng theo hợp đồng
    public int TotalQuantity { get; set; } = 0;            // Tổng số lượng xe / bộ linh kiện trong hợp đồng
    public decimal TotalAmountForeign { get; set; } = 0;   // Tổng giá trị hợp đồng bằng ngoại tệ (USD/EUR...)
    public decimal TotalAmount { get; set; } = 0;          // Tổng giá trị quy đổi sang VNĐ = TotalAmountForeign * ExchangeRate
    public string? FileSigned { get; set; }                // Tệp văn bản hợp đồng ngoại thương ký số điện tử
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved → InExecution → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Điều khoản / ghi chú hợp đồng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo xuất nhập khẩu / Ban Giám Đốc phê duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng xe trong hợp đồng ngoại thương (BizHTC.Contract.CT_ContractOverseaDetail / ContractOverseaLine): thông tin model xe CBU/CKD, phiên bản spec, màu sắc, nhà máy sản xuất, đơn giá ngoại tệ USD và quy đổi VNĐ.</summary>
public sealed class ContractOverseaLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ContractOverseaId { get; set; }
    public string ContractNo { get; set; } = "";
    public string? Vin { get; set; }                       // Số khung VIN cụ thể (nếu đã phân bổ trước)
    public string Model { get; set; } = "";                // Dòng xe (SantaFe, Tucson, Palisade, Custin, Ioniq 5, Creta, Accent...)
    public string? SpecCode { get; set; }                  // Phiên bản xe (2.5T AWD Calligraphy, 1.6T HTRAC, 2.0 AT Cao cấp, Hybrid...)
    public string? Color { get; set; }                     // Màu sắc xe
    public string? ColorCode { get; set; }                 // Mã màu sơn quốc tế (NWAC, SAW, R2P, T2X...)
    public int? ModelYear { get; set; } = 2026;            // Năm sản xuất / Model Year
    public string? PlantCode { get; set; }                 // Nhà máy sản xuất (ULSAN_PLANT_1, ASAN_PLANT, CHENNAI_PLANT...)
    public string? PortCode { get; set; }                  // Mã cảng bốc hàng (BUSAN, ULSAN, CHENNAI...)
    public string? WorkOrderNo { get; set; }               // Lệnh sản xuất / Work Order liên kết
    public string? LCTemp { get; set; }                    // Mã L/C dự kiến / thư tín dụng thanh toán (CT_LC)
    public int OrderQty { get; set; } = 1;                 // Số lượng xe đặt mua
    public decimal UnitPriceForeign { get; set; } = 0;     // Đơn giá ngoại tệ (USD)
    public decimal TotalAmountForeign { get; set; } = 0;   // Thành tiền ngoại tệ (USD) = OrderQty * UnitPriceForeign
    public decimal UnitPrice { get; set; } = 0;            // Đơn giá quy đổi (VNĐ) = UnitPriceForeign * ExchangeRate
    public decimal TotalAmount { get; set; } = 0;          // Thành tiền quy đổi (VNĐ) = TotalAmountForeign * ExchangeRate
    public string Status { get; set; } = "Pending";        // Pending → Submitted → Approved → InProduction → Shipped → Delivered (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Thư tín dụng thanh toán quốc tế nhập khẩu ô tô CBU/CKD (BizHTC.Contract.ContractLC / CT_LC): mở L/C tại ngân hàng bảo đảm thanh toán hợp đồng ngoại thương cho nhà sản xuất quốc tế (HMC Hàn Quốc, Ấn Độ...).</summary>
public sealed class LetterOfCredit
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string LCNo { get; set; } = "";             // Số thư tín dụng L/C (LC-VCB-2026-...)
    public string? LCNoUser { get; set; }            // Mã L/C nội bộ / tham chiếu
    public string ContractNo { get; set; } = "";     // Số hợp đồng ngoại thương liên kết (CT_ContractOversea)
    public string BankCode { get; set; } = "VCB";    // Ngân hàng mở L/C (VCB, CTG, BIDV, TCB, MBB...)
    public string? BankName { get; set; }            // Tên ngân hàng mở L/C
    public string? BeneficiaryName { get; set; }     // Đơn vị thụ hưởng (Hyundai Motor Company - Ulsan / Seoul)
    public string? ApplicantName { get; set; }       // Đơn vị mở L/C (Công ty CP Liên doanh Ô tô Hyundai Thành Công VN)
    public string Currency { get; set; } = "USD";    // Đồng tiền L/C (USD, EUR, KRW, JPY, VND)
    public decimal ExchangeRate { get; set; } = 25450m; // Tỷ giá quy đổi L/C sang VNĐ
    public decimal LCAmountForeign { get; set; } = 0; // Trị giá L/C ngoại tệ (USD)
    public decimal LCAmount { get; set; } = 0;       // Trị giá L/C quy đổi (VNĐ) = LCAmountForeign * ExchangeRate
    public decimal MarginRate { get; set; } = 10m;   // Tỷ lệ ký quỹ mở L/C (%) (VD: 10% = 10)
    public decimal MarginAmount { get; set; } = 0;   // Tiền ký quỹ mở L/C thực tế (VNĐ) = LCAmount * MarginRate / 100
    public DateTime IssueDate { get; set; } = DateTime.Now; // Ngày phát hành mở L/C
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(90); // Ngày hết hạn hiệu lực L/C
    public DateTime? LatestShipmentDate { get; set; } // Hạn chót bốc hàng / giao hàng lên tàu
    public string PaymentTerm { get; set; } = "AtSight"; // Điều kiện: AtSight (Trả ngay), Usance30 (Trả chậm 30 ngày), Usance60, Usance90, Usance180
    public string DeparturePort { get; set; } = "BUSAN"; // Cảng bốc hàng (BUSAN, ULSAN, CHENNAI...)
    public string ArrivalPort { get; set; } = "CANG_HAI_PHONG"; // Cảng dỡ hàng (CANG_HAI_PHONG, CANG_CAT_LAI, CANG_CAI_MEP...)
    public int TotalVehicleCount { get; set; } = 0;  // Tổng số lượng xe thanh toán qua L/C
    public decimal UtilizedAmountForeign { get; set; } = 0; // Giá trị ngoại tệ đã thanh toán theo chứng từ
    public decimal UtilizedAmount { get; set; } = 0; // Giá trị quy đổi VNĐ đã thanh toán
    public decimal RemainingAmountForeign { get; set; } = 0; // Giá trị ngoại tệ còn lại chưa giải ngân
    public decimal RemainingAmount { get; set; } = 0; // Giá trị quy đổi VNĐ còn lại
    public string? SwiftCode { get; set; }           // Mã điện SWIFT MT700 / số điện chuyển tiền
    public string? FileSigned { get; set; }          // File chứng thư L/C ký số điện tử
    public string Status { get; set; } = "Draft";    // Draft → Submitted → Issued → Utilized → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }              // Ghi chú điều hành L/C
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }          // Lãnh đạo tài chính OEM duyệt mở L/C
    public DateTime? ApprovedAt { get; set; }
    public string? UtilizedBy { get; set; }          // Kế toán thanh toán quốc tế xác nhận khớp chứng từ
    public DateTime? UtilizedAt { get; set; }
    public string? SettledBy { get; set; }           // Kế toán trưởng duyệt tất toán L/C
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe / lô hàng trong Thư tín dụng L/C (BizHTC.Contract.CT_LCDetail / LetterOfCreditLine): danh mục VIN/model, số lượng, đơn giá ngoại tệ USD, quy đổi VNĐ, liên kết Packing List và Tờ khai hải quan.</summary>
public sealed class LetterOfCreditLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long LetterOfCreditId { get; set; }
    public string LCNo { get; set; } = "";
    public string ContractNo { get; set; } = "";
    public string? Vin { get; set; }
    public string Model { get; set; } = "";
    public string? SpecCode { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public int OrderQty { get; set; } = 1;
    public decimal UnitPriceForeign { get; set; } = 0;
    public decimal TotalAmountForeign { get; set; } = 0;
    public decimal UnitPrice { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public string? PackingListNo { get; set; }
    public string? DeclarationNo { get; set; }
    public string Status { get; set; } = "Pending";  // Pending → Issued → Utilized → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Lệnh sửa chữa & Dịch vụ xưởng đại lý ủy quyền (BizHTC.Car / DMS.CarService / SerROService / Ser_RO): quản lý tiếp nhận xe làm bảo dưỡng định kỳ, sửa chữa chung, đồng sơn, bảo hành và PDI tại xưởng dịch vụ.</summary>
public sealed class RepairOrder
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string RoNo { get; set; } = "";                 // Mã lệnh sửa chữa (RO-HN01-2026-0001)
    public string? RoNoUser { get; set; }                // Số phiếu tiếp nhận nội bộ xưởng (RO/2026/03/...)
    public string DealerCode { get; set; } = "";          // Đại lý / Xưởng dịch vụ thực hiện
    public string Vin { get; set; } = "";                 // Số khung VIN vào xưởng
    public string Model { get; set; } = "";               // Dòng xe
    public string? EngineNo { get; set; }                 // Số máy
    public string? PlateNo { get; set; }                  // Biển số xe vào xưởng
    public string? CustomerName { get; set; }             // Tên khách hàng / chủ xe
    public string? CustomerPhone { get; set; }            // SĐT liên hệ của khách
    public string RoType { get; set; } = "PeriodicMaintenance"; // Loại dịch vụ: PeriodicMaintenance (Bảo dưỡng định kỳ), GeneralRepair (Sửa chữa chung), BodyPaint (Đồng sơn), Warranty (Bảo hành chính hãng), PdiRepair (Khắc phục PDI), Recall (Khắc phục triệu hồi)
    public string? ServiceAdvisor { get; set; }           // Cố vấn dịch vụ tiếp nhận (Service Advisor)
    public string? Technician { get; set; }               // Kỹ thuật viên chính / Quản đốc xưởng
    public int OdoKm { get; set; } = 0;                   // Số km ODO lúc tiếp nhận vào xưởng
    public string? FuelLevel { get; set; } = "1/2";       // Mức nhiên liệu trong bình (1/4, 1/2, 3/4, Full)
    public string? CarStatus { get; set; }                // Tình trạng ngoại quan / tài sản trên xe lúc tiếp nhận
    public string? CustomerRequest { get; set; }          // Yêu cầu của khách hàng khi mang xe đến
    public string? DiagnosisNotes { get; set; }           // Kết quả chẩn đoán kỹ thuật / nguyên nhân hư hỏng
    public DateTime CheckInDate { get; set; } = DateTime.Now; // Thời điểm tiếp nhận xe vào xưởng
    public DateTime? ExpectedDeliveryDate { get; set; }   // Hạn dự kiến hoàn thành bàn giao xe
    public DateTime? ActualDeliveryDate { get; set; }     // Thời điểm thực tế bàn giao xe cho khách
    public decimal TotalLaborAmount { get; set; } = 0;    // Tổng tiền công các hạng mục dịch vụ (VNĐ)
    public decimal TotalPartAmount { get; set; } = 0;     // Tổng tiền phụ tùng thay thế (VNĐ)
    public decimal DiscountAmount { get; set; } = 0;      // Tổng tiền giảm giá / khuyến mãi (VNĐ)
    public decimal VatRate { get; set; } = 10;            // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;      // Tiền thuế VAT = (TotalLaborAmount + TotalPartAmount - DiscountAmount) * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;         // Tổng tiền thanh toán = TotalLaborAmount + TotalPartAmount - DiscountAmount + TotalVatAmount
    public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid (Chưa thanh toán), Paid (Đã thanh toán đủ), WarrantyCovered (Hãng bảo hành chi trả), InsuranceCovered (Bảo hiểm chi trả)
    public string PaymentMethod { get; set; } = "Cash";   // Cash (Tiền mặt), BankTransfer (Chuyển khoản), CreditCard (Thẻ POS), OEMWarranty (Bảo hành OEM), Insurance (Bảo hiểm)
    public string? PaymentNotes { get; set; }             // Ghi chú thanh toán / mã giao dịch ngân hàng
    public string Status { get; set; } = "Draft";         // Draft → InGarage → Repaired → Delivered → Paid (hoặc Cancelled)
    public string? Remark { get; set; }                   // Ghi chú điều hành lệnh sửa chữa
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Quản đốc duyệt tiếp nhận & báo giá
    public DateTime? ApprovedAt { get; set; }
    public string? RepairedBy { get; set; }               // Quản đốc QC nghiệm thu kỹ thuật
    public DateTime? RepairedAt { get; set; }
    public string? DeliveredBy { get; set; }              // Cố vấn dịch vụ bàn giao xe cho khách
    public DateTime? DeliveredAt { get; set; }
    public string? PaidBy { get; set; }                   // Thu ngân xưởng xác nhận thu tiền
    public DateTime? PaidAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết hạng mục công việc / Dịch vụ trong Lệnh sửa chữa (BizHTC.Car / Ser_RODetail / RepairOrderServiceLine): mã công việc, tên dịch vụ, giờ công định mức, tiền công, KTV thực hiện và kết quả xử lý.</summary>
public sealed class RepairOrderServiceLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long RepairOrderId { get; set; }
    public string RoNo { get; set; } = "";
    public string SerCode { get; set; } = "";             // Mã công việc (BD-5K, BD-10K, SC-PHANH, DS-CAN-TRUOC, KIEM-TRA-DIEN...)
    public string SerName { get; set; } = "";             // Tên hạng mục dịch vụ / công việc
    public string ServiceType { get; set; } = "Maintenance"; // Maintenance (Bảo dưỡng), Repair (Sửa chữa), BodyPaint (Đồng sơn), Inspection (Kiểm tra/Chẩn đoán)
    public decimal StandardHours { get; set; } = 1.0m;    // Số giờ công định mức (Flat rate)
    public decimal LaborPrice { get; set; } = 300000m;    // Đơn giá 1 giờ công (VNĐ)
    public decimal Discount { get; set; } = 0;            // Giảm giá tiền công (VNĐ)
    public decimal LaborAmount { get; set; } = 300000m;   // Tiền công thực tế = StandardHours * LaborPrice - Discount (VNĐ)
    public string? Technician { get; set; }               // KTV trực tiếp thực hiện hạng mục này
    public string Status { get; set; } = "Pending";       // Pending → InProgress → Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chi tiết phụ tùng thay thế trong Lệnh sửa chữa (BizHTC.Car / Ser_ROPart / RepairOrderPartLine): mã phụ tùng, tên phụ tùng, số lượng, đơn vị tính, đơn giá, nguồn thanh toán và trạng thái xuất kho xưởng.</summary>
public sealed class RepairOrderPartLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long RepairOrderId { get; set; }
    public string RoNo { get; set; } = "";
    public string PartCode { get; set; } = "";            // Mã phụ tùng chính hãng Mobis/OEM (26300-35505, 05100-00441...)
    public string PartName { get; set; } = "";            // Tên phụ tùng thay thế (Lọc dầu động cơ, Dầu máy 5W-30...)
    public string Unit { get; set; } = "Cái";             // Đơn vị tính: Cái, Lít, Bình, Bộ, Hộp...
    public decimal Quantity { get; set; } = 1;            // Số lượng xuất xưởng
    public decimal UnitPrice { get; set; } = 0;           // Đơn giá phụ tùng (VNĐ)
    public decimal Discount { get; set; } = 0;            // Giảm giá phụ tùng (VNĐ)
    public decimal TotalAmount { get; set; } = 0;         // Thành tiền = Quantity * UnitPrice - Discount (VNĐ)
    public string PaymentType { get; set; } = "Customer"; // Customer (Khách thanh toán), Warranty (Hãng bảo hành chi trả), Insurance (Bảo hiểm chi trả)
    public string Status { get; set; } = "Pending";       // Pending → Issued (Đã xuất kho xưởng) → Returned (Trả lại kho) (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Lịch hẹn Dịch vụ & Tiếp nhận xe xưởng (BizCarSv.Appointment / Ser_App): quản lý đặt lịch hẹn dịch vụ bảo dưỡng, sửa chữa, đồng sơn, bảo hành và PDI tại xưởng dịch vụ đại lý.</summary>
public sealed class ServiceAppointment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string AppNo { get; set; } = "";             // Mã lịch hẹn (APP-HN01-2026-0001)
    public string? AppNoUser { get; set; }            // Số phiếu hẹn nội bộ đại lý
    public string DealerCode { get; set; } = "";      // Mã đại lý xưởng dịch vụ tiếp nhận
    public string Vin { get; set; } = "";             // Số khung xe hẹn làm dịch vụ
    public string Model { get; set; } = "";           // Dòng xe
    public string? EngineNo { get; set; }             // Số máy
    public string? PlateNo { get; set; }              // Biển số xe hẹn dịch vụ
    public string CustomerName { get; set; } = "";   // Tên khách hàng đặt hẹn
    public string CustomerPhone { get; set; } = "";  // SĐT khách hàng
    public string ServiceType { get; set; } = "PeriodicMaintenance"; // PeriodicMaintenance (Bảo dưỡng định kỳ), GeneralRepair (Sửa chữa chung), BodyPaint (Đồng sơn), Warranty (Bảo hành), Recall (Triệu hồi), PdiRepair (Khắc phục PDI), Inspection (Kiểm tra chẩn đoán)
    public DateTime AppointmentDate { get; set; } = DateTime.Now; // Ngày hẹn làm dịch vụ
    public string AppointmentTime { get; set; } = "08:30"; // Khung giờ hẹn (08:30, 09:30, 10:30, 13:30, 14:30...)
    public int EstimatedDurationMinutes { get; set; } = 60; // Thời gian dự kiến thực hiện (phút)
    public string? ServiceAdvisor { get; set; }       // Cố vấn dịch vụ phân công tiếp đón
    public string? Technician { get; set; }           // Kỹ thuật viên chính tiếp nhận
    public string? InsNo { get; set; }                // Số thẻ / hợp đồng bảo hiểm (nếu làm bảo hiểm)
    public string? CustomerRequest { get; set; }      // Yêu cầu chi tiết của khách hàng khi đặt hẹn
    public decimal TotalEstimatedLabor { get; set; } = 0; // Tổng tiền công ước tính (VNĐ)
    public decimal TotalEstimatedParts { get; set; } = 0; // Tổng tiền phụ tùng ước tính (VNĐ)
    public decimal TotalEstimatedAmount { get; set; } = 0; // Tổng chi phí dự kiến = TotalEstimatedLabor + TotalEstimatedParts (VNĐ)
    public string Status { get; set; } = "Booked";    // Booked → Confirmed → CheckedIn → InService → Completed (hoặc Cancelled / NoShow)
    public string? RoNo { get; set; }                 // Mã lệnh sửa chữa xưởng liên kết (Ser_RO / RepairOrder)
    public string? Remark { get; set; }               // Ghi chú điều hành lịch hẹn
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ConfirmedBy { get; set; }          // CVDV / Tổng đài viên xác nhận lịch hẹn
    public DateTime? ConfirmedAt { get; set; }
    public string? CheckedInBy { get; set; }          // Cố vấn dịch vụ tiếp nhận xe thực tế tại xưởng
    public DateTime? CheckedInAt { get; set; }
    public string? CompletedBy { get; set; }          // Người xác nhận hoàn thành đợt dịch vụ
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }          // Người hủy lịch hẹn
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? NoShowAt { get; set; }           // Thời điểm đánh dấu khách vắng mặt
    public string? NoShowReason { get; set; }
}

/// <summary>Chi tiết hạng mục công việc / Dịch vụ trong Lịch hẹn (BizCarSv.Appointment / Ser_AppServiceItems / ServiceAppointmentServiceLine): mã gói dịch vụ, tên công việc, giờ công và tiền công ước tính.</summary>
public sealed class ServiceAppointmentServiceLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServiceAppointmentId { get; set; }
    public string AppNo { get; set; } = "";
    public string SerCode { get; set; } = "";         // Mã công việc / dịch vụ (BD-5K, BD-10K, KT-DIEN, SC-PHANH...)
    public string SerName { get; set; } = "";         // Tên hạng mục dịch vụ dự kiến
    public string ServiceType { get; set; } = "Maintenance"; // Maintenance (Bảo dưỡng), Repair (Sửa chữa), BodyPaint (Đồng sơn), Inspection (Kiểm tra)
    public decimal StandardHours { get; set; } = 1.0m;// Số giờ công định mức
    public decimal LaborPrice { get; set; } = 300000m;// Đơn giá 1 giờ công (VNĐ)
    public decimal Discount { get; set; } = 0;        // Giảm giá tiền công (VNĐ)
    public decimal LaborAmount { get; set; } = 300000m;// Tiền công = StandardHours * LaborPrice - Discount (VNĐ)
    public string? Technician { get; set; }           // KTV dự kiến phân công
    public string Status { get; set; } = "Pending";   // Pending → Confirmed → Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chi tiết phụ tùng / vật tư tiêu hao đặt trước trong Lịch hẹn (BizCarSv.Appointment / Ser_AppPartItems / ServiceAppointmentPartLine): mã phụ tùng Mobis/OEM, tên phụ tùng, số lượng và đơn giá dự kiến.</summary>
public sealed class ServiceAppointmentPartLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServiceAppointmentId { get; set; }
    public string AppNo { get; set; } = "";
    public string PartCode { get; set; } = "";        // Mã phụ tùng chính hãng (26300-35505, 05100-00441...)
    public string PartName { get; set; } = "";        // Tên phụ tùng đặt trước
    public string Unit { get; set; } = "Cái";         // Cái, Lít, Bình, Bộ...
    public decimal Quantity { get; set; } = 1;        // Số lượng đặt trước
    public decimal UnitPrice { get; set; } = 0;       // Đơn giá phụ tùng dự kiến (VNĐ)
    public decimal Discount { get; set; } = 0;        // Giảm giá (VNĐ)
    public decimal TotalAmount { get; set; } = 0;     // Thành tiền = Quantity * UnitPrice - Discount (VNĐ)
    public string PaymentType { get; set; } = "Customer"; // Customer (Khách thanh toán), Warranty (Bảo hành OEM), Insurance (Bảo hiểm)
    public string Status { get; set; } = "Pending";   // Pending → Confirmed → Issued (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Bản tin kỹ thuật & Hướng dẫn kỹ thuật dịch vụ xe ô tô (BizCarSv.Bulletin / Blt_Bulletin / TechnicalBulletin): hãng xe OEM phát hành các bản tin kỹ thuật TSB hướng dẫn xưởng dịch vụ đại lý cách kiểm tra, xử lý, cập nhật phần mềm hoặc thay thế phụ tùng khắc phục lỗi cho từng model xe hoặc dải VIN cụ thể.</summary>
public sealed class TechnicalBulletin
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string BulletinNo { get; set; } = "";             // Mã bản tin kỹ thuật TSB (TSB-2026-001...)
    public string? BulletinNoUser { get; set; }            // Số hiệu bản tin nội bộ / tham chiếu
    public string Title { get; set; } = "";                  // Tiêu đề bản tin kỹ thuật
    public string Category { get; set; } = "SoftwareUpdate"; // SoftwareUpdate (Cập nhật phần mềm ECU/TCU), TechnicalGuideline (Hướng dẫn kỹ thuật), ServiceCampaign (Chiến dịch dịch vụ), PartReplacement (Thay thế phụ tùng), QualityNotice (Thông báo chất lượng)
    public string? Model { get; set; }                      // Dòng xe áp dụng (SantaFe, Tucson, Accent, Creta, Elantra...)
    public string Severity { get; set; } = "Medium";         // Critical (Khẩn cấp), High (Cao), Medium (Trung bình), Low (Thấp/Khuyến nghị)
    public DateTime ReleaseDate { get; set; } = DateTime.Now; // Ngày phát hành bản tin
    public DateTime? ExpiryDate { get; set; }               // Ngày hết hiệu lực
    public string? Description { get; set; }                // Hiện tượng / Mô tả sự cố kỹ thuật
    public string? Remedy { get; set; }                     // Hướng dẫn quy trình xử lý kỹ thuật / Khắc phục
    public string? AttachmentFileName { get; set; }         // Tên tệp tài liệu PDF đính kèm
    public string? AttachmentUrl { get; set; }              // Link xem / tải tài liệu kỹ thuật
    public int TotalVehicleCount { get; set; } = 0;         // Tổng số lượng xe VIN trong phạm vi áp dụng
    public int CompletedVehicleCount { get; set; } = 0;     // Số lượng xe đã hoàn tất kiểm tra / khắc phục
    public string Status { get; set; } = "Draft";           // Draft → Published → Suspended → Archived (hoặc Cancelled)
    public string? Remark { get; set; }                     // Ghi chú điều hành bản tin
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? PublishedBy { get; set; }                // Kỹ sư trưởng / Lãnh đạo dịch vụ OEM duyệt phát hành
    public DateTime? PublishedAt { get; set; }
    public string? ArchivedBy { get; set; }                 // Người đóng / lưu trữ bản tin
    public DateTime? ArchivedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong bản tin kỹ thuật (BizCarSv.Bulletin / Btl_Bulletin_VIN / TechnicalBulletinLine): danh sách số khung VIN nằm trong phạm vi bản tin kỹ thuật TSB, theo dõi tiến độ đại lý tiếp nhận và kết quả xử lý kỹ thuật trên từng xe.</summary>
public sealed class TechnicalBulletinLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TechnicalBulletinId { get; set; }
    public string BulletinNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? PlateNo { get; set; }
    public string? DealerCode { get; set; }                 // Đại lý được phân công / thực hiện xử lý
    public string Status { get; set; } = "Pending";         // Pending → Notified → InProgress → Completed (hoặc Waived)
    public DateTime? InspectedAt { get; set; }              // Ngày giờ thực hiện kiểm tra / xử lý xe
    public DateTime? CompletedAt { get; set; }              // Ngày giờ hoàn tất
    public string? Technician { get; set; }                 // KTV trực tiếp thực hiện
    public int? OdoKm { get; set; }                         // Số km ODO lúc xử lý
    public string? RoNo { get; set; }                       // Số Repair Order / Lệnh sửa chữa dịch vụ nếu xử lý qua RO xưởng
    public string? ResultNotes { get; set; }                // Ghi chú kết quả xử lý
    public string? Remark { get; set; }
}

/// <summary>Đề nghị & Lệnh giao dịch giải ngân ngân hàng mua xe ô tô cho Đại lý (BizHTC.VietinBank &amp; BizHTC.MBBank / RQ_BankingTransactions / BankDisbursement): quản lý hồ sơ và giao dịch giải ngân tín dụng trực tuyến từ các ngân hàng thương mại (VietinBank, MBBank, VCB, BIDV, VPBank, Techcombank...) thanh toán mua xe cho hệ thống đại lý phân phối OEM.</summary>
public sealed class BankDisbursement
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string RQ_BankingTransNo { get; set; } = "";     // Mã lệnh/giao dịch giải ngân (BDIS...)
    public string? RQ_BankingTransNoUser { get; set; }     // Số tham chiếu nội bộ / hồ sơ tín dụng đại lý
    public string DealerCode { get; set; } = "";           // Mã đại lý vay vốn giải ngân mua xe
    public string BankCode { get; set; } = "VIETINBANK";   // Mã ngân hàng tài trợ giải ngân (VIETINBANK, MBBANK, VCB, BIDV, VPB, TCB...)
    public string? BankName { get; set; }                  // Tên ngân hàng / Chi nhánh tài trợ
    public string? BizResNumber { get; set; }              // Số hợp đồng tín dụng hạn mức / Giấy ĐKKD đại lý
    public string? BeneficiaryAccountNo { get; set; }      // Số tài khoản thụ hưởng của Hãng OEM (VietinBank SGD, VCB...)
    public string? BeneficiaryAccountName { get; set; }    // Tên đơn vị thụ hưởng OEM (Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam)
    public string? BeneficiaryBankCode { get; set; }       // Ngân hàng tài khoản thụ hưởng OEM
    public string DisbursementType { get; set; } = "AutoLoan"; // AutoLoan (Tài trợ lô xe đại lý), FloorPlan (Vay hạn mức lưu kho), WorkingCapital (Bổ sung vốn lưu động)
    public int TotalVehicleCount { get; set; } = 0;        // Tổng số lượng xe trong hồ sơ giải ngân
    public decimal TotalCollateralValue { get; set; } = 0; // Tổng giá trị định giá các xe thế chấp / bảo lãnh (VNĐ)
    public decimal DisbursementRate { get; set; } = 80;    // Tỷ lệ giải ngân bình quân (%) (VD: 80% = 80)
    public decimal TotalDisbursementAmount { get; set; } = 0; // Tổng số tiền đề nghị giải ngân (VNĐ)
    public decimal DisbursedAmount { get; set; } = 0;      // Tổng số tiền ngân hàng đã giải ngân thực tế (VNĐ)
    public string BkTransStatus { get; set; } = "Draft";   // Draft → Submitted → Approved → PushedToBank → Disbursed (hoặc Rejected / Cancelled)
    public string BkTransBankStatus { get; set; } = "Pending"; // Pending → BankProcessing → BankApproved → Disbursed (hoặc BankRejected)
    public string? RefBankCode { get; set; }               // Mã giao dịch Core Banking / Số bút toán giải ngân của ngân hàng
    public DateTime? DisbursementDate { get; set; }        // Ngày ngân hàng hạch toán giải ngân tiền về tài khoản OEM
    public string? BankRemark { get; set; }                // Phản hồi / ghi chú từ hệ thống Ngân hàng
    public string? FilePath { get; set; }                  // Đường dẫn tệp đính kèm ủy nhiệm chi / khế ước nhận nợ
    public string? Remark { get; set; }                    // Diễn giải / ghi chú đề nghị giải ngân
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo tài chính OEM duyệt hồ sơ
    public DateTime? ApprovedAt { get; set; }
    public string? PushedBy { get; set; }                  // Cán bộ IT / Tài chính thực hiện đẩy lệnh sang Core Banking
    public DateTime? PushedAt { get; set; }
    public string? DisbursedBy { get; set; }               // Kế toán OEM xác nhận tiền về & giải phóng công nợ xe
    public DateTime? DisbursedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng xe trong hồ sơ giải ngân ngân hàng (BizHTC.VietinBank &amp; BizHTC.MBBank / RQ_BankingTransactionsDetail / BankDisbursementLine): thông tin xe VIN, hóa đơn GTGT, bảo lãnh ngân hàng, đơn giá xe, định giá thế chấp, tỷ lệ và số tiền giải ngân từng xe.</summary>
public sealed class BankDisbursementLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long BankDisbursementId { get; set; }
    public string RQ_BankingTransNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public string? InvoiceNo { get; set; }                 // Số hóa đơn GTGT bán xe cho đại lý (nếu có)
    public DateTime? InvoiceDate { get; set; }             // Ngày hóa đơn GTGT
    public string? GuaranteeNo { get; set; }               // Mã chứng thư bảo lãnh ngân hàng liên quan (Pmt_Guarantee)
    public decimal UnitPrice { get; set; } = 0;            // Đơn giá xe xuất buôn (VNĐ)
    public decimal CollateralValue { get; set; } = 0;      // Trị giá xe định giá thế chấp / giải ngân (VNĐ)
    public decimal DisbursementPercent { get; set; } = 80; // Tỷ lệ giải ngân cho xe này (%)
    public decimal DisbursementAmount { get; set; } = 0;   // Số tiền đề nghị giải ngân xe này (VNĐ) = CollateralValue * DisbursementPercent / 100
    public decimal DisbursedAmount { get; set; } = 0;      // Số tiền ngân hàng đã giải ngân thực tế cho xe này (VNĐ)
    public string Status { get; set; } = "Pending";        // Pending → Approved → Disbursed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chiến dịch Dịch vụ &amp; Khuyến mãi Hậu mãi xe ô tô (BizCarSv.CampaignMarketing / Ser_CampaignMarketing / ServiceCampaign): hãng xe OEM hoặc đại lý phát hành các chiến dịch bảo dưỡng, kiểm tra miễn phí, giảm giá dầu mỡ/phụ tùng và tặng quà tri ân khách hàng theo mùa vụ hoặc dải VIN.</summary>
public sealed class ServiceCampaign
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CamMarketingNo { get; set; } = "";         // Mã chiến dịch khuyến mãi (CAM-2026-001...)
    public string? CamMarketingNoUser { get; set; }        // Mã chiến dịch nội bộ / tham chiếu
    public string CampaignName { get; set; } = "";           // Tên chiến dịch dịch vụ & CSKH
    public string CampaignType { get; set; } = "SeasonalService"; // SeasonalService (Chăm sóc xe mùa hè/mùa đông/Tết), FreeCheckup (Kiểm tra miễn phí 20 hạng mục), DiscountOilParts (Khuyến mại dầu nhớt phụ tùng), CustomerCare (Tri ân khách hàng), SafetyCheck (Chiến dịch an toàn kỹ thuật), RecallRelated (Hỗ trợ triệu hồi)
    public string? Model { get; set; }                      // Dòng xe áp dụng (All, SantaFe, Tucson, Accent, Creta, Grand i10...)
    public DateTime DateStart { get; set; } = DateTime.Now; // Ngày bắt đầu chiến dịch
    public DateTime DateEnd { get; set; } = DateTime.Now.AddDays(30); // Ngày kết thúc chiến dịch
    public decimal DiscountLaborPercent { get; set; } = 0;   // % giảm giá tiền công bảo dưỡng/sửa chữa (VD: 20% = 20)
    public decimal DiscountPartPercent { get; set; } = 0;    // % giảm giá phụ tùng tiêu hao & dầu nhớt (VD: 15% = 15)
    public string? FreeInspectionItems { get; set; }         // Danh mục hạng mục kiểm tra kỹ thuật miễn phí (20 hạng mục an toàn, ắc quy, phanh, lốp...)
    public string? GiftDescription { get; set; }             // Quà tặng tri ân kèm theo (Ô dù cao cấp Hyundai, Bình nước Lock&Lock, Gối tựa đầu, Nước hoa xe hơi...)
    public decimal BudgetAmount { get; set; } = 0;           // Ngân sách dự toán cho chiến dịch (VNĐ)
    public decimal ActualAmount { get; set; } = 0;           // Tổng chi phí ưu đãi thực tế đã chi trả cho khách hàng (VNĐ)
    public int TotalVehicleCount { get; set; } = 0;          // Tổng số lượng xe trong danh sách chiến dịch
    public int AttendedVehicleCount { get; set; } = 0;       // Số lượng xe đã thực tế vào xưởng tham gia hưởng ưu đãi
    public string Status { get; set; } = "Draft";            // Draft → Approved / Active → InProgress → Completed (hoặc Suspended / Cancelled)
    public string? Remark { get; set; }                      // Ghi chú / điều kiện áp dụng chiến dịch
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                  // Lãnh đạo dịch vụ / Marketing OEM duyệt ban hành
    public DateTime? ApprovedAt { get; set; }
    public string? CompletedBy { get; set; }                 // Người đóng / tổng kết chiến dịch
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe &amp; Đại lý tham gia trong Chiến dịch dịch vụ (BizCarSv.CampaignMarketing / Ser_CampaignMarketingLine / ServiceCampaignLine): danh sách số khung VIN, thông tin khách hàng, số RO xưởng phát sinh, tiền ưu đãi công &amp; phụ tùng, quà tặng đã trao và trạng thái hoàn tất.</summary>
public sealed class ServiceCampaignLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServiceCampaignId { get; set; }
    public string CamMarketingNo { get; set; } = "";
    public string DealerCode { get; set; } = "";             // Đại lý thực hiện tiếp nhận xe
    public string Vin { get; set; } = "";                    // Số khung VIN
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? PlateNo { get; set; }                     // Biển số xe vào xưởng
    public string? CustomerName { get; set; }                // Tên khách hàng
    public string? CustomerPhone { get; set; }               // SĐT liên hệ
    public DateTime? ServiceDate { get; set; }               // Thời điểm xe vào xưởng làm dịch vụ
    public string? RoNo { get; set; }                        // Mã Lệnh sửa chữa xưởng liên kết (Ser_RO / RepairOrder)
    public decimal DiscountLaborAmount { get; set; } = 0;    // Tiền công bảo dưỡng được giảm (VNĐ)
    public decimal DiscountPartAmount { get; set; } = 0;     // Tiền phụ tùng & dầu nhờn được giảm (VNĐ)
    public decimal TotalDiscountAmount { get; set; } = 0;    // Tổng tiền ưu đãi giảm giá = DiscountLaborAmount + DiscountPartAmount
    public bool IsGiftDelivered { get; set; } = false;       // Đã trao quà tặng tri ân cho khách
    public string? GiftName { get; set; }                    // Tên quà tặng đã trao
    public string? Technician { get; set; }                  // Kỹ thuật viên trực tiếp thực hiện
    public string? ServiceAdvisor { get; set; }              // Cố vấn dịch vụ tiếp đón
    public string Status { get; set; } = "Pending";          // Pending → Registered → Attended → Completed (hoặc Waived)
    public string? Remark { get; set; }
}

/// <summary>Báo cáo & Quyết toán Bảo hành xe ô tô OEM / Đại lý ủy quyền (BizCarSv.WarrantyReport / Ser_ROWarrantyReport / WarrantyReport): quản lý lập hồ sơ đề nghị hãng OEM thanh toán chi phí bảo hành (tiền công + phụ tùng thay mới) theo lệnh sửa chữa xưởng RO, quy trình thẩm định kỹ thuật, mã bản chất hư hỏng, mã nguyên nhân gốc, phê duyệt duyệt chi và quyết toán bù trừ công nợ.</summary>
public sealed class WarrantyReport
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ROWNo { get; set; } = "";             // Mã số báo cáo bảo hành (WR-HN01-2026-0001...)
    public string? ROWNoUser { get; set; }            // Số báo cáo bảo hành nội bộ / tham chiếu đại lý
    public string DealerCode { get; set; } = "";       // Mã đại lý ủy quyền lập báo cáo
    public string? DealerName { get; set; }            // Tên đại lý
    public string? RoNo { get; set; }                  // Số Lệnh sửa chữa xưởng liên quan (Ser_RO / RepairOrder)
    public string Vin { get; set; } = "";              // Số khung VIN bảo hành
    public string? PlateNo { get; set; }               // Biển số xe
    public string Model { get; set; } = "";            // Dòng xe
    public string? EngineNo { get; set; }              // Số máy
    public int OdoKm { get; set; } = 0;                // Số km ODO lúc phát sinh sự cố hư hỏng
    public DateTime CheckInDate { get; set; } = DateTime.Now; // Ngày tiếp nhận / phát sinh hư hỏng
    public DateTime? StartDate { get; set; }           // Ngày bắt đầu thực hiện bảo hành
    public DateTime? FinishedDate { get; set; }        // Ngày sửa chữa thay thế bảo hành hoàn tất
    public DateTime? WarrantyStartDate { get; set; }   // Ngày bắt đầu bảo hành xe
    public DateTime? WarrantyEndDate { get; set; }     // Ngày hết hạn bảo hành xe
    public int WarrantyMonths { get; set; } = 36;      // Thời hạn bảo hành tiêu chuẩn (tháng)
    public string? CusName { get; set; }               // Tên khách hàng / chủ xe
    public string? CusTel { get; set; }                // SĐT chủ xe
    public string? CusAddress { get; set; }            // Địa chỉ chủ xe
    public string? CusRequest { get; set; }            // Hiện tượng hư hỏng / Triệu chứng phàn nàn của khách
    public string? DiagnosticResult { get; set; }      // Kết quả chẩn đoán kỹ thuật của đại lý
    public string NaturalCode { get; set; } = "C01";   // Phân loại bản chất hư hỏng: C01 (Cháy hỏng), C02 (Rò rỉ), C03 (Nứt vỡ/Biến dạng), C04 (Lỗi điện tử/Cảm biến), C05 (Tiếng kêu bất thường), C06 (Mòn sớm)
    public string CauseCode { get; set; } = "M01";     // Phân loại nguyên nhân gốc: M01 (Khuyết tật vật liệu), M02 (Lỗi lắp ráp nhà máy), M03 (Khiếm khuyết thiết kế), M04 (Ăn mòn tự nhiên), M05 (Lỗi linh kiện Tier-1)
    public string? MainPartCode { get; set; }          // Mã phụ tùng chính gây hư hỏng (Causal Part)
    public string? MainPartName { get; set; }          // Tên phụ tùng chính gây hư hỏng
    public string WarrantyType { get; set; } = "Standard"; // Standard (Bảo hành tiêu chuẩn), Campaign (Bảo hành theo chiến dịch), GoodWill (Bảo hành thiện chí), Extended (Bảo hành mở rộng)
    public decimal TotalLaborAmount { get; set; } = 0; // Tổng tiền công đại lý đề nghị bồi hoàn (VNĐ)
    public decimal TotalPartAmount { get; set; } = 0;  // Tổng tiền phụ tùng đại lý đề nghị bồi hoàn (VNĐ)
    public decimal TotalAmount { get; set; } = 0;      // Tổng chi phí bảo hành đề nghị = TotalLaborAmount + TotalPartAmount (VNĐ)
    public decimal ApprovedLaborAmount { get; set; } = 0; // Tiền công OEM duyệt chi trả (VNĐ)
    public decimal ApprovedPartAmount { get; set; } = 0;  // Tiền phụ tùng OEM duyệt chi trả (VNĐ)
    public decimal ApprovedTotalAmount { get; set; } = 0; // Tổng tiền bảo hành OEM duyệt quyết toán = ApprovedLaborAmount + ApprovedPartAmount (VNĐ)
    public decimal ReimbursedAmount { get; set; } = 0; // Tiền thực tế đã thanh toán bù trừ cho đại lý (VNĐ)
    public DateTime? ReimburseDate { get; set; }       // Ngày thực hiện chi trả / hạch toán bù trừ
    public string? AccountingRefNo { get; set; }       // Số chứng từ hạch toán bù trừ công nợ ERP
    public string OldPartsInspectionStatus { get; set; } = "PendingReturn"; // PendingReturn (Chờ trả về OEM), Inspected (Đã nghiệm thu xác phụ tùng), ReturnedToFactory (Đã về kho bảo hành OEM), ScrappedOnSite (Hủy tại chỗ có giám sát), Waived (Miễn thu hồi)
    public string Status { get; set; } = "Draft";      // Draft → Submitted → Confirmed → Approved → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                // Ghi chú điều hành bảo hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ConfirmedBy { get; set; }           // Kỹ thuật viên/Cố vấn kỹ thuật OEM sơ duyệt
    public DateTime? ConfirmedAt { get; set; }
    public string? ApprovedBy { get; set; }            // Lãnh đạo Phòng Dịch vụ/Bảo hành OEM duyệt quyết toán
    public DateTime? ApprovedAt { get; set; }
    public string? SettledBy { get; set; }             // Kế toán OEM hạch toán chi trả
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết hạng mục công việc bảo hành (BizCarSv.WarrantyReport / Ser_ROWarrantyReportServiceItems / WarrantyReportLaborLine): mã công việc, tên dịch vụ bảo hành, giờ công định mức OEM, tiền công đề nghị và tiền công OEM duyệt chi trả.</summary>
public sealed class WarrantyReportLaborLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long WarrantyReportId { get; set; }
    public string ROWNo { get; set; } = "";
    public string SerCode { get; set; } = "";          // Mã công việc sửa chữa/thay thế bảo hành (BH-THAY-LOC, BH-THAY-HOP-SO, BH-THAY-THUOC-LAI...)
    public string SerName { get; set; } = "";          // Tên hạng mục công việc bảo hành
    public decimal StdManHour { get; set; } = 1.0m;    // Giờ công định mức (Flat Rate OEM)
    public decimal LaborPrice { get; set; } = 300000m; // Đơn giá 1 giờ công bảo hành hãng quy định (VNĐ)
    public decimal LaborAmount { get; set; } = 300000m;// Tiền công đề nghị = StdManHour * LaborPrice (VNĐ)
    public decimal ApprovedManHour { get; set; } = 1.0m; // Số giờ công OEM thẩm định duyệt
    public decimal ApprovedLaborAmount { get; set; } = 300000m; // Tiền công OEM duyệt chi trả (VNĐ)
    public string? Technician { get; set; }            // KTV thực hiện xử lý
    public string Status { get; set; } = "Pending";    // Pending → Approved → Settled (hoặc Rejected)
    public string? RejectReason { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Chi tiết phụ tùng bảo hành thay thế (BizCarSv.WarrantyReport / Ser_ROWarrantyReportPartItems / WarrantyReportPartLine): mã phụ tùng Mobis/OEM, số lượng, đơn giá, số tiền đề nghị, số lượng & tiền OEM duyệt và tình trạng thu hồi xác linh kiện cũ.</summary>
public sealed class WarrantyReportPartLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long WarrantyReportId { get; set; }
    public string ROWNo { get; set; } = "";
    public string PartCode { get; set; } = "";         // Mã phụ tùng chính hãng Mobis/OEM (26300-35505, 56500-D3000...)
    public string PartName { get; set; } = "";         // Tên phụ tùng thay mới bảo hành
    public string Unit { get; set; } = "Cái";          // Đơn vị tính: Cái, Bình, Bộ, Hộp...
    public decimal Quantity { get; set; } = 1;         // Số lượng phụ tùng thay thế đề nghị bồi hoàn
    public decimal UnitPrice { get; set; } = 0;        // Đơn giá phụ tùng bảo hành xuất hãng (VNĐ)
    public decimal TotalAmount { get; set; } = 0;      // Tiền phụ tùng đề nghị = Quantity * UnitPrice (VNĐ)
    public decimal ApprovedQty { get; set; } = 1;      // Số lượng phụ tùng OEM duyệt bồi hoàn
    public decimal ApprovedAmount { get; set; } = 0;   // Tiền phụ tùng OEM duyệt bồi hoàn = ApprovedQty * UnitPrice (VNĐ)
    public bool IsMainPart { get; set; } = false;      // Là phụ tùng chính gây ra hỏng hóc (Causal Part)
    public string? OldPartSerialNo { get; set; }       // Số serial / mã barcode linh kiện cũ hỏng
    public string OldPartReturnStatus { get; set; } = "PendingReturn"; // PendingReturn (Chờ trả về OEM), Returned (Đã nhập kho linh kiện lỗi OEM), ScrappedOnSite (Hủy tại đại lý), Waived (Miễn thu hồi)
    public string Status { get; set; } = "Pending";    // Pending → Approved → Settled (hoặc Rejected)
    public string? RejectReason { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Báo giá Dịch vụ & Phụ tùng xưởng sửa chữa xe ô tô (BizCarSv.Inventory.Quote / Ser_Quotation / Ser_Inv_Quote / ServiceQuotation): quản lý lập báo giá dự toán chi phí sửa chữa, bảo dưỡng định kỳ, phụ tùng thay thế và tiền công trước khi khách hàng duyệt thực hiện dịch vụ, luồng phê duyệt & chuyển đổi sang Lệnh sửa chữa xưởng RepairOrder.</summary>
public sealed class ServiceQuotation
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string QuoteNo { get; set; } = "";             // Mã số báo giá (QT-HN01-2026-0001...)
    public string? QuoteNoUser { get; set; }            // Số báo giá nội bộ / tham chiếu đại lý
    public string DealerCode { get; set; } = "";        // Mã đại lý lập báo giá
    public string? DealerName { get; set; }             // Tên đại lý
    public string Vin { get; set; } = "";               // Số khung VIN báo giá
    public string Model { get; set; } = "";             // Dòng xe
    public string? EngineNo { get; set; }               // Số máy
    public string? PlateNo { get; set; }                // Biển số xe vào xưởng
    public int OdoKm { get; set; } = 0;                 // Số km ODO lúc lập báo giá
    public string CustomerName { get; set; } = "";      // Tên khách hàng / chủ xe
    public string CustomerPhone { get; set; } = "";     // SĐT liên hệ của khách hàng
    public string? CustomerAddress { get; set; }        // Địa chỉ khách hàng
    public string CustomerType { get; set; } = "Individual"; // Individual (Cá nhân), Corporate (Doanh nghiệp), InsuranceCompany (Bảo hiểm)
    public string QuotationType { get; set; } = "PeriodicMaintenance"; // PeriodicMaintenance (Bảo dưỡng định kỳ), GeneralRepair (Sửa chữa chung), BodyPaint (Đồng sơn), InsuranceClaim (Tổn thất bảo hiểm), WarrantyEstimate (Dự toán bảo hành), Custom (Khác)
    public string? ServiceAdvisor { get; set; }         // Cố vấn dịch vụ lập báo giá
    public DateTime QuoteDate { get; set; } = DateTime.Now; // Ngày lập báo giá
    public DateTime? ValidUntilDate { get; set; }       // Hạn hiệu lực của báo giá (thường 15-30 ngày)
    public string PaymentMethod { get; set; } = "Cash"; // Cash (Tiền mặt), BankTransfer (Chuyển khoản), Insurance (Bảo hiểm), Warranty (Bảo hành OEM)
    public decimal TotalLaborAmount { get; set; } = 0;  // Tổng tiền công các hạng mục dịch vụ (VNĐ)
    public decimal TotalPartAmount { get; set; } = 0;   // Tổng tiền phụ tùng thay thế (VNĐ)
    public decimal DiscountAmount { get; set; } = 0;    // Tổng tiền giảm giá / chiết khấu ưu đãi (VNĐ)
    public decimal VatRate { get; set; } = 10;          // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;    // Tiền thuế VAT = (TotalLaborAmount + TotalPartAmount - DiscountAmount) * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;       // Tổng tiền thanh toán dự toán = TotalLaborAmount + TotalPartAmount - DiscountAmount + TotalVatAmount
    public string Status { get; set; } = "Draft";       // Draft → Sent → CustomerApproved → Converted (hoặc CustomerRejected / Cancelled / Expired)
    public bool ApprovedByCustomer { get; set; } = false; // Khách hàng đã ký duyệt đồng ý sửa chữa
    public DateTime? CustomerApprovedAt { get; set; }   // Thời điểm khách duyệt
    public string? CustomerSignature { get; set; }      // Chữ ký số / ghi chú xác nhận của khách hàng
    public string? ConvertedRoNo { get; set; }          // Mã Lệnh sửa chữa RepairOrder được sinh tự động khi chuyển đổi
    public DateTime? ConvertedAt { get; set; }          // Thời điểm chuyển đổi thành RO
    public string? Remark { get; set; }                 // Ghi chú điều khoản / điều kiện báo giá
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? SentBy { get; set; }                 // Cố vấn dịch vụ gửi báo giá cho khách
    public DateTime? SentAt { get; set; }
    public string? RejectedBy { get; set; }             // Người từ chối báo giá (khách / đại lý)
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }            // Người hủy báo giá
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết hạng mục công việc / Dịch vụ trong Báo giá (BizCarSv.Inventory.Quote / ServiceQuotationLaborLine): mã công việc, tên dịch vụ, giờ công định mức, đơn giá tiền công, chiết khấu và KTV dự kiến.</summary>
public sealed class ServiceQuotationLaborLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServiceQuotationId { get; set; }
    public string QuoteNo { get; set; } = "";
    public string SerCode { get; set; } = "";           // Mã công việc (BD-5K, BD-10K, SC-PHANH, DS-CAN-TRUOC, KT-DIEN...)
    public string SerName { get; set; } = "";           // Tên hạng mục dịch vụ dự toán
    public string ServiceType { get; set; } = "Maintenance"; // Maintenance (Bảo dưỡng), Repair (Sửa chữa), BodyPaint (Đồng sơn), Inspection (Kiểm tra chẩn đoán)
    public decimal StandardHours { get; set; } = 1.0m;  // Số giờ công định mức (Flat Rate)
    public decimal LaborPrice { get; set; } = 300000m;  // Đơn giá 1 giờ công (VNĐ)
    public decimal Discount { get; set; } = 0;          // Giảm giá tiền công (VNĐ)
    public decimal LaborAmount { get; set; } = 300000m; // Tiền công thực tế = StandardHours * LaborPrice - Discount (VNĐ)
    public string? Technician { get; set; }             // KTV dự kiến phân công
    public string Status { get; set; } = "Pending";     // Pending → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chi tiết phụ tùng thay thế trong Báo giá (BizCarSv.Inventory.Quote / ServiceQuotationPartLine): mã phụ tùng Mobis/OEM chính hãng, tên phụ tùng, đơn vị tính, số lượng, đơn giá, chiết khấu và nguồn chi trả.</summary>
public sealed class ServiceQuotationPartLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServiceQuotationId { get; set; }
    public string QuoteNo { get; set; } = "";
    public string PartCode { get; set; } = "";          // Mã phụ tùng chính hãng Mobis/OEM (26300-35505, 05100-00441...)
    public string PartName { get; set; } = "";          // Tên phụ tùng thay thế
    public string Unit { get; set; } = "Cái";           // Đơn vị tính: Cái, Lít, Bình, Bộ, Hộp...
    public decimal Quantity { get; set; } = 1;          // Số lượng phụ tùng dự toán
    public decimal UnitPrice { get; set; } = 0;         // Đơn giá phụ tùng (VNĐ)
    public decimal Discount { get; set; } = 0;          // Giảm giá phụ tùng (VNĐ)
    public decimal TotalAmount { get; set; } = 0;       // Thành tiền = Quantity * UnitPrice - Discount (VNĐ)
    public string PaymentType { get; set; } = "Customer"; // Customer (Khách thanh toán), Warranty (Bảo hành OEM chi trả), Insurance (Bảo hiểm chi trả)
    public string Status { get; set; } = "Pending";     // Pending → Approved (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Chăm sóc khách hàng & Khảo sát chỉ số hài lòng CSI sau dịch vụ/bán xe (BizCarSv.Customer / Ser_CustomerCare, Ser_CustomerCare24h, Ser_CustomerCare72h, Ser_CustomerCareMaintance, Ser_CustomerCareBth): quản lý luồng chăm sóc khách hàng sau khi nhận xe mới hoặc sau bảo dưỡng sửa chữa xưởng, khảo sát CSI 24h & 72h, nhắc hạn bảo dưỡng định kỳ và chúc mừng sinh nhật chủ xe.</summary>
public sealed class CustomerCare
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CareNo { get; set; } = "";             // Mã phiếu CSKH (CC26-...)
    public string? CareNoUser { get; set; }              // Mã phiếu do người dùng / đại lý nhập (nếu có)
    public string DealerCode { get; set; } = "";         // Đại lý thực hiện CSKH
    public string? DealerName { get; set; }              // Tên đại lý
    public string Vin { get; set; } = "";                // Số khung xe
    public string? PlateNo { get; set; }                 // Biển số xe
    public string? Model { get; set; }                   // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra...)
    public string? EngineNo { get; set; }                // Số máy
    public string? CustomerName { get; set; }            // Tên chủ xe / khách hàng
    public string? CustomerPhone { get; set; }           // Số điện thoại khách hàng
    public string? CustomerEmail { get; set; }           // Email khách hàng
    public string? CustomerAddress { get; set; }         // Địa chỉ khách hàng
    public string CareType { get; set; } = "FollowUp72h"; // FollowUp24h (Bàn giao xe mới), FollowUp72h (Khảo sát dịch vụ xưởng), MaintenanceReminder (Nhắc bảo dưỡng định kỳ), Birthday (Chúc mừng sinh nhật), SeasonalCare (Chăm sóc xe theo mùa vụ)
    public string ContactMethod { get; set; } = "PhoneCall"; // PhoneCall (Gọi điện), SMS (Tin nhắn), ZaloZNS (Zalo), Email (Email), InGarage (Trực tiếp)
    public string? RoNo { get; set; }                    // Mã lệnh sửa chữa xưởng liên quan (Ser_RO)
    public string? DoNo { get; set; }                    // Mã lệnh giao xe liên quan (DeliveryOrder / DLS_Deal)
    public int? OdoKm { get; set; }                      // Số km ODO ghi nhận
    public DateTime? ServiceDate { get; set; }           // Ngày làm dịch vụ / Ngày nhận xe
    public DateTime? ContactDate { get; set; }           // Ngày liên hệ thực tế
    public DateTime? NextCareDate { get; set; }          // Ngày hẹn chăm sóc / bảo dưỡng tiếp theo
    public int CallAttempts { get; set; } = 1;           // Số lần thử gọi / liên hệ
    public string? CareStaff { get; set; }               // Nhân viên CSKH phụ trách
    public string? ServiceAdvisor { get; set; }          // Cố vấn dịch vụ phụ trách xe

    // Khảo sát & Đánh giá chỉ số CSI (Customer Satisfaction Index)
    public decimal ScoreOverall { get; set; } = 5.0m;    // Điểm hài lòng tổng quan (1.0 - 5.0 sao)
    public decimal ScoreQuality { get; set; } = 5.0m;    // Điểm chất lượng kỹ thuật sửa chữa / xe (1.0 - 5.0 sao)
    public decimal ScoreAdvisor { get; set; } = 5.0m;    // Điểm thái độ & tính chuyên nghiệp của Cố vấn (1.0 - 5.0 sao)
    public decimal ScoreFacility { get; set; } = 5.0m;   // Điểm cơ sở vật chất phòng chờ & tiếp đón (1.0 - 5.0 sao)
    public bool IsProblemSolved { get; set; } = true;    // Sự cố kỹ thuật của xe đã được giải quyết triệt để
    public int NpsScore { get; set; } = 10;              // Chỉ số NPS sẵn sàng giới thiệu bạn bè (0 - 10)
    public string? CustomerFeedback { get; set; }        // Ý kiến phản hồi / góp ý chi tiết của chủ xe
    public string? RemedyAction { get; set; }            // Phương án xử lý khiếu nại / khắc phục sự cố nếu khách chưa hài lòng
    public bool IsResolved { get; set; } = true;         // Khiếu nại đã được giải quyết thỏa đáng

    public string Status { get; set; } = "Pending";      // Pending → Contacting → Completed (hoặc Escalated / Unreachable / Cancelled)
    public string? EscalatedTo { get; set; }             // Chuyển cấp quản lý xử lý (ServiceManager, Director...)
    public DateTime? EscalatedAt { get; set; }           // Thời điểm chuyển cấp khiếu nại
    public string? CompletedBy { get; set; }             // Người hoàn tất CSKH
    public DateTime? CompletedAt { get; set; }           // Thời điểm hoàn tất CSKH
    public string? CancelledBy { get; set; }             // Người hủy
    public DateTime? CancelledAt { get; set; }           // Thời điểm hủy
    public string? CancelReason { get; set; }            // Lý do hủy
    public string? Remark { get; set; }                  // Ghi chú nghiệp vụ
    public string? CreatedBy { get; set; }               // Người tạo
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Lệnh sản xuất & Kế hoạch sản xuất xe ô tô tại Nhà máy OEM (BizHTC.WorkOrder & BizHTC.MMSIntergration / MnfPl_Order / ProductionOrder): quản lý kế hoạch đặt hàng sản xuất xe hàng tháng/quý cho Nhà máy HTMV Ninh Bình, theo dõi số lượng kế hoạch, số lượng đã xuất xưởng KCS, ngày ETA dự kiến và tự động sinh mã số khung VIN vào kho InStock khi hoàn tất sản xuất.</summary>
public sealed class ProductionOrder
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string OrderNo { get; set; } = "";             // Mã lệnh sản xuất (PO-2026-03-0001, WO-...)
    public string? OrderNoUser { get; set; }            // Mã lệnh nội bộ nhà máy / tham chiếu
    public string OrdMonth { get; set; } = "";           // Tháng sản xuất kế hoạch (YYYY-MM, ví dụ: 2026-03)
    public string OrdType { get; set; } = "MTO";         // Loại đơn hàng: MTO (Make to Order), MTS (Make to Stock), SAMPLE (Xe mẫu thử nghiệm), EXPORT (Xuất khẩu)
    public string OrdCategoryType { get; set; } = "MakeToOrder"; // Phân loại: MakeToOrder, Regular, Urgent
    public string PlantCode { get; set; } = "HTMV_NINHBINH_1"; // Nhà máy sản xuất: HTMV_NINHBINH_1 (Nhà máy Hyundai Ninh Bình 1), HTMV_NINHBINH_2 (Nhà máy 2), TCV_PLANT (Nhà máy xe thương mại)
    public string? PlantName { get; set; }              // Tên nhà máy sản xuất
    public int TotalPlanQty { get; set; } = 0;          // Tổng số lượng xe kế hoạch đặt sản xuất trong đợt
    public int TotalProducedQty { get; set; } = 0;      // Tổng số lượng xe thực tế đã xuất xưởng KCS
    public DateTime? EstimatedCompletionDate { get; set; } // Ngày hoàn thành dự kiến của toàn bộ lô sản xuất
    public string Status { get; set; } = "Draft";       // Draft → Submitted → Scheduled → InProduction → Completed (hoặc Cancelled)
    public string? Remark { get; set; }                 // Ghi chú yêu cầu sản xuất
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ScheduledBy { get; set; }            // Kế hoạch viên nhà máy lập lịch sản xuất
    public DateTime? ScheduledAt { get; set; }
    public string? StartedBy { get; set; }              // Quản đốc phân xưởng đưa vào dây chuyền sản xuất
    public DateTime? StartedAt { get; set; }
    public string? CompletedBy { get; set; }            // Quản đốc KCS nghiệm thu xuất xưởng đóng lệnh
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng sản phẩm trong Lệnh sản xuất (BizHTC.WorkOrder & BizHTC.MMSIntergration / MnfPl_OrderDtl / ProductionOrderLine): dòng xe Model, phiên bản Spec, mã màu sơn, số lượng kế hoạch tháng N0, dự kiến N+1..N+3, ngày ETA, công đoạn sản xuất và số lượng đã xuất xưởng KCS.</summary>
public sealed class ProductionOrderLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ProductionOrderId { get; set; }
    public string OrderNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;              // Thứ tự dòng sản xuất
    public string Model { get; set; } = "";              // Dòng xe (SantaFe, Tucson, Accent, Creta, Grand i10, Custin, Palisade, Stargazer, Ioniq 5...)
    public string SpecCode { get; set; } = "";          // Mã phiên bản xe (2.5T AWD Calligraphy, 2.0 AT Đặc Biệt, 1.5 AT Cao Cấp, EV 72.6kWh...)
    public string? SpecDescription { get; set; }        // Mô tả chi tiết cấu hình kỹ thuật xe
    public string ColorCode { get; set; } = "NWAC/Black"; // Mã màu sơn ngoại thất / nội thất
    public string? ColorName { get; set; }              // Tên màu sắc (Trắng Ngọc Trai / Nội Thất Đen...)
    public int PlanQty { get; set; } = 1;               // Số lượng xe kế hoạch sản xuất trong tháng N0
    public int QtyMonthN1 { get; set; } = 0;            // Số lượng dự kiến tháng N+1
    public int QtyMonthN2 { get; set; } = 0;            // Số lượng dự kiến tháng N+2
    public int QtyMonthN3 { get; set; } = 0;            // Số lượng dự kiến tháng N+3
    public int ProducedQty { get; set; } = 0;           // Số lượng xe thực tế đã hoàn thành xuất xưởng KCS
    public DateTime? ETADate { get; set; }              // Ngày dự kiến hoàn thành xuất xưởng dòng xe
    public string Stage { get; set; } = "Stamping";     // Công đoạn sản xuất hiện tại: Stamping (Dập thân vỏ), Body (Hàn khung xe), Paint (Sơn nhúng & tĩnh điện), Assembly (Lắp ráp hoàn thiện), FinalQC (Kiểm định KCS cuối dây chuyền), Finished (Đã xuất xưởng)
    public string Status { get; set; } = "Pending";     // Pending → Scheduled → InProduction → Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Mốc lịch sử vòng đời xe (audit) — thay cho việc dò log rời.</summary>
public sealed class VehicleEvent
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Vin { get; set; } = "";
    public string Kind { get; set; } = "";          // Created/Allocated/DeliveryOrder/Delivered/Recall/Manufactured...
    public string? Note { get; set; }
    public DateTime At { get; set; } = DateTime.Now;
}

// ===== DTOs cho Lệnh sản xuất & Kế hoạch sản xuất nhà máy OEM (MnfPl_Order / ProductionOrder) =====

public sealed record CreateProductionOrderDto(
    string? OrderNo,
    string? OrderNoUser,
    string OrdMonth,
    string? OrdType,
    string? OrdCategoryType,
    string? PlantCode,
    string? PlantName,
    DateTime? EstimatedCompletionDate,
    string? Remark,
    string? CreatedBy,
    List<ProductionOrderItemInputDto>? Items
);

public sealed record ProductionOrderItemInputDto(
    string Model,
    string SpecCode,
    string? SpecDescription,
    string ColorCode,
    string? ColorName,
    int PlanQty,
    int? QtyMonthN1,
    int? QtyMonthN2,
    int? QtyMonthN3,
    DateTime? ETADate,
    string? Stage,
    string? Remark
);

public sealed record UpdateProductionOrderDto(
    string? OrderNoUser,
    string? OrdMonth,
    string? OrdType,
    string? OrdCategoryType,
    string? PlantCode,
    string? PlantName,
    DateTime? EstimatedCompletionDate,
    string? Remark
);

public sealed record ProductionOrderTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record ProduceVinDto(
    string? Vin,
    string? EngineNo,
    int? ModelYear,
    string? StorageCode,
    string? Remark,
    string? OperatorName
);

public sealed record UpdateProductionOrderLineDto(
    string? Model,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    int? PlanQty,
    int? QtyMonthN1,
    int? QtyMonthN2,
    int? QtyMonthN3,
    DateTime? ETADate,
    string? Stage,
    string? Status,
    string? Remark
);

public sealed record ProductionSummaryDto(
    int TotalOrders,
    int TotalDraft,
    int TotalSubmitted,
    int TotalScheduled,
    int TotalInProduction,
    int TotalCompleted,
    int TotalCancelled,
    int TotalPlanQty,
    int TotalProducedQty,
    decimal CompletionRate,
    List<ProductionModelStatsDto> ByModel,
    List<ProductionPlantStatsDto> ByPlant
);

public sealed record ProductionModelStatsDto(string Model, int PlanQty, int ProducedQty, decimal Rate);
public sealed record ProductionPlantStatsDto(string PlantCode, string PlantName, int OrderCount, int PlanQty, int ProducedQty);

public sealed record VehicleProductionInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    int? ModelYear,
    string? StorageCode,
    string? LastWorkOrderNo,
    DateTime? ManufacturedDate,
    string? PlantCode,
    ProductionOrder? ProductionOrder,
    ProductionOrderLine? ProductionOrderLine
);

/// <summary>Hóa đơn chiếu lệ / Báo giá đơn hàng bán buôn xe đại lý &amp; Nhập khẩu ô tô (BizHTC.Order.PerformanceInvoice / Ord_PI / Ord_PerformanceInvoice / ProformaInvoice): chứng từ xác nhận đặt xe chính thức do hãng OEM ban hành cho Đại lý hoặc Nhà máy sản xuất quốc tế, làm căn cứ mở Thư tín dụng LC, lập Hợp đồng mua bán xe và kế hoạch sản xuất.</summary>
public sealed class ProformaInvoice
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string RefNo { get; set; } = "";                 // Mã số PI (PI-2026-03-0001, PI-HMC-2026-...)
    public string? RefNoUser { get; set; }                // Mã số PI tham chiếu nội bộ / do người dùng nhập
    public string DealerCode { get; set; } = "";          // Mã đại lý thụ hưởng / đối tác nhận PI
    public string? DealerName { get; set; }               // Tên đại lý
    public string OrderMonth { get; set; } = "";          // Tháng đặt hàng kế hoạch (YYYY-MM)
    public string? ProductionMonth { get; set; }          // Tháng sản xuất dự kiến (YYYY-MM)
    public string? ExpectedDeliveryMonth { get; set; }    // Tháng dự kiến giao xe (YYYY-MM)
    public string Currency { get; set; } = "USD";         // Đồng tiền giao dịch (USD, EUR, VND)
    public decimal ExchangeRate { get; set; } = 25450m;   // Tỷ giá quy đổi ngoại tệ sang VNĐ
    public int TotalQuantity { get; set; } = 0;           // Tổng số lượng xe trong PI
    public decimal TotalAmountForeign { get; set; } = 0;  // Tổng giá trị ngoại tệ USD
    public decimal TotalAmount { get; set; } = 0;         // Tổng trị giá quy đổi sang VNĐ = TotalAmountForeign * ExchangeRate (hoặc tổng giá VNĐ)
    public decimal DepositRate { get; set; } = 10m;       // Tỷ lệ tiền đặt cọc theo PI (%) (VD: 10% = 10)
    public decimal DepositAmount { get; set; } = 0;       // Tiền đặt cọc theo PI (VNĐ) = TotalAmount * DepositRate / 100
    public string PaymentTerm { get; set; } = "LC";       // Điều kiện thanh toán (LC, TT, BankGuarantee, Cash, Clearing)
    public string? DeparturePort { get; set; } = "BUSAN"; // Cảng bốc hàng / xuất xưởng (BUSAN, ULSAN, CHENNAI, HTMV_FACTORY...)
    public string? ArrivalPort { get; set; } = "CANG_HAI_PHONG"; // Cảng dỡ hàng / Điểm nhận xe (CANG_HAI_PHONG, CANG_CAT_LAI, SHOWROOM_HN01...)
    public string? LCTemp { get; set; }                   // Mã L/C tạm / Thư tín dụng dự kiến (CT_LC)
    public string? LCNo { get; set; }                     // Mã L/C chính thức sau khi mở
    public string? ContractNo { get; set; }               // Mã hợp đồng mua bán ngoại thương hoặc bán buôn liên quan
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved → InExecution → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú điều khoản / điều kiện giao nhận
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Lãnh đạo kinh doanh / Kế hoạch bán hàng OEM phê duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? ExecutedBy { get; set; }               // Chuyên viên điều vận / XNK đưa vào thực thi sản xuất & mở LC
    public DateTime? ExecutedAt { get; set; }
    public string? CompletedBy { get; set; }              // Người xác nhận hoàn tất PI
    public DateTime? CompletedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng xe trong Hóa đơn chiếu lệ Proforma Invoice (BizHTC.Order.PerformanceInvoice / Ord_PIDetail / Ord_PerformanceInvoiceDetail / ProformaInvoiceLine): model xe, phiên bản, màu sơn, lệnh sản xuất, nhà máy, cảng xuất/nhập, số lượng đặt, đơn giá ngoại tệ USD và quy đổi VNĐ.</summary>
public sealed class ProformaInvoiceLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ProformaInvoiceId { get; set; }
    public string RefNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng trong PI
    public string? Vin { get; set; }                      // Số khung VIN cụ thể nếu đã phân bổ / chỉ định
    public string Model { get; set; } = "";               // Dòng xe (SantaFe, Tucson, Accent, Creta, Grand i10, Custin, Palisade, Ioniq 5...)
    public string SpecCode { get; set; } = "";           // Mã phiên bản xe (2.5T AWD Calligraphy, 2.0 AT Đặc Biệt, 1.5 AT Tiêu Chuẩn...)
    public string? SpecDescription { get; set; }         // Mô tả cấu hình xe
    public string ColorCode { get; set; } = "NWAC";      // Mã màu sơn xe (NWAC, SAW, R2P, T2X...)
    public string? ColorName { get; set; }               // Tên màu sắc (Trắng Ngọc Trai, Đen Phantom, Đỏ Đô...)
    public string? WorkOrderNo { get; set; }             // Lệnh sản xuất liên kết (PO-2026-03-0001...)
    public string? PlantCode { get; set; }               // Nhà máy sản xuất (HTMV_NINHBINH_1, HTMV_NINHBINH_2, ULSAN_PLANT...)
    public string? PortCode { get; set; }                // Mã cảng xuất / dỡ hàng
    public string? LCTemp { get; set; }                  // Mã LC dự kiến
    public string? ContractNo { get; set; }              // Mã hợp đồng liên kết
    public int OrderQty { get; set; } = 1;               // Số lượng xe đặt mua dòng này
    public int AllocatedQty { get; set; } = 0;           // Số lượng xe đã thực tế phân bổ VIN
    public decimal UnitPriceForeign { get; set; } = 0;   // Đơn giá ngoại tệ (USD)
    public decimal TotalAmountForeign { get; set; } = 0; // Thành tiền ngoại tệ (USD) = OrderQty * UnitPriceForeign
    public decimal UnitPrice { get; set; } = 0;          // Đơn giá quy đổi VNĐ = UnitPriceForeign * ExchangeRate (hoặc đơn giá xuất buôn VNĐ)
    public decimal TotalAmount { get; set; } = 0;        // Thành tiền quy đổi VNĐ = OrderQty * UnitPrice
    public string Status { get; set; } = "Pending";      // Pending → Approved → Allocated → Delivered (hoặc Cancelled)
    public string? Remark { get; set; }
}

// ===== DTOs cho Hóa đơn chiếu lệ Proforma Invoice (BizHTC.Order.PerformanceInvoice / Ord_PI / ProformaInvoice) =====

public sealed record CreateProformaInvoiceDto(
    string? RefNo,
    string? RefNoUser,
    string DealerCode,
    string? DealerName,
    string OrderMonth,
    string? ProductionMonth,
    string? ExpectedDeliveryMonth,
    string? Currency,
    decimal? ExchangeRate,
    decimal? DepositRate,
    string? PaymentTerm,
    string? DeparturePort,
    string? ArrivalPort,
    string? LCTemp,
    string? LCNo,
    string? ContractNo,
    string? Remark,
    string? CreatedBy,
    List<ProformaInvoiceItemInputDto>? Items
);

public sealed record ProformaInvoiceItemInputDto(
    string Model,
    string SpecCode,
    string? SpecDescription,
    string ColorCode,
    string? ColorName,
    string? WorkOrderNo,
    string? PlantCode,
    string? PortCode,
    string? LCTemp,
    string? ContractNo,
    int OrderQty,
    decimal? UnitPriceForeign,
    decimal? UnitPrice,
    string? Vin,
    string? Remark
);

public sealed record UpdateProformaInvoiceHeaderDto(
    string? RefNoUser,
    string? DealerCode,
    string? DealerName,
    string? OrderMonth,
    string? ProductionMonth,
    string? ExpectedDeliveryMonth,
    string? Currency,
    decimal? ExchangeRate,
    decimal? DepositRate,
    string? PaymentTerm,
    string? DeparturePort,
    string? ArrivalPort,
    string? LCTemp,
    string? LCNo,
    string? ContractNo,
    string? Remark
);

public sealed record ProformaInvoiceTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? RefNoUser,
    string? LCNo,
    string? ContractNo
);

public sealed record AllocateVinToPiLineDto(
    string Vin,
    string? Remark,
    string? Actor
);

public sealed record UpdateProformaInvoiceLineDto(
    string? Model,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    string? WorkOrderNo,
    string? PlantCode,
    string? PortCode,
    string? LCTemp,
    string? ContractNo,
    int? OrderQty,
    int? AllocatedQty,
    decimal? UnitPriceForeign,
    decimal? UnitPrice,
    string? Status,
    string? Vin,
    string? Remark
);

public sealed record ProformaInvoiceSummaryDto(
    int TotalInvoices,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalInExecution,
    int TotalCompleted,
    int TotalCancelled,
    int TotalOrderQty,
    int TotalAllocatedQty,
    decimal TotalAmountForeign,
    decimal TotalAmount,
    decimal ExecutionRate,
    List<ProformaInvoiceModelStatsDto> ByModel,
    List<ProformaInvoiceDealerStatsDto> ByDealer,
    List<ProformaInvoiceMonthStatsDto> ByOrderMonth
);

public sealed record ProformaInvoiceModelStatsDto(string Model, int InvoiceCount, int OrderQty, int AllocatedQty, decimal TotalAmount);
public sealed record ProformaInvoiceDealerStatsDto(string DealerCode, string DealerName, int InvoiceCount, int TotalQty, decimal TotalAmount);
public sealed record ProformaInvoiceMonthStatsDto(string OrderMonth, int InvoiceCount, int TotalQty, decimal TotalAmount);

public sealed record VehiclePiInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    int? ModelYear,
    string? LastPiNo,
    DateTime? LastPiDate,
    int PiCount,
    ProformaInvoice? ProformaInvoice,
    ProformaInvoiceLine? ProformaInvoiceLine
);

/// <summary>Bảng kê & Quyết toán chi phí kiểm tra xe PDI cho Đại lý & Kho bãi (BizHTC.Payment.Pmt_PaymentPDI / PdiPayment): OEM thanh toán tiền công/chi phí kiểm tra xe PDI nhập bãi (PDIN / CostInCheck) và PDI xuất bãi/giao xe (PDIX / CostOutCheck) cho đại lý hoặc đơn vị vận hành bãi kho.</summary>
public sealed class PdiPayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PmtPdiNo { get; set; } = "";             // Mã bảng kê quyết toán PDI (PDI-PAY-2026-03-0001, PDI202603-HN01-01...)
    public string? PmtPdiNoUser { get; set; }            // Mã số bảng kê nội bộ tham chiếu
    public string DealerCode { get; set; } = "";          // Mã đại lý thụ hưởng chi phí PDI
    public string? DealerName { get; set; }               // Tên đại lý
    public string? StorageCode { get; set; }              // Mã kho bãi kiểm tra (nếu có)
    public string PeriodMonth { get; set; } = "";         // Kỳ quyết toán (YYYY-MM, ví dụ: 2026-03)
    public DateTime PaymentDate { get; set; } = DateTime.Now; // Ngày lập bảng kê quyết toán
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng xe quyết toán PDI trong kỳ
    public decimal TotalCostIn { get; set; } = 0;         // Tổng chi phí PDI nhập bãi/kho trước thuế (VNĐ)
    public decimal TotalCostOut { get; set; } = 0;        // Tổng chi phí PDI xuất bãi/giao xe trước thuế (VNĐ)
    public decimal TotalAmount { get; set; } = 0;         // Tổng chi phí PDI trước thuế = TotalCostIn + TotalCostOut (VNĐ)
    public decimal VatRate { get; set; } = 10m;           // Thuế suất GTGT VAT (%)
    public decimal VatAmount { get; set; } = 0;           // Tiền thuế VAT (VNĐ) = TotalAmount * VatRate / 100
    public decimal TotalAmountAfterVAT { get; set; } = 0; // Tổng tiền thanh toán sau thuế = TotalAmount + VatAmount (VNĐ)
    public string? FileSigned { get; set; }               // Tệp văn bản bảng kê quyết toán PDI ký số điện tử
    public string? BankRefNo { get; set; }                // Mã giao dịch / Ủy nhiệm chi ngân hàng chuyển khoản thanh toán
    public DateTime? SettledDate { get; set; }            // Ngày thực tế thanh toán / bù trừ công nợ tiền PDI
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved1 → Approved2 → TCMSSigned → HTVSigned (Settled) (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú giải trình bảng kê
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Approved1By { get; set; }              // Kỹ thuật/Dịch vụ xưởng OEM duyệt danh sách xe đủ tiêu chuẩn
    public DateTime? Approved1At { get; set; }
    public string? Approved2By { get; set; }              // Kế toán/Tài chính OEM duyệt chi phí
    public DateTime? Approved2At { get; set; }
    public string? TCMSSignedBy { get; set; }             // Lãnh đạo Khối Phân phối OEM ký số
    public DateTime? TCMSSignedAt { get; set; }
    public string? HTVSignedBy { get; set; }              // Lãnh đạo Nhà máy HTV ký số
    public DateTime? HTVSignedAt { get; set; }
    public string? SettledBy { get; set; }                // Kế toán thanh toán xác nhận đã chi tiền
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong bảng kê quyết toán chi phí PDI (BizHTC.Payment.Pmt_PaymentPDIDetail / PdiPaymentLine): danh sách VIN, chi phí PDI nhập bãi (CostInCheck), chi phí PDI xuất bãi (CostOutCheck), tổng chi phí và kết quả kiểm tra PDI.</summary>
public sealed class PdiPaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long PdiPaymentId { get; set; }
    public string PmtPdiNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng trong bảng kê
    public string Vin { get; set; } = "";                 // Số khung VIN
    public string Model { get; set; } = "";               // Dòng xe (Accent, Creta, Tucson, SantaFe...)
    public string? SpecCode { get; set; }                 // Phiên bản xe
    public string? EngineNo { get; set; }                 // Số máy
    public string? Color { get; set; }                    // Màu sắc
    public string? StorageCode { get; set; }              // Kho bãi kiểm tra
    public string? DealerCode { get; set; }               // Đại lý thực hiện PDI
    public string? PdiReqNo { get; set; }                 // Mã yêu cầu PDI gốc liên kết (Dlr_PDIRequest)
    public string? DlvMnNo { get; set; }                  // Mã biên bản giao xe liên kết (Sto_DlvMinutes)
    public decimal CostInCheck { get; set; } = 0;         // Chi phí PDI nhập bãi / kho (VNĐ)
    public decimal CostOutCheck { get; set; } = 0;        // Chi phí PDI xuất bãi / giao xe (VNĐ)
    public decimal TotalCostCheck { get; set; } = 0;      // Tổng chi phí kiểm tra xe = CostInCheck + CostOutCheck (VNĐ)
    public DateTime? PdiCompletedDate { get; set; }       // Ngày nghiệm thu hoàn thành PDI
    public string PdiResult { get; set; } = "Passed";     // Kết quả PDI: Passed, Approved, DefectResolved
    public string Status { get; set; } = "Pending";       // Pending → Submitted → Approved1 → Approved2 → TCMSSigned → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

// ===== DTOs cho Bảng kê & Quyết toán chi phí kiểm tra PDI (BizHTC.Payment.Pmt_PaymentPDI / PdiPayment) =====

public sealed record CreatePdiPaymentDto(
    string? PmtPdiNo,
    string? PmtPdiNoUser,
    string DealerCode,
    string? DealerName,
    string? StorageCode,
    string PeriodMonth,
    DateTime? PaymentDate,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<PdiPaymentItemInputDto>? Items,
    List<string>? Vins
);

public sealed record PdiPaymentItemInputDto(
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? PdiReqNo,
    string? DlvMnNo,
    decimal? CostInCheck,
    decimal? CostOutCheck,
    DateTime? PdiCompletedDate,
    string? PdiResult,
    string? Remark
);

public sealed record UpdatePdiPaymentHeaderDto(
    string? PmtPdiNoUser,
    string? DealerCode,
    string? DealerName,
    string? StorageCode,
    string? PeriodMonth,
    DateTime? PaymentDate,
    decimal? VatRate,
    string? BankRefNo,
    DateTime? SettledDate,
    string? FileSigned,
    string? Remark
);

public sealed record PdiPaymentTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    string? FileSigned
);

public sealed record UpdatePdiPaymentLineDto(
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? PdiReqNo,
    string? DlvMnNo,
    decimal? CostInCheck,
    decimal? CostOutCheck,
    DateTime? PdiCompletedDate,
    string? PdiResult,
    string? Status,
    string? Remark
);

public sealed record PdiPaymentSummaryDto(
    int TotalPayments,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved1,
    int TotalApproved2,
    int TotalTCMSSigned,
    int TotalSettled,
    int TotalCancelled,
    int TotalVehicleCount,
    decimal TotalCostIn,
    decimal TotalCostOut,
    decimal TotalAmount,
    decimal TotalVatAmount,
    decimal TotalAmountAfterVAT,
    decimal SettlementRatePercent,
    List<PdiPaymentDealerStatsDto> ByDealer,
    List<PdiPaymentStorageStatsDto> ByStorage,
    List<PdiPaymentMonthStatsDto> ByPeriodMonth
);

public sealed record PdiPaymentDealerStatsDto(string DealerCode, string DealerName, int PaymentCount, int TotalVehicles, decimal TotalAmount, decimal SettledAmount);
public sealed record PdiPaymentStorageStatsDto(string StorageCode, int PaymentCount, int TotalVehicles, decimal TotalAmount);
public sealed record PdiPaymentMonthStatsDto(string PeriodMonth, int PaymentCount, int TotalVehicles, decimal TotalAmount);

public sealed record VehiclePdiPaymentInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    int? ModelYear,
    bool IsPdiPaid,
    decimal PdiPaidAmount,
    string? LastPdiPaymentNo,
    DateTime? LastPdiPaymentDate,
    int PdiPaymentCount,
    PdiPayment? PdiPayment,
    PdiPaymentLine? PdiPaymentLine
);

/// <summary>Chính sách giá &amp; Hỗ trợ bán hàng / Kích cầu bán lẻ xe ô tô cho Đại lý (BizHTC.DealerSales / SPL_SalesPolicyMst &amp; SPL_SalesPolicyMstDetail): Hãng OEM ban hành văn bản chính sách hỗ trợ giá theo từng dòng xe, phiên bản và thời gian áp dụng.</summary>
public sealed class SalesPolicy
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SPSRCode { get; set; } = "";             // Mã chính sách hỗ trợ bán hàng (SPL-2026-03-01, SPSR...)
    public string SPNo { get; set; } = "";                 // Số hiệu văn bản ban hành chính thức (CV-2026/HTV-SALES-01)
    public string? SPSRType { get; set; } = "RetailSupport"; // Loại chính sách: RetailSupport (Hỗ trợ bán lẻ), TradeDiscount (Chiết khấu thương mại), CampaignPromotion (Khuyến mại tháng), InterestSubsidy (Hỗ trợ lãi suất), RegistrationSupport (Hỗ trợ trước bạ)
    public string? SPSRRoot { get; set; }                  // Mã chính sách cha / gốc (nếu là chính sách bổ sung/gia hạn)
    public string? FormBusinessSupportCode { get; set; } = "DirectCash"; // Hình thức hỗ trợ: DirectCash (Tiền mặt), InvoiceDeduction (Giảm trừ hóa đơn), GiftVoucher (Phiếu quà tặng), FuelVoucher (Hỗ trợ nhiên liệu)
    public DateTime StartDate { get; set; } = DateTime.Now; // Ngày bắt đầu áp dụng chính sách
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30); // Ngày kết thúc chính sách
    public int TotalModelsCount { get; set; } = 0;         // Tổng số lượng model/phiên bản áp dụng
    public decimal TotalSupportBudget { get; set; } = 0;   // Tổng ngân sách dự toán hỗ trợ của chính sách (VNĐ)
    public int TotalVinApplied { get; set; } = 0;          // Tổng số lượng xe VIN đã gán hỗ trợ
    public decimal TotalActualPaidAmount { get; set; } = 0; // Tổng số tiền thực tế đã quyết toán chi trả (VNĐ)
    public string? FilePath { get; set; }                  // Văn bản quyết định ban hành chính sách có ký số (PDF)
    public string Status { get; set; } = "Draft";          // Draft → Active → Expired (hoặc Suspended / Cancelled)
    public string? Remark { get; set; }                    // Diễn giải / điều kiện chi tiết của chính sách
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo Khối Bán hàng / Ban Giám Đốc phê duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? SuspendedBy { get; set; }               // Người tạm dừng áp dụng chính sách
    public DateTime? SuspendedAt { get; set; }
    public string? CancelledBy { get; set; }               // Người hủy chính sách
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng xe áp dụng trong Chính sách hỗ trợ bán hàng (BizHTC.DealerSales / SPL_SalesPolicyMstDetail / SalesPolicyLine): model xe, phiên bản spec, đại lý chỉ định, năm sản xuất và mức tiền hỗ trợ.</summary>
public sealed class SalesPolicyLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesPolicyId { get; set; }
    public string SPSRCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;                // Thứ tự dòng trong chính sách
    public string Model { get; set; } = "";                // Dòng xe áp dụng (Accent, Creta, Tucson, SantaFe, Grand i10, Custin, Palisade...)
    public string SpecCode { get; set; } = "";            // Mã phiên bản (1.4 AT Đặc biệt, 1.5 Cao cấp, 2.0 AT, 2.5T...)
    public string? SpecDescription { get; set; }          // Mô tả chi tiết cấu hình xe
    public string? DealerCode { get; set; }                // Áp dụng riêng cho 1 đại lý cụ thể (nếu rỗng/null = áp dụng Toàn quốc)
    public int? ModelYear { get; set; } = 2026;            // Năm sản xuất áp dụng
    public decimal AmountSupport { get; set; } = 0;        // Mức tiền hỗ trợ cho mỗi xe VIN bán ra (VNĐ)
    public string Status { get; set; } = "Active";         // Active, Inactive
    public string? Remark { get; set; }
}

/// <summary>Gán &amp; Quyết toán Hỗ trợ bán lẻ xe ô tô theo số khung VIN (BizHTC.DealerSales / SPL_SPSupportRetail / SalesPolicySupport): ghi nhận xe bán ra đủ điều kiện hưởng chính sách, lưu thông tin hóa đơn Hãng và ngày Hãng quyết toán chi trả cho Đại lý.</summary>
public sealed class SalesPolicySupport
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SupportNo { get; set; } = "";             // Mã số phiếu hỗ trợ VIN (SPSR-2026-03-0001, SPSR...)
    public string SPSRCode { get; set; } = "";             // Mã chính sách hỗ trợ áp dụng (SPL-2026-03-01)
    public string? SPNo { get; set; }                      // Số hiệu văn bản chính sách liên kết
    public string Vin { get; set; } = "";                  // Số khung xe VIN được hưởng hỗ trợ
    public string DealerCode { get; set; } = "";           // Mã đại lý bán lẻ được nhận hỗ trợ
    public string? DealerName { get; set; }                // Tên đại lý
    public string Model { get; set; } = "";                // Dòng xe
    public string? SpecCode { get; set; }                  // Mã phiên bản xe
    public string? EngineNo { get; set; }                  // Số máy
    public string? Color { get; set; }                     // Màu sắc
    public DateTime DateSupport { get; set; } = DateTime.Now; // Ngày bán xe / Ngày phát sinh đề nghị hỗ trợ
    public DateTime? DateFullStatus { get; set; }          // Ngày xe đạt đủ điều kiện nhận hỗ trợ (sau khi xuất hóa đơn HTC và bàn giao xe)
    public decimal AmountSupport { get; set; } = 0;        // Số tiền hỗ trợ được hưởng cho xe này (VNĐ)
    public string? HTCInvoiceNo { get; set; }              // Số hóa đơn GTGT của Hãng cho xe này (HD26-...)
    public DateTime? HTCInvoiceDate { get; set; }          // Ngày hóa đơn của Hãng
    public DateTime? HTCDatePayment { get; set; }          // Ngày Hãng hoàn tất chi trả / bù trừ công nợ hỗ trợ cho Đại lý
    public string? BankRefNo { get; set; }                 // Mã ủy nhiệm chi / giao dịch ngân hàng thanh toán
    public string Status { get; set; } = "Draft";          // Draft → Submitted → Approved → Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                    // Ghi chú hồ sơ hỗ trợ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                // Lãnh đạo bán hàng OEM duyệt hỗ trợ
    public DateTime? ApprovedAt { get; set; }
    public string? SettledBy { get; set; }                 // Kế toán thanh toán OEM xác nhận chi trả
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

// ===== DTOs cho Chính sách hỗ trợ bán hàng & Gán hỗ trợ theo VIN (BizHTC.DealerSales / SPL_SalesPolicyMst & SPL_SPSupportRetail) =====

public sealed record CreateSalesPolicyDto(
    string? SPSRCode,
    string SPNo,
    string? SPSRType,
    string? SPSRRoot,
    string? FormBusinessSupportCode,
    DateTime StartDate,
    DateTime EndDate,
    decimal? TotalSupportBudget,
    string? FilePath,
    string? Remark,
    string? CreatedBy,
    List<SalesPolicyLineInputDto>? Lines
);

public sealed record SalesPolicyLineInputDto(
    string Model,
    string SpecCode,
    string? SpecDescription,
    string? DealerCode,
    int? ModelYear,
    decimal AmountSupport,
    string? Remark
);

public sealed record UpdateSalesPolicyHeaderDto(
    string? SPNo,
    string? SPSRType,
    string? SPSRRoot,
    string? FormBusinessSupportCode,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal? TotalSupportBudget,
    string? FilePath,
    string? Remark
);

public sealed record SalesPolicyTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record UpdateSalesPolicyLineDto(
    string? Model,
    string? SpecCode,
    string? SpecDescription,
    string? DealerCode,
    int? ModelYear,
    decimal? AmountSupport,
    string? Status,
    string? Remark
);

public sealed record CreateSalesPolicySupportDto(
    string? SupportNo,
    string SPSRCode,
    string Vin,
    string? DealerCode,
    string? DealerName,
    DateTime? DateSupport,
    DateTime? DateFullStatus,
    decimal? AmountSupport,
    string? HTCInvoiceNo,
    DateTime? HTCInvoiceDate,
    string? Remark,
    string? CreatedBy
);

public sealed record BatchAssignPolicySupportDto(
    string SPSRCode,
    List<string> Vins,
    string? DealerCode,
    DateTime? DateSupport,
    DateTime? DateFullStatus,
    string? Remark,
    string? CreatedBy
);

public sealed record UpdateSalesPolicySupportDto(
    string? DealerCode,
    string? DealerName,
    DateTime? DateSupport,
    DateTime? DateFullStatus,
    decimal? AmountSupport,
    string? HTCInvoiceNo,
    DateTime? HTCInvoiceDate,
    string? BankRefNo,
    string? Remark
);

public sealed record SalesPolicySupportTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    DateTime? HTCDatePayment
);

public sealed record SalesPolicySummaryDto(
    int TotalPolicies,
    int TotalActivePolicies,
    int TotalDraftPolicies,
    int TotalExpiredPolicies,
    int TotalSupports,
    int TotalDraftSupports,
    int TotalSubmittedSupports,
    int TotalApprovedSupports,
    int TotalSettledSupports,
    int TotalCancelledSupports,
    decimal TotalBudgetAmount,
    decimal TotalApprovedAmount,
    decimal TotalSettledAmount,
    decimal SettlementRatePercent,
    List<SalesPolicyModelStatsDto> ByModel,
    List<SalesPolicyDealerStatsDto> ByDealer
);

public sealed record SalesPolicyModelStatsDto(string Model, int SupportCount, decimal TotalAmount, decimal SettledAmount);
public sealed record SalesPolicyDealerStatsDto(string DealerCode, string DealerName, int SupportCount, decimal TotalAmount, decimal SettledAmount);

public sealed record VehiclePolicySupportInfoDto(
    string Vin,
    string Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    int? ModelYear,
    bool IsPolicySupported,
    decimal PolicySupportAmount,
    string? LastPolicyCode,
    DateTime? LastPolicyDate,
    int PolicySupportCount,
    List<SalesPolicySupport> Supports
);

/// <summary>Thiết bị định vị GPS giám sát vị trí xe tồn kho &amp; vận chuyển (BizHTC.StorageFG.Sto_StoBalanceGPS / GpsDevice): quản lý danh mục thiết bị định vị, số SIM, IMEI, nhà cung cấp, dung lượng pin và tọa độ thời gian thực.</summary>
public sealed class GpsDevice
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GpsCode { get; set; } = "";             // Mã định danh thiết bị GPS (GPSDvNo: GPS-2026-0001, DEV-...)
    public string? GpsBoxNo { get; set; }                 // Mã lô / thùng thiết bị (GPSBoxNo)
    public string? SerialNo { get; set; }                 // Số sê-ri nhà sản xuất
    public string? ImeiNo { get; set; }                   // Số IMEI viễn thông
    public string? SimNo { get; set; }                    // Số thuê bao SIM 4G
    public string Provider { get; set; } = "Viettel";     // Nhà mạng / đối tác cung cấp: Viettel, Veloca, VNPT, MobiFone
    public string ModelName { get; set; } = "OBD-4G";     // Chủng loại: OBD-4G, VT-03D, GT06N, AT4, Solar-GPS
    public string StorageCodeGps { get; set; } = "KHO_GPS_NINHBINH"; // Kho lưu thiết bị: KHO_GPS_NINHBINH, KHO_GPS_HN, KHO_GPS_HCM
    public string DeviceStatus { get; set; } = "InStock"; // InStock (Trong kho sẵn sàng lắp), Installed (Đang gắn trên xe), InTransit (Đang vận chuyển), ClaimFaulty (Đang bảo hành hỏng lỗi), Decommissioned (Đã thanh lý)
    public decimal BatteryVolt { get; set; } = 12.6m;     // Điện áp pin (V)
    public int BatteryPercent { get; set; } = 100;        // % dung lượng pin (0 - 100%)
    public string? CurrentVin { get; set; }               // Số khung xe VIN đang gắn thiết bị
    public string? CurrentModel { get; set; }             // Dòng xe đang gắn
    public string? CurrentLocation { get; set; }          // Vị trí / địa chỉ hiện tại
    public decimal? Latitude { get; set; }                // Vĩ độ GPS hiện tại
    public decimal? Longitude { get; set; }               // Kinh độ GPS hiện tại
    public decimal SpeedKmH { get; set; } = 0;            // Vận tốc di chuyển hiện tại (km/h)
    public bool IsInGeofence { get; set; } = true;        // Nằm trong bãi đỗ / lộ trình cho phép (Geofence)
    public DateTime? LastSignalAt { get; set; }           // Thời điểm nhận tín hiệu định vị cuối cùng
    public string? LastGpsInNo { get; set; }              // Mã phiếu lắp đặt gần nhất (StoF_GPSIn)
    public string? LastGpsOutNo { get; set; }             // Mã phiếu tháo gỡ gần nhất (StoF_GPSOut)
    public string? LastClaimNo { get; set; }              // Mã phiếu claim bảo hành gần nhất (GPSF_GPSClaim)
    public string? Remark { get; set; }                   // Ghi chú thiết bị
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>Lệnh Lắp đặt &amp; Gắn thiết bị định vị GPS vào xe ô tô VIN (BizHTC.StorageFG.StoF_GPSIn / GpsInstallation): quy trình xuất kho và kích hoạt gắn thiết bị định vị GPS lên xe tồn bãi nhà máy OEM hoặc xe chuẩn bị vận chuyển.</summary>
public sealed class GpsInstallation
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GpsInNo { get; set; } = "";             // Mã phiếu lắp đặt GPS (GPSIN-2026-03-0001, SF_GPSInNo)
    public string? GpsInNoUser { get; set; }            // Mã số phiếu tham chiếu nội bộ
    public string GpsInType { get; set; } = "First_In";   // Loại lắp đặt: First_In (Lắp mới xe xuất xưởng), Re_In (Lắp lại xe tồn kho), Replacement (Lắp thay thế lỗi)
    public string StorageCodeGps { get; set; } = "KHO_GPS_NINHBINH"; // Kho xuất thiết bị GPS
    public DateTime InstallationDate { get; set; } = DateTime.Now; // Ngày thực hiện lắp đặt
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng xe lắp đặt trong đợt
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved / Installed (hoặc Cancelled)
    public string? Remark { get; set; }                   // Ghi chú lệnh lắp đặt
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Quản đốc kỹ thuật / Trưởng kho duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe &amp; thiết bị trong Phiếu lắp đặt GPS (BizHTC.StorageFG.StoF_GPSInDtl / GpsInstallationLine): số khung VIN, mã thiết bị GPS, thông tin SIM/IMEI, người lắp và kiểm tra tín hiệu ban đầu.</summary>
public sealed class GpsInstallationLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long GpsInstallationId { get; set; }
    public string GpsInNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng
    public string Vin { get; set; } = "";                 // Số khung VIN được gắn GPS
    public string Model { get; set; } = "";               // Dòng xe
    public string? EngineNo { get; set; }                 // Số máy
    public string? Color { get; set; }                    // Màu sơn
    public string? StorageCode { get; set; }              // Vị trí bãi ô đỗ của xe (YARD-A1...)
    public string GpsCode { get; set; } = "";             // Mã thiết bị GPS được lắp (GPSDvNo)
    public string? ImeiNo { get; set; }                   // IMEI chip
    public string? SimNo { get; set; }                    // Số SIM
    public decimal BatteryVolt { get; set; } = 12.6m;     // Điện áp pin lúc lắp (V)
    public string? Technician { get; set; }               // Kỹ thuật viên thực hiện lắp đặt
    public DateTime? InstalledAt { get; set; }            // Thời điểm hoàn tất lắp đặt
    public string InitialSignalStatus { get; set; } = "SignalOK"; // Trạng thái tín hiệu: SignalOK, GPSLocked, WeakSignal, Offline
    public string Status { get; set; } = "Pending";       // Pending → Approved / Installed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Lệnh Tháo gỡ &amp; Thu hồi thiết bị định vị GPS từ xe VIN (BizHTC.StorageFG.StoF_GPSOut / GpsUninstallation): quy trình tháo gỡ thiết bị GPS khi xuất kho bàn giao xe cho Đại lý/khách hàng hoặc bảo dưỡng xe.</summary>
public sealed class GpsUninstallation
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GpsOutNo { get; set; } = "";            // Mã phiếu tháo gỡ thiết bị (GPSOUT-2026-03-0001, SF_GPSOutNo)
    public string? GpsOutNoUser { get; set; }           // Mã số phiếu tham chiếu nội bộ
    public string Reason { get; set; } = "DeliveryToDealer"; // Lý do tháo gỡ: DeliveryToDealer (Giao xe đại lý), CustomerDelivery (Giao xe khách), FaultyReplacement (Tháo thiết bị hỏng), StorageMaintenance (Bảo dưỡng), Decommission (Thanh lý xe)
    public string StorageCodeGps { get; set; } = "KHO_GPS_NINHBINH"; // Kho tiếp nhận lại thiết bị thu hồi
    public string? ReceiverName { get; set; }             // Thủ kho / Cán bộ tiếp nhận thiết bị
    public DateTime UninstallDate { get; set; } = DateTime.Now; // Ngày tháo gỡ
    public int TotalVehicleCount { get; set; } = 0;       // Tổng số lượng thiết bị thu hồi trong đợt
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved / Completed (hoặc Cancelled)
    public string? Remark { get; set; }                   // Ghi chú lệnh tháo gỡ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Quản đốc kỹ thuật / Trưởng kho duyệt thu hồi
    public DateTime? ApprovedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết tháo gỡ thiết bị GPS từ xe VIN (BizHTC.StorageFG.StoF_GPSOutDtl / GpsUninstallationLine): số khung VIN, mã thiết bị, ODO lúc tháo và tình trạng kỹ thuật của thiết bị khi thu hồi.</summary>
public sealed class GpsUninstallationLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long GpsUninstallationId { get; set; }
    public string GpsOutNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng
    public string Vin { get; set; } = "";                 // Số khung VIN được tháo GPS
    public string Model { get; set; } = "";               // Dòng xe
    public string? GpsCode { get; set; }                  // Mã thiết bị GPS tháo ra
    public int? OdoKm { get; set; }                       // Số km ODO xe tại thời điểm tháo
    public string DeviceCondition { get; set; } = "Good"; // Tình trạng thiết bị: Good (Tốt - Tái sử dụng InStock), Faulty (Hỏng - Chuyển Claim bảo hành), LowBattery (Hết pin - Cần nạp điện)
    public string? Technician { get; set; }               // Kỹ thuật viên thực hiện tháo gỡ
    public DateTime? UninstalledAt { get; set; }          // Thời điểm hoàn tất tháo
    public string Status { get; set; } = "Pending";       // Pending → Approved / Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Phiếu Yêu cầu Bảo hành &amp; Sửa chữa Đổi trả Thiết bị GPS hỏng lỗi (BizHTC.StorageFG.GPSF_GPSClaim / GpsClaim): quy trình gửi thiết bị định vị hư hỏng sang đối tác nhà cung cấp bảo hành, sửa chữa hoặc đổi mới.</summary>
public sealed class GpsClaim
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GpsClaimNo { get; set; } = "";          // Mã phiếu claim bảo hành (CLM-GPS-2026-0001, GPSClaimNo)
    public string? GpsClaimNoUser { get; set; }         // Số tham chiếu nội bộ
    public string GpsCode { get; set; } = "";             // Mã thiết bị GPS bị hư hỏng (GPSDvNo)
    public string? ImeiNo { get; set; }                   // Số IMEI của thiết bị
    public string? SimNo { get; set; }                    // Số SIM của thiết bị
    public string VendorCode { get; set; } = "VELOCA";    // Nhà cung cấp thiết bị: VELOCA, VIETTEL_TELECOM, VNPT_TRACKING
    public string? VendorName { get; set; }               // Tên nhà cung cấp
    public string FaultType { get; set; } = "PowerLoss";  // Loại sự cố: PowerLoss (Mất nguồn), SimCardError (Lỗi SIM/mất sóng), GpsSignalLoss (Mất tín hiệu GPS), AntennaDefect (Hỏng ăng ten), PhysicalDamage (Vỡ vỏ/vào nước), BatteryFailure (Chai pin)
    public string? FaultDescription { get; set; }         // Mô tả chi tiết hiện tượng hư hỏng
    public string? Vin { get; set; }                      // Số khung VIN phát hiện lỗi (nếu có)
    public decimal RepairCost { get; set; } = 0;          // Chi phí sửa chữa phát sinh (VNĐ)
    public string? ReplacementGpsCode { get; set; }       // Mã thiết bị mới đổi trả (nếu đổi mới 1-1)
    public string Status { get; set; } = "Draft";         // Draft → Submitted → SentToVendor → Repaired / Replaced → Received / Settled (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú hồ sơ bảo hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? SubmittedBy { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? SentBy { get; set; }                   // Người gửi thiết bị sang NCC
    public DateTime? SentAt { get; set; }
    public string? RepairedBy { get; set; }               // Kỹ sư NCC xác nhận sửa xong/đổi mới
    public DateTime? RepairedAt { get; set; }
    public string? ReceivedBy { get; set; }               // Thủ kho OEM nhận lại thiết bị về kho
    public DateTime? ReceivedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Nhật ký Tọa độ &amp; Vị trí Định vị GPS của xe VIN / Thiết bị (GpsLocationLog): lưu vết lịch sử di chuyển, cảnh báo địa giới bãi xe và điện áp bình.</summary>
public sealed class GpsLocationLog
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string GpsCode { get; set; } = "";             // Mã thiết bị định vị
    public string? Vin { get; set; }                      // Số khung xe VIN (nếu đang gắn trên xe)
    public decimal Latitude { get; set; }                 // Vĩ độ GPS
    public decimal Longitude { get; set; }                // Kinh độ GPS
    public decimal SpeedKmH { get; set; } = 0;            // Vận tốc di chuyển (km/h)
    public decimal BatteryVolt { get; set; } = 12.6m;     // Điện áp ắc quy / pin (V)
    public string? EngineStatus { get; set; } = "Off";    // Trạng thái động cơ: On, Off, Idle
    public string? Address { get; set; }                  // Tên địa chỉ / vị trí bãi đỗ
    public bool IsInGeofence { get; set; } = true;        // Trong khu vực bãi đỗ hợp lệ
    public DateTime RecordedAt { get; set; } = DateTime.Now; // Thời điểm ghi nhận tọa độ
}

// ===== DTOs cho Quản lý Thiết bị Định vị GPS & Giám sát Vị trí Xe (BizHTC.StorageFG / Sto_StoBalanceGPS, StoF_GPSIn, StoF_GPSOut, GPSF_GPSClaim) =====

public sealed record RegisterGpsDeviceDto(
    string GpsCode,
    string? GpsBoxNo,
    string? SerialNo,
    string? ImeiNo,
    string? SimNo,
    string? Provider,
    string? ModelName,
    string? StorageCodeGps,
    decimal? BatteryVolt,
    int? BatteryPercent,
    string? Remark
);

public sealed record UpdateGpsDeviceDto(
    string? GpsBoxNo,
    string? SerialNo,
    string? ImeiNo,
    string? SimNo,
    string? Provider,
    string? ModelName,
    string? StorageCodeGps,
    string? DeviceStatus,
    decimal? BatteryVolt,
    int? BatteryPercent,
    string? Remark
);

public sealed record CreateGpsInstallationDto(
    string? GpsInNo,
    string? GpsInNoUser,
    string? GpsInType,
    string? StorageCodeGps,
    DateTime? InstallationDate,
    string? Remark,
    string? CreatedBy,
    List<GpsInstallationItemInputDto>? Items
);

public sealed record GpsInstallationItemInputDto(
    string Vin,
    string GpsCode,
    string? StorageCode,
    string? Technician,
    string? InitialSignalStatus,
    string? Remark
);

public sealed record UpdateGpsInstallationHeaderDto(
    string? GpsInNoUser,
    string? GpsInType,
    string? StorageCodeGps,
    DateTime? InstallationDate,
    string? Remark
);

public sealed record UpdateGpsInstallationLineDto(
    string? GpsCode,
    string? StorageCode,
    string? Technician,
    DateTime? InstalledAt,
    string? InitialSignalStatus,
    string? Status,
    string? Remark
);

public sealed record GpsInstallationTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record CreateGpsUninstallationDto(
    string? GpsOutNo,
    string? GpsOutNoUser,
    string? Reason,
    string? StorageCodeGps,
    string? ReceiverName,
    DateTime? UninstallDate,
    string? Remark,
    string? CreatedBy,
    List<GpsUninstallationItemInputDto>? Items
);

public sealed record GpsUninstallationItemInputDto(
    string Vin,
    string? GpsCode,
    int? OdoKm,
    string? DeviceCondition,
    string? Technician,
    string? Remark
);

public sealed record UpdateGpsUninstallationHeaderDto(
    string? GpsOutNoUser,
    string? Reason,
    string? StorageCodeGps,
    string? ReceiverName,
    DateTime? UninstallDate,
    string? Remark
);

public sealed record UpdateGpsUninstallationLineDto(
    string? GpsCode,
    int? OdoKm,
    string? DeviceCondition,
    string? Technician,
    DateTime? UninstalledAt,
    string? Status,
    string? Remark
);

public sealed record GpsUninstallationTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record CreateGpsClaimDto(
    string? GpsClaimNo,
    string? GpsClaimNoUser,
    string GpsCode,
    string? VendorCode,
    string? VendorName,
    string? FaultType,
    string? FaultDescription,
    string? Vin,
    decimal? RepairCost,
    string? ReplacementGpsCode,
    string? Remark,
    string? CreatedBy
);

public sealed record UpdateGpsClaimDto(
    string? GpsClaimNoUser,
    string? VendorCode,
    string? VendorName,
    string? FaultType,
    string? FaultDescription,
    string? Vin,
    decimal? RepairCost,
    string? ReplacementGpsCode,
    string? Remark
);

public sealed record GpsClaimTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    string? ReplacementGpsCode,
    decimal? RepairCost,
    DateTime? TransitionDate
);

public sealed record UpdateGpsLocationDto(
    decimal Latitude,
    decimal Longitude,
    decimal? SpeedKmH,
    decimal? BatteryVolt,
    string? EngineStatus,
    string? Address,
    bool? IsInGeofence,
    DateTime? RecordedAt
);

public sealed record GpsFleetSummaryDto(
    int TotalDevices,
    int TotalInStock,
    int TotalInstalled,
    int TotalInTransit,
    int TotalClaimFaulty,
    int TotalDecommissioned,
    int TotalVehiclesWithGps,
    int TotalActiveSignals24h,
    int TotalGeofenceAlerts,
    List<GpsProviderStatsDto> ByProvider,
    List<GpsStorageStatsDto> ByStorage
);

public sealed record GpsProviderStatsDto(string Provider, int TotalCount, int InstalledCount, int InStockCount);
public sealed record GpsStorageStatsDto(string StorageCodeGps, int TotalCount, int InStockCount);

public sealed record VehicleGpsInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    int? ModelYear,
    string? StorageCode,
    bool IsGpsInstalled,
    string? GpsCode,
    DateTime? GpsInstallDate,
    DateTime? GpsUninstallDate,
    decimal? LastGpsLatitude,
    decimal? LastGpsLongitude,
    string? LastGpsAddress,
    decimal? LastGpsSpeed,
    decimal? LastGpsBatteryVolt,
    DateTime? LastGpsSignalTime,
    int GpsDeviceCount,
    GpsDevice? GpsDevice
);

// ===== Quản lý Khoang sửa chữa xưởng dịch vụ / Cầu nâng & Bảng điều phối khoang xưởng (BizCarSv / Ser_Cavity & Ser_CavityDispatch / FrmCavityCreate, FrmCavitySearch, FrmShowCavityStatus) =====

/// <summary>Khoang sửa chữa / Cầu nâng xưởng dịch vụ xe đại lý (BizCarSv / Ser_Cavity / ServiceCavity): quản lý danh mục khoang cầu xưởng (SCC, Đồng sơn BP, Bảo dưỡng nhanh EM, Rửa xe, PDI), trạng thái sẵn sàng/đang chiếm dụng/bảo trì và thông tin xe đang thao tác trên cầu.</summary>
public sealed class ServiceCavity
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CavityNo { get; set; } = "";             // Mã định danh khoang / số hiệu cầu (BAY-01, BAY-02, EM-01, BP-01, PDI-01, WASH-01...)
    public string? CavityNoUser { get; set; }            // Mã số khoang tham chiếu nội bộ đại lý
    public string CavityName { get; set; } = "";           // Tên khoang (Khoang sửa chữa chung 01, Khoang bảo dưỡng nhanh EM, Buồng sơn sấy...)
    public string DealerCode { get; set; } = "";          // Mã đại lý sở hữu khoang/xưởng dịch vụ (DLR-HN01...)
    public string CavityType { get; set; } = "GeneralRepair"; // Loại khoang: GeneralRepair (Sửa chữa chung SCC), BodyPaint (Đồng sơn BP / Buồng sơn sấy), QuickService (Bảo dưỡng nhanh EM), Washing (Rửa xe & Car Care), PDIInspection (Kiểm định PDI), Parking (Khoang đỗ xe / chờ phụ tùng)
    public string Status { get; set; } = "Available";     // Trạng thái khoang: Available (Sẵn sàng/trống), Occupied (Đang có xe sửa chữa), Reserved (Đã đặt trước theo lịch hẹn), Maintenance (Đang bảo trì thiết bị/cầu nâng), Closed (Tạm ngừng sử dụng)
    public string? LiftType { get; set; } = "2PostLift";  // Chủng loại cầu nâng: 2PostLift (Cầu 2 trụ), 4PostLift (Cầu 4 trụ), ScissorLift (Cầu cắt kéo), PaintBooth (Buồng sơn sấy), GroundBay (Khoang mặt sàn), WashBay (Cầu rửa xe)
    public decimal MaxPayloadKg { get; set; } = 4000m;    // Tải trọng tối đa của cầu nâng (kg)
    public string? CurrentVin { get; set; }               // Số khung VIN của xe đang chiếm dụng khoang
    public string? CurrentModel { get; set; }             // Dòng xe đang trên cầu
    public string? CurrentPlateNo { get; set; }           // Biển số xe đang trên cầu
    public string? CurrentRoNo { get; set; }              // Mã Lệnh sửa chữa RO đang thực hiện (Ser_RO)
    public string? CurrentAppNo { get; set; }             // Mã Lịch hẹn dịch vụ liên kết (Ser_App)
    public string? CurrentTechnician { get; set; }        // Kỹ thuật viên chính phụ trách
    public string? CurrentAdvisor { get; set; }           // Cố vấn dịch vụ phụ trách
    public string? CurrentWorkItem { get; set; }          // Hạng mục công việc đang thực hiện
    public DateTime? OccupiedAt { get; set; }             // Thời điểm xe vào khoang cầu
    public DateTime? EstimatedReleaseAt { get; set; }     // Thời điểm dự kiến bàn giao xe / trả khoang
    public DateTime? StartUseDate { get; set; }           // Ngày đưa cầu nâng vào khai thác
    public DateTime? FinishUseDate { get; set; }          // Ngày hết hạn kiểm định / khai thác
    public bool IsActive { get; set; } = true;            // Đang hoạt động
    public string? Remark { get; set; }                   // Ghi chú thiết bị/khoang
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>Nhật ký Điều phối xe vào / ra khoang sửa chữa cầu nâng (BizCarSv / Ser_CavityDispatch / CavityDispatchLog): lưu vết lịch sử xe chiếm dụng khoang, chuyển khoang công đoạn, thời gian hoàn thành dịch vụ và giải phóng cầu nâng.</summary>
public sealed class CavityDispatchLog
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CavityId { get; set; }
    public string CavityNo { get; set; } = "";             // Mã khoang/cầu
    public string DealerCode { get; set; } = "";          // Mã đại lý
    public string DispatchNo { get; set; } = "";          // Mã phiếu điều phối (DSP-2026-0001...)
    public string Vin { get; set; } = "";                 // Số khung VIN
    public string? Model { get; set; }                    // Dòng xe
    public string? PlateNo { get; set; }                  // Biển số xe
    public string? RoNo { get; set; }                     // Mã lệnh sửa chữa (Ser_RO)
    public string? AppNo { get; set; }                    // Mã lịch hẹn (Ser_App)
    public string DispatchType { get; set; } = "CheckIn"; // Loại điều phối: CheckIn (Xe vào khoang), Transfer (Chuyển khoang công đoạn), Release (Rời khoang/hoàn tất), Reservation (Giữ chỗ lịch hẹn)
    public string? FromCavityNo { get; set; }             // Khoang trước đó (nếu chuyển khoang)
    public string? ToCavityNo { get; set; }               // Khoang chuyển đến
    public string? Technician { get; set; }               // Kỹ thuật viên thao tác
    public string? ServiceAdvisor { get; set; }           // Cố vấn dịch vụ
    public string? WorkDescription { get; set; }          // Mô tả công việc thực hiện trong khoang
    public DateTime CheckInTime { get; set; } = DateTime.Now; // Thời điểm vào khoang
    public DateTime? CheckOutTime { get; set; }           // Thời điểm rời khoang
    public int? DurationMinutes { get; set; }             // Tổng thời gian chiếm dụng khoang (phút)
    public string Status { get; set; } = "InCavity";      // Trạng thái: InCavity (Đang trong khoang), Completed (Hoàn thành rời khoang), Transferred (Đã chuyển khoang khác), Cancelled (Hủy điều phối)
    public string? Remark { get; set; }                   // Ghi chú điều phối
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// ===== DTOs cho Quản lý Khoang sửa chữa xưởng dịch vụ & Điều phối xe (BizCarSv / Ser_Cavity & Ser_CavityDispatch) =====

public sealed record CreateServiceCavityDto(
    string CavityNo,
    string? CavityNoUser,
    string CavityName,
    string DealerCode,
    string? CavityType,
    string? LiftType,
    decimal? MaxPayloadKg,
    DateTime? StartUseDate,
    DateTime? FinishUseDate,
    string? Remark,
    string? CreatedBy
);

public sealed record UpdateServiceCavityDto(
    string? CavityNoUser,
    string? CavityName,
    string? DealerCode,
    string? CavityType,
    string? Status,
    string? LiftType,
    decimal? MaxPayloadKg,
    DateTime? StartUseDate,
    DateTime? FinishUseDate,
    bool? IsActive,
    string? Remark
);

public sealed record DispatchVehicleToCavityDto(
    string Vin,
    string? PlateNo,
    string? Model,
    string? RoNo,
    string? AppNo,
    string? Technician,
    string? Advisor,
    string? WorkItem,
    DateTime? CheckInTime,
    DateTime? EstimatedReleaseAt,
    string? Remark,
    string? CreatedBy
);

public sealed record TransferCavityDto(
    string TargetCavityNo,
    string? Technician,
    string? WorkItem,
    DateTime? EstimatedReleaseAt,
    string? Remark,
    string? CreatedBy
);

public sealed record ReleaseCavityDto(
    DateTime? CheckOutTime,
    string? NextStatus,
    string? Remark,
    string? Actor
);

public sealed record SetCavityMaintenanceDto(
    bool IsMaintenance,
    string? Reason,
    DateTime? EstimatedFinishDate,
    string? Remark,
    string? Actor
);

public sealed record ServiceCavitySummaryDto(
    int TotalCavities,
    int TotalActive,
    int TotalAvailable,
    int TotalOccupied,
    int TotalReserved,
    int TotalMaintenance,
    int TotalClosed,
    decimal UtilizationRatePercent,
    List<CavityTypeStatsDto> ByType,
    List<CavityDealerStatsDto> ByDealer
);

public sealed record CavityTypeStatsDto(
    string CavityType,
    string CavityTypeName,
    int TotalCount,
    int AvailableCount,
    int OccupiedCount,
    int MaintenanceCount
);

public sealed record CavityDealerStatsDto(
    string DealerCode,
    int TotalCount,
    int AvailableCount,
    int OccupiedCount,
    int MaintenanceCount,
    decimal UtilizationRatePercent
);

public sealed record CavityDispatchBoardDto(
    string? DealerCode,
    DateTime GeneratedAt,
    int TotalCavities,
    int AvailableCount,
    int OccupiedCount,
    int MaintenanceCount,
    decimal UtilizationRatePercent,
    List<CavityBoardItemDto> Cavities
);

public sealed record CavityBoardItemDto(
    long Id,
    string CavityNo,
    string CavityName,
    string DealerCode,
    string CavityType,
    string Status,
    string? LiftType,
    decimal MaxPayloadKg,
    string? CurrentVin,
    string? CurrentModel,
    string? CurrentPlateNo,
    string? CurrentRoNo,
    string? CurrentAppNo,
    string? CurrentTechnician,
    string? CurrentAdvisor,
    string? CurrentWorkItem,
    DateTime? OccupiedAt,
    DateTime? EstimatedReleaseAt,
    int? ElapsedMinutes,
    int? RemainingMinutes,
    bool IsOverdue,
    bool IsActive
);

public sealed record VehicleCavityInfoDto(
    string Vin,
    string Model,
    string? PlateNo,
    string? EngineNo,
    string? Color,
    string? CurrentCavityNo,
    string? CurrentCavityName,
    string? LastCavityNo,
    string? LastCavityName,
    DateTime? LastCavityDate,
    int CavityVisitCount,
    ServiceCavity? CurrentCavity,
    List<CavityDispatchLog> RecentDispatches
);

// ===== Bảng kê & Quyết toán chi phí Lưu kho bãi xe ô tô tồn kho OEM (BizHTC.Payment / Pmt_PaymentStorage, Pmt_PaymentStorageDetail / FrmQuanLyThanhToanLuuKho) =====

/// <summary>Bảng kê & Quyết toán chi phí lưu kho bãi ô tô tồn bãi OEM / Cảng (BizHTC.Payment.Pmt_PaymentStorage / StoragePayment): quản lý thanh toán chi phí lưu bãi ô tô giữa Hãng xe OEM và Ban quản lý bãi/đơn vị kho bãi TCMS.</summary>
public sealed class StoragePayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PaymentStorageNo { get; set; } = "";        // Mã bảng kê quyết toán lưu kho (STP202603-0001, STP...)
    public string? PaymentStorageNoUser { get; set; }       // Mã số bảng kê do người dùng nhập / tham chiếu nội bộ
    public string PmtMonth { get; set; } = "";              // Kỳ / tháng quyết toán chi phí (YYYY-MM, ví dụ: 2026-03)
    public string StorageCode { get; set; } = "TCV_YARD";   // Mã kho bãi quyết toán (TCV_YARD, NINHBINH_FACTORY, HAIPHONG_PORT, CATLAI_PORT, DANANG_YARD...)
    public string? StorageProvider { get; set; } = "TCMS - Thanh Cong Motor Services"; // Đơn vị quản lý / vận hành kho bãi
    public int TotalVehicleCount { get; set; } = 0;         // Tổng số lượng xe tồn bãi trong kỳ quyết toán
    public int TotalStorageDays { get; set; } = 0;          // Tổng số ngày lưu bãi của toàn bộ các xe trong kỳ
    public decimal TotalBeforeVAT { get; set; } = 0;        // Tổng chi phí lưu kho trước thuế VAT (VNĐ)
    public decimal VatRate { get; set; } = 10;              // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;        // Tiền thuế VAT = TotalBeforeVAT * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;           // Tổng số tiền thanh toán đã bao gồm VAT = TotalBeforeVAT + TotalVatAmount
    public string Status { get; set; } = "Draft";           // Draft → Submitted → Approved1 → Approved2 → TCMSSigned → HTVSigned → Settled (hoặc Rejected / Cancelled)
    public string? TCMSSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số Ban Quản lý Bãi TCMS (Unsigned, Signed)
    public DateTime? TCMSSignDate { get; set; }             // Ngày ký số TCMS
    public string? TCMSSignBy { get; set; }                 // Người đại diện TCMS ký số
    public string? HTVSignStatus { get; set; } = "Unsigned";  // Trạng thái ký số Hãng xe HTV (Unsigned, Signed)
    public DateTime? HTVSignDate { get; set; }              // Ngày ký số HTV
    public string? HTVSignBy { get; set; }                  // Người đại diện HTV ký số
    public string? BankRefNo { get; set; }                  // Số chứng từ / Ủy nhiệm chi UNC ngân hàng giải ngân thanh toán
    public DateTime? PaymentDate { get; set; }              // Ngày thực tế chuyển khoản thanh toán
    public string? FilePath { get; set; }                   // Tệp đính kèm bảng kê có chữ ký số (PDF)
    public string? Remark { get; set; }                     // Ghi chú đợt quyết toán
    public string? CreatedBy { get; set; }                  // Người lập bảng kê
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Approved1By { get; set; }                // Kế toán chi phí OEM sơ duyệt
    public DateTime? Approved1At { get; set; }
    public string? Approved2By { get; set; }                // Lãnh đạo Khối Tài chính / Bán hàng duyệt
    public DateTime? Approved2At { get; set; }
    public string? SettledBy { get; set; }                  // Kế toán trưởng / Thủ quỹ xác nhận giải ngân
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong Bảng kê quyết toán chi phí lưu kho (BizHTC.Payment.Pmt_PaymentStorageDetail / StoragePaymentLine): số khung VIN, model, ngày vào/ra kho bãi, số ngày tính phí, đơn giá lưu kho/ngày, phí bạt phủ và tổng chi phí.</summary>
public sealed class StoragePaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long StoragePaymentId { get; set; }
    public string PaymentStorageNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;                 // Thứ tự dòng
    public string Vin { get; set; } = "";                   // Số khung VIN xe lưu bãi
    public string Model { get; set; } = "";                 // Dòng xe (Accent, Creta, Tucson, SantaFe, Grand i10, Custin...)
    public string? SpecCode { get; set; }                   // Phiên bản xe
    public string? EngineNo { get; set; }                   // Số máy
    public string? Color { get; set; }                      // Màu sắc
    public string? StorageCodeInit { get; set; }            // Vị trí bãi đỗ lưu xe ban đầu (TCV_YARD, YARD-A1, PORT_HP...)
    public DateTime? StoreDate { get; set; }                // Ngày xe bắt đầu nhập kho bãi lưu giữ
    public DateTime? DeliveryOutDate { get; set; }          // Ngày xuất bãi giao xe / hạ tải đại lý (nếu có trong kỳ)
    public string? DealerCode { get; set; }                 // Đại lý phân bổ / nhận xe (nếu có)
    public DateTime InCostStorageDate { get; set; } = DateTime.Now;  // Ngày bắt đầu tính phí lưu kho trong kỳ (From Date)
    public DateTime OutCostStorageDate { get; set; } = DateTime.Now; // Ngày kết thúc tính phí lưu kho trong kỳ (To Date)
    public int StorageDays { get; set; } = 1;               // Số ngày lưu bãi thực tế tính phí trong kỳ
    public decimal DailyRate { get; set; } = 35000;         // Đơn giá lưu bãi theo ngày (VNĐ/xe/ngày)
    public decimal CoverDailyRate { get; set; } = 0;        // Đơn giá bạt phủ che chắn ngoài trời / ngày (VNĐ/ngày)
    public decimal StorageCost { get; set; } = 35000;       // Tiền phí lưu bãi cơ bản = StorageDays * DailyRate (VNĐ)
    public decimal CoverCost { get; set; } = 0;             // Tiền phí bạt phủ = StorageDays * CoverDailyRate (VNĐ)
    public decimal TotalAmount { get; set; } = 35000;       // Tổng tiền lưu bãi xe = StorageCost + CoverCost (VNĐ)
    public string StorageLevel { get; set; } = "Standard";  // Mức lưu kho: Standard (Bãi tiêu chuẩn), Covered (Có mái che), Port (Cảng chờ thông quan), Overdue (Tồn bãi quá hạn > 60 ngày)
    public string Status { get; set; } = "Pending";         // Pending → Approved → Settled (hoặc Cancelled)
    public string? Remark { get; set; }                     // Ghi chú tình trạng xe
}

// ===== DTOs cho Bảng kê & Quyết toán chi phí Lưu kho bãi OEM (BizHTC.Payment / Pmt_PaymentStorage & StoragePayment) =====

public sealed record CreateStoragePaymentDto(
    string? PaymentStorageNo,
    string? PaymentStorageNoUser,
    string PmtMonth,
    string? StorageCode,
    string? StorageProvider,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<StoragePaymentLineInputDto>? Items
);

public sealed record StoragePaymentLineInputDto(
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? StorageCodeInit,
    DateTime? StoreDate,
    DateTime? DeliveryOutDate,
    string? DealerCode,
    DateTime? InCostStorageDate,
    DateTime? OutCostStorageDate,
    int? StorageDays,
    decimal? DailyRate,
    decimal? CoverDailyRate,
    string? StorageLevel,
    string? Remark
);

public sealed record UpdateStoragePaymentHeaderDto(
    string? PaymentStorageNoUser,
    string? PmtMonth,
    string? StorageCode,
    string? StorageProvider,
    decimal? VatRate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath,
    string? Remark
);

public sealed record UpdateStoragePaymentLineDto(
    string? Model,
    string? SpecCode,
    string? StorageCodeInit,
    DateTime? StoreDate,
    DateTime? DeliveryOutDate,
    string? DealerCode,
    DateTime? InCostStorageDate,
    DateTime? OutCostStorageDate,
    int? StorageDays,
    decimal? DailyRate,
    decimal? CoverDailyRate,
    string? StorageLevel,
    string? Status,
    string? Remark
);

public sealed record StoragePaymentTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath
);

public sealed record StoragePaymentSummaryDto(
    int TotalPayments,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalSigned,
    int TotalSettled,
    int TotalCancelled,
    int TotalVehicles,
    int TotalStorageDays,
    decimal TotalBeforeVAT,
    decimal TotalVatAmount,
    decimal TotalAmount,
    decimal TotalSettledAmount,
    List<StoragePaymentYardStatsDto> ByYard,
    List<StoragePaymentMonthStatsDto> ByMonth
);

public sealed record StoragePaymentYardStatsDto(string StorageCode, string StorageProvider, int PaymentCount, int VehicleCount, int StorageDays, decimal TotalAmount, decimal SettledAmount);
public sealed record StoragePaymentMonthStatsDto(string PmtMonth, int PaymentCount, int VehicleCount, int StorageDays, decimal TotalAmount, decimal SettledAmount);

public sealed record VehicleStoragePaymentInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    bool IsStoragePaid,
    decimal StoragePaidAmount,
    string? LastStoragePaymentNo,
    DateTime? LastStoragePaymentDate,
    int StoragePaymentCount,
    List<StoragePaymentLine> PaymentLines
);





