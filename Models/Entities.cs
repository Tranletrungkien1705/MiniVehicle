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
    public bool IsGpsPaid { get; set; } = false;      // Đã thanh toán / quyết toán chi phí dịch vụ thiết bị định vị GPS (BizHTC.Payment.Pmt_PaymentGPS)
    public decimal GpsPaidAmount { get; set; } = 0;   // Tổng tiền dịch vụ GPS đã thanh toán của xe (VNĐ)
    public string? LastGpsPaymentNo { get; set; }     // Mã bảng kê quyết toán GPS gần nhất (PaymentGPSNo)
    public DateTime? LastGpsPaymentDate { get; set; } // Ngày quyết toán chi phí GPS gần nhất
    public int GpsPaymentCount { get; set; } = 0;     // Số lần xe phát sinh trong bảng kê quyết toán GPS
    public string? LastCavityNo { get; set; }       // Mã khoang/cầu sửa chữa xưởng dịch vụ gần nhất xe vào (BizCarSv.Ser_Cavity / ServiceCavity)
    public string? LastCavityName { get; set; }     // Tên khoang/cầu sửa chữa gần nhất
    public DateTime? LastCavityDate { get; set; }   // Thời điểm vào khoang sửa chữa gần nhất
    public int CavityVisitCount { get; set; } = 0;   // Tổng số lượt xe đã vào khoang cầu làm dịch vụ
    public bool IsStoragePaid { get; set; } = false; // Đã thanh toán / quyết toán chi phí lưu kho bãi OEM (BizHTC.Payment.Pmt_PaymentStorage)
    public decimal StoragePaidAmount { get; set; } = 0; // Tổng tiền lưu kho đã thanh toán của xe (VNĐ)
    public string? LastStoragePaymentNo { get; set; } // Mã bảng kê quyết toán lưu kho gần nhất (PaymentStorageNo)
    public DateTime? LastStoragePaymentDate { get; set; } // Ngày quyết toán chi phí lưu kho gần nhất
    public int StoragePaymentCount { get; set; } = 0; // Số lần xe phát sinh trong bảng kê quyết toán lưu kho
    public bool IsAvnInstalled { get; set; } = false; // Đã trang bị màn hình giải trí & dẫn đường AVN (BizHTC.Payment.Pmt_PaymentAVN)
    public string? AvnDeviceCode { get; set; }        // Mã chủng loại màn hình AVN gắn trên xe
    public string? AvnSerialNo { get; set; }          // Số Serial màn hình AVN
    public string? MapCardSerialNo { get; set; }      // Số Serial thẻ nhớ bản đồ dẫn đường
    public bool IsAvnPaid { get; set; } = false;      // Đã thanh toán / quyết toán chi phí thiết bị AVN (BizHTC.Payment.Pmt_PaymentAVN)
    public decimal AvnPaidAmount { get; set; } = 0;   // Tổng tiền AVN đã thanh toán của xe (VNĐ)
    public string? LastAvnPaymentNo { get; set; }     // Mã bảng kê quyết toán AVN gần nhất (PaymentAVNNo)
    public DateTime? LastAvnPaymentDate { get; set; } // Ngày quyết toán chi phí AVN gần nhất
    public int AvnPaymentCount { get; set; } = 0;     // Số lần xe phát sinh trong bảng kê quyết toán AVN
    public string? LastTestDriveNo { get; set; }      // Mã phiếu khách hàng lái thử xe gần nhất (CustomerTestDrive / DLR_DriveTest)
    public DateTime? LastTestDriveDate { get; set; }  // Ngày lái thử xe gần nhất
    public int TestDriveCount { get; set; } = 0;      // Tổng số lượt khách hàng đã lái thử trên xe này
    public string? LastTranspPlanNo { get; set; }     // Mã kế hoạch điều độ vận tải gần nhất (Sto_TranspPlan / TransportPlan)
    public DateTime? LastTranspPlanDate { get; set; } // Ngày lập kế hoạch điều độ vận tải gần nhất
    public int TranspPlanCount { get; set; } = 0;     // Tổng số lần xe phát sinh trong kế hoạch điều độ vận tải
    public bool IsTranspInsPaid { get; set; } = false; // Đã thanh toán / quyết toán chi phí vận tải & bảo hiểm xe (BizHTC.Payment.Pmt_TransportIns)
    public decimal TranspInsPaidAmount { get; set; } = 0; // Tổng tiền cước vận chuyển & bảo hiểm đã thanh toán của xe (VNĐ)
    public string? LastTranspInsPaymentNo { get; set; } // Mã bảng kê quyết toán vận tải & bảo hiểm gần nhất (TransportInsNo)
    public DateTime? LastTranspInsPaymentDate { get; set; } // Ngày quyết toán chi phí vận tải & bảo hiểm gần nhất
    public int TranspInsPaymentCount { get; set; } = 0; // Số lần xe phát sinh trong bảng kê quyết toán vận chuyển & bảo hiểm
    public DateTime? LastInventoryAuditDate { get; set; } // Ngày kiểm kê & đối soát định mức tồn kho an toàn gần nhất (Mst_DealerInventoryThreshold)
    public string? InventoryAlertStatus { get; set; }   // Trạng thái sức khỏe tồn kho: Optimal, Shortage, CriticalShortage, Surplus, OutOfStock
    public string? LastThresholdNo { get; set; }        // Mã quyết định định mức tồn kho áp dụng gần nhất (DIT...)
    public int ThresholdAuditCount { get; set; } = 0;   // Tổng số lần xe được đối soát trong các đợt kiểm kê tồn kho đại lý
    public string? LastServicePackageNo { get; set; }   // Mã gói dịch vụ gần nhất xe đăng ký (BizCarSv.ServicePackage / Ser_ServicePackage)
    public string? LastPackageCardNo { get; set; }      // Mã thẻ bảo dưỡng trọn gói điện tử đang kích hoạt (PackageCardNo)
    public int ActiveServicePackageCount { get; set; } = 0; // Số lượng thẻ / gói dịch vụ đang còn hiệu lực
    public int PackageUsageCount { get; set; } = 0;     // Tổng số lượt xe đã sử dụng quyền lợi gói dịch vụ bảo dưỡng
    public string? LastAutoDoNo { get; set; }           // Mã Lệnh giao xe DO tự động gần nhất sinh ra cho xe (DO-AUTO-...)
    public DateTime? LastAutoDoDate { get; set; }       // Ngày tự động phân bổ & sinh lệnh giao xe gần nhất
    public int AutoDoCount { get; set; } = 0;           // Tổng số lần xe được xử lý trong các đợt phân bổ giao xe tự động
    public string? LastSsiNo { get; set; }              // Mã phiếu khảo sát hài lòng bán hàng SSI gần nhất (BizHTC.DealerSales.DLS_VINSurvey / SalesSatisfactionSurvey)
    public DateTime? LastSsiDate { get; set; }          // Ngày thực hiện khảo sát SSI gần nhất
    public decimal? LastSsiScore { get; set; }          // Điểm chỉ số hài lòng bán hàng SSI gần nhất (1.0 - 5.0 sao)
    public int? LastSsiIndex1000 { get; set; }          // Điểm chỉ số SSI quy đổi thang 1000 điểm tiêu chuẩn J.D. Power (0 - 1000)
    public int SsiSurveyCount { get; set; } = 0;        // Tổng số lượt đã thực hiện khảo sát SSI cho xe này
    public string? LastCustomerVisitNo { get; set; }    // Mã phiếu khách tham quan showroom quan tâm xe gần nhất (BizHTC.RetailContract.CtmVisit / CustomerVisit)
    public DateTime? LastCustomerVisitDate { get; set; } // Ngày lượt khách gần nhất đến xem xe
    public int CustomerVisitCount { get; set; } = 0;    // Tổng số lượt khách đã đến xem / quan tâm dòng xe này
    public bool IsMktFeeSupported { get; set; } = false; // Đã được duyệt quyết toán hỗ trợ kinh phí Marketing (BizHTC.Marketing.MKT_MarketingFee)
    public decimal MktFeeSupportedAmount { get; set; } = 0; // Tổng tiền Marketing đã duyệt chi cho xe / dòng xe (VNĐ)
    public string? LastMktFeeNo { get; set; }           // Mã hồ sơ quyết toán Marketing gần nhất (MKT-...)
    public DateTime? LastMktFeeDate { get; set; }       // Ngày quyết toán Marketing gần nhất
    public int MktFeeCount { get; set; } = 0;           // Tổng số lần xe phát sinh trong hồ sơ quyết toán Marketing
    public string? LastSalesKpiNo { get; set; }         // Mã kế hoạch chỉ tiêu KPI bán hàng gần nhất xe tham gia (SP_KPIMonth / SalesTargetKpi)
    public DateTime? LastSalesKpiDate { get; set; }     // Ngày phát sinh chỉ tiêu / ghi nhận KPI
    public int SalesKpiCount { get; set; } = 0;         // Tổng số lần xe được ghi nhận trong các kỳ đánh giá KPI bán hàng
    public string? DocumentStatus { get; set; }     // Trạng thái hồ sơ xe (Car_VIN.DOCUMENTSTATUS): tình trạng giấy tờ/hồ sơ pháp lý của xe
    public DateTime? FullDocDate { get; set; }      // Ngày đủ hồ sơ (Car_VIN.FULLDOCDATE): ngày hoàn tất đầy đủ hồ sơ giấy tờ xe
    public string? DocRemarkDetail { get; set; }    // Ghi chú chi tiết tình trạng hồ sơ (Car_VIN.REMARKDETAIL)
    public string? LastDocStatusNo { get; set; }    // Mã lần cập nhật trạng thái hồ sơ gần nhất (DSL...)
    public DateTime? LastDocStatusDate { get; set; } // Ngày cập nhật trạng thái hồ sơ gần nhất
    public int DocStatusUpdateCount { get; set; } = 0; // Tổng số lần cập nhật trạng thái hồ sơ của xe
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
    public string? BankCodeMD { get; set; }                // Mã ngân hàng thanh toán (BankCodeMD) — gán khi biên bản hủy thanh toán qua NH hoàn tất
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

/// <summary>Danh mục loại công việc bảo dưỡng lưu kho (BizHTC.StorageFG.Mst_MaintainTask / MaintenanceTask): nhóm hạng mục kiểm tra kỹ thuật chuẩn cho xe tồn bãi OEM (ắc quy, lốp, động cơ, chất lỏng, điện, vệ sinh...).</summary>
public sealed class MaintenanceTask
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MtnTkCode { get; set; } = "";          // Mã loại công việc bảo dưỡng (MtnTkCode)
    public string MtnTkName { get; set; } = "";          // Tên loại công việc bảo dưỡng
    public string? MtnTp { get; set; }                    // Loại bảo dưỡng áp dụng (PERMANENT / STOCK)
    public int SortOrder { get; set; } = 0;               // Thứ tự hiển thị
    public bool FlagActive { get; set; } = true;          // Đang áp dụng (FlagActive)
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Danh mục hạng mục kiểm tra chi tiết trong 1 loại công việc bảo dưỡng (BizHTC.StorageFG.Mst_MaintainTaskItem / MaintenanceTaskItem): từng tiêu chí kiểm tra kèm đơn vị đo và giá trị chuẩn.</summary>
public sealed class MaintenanceTaskItem
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MtnTkCode { get; set; } = "";          // Mã loại công việc bảo dưỡng (FK MaintenanceTask)
    public string MtnTkItemCode { get; set; } = "";      // Mã hạng mục kiểm tra (MtnTkItemCode)
    public string MtnTkItemName { get; set; } = "";      // Tên hạng mục kiểm tra
    public string? Unit { get; set; }                     // Đơn vị đo (V, bar, %, OK/NG...)
    public string? StandardValue { get; set; }            // Giá trị / ngưỡng chuẩn kỹ thuật
    public int SortOrder { get; set; } = 0;               // Thứ tự hiển thị
    public bool FlagActive { get; set; } = true;          // Đang áp dụng (FlagActive)
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Kết quả kiểm tra chi tiết từng hạng mục bảo dưỡng theo VIN (BizHTC.StorageFG.StoF_MaintainMix / StorageMaintenanceChecklist): giá trị đo thực tế (MtnVal) và kết luận đạt/không đạt cho từng tiêu chí kiểm tra của xe trong đợt bảo dưỡng.</summary>
public sealed class StorageMaintenanceChecklist
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long StorageMaintenanceId { get; set; }
    public string MtnNo { get; set; } = "";              // Mã phiếu bảo dưỡng (FK StorageMaintenance)
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string MtnTkCode { get; set; } = "";          // Mã loại công việc bảo dưỡng
    public string MtnTkItemCode { get; set; } = "";      // Mã hạng mục kiểm tra
    public string? MtnTkItemName { get; set; }            // Tên hạng mục kiểm tra (snapshot)
    public string? MtnVal { get; set; }                   // Giá trị đo / kết quả thực tế (MtnVal)
    public bool IsPassed { get; set; } = true;            // Kết luận đạt chuẩn cho hạng mục này
    public string Status { get; set; } = "Pending";       // Pending → Approved (hoặc Rejected)
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
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

// ===== Bảng kê & Quyết toán chi phí Màn hình / Thiết bị Audio Visual Navigation AVN & Thẻ bản đồ định vị trên xe ô tô (BizHTC.Payment / Pmt_PaymentAVN, Pmt_PaymentAVNDetail, Mst_AVNPrice / FrmQuanLyThanhToanAVN, FrmTaoThanhToanAVN) =====

/// <summary>Bảng kê & Quyết toán chi phí Màn hình giải trí & Dẫn đường thông minh AVN kèm Thẻ bản đồ định vị GPS bản quyền (BizHTC.Payment.Pmt_PaymentAVN / AvnPayment): quản lý đối soát và thanh toán chi phí thiết bị AVN, thẻ bản đồ và công lắp đặt giữa Hãng xe OEM HTV và Nhà cung cấp giải pháp màn hình/bản đồ AVN (Mobis, Panasonic, Vietmap, FPT...).</summary>
public sealed class AvnPayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PaymentAVNNo { get; set; } = "";        // Mã bảng kê quyết toán AVN (AVN-202603-001, PAVN...)
    public string? PaymentAVNNoUser { get; set; }       // Mã số bảng kê do người dùng nhập / tham chiếu nội bộ
    public string PmtMonth { get; set; } = "";          // Kỳ / tháng quyết toán chi phí (YYYY-MM, ví dụ: 2026-03)
    public string SupplierCode { get; set; } = "MOBIS"; // Mã nhà cung cấp thiết bị AVN (MOBIS, PANASONIC, VIETMAP, FPT_AUTO...)
    public string? SupplierName { get; set; } = "Mobis Auto Parts Vietnam"; // Tên nhà cung cấp thiết bị AVN
    public int TotalVehicleCount { get; set; } = 0;     // Tổng số lượng xe lắp đặt thiết bị AVN trong bảng kê
    public decimal TotalBeforeVAT { get; set; } = 0;    // Tổng chi phí AVN trước thuế VAT (VNĐ)
    public decimal VatRate { get; set; } = 10;          // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;    // Tiền thuế VAT = TotalBeforeVAT * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;       // Tổng số tiền thanh toán đã bao gồm VAT = TotalBeforeVAT + TotalVatAmount
    public string Status { get; set; } = "Draft";       // Draft → Submitted → Approved1 → Approved2 → SupplierSigned → HTVSigned → Settled (hoặc Rejected / Cancelled)
    public string? SupplierSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số Nhà cung cấp AVN (Unsigned, Signed)
    public DateTime? SupplierSignDate { get; set; }     // Ngày ký số Nhà cung cấp AVN
    public string? SupplierSignBy { get; set; }         // Người đại diện Nhà cung cấp ký số
    public string? HTVSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số Hãng xe OEM HTV (Unsigned, Signed)
    public DateTime? HTVSignDate { get; set; }          // Ngày ký số HTV
    public string? HTVSignBy { get; set; }              // Người đại diện HTV ký số
    public string? BankRefNo { get; set; }              // Số chứng từ / Ủy nhiệm chi UNC ngân hàng giải ngân thanh toán
    public DateTime? PaymentDate { get; set; }          // Ngày thực tế chuyển khoản thanh toán
    public string? FilePath { get; set; }               // Tệp đính kèm bảng kê có chữ ký số (PDF)
    public string? Remark { get; set; }                 // Ghi chú đợt quyết toán
    public string? CreatedBy { get; set; }              // Người lập bảng kê
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Approved1By { get; set; }            // Kế toán chi phí vật tư sơ duyệt A1
    public DateTime? Approved1At { get; set; }
    public string? Approved2By { get; set; }            // Giám đốc Khối Phụ tùng / Khối Sản xuất duyệt A2
    public DateTime? Approved2At { get; set; }
    public string? SettledBy { get; set; }              // Kế toán trưởng / Thủ quỹ xác nhận giải ngân
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong Bảng kê quyết toán chi phí AVN (BizHTC.Payment.Pmt_PaymentAVNDetail / AvnPaymentLine): số khung VIN, model, mã đầu AVN, serial đầu AVN, serial thẻ bản đồ GPS, phiên bản bản đồ, đơn giá thiết bị, giá thẻ bản đồ, công lắp đặt và tổng chi phí.</summary>
public sealed class AvnPaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long AvnPaymentId { get; set; }
    public string PaymentAVNNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;             // Thứ tự dòng
    public string Vin { get; set; } = "";               // Số khung VIN xe gắn màn hình AVN
    public string Model { get; set; } = "";             // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra, Stargazer, Custin, Ioniq 5...)
    public string? SpecCode { get; set; }               // Phiên bản xe
    public string? EngineNo { get; set; }               // Số máy
    public string? Color { get; set; }                  // Màu sắc
    public string AvnDeviceCode { get; set; } = "AVN-GEN5W-10INCH"; // Mã chủng loại màn hình AVN (AVN-GEN5W-10INCH, AVN-GEN5-8INCH, AVN-OLED-12.3INCH...)
    public string AvnSerialNo { get; set; } = "";       // Số Serial thân máy màn hình AVN
    public string? MapCardSerialNo { get; set; }        // Số Serial thẻ nhớ bản đồ dẫn đường GPS
    public string? MapVersion { get; set; } = "VN-MAP-2026.Q1"; // Phiên bản phần mềm bản đồ số
    public decimal DevicePrice { get; set; } = 7500000m; // Đơn giá thiết bị màn hình AVN (VNĐ)
    public decimal MapPrice { get; set; } = 1200000m;   // Đơn giá thẻ nhớ bản đồ / bản quyền bản đồ dẫn đường (VNĐ)
    public decimal InstallationFee { get; set; } = 300000m; // Chi phí công lắp ráp kết nối điện (VNĐ)
    public decimal AccessoryCost { get; set; } = 200000m;   // Chi phí phụ kiện cáp giắc anten GPS (VNĐ)
    public decimal TotalAmount { get; set; } = 9200000m; // Tổng chi phí AVN trên xe = DevicePrice + MapPrice + InstallationFee + AccessoryCost (VNĐ)
    public DateTime? InStorageDate { get; set; }        // Ngày xe nhập kho bãi / xuất xưởng
    public DateTime? AvnInstallDate { get; set; }       // Ngày hoàn tất lắp đặt kích hoạt thiết bị AVN trên xe
    public string Status { get; set; } = "Pending";     // Pending → Approved → Settled (hoặc Cancelled)
    public string? Remark { get; set; }                 // Ghi chú kỹ thuật chi tiết
}

// ===== DTOs cho Bảng kê & Quyết toán chi phí Màn hình AVN & Bản đồ dẫn đường (BizHTC.Payment / Pmt_PaymentAVN & AvnPayment) =====

public sealed record CreateAvnPaymentDto(
    string? PaymentAVNNo,
    string? PaymentAVNNoUser,
    string PmtMonth,
    string? SupplierCode,
    string? SupplierName,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<AvnPaymentLineInputDto>? Items
);

public sealed record AvnPaymentLineInputDto(
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? AvnDeviceCode,
    string? AvnSerialNo,
    string? MapCardSerialNo,
    string? MapVersion,
    decimal? DevicePrice,
    decimal? MapPrice,
    decimal? InstallationFee,
    decimal? AccessoryCost,
    DateTime? InStorageDate,
    DateTime? AvnInstallDate,
    string? Remark
);

public sealed record UpdateAvnPaymentHeaderDto(
    string? PaymentAVNNoUser,
    string? PmtMonth,
    string? SupplierCode,
    string? SupplierName,
    decimal? VatRate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath,
    string? Remark
);

public sealed record UpdateAvnPaymentLineDto(
    string? Model,
    string? SpecCode,
    string? AvnDeviceCode,
    string? AvnSerialNo,
    string? MapCardSerialNo,
    string? MapVersion,
    decimal? DevicePrice,
    decimal? MapPrice,
    decimal? InstallationFee,
    decimal? AccessoryCost,
    DateTime? InStorageDate,
    DateTime? AvnInstallDate,
    string? Status,
    string? Remark
);

public sealed record AvnPaymentTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath
);

public sealed record AvnPaymentSummaryDto(
    int TotalPayments,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalSigned,
    int TotalSettled,
    int TotalCancelled,
    int TotalVehicles,
    decimal TotalBeforeVAT,
    decimal TotalVatAmount,
    decimal TotalAmount,
    decimal TotalSettledAmount,
    List<AvnPaymentSupplierStatsDto> BySupplier,
    List<AvnPaymentMonthStatsDto> ByMonth
);

public sealed record AvnPaymentSupplierStatsDto(string SupplierCode, string SupplierName, int PaymentCount, int VehicleCount, decimal TotalAmount, decimal SettledAmount);
public sealed record AvnPaymentMonthStatsDto(string PmtMonth, int PaymentCount, int VehicleCount, decimal TotalAmount, decimal SettledAmount);

public sealed record VehicleAvnPaymentInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    bool IsAvnInstalled,
    string? AvnDeviceCode,
    string? AvnSerialNo,
    string? MapCardSerialNo,
    bool IsAvnPaid,
    decimal AvnPaidAmount,
    string? LastAvnPaymentNo,
    DateTime? LastAvnPaymentDate,
    int AvnPaymentCount,
    List<AvnPaymentLine> PaymentLines
);

// ===== Đăng ký & Nhật ký Khách hàng Lái thử xe tại Đại lý / Roadshow (BizHTC.RetailContract / DLR_DriveTest, Mst_CarDriverTest / FrmMngTestDriver, FrmNewTestDriver) =====

/// <summary>Đăng ký &amp; Nhật ký Khách hàng Lái thử xe tại Đại lý / Roadshow (BizHTC.RetailContract / DLR_DriveTest / TestDrive): quản lý tiếp nhận khách hàng trải nghiệm lái thử xe thực tế, kiểm tra thông tin GPLX, xe lái thử VIN, loại hình lái thử (Showroom, HomeDrive, Roadshow), quãng đường ODO, khảo sát phản hồi đánh giá CSI (Động cơ, Cảm giác lái, Cách âm NVH, Tiện nghi ADAS), mức độ tiềm năng mua xe và dự kiến chốt hợp đồng.</summary>
public sealed class CustomerTestDrive
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DriveTestCode { get; set; } = "";             // Mã phiếu lái thử (DT202603-0001, DT...)
    public string? DriveTestCodeUser { get; set; }            // Mã số phiếu tham chiếu nội bộ đại lý
    public string DealerCode { get; set; } = "";              // Mã đại lý tổ chức lái thử
    public string? DealerName { get; set; }                   // Tên đại lý
    public string Vin { get; set; } = "";                     // Số khung xe lái thử
    public string Model { get; set; } = "";                   // Dòng xe lái thử (SantaFe, Tucson, Creta, Accent, Custin, Palisade, Ioniq 5...)
    public string? SpecCode { get; set; }                     // Phiên bản xe (1.6T HTRAC, 2.0 AT, EV 72.6kWh...)
    public string? DrvTestPlateNo { get; set; }               // Biển số xe lái thử (30E-999.88, 51K-888.66...)
    public string FullName { get; set; } = "";                // Họ và tên khách hàng lái thử
    public string PhoneNo { get; set; } = "";                 // SĐT liên hệ của khách hàng
    public string? Email { get; set; }                        // Email khách hàng
    public string? CusAddress { get; set; }                   // Địa chỉ khách hàng
    public string Gender { get; set; } = "Nam";               // Giới tính (Nam, Nữ, Khác)
    public int? BirthYear { get; set; }                       // Năm sinh
    public string? RangeAgeCode { get; set; } = "26-35";      // Nhóm tuổi: 18-25, 26-35, 36-45, 46-55, Over55
    public string DriverLicenseNo { get; set; } = "";         // Số giấy phép lái xe GPLX
    public string? LicenseClass { get; set; } = "B2";         // Hạng GPLX: B1, B2, C, D, E...
    public string DriveTestType { get; set; } = "Showroom";   // Loại hình lái thử: Showroom (Tại đại lý), HomeDrive (Lái thử tại nhà), RoadshowEvent (Sự kiện trải nghiệm), WeekendDrive (Lái thử cuối tuần)
    public string? EventName { get; set; }                    // Tên sự kiện Roadshow / Ngày hội lái thử (nếu có)
    public string? RoutePath { get; set; }                    // Lộ trình / Tuyến đường lái thử (Nội đô, Đường trường, Cao tốc, Cung đường đèo dốc...)
    public DateTime DriveDTime { get; set; } = DateTime.Now;  // Ngày giờ hẹn lái thử
    public int DurationMinutes { get; set; } = 30;            // Thời lượng lái thử thực tế (phút)
    public int OdoStart { get; set; } = 0;                    // Số km ODO trước khi lái thử
    public int? OdoEnd { get; set; }                          // Số km ODO sau khi kết thúc lái thử
    public int DistanceKm { get; set; } = 0;                  // Quãng đường đã chạy (km) = OdoEnd - OdoStart
    public string? SalesManCode { get; set; }                 // Mã tư vấn bán hàng TVBH đồng hành
    public string? SalesManName { get; set; }                 // Tên tư vấn bán hàng TVBH
    public string? Instructor { get; set; }                   // Chuyên gia / KTV hướng dẫn kỹ thuật lái xe an toàn
    public decimal? ScoreEngine { get; set; } = 5.0m;         // Đánh giá động cơ & khả năng tăng tốc (1.0 - 5.0 sao)
    public decimal? ScoreHandling { get; set; } = 5.0m;       // Đánh giá cảm giác lái & vô lăng (1.0 - 5.0 sao)
    public decimal? ScoreNVH { get; set; } = 5.0m;            // Đánh giá độ cách âm & độ êm ái giảm xóc NVH (1.0 - 5.0 sao)
    public decimal? ScoreDesign { get; set; } = 5.0m;         // Đánh giá thiết kế ngoại thất & nội thất (1.0 - 5.0 sao)
    public decimal? ScoreFeatures { get; set; } = 5.0m;       // Đánh giá tính năng an toàn ADAS & tiện nghi công nghệ (1.0 - 5.0 sao)
    public decimal? ScoreOverall { get; set; } = 5.0m;        // Điểm đánh giá hài lòng chung (1.0 - 5.0 sao)
    public string? CustomerFeedback { get; set; }             // Ý kiến nhận xét chi tiết của khách hàng sau khi lái thử
    public string PurchaseIntent { get; set; } = "High";     // Mức độ tiềm năng mua xe: VeryHigh (Rất cao - Trong tuần), High (Cao - Trong tháng), Medium (Trung bình - Đang cân nhắc), Low (Thấp - Tham khảo)
    public string? CompetitorModel { get; set; }              // Dòng xe đối thủ khách đang so sánh (Mazda CX-5, Ford Territory, Honda CR-V, Toyota Corolla Cross...)
    public DateTime? ExpectedDealDate { get; set; }           // Ngày dự kiến ký hợp đồng mua xe
    public string Status { get; set; } = "Draft";             // Draft → Scheduled → InProgress → Completed (hoặc Cancelled / NoShow / Rejected)
    public string? Remark { get; set; }                       // Ghi chú điều hành buổi lái thử
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                   // Trưởng phòng bán hàng duyệt lịch lái thử
    public DateTime? ApprovedAt { get; set; }
    public string? StartedBy { get; set; }                    // TVBH xuất phát bàn giao xe
    public DateTime? StartedAt { get; set; }
    public string? CompletedBy { get; set; }                  // TVBH & khách hàng nghiệm thu hoàn tất
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

// ===== DTOs cho Khách hàng Lái thử xe (BizHTC.RetailContract / DLR_DriveTest / CustomerTestDrive) =====

public sealed record CreateCustomerTestDriveDto(
    string? DriveTestCode,
    string? DriveTestCodeUser,
    string DealerCode,
    string? DealerName,
    string Vin,
    string? Model,
    string? SpecCode,
    string? DrvTestPlateNo,
    string FullName,
    string PhoneNo,
    string? Email,
    string? CusAddress,
    string? Gender,
    int? BirthYear,
    string? RangeAgeCode,
    string DriverLicenseNo,
    string? LicenseClass,
    string? DriveTestType,
    string? EventName,
    string? RoutePath,
    DateTime? DriveDTime,
    int? DurationMinutes,
    int? OdoStart,
    string? SalesManCode,
    string? SalesManName,
    string? Instructor,
    string? PurchaseIntent,
    string? CompetitorModel,
    DateTime? ExpectedDealDate,
    string? Remark,
    string? CreatedBy
);

public sealed record UpdateCustomerTestDriveDto(
    string? DriveTestCodeUser,
    string? DealerCode,
    string? DealerName,
    string? Vin,
    string? Model,
    string? SpecCode,
    string? DrvTestPlateNo,
    string? FullName,
    string? PhoneNo,
    string? Email,
    string? CusAddress,
    string? Gender,
    int? BirthYear,
    string? RangeAgeCode,
    string? DriverLicenseNo,
    string? LicenseClass,
    string? DriveTestType,
    string? EventName,
    string? RoutePath,
    DateTime? DriveDTime,
    int? DurationMinutes,
    int? OdoStart,
    int? OdoEnd,
    string? SalesManCode,
    string? SalesManName,
    string? Instructor,
    decimal? ScoreEngine,
    decimal? ScoreHandling,
    decimal? ScoreNVH,
    decimal? ScoreDesign,
    decimal? ScoreFeatures,
    decimal? ScoreOverall,
    string? CustomerFeedback,
    string? PurchaseIntent,
    string? CompetitorModel,
    DateTime? ExpectedDealDate,
    string? Remark
);

public sealed record CustomerTestDriveTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    int? OdoStart,
    int? OdoEnd,
    DateTime? TransitionDate
);

public sealed record RecordTestDriveFeedbackDto(
    decimal? ScoreEngine,
    decimal? ScoreHandling,
    decimal? ScoreNVH,
    decimal? ScoreDesign,
    decimal? ScoreFeatures,
    decimal? ScoreOverall,
    string? CustomerFeedback,
    string? PurchaseIntent,
    string? CompetitorModel,
    DateTime? ExpectedDealDate,
    int? OdoEnd,
    string? Actor,
    string? Remark
);

public sealed record CustomerTestDriveSummaryDto(
    int TotalTestDrives,
    int TotalDraft,
    int TotalScheduled,
    int TotalInProgress,
    int TotalCompleted,
    int TotalCancelled,
    int TotalNoShow,
    int TotalHighPotential,
    decimal AverageScoreOverall,
    decimal AverageScoreEngine,
    decimal AverageScoreHandling,
    decimal AverageScoreNVH,
    int TotalDistanceKm,
    decimal ConversionRatePercent,
    List<TestDriveModelStatsDto> ByModel,
    List<TestDriveDealerStatsDto> ByDealer,
    List<TestDriveTypeStatsDto> ByDriveType
);

public sealed record TestDriveModelStatsDto(string Model, int TotalDrives, int CompletedDrives, int HighPotentialCount, decimal AverageScore);
public sealed record TestDriveDealerStatsDto(string DealerCode, string DealerName, int TotalDrives, int CompletedDrives, int HighPotentialCount);
public sealed record TestDriveTypeStatsDto(string DriveTestType, int TotalDrives, int CompletedDrives, decimal AverageScore);

public sealed record VehicleTestDriveInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? PlateNo,
    bool IsTestCar,
    string? LastTestDriveNo,
    DateTime? LastTestDriveDate,
    int TestDriveCount,
    List<CustomerTestDrive> RecentTestDrives
);

// ===== Kế hoạch Điều độ Vận tải & Phân bổ Xe ô tô OEM (BizHTC.Storage / Sto_TranspPlan, Sto_TranspPlanDetail / FrmSto_TranspPlan, FrmLenKeHoach_BanHang, FrmUpdateFVINToRVIN) =====

/// <summary>Bảng kê / Đợt Kế hoạch Điều độ Vận tải Phân bổ Xe ô tô OEM (BizHTC.Storage / Sto_TranspPlan / TransportPlan): quản lý kế hoạch điều phối xe từ Nhà máy HTMV Ninh Bình đến các tỉnh/thành phố và đại lý toàn quốc, phối hợp 3 phòng ban Kế hoạch - Bán hàng - Logistics, quản lý gán số khung thực tế (Map VIN Real) và phê duyệt xuất bến.</summary>
public sealed class TransportPlan
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PlanNo { get; set; } = "";             // Mã đợt kế hoạch điều độ vận tải (TP202603-0001, TP...)
    public string? PlanNoUser { get; set; }            // Số hiệu tham chiếu kế hoạch nội bộ OEM (KHVT-2026/03-01)
    public string PlanMonth { get; set; } = "";        // Tháng kế hoạch (yyyy-MM)
    public DateTime PlanDate { get; set; } = DateTime.Now; // Ngày lập kế hoạch
    public string StorageCode { get; set; } = "PLANT-HTMV1"; // Kho / Bãi xuất phát nhà máy (PLANT-HTMV1, PLANT-HTMV2, TCV_YARD)
    public string? StorageName { get; set; } = "Kho Tổng Nhà máy HTMV Ninh Bình 1";
    public string TPType { get; set; } = "Road";       // Hình thức vận tải: Road (Đường bộ xe lồng), Sea (Đường biển), Rail (Đường sắt), Internal (Nội bộ trung chuyển)
    public int TotalVehicleCount { get; set; } = 0;    // Tổng số lượng xe kế hoạch điều độ trong đợt
    public int TotalRealVinCount { get; set; } = 0;    // Tổng số lượng xe đã gán số khung thật RVIN
    public string Status { get; set; } = "Draft";      // Draft → Submitted → Approved → InExecution → Completed (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                // Ghi chú điều hành kế hoạch điều độ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }            // Trưởng phòng Kế hoạch / Giám đốc Logistics duyệt
    public DateTime? ApprovedAt { get; set; }
    public string? ExecutedBy { get; set; }            // Điều phối viên vận tải xuất lệnh
    public DateTime? ExecutedAt { get; set; }
    public string? CompletedBy { get; set; }           // Nghiệm thu hoàn tất toàn bộ kế hoạch
    public DateTime? CompletedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết dòng xe trong Kế hoạch Điều độ Vận tải (BizHTC.Storage / Sto_TranspPlan / TransportPlanLine): thông tin số khung kế hoạch FVIN, số khung thực tế RVIN, dòng xe, phiên bản, màu sắc, kho xuất, đại lý đích, địa bàn (Tỉnh/Huyện đi - Tỉnh/Huyện đến), đơn vị vận tải, ngày QC và ngày dự kiến xuất phát.</summary>
public sealed class TransportPlanLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TransportPlanId { get; set; }
    public string PlanNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string VINPlan { get; set; } = "";          // Mã số khung kế hoạch dự kiến (FVIN, VD: PLN-ACC-001, PLN-TUC-002)
    public string? Vin { get; set; }                   // Mã số khung thực tế (RVIN, được gán từ kho xe InStock)
    public bool FlagRealVin { get; set; } = false;     // Đã gán số khung thực tế hay chưa
    public string Model { get; set; } = "";            // Dòng xe (SantaFe, Tucson, Accent, Creta, Custin, Palisade, Ioniq 5...)
    public string? SpecCode { get; set; }              // Phiên bản xe (1.6T HTRAC, 2.0 AT Tiêu chuẩn, 2.0 AT Đặc biệt...)
    public string? SpecDescription { get; set; }       // Tên diễn giải phiên bản
    public string? ColorCode { get; set; } = "NWAC";   // Mã màu xe (NWAC, T2X, R4R...)
    public string? ColorName { get; set; } = "Trắng ngọc trai";
    public string StorageCode { get; set; } = "PLANT-HTMV1"; // Kho bãi OEM xuất phát
    public string DealerCode { get; set; } = "";       // Đại lý nhận phân bổ (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }            // Tên đại lý
    public string FProvinceCode { get; set; } = "NB";  // Mã tỉnh xuất phát (NB: Ninh Bình, HP: Hải Phòng...)
    public string? FProvinceName { get; set; } = "Ninh Bình";
    public string FDistrictCode { get; set; } = "GV";  // Mã huyện xuất phát (GV: Gia Viễn...)
    public string? FDistrictName { get; set; } = "Gia Viễn";
    public string TProvinceCode { get; set; } = "HN";  // Mã tỉnh đích đến / Đại lý (HN: Hà Nội, HCM: Hồ Chí Minh...)
    public string? TProvinceName { get; set; } = "Hà Nội";
    public string TDistrictCode { get; set; } = "CG";  // Mã huyện đích đến (CG: Cầu Giấy, TX: Thanh Xuân...)
    public string? TDistrictName { get; set; } = "Cầu Giấy";
    public string TransporterCode { get; set; } = "NYK"; // Mã đơn vị vận tải / Nhà xe (NYK, TRACO, VINAFCO, TT_LOGISTICS...)
    public string? TransporterName { get; set; } = "Công ty TNHH Vận tải Hàng hải NYK Việt Nam";
    public string? TruckPlateNo { get; set; }          // Biển số xe lồng chở ô tô (29C-888.99)
    public string? DriverName { get; set; }            // Họ tên lái xe chuyên dụng
    public string? DriverPhone { get; set; }           // SĐT lái xe
    public DateTime? CQStartDate { get; set; }         // Ngày kiểm tra chất lượng KCS/QC xuất xưởng tại nhà máy
    public DateTime ExpectedDate { get; set; } = DateTime.Now.AddDays(2); // Ngày dự kiến vận chuyển xuất bến
    public DateTime? ActualDepartureDate { get; set; } // Ngày thực tế xe lồng xuất bến
    public DateTime? ActualArrivalDate { get; set; }   // Ngày thực tế xe hạ tải tại đại lý
    public string TPStatus { get; set; } = "Pending";  // Trạng thái điều độ: Pending → ApprovedByPlan → ApprovedBySales → DispatchedByLogistics → Finished (hoặc Cancelled)
    public string TransporterStatus { get; set; } = "Pending"; // Trạng thái xác nhận phía nhà xe: Pending → Confirmed → InTransit → Delivered (hoặc Rejected)
    public DateTime? TransporterAppDate { get; set; }  // Ngày đơn vị vận tải xác nhận tiếp nhận
    public string? TransporterAppBy { get; set; }      // Người đại diện nhà xe xác nhận
    public string? TransporterRejectReason { get; set; }
    public string Status { get; set; } = "Pending";    // Trạng thái tổng thể: Pending → Approved → InTransit → Delivered / Completed (hoặc Cancelled)
    public string? Remark { get; set; }
}

// ===== DTOs cho Kế hoạch Điều độ Vận tải (BizHTC.Storage / Sto_TranspPlan / TransportPlan) =====

public sealed record CreateTransportPlanDto(
    string? PlanNo,
    string? PlanNoUser,
    string PlanMonth,
    DateTime? PlanDate,
    string? StorageCode,
    string? StorageName,
    string? TPType,
    string? Remark,
    string? CreatedBy,
    List<CreateTransportPlanLineDto>? Lines
);

public sealed record CreateTransportPlanLineDto(
    string VINPlan,
    string? Vin,
    string Model,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    string? StorageCode,
    string DealerCode,
    string? DealerName,
    string? FProvinceCode,
    string? FProvinceName,
    string? FDistrictCode,
    string? FDistrictName,
    string? TProvinceCode,
    string? TProvinceName,
    string? TDistrictCode,
    string? TDistrictName,
    string? TransporterCode,
    string? TransporterName,
    string? TruckPlateNo,
    string? DriverName,
    string? DriverPhone,
    DateTime? CQStartDate,
    DateTime? ExpectedDate,
    string? Remark
);

public sealed record UpdateTransportPlanDto(
    string? PlanNoUser,
    string? PlanMonth,
    DateTime? PlanDate,
    string? StorageCode,
    string? StorageName,
    string? TPType,
    string? Remark
);

public sealed record AddTransportPlanLineDto(
    string VINPlan,
    string? Vin,
    string Model,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    string? StorageCode,
    string DealerCode,
    string? DealerName,
    string? FProvinceCode,
    string? FProvinceName,
    string? FDistrictCode,
    string? FDistrictName,
    string? TProvinceCode,
    string? TProvinceName,
    string? TDistrictCode,
    string? TDistrictName,
    string? TransporterCode,
    string? TransporterName,
    string? TruckPlateNo,
    string? DriverName,
    string? DriverPhone,
    DateTime? CQStartDate,
    DateTime? ExpectedDate,
    string? Remark
);

public sealed record UpdateTransportPlanLineByKeHoachDto(
    DateTime? CQStartDate,
    DateTime? ExpectedDate,
    string? StorageCode,
    string? Model,
    string? SpecCode,
    string? ColorCode,
    string? ColorName,
    string? Remark
);

public sealed record UpdateTransportPlanLineByBanHangDto(
    string DealerCode,
    string? DealerName,
    string? TPStatus,
    string? Remark
);

public sealed record UpdateTransportPlanLineByLogisticDto(
    string TransporterCode,
    string? TransporterName,
    string? FProvinceCode,
    string? FProvinceName,
    string? FDistrictCode,
    string? FDistrictName,
    string? TProvinceCode,
    string? TProvinceName,
    string? TDistrictCode,
    string? TDistrictName,
    string? TruckPlateNo,
    string? DriverName,
    string? DriverPhone,
    string? TransporterStatus,
    string? Remark
);

public sealed record MapVinRealDto(
    string Vin,
    string? Actor,
    string? Remark
);

public sealed record MapVinRealItemDto(
    string VINPlan,
    string Vin
);

public sealed record MapVinRealBatchDto(
    List<MapVinRealItemDto> Mappings,
    string? Actor
);

public sealed record UnmapVinRealDto(
    string? Actor,
    string? Reason
);

public sealed record TransporterApproveLineDto(
    bool IsAccepted,
    string? Actor,
    string? TruckPlateNo,
    string? DriverName,
    string? DriverPhone,
    DateTime? EstimatedArrivalDate,
    string? RejectReason,
    string? Remark
);

public sealed record TransportPlanTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record TransportPlanSummaryDto(
    int TotalPlans,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalInExecution,
    int TotalCompleted,
    int TotalCancelled,
    int TotalVehicles,
    int TotalMappedVehicles,
    decimal VinMappingRatePercent,
    List<TransportPlanMonthStatsDto> ByMonth,
    List<TransportPlanTransporterStatsDto> ByTransporter,
    List<TransportPlanDealerStatsDto> ByDealer,
    List<TransportPlanModelStatsDto> ByModel
);

public sealed record TransportPlanMonthStatsDto(string PlanMonth, int PlanCount, int VehicleCount, int MappedCount);
public sealed record TransportPlanTransporterStatsDto(string TransporterCode, string TransporterName, int VehicleCount, int CompletedCount);
public sealed record TransportPlanDealerStatsDto(string DealerCode, string DealerName, int VehicleCount, int CompletedCount);
public sealed record TransportPlanModelStatsDto(string Model, int VehicleCount, int MappedCount);

public sealed record VehicleTransportPlanInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? LastTranspPlanNo,
    DateTime? LastTranspPlanDate,
    int TranspPlanCount,
    List<TransportPlanLine> PlanLines
);

// ===== Bảng kê & Quyết toán chi phí Mua sắm/Thuê thiết bị định vị GPS & Dịch vụ SIM 4G data viễn thông theo lô xe VIN (BizHTC.Payment / Pmt_PaymentGPS, Pmt_PaymentGPSDetail / FrmQuanLyThanhToanGPS, FrmTaoThanhToanGPS, FrmTaoThanhToanGPS_AddCar) =====

/// <summary>Bảng kê & Quyết toán chi phí Thiết bị định vị GPS & Dịch vụ SIM 4G data viễn thông theo lô xe VIN (BizHTC.Payment.Pmt_PaymentGPS / GpsPayment): quản lý đối soát và thanh toán chi phí thuê/mua thiết bị GPS và cước phí duy trì SIM 4G viễn thông giám sát hành trình giữa Hãng xe OEM HTV và Đơn vị cung cấp giải pháp/quản lý bãi kho TCMS/Veloca/Viettel/VNPT.</summary>
public sealed class GpsPayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PaymentGPSNo { get; set; } = "";        // Mã bảng kê quyết toán GPS (GPS-202604-001, PMGPS...)
    public string? PaymentGPSNoUser { get; set; }       // Mã số bảng kê do người dùng nhập / tham chiếu nội bộ
    public string PmtMonth { get; set; } = "";          // Kỳ / tháng quyết toán chi phí (YYYY-MM, ví dụ: 2026-04)
    public string SupplierCode { get; set; } = "VELOCA"; // Mã nhà cung cấp thiết bị/dịch vụ GPS (VELOCA, VIETTEL, VNPT, MOBIS...)
    public string? SupplierName { get; set; } = "Công ty Cổ phần Công nghệ Veloca"; // Tên nhà cung cấp giải pháp GPS
    public int TotalVehicleCount { get; set; } = 0;     // Tổng số lượng xe lắp đặt thiết bị GPS trong bảng kê
    public decimal TotalBeforeVAT { get; set; } = 0;    // Tổng chi phí GPS trước thuế VAT (VNĐ)
    public decimal VatRate { get; set; } = 10;          // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;    // Tiền thuế VAT = TotalBeforeVAT * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;       // Tổng số tiền thanh toán đã bao gồm VAT = TotalBeforeVAT + TotalVatAmount
    public string Status { get; set; } = "Draft";       // Draft → Submitted → Approved1 → Approved2 → TCMSSigned → HTVSigned → Settled (hoặc Rejected / Cancelled)
    public string? TCMSSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số Đơn vị GPS/TCMS (Unsigned, Signed)
    public DateTime? TCMSSignDate { get; set; }         // Ngày ký số Đơn vị GPS/TCMS
    public string? TCMSSignBy { get; set; }             // Người đại diện Đơn vị GPS ký số
    public string? HTVSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số Hãng xe OEM HTV (Unsigned, Signed)
    public DateTime? HTVSignDate { get; set; }          // Ngày ký số HTV
    public string? HTVSignBy { get; set; }              // Người đại diện HTV ký số
    public string? BankRefNo { get; set; }              // Số chứng từ / Ủy nhiệm chi UNC ngân hàng giải ngân thanh toán
    public DateTime? PaymentDate { get; set; }          // Ngày thực tế chuyển khoản thanh toán
    public string? FilePath { get; set; }               // Tệp đính kèm bảng kê có chữ ký số (PDF)
    public string? Remark { get; set; }                 // Ghi chú đợt quyết toán
    public string? CreatedBy { get; set; }              // Người lập bảng kê
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Approved1By { get; set; }            // Kế toán chi phí vật tư sơ duyệt A1
    public DateTime? Approved1At { get; set; }
    public string? Approved2By { get; set; }            // Giám đốc Khối Phụ tùng / Khối Logistics duyệt A2
    public DateTime? Approved2At { get; set; }
    public string? SettledBy { get; set; }              // Kế toán trưởng / Thủ quỹ xác nhận giải ngân
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong Bảng kê quyết toán chi phí GPS (BizHTC.Payment.Pmt_PaymentGPSDetail / GpsPaymentLine): số khung VIN, model, mã thiết bị GPS, số SIM 4G, số IMEI, số ngày tính phí kế hoạch/thực tế, ngày khấu trừ, đơn giá thuê/ngày, cước data 4G và thành tiền.</summary>
public sealed class GpsPaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long GpsPaymentId { get; set; }
    public string PaymentGPSNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;             // Thứ tự dòng
    public string Vin { get; set; } = "";               // Số khung VIN xe gắn thiết bị GPS
    public string Model { get; set; } = "";             // Dòng xe (SantaFe, Tucson, Accent, Creta, Elantra, Stargazer, Custin, Ioniq 5...)
    public string? SpecCode { get; set; }               // Phiên bản xe
    public string? EngineNo { get; set; }               // Số máy
    public string? Color { get; set; }                  // Màu sắc
    public string GpsCode { get; set; } = "";           // Mã thiết bị GPS (GPSDvNo / GPSID)
    public string? SimCardNo { get; set; }              // Số SIM 4G data viễn thông
    public string? ImeiNo { get; set; }                 // Số IMEI thiết bị phần cứng
    public DateTime CostGPSStartDate { get; set; } = DateTime.Now; // Ngày bắt đầu tính phí GPS
    public DateTime CostGPSEndDate { get; set; } = DateTime.Now;   // Ngày kết thúc tính phí GPS
    public int PlanCostGPSDate { get; set; } = 30;      // Số ngày kế hoạch tính phí = (CostGPSEndDate - CostGPSStartDate).Days + 1
    public int DeductDate { get; set; } = 0;            // Số ngày khấu trừ không tính phí (bảo hành, đổi thiết bị, lưu kho chưa kích hoạt)
    public int ActualCostGPSDate { get; set; } = 30;    // Số ngày tính phí thực tế = Max(0, PlanCostGPSDate - DeductDate)
    public decimal DailyRate { get; set; } = 15000m;    // Đơn giá thuê thiết bị GPS theo ngày (VNĐ/ngày) (PriceGPS)
    public decimal SimDataFee { get; set; } = 50000m;   // Cước phí data viễn thông 4G theo tháng (VNĐ)
    public decimal AmountGPS { get; set; } = 500000m;   // Tổng chi phí GPS của xe = (ActualCostGPSDate * DailyRate) + SimDataFee (VNĐ)
    public string? ContractGPS { get; set; }            // Số hợp đồng dịch vụ / gói cước GPS tham chiếu
    public DateTime? InStorageDate { get; set; }        // Ngày xe nhập kho bãi / xuất xưởng
    public string Status { get; set; } = "Pending";     // Pending → Approved → Settled (hoặc Cancelled)
    public string? Remark { get; set; }                 // Ghi chú kỹ thuật chi tiết
}

// ===== DTOs cho Bảng kê & Quyết toán chi phí Thiết bị GPS & SIM 4G (BizHTC.Payment / Pmt_PaymentGPS & GpsPayment) =====

public sealed record CreateGpsPaymentDto(
    string? PaymentGPSNo,
    string? PaymentGPSNoUser,
    string PmtMonth,
    string? SupplierCode,
    string? SupplierName,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<GpsPaymentLineInputDto>? Items
);

public sealed record GpsPaymentLineInputDto(
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? GpsCode,
    string? SimCardNo,
    string? ImeiNo,
    DateTime? CostGPSStartDate,
    DateTime? CostGPSEndDate,
    int? DeductDate,
    decimal? DailyRate,
    decimal? SimDataFee,
    string? ContractGPS,
    DateTime? InStorageDate,
    string? Remark
);

public sealed record UpdateGpsPaymentHeaderDto(
    string? PaymentGPSNoUser,
    string? PmtMonth,
    string? SupplierCode,
    string? SupplierName,
    decimal? VatRate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath,
    string? Remark
);

public sealed record UpdateGpsPaymentLineDto(
    string? Model,
    string? SpecCode,
    string? GpsCode,
    string? SimCardNo,
    string? ImeiNo,
    DateTime? CostGPSStartDate,
    DateTime? CostGPSEndDate,
    int? DeductDate,
    decimal? DailyRate,
    decimal? SimDataFee,
    string? ContractGPS,
    DateTime? InStorageDate,
    string? Status,
    string? Remark
);

public sealed record GpsPaymentTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath
);

public sealed record GpsPaymentSummaryDto(
    int TotalPayments,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalSigned,
    int TotalSettled,
    int TotalCancelled,
    int TotalVehicles,
    decimal TotalBeforeVAT,
    decimal TotalVatAmount,
    decimal TotalAmount,
    decimal TotalSettledAmount,
    List<GpsPaymentSupplierStatsDto> BySupplier,
    List<GpsPaymentMonthStatsDto> ByMonth
);

public sealed record GpsPaymentSupplierStatsDto(string SupplierCode, string SupplierName, int PaymentCount, int VehicleCount, decimal TotalAmount, decimal SettledAmount);
public sealed record GpsPaymentMonthStatsDto(string PmtMonth, int PaymentCount, int VehicleCount, decimal TotalAmount, decimal SettledAmount);

public sealed record VehicleGpsPaymentInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    bool IsGpsInstalled,
    string? GpsCode,
    bool IsGpsPaid,
    decimal GpsPaidAmount,
    string? LastGpsPaymentNo,
    DateTime? LastGpsPaymentDate,
    int GpsPaymentCount,
    List<GpsPaymentLine> PaymentLines
);

/// <summary>Bảng kê &amp; Quyết toán chi phí Vận tải &amp; Bảo hiểm xe ô tô vận chuyển theo lô VIN (BizHTC.Payment.Pmt_TransportIns / TransportInsurancePayment): quản lý thanh toán cước vận tải đường bộ giữa OEM HTV và Đơn vị vận chuyển (NYK, Traco, Vinafco...) kết hợp phí bảo hiểm hàng hóa vận chuyển (Bảo Việt, PVI, PTI...), đối soát phạt trễ hạn giao xe, phê duyệt 4 cấp và bù trừ công nợ.</summary>
public sealed class TransportInsurancePayment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TransportInsNo { get; set; } = "";             // Mã bảng kê quyết toán vận tải & bảo hiểm (TIP202603-0001, TIP...)
    public string? TransportInsNoUser { get; set; }            // Số hiệu bảng kê tham chiếu nội bộ (BK-VT-BH/2026/03/NYK-01)
    public string PmtMonth { get; set; } = "";                 // Kỳ quyết toán (YYYY-MM, ví dụ: 2026-03)
    public string TransporterCode { get; set; } = "NYK";       // Mã đơn vị vận tải (NYK, TRACO, VINAFCO, TT_LOGISTICS...)
    public string? TransporterName { get; set; } = "Công ty TNHH Vận tải Hàng hải NYK Việt Nam";
    public string InsuranceCompanyCode { get; set; } = "BAOVIET"; // Mã công ty bảo hiểm (BAOVIET, PVI, PTI, BIC, MIC...)
    public string? InsuranceCompanyName { get; set; } = "Tổng Công ty Bảo hiểm Bảo Việt";
    public int TotalVehicleCount { get; set; } = 0;            // Tổng số lượng xe trong đợt quyết toán
    public decimal TotalFreightAmount { get; set; } = 0;       // Tổng cước phí vận chuyển trước thuế (VNĐ)
    public decimal TotalDelayPenalty { get; set; } = 0;        // Tổng tiền phạt chậm trễ giao hàng (VNĐ)
    public decimal TotalInsuranceFee { get; set; } = 0;        // Tổng phí bảo hiểm hàng hóa vận chuyển (VNĐ)
    public decimal TotalBeforeVAT { get; set; } = 0;           // Tổng chi phí trước thuế VAT = TotalFreightAmount - TotalDelayPenalty + TotalInsuranceFee (VNĐ)
    public decimal VatRate { get; set; } = 10;                 // Thuế suất VAT (%) (VD: 10% = 10)
    public decimal TotalVatAmount { get; set; } = 0;           // Tiền thuế VAT (VNĐ) = TotalBeforeVAT * VatRate / 100
    public decimal TotalAmount { get; set; } = 0;              // Tổng số tiền thanh toán sau thuế = TotalBeforeVAT + TotalVatAmount
    public string Status { get; set; } = "Draft";              // Draft → Submitted → Approved1 → Approved2 → TransporterSigned → HTVSigned → Settled (hoặc Rejected / Cancelled)
    public string? TransporterSignStatus { get; set; } = "Unsigned"; // Trạng thái ký số đơn vị vận tải (Unsigned, Signed)
    public DateTime? TransporterSignDate { get; set; }         // Ngày ký số đơn vị vận tải
    public string? TransporterSignBy { get; set; }             // Người đại diện đơn vị vận tải ký số
    public string? HTVSignStatus { get; set; } = "Unsigned";   // Trạng thái ký số Hãng xe HTV (Unsigned, Signed)
    public DateTime? HTVSignDate { get; set; }                 // Ngày ký số HTV
    public string? HTVSignBy { get; set; }                     // Người đại diện HTV ký số
    public string? BankRefNo { get; set; }                     // Số chứng từ / Ủy nhiệm chi UNC ngân hàng giải ngân thanh toán
    public DateTime? PaymentDate { get; set; }                 // Ngày thực tế chuyển khoản thanh toán
    public string? FilePath { get; set; }                      // Tệp đính kèm bảng kê có chữ ký số (PDF)
    public string? Remark { get; set; }                        // Ghi chú đợt quyết toán
    public string? CreatedBy { get; set; }                     // Người lập bảng kê
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Approved1By { get; set; }                   // Kế toán chi phí / Chuyên viên Logistics sơ duyệt A1
    public DateTime? Approved1At { get; set; }
    public string? Approved2By { get; set; }                   // Giám đốc Khối Logistics / Giám đốc Tài chính duyệt A2
    public DateTime? Approved2At { get; set; }
    public string? SettledBy { get; set; }                     // Kế toán trưởng / Thủ quỹ xác nhận giải ngân
    public DateTime? SettledAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong Bảng kê quyết toán vận tải &amp; bảo hiểm (BizHTC.Payment.Pmt_TransportInsDetail / TransportInsurancePaymentLine): số khung VIN, model, chặng vận chuyển, ngày xuất/đến, ngày trễ hạn, cước vận tải, phạt trễ, giá trị định giá, phí bảo hiểm và tổng chi phí.</summary>
public sealed class TransportInsurancePaymentLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TransportInsurancePaymentId { get; set; }
    public string TransportInsNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;                    // Thứ tự dòng
    public string Vin { get; set; } = "";                      // Số khung VIN xe vận chuyển
    public string Model { get; set; } = "";                    // Dòng xe (SantaFe, Tucson, Accent, Creta, Custin, Palisade, Grand i10, Venue, Ioniq 5...)
    public string? SpecCode { get; set; }                      // Phiên bản xe
    public string? EngineNo { get; set; }                      // Số máy
    public string? Color { get; set; }                         // Màu sắc
    public string? FStorageCode { get; set; } = "PLANT-HTMV1"; // Kho bãi xuất phát (PLANT-HTMV1, PLANT-HTMV2, PORT_HP, TCV_YARD)
    public string? FProvinceName { get; set; } = "Ninh Bình";   // Tỉnh xuất phát
    public string? TStorageCode { get; set; }                  // Kho / Đại lý nhận xe (DLR-HN01, DLR-HCM01, DLR-DN01...)
    public string? TProvinceName { get; set; } = "Hà Nội";     // Tỉnh đích đến
    public string? DealerCode { get; set; }                    // Mã đại lý nhận phân bổ
    public DateTime DlvStartDate { get; set; } = DateTime.Now.AddDays(-5); // Ngày bắt đầu vận chuyển xuất bến
    public int ExpectedDays { get; set; } = 2;                 // Số ngày định mức vận tải theo thỏa thuận SLA tuyến
    public DateTime ExpectedDlvEndDate { get; set; } = DateTime.Now.AddDays(-3); // Hạn giao xe dự kiến
    public DateTime DlvEndDate { get; set; } = DateTime.Now.AddDays(-3); // Ngày thực tế hạ tải nhận xe tại đại lý
    public int DelayDate { get; set; } = 0;                    // Số ngày chậm trễ giao hàng = Max(0, (DlvEndDate.Date - ExpectedDlvEndDate.Date).Days)
    public decimal FreightAmount { get; set; } = 2500000m;     // Cước vận chuyển xe lồng đường bộ theo chặng (VNĐ) (TFValReal)
    public decimal PenaltyPerDay { get; set; } = 100000m;      // Định mức phạt trễ hạn/ngày (VNĐ/ngày)
    public decimal DelayPenalty { get; set; } = 0;             // Tiền phạt trễ hạn giao xe = DelayDate * PenaltyPerDay (TPValReal)
    public decimal CarValue { get; set; } = 550000000m;        // Giá trị định giá xe mua bảo hiểm (VNĐ) (PriceCar)
    public decimal InsuranceRate { get; set; } = 0.05m;        // Tỷ lệ phí bảo hiểm (%) (VD: 0.05% = 0.05)
    public decimal InsuranceFee { get; set; } = 275000m;       // Tiền phí bảo hiểm = CarValue * InsuranceRate / 100 (VNĐ)
    public decimal TotalAmount { get; set; } = 2775000m;       // Tổng tiền quyết toán của xe = FreightAmount - DelayPenalty + InsuranceFee (Val_Transport)
    public string? DlvMnNo { get; set; }                       // Mã biên bản giao nhận bàn giao xe (Sto_DlvMinutes)
    public string? TranspReqType { get; set; } = "OEMToDealer"; // Loại hình vận chuyển: OEMToDealer, InterDealer, PlantToPort, ReturnRetrieve
    public string Status { get; set; } = "Pending";            // Pending → Approved → Settled (hoặc Cancelled)
    public string? StandardRemark { get; set; }                // Ghi chú tuyến tiêu chuẩn
    public string? Remark { get; set; }                        // Ghi chú chi tiết dòng xe
}

// ===== DTOs cho Bảng kê & Quyết toán chi phí Vận tải & Bảo hiểm (BizHTC.Payment / Pmt_TransportIns & TransportInsurancePayment) =====

public sealed record CreateTransportInsurancePaymentDto(
    string? TransportInsNo,
    string? TransportInsNoUser,
    string PmtMonth,
    string? TransporterCode,
    string? TransporterName,
    string? InsuranceCompanyCode,
    string? InsuranceCompanyName,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<TransportInsurancePaymentLineInputDto>? Items
);

public sealed record TransportInsurancePaymentLineInputDto(
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? FStorageCode,
    string? FProvinceName,
    string? TStorageCode,
    string? TProvinceName,
    string? DealerCode,
    DateTime? DlvStartDate,
    int? ExpectedDays,
    DateTime? ExpectedDlvEndDate,
    DateTime? DlvEndDate,
    decimal? FreightAmount,
    decimal? PenaltyPerDay,
    decimal? CarValue,
    decimal? InsuranceRate,
    string? DlvMnNo,
    string? TranspReqType,
    string? StandardRemark,
    string? Remark
);

public sealed record UpdateTransportInsurancePaymentHeaderDto(
    string? TransportInsNoUser,
    string? PmtMonth,
    string? TransporterCode,
    string? TransporterName,
    string? InsuranceCompanyCode,
    string? InsuranceCompanyName,
    decimal? VatRate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath,
    string? Remark
);

public sealed record UpdateTransportInsurancePaymentLineDto(
    string? Model,
    string? SpecCode,
    string? FStorageCode,
    string? FProvinceName,
    string? TStorageCode,
    string? TProvinceName,
    string? DealerCode,
    DateTime? DlvStartDate,
    int? ExpectedDays,
    DateTime? ExpectedDlvEndDate,
    DateTime? DlvEndDate,
    int? DelayDate,
    decimal? FreightAmount,
    decimal? PenaltyPerDay,
    decimal? DelayPenalty,
    decimal? CarValue,
    decimal? InsuranceRate,
    decimal? InsuranceFee,
    string? DlvMnNo,
    string? TranspReqType,
    string? Status,
    string? StandardRemark,
    string? Remark
);

public sealed record TransportInsurancePaymentTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate,
    string? BankRefNo,
    DateTime? PaymentDate,
    string? FilePath
);

public sealed record TransportInsurancePaymentSummaryDto(
    int TotalPayments,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalSigned,
    int TotalSettled,
    int TotalCancelled,
    int TotalVehicles,
    decimal TotalFreightAmount,
    decimal TotalDelayPenalty,
    decimal TotalInsuranceFee,
    decimal TotalBeforeVAT,
    decimal TotalVatAmount,
    decimal TotalAmount,
    decimal TotalSettledAmount,
    List<TransportInsuranceTransporterStatsDto> ByTransporter,
    List<TransportInsuranceCompanyStatsDto> ByInsuranceCompany,
    List<TransportInsuranceMonthStatsDto> ByMonth
);

public sealed record TransportInsuranceTransporterStatsDto(string TransporterCode, string TransporterName, int PaymentCount, int VehicleCount, decimal TotalFreight, decimal TotalPenalty, decimal TotalAmount, decimal SettledAmount);
public sealed record TransportInsuranceCompanyStatsDto(string InsuranceCompanyCode, string InsuranceCompanyName, int PaymentCount, int VehicleCount, decimal TotalInsuranceFee, decimal TotalAmount, decimal SettledAmount);
public sealed record TransportInsuranceMonthStatsDto(string PmtMonth, int PaymentCount, int VehicleCount, decimal TotalFreight, decimal TotalPenalty, decimal TotalInsuranceFee, decimal TotalAmount, decimal SettledAmount);

public sealed record VehicleTranspInsPaymentInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    bool IsTranspInsPaid,
    decimal TranspInsPaidAmount,
    string? LastTranspInsPaymentNo,
    DateTime? LastTranspInsPaymentDate,
    int TranspInsPaymentCount,
    List<TransportInsurancePaymentLine> PaymentLines
);

// ===== Quản lý Định mức Tồn kho An toàn & Cân đối Tồn kho Đại lý OEM (BizHTC.MasterData & BizHTC.StorageFG / Mst_DealerInventoryThreshold, Mst_MinInventory, St_MinInvBalance / FrmMstSalesInventoryThreshold, FrmSt_MinInvBalance, FrmReportMinInventory) =====

/// <summary>Thiết lập Định mức / Hạn mức Tồn kho An toàn Xe ô tô cho Đại lý (BizHTC.MasterData.Mst_DealerInventoryThreshold / DealerInventoryThreshold): quản lý định mức tồn kho tối thiểu sàn (Safety Stock), định mức tồn kho mục tiêu (Target Stock) và hạn mức tồn kho tối đa trần (Ceiling Stock) theo từng Đại lý, Dòng xe (Model) / Phiên bản (Spec), kỳ tháng/năm áp dụng và biên độ cảnh báo bán hàng.</summary>
public sealed class DealerInventoryThreshold
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ThresholdNo { get; set; } = "";             // Mã thiết lập định mức tồn kho (TH202605-0001, DIT...)
    public string? ThresholdNoUser { get; set; }            // Số hiệu văn bản / quyết định ban hành định mức (QĐ-ĐMTK/2026/05-01)
    public string DealerCode { get; set; } = "";            // Mã đại lý áp dụng định mức (DLR-HN01, DLR-HN02, DLR-HCM01...)
    public string? DealerName { get; set; }                 // Tên đại lý
    public string? RegionCode { get; set; } = "MienBac";    // Khu vực địa lý: MienBac, MienTrung, MienNam, TayNguyen
    public string Model { get; set; } = "";                 // Dòng xe áp dụng (SantaFe, Tucson, Accent, Creta, Custin, Grand i10, Venue, Ioniq 5...)
    public string? SpecCode { get; set; }                   // Phiên bản cấu hình chi tiết (1.5 AT Tiêu Chuẩn, 1.6 Turbo...)
    public int PeriodMonth { get; set; } = DateTime.Now.Month; // Tháng áp dụng định mức (1 - 12)
    public int PeriodYear { get; set; } = DateTime.Now.Year;   // Năm áp dụng định mức (2026...)
    public int MinInvQty { get; set; } = 5;                 // Định mức tồn kho tối thiểu sàn (Safety Stock MinQty)
    public int TargetInvQty { get; set; } = 10;             // Định mức tồn kho mục tiêu (Target Stock Qty)
    public int MaxInvQty { get; set; } = 25;                // Định mức tồn kho tối đa trần (Ceiling Stock MaxQty)
    public decimal WarningThresholdPercent { get; set; } = 20.0m; // Ngưỡng cảnh báo bán hàng / biên độ nguy cấp (%) (SalesThresholdPercent / NguongBH)
    public decimal DailySalesRate { get; set; } = 0.5m;     // Tốc độ bán hàng trung bình ngày (xe/ngày) làm cơ sở tính Days of Supply
    public DateTime? EffectiveFrom { get; set; }            // Ngày bắt đầu có hiệu lực
    public DateTime? EffectiveTo { get; set; }              // Ngày hết hạn hiệu lực
    public string Status { get; set; } = "Draft";           // Draft → Active → Expired (hoặc Suspended / Cancelled)
    public string? Remark { get; set; }                     // Ghi chú căn cứ thiết lập định mức tồn kho
    public string? CreatedBy { get; set; }                  // Người lập thiết lập định mức
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                 // Trưởng phòng Kế hoạch / Giám đốc Bán hàng OEM duyệt ban hành
    public DateTime? ApprovedAt { get; set; }
    public string? SuspendedBy { get; set; }                // Người tạm dừng áp dụng định mức
    public DateTime? SuspendedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Nhật ký Kiểm kê &amp; Cân đối Sức khỏe Tồn kho Đại lý theo Thời điểm (BizHTC.StorageFG.St_MinInvBalance / InventoryAuditRecord): ghi nhận kết quả đối soát số lượng tồn kho thực tế (InStock, Allocated, InTransit) với định mức tồn an toàn, chênh lệch tồn kho Variance, số ngày bán hàng dự trữ DOS, xếp loại sức khỏe và hành động điều chuyển đề xuất.</summary>
public sealed class InventoryAuditRecord
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string AuditNo { get; set; } = "";               // Mã đợt kiểm kê cân đối tồn kho (AUD202605-0001, AUD...)
    public long? ThresholdId { get; set; }                  // Liên kết định mức tồn kho đang áp dụng
    public string? ThresholdNo { get; set; }
    public string DealerCode { get; set; } = "";            // Mã đại lý được kiểm kê
    public string? DealerName { get; set; }
    public string? RegionCode { get; set; }
    public string Model { get; set; } = "";                 // Dòng xe
    public string? SpecCode { get; set; }                   // Phiên bản xe
    public int MinInvQty { get; set; } = 5;                 // Định mức tồn kho tối thiểu
    public int TargetInvQty { get; set; } = 10;             // Định mức tồn kho mục tiêu
    public int MaxInvQty { get; set; } = 25;                // Định mức tồn kho tối đa
    public int InStockCount { get; set; } = 0;              // Số lượng xe thực tế trong kho đại lý
    public int AllocatedCount { get; set; } = 0;            // Số lượng xe đã phân bổ chờ giao cho đại lý
    public int InTransitCount { get; set; } = 0;            // Số lượng xe đang trên đường vận chuyển tới đại lý
    public int TotalOnHand { get; set; } = 0;               // Tổng tồn thực tế sẵn sàng = InStockCount + AllocatedCount + InTransitCount
    public int VarianceQty { get; set; } = 0;               // Chênh lệch so với định mức tối thiểu = TotalOnHand - MinInvQty
    public decimal StockFulfillmentRate { get; set; } = 0;  // Tỷ lệ đáp ứng định mức tồn (%) = (TotalOnHand / MinInvQty) * 100
    public decimal DaysOfSupply { get; set; } = 0;          // Số ngày bán hàng dự trữ (DOS) = TotalOnHand / DailySalesRate
    public string HealthStatus { get; set; } = "Optimal";   // Trạng thái sức khỏe tồn kho: OutOfStock, CriticalShortage, Shortage, Optimal, Surplus
    public string? RebalanceAction { get; set; }            // Đề xuất điều phối: NoAction, UrgentOrder, TransferIn, TransferOut, RestockFromPlant
    public string? RecommendedTransferDealer { get; set; }  // Đại lý đối ứng đề xuất điều chuyển nhận hoặc chuyển xe
    public int RecommendedTransferQty { get; set; } = 0;    // Số lượng xe đề xuất điều chuyển
    public DateTime AuditDate { get; set; } = DateTime.Now; // Thời điểm thực hiện kiểm kê đối soát
    public string? AuditedBy { get; set; }                  // Chuyên viên điều phối / Hệ thống tự động kiểm kê
    public string? Remark { get; set; }                     // Nhận xét & khuyến nghị điều hành tồn kho
}

// ===== DTOs cho Quản lý Định mức Tồn kho An toàn & Cân đối Kho Đại lý (DealerInventoryThreshold) =====

public sealed record CreateDealerInventoryThresholdDto(
    string? ThresholdNo,
    string? ThresholdNoUser,
    string DealerCode,
    string? DealerName,
    string? RegionCode,
    string Model,
    string? SpecCode,
    int? PeriodMonth,
    int? PeriodYear,
    int? MinInvQty,
    int? TargetInvQty,
    int? MaxInvQty,
    decimal? WarningThresholdPercent,
    decimal? DailySalesRate,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Remark,
    string? CreatedBy
);

public sealed record BatchCreateDealerInventoryThresholdItemDto(
    string DealerCode,
    string? DealerName,
    string? RegionCode,
    string Model,
    string? SpecCode,
    int? MinInvQty,
    int? TargetInvQty,
    int? MaxInvQty,
    decimal? WarningThresholdPercent,
    decimal? DailySalesRate,
    string? Remark
);

public sealed record BatchCreateDealerInventoryThresholdDto(
    int? PeriodMonth,
    int? PeriodYear,
    string? ThresholdNoUser,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? CreatedBy,
    List<BatchCreateDealerInventoryThresholdItemDto> Items
);

public sealed record UpdateDealerInventoryThresholdDto(
    string? ThresholdNoUser,
    string? DealerName,
    string? RegionCode,
    string? Model,
    string? SpecCode,
    int? PeriodMonth,
    int? PeriodYear,
    int? MinInvQty,
    int? TargetInvQty,
    int? MaxInvQty,
    decimal? WarningThresholdPercent,
    decimal? DailySalesRate,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    string? Remark
);

public sealed record DealerInventoryThresholdTransitionDto(
    string? Actor,
    string? Note,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record RunInventoryAuditDto(
    string? DealerCode,
    string? Model,
    string? RegionCode,
    string? AuditedBy,
    string? Remark
);

public sealed record DealerStockHealthDto(
    string DealerCode,
    string DealerName,
    string RegionCode,
    string Model,
    string? SpecCode,
    int MinInvQty,
    int TargetInvQty,
    int MaxInvQty,
    int InStockCount,
    int AllocatedCount,
    int InTransitCount,
    int TotalOnHand,
    int VarianceQty,
    decimal StockFulfillmentRate,
    decimal DaysOfSupply,
    string HealthStatus,
    string RebalanceAction,
    string? RecommendedTransferDealer,
    int RecommendedTransferQty,
    string? ThresholdNo,
    DateTime? LastAuditDate
);

public sealed record DealerRebalanceSuggestionDto(
    string Model,
    string SourceDealerCode,
    string SourceDealerName,
    int SourceTotalOnHand,
    int SourceMaxInvQty,
    int SourceSurplusQty,
    string TargetDealerCode,
    string TargetDealerName,
    int TargetTotalOnHand,
    int TargetMinInvQty,
    int TargetDeficitQty,
    int SuggestedTransferQty,
    string RecommendationReason
);

public sealed record DealerInventoryThresholdSummaryDto(
    int TotalThresholdRules,
    int TotalActiveRules,
    int TotalDraftRules,
    int TotalExpiredRules,
    int TotalSuspendedRules,
    int TotalAuditedItems,
    int TotalOptimalItems,
    int TotalShortageItems,
    int TotalCriticalShortageItems,
    int TotalSurplusItems,
    int TotalOutOfStockItems,
    int TotalOnHandVehicles,
    decimal AverageFulfillmentRate,
    List<ThresholdByDealerStatsDto> ByDealer,
    List<ThresholdByModelStatsDto> ByModel,
    List<ThresholdByRegionStatsDto> ByRegion
);

public sealed record ThresholdByDealerStatsDto(
    string DealerCode,
    string DealerName,
    string RegionCode,
    int RuleCount,
    int TotalOnHand,
    int TotalMinQty,
    int TotalTargetQty,
    int TotalMaxQty,
    int ShortageCount,
    int SurplusCount,
    int OptimalCount,
    decimal AverageFulfillmentRate
);

public sealed record ThresholdByModelStatsDto(
    string Model,
    int RuleCount,
    int TotalOnHand,
    int TotalMinQty,
    int TotalTargetQty,
    int TotalMaxQty,
    int ShortageCount,
    int SurplusCount,
    int OptimalCount,
    decimal AverageFulfillmentRate
);

public sealed record ThresholdByRegionStatsDto(
    string RegionCode,
    int RuleCount,
    int TotalOnHand,
    int TotalMinQty,
    int TotalTargetQty,
    int TotalMaxQty,
    int ShortageCount,
    int SurplusCount,
    int OptimalCount,
    decimal AverageFulfillmentRate
);

public sealed record VehicleInventoryThresholdInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    DateTime? LastInventoryAuditDate,
    string? InventoryAlertStatus,
    string? LastThresholdNo,
    int ThresholdAuditCount,
    List<DealerInventoryThreshold> ApplicableThresholds,
    List<InventoryAuditRecord> RecentAuditRecords
);









// ===== Quản lý Khóa Đào tạo, Sát hạch & Cấp Chứng chỉ Chuẩn hóa Nhân sự Đại lý (BizHTC.MasterData / Mst_Training, Mst_TrainingDtl, Mst_SalesManCertificate) =====

/// <summary>Khóa Đào tạo Chuẩn hóa Nhân sự Bán hàng & Dịch vụ Đại lý (BizHTC.MasterData / Mst_Training / TrainingCourse): quản lý các khóa đào tạo nâng cao nghiệp vụ (Sản phẩm mới NewProduct, Tư vấn bán hàng TVBH, Cố vấn dịch vụ CVDV, Kỹ thuật viên KTV Master/EV, Chăm sóc khách hàng & Trải nghiệm lái thử) do Hãng xe OEM HTV tổ chức.</summary>
public sealed class TrainingCourse
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TrainingCode { get; set; } = "";             // Mã khóa đào tạo (TRN-2026-001, TRN...)
    public string? TrainingCodeUser { get; set; }            // Mã khóa nội bộ tham chiếu
    public string CourseName { get; set; } = "";              // Tên khóa đào tạo
    public string TrainingType { get; set; } = "SalesConsultant"; // Loại khóa học: SalesConsultant (Tư vấn bán hàng TVBH), ServiceAdvisor (Cố vấn dịch vụ CVDV), Technical (Kỹ thuật viên xưởng KTV), EVTechnician (KTV Chuyên gia Xe điện EV), NewProduct (Sản phẩm & Tính năng xe mới), CustomerExperience (Trải nghiệm khách hàng & Lái thử), Management (Kỹ năng Quản lý Đại lý)
    public string Level { get; set; } = "Intermediate";       // Cấp độ: Basic (Cơ bản), Intermediate (Trung cấp), Advanced (Nâng cao), Master (Chuyên gia)
    public string Format { get; set; } = "OfflineInClass";    // Hình thức: OfflineInClass (Trực tiếp tập trung tại HTV Training Center), Online (Trực tuyến E-Learning / Zoom), PracticalWorkshop (Thực hành xưởng / Thực chiến), RoadshowField (Đào tạo thực địa tại Đại lý)
    public string? TrainerName { get; set; }                  // Giảng viên / Chuyên gia đào tạo phụ trách
    public string? Location { get; set; }                     // Địa điểm tổ chức (Trung tâm Đào tạo HTV Ninh Bình, Showroom Đào tạo Hà Nội, Showroom Đào tạo TP.HCM...)
    public DateTime StartDate { get; set; } = DateTime.Now;   // Ngày bắt đầu khóa học
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(3); // Ngày kết thúc khóa học
    public int MaxCapacity { get; set; } = 30;                // Số lượng học viên tối đa
    public int TotalEnrolled { get; set; } = 0;               // Tổng số học viên đã đăng ký tham gia
    public int TotalPassed { get; set; } = 0;                 // Số học viên sát hạch Đạt chuẩn (Pass/Good/Excellent)
    public int TotalFailed { get; set; } = 0;                 // Số học viên Chưa đạt (Fail)
    public decimal PassingScore { get; set; } = 70.0m;        // Điểm sàn sát hạch tối thiểu để đạt chuẩn (thang 100 điểm)
    public decimal BudgetAmount { get; set; } = 0;            // Ngân sách dự toán cho khóa đào tạo (VNĐ)
    public decimal ActualCost { get; set; } = 0;              // Chi phí thực tế tổ chức khóa học (VNĐ)
    public string Status { get; set; } = "Draft";             // Draft → Scheduled → InProgress → Completed (hoặc Cancelled)
    public string? Remark { get; set; }                       // Ghi chú / Yêu cầu đầu vào khóa học
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                   // Trưởng ban Đào tạo / Giám đốc Nhân sự duyệt kế hoạch khóa học
    public DateTime? ApprovedAt { get; set; }
    public string? CompletedBy { get; set; }                  // Người nghiệm thu / tổng kết khóa đào tạo
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết Học viên & Kết quả Sát hạch Khóa Đào tạo (BizHTC.MasterData / Mst_TrainingDtl / TrainingEnrollment): danh sách nhân sự đại lý tham gia khóa học, tỷ lệ điểm danh, điểm thi lý thuyết, điểm thực hành, điểm tổng kết, xếp loại sát hạch và trạng thái cấp chứng chỉ.</summary>
public sealed class TrainingEnrollment
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long TrainingCourseId { get; set; }
    public string TrainingCode { get; set; } = "";
    public string EnrollmentNo { get; set; } = "";            // Mã lượt đăng ký học viên (ENR-2026-0001...)
    public int LineIndex { get; set; } = 1;
    public string DealerCode { get; set; } = "";              // Mã đại lý cử nhân sự đi học (DLR-HN01...)
    public string? DealerName { get; set; }                   // Tên đại lý
    public string StaffCode { get; set; } = "";               // Mã nhân viên / học viên (TVBH-01, CVDV-02, KTV-03...)
    public string StaffName { get; set; } = "";               // Họ và tên học viên
    public string? StaffEmail { get; set; }                   // Email học viên
    public string? StaffPhone { get; set; }                   // SĐT học viên
    public string Position { get; set; } = "SalesConsultant"; // Vị trí / chức danh: SalesConsultant (Tư vấn bán hàng TVBH), ServiceAdvisor (Cố vấn dịch vụ CVDV), Technician (Kỹ thuật viên KTV), SalesManager (Trưởng phòng Bán hàng), ServiceManager (Trưởng phòng Dịch vụ), CustomerCare (Chuyên viên CSKH)
    public decimal AttendancePercent { get; set; } = 100;     // Tỷ lệ tham gia buổi học (%)
    public decimal TheoryScore { get; set; } = 0;             // Điểm thi trắc nghiệm lý thuyết (0-100)
    public decimal PracticeScore { get; set; } = 0;           // Điểm thi thực hành / đóng vai tình huống (0-100)
    public decimal FinalScore { get; set; } = 0;              // Điểm tổng kết = TheoryScore * 0.4 + PracticeScore * 0.6
    public string EvaluationGrade { get; set; } = "Pending";  // Xếp loại: Pending, Fail (<70), Pass (70-79), Good (80-89), Excellent (90-100)
    public string ResultStatus { get; set; } = "Registered";  // Trạng thái kết quả: Registered (Đã ghi danh), Attended (Đã tham gia học), Passed (Đạt chuẩn sát hạch), Failed (Không đạt), Dropped (Bỏ học / Vắng mặt)
    public bool IsCertificateIssued { get; set; } = false;    // Đã được cấp chứng chỉ chuẩn hóa
    public string? CertificateNo { get; set; }                // Mã chứng chỉ được cấp (CERT-...)
    public DateTime? CertificateIssueDate { get; set; }       // Ngày cấp chứng chỉ
    public string Status { get; set; } = "Pending";           // Pending → InTraining → Evaluated → Certified (hoặc Cancelled)
    public string? Remark { get; set; }                       // Nhận xét chi tiết của Giảng viên về học viên
}

/// <summary>Hồ sơ Chứng chỉ Chuẩn hóa Chức danh Nhân sự Đại lý Ô tô OEM (BizHTC.MasterData / Mst_SalesManCertificate / StaffCertificate): chứng chỉ hành nghề chuẩn hãng Hyundai cấp cho TVBH, CVDV, KTV khi hoàn thành đạt yêu cầu khóa đào tạo, dùng để đối soát tính hợp lệ khi bán hàng và làm dịch vụ.</summary>
public sealed class StaffCertificate
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CertificateNo { get; set; } = "";           // Mã số chứng chỉ (CERT-2026-TVBH-0001, CERT-2026-CVDV-0001...)
    public string? CertificateNoUser { get; set; }          // Số hiệu chứng chỉ in trên văn bằng giấy
    public string StaffCode { get; set; } = "";               // Mã nhân viên được cấp chứng chỉ
    public string StaffName { get; set; } = "";               // Họ và tên nhân viên
    public string? StaffEmail { get; set; }
    public string? StaffPhone { get; set; }
    public string DealerCode { get; set; } = "";              // Đại lý công tác hiện tại
    public string? DealerName { get; set; }
    public string Position { get; set; } = "SalesConsultant"; // Vị trí chức danh
    public string CertificateType { get; set; } = "SalesConsultant"; // Loại chứng chỉ: SalesConsultant (Tư vấn bán hàng chuẩn Hyundai), ServiceAdvisor (Cố vấn dịch vụ chuẩn), MasterTechnician (KTV Bậc cao Master), EVTechnician (Chuyên gia Kỹ thuật Xe điện EV), CustomerCareSpecialist (Chuyên viên CSKH chuẩn hãng)
    public string Level { get; set; } = "Certified";          // Hạng chứng chỉ: Certified (Đạt chuẩn), Silver (Bạc), Gold (Vàng), Platinum (Bạch kim), Master (Bậc thầy)
    public DateTime IssueDate { get; set; } = DateTime.Now;   // Ngày cấp chứng chỉ
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(2); // Ngày hết hạn hiệu lực (mặc định 2 năm = 730 ngày)
    public string IssuedBy { get; set; } = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)"; // Đơn vị cấp chứng chỉ
    public string Status { get; set; } = "Active";            // Active (Đang có hiệu lực), Expired (Đã hết hạn), Suspended (Tạm đình chỉ), Revoked (Bị thu hồi do vi phạm quy chế)
    public string? LinkedTrainingCode { get; set; }           // Mã khóa đào tạo hoàn thành để được cấp chứng chỉ
    public string? LinkedEnrollmentNo { get; set; }           // Mã lượt học viên sát hạch tương ứng
    public decimal ScoreAchieved { get; set; } = 0;           // Điểm thi đạt được khi sát hạch
    public string? Grade { get; set; }                        // Xếp loại tốt nghiệp (Excellent, Good, Pass)
    public string? RevokeReason { get; set; }                 // Lý do thu hồi chứng chỉ (nếu có)
    public string? RevokedBy { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? FileUrl { get; set; }                     // Đường dẫn file PDF bản scan chứng chỉ điện tử
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

// ===== DTOs cho Quản lý Đào tạo & Cấp Chứng chỉ Nhân sự Đại lý (TrainingCourse & StaffCertificate) =====

public sealed record CreateTrainingCourseDto(
    string? TrainingCode,
    string? TrainingCodeUser,
    string CourseName,
    string? TrainingType,
    string? Level,
    string? Format,
    string? TrainerName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    int? MaxCapacity,
    decimal? PassingScore,
    decimal? BudgetAmount,
    string? Remark,
    string? CreatedBy,
    List<EnrollStaffDto>? Enrollments
);

public sealed record UpdateTrainingCourseHeaderDto(
    string? TrainingCodeUser,
    string? CourseName,
    string? TrainingType,
    string? Level,
    string? Format,
    string? TrainerName,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    int? MaxCapacity,
    decimal? PassingScore,
    decimal? BudgetAmount,
    decimal? ActualCost,
    string? Remark
);

public sealed record TrainingCourseTransitionDto(
    string? Actor,
    string? Note,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record EnrollStaffDto(
    string DealerCode,
    string? DealerName,
    string StaffCode,
    string StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string? Position,
    string? Remark
);

public sealed record BatchEnrollStaffItemDto(
    string DealerCode,
    string? DealerName,
    string StaffCode,
    string StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string? Position,
    string? Remark
);

public sealed record BatchEnrollStaffDto(
    List<BatchEnrollStaffItemDto> Items,
    string? CreatedBy
);

public sealed record UpdateEnrollmentDto(
    string? StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string? Position,
    decimal? AttendancePercent,
    decimal? TheoryScore,
    decimal? PracticeScore,
    string? ResultStatus,
    string? Remark
);

public sealed record GradeEnrollmentDto(
    decimal TheoryScore,
    decimal PracticeScore,
    decimal? AttendancePercent,
    string? InstructorFeedback,
    string? Actor
);

public sealed record BatchGradeEnrollmentItemDto(
    string EnrollmentNo,
    decimal TheoryScore,
    decimal PracticeScore,
    decimal? AttendancePercent,
    string? InstructorFeedback
);

public sealed record BatchGradeEnrollmentDto(
    List<BatchGradeEnrollmentItemDto> Items,
    string? Actor
);

public sealed record AutoIssueCertificatesDto(
    int? ValidityYears,
    string? IssuedBy,
    string? Actor,
    string? CertificateType,
    string? Level
);

public sealed record CreateStaffCertificateDto(
    string? CertificateNo,
    string? CertificateNoUser,
    string StaffCode,
    string StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string DealerCode,
    string? DealerName,
    string? Position,
    string? CertificateType,
    string? Level,
    DateTime? IssueDate,
    DateTime? ExpiryDate,
    string? IssuedBy,
    string? LinkedTrainingCode,
    string? LinkedEnrollmentNo,
    decimal? ScoreAchieved,
    string? Grade,
    string? FileUrl,
    string? Remark
);

public sealed record UpdateStaffCertificateDto(
    string? CertificateNoUser,
    string? StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string? DealerCode,
    string? DealerName,
    string? Position,
    string? CertificateType,
    string? Level,
    DateTime? IssueDate,
    DateTime? ExpiryDate,
    string? IssuedBy,
    string? FileUrl,
    string? Remark
);

public sealed record StaffCertificateTransitionDto(
    string? Actor,
    string? Note,
    string? Reason,
    DateTime? ExpiryDate,
    DateTime? TransitionDate
);

public sealed record TrainingSummaryDto(
    int TotalCourses,
    int TotalDraft,
    int TotalScheduled,
    int TotalInProgress,
    int TotalCompleted,
    int TotalCancelled,
    int TotalEnrollments,
    int TotalPassed,
    int TotalFailed,
    decimal PassRatePercent,
    int TotalCertificatesIssued,
    int TotalActiveCertificates,
    int TotalExpiredCertificates,
    int TotalRevokedCertificates,
    List<TrainingCourseTypeStatsDto> ByTrainingType,
    List<TrainingDealerStatsDto> ByDealer,
    List<TrainingLevelStatsDto> ByLevel
);

public sealed record TrainingCourseTypeStatsDto(string TrainingType, int CourseCount, int EnrolledCount, int PassedCount, decimal PassRatePercent);
public sealed record TrainingDealerStatsDto(string DealerCode, string DealerName, int EnrolledCount, int PassedCount, int ActiveCertificatesCount, decimal StandardizationRatePercent);
public sealed record TrainingLevelStatsDto(string Level, int CourseCount, int EnrolledCount, int PassedCount);

public sealed record DealerTrainingMatrixDto(
    string DealerCode,
    string DealerName,
    int TotalStaffCount,
    int CertifiedSalesConsultantCount,
    int CertifiedServiceAdvisorCount,
    int CertifiedTechnicianCount,
    int CertifiedEVTechnicianCount,
    int TotalActiveCertificates,
    decimal DealerStandardizationScore,
    List<DealerStaffTrainingItemDto> StaffMembers
);

public sealed record DealerStaffTrainingItemDto(
    string StaffCode,
    string StaffName,
    string Position,
    bool IsCertified,
    string? ActiveCertificateNo,
    string? CertificateType,
    string? CertificateLevel,
    DateTime? ExpiryDate,
    int CoursesAttendedCount,
    int CoursesPassedCount,
    decimal AverageScore
);

public sealed record StaffTrainingProfileDto(
    string StaffCode,
    string StaffName,
    string? StaffEmail,
    string? StaffPhone,
    string DealerCode,
    string? DealerName,
    string Position,
    bool HasActiveCertificate,
    StaffCertificate? ActiveCertificate,
    List<StaffCertificate> AllCertificates,
    List<StaffEnrollmentHistoryDto> EnrollmentHistory
);

public sealed record StaffEnrollmentHistoryDto(
    string EnrollmentNo,
    string TrainingCode,
    string CourseName,
    string TrainingType,
    string Level,
    DateTime StartDate,
    DateTime EndDate,
    decimal AttendancePercent,
    decimal TheoryScore,
    decimal PracticeScore,
    decimal FinalScore,
    string EvaluationGrade,
    string ResultStatus,
    bool IsCertificateIssued,
    string? CertificateNo
);

// ===== Quản lý Gói Dịch Vụ & Thẻ Bảo Dưỡng Trọn Gói Xe Ô Tô (BizCarSv.ServicePackage / Ser_ServicePackage, Ser_ServicePackageServiceItem, Ser_ServicePackagePartItem & FrmMst_ServicePackage) =====

/// <summary>Gói Dịch Vụ Bảo Dưỡng Định Kỳ Chuẩn Hãng OEM &amp; Đại Lý (BizCarSv.ServicePackage / Ser_ServicePackage / ServicePackage): định nghĩa cấu hình các gói bảo dưỡng mốc km (5.000km, 10.000km, 20.000km, 40.000km...), gói thay dầu nhớt trọn gói, chăm sóc làm đẹp xe, định mức giờ công chuẩn và phụ tùng Mobis tiêu hao kèm đơn giá ưu đãi combo.</summary>
public sealed class ServicePackage
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PackageNo { get; set; } = "";             // Mã gói dịch vụ (PKG-5K-CARE, PKG-10K-CARE, PKG-20K-CARE, PKG-40K-MAJOR, PKG-OIL-3Y, PKG-SPA-GOLD...)
    public string? PackageNoUser { get; set; }            // Mã hiệu nội bộ tham chiếu
    public string PackageName { get; set; } = "";           // Tên gói dịch vụ (Gói bảo dưỡng cấp 1 - 5.000 km, Gói bảo dưỡng cấp 4 - 40.000 km, Gói thay dầu trọn gói 3 năm...)
    public string DealerCode { get; set; } = "ALL";       // "ALL" (Hãng OEM áp dụng toàn quốc) hoặc mã đại lý cụ thể (DLR-HN01...)
    public string? DealerName { get; set; } = "Hyundai Toàn Quốc OEM";
    public string PackageType { get; set; } = "PeriodicMaintenance"; // PeriodicMaintenance (Bảo dưỡng định kỳ mốc km), OilService (Gói thay dầu nhớt trọn gói), BodyCare (Gói làm đẹp & chăm sóc xe Ceramic), BrakeTireService (Bảo dưỡng hệ thống phanh & lốp), AllInclusive (Gói bảo dưỡng toàn diện xe mới)
    public string ApplicableModel { get; set; } = "ALL";  // Dòng xe áp dụng: ALL, Accent, Creta, Tucson, SantaFe, Custin, Stargazer, Palisade, Ioniq 5...
    public int? MilestoneKm { get; set; } = 5000;         // Mốc số km áp dụng (5000, 10000, 20000, 40000, 80000, 100000...)
    public int StandardTakingTimeMinutes { get; set; } = 60; // Thời gian tiêu chuẩn thực hiện dịch vụ (phút)
    public int ValidityMonths { get; set; } = 12;         // Thời hạn hiệu lực của gói khi mua (12, 24, 36 tháng)
    public int MaxUsageCount { get; set; } = 1;           // Số lượt sử dụng tối đa của thẻ/gói (1 lần cho mốc km, 3/6 lần cho gói năm)
    public bool IsPublic { get; set; } = true;            // Gói công khai dùng chung toàn hệ thống hay riêng đại lý
    public bool IsUseBasePrice { get; set; } = true;      // Dùng đơn giá niêm yết chuẩn của hãng
    public decimal TotalLaborAmount { get; set; } = 0;    // Tổng tiền công định mức tiêu chuẩn (VNĐ)
    public decimal TotalPartAmount { get; set; } = 0;     // Tổng tiền phụ tùng & dầu nhớt định mức tiêu chuẩn (VNĐ)
    public decimal OriginalPrice { get; set; } = 0;       // Tổng giá trị gốc = TotalLaborAmount + TotalPartAmount
    public decimal DiscountPercent { get; set; } = 15;    // % chiết khấu ưu đãi combo của gói (%)
    public decimal PackagePrice { get; set; } = 0;        // Đơn giá bán trọn gói cho khách hàng = OriginalPrice * (1 - DiscountPercent/100)
    public int TotalSubscribedCount { get; set; } = 0;    // Tổng số lượng thẻ/xe đã mua gói dịch vụ này
    public int TotalUsedCount { get; set; } = 0;          // Tổng số lượt đã thực tế sử dụng dịch vụ theo gói này
    public string Status { get; set; } = "Draft";         // Draft → Active → Suspended → Archived
    public string? Description { get; set; }              // Mô tả chi tiết quyền lợi và quy cách gói
    public string? Remark { get; set; }                   // Ghi chú
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Giám đốc dịch vụ OEM / Đại lý duyệt ban hành
    public DateTime? ApprovedAt { get; set; }
    public string? ArchivedBy { get; set; }
    public DateTime? ArchivedAt { get; set; }
}

/// <summary>Chi tiết Hạng mục Tiền công &amp; Công việc trong Gói Dịch Vụ (BizCarSv.ServicePackage / Ser_ServicePackageServiceItem / ServicePackageLaborLine): mã công việc, số giờ công tiêu chuẩn standard hours, đơn giá giờ công, % giảm giá và thành tiền công.</summary>
public sealed class ServicePackageLaborLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServicePackageId { get; set; }
    public string PackageNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string ServiceItemCode { get; set; } = "";     // Mã công việc (PM_OIL_CHANGE, PM_FILTER_REPLACE, PM_BRAKE_CLEAN, PM_TIRE_ROTATION, PM_DIAGNOSTIC_OBD, PM_BATTERY_TEST, PM_CAR_WASH...)
    public string ServiceItemName { get; set; } = "";     // Tên công việc (Thay dầu động cơ & lọc nhớt, Vệ sinh phanh 4 bánh, Đảo lốp, Chẩn đoán OBD, Rửa xe hút bụi...)
    public decimal StandardHours { get; set; } = 0.5m;    // Số giờ công định mức (0.3h, 0.5h, 1.0h, 1.5h...)
    public decimal LaborPrice { get; set; } = 350000m;    // Đơn giá giờ công tiêu chuẩn (VNĐ/giờ)
    public decimal DiscountPercent { get; set; } = 0m;    // % giảm giá tiền công trong gói
    public decimal LaborAmount { get; set; } = 175000m;   // Tiền công = StandardHours * LaborPrice * (1 - DiscountPercent/100)
    public bool IsMandatory { get; set; } = true;         // Bắt buộc thực hiện trong gói
    public string? Remark { get; set; }
}

/// <summary>Chi tiết Phụ tùng &amp; Vật tư Phụ gia Mobis trong Gói Dịch Vụ (BizCarSv.ServicePackage / Ser_ServicePackagePartItem / ServicePackagePartLine): mã phụ tùng Mobis chính hãng, tên phụ tùng, đơn vị tính, số lượng định mức, đơn giá niêm yết, % giảm giá và thành tiền.</summary>
public sealed class ServicePackagePartLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ServicePackageId { get; set; }
    public string PackageNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string PartCode { get; set; } = "";            // Mã phụ tùng chính hãng Mobis (26300-35505, 05100-00441, 28113-A5800, 97133-D3000, 58101-D3A00, 04500-00115...)
    public string PartName { get; set; } = "";            // Tên phụ tùng (Lọc dầu động cơ, Dầu nhớt tổng hợp Hyundai Fully Synthetic 5W-30, Lọc gió động cơ, Lọc gió điều hòa than hoạt tính...)
    public string Unit { get; set; } = "Cái";             // Đơn vị tính: Cái, Lít, Can 4L, Bình, Bộ, Chai
    public decimal Quantity { get; set; } = 1.0m;         // Số lượng định mức tiêu chuẩn
    public decimal UnitPrice { get; set; } = 0m;          // Đơn giá niêm yết Mobis (VNĐ)
    public decimal DiscountPercent { get; set; } = 0m;    // % chiết khấu phụ tùng trong gói
    public decimal PartAmount { get; set; } = 0m;         // Tiền phụ tùng = Quantity * UnitPrice * (1 - DiscountPercent/100)
    public bool IsMandatory { get; set; } = true;         // Bắt buộc thay thế trong gói
    public string? Remark { get; set; }
}

/// <summary>Hợp đồng Mua Thẻ / Gói Bảo Dưỡng Trọn Gói Gán Theo Xe VIN (BizCarSv.ServicePackage / ServicePackageSubscription): quản lý đăng ký thẻ bảo dưỡng điện tử, ngày kích hoạt, ngày hết hạn, tổng số lượt sử dụng được cấp, số lượt đã dùng, số lượt còn lại và giá trị hợp đồng.</summary>
public sealed class ServicePackageSubscription
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SubscriptionNo { get; set; } = "";       // Mã hợp đồng đăng ký thẻ (SUB-2026-0001, SUB...)
    public string? SubscriptionNoUser { get; set; }       // Số tham chiếu hợp đồng nội bộ
    public string PackageCardNo { get; set; } = "";        // Mã thẻ dịch vụ / thẻ thành viên điện tử (CRD-HYUNDAI-001, CRD...)
    public long ServicePackageId { get; set; }
    public string PackageNo { get; set; } = "";            // Mã gói dịch vụ liên kết
    public string PackageName { get; set; } = "";          // Tên gói dịch vụ
    public string PackageType { get; set; } = "PeriodicMaintenance";
    public string Vin { get; set; } = "";                  // Số khung xe VIN sở hữu thẻ
    public string Model { get; set; } = "";                // Dòng xe
    public string? EngineNo { get; set; }
    public string? PlateNo { get; set; }                   // Biển số xe (nếu đã đăng ký)
    public string CustomerName { get; set; } = "";         // Tên chủ xe / khách hàng
    public string CustomerPhone { get; set; } = "";        // SĐT khách hàng
    public string? CustomerEmail { get; set; }
    public string DealerCode { get; set; } = "DLR-HN01";   // Đại lý phát hành / bán thẻ
    public string? DealerName { get; set; } = "Hyundai Hà Nội 01";
    public string? SalesAdvisor { get; set; }             // Tư vấn bán hàng / Cố vấn dịch vụ bán gói
    public DateTime PurchaseDate { get; set; } = DateTime.Now; // Ngày mua thẻ
    public DateTime StartDate { get; set; } = DateTime.Now;    // Ngày kích hoạt hiệu lực thẻ
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(1); // Ngày hết hạn thẻ
    public decimal TotalPackagePrice { get; set; } = 0;    // Giá bán gói bảo dưỡng (VNĐ)
    public decimal PaidAmount { get; set; } = 0;           // Số tiền thực tế khách đã thanh toán (VNĐ)
    public bool IsPaid { get; set; } = true;               // Đã hoàn tất thanh toán
    public string PaymentMethod { get; set; } = "Cash";    // Phương thức thanh toán: Cash, BankTransfer, CreditCard, FreeOEMBonus
    public int MaxUsageCount { get; set; } = 1;            // Tổng số lượt sử dụng được cấp của gói
    public int UsedCount { get; set; } = 0;                // Số lượt đã thực tế sử dụng dịch vụ tại xưởng
    public int RemainingCount { get; set; } = 1;           // Số lượt còn lại = MaxUsageCount - UsedCount
    public decimal TotalSavedAmount { get; set; } = 0;     // Tổng số tiền đã tiết kiệm được qua các lần sử dụng (VNĐ)
    public string Status { get; set; } = "Active";         // Active (Đang hoạt động & còn lượt), Exhausted (Đã dùng hết lượt), Expired (Hết hạn thời gian), Suspended (Tạm khóa thẻ), Cancelled (Hủy / Hoàn tiền)
    public string? Remark { get; set; }                    // Ghi chú hợp đồng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Nhật ký Từng Lần Đưa Xe Vào Xưởng Sử Dụng Quyền Lợi Gói Bảo Dưỡng (BizCarSv.ServicePackage / ServicePackageUsage): ghi nhận xe VIN mang thẻ gói bảo dưỡng vào xưởng sử dụng, trừ 1 lượt sử dụng trên thẻ, liên kết với Lệnh sửa chữa RO, ODO, tiền tiết kiệm cho chủ xe và đánh giá CSI.</summary>
public sealed class ServicePackageUsage
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string UsageNo { get; set; } = "";              // Mã lượt sử dụng (USG-2026-0001, USG...)
    public long SubscriptionId { get; set; }
    public string SubscriptionNo { get; set; } = "";       // Mã hợp đồng đăng ký thẻ
    public string PackageCardNo { get; set; } = "";        // Mã thẻ dịch vụ
    public string PackageNo { get; set; } = "";            // Mã gói dịch vụ
    public string PackageName { get; set; } = "";          // Tên gói dịch vụ
    public string Vin { get; set; } = "";                  // Số khung VIN sử dụng
    public string Model { get; set; } = "";                // Dòng xe
    public string? PlateNo { get; set; }                   // Biển số xe
    public string DealerCode { get; set; } = "DLR-HN01";   // Đại lý tiếp nhận làm dịch vụ
    public string? DealerName { get; set; } = "Hyundai Hà Nội 01";
    public DateTime UsageDate { get; set; } = DateTime.Now; // Thời điểm xe vào xưởng
    public int OdoKm { get; set; } = 5000;                 // Số km ODO ghi nhận tại xưởng khi dùng gói
    public int? MilestoneUsed { get; set; } = 5000;        // Mốc bảo dưỡng được thực hiện lần này (km)
    public string? RoNo { get; set; }                      // Mã Lệnh sửa chữa xưởng liên kết (Ser_RO / RepairOrder)
    public string? CavityNo { get; set; }                  // Mã khoang/cầu sửa chữa thực hiện (BAY-01...)
    public string? Technician { get; set; }                // Kỹ thuật viên chính thực hiện
    public string? ServiceAdvisor { get; set; }            // Cố vấn dịch vụ tiếp nhận
    public decimal LaborSavedAmount { get; set; } = 0;     // Tiền công được miễn phí theo gói (VNĐ)
    public decimal PartSavedAmount { get; set; } = 0;      // Tiền phụ tùng & dầu nhớt được miễn phí theo gói (VNĐ)
    public decimal TotalSavedAmount { get; set; } = 0;     // Tổng số tiền khách hàng được khấu trừ miễn phí = LaborSavedAmount + PartSavedAmount
    public string Status { get; set; } = "Confirmed";      // Pending → Confirmed → Completed (hoặc Cancelled)
    public decimal? CustomerRating { get; set; } = 5.0m;   // Điểm đánh giá hài lòng CSI của khách (1.0 - 5.0 sao)
    public string? CustomerFeedback { get; set; }          // Ý kiến nhận xét của chủ xe
    public string? Remark { get; set; }                    // Ghi chú lượt sử dụng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ConfirmedBy { get; set; }               // Cố vấn dịch vụ / Thủ quỹ xác nhận trừ lượt
    public DateTime? ConfirmedAt { get; set; }
}

// ===== DTOs cho Quản lý Gói Dịch Vụ & Thẻ Bảo Dưỡng Trọn Gói (BizCarSv.ServicePackage / Ser_ServicePackage) =====

public sealed record CreateServicePackageDto(
    string? PackageNo,
    string? PackageNoUser,
    string PackageName,
    string? DealerCode,
    string? DealerName,
    string? PackageType,
    string? ApplicableModel,
    int? MilestoneKm,
    int? StandardTakingTimeMinutes,
    int? ValidityMonths,
    int? MaxUsageCount,
    bool? IsPublic,
    bool? IsUseBasePrice,
    decimal? DiscountPercent,
    decimal? PackagePrice,
    string? Description,
    string? Remark,
    string? CreatedBy,
    List<ServicePackageLaborLineInputDto>? LaborLines,
    List<ServicePackagePartLineInputDto>? PartLines
);

public sealed record ServicePackageLaborLineInputDto(
    string ServiceItemCode,
    string ServiceItemName,
    decimal StandardHours,
    decimal LaborPrice,
    decimal? DiscountPercent,
    bool? IsMandatory,
    string? Remark
);

public sealed record ServicePackagePartLineInputDto(
    string PartCode,
    string PartName,
    string Unit,
    decimal Quantity,
    decimal UnitPrice,
    decimal? DiscountPercent,
    bool? IsMandatory,
    string? Remark
);

public sealed record UpdateServicePackageHeaderDto(
    string? PackageNoUser,
    string? PackageName,
    string? DealerCode,
    string? DealerName,
    string? PackageType,
    string? ApplicableModel,
    int? MilestoneKm,
    int? StandardTakingTimeMinutes,
    int? ValidityMonths,
    int? MaxUsageCount,
    bool? IsPublic,
    bool? IsUseBasePrice,
    decimal? DiscountPercent,
    decimal? PackagePrice,
    string? Description,
    string? Remark
);

public sealed record UpdateServicePackageLaborLineDto(
    string? ServiceItemCode,
    string? ServiceItemName,
    decimal? StandardHours,
    decimal? LaborPrice,
    decimal? DiscountPercent,
    bool? IsMandatory,
    string? Remark
);

public sealed record UpdateServicePackagePartLineDto(
    string? PartCode,
    string? PartName,
    string? Unit,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? DiscountPercent,
    bool? IsMandatory,
    string? Remark
);

public sealed record ServicePackageTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record SubscribeServicePackageDto(
    string PackageNo,
    string Vin,
    string? PackageCardNo,
    string? SubscriptionNoUser,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    string? DealerCode,
    string? DealerName,
    string? SalesAdvisor,
    DateTime? PurchaseDate,
    DateTime? StartDate,
    DateTime? ExpiryDate,
    decimal? TotalPackagePrice,
    decimal? PaidAmount,
    string? PaymentMethod,
    int? MaxUsageCount,
    string? Remark,
    string? CreatedBy
);

public sealed record UpdateServicePackageSubscriptionDto(
    string? SubscriptionNoUser,
    string? PackageCardNo,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    string? DealerCode,
    string? DealerName,
    string? SalesAdvisor,
    DateTime? StartDate,
    DateTime? ExpiryDate,
    decimal? TotalPackagePrice,
    decimal? PaidAmount,
    bool? IsPaid,
    string? PaymentMethod,
    int? MaxUsageCount,
    string? Status,
    string? Remark
);

public sealed record ServicePackageSubscriptionTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? ExpiryDate,
    DateTime? TransitionDate
);

public sealed record RecordServicePackageUsageDto(
    string SubscriptionNo,
    string? PackageCardNo,
    string? Vin,
    int OdoKm,
    int? MilestoneUsed,
    string? DealerCode,
    string? DealerName,
    string? RoNo,
    string? CavityNo,
    string? Technician,
    string? ServiceAdvisor,
    decimal? LaborSavedAmount,
    decimal? PartSavedAmount,
    decimal? CustomerRating,
    string? CustomerFeedback,
    string? Remark,
    string? CreatedBy,
    DateTime? UsageDate
);

public sealed record ServicePackageUsageTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record RecordServicePackageUsageFeedbackDto(
    decimal CustomerRating,
    string? CustomerFeedback,
    string? Actor
);

public sealed record ServicePackageSummaryDto(
    int TotalPackages,
    int TotalActivePackages,
    int TotalDraftPackages,
    int TotalArchivedPackages,
    int TotalSubscriptions,
    int TotalActiveSubscriptions,
    int TotalExhaustedSubscriptions,
    int TotalExpiredSubscriptions,
    int TotalUsages,
    decimal TotalSubscriptionRevenue,
    decimal TotalCustomerSavings,
    decimal PackageUtilizationRatePercent,
    List<ServicePackageTypeStatsDto> ByPackageType,
    List<ServicePackageDealerStatsDto> ByDealer,
    List<ServicePackageModelStatsDto> ByModel
);

public sealed record ServicePackageTypeStatsDto(string PackageType, int PackageCount, int SubscriptionsCount, int UsagesCount, decimal Revenue);
public sealed record ServicePackageDealerStatsDto(string DealerCode, string DealerName, int SubscriptionsCount, int UsagesCount, decimal Revenue);
public sealed record ServicePackageModelStatsDto(string Model, int SubscriptionsCount, int UsagesCount, decimal Savings);

public sealed record VehicleServicePackageInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? LastServicePackageNo,
    string? LastPackageCardNo,
    int ActiveServicePackageCount,
    int PackageUsageCount,
    List<ServicePackageSubscription> Subscriptions,
    List<ServicePackageUsage> RecentUsages
);

// ===== Cấu hình Điều kiện & Tự động Phân bổ & Sinh Lệnh Giao Xe DO Tự Động (BizHTC.Car / Car_ConditionForDOAuto, Mst_DOATCondition, Mst_DOATConditionDtl, Car_DeliveryOrderAuto / FrmMngSetupConditionForDOAuto, FrmNewSetupConditionForDOAuto, FrmNewDOAuto) =====

/// <summary>Cấu hình Điều kiện Giao xe Tự động DO Auto (BizHTC.Car / Mst_DOATCondition / DOAutoCondition): thiết lập các tiêu chí tài chính, pháp lý, chất lượng KCS, quy tắc ưu tiên (FIFO theo ngày tồn kho / ngày hợp đồng, tỷ lệ thanh toán, phân hạng đại lý) và hạn mức tự động phân bổ xe InStock sinh Lệnh giao xe DO.</summary>
public sealed class DOAutoCondition
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ConditionCode { get; set; } = "";         // Mã cấu hình điều kiện (COND-DO-2026-01, COND...)
    public string? ConditionNoUser { get; set; }           // Số hiệu / ký hiệu tham chiếu nội bộ
    public string ConditionName { get; set; } = "";        // Tên cấu hình điều kiện giao xe tự động
    public string? Description { get; set; }               // Mô tả chi tiết mục đích cấu hình
    public string PriorityRule { get; set; } = "FIFO_StoreDate"; // Quy tắc ưu tiên: FIFO_StoreDate (Xe tồn kho lâu nhất), FIFO_ContractDate (Hợp đồng/Đơn hàng ký sớm nhất), PaymentRatio_Desc (Tỷ lệ thanh toán/bảo lãnh cao nhất), DealerTier (Ưu tiên đại lý cấp 1/trọng điểm)
    public DateTime EffectiveFrom { get; set; } = DateTime.Now; // Ngày bắt đầu có hiệu lực
    public DateTime EffectiveTo { get; set; } = DateTime.Now.AddMonths(3); // Ngày hết hạn hiệu lực
    public decimal MinDepositPercent { get; set; } = 10m;   // Tỷ lệ % tiền đặt cọc tối thiểu (VD: 10% = 10)
    public decimal MinPaymentPercent { get; set; } = 80m;   // Tỷ lệ % thanh toán hoặc bảo lãnh ngân hàng tối thiểu (VD: 80% = 80)
    public bool RequireGuaranteeOrPaid { get; set; } = true; // Bắt buộc xe đã có Bảo lãnh ngân hàng hoặc đã hoàn tất thanh toán
    public bool RequireQC { get; set; } = true;             // Bắt buộc xe đã kiểm tra KCS xuất xưởng nhà máy (ManufacturedDate != null)
    public bool RequireCustomsClearance { get; set; } = false; // Bắt buộc xe nhập khẩu đã thông quan hải quan (IsCustomsCleared)
    public bool RequireTaxPaid { get; set; } = false;       // Bắt buộc xe đã nộp đủ thuế hải quan (TaxPaymentDate != null)
    public bool RequirePdiPassed { get; set; } = true;      // Bắt buộc xe đã qua kiểm tra tiền bàn giao PDI đạt chuẩn
    public bool RequireRedeemed { get; set; } = true;       // Bắt buộc xe không bị thế chấp hoặc đã giải chấp (IsMortgaged == false || RedeemDate != null)
    public bool RequireGpsInstalled { get; set; } = false;  // Bắt buộc xe đã gắn thiết bị định vị GPS giám sát vận chuyển
    public int MaxBatchVehicleQuota { get; set; } = 100;    // Số lượng xe tối đa được phân bổ trong 1 đợt chạy
    public int TotalExecutedBatches { get; set; } = 0;      // Tổng số đợt chạy đã áp dụng điều kiện này
    public int TotalAllocatedVehicles { get; set; } = 0;    // Tổng số lượng xe đã được phân bổ thành công
    public string Status { get; set; } = "Draft";           // Draft → Active → Expired (hoặc Suspended / Cancelled)
    public string? Remark { get; set; }                     // Ghi chú điều hành
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }                 // Giám đốc Bán hàng / Khối Phân phối duyệt cấu hình
    public DateTime? ApprovedAt { get; set; }
    public string? SuspendedBy { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết Dòng xe áp dụng trong Cấu hình Giao xe Tự động (BizHTC.Car / Mst_DOATConditionDtl / DOAutoConditionLine): quy định model, phiên bản spec, màu sắc, số lượng phân bổ tối đa quota và thứ tự ưu tiên dòng xe.</summary>
public sealed class DOAutoConditionLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DOAutoConditionId { get; set; }
    public string ConditionCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string Model { get; set; } = "";                 // Dòng xe áp dụng (SantaFe, Tucson, Accent, Creta, Custin, Stargazer, Palisade, Ioniq 5...)
    public string? SpecCode { get; set; }                   // Phiên bản cấu hình (1.5 AT Tiêu Chuẩn, 1.6T AWD, Calligraphy...)
    public string? ColorCode { get; set; }                  // Mã màu (NWAC, SAW, R2P...) hoặc null (tất cả màu)
    public int MaxQuotaQty { get; set; } = 50;              // Số lượng xe tối đa cho phép phân bổ dòng này trong đợt
    public int PriorityRank { get; set; } = 1;              // Thứ tự ưu tiên dòng xe (1 = Ưu tiên cao nhất)
    public string Status { get; set; } = "Active";          // Active, Inactive
    public string? Remark { get; set; }
}

/// <summary>Chi tiết Đại lý áp dụng trong Cấu hình Giao xe Tự động (BizHTC.Car / DOAutoConditionDealerLine): quy định danh sách đại lý được nhận xe tự động, phân hạng đại lý tier, khu vực và hạn mức tối đa cho mỗi đại lý.</summary>
public sealed class DOAutoConditionDealerLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DOAutoConditionId { get; set; }
    public string ConditionCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string DealerCode { get; set; } = "";            // Mã đại lý (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }                 // Tên đại lý
    public string? RegionCode { get; set; } = "MienBac";    // MienBac, MienTrung, MienNam, TayNguyen
    public int MaxDealerQuota { get; set; } = 20;           // Hạn mức số lượng xe tối đa cho đại lý trong đợt
    public string TierLevel { get; set; } = "Tier1";        // Phân hạng đại lý: PriorityVIP, Tier1, Tier2, Tier3
    public string Status { get; set; } = "Active";          // Active, Inactive
    public string? Remark { get; set; }
}

/// <summary>Đợt Chạy Phân Bổ &amp; Sinh Lệnh Giao Xe DO Tự Động (BizHTC.Car / Car_DeliveryOrderAuto / AutoDeliveryOrderBatch): quản lý phiên chạy tự động quét kho tồn InStock, đối soát đơn hàng SO / hợp đồng đại lý, lọc điều kiện và tự động sinh Lệnh giao xe DeliveryOrder theo từng đại lý.</summary>
public sealed class AutoDeliveryOrderBatch
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string BatchNo { get; set; } = "";               // Mã đợt chạy (ADOB-2026-03-0001, ADOB...)
    public string? BatchNoUser { get; set; }              // Ký hiệu tham chiếu nội bộ
    public DateTime BatchDate { get; set; } = DateTime.Now; // Ngày giờ thực hiện đợt chạy
    public long? ConditionId { get; set; }                 // Liên kết cấu hình điều kiện áp dụng
    public string ConditionCode { get; set; } = "";         // Mã cấu hình điều kiện
    public string? ConditionName { get; set; }             // Tên cấu hình điều kiện
    public string? StorageCode { get; set; } = "ALL";       // Kho bãi OEM quét xe (ALL, PLANT-HTMV1, PLANT-HTMV2, TCV_YARD...)
    public int TotalScannedVehicles { get; set; } = 0;      // Tổng số xe tồn kho InStock được quét
    public int TotalEligibleVehicles { get; set; } = 0;     // Số lượng xe thỏa mãn toàn bộ tiêu chí điều kiện
    public int TotalAllocatedVehicles { get; set; } = 0;    // Số lượng xe đã thực tế phân bổ & sinh Lệnh giao xe DO
    public int TotalSkippedVehicles { get; set; } = 0;      // Số lượng xe bị bỏ qua do không thỏa mãn hoặc vượt quota
    public int TotalGeneratedDOs { get; set; } = 0;         // Tổng số phiếu Lệnh giao xe DeliveryOrder được tự động tạo mới
    public string Status { get; set; } = "Draft";           // Draft → Simulated → Executed → Confirmed (hoặc Cancelled / Rollbacked)
    public string ExecutionMode { get; set; } = "LiveExecution"; // LiveExecution (Chạy thực tế sinh DO), Simulation (Chạy thử mô phỏng kiểm tra)
    public string? Remark { get; set; }                     // Ghi chú đợt chạy
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ExecutedBy { get; set; }                 // Chuyên viên điều phối / Hệ thống chạy lệnh
    public DateTime? ExecutedAt { get; set; }
    public string? ConfirmedBy { get; set; }                // Lãnh đạo xác nhận kết quả đợt phân bổ
    public DateTime? ConfirmedAt { get; set; }
    public string? RollbackedBy { get; set; }               // Người thực hiện hoàn tác đợt chạy
    public DateTime? RollbackedAt { get; set; }
    public string? RollbackReason { get; set; }
}

/// <summary>Chi tiết Xe &amp; Kết quả Phân bổ trong Đợt Chạy Giao Xe Tự Động (BizHTC.Car / AutoDeliveryOrderBatchLine): ghi nhận từng số khung VIN, thông tin xe, đại lý được gán, mã DO được sinh, trạng thái hợp lệ và lý do không đạt nếu bị bỏ qua.</summary>
public sealed class AutoDeliveryOrderBatchLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long AutoDeliveryOrderBatchId { get; set; }
    public string BatchNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string Vin { get; set; } = "";                   // Số khung xe VIN
    public string Model { get; set; } = "";                 // Dòng xe
    public string? SpecCode { get; set; }                   // Phiên bản xe
    public string? EngineNo { get; set; }                   // Số máy
    public string? Color { get; set; }                      // Màu sơn
    public string? StorageCode { get; set; }                // Vị trí kho bãi hiện tại của xe
    public string? DealerCode { get; set; }                 // Đại lý được phân bổ nhận xe
    public string? DealerName { get; set; }                 // Tên đại lý
    public string? SOCode { get; set; }                     // Đơn đặt hàng SO liên kết (nếu có)
    public string? ContractNo { get; set; }                 // Hợp đồng mua bán liên kết (nếu có)
    public string? AllocatedDoNo { get; set; }              // Mã Lệnh giao xe DeliveryOrder được tự động tạo (DO-AUTO-...)
    public string AllocationStatus { get; set; } = "Allocated"; // Allocated (Đã sinh DO), Simulated (Mô phỏng đạt), Skipped (Bỏ qua không đạt), Failed (Lỗi xử lý)
    public string? EligibilityReason { get; set; }          // Lý do hợp lệ hoặc lý do không đủ điều kiện (NotEligibleReason)
    public bool IsQCPassed { get; set; } = true;            // Đã nghiệm thu KCS xuất xưởng
    public bool IsCustomsCleared { get; set; } = true;      // Đã thông quan hải quan
    public bool IsTaxPaid { get; set; } = true;             // Đã hoàn tất nộp thuế
    public bool IsPdiPaid { get; set; } = true;             // Đã hoàn tất nghiệm thu PDI
    public bool IsRedeemed { get; set; } = true;            // Không thế chấp / Đã giải chấp
    public bool IsGpsInstalled { get; set; } = true;        // Đã gắn thiết bị định vị GPS
    public bool IsGuaranteedOrPaid { get; set; } = true;    // Đã có bảo lãnh ngân hàng hoặc đã thanh toán
    public string Status { get; set; } = "Pending";         // Pending → Allocated → Delivered (hoặc Rollbacked / Cancelled)
    public string? Remark { get; set; }
}

// ===== DTOs cho Cấu hình Điều kiện & Tự động Phân bổ Sinh Lệnh Giao Xe DO Tự Động =====

public sealed record CreateDOAutoConditionDto(
    string? ConditionCode,
    string? ConditionNoUser,
    string ConditionName,
    string? Description,
    string? PriorityRule,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    decimal? MinDepositPercent,
    decimal? MinPaymentPercent,
    bool? RequireGuaranteeOrPaid,
    bool? RequireQC,
    bool? RequireCustomsClearance,
    bool? RequireTaxPaid,
    bool? RequirePdiPassed,
    bool? RequireRedeemed,
    bool? RequireGpsInstalled,
    int? MaxBatchVehicleQuota,
    string? Remark,
    string? CreatedBy,
    List<DOAutoConditionLineInputDto>? ModelLines,
    List<DOAutoConditionDealerLineInputDto>? DealerLines
);

public sealed record DOAutoConditionLineInputDto(
    string Model,
    string? SpecCode,
    string? ColorCode,
    int? MaxQuotaQty,
    int? PriorityRank,
    string? Remark
);

public sealed record DOAutoConditionDealerLineInputDto(
    string DealerCode,
    string? DealerName,
    string? RegionCode,
    int? MaxDealerQuota,
    string? TierLevel,
    string? Remark
);

public sealed record UpdateDOAutoConditionHeaderDto(
    string? ConditionNoUser,
    string? ConditionName,
    string? Description,
    string? PriorityRule,
    DateTime? EffectiveFrom,
    DateTime? EffectiveTo,
    decimal? MinDepositPercent,
    decimal? MinPaymentPercent,
    bool? RequireGuaranteeOrPaid,
    bool? RequireQC,
    bool? RequireCustomsClearance,
    bool? RequireTaxPaid,
    bool? RequirePdiPassed,
    bool? RequireRedeemed,
    bool? RequireGpsInstalled,
    int? MaxBatchVehicleQuota,
    string? Remark
);

public sealed record UpdateDOAutoConditionLineDto(
    string? Model,
    string? SpecCode,
    string? ColorCode,
    int? MaxQuotaQty,
    int? PriorityRank,
    string? Status,
    string? Remark
);

public sealed record UpdateDOAutoConditionDealerLineDto(
    string? DealerName,
    string? RegionCode,
    int? MaxDealerQuota,
    string? TierLevel,
    string? Status,
    string? Remark
);

public sealed record DOAutoConditionTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record CreateAutoDeliveryBatchDto(
    string? BatchNo,
    string? BatchNoUser,
    string ConditionCode,
    string? StorageCode,
    string? ExecutionMode,
    string? Remark,
    string? CreatedBy
);

public sealed record SimulateAutoDeliveryAllocationDto(
    string ConditionCode,
    string? StorageCode,
    string? DealerCode,
    string? Model,
    int? MaxVehicleCount,
    string? Actor
);

public sealed record ExecuteAutoDeliveryAllocationDto(
    string ConditionCode,
    string? BatchNoUser,
    string? StorageCode,
    string? DealerCode,
    string? Model,
    int? MaxVehicleCount,
    string? Remark,
    string? CreatedBy
);

public sealed record AutoDeliveryBatchTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record SimulationAllocationResultDto(
    string ConditionCode,
    string ConditionName,
    string PriorityRule,
    int TotalScanned,
    int TotalEligible,
    int TotalSimulatedAllocated,
    int TotalSkipped,
    List<SimulationAllocationItemDto> AllocatedVehicles,
    List<SimulationAllocationItemDto> SkippedVehicles
);

public sealed record SimulationAllocationItemDto(
    string Vin,
    string Model,
    string? SpecCode,
    string? Color,
    string? StorageCode,
    string? AssignedDealerCode,
    string? AssignedDealerName,
    string? MatchedOrderOrContract,
    string Status,
    string Reason
);

public sealed record AutoDeliveryOrderSummaryDto(
    int TotalConditions,
    int TotalActiveConditions,
    int TotalDraftConditions,
    int TotalBatches,
    int TotalExecutedBatches,
    int TotalSimulatedBatches,
    int TotalScannedVehicles,
    int TotalEligibleVehicles,
    int TotalAllocatedVehicles,
    int TotalGeneratedDOs,
    decimal AutoAllocationSuccessRatePercent,
    List<AutoDeliveryModelStatsDto> ByModel,
    List<AutoDeliveryDealerStatsDto> ByDealer
);

public sealed record AutoDeliveryModelStatsDto(string Model, int ScannedCount, int EligibleCount, int AllocatedCount, decimal AllocationRate);
public sealed record AutoDeliveryDealerStatsDto(string DealerCode, string DealerName, int AllocatedVehicles, int GeneratedDOs);

public sealed record VehicleAutoDoInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? LastAutoDoNo,
    DateTime? LastAutoDoDate,
    int AutoDoCount,
    List<AutoDeliveryOrderBatchLine> BatchHistory
);

// ===== Khảo sát Chỉ số Hài lòng Bán hàng & Bàn giao xe mới SSI (BizHTC.DealerSales / DLS_VINSurvey, RptSSI_ICIC, DlsVINSurvey_Update) =====

/// <summary>Khảo sát Đánh giá Chỉ số Hài lòng Bán hàng SSI (BizHTC.DealerSales / DLS_VINSurvey / SalesSatisfactionSurvey): Trung tâm CSKH ICIC / Đại lý thực hiện khảo sát độc lập sau khi bàn giao xe mới cho khách hàng theo tiêu chuẩn Hyundai &amp; J.D. Power SSI (29 tiêu chí: TVBH, Showroom, Quy trình bàn giao xe, Hướng dẫn ADAS/AVN, Giấy tờ tài chính, Tiến độ giao xe, Net Promoter Score NPS và xử lý khiếu nại).</summary>
public sealed class SalesSatisfactionSurvey
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SurveyNo { get; set; } = "";             // Mã số phiếu khảo sát SSI (SSI-2026-03-0001, SSI...)
    public string? SurveyNoUser { get; set; }            // Ký hiệu tham chiếu phiếu nội bộ
    public string? DealNo { get; set; }                  // Mã hợp đồng / giao dịch bán lẻ xe liên kết (DLS_Deal / DealerDeal)
    public string Vin { get; set; } = "";                // Số khung xe VIN được bàn giao
    public string Model { get; set; } = "";              // Dòng xe (SantaFe, Tucson, Accent, Creta, Custin, Stargazer, Palisade, Ioniq 5...)
    public string? SpecCode { get; set; }                // Phiên bản xe
    public string? EngineNo { get; set; }                // Số máy
    public string? Color { get; set; }                   // Màu sơn
    public string? PlateNo { get; set; }                 // Biển số xe (nếu đã đăng ký)
    public string CustomerName { get; set; } = "";       // Họ tên khách hàng / chủ sở hữu
    public string CustomerPhone { get; set; } = "";      // Số điện thoại liên hệ
    public string? CustomerEmail { get; set; }           // Email khách hàng
    public string CustomerType { get; set; } = "Individual"; // Individual (Cá nhân), Corporate (Doanh nghiệp)
    public string DealerCode { get; set; } = "";         // Mã đại lý bán xe (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }              // Tên đại lý
    public string? SalesConsultantCode { get; set; }     // Mã nhân viên TVBH phụ trách hợp đồng
    public string? SalesConsultantName { get; set; }     // Tên nhân viên TVBH
    public DateTime DeliveryDate { get; set; } = DateTime.Now; // Ngày thực tế bàn giao xe cho khách hàng
    public DateTime SurveyDate { get; set; } = DateTime.Now;   // Ngày thực hiện cuộc khảo sát
    public string ContactChannel { get; set; } = "PhoneCall";  // PhoneCall (Cuộc gọi tổng đài ICIC), OnlineSurvey (Khảo sát trực tuyến Web), SMS (Tin nhắn), ZaloZNS (Zalo OA), ShowroomTablet (Tablet tại phòng giao xe), InPerson (Trực tiếp)
    public int CallAttempts { get; set; } = 1;           // Số lần thử liên hệ cuộc gọi
    public string? SurveyorStaff { get; set; }           // Chuyên viên CSKH / Khảo sát viên ICIC

    // ===== Điểm số SSI theo các nhóm tiêu chí chuẩn (Thang điểm 1.0 - 5.0 sao) =====
    public decimal ScoreSalesConsultant { get; set; } = 5.0m;  // 1. Tư vấn bán hàng: Thái độ, tác phong đón tiếp, am hiểu sản phẩm, tư vấn trung thực
    public decimal ScoreDealershipFacility { get; set; } = 5.0m; // 2. Cơ sở vật chất Showroom: Khang trang, sạch sẽ, khu vực tiếp khách tiện nghi
    public decimal ScoreDeliveryProcess { get; set; } = 5.0m;    // 3. Quy trình bàn giao xe: Lễ bàn giao trang trọng, giải thích kỹ thuật và bàn giao phụ kiện
    public decimal ScorePaperworkFinance { get; set; } = 5.0m;   // 4. Giấy tờ & Tài chính: Minh bạch giá cả, hợp đồng, bảo hiểm, hỗ trợ ngân hàng trả góp
    public decimal ScoreTimeliness { get; set; } = 5.0m;         // 5. Tiến độ giao xe: Bàn giao xe đúng hẹn cam kết
    public decimal ScoreOverall { get; set; } = 5.0m;            // Điểm đánh giá trải nghiệm mua xe tổng thể (1.0 - 5.0 sao)
    public decimal CalculatedSsiScore { get; set; } = 5.0m;      // Điểm SSI bình quân gia quyền theo chuẩn OEM (Thang 1.0 - 5.0)
    public int SsiIndex1000 { get; set; } = 1000;                // Điểm SSI quy đổi hệ 1000 điểm chuẩn J.D. Power SSI = Round(CalculatedSsiScore / 5.0 * 1000)

    // ===== Danh mục kiểm tra xác thực chi tiết (Checklist Nghiệm thu Bàn giao) =====
    public bool IsCleanCarDelivered { get; set; } = true;        // Xe được vệ sinh sạch sẽ tinh tươm, không trầy xước
    public bool IsFeatureExplained { get; set; } = true;         // TVBH đã hướng dẫn chi tiết các tính năng cơ bản & Sách HDSD
    public bool IsAdasExplained { get; set; } = true;            // TVBH đã giải thích công nghệ an toàn chủ động Hyundai SmartSense / ADAS
    public bool IsAvnBluelinkSetup { get; set; } = true;         // Đã kết nối điện thoại & kích hoạt Màn hình AVN / Ứng dụng Hyundai Bluelink
    public bool IsOriginalDocsHandedOver { get; set; } = true;   // Đã nhận đầy đủ bộ hồ sơ gốc, hóa đơn GTGT, Phiếu xuất xưởng và Sổ bảo hành
    public bool IsFollowUpCallPromised { get; set; } = true;     // TVBH đã hẹn gọi điện chăm sóc sau bàn giao xe

    // ===== Chỉ số Net Promoter Score (NPS) & Ý kiến phản hồi =====
    public int NpsScore { get; set; } = 10;                      // Điểm NPS (0 - 10 điểm): Mức độ sẵn sàng giới thiệu Hyundai cho người thân/bạn bè
    public string NpsCategory { get; set; } = "Promoter";        // Promoter (9-10 điểm), Passive (7-8 điểm), Detractor (0-6 điểm)
    public string? CustomerFeedback { get; set; }                // Lời nhận xét, cảm nhận hoặc khen ngợi của khách hàng

    // ===== Quản lý Khiếu nại & Phương án Khắc phục Sự cố =====
    public bool HasComplaint { get; set; } = false;              // Khách hàng có phản ánh không hài lòng hoặc khiếu nại (true/false)
    public string? ComplaintCategory { get; set; }               // Phân loại khiếu nại: DeliveryDelay (Chậm giao xe), PriceFinance (Bất đồng giá/chi phí/vay), SalesAttitude (Thái độ TVBH), CarDefect (Lỗi kỹ thuật xe), MissingAccessories (Thiếu phụ kiện/quà), PaperworkDelay (Chậm hồ sơ/biển số), Other (Khác)
    public string? ComplaintDetails { get; set; }                // Nội dung chi tiết bức xúc / khiếu nại của khách hàng
    public string? RemedyAction { get; set; }                    // Phương án xử lý / giải pháp khắc phục từ Đại lý hoặc Hãng OEM
    public bool IsComplaintResolved { get; set; } = false;       // Khiếu nại đã được xử lý thỏa đáng và khách hàng đồng thuận
    public string? ResolvedBy { get; set; }                      // Người xác nhận giải quyết khiếu nại
    public DateTime? ResolvedAt { get; set; }                    // Thời điểm giải quyết xong khiếu nại

    // ===== Trạng thái & Nhật ký kiểm toán =====
    public string Status { get; set; } = "Draft";                // Draft → Pending → InProgress → Completed (hoặc Unreachable / Escalated / Cancelled)
    public string? Remark { get; set; }                          // Ghi chú nghiệp vụ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? EscalatedBy { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public string? EscalateReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết Câu hỏi &amp; Trả lời khảo sát chuyên sâu theo chuẩn 2010.HTC DLS_VINSurvey (BizHTC.DealerSales / SalesSatisfactionSurveyQuestionLine): lưu vết chi tiết 29 câu hỏi nghiệp vụ khảo sát chuẩn hóa theo từng nhóm tiêu chí.</summary>
public sealed class SalesSatisfactionSurveyQuestionLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesSatisfactionSurveyId { get; set; }
    public string SurveyNo { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string QuestionCode { get; set; } = "";               // Mã câu hỏi chuẩn: Q01_GREETING, Q02_NEEDS_ANALYSIS, Q03_TEST_DRIVE, Q04_PRICE_TRANSPARENCY, Q05_CONTRACT_EXPLANATION, Q06_PAYMENT_FINANCE, Q07_DELIVERY_TIMELINESS, Q08_CLEAN_CONDITION, Q09_CEREMONY, Q10_FEATURE_DEMO, Q11_ADAS_DEMO, Q12_AVN_BLUELINK, Q13_DOCS_WARRANTY, Q14_FOLLOW_UP_PROMISE, Q15_SHOWROOM_COMFORT, Q16_SALES_ATTITUDE, Q17_OVERALL_EXPERIENCE, Q18_NPS_RECOMMEND...
    public string QuestionCategory { get; set; } = "SalesConsultant"; // SalesConsultant (TVBH), Facility (Cơ sở vật chất), DeliveryProcess (Bàn giao xe), PaperworkFinance (Hồ sơ tài chính), Timeliness (Đúng hẹn), TechnologyADAS (Công nghệ & An toàn), OverallNPS (Tổng thể & Giới thiệu)
    public string QuestionText { get; set; } = "";               // Nội dung câu hỏi phỏng vấn
    public decimal Score { get; set; } = 5.0m;                   // Điểm đánh giá (1.0 - 5.0 hoặc thang 10)
    public string? AnswerText { get; set; }                      // Câu trả lời (Có / Không / Đạt / Chưa Đạt / Ý kiến khách)
    public decimal Weight { get; set; } = 1.0m;                  // Trọng số câu hỏi
    public string? Remark { get; set; }                          // Ghi chú câu hỏi
}

// ===== DTOs cho Khảo sát Chỉ số Hài lòng Bán hàng SSI (BizHTC.DealerSales / DLS_VINSurvey & SalesSatisfactionSurvey) =====

public sealed record CreateSalesSatisfactionSurveyDto(
    string? SurveyNo,
    string? SurveyNoUser,
    string? DealNo,
    string Vin,
    string? Model,
    string? SpecCode,
    string? EngineNo,
    string? Color,
    string? PlateNo,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    string? CustomerType,
    string? DealerCode,
    string? DealerName,
    string? SalesConsultantCode,
    string? SalesConsultantName,
    DateTime? DeliveryDate,
    DateTime? SurveyDate,
    string? ContactChannel,
    string? SurveyorStaff,
    string? Remark,
    string? CreatedBy,
    List<SsiQuestionInputDto>? Questions
);

public sealed record SsiQuestionInputDto(
    string QuestionCode,
    string? QuestionCategory,
    string QuestionText,
    decimal Score,
    string? AnswerText,
    decimal? Weight,
    string? Remark
);

public sealed record UpdateSalesSatisfactionSurveyDto(
    string? SurveyNoUser,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    string? PlateNo,
    string? SalesConsultantCode,
    string? SalesConsultantName,
    DateTime? DeliveryDate,
    DateTime? SurveyDate,
    string? ContactChannel,
    string? SurveyorStaff,
    decimal? ScoreSalesConsultant,
    decimal? ScoreDealershipFacility,
    decimal? ScoreDeliveryProcess,
    decimal? ScorePaperworkFinance,
    decimal? ScoreTimeliness,
    decimal? ScoreOverall,
    bool? IsCleanCarDelivered,
    bool? IsFeatureExplained,
    bool? IsAdasExplained,
    bool? IsAvnBluelinkSetup,
    bool? IsOriginalDocsHandedOver,
    bool? IsFollowUpCallPromised,
    int? NpsScore,
    string? CustomerFeedback,
    bool? HasComplaint,
    string? ComplaintCategory,
    string? ComplaintDetails,
    string? RemedyAction,
    bool? IsComplaintResolved,
    string? Remark
);

public sealed record CompleteSalesSatisfactionSurveyDto(
    decimal? ScoreSalesConsultant,
    decimal? ScoreDealershipFacility,
    decimal? ScoreDeliveryProcess,
    decimal? ScorePaperworkFinance,
    decimal? ScoreTimeliness,
    decimal? ScoreOverall,
    bool? IsCleanCarDelivered,
    bool? IsFeatureExplained,
    bool? IsAdasExplained,
    bool? IsAvnBluelinkSetup,
    bool? IsOriginalDocsHandedOver,
    bool? IsFollowUpCallPromised,
    int? NpsScore,
    string? CustomerFeedback,
    bool? HasComplaint,
    string? ComplaintCategory,
    string? ComplaintDetails,
    string? RemedyAction,
    bool? IsComplaintResolved,
    string? SurveyorStaff,
    string? CompletedBy,
    string? Remark
);

public sealed record RecordSsiContactAttemptDto(
    string? ContactChannel,
    string? SurveyorStaff,
    string? AttemptNote,
    bool IsReached,
    DateTime? AttemptDate,
    string? Actor
);

public sealed record EscalateSsiSurveyDto(
    string ComplaintCategory,
    string ComplaintDetails,
    string EscalateReason,
    string? RecommendedRemedy,
    string? EscalatedBy
);

public sealed record ResolveSsiComplaintDto(
    string RemedyAction,
    string? CustomerAgreementNote,
    string? ResolvedBy,
    DateTime? ResolvedAt
);

public sealed record SalesSatisfactionSurveyTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record UpdateSsiQuestionLineDto(
    decimal? Score,
    string? AnswerText,
    decimal? Weight,
    string? Remark
);

public sealed record SalesSatisfactionSummaryDto(
    int TotalSurveys,
    int TotalCompleted,
    int TotalPending,
    int TotalInProgress,
    int TotalUnreachable,
    int TotalEscalated,
    int TotalCancelled,
    decimal AverageSsiScore,
    int AverageSsiIndex1000,
    decimal AverageScoreConsultant,
    decimal AverageScoreFacility,
    decimal AverageScoreDelivery,
    decimal AverageScorePaperwork,
    decimal AverageScoreTimeliness,
    decimal AverageScoreOverall,
    int TotalPromoters,
    int TotalPassives,
    int TotalDetractors,
    decimal NpsScorePercent,
    decimal CsatSatisfactionRatePercent,
    int TotalComplaints,
    int TotalResolvedComplaints,
    decimal ComplaintResolutionRatePercent,
    List<SsiDealerStatsDto> ByDealer,
    List<SsiModelStatsDto> ByModel,
    List<SsiSalesConsultantStatsDto> BySalesConsultant,
    List<SsiComplaintCategoryStatsDto> ByComplaintCategory
);

public sealed record SsiDealerStatsDto(
    string DealerCode,
    string DealerName,
    int TotalSurveys,
    int CompletedCount,
    decimal AverageSsiScore,
    int AverageSsiIndex1000,
    decimal NpsPercent,
    decimal CsatPercent,
    int ComplaintCount
);

public sealed record SsiModelStatsDto(
    string Model,
    int TotalSurveys,
    int CompletedCount,
    decimal AverageSsiScore,
    int AverageSsiIndex1000,
    decimal NpsPercent,
    decimal CsatPercent
);

public sealed record SsiSalesConsultantStatsDto(
    string SalesConsultantCode,
    string SalesConsultantName,
    string DealerCode,
    int TotalSurveys,
    decimal AverageScore,
    int AverageIndex1000,
    int PromoterCount,
    int ComplaintCount
);

public sealed record SsiComplaintCategoryStatsDto(
    string ComplaintCategory,
    string CategoryName,
    int TotalCount,
    int ResolvedCount,
    decimal ResolutionRatePercent
);

public sealed record VehicleSsiInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? PlateNo,
    string? OwnerName,
    string? DealerCode,
    string? LastSsiNo,
    DateTime? LastSsiDate,
    decimal? LastSsiScore,
    int? LastSsiIndex1000,
    int SsiSurveyCount,
    SalesSatisfactionSurvey? LastSurvey,
    List<SalesSatisfactionSurvey> SurveyHistory
);

// ===== Quản lý Khách hàng Tham quan Showroom / Tiếp đón khách xem xe tại Đại lý & Phân tích Phễu Bán hàng (BizHTC.RetailContract / DLR_CtmVisit, FrmCusVisit, RptPivot_DlrCtmVisit / CustomerVisit) =====

/// <summary>Phiếu Tiếp đón & Quản lý Khách hàng Tham quan Showroom Đại lý (BizHTC.RetailContract / DLR_CtmVisit / CustomerVisit): ghi nhận khách hàng đến showroom xem xe, lái thử, nhu cầu dòng xe quan tâm, xe cũ đổi xe mới Trade-in, TVBH tiếp đón, ngân sách dự kiến, nguồn khách hàng và theo dõi phễu bán hàng Showroom Traffic.</summary>
public sealed class CustomerVisit
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string VisitCode { get; set; } = "";             // Mã lượt khách tham quan (VIS-2026-03-0001, VIS...)
    public string? VisitCodeUser { get; set; }            // Ký hiệu tham chiếu phiếu nội bộ đại lý
    public string DealerCode { get; set; } = "";          // Mã đại lý tiếp đón khách (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }               // Tên đại lý
    public DateTime VisitDate { get; set; } = DateTime.Now; // Ngày giờ khách vào showroom
    public string CustomerName { get; set; } = "";        // Họ tên khách hàng
    public string CustomerPhone { get; set; } = "";       // SĐT khách hàng
    public string? CustomerEmail { get; set; }            // Email khách hàng
    public string? CustomerAddress { get; set; }          // Địa chỉ khách hàng
    public string Gender { get; set; } = "Nam";           // Giới tính: Nam, Nữ, Khác
    public string RangeAgeCode { get; set; } = "26-35";   // Nhóm tuổi: 18-25, 26-35, 36-45, 46-55, Over55
    public string CustomerType { get; set; } = "Individual"; // Individual (Cá nhân), Corporate (Doanh nghiệp)

    // Nhu cầu xe quan tâm
    public string InterestedModel { get; set; } = "";     // Dòng xe quan tâm (SantaFe, Tucson, Accent, Creta, Custin, Stargazer, Palisade, Venue, Ioniq 5...)
    public string? SpecCode { get; set; }                 // Phiên bản xe quan tâm (1.5 AT Tiêu Chuẩn, 1.6T HTRAC, Calligraphy, EV...)
    public string? SpecDescription { get; set; }          // Mô tả chi tiết cấu hình
    public string? ColorCode { get; set; } = "NWAC";      // Mã màu sắc ưa thích (NWAC, SAW, R2P, T2X...)
    public string? ColorName { get; set; } = "Trắng ngọc trai";
    public string? Vin { get; set; }                      // Số khung VIN cụ thể nếu khách ưng ý xe có sẵn tại showroom

    // Mục đích & Nguồn gốc tiếp cận
    public string VisitPurpose { get; set; } = "XemXeMoi"; // XemXeMoi (Tham quan xe mới), LaiThu (Trải nghiệm lái thử), BaoGia (Đề nghị báo giá), DamPhanHopDong (Thương lượng hợp đồng), NhanBanGiaoXe (Nhận xe bàn giao), DichVuHauMai (Dịch vụ bảo dưỡng)
    public string LeadSource { get; set; } = "ShowroomWalkIn"; // ShowroomWalkIn (Khách vãng lai), DigitalAds (Quảng cáo online), WebsiteHyundai (Đăng ký web), Referral (Giới thiệu), RoadshowEvent (Sự kiện lưu động), Hotline (Tổng đài đại lý), Other (Khác)

    // Nhân sự tiếp đón & Dự định tài chính
    public string? SalesConsultantCode { get; set; }      // Mã tư vấn bán hàng TVBH tiếp đón
    public string? SalesConsultantName { get; set; }      // Tên tư vấn bán hàng TVBH
    public bool HasTradeIn { get; set; } = false;         // Có nhu cầu đổi xe cũ lấy xe mới Trade-in
    public string? TradeInModel { get; set; }             // Dòng xe cũ muốn đổi (VD: Grand i10 2018, Accent 2020...)
    public int? TradeInYear { get; set; }                 // Năm sản xuất xe cũ
    public decimal TradeInEstimatedPrice { get; set; } = 0; // Giá định giá xe cũ ước tính (VNĐ)
    public string PaymentMethodExpected { get; set; } = "Cash"; // Cash (Tiền mặt/Chuyển khoản), BankLoan (Trả góp ngân hàng), Installment (Kỳ hạn)
    public decimal LoanPercentExpected { get; set; } = 0; // Tỷ lệ vay ngân hàng dự kiến (%) (VD: 70%, 80%)
    public string EstimatedPurchaseTime { get; set; } = "TrongThang"; // TrongTuan (Trong 7 ngày), TrongThang (Trong 30 ngày), 1Den3Thang (1-3 tháng), ThamKhao (Chỉ tham khảo/Chưa xác định)
    public string PurchaseProbability { get; set; } = "High"; // VeryHigh (Rất cao - Hot Lead), High (Cao - Warm Lead), Medium (Trung bình - Cool Lead), Low (Thấp - Cold Lead)
    public decimal BudgetAmount { get; set; } = 0;        // Ngân sách dự kiến của khách (VNĐ)
    public string? CompetitorModel { get; set; }          // Dòng xe đối thủ đang cân nhắc (Mazda CX-5, Ford Territory, Honda CR-V, Toyota Vios...)

    // Trải nghiệm & Chuyển đổi
    public bool IsTestDriveTaken { get; set; } = false;   // Đã lái thử trực tiếp trong buổi đến showroom
    public string? LinkedDriveTestCode { get; set; }      // Mã phiếu lái thử liên kết (CustomerTestDrive)
    public string? LinkedDealNo { get; set; }             // Mã hợp đồng bán lẻ sinh ra nếu chốt deal ngay (DealerDeal)
    public DateTime? NextFollowUpDate { get; set; }       // Lịch hẹn liên hệ chăm sóc lại tiếp theo
    public string? FollowUpAction { get; set; } = "CallBack"; // CallBack (Gọi điện chăm sóc), SendQuotation (Gửi báo giá), ScheduleTestDrive (Hẹn lái thử), InviteShowroomEvent (Mời sự kiện), NegotiateContract (Đàm phán HĐ)
    public string? CustomerFeedback { get; set; }         // Ý kiến trao đổi & cảm nhận của khách hàng

    // Trạng thái & Kiểm toán
    public string Status { get; set; } = "CheckedIn";     // CheckedIn (Đang tiếp đón tại showroom), InConsultation (Đang tư vấn chi tiết), Quoted (Đã gửi báo giá), ConvertedToDeal (Đã ký hợp đồng/chốt cọc), ScheduledFollowUp (Đã hẹn lịch chăm sóc), ClosedLost (Không mua/mua đối thủ), Cancelled (Hủy lượt)
    public string? Remark { get; set; }                   // Ghi chú nghiệp vụ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CompletedBy { get; set; }              // Người xác nhận hoàn tất / chốt hồ sơ
    public DateTime? CompletedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Nhật ký Tiến trình Chăm sóc & Follow-up Khách hàng sau khi rời Showroom (BizHTC.RetailContract / CustomerVisitActionLog): ghi nhận từng lần gọi điện, gửi báo giá, lái thử tại nhà, đàm phán giá và chốt hợp đồng.</summary>
public sealed class CustomerVisitActionLog
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CustomerVisitId { get; set; }
    public string VisitCode { get; set; } = "";
    public string ActionNo { get; set; } = "";             // Mã hành động (ACT-2026-0001...)
    public int LineIndex { get; set; } = 1;
    public string ActionType { get; set; } = "ShowroomGreeting"; // ShowroomGreeting (Tiếp đón showroom), ProductConsultation (Tư vấn sản phẩm), TestDriveDone (Thực hiện lái thử), QuotationSent (Gửi bảng giá), FollowUpCall (Gọi điện chăm sóc), HomeVisit (Gặp khách tại nhà), PriceNegotiation (Đàm phán giá), DealClosed (Chốt hợp đồng), LostNote (Ghi nhận mất khách)
    public DateTime ActionDate { get; set; } = DateTime.Now; // Ngày giờ thực hiện tương tác
    public string? SalesConsultant { get; set; }          // TVBH thực hiện
    public string DiscussionSummary { get; set; } = "";   // Tóm tắt nội dung trao đổi
    public string? CustomerResponse { get; set; }         // Phản hồi của khách hàng
    public string? NextActionPlan { get; set; }           // Kế hoạch hành động tiếp theo
    public DateTime? NextActionDate { get; set; }         // Hạn thực hiện kế hoạch tiếp theo
    public string Status { get; set; } = "Completed";     // Completed, Pending, Cancelled
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// ===== DTOs cho Quản lý Khách hàng Tham quan Showroom & Phễu Bán hàng (CustomerVisit) =====

public sealed record CreateCustomerVisitDto(
    string? VisitCode,
    string? VisitCodeUser,
    string DealerCode,
    string? DealerName,
    string CustomerName,
    string CustomerPhone,
    string? CustomerEmail,
    string? CustomerAddress,
    string? Gender,
    string? RangeAgeCode,
    string? CustomerType,
    string InterestedModel,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    string? Vin,
    string? VisitPurpose,
    string? LeadSource,
    string? SalesConsultantCode,
    string? SalesConsultantName,
    bool? HasTradeIn,
    string? TradeInModel,
    int? TradeInYear,
    decimal? TradeInEstimatedPrice,
    string? PaymentMethodExpected,
    decimal? LoanPercentExpected,
    string? EstimatedPurchaseTime,
    string? PurchaseProbability,
    decimal? BudgetAmount,
    string? CompetitorModel,
    bool? IsTestDriveTaken,
    string? LinkedDriveTestCode,
    string? LinkedDealNo,
    DateTime? NextFollowUpDate,
    string? FollowUpAction,
    string? CustomerFeedback,
    string? Remark,
    string? CreatedBy,
    List<CustomerVisitActionInputDto>? InitialActions
);

public sealed record CustomerVisitActionInputDto(
    string ActionType,
    string DiscussionSummary,
    string? CustomerResponse,
    string? NextActionPlan,
    DateTime? NextActionDate,
    string? SalesConsultant,
    string? Status,
    string? Remark
);

public sealed record UpdateCustomerVisitDto(
    string? VisitCodeUser,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    string? CustomerAddress,
    string? Gender,
    string? RangeAgeCode,
    string? CustomerType,
    string? InterestedModel,
    string? SpecCode,
    string? SpecDescription,
    string? ColorCode,
    string? ColorName,
    string? Vin,
    string? VisitPurpose,
    string? LeadSource,
    string? SalesConsultantCode,
    string? SalesConsultantName,
    bool? HasTradeIn,
    string? TradeInModel,
    int? TradeInYear,
    decimal? TradeInEstimatedPrice,
    string? PaymentMethodExpected,
    decimal? LoanPercentExpected,
    string? EstimatedPurchaseTime,
    string? PurchaseProbability,
    decimal? BudgetAmount,
    string? CompetitorModel,
    bool? IsTestDriveTaken,
    string? LinkedDriveTestCode,
    string? LinkedDealNo,
    DateTime? NextFollowUpDate,
    string? FollowUpAction,
    string? CustomerFeedback,
    string? Remark
);

public sealed record CustomerVisitTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    string? LinkedDriveTestCode,
    string? LinkedDealNo,
    DateTime? TransitionDate
);

public sealed record RecordVisitFollowUpDto(
    string ActionType,
    string DiscussionSummary,
    string? CustomerResponse,
    string? NextActionPlan,
    DateTime? NextActionDate,
    string? SalesConsultant,
    string? NewStatus,
    string? NewPurchaseProbability,
    DateTime? NextFollowUpDate,
    string? FollowUpAction,
    string? Actor,
    string? Remark
);

public sealed record ConvertToTestDriveDto(
    string? DriveTestCode,
    string? DrvTestPlateNo,
    string? DriverLicenseNo,
    string? LicenseClass,
    string? DriveTestType,
    string? RoutePath,
    DateTime? DriveDTime,
    int? DurationMinutes,
    int? OdoStart,
    string? Instructor,
    string? SalesManCode,
    string? SalesManName,
    string? Actor,
    string? Remark
);

public sealed record ConvertToDealDto(
    string? DealNo,
    string? DealNoUser,
    decimal UnitPrice,
    decimal? Discount,
    decimal? DepositAmount,
    string? PaymentType,
    string? BankCode,
    decimal? LoanAmount,
    string? Vin,
    string? SalesManCode,
    string? SalesManName,
    string? Actor,
    string? Remark
);

public sealed record UpdateVisitActionLogDto(
    string? ActionType,
    string? DiscussionSummary,
    string? CustomerResponse,
    string? NextActionPlan,
    DateTime? NextActionDate,
    string? SalesConsultant,
    string? Status,
    string? Remark
);

public sealed record CustomerVisitSummaryDto(
    int TotalVisits,
    int TotalCheckedIn,
    int TotalInConsultation,
    int TotalQuoted,
    int TotalConvertedToDeal,
    int TotalScheduledFollowUp,
    int TotalClosedLost,
    int TotalCancelled,
    int TotalTestDriveTaken,
    int TotalTradeInRequested,
    int TotalHighPotentialVisits,
    decimal OverallConversionRatePercent,
    decimal TotalBudgetAmount,
    decimal AverageBudgetAmount,
    List<VisitModelStatsDto> ByModel,
    List<VisitDealerStatsDto> ByDealer,
    List<VisitLeadSourceStatsDto> ByLeadSource,
    List<VisitSalesConsultantStatsDto> BySalesConsultant,
    List<VisitPurchaseTimeStatsDto> ByPurchaseTime
);

public sealed record VisitModelStatsDto(string Model, int TotalVisits, int ConvertedCount, int TestDriveCount, decimal ConversionRatePercent);
public sealed record VisitDealerStatsDto(string DealerCode, string DealerName, int TotalVisits, int ConvertedCount, int TestDriveCount, decimal ConversionRatePercent);
public sealed record VisitLeadSourceStatsDto(string LeadSource, string LeadSourceName, int TotalVisits, int ConvertedCount, decimal ConversionRatePercent);
public sealed record VisitSalesConsultantStatsDto(string SalesConsultantCode, string SalesConsultantName, string DealerCode, int TotalVisits, int ConvertedCount, decimal ConversionRatePercent);
public sealed record VisitPurchaseTimeStatsDto(string EstimatedPurchaseTime, string PurchaseTimeName, int TotalVisits, int ConvertedCount);

public sealed record ShowroomFunnelAnalyticsDto(
    string? DealerCode,
    int TotalTraffic,
    int Step1_CheckedIn,
    int Step2_InConsultation,
    int Step3_TestDriveTaken,
    int Step4_Quoted,
    int Step5_ConvertedToDeal,
    decimal FunnelConversionRatePercent,
    List<ShowroomFunnelStageDto> Stages
);

public sealed record ShowroomFunnelStageDto(
    int StageOrder,
    string StageCode,
    string StageName,
    int Count,
    decimal ConversionFromPreviousPercent,
    decimal ConversionFromTotalPercent
);

public sealed record VehicleVisitInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? LastCustomerVisitNo,
    DateTime? LastCustomerVisitDate,
    int CustomerVisitCount,
    CustomerVisit? LastVisit,
    List<CustomerVisit> VisitHistory
);

public sealed record CustomerVisitHistoryDto(
    string CustomerPhone,
    string CustomerName,
    int TotalVisits,
    int ConvertedDealsCount,
    List<CustomerVisit> Visits
);

// ===== Quản lý Đề nghị & Quyết toán Chi phí Hỗ trợ Hoạt động Marketing Đại lý Phân phối OEM (BizHTC.Marketing / MKT_MarketingFee, MKT_MarketingFeeDetail, MKT_MarketingFeeDetailAttach, Mst_MarketingActivity, Mst_MarketingActivityType) =====

/// <summary>Danh mục Nhóm / Loại hình hoạt động Marketing (BizHTC.Marketing / Mst_MarketingActivityType): phân loại các nhóm quảng bá (OOH, Digital, Event/Roadshow, POSM Showroom, PR Báo chí, Radio VOV...).</summary>
public sealed class MarketingActivityType
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MKTActivityTypeCode { get; set; } = ""; // Mã nhóm hoạt động (OOH, DIGITAL, EVENT, POSM, PR_MEDIA, RADIO_VOV, SPONSOR, ROADSHOW)
    public string MKTActivityTypeName { get; set; } = ""; // Tên nhóm hoạt động
    public bool FlagActive { get; set; } = true;          // Đang hoạt động
    public string? Remark { get; set; }                   // Ghi chú
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Danh mục Hoạt động Marketing chuẩn Hãng OEM (BizHTC.Marketing / Mst_MarketingActivity): định nghĩa từng hoạt động cụ thể, đơn giá định mức tài trợ tối đa OEM (HTCLimitPrice) và yêu cầu 4 loại chứng từ bắt buộc (Maket thiết kế, Ảnh nghiệm thu, Hợp đồng bên thứ 3, Hóa đơn GTGT).</summary>
public sealed class MarketingActivity
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MKTActivityCode { get; set; } = "";     // Mã hoạt động chuẩn (OOH_BILLBOARD, DIGI_FACEBOOK_ADS, DIGI_GOOGLE_SEARCH, EVENT_TEST_DRIVE, POSM_SHOWROOM_STANDEE, PR_PRESS_ARTICLE, VOV_TRAFFIC_RADIO, TIKTOK_KOL_REVIEW...)
    public string MKTActivityName { get; set; } = "";     // Tên hoạt động marketing
    public string MKTActivityTypeCode { get; set; } = "DIGITAL"; // Mã nhóm hoạt động
    public decimal DefaultHTCLimitPrice { get; set; } = 0;// Định mức trần kinh phí OEM chấp thuận tài trợ cho 1 đơn vị (VNĐ)
    public bool FlagDesignImage { get; set; } = true;     // Bắt buộc nộp File/Maket thiết kế duyệt chuẩn nhận diện CI/VI
    public bool FlagActualImage { get; set; } = true;     // Bắt buộc nộp Ảnh/Video chụp nghiệm thu hiện trường
    public bool FlagContract { get; set; } = true;        // Bắt buộc nộp Hợp đồng thuê đơn vị thực hiện / Agency
    public bool FlagInvoice { get; set; } = true;         // Bắt buộc nộp Hóa đơn GTGT đầu vào hợp lệ
    public bool FlagActive { get; set; } = true;          // Đang áp dụng
    public string? Remark { get; set; }                   // Ghi chú quy chuẩn
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Hồ sơ Đề nghị & Quyết toán Chi phí Hỗ trợ Marketing Đại lý Phân phối OEM (BizHTC.Marketing.MKT_MarketingFee / MarketingFeeSettlement): quản lý hồ sơ xin phê duyệt và quyết toán kinh phí tài trợ hoạt động tiếp thị từ hãng xe OEM cho Đại lý phân phối ủy quyền.</summary>
public sealed class MarketingFeeSettlement
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string MKTFeeCode { get; set; } = "";          // Mã phiếu quyết toán Marketing (MKT-2026-HN01-0001, MKT...)
    public string? MKTFeeCodeUser { get; set; }         // Số hiệu hồ sơ nội bộ tham chiếu của Đại lý
    public string MKTFeeName { get; set; } = "";          // Tên chương trình / chiến dịch Marketing (Chiến dịch Quảng bá Ra mắt SantaFe All-New, Lái thử Creta & Custin...)
    public string DealerCode { get; set; } = "";          // Mã đại lý đề xuất kinh phí hỗ trợ (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }               // Tên đại lý
    public string CampaignMonth { get; set; } = "";       // Kỳ / Tháng ngân sách Marketing (YYYY-MM, ví dụ: 2026-03)
    public DateTime DateStart { get; set; } = DateTime.Now; // Ngày bắt đầu diễn ra hoạt động
    public DateTime DateEnd { get; set; } = DateTime.Now;   // Ngày kết thúc hoạt động
    public int TotalActivityCount { get; set; } = 0;      // Tổng số hạng mục hoạt động trong hồ sơ
    public decimal TotalAmountDealer { get; set; } = 0;   // Tổng kinh phí Đại lý đề nghị tài trợ trước thuế (VNĐ)
    public decimal TotalAmountApproved { get; set; } = 0; // Tổng kinh phí Hãng xe OEM phê duyệt tài trợ trước thuế (VNĐ)
    public decimal VatRate { get; set; } = 10m;           // Thuế suất GTGT VAT (%) (mặc định 10%)
    public decimal TotalVatAmount { get; set; } = 0;      // Tiền thuế VAT = TotalAmountApproved * VatRate / 100 (VNĐ)
    public decimal TotalAmountAfterVAT { get; set; } = 0; // Tổng tiền thanh toán sau thuế = TotalAmountApproved + TotalVatAmount (VNĐ)
    public string Status { get; set; } = "Draft";         // Draft → Submitted (Pending) → Approved → Finished (Settled) (hoặc Rejected / Cancelled)
    public string? BankRefNo { get; set; }                // Số ủy nhiệm chi UNC / Mã giao dịch ngân hàng giải ngân
    public DateTime? SettledDate { get; set; }            // Ngày thực tế chuyển khoản giải ngân chi phí marketing
    public string? SettledBy { get; set; }                // Kế toán thanh toán xác nhận giải ngân
    public string? ApprovedBy { get; set; }               // Lãnh đạo Marketing OEM duyệt quyết toán
    public DateTime? ApprovedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public string? Remark { get; set; }                   // Ghi chú hồ sơ
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Chi tiết dòng hoạt động Marketing trong hồ sơ quyết toán (BizHTC.Marketing.MKT_MarketingFeeDetail / MarketingFeeDetail): ghi nhận từng hạng mục, số lượng, đơn giá đề nghị, định mức trần OEM, số lượng & số tiền được duyệt, liên kết số khung xe VIN / Model tiêu điểm và trạng thái kiểm tra 4 loại chứng từ bắt buộc.</summary>
public sealed class MarketingFeeDetail
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long MarketingFeeSettlementId { get; set; }
    public string MKTFeeCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng (1, 2, 3...)
    public string MKTActivityCode { get; set; } = "";     // Mã hoạt động marketing (OOH_BILLBOARD, DIGI_FACEBOOK_ADS...)
    public string MKTActivityName { get; set; } = "";     // Tên hoạt động marketing
    public string MKTActivityTypeCode { get; set; } = "DIGITAL"; // Phân loại nhóm hoạt động (OOH, DIGITAL, EVENT...)
    public string? Vin { get; set; }                      // Số khung xe VIN tiêu điểm quảng bá (nếu có)
    public string? Model { get; set; }                    // Dòng xe tiêu điểm (SantaFe, Tucson, Accent, Creta, Custin, Ioniq 5...)
    public string? SpecCode { get; set; }                 // Phiên bản xe
    public decimal Qty { get; set; } = 1;                 // Số lượng thực hiện đề nghị (biển, bài, tháng, sự kiện...)
    public decimal Price { get; set; } = 0;               // Đơn giá thực hiện thực tế của Đại lý (VNĐ)
    public decimal TotalAmountDealer { get; set; } = 0;   // Thành tiền Đại lý đề nghị = Qty * Price (VNĐ)
    public decimal HTCLimitPrice { get; set; } = 0;       // Định mức trần kinh phí OEM chấp thuận hỗ trợ cho 1 đơn vị (VNĐ)
    public decimal ApprovedQty { get; set; } = 0;         // Số lượng OEM nghiệm thu duyệt
    public decimal ApprovedAmount { get; set; } = 0;      // Số tiền OEM phê duyệt tài trợ = min(TotalAmountDealer, ApprovedQty * HTCLimitPrice) (VNĐ)

    // Cờ quy chuẩn kiểm tra 4 loại chứng từ
    public bool FlagDesignImage { get; set; } = true;     // Yêu cầu nộp Maket thiết kế duyệt chuẩn CI/VI
    public bool FlagActualImage { get; set; } = true;     // Yêu cầu nộp Ảnh/Video chụp nghiệm thu hiện trường
    public bool FlagContract { get; set; } = true;        // Yêu cầu nộp Hợp đồng bên thứ 3
    public bool FlagInvoice { get; set; } = true;         // Yêu cầu nộp Hóa đơn GTGT đầu vào
    public bool HasDesignImage { get; set; } = false;     // Đã có chứng từ Maket thiết kế hợp lệ
    public bool HasActualImage { get; set; } = false;     // Đã có chứng từ Ảnh/Video nghiệm thu hiện trường hợp lệ
    public bool HasContract { get; set; } = false;        // Đã có Hợp đồng bên thứ 3 hợp lệ
    public bool HasInvoice { get; set; } = false;         // Đã có Hóa đơn GTGT đầu vào hợp lệ

    public string Status { get; set; } = "Pending";       // Pending → Approved → Finished (hoặc Rejected)
    public string? RejectReason { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Tài liệu & Chứng từ nghiệm thu Marketing đính kèm (BizHTC.Marketing.MKT_MarketingFeeDetailAttach / MarketingFeeDetailAttach): lưu trữ tệp đính kèm maket thiết kế, ảnh chụp thực tế, hợp đồng, hóa đơn VAT và báo cáo KPI nghiệm thu.</summary>
public sealed class MarketingFeeDetailAttach
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long MarketingFeeDetailId { get; set; }
    public string MKTFeeCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string AttachCode { get; set; } = "";          // Mã chứng từ (ATT-MKT-2026-0001...)
    public string FileType { get; set; } = "ActualImage"; // DesignImage, ActualImage, Contract, Invoice, ReportAnalytics, AcceptanceMinutes
    public string FileName { get; set; } = "";            // Tên tệp tin (Maket_Billboard_SantaFe_2026.png, HĐ_Agency_VN01.pdf...)
    public string? FilePath { get; set; }                 // Đường dẫn lưu trữ / URL tệp
    public long FileSizeKb { get; set; } = 1024;          // Dung lượng tệp (KB)
    public string Status { get; set; } = "Approved";      // Pending → Approved / Rejected
    public string? ApprovedBy { get; set; }               // Chuyên viên Marketing thẩm định duyệt tệp
    public DateTime? ApprovedAt { get; set; }
    public string? Remark { get; set; }                   // Nhận xét kiểm tra chứng từ
    public DateTime UploadedAt { get; set; } = DateTime.Now;
}

// ===== DTOs cho Quản lý Đề nghị & Quyết toán Chi phí Hỗ trợ Marketing (BizHTC.Marketing / MKT_MarketingFee) =====

public sealed record CreateMarketingActivityTypeDto(
    string MKTActivityTypeCode,
    string MKTActivityTypeName,
    bool? FlagActive,
    string? Remark
);

public sealed record CreateMarketingActivityDto(
    string MKTActivityCode,
    string MKTActivityName,
    string MKTActivityTypeCode,
    decimal? DefaultHTCLimitPrice,
    bool? FlagDesignImage,
    bool? FlagActualImage,
    bool? FlagContract,
    bool? FlagInvoice,
    bool? FlagActive,
    string? Remark
);

public sealed record UpdateMarketingActivityDto(
    string? MKTActivityName,
    string? MKTActivityTypeCode,
    decimal? DefaultHTCLimitPrice,
    bool? FlagDesignImage,
    bool? FlagActualImage,
    bool? FlagContract,
    bool? FlagInvoice,
    bool? FlagActive,
    string? Remark
);

public sealed record CreateMarketingFeeDto(
    string? MKTFeeCode,
    string? MKTFeeCodeUser,
    string MKTFeeName,
    string DealerCode,
    string? DealerName,
    string CampaignMonth,
    DateTime? DateStart,
    DateTime? DateEnd,
    decimal? VatRate,
    string? Remark,
    string? CreatedBy,
    List<MarketingFeeDetailInputDto>? Details
);

public sealed record MarketingFeeDetailInputDto(
    string MKTActivityCode,
    string? MKTActivityName,
    string? MKTActivityTypeCode,
    string? Vin,
    string? Model,
    string? SpecCode,
    decimal Qty,
    decimal Price,
    decimal? HTCLimitPrice,
    bool? FlagDesignImage,
    bool? FlagActualImage,
    bool? FlagContract,
    bool? FlagInvoice,
    string? Remark,
    List<MarketingFeeAttachInputDto>? Attachments
);

public sealed record MarketingFeeAttachInputDto(
    string FileType,
    string FileName,
    string? FilePath,
    long? FileSizeKb,
    string? Remark
);

public sealed record UpdateMarketingFeeHeaderDto(
    string? MKTFeeCodeUser,
    string? MKTFeeName,
    string? DealerName,
    string? CampaignMonth,
    DateTime? DateStart,
    DateTime? DateEnd,
    decimal? VatRate,
    string? Remark
);

public sealed record UpdateMarketingFeeDetailDto(
    string? MKTActivityCode,
    string? MKTActivityName,
    string? Vin,
    string? Model,
    string? SpecCode,
    decimal? Qty,
    decimal? Price,
    decimal? HTCLimitPrice,
    bool? FlagDesignImage,
    bool? FlagActualImage,
    bool? FlagContract,
    bool? FlagInvoice,
    string? Remark
);

public sealed record UpdateHTCLimitPriceDto(
    decimal HTCLimitPrice,
    string? Note
);

public sealed record MarketingFeeTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    string? BankRefNo,
    DateTime? SettledDate
);

public sealed record AddMarketingFeeAttachDto(
    string FileType,
    string FileName,
    string? FilePath,
    long? FileSizeKb,
    string? Remark,
    string? Actor
);

public sealed record ReviewMarketingFeeAttachDto(
    bool Approved,
    string? Actor,
    string? Remark
);

public sealed record MarketingFeeSummaryDto(
    int TotalSettlements,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalFinished,
    int TotalCancelled,
    int TotalActivities,
    decimal TotalAmountDealer,
    decimal TotalAmountApproved,
    decimal TotalVatAmount,
    decimal TotalAmountAfterVAT,
    decimal TotalSettledAmount,
    decimal ApprovalRatePercent,
    List<MarketingFeeDealerStatsDto> ByDealer,
    List<MarketingFeeActivityStatsDto> ByActivityType,
    List<MarketingFeeModelStatsDto> ByModel,
    List<MarketingFeeMonthStatsDto> ByMonth
);

public sealed record MarketingFeeDealerStatsDto(string DealerCode, string DealerName, int SettlementCount, int ActivityCount, decimal TotalDealerAmount, decimal TotalApprovedAmount, decimal TotalSettledAmount);
public sealed record MarketingFeeActivityStatsDto(string ActivityTypeCode, string ActivityTypeName, int ActivityCount, decimal TotalDealerAmount, decimal TotalApprovedAmount);
public sealed record MarketingFeeModelStatsDto(string Model, int ActivityCount, decimal TotalApprovedAmount);
public sealed record MarketingFeeMonthStatsDto(string CampaignMonth, int SettlementCount, int ActivityCount, decimal TotalDealerAmount, decimal TotalApprovedAmount, decimal TotalSettledAmount);

public sealed record VehicleMarketingFeeInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    bool IsMktFeeSupported,
    decimal MktFeeSupportedAmount,
    string? LastMktFeeNo,
    DateTime? LastMktFeeDate,
    int MktFeeCount,
    List<MarketingFeeDetail> MarketingFeeLines
);

// ===== Quản lý Chỉ tiêu Bán hàng & KPI Doanh số Xe Ô tô Đại lý & Tư vấn Bán hàng TVBH (BizHTC.MasterData & DMS.NP.Biz / SP_KPIMonth, Mst_SMKPI, MngKPIMonthPresenter / SalesTargetKpi) =====

/// <summary>Danh mục Chỉ số KPI Bán hàng & Hiệu suất Hoạt động Đại lý Ô tô chuẩn Hãng (BizHTC.MasterData / Mst_SMKPI / SalesKpiIndicator): định nghĩa danh mục các chỉ số KPI chuẩn (Số cuộc gọi tiếp cận Leads, Lượt tiếp đón showroom, Lượt lái thử, Số báo giá, Hợp đồng ký mới, Xe bàn giao bán lẻ, HĐ bảo hiểm, Doanh số phụ kiện, Hồ sơ vay ngân hàng, Điểm SSI/CSI).</summary>
public sealed class SalesKpiIndicator
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string KPICode { get; set; } = "";             // Mã chỉ số KPI (KPI_CALL_LEADS, KPI_SHOWROOM_VISIT, KPI_TEST_DRIVE, KPI_QUOTATION, KPI_CONTRACT_SIGN, KPI_RETAIL_DELIVERY, KPI_INSURANCE_SOLD, KPI_ACCESSORIES_VAL, KPI_FINANCE_LOAN, KPI_CSI_SCORE)
    public string KPIName { get; set; } = "";             // Tên chỉ số KPI (Số cuộc gọi tiếp cận khách hàng tiềm năng, Lượt khách tham quan showroom, Lượt lái thử xe thực tế, Số báo giá gửi khách, Hợp đồng bán lẻ ký mới, Xe thực tế bàn giao cho khách, Hợp đồng bảo hiểm bán kèm, Doanh số phụ kiện bán lẻ, Hồ sơ vay ngân hàng giải ngân, Điểm hài lòng khách hàng SSI/CSI)
    public string KPICategory { get; set; } = "SalesVolume"; // Activity (Hoạt động phễu), SalesVolume (Sản lượng xe), Revenue (Doanh thu phụ trợ), Quality (Chất lượng & Hài lòng)
    public string Unit { get; set; } = "Xe";              // Đơn vị tính: Cuộc, Lượt, Xe, HĐ, VNĐ, Điểm
    public decimal Weight { get; set; } = 10.0m;          // Trọng số đánh giá chuẩn (%) trong bộ chỉ số tổng hợp
    public decimal TargetDefault { get; set; } = 10.0m;   // Định mức chỉ tiêu giao mặc định hàng tháng
    public bool FlagActive { get; set; } = true;          // Đang áp dụng trong hệ thống
    public string? Remark { get; set; }                   // Mô tả hướng dẫn đo lường chỉ số
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Kế hoạch Chỉ tiêu Bán hàng & KPI Doanh số Xe Ô tô theo Tháng / Quý (DMS.NP.Biz & BizHTC.MasterData / SP_KPIMonth / SalesTargetKpi): quản lý giao chỉ tiêu và nghiệm thu đánh giá hiệu suất kinh doanh cho Đại lý và từng Tư vấn bán hàng TVBH / Trưởng nhóm bán hàng.</summary>
public sealed class SalesTargetKpi
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string TargetCode { get; set; } = "";          // Mã kế hoạch chỉ tiêu KPI (KPI-2026-03-HN01-0001, KPI...)
    public string? TargetCodeUser { get; set; }         // Số hiệu kế hoạch nội bộ tham chiếu của Đại lý
    public string PeriodMonth { get; set; } = "";       // Kỳ / Tháng giao chỉ tiêu (YYYY-MM, ví dụ: 2026-03)
    public string PeriodQuarter { get; set; } = "Q1";   // Quý áp dụng (Q1, Q2, Q3, Q4)
    public int PeriodYear { get; set; } = 2026;         // Năm áp dụng
    public string DealerCode { get; set; } = "";        // Mã đại lý phân phối (DLR-HN01, DLR-HCM01...)
    public string? DealerName { get; set; }             // Tên đại lý
    public string UserCode { get; set; } = "ALL";       // Mã nhân viên TVBH / Trưởng nhóm (hoặc "ALL" / "DEALER_OVERALL" cho chỉ tiêu toàn đại lý)
    public string? UserName { get; set; } = "Toàn Đại Lý"; // Họ tên nhân viên TVBH / Trưởng nhóm
    public string Position { get; set; } = "SalesConsultant"; // SalesConsultant (Tư vấn bán hàng), TeamLeader (Trưởng nhóm bán hàng), SalesManager (Trưởng phòng bán hàng), DealerOverall (Toàn đại lý)

    // Chỉ tiêu & Thực đạt về Sản lượng & Doanh số Xe
    public int TargetCarCount { get; set; } = 0;        // Chỉ tiêu số lượng xe bán lẻ (xe)
    public int ActualCarCount { get; set; } = 0;        // Số lượng xe thực tế đã bàn giao (xe)
    public decimal CarCompletionRate { get; set; } = 0; // Tỷ lệ hoàn thành sản lượng xe (%) = ActualCarCount / TargetCarCount * 100
    public decimal TargetRevenue { get; set; } = 0;     // Chỉ tiêu tổng doanh thu bán xe (VNĐ)
    public decimal ActualRevenue { get; set; } = 0;     // Tổng doanh thu bán xe thực tế đạt được (VNĐ)
    public decimal RevenueCompletionRate { get; set; } = 0; // Tỷ lệ hoàn thành doanh thu (%) = ActualRevenue / TargetRevenue * 100

    // Chỉ tiêu & Thực đạt về Hoạt động phễu & Gia tăng giá trị
    public int TargetTestDriveCount { get; set; } = 0;  // Chỉ tiêu số lượt lái thử xe
    public int ActualTestDriveCount { get; set; } = 0;  // Số lượt lái thử xe thực tế
    public int TargetContractCount { get; set; } = 0;   // Chỉ tiêu số hợp đồng bán lẻ ký mới
    public int ActualContractCount { get; set; } = 0;   // Số hợp đồng bán lẻ thực tế ký mới
    public int TargetInsuranceCount { get; set; } = 0;  // Chỉ tiêu số hợp đồng bảo hiểm bán kèm
    public int ActualInsuranceCount { get; set; } = 0;  // Số hợp đồng bảo hiểm thực tế bán kèm
    public decimal TargetAccessoriesRevenue { get; set; } = 0; // Chỉ tiêu doanh thu phụ kiện bán lẻ (VNĐ)
    public decimal ActualAccessoriesRevenue { get; set; } = 0; // Doanh thu phụ kiện thực tế (VNĐ)

    // Đánh giá Tổng kết & Thưởng hiệu suất KPI
    public decimal OverallScore { get; set; } = 0;      // Điểm tổng hợp kết quả KPI cuối kỳ (Thang 0 - 100 điểm)
    public string KpiGrade { get; set; } = "Pending";   // Xếp loại thành tích: Pending, Excellent (>=110%), Good (100-109%), Pass (80-99%), Underperformed (<80%)
    public decimal BonusRate { get; set; } = 0;         // Tỷ lệ % thưởng vượt chỉ tiêu (%)
    public decimal BonusAmount { get; set; } = 0;       // Tổng tiền thưởng KPI đạt được trong kỳ (VNĐ)

    // Trạng thái & Kiểm toán
    public string Status { get; set; } = "Draft";       // Draft → Submitted (Pending) → Approved (Active) → Evaluated (Completed) (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                 // Ghi chú / Cam kết doanh số
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }             // Lãnh đạo Đại lý / Giám đốc Kinh doanh duyệt giao chỉ tiêu
    public DateTime? ApprovedAt { get; set; }
    public string? EvaluatedBy { get; set; }            // Trưởng bộ phận Kinh doanh / HR nghiệm thu đánh giá
    public DateTime? EvaluatedAt { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết Phân rã Chỉ tiêu Bán hàng theo từng Dòng xe Model / Phiên bản (DMS.NP.Biz / SP_KPIMonthDetail / SalesTargetKpiLine): giao số lượng xe, doanh thu dự kiến, theo dõi thực tế bàn giao và định mức hoa hồng thưởng theo từng model xe Hyundai.</summary>
public sealed class SalesTargetKpiLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesTargetKpiId { get; set; }
    public string TargetCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;               // Thứ tự dòng (1, 2, 3...)
    public string Model { get; set; } = "";               // Dòng xe phân bổ chỉ tiêu (SantaFe, Tucson, Accent, Creta, Grand i10, Custin, Stargazer, Palisade, Venue, Ioniq 5...)
    public string? SpecCode { get; set; }                 // Mã phiên bản xe (1.5 AT Tiêu Chuẩn, 2.0 AT Đặc Biệt, 2.5T AWD Calligraphy...)
    public int TargetQty { get; set; } = 1;               // Chỉ tiêu số lượng xe dòng này (xe)
    public int ActualQty { get; set; } = 0;               // Số lượng xe thực tế đã bàn giao (xe)
    public decimal CompletionRate { get; set; } = 0;      // Tỷ lệ hoàn thành sản lượng dòng xe (%) = ActualQty / TargetQty * 100
    public decimal TargetRevenue { get; set; } = 0;       // Chỉ tiêu doanh thu dự kiến dòng này (VNĐ)
    public decimal ActualRevenue { get; set; } = 0;       // Doanh thu thực tế đạt được dòng này (VNĐ)
    public decimal CommissionPerCar { get; set; } = 3000000m; // Định mức hoa hồng / thưởng nóng trên mỗi xe bán được (VNĐ/xe)
    public decimal BonusAmount { get; set; } = 0;         // Tổng tiền thưởng đạt được dòng này = ActualQty * CommissionPerCar (VNĐ)
    public string Status { get; set; } = "Pending";       // Pending → Approved → Evaluated (hoặc Cancelled)
    public string? Remark { get; set; }                   // Ghi chú định hướng bán hàng cho model xe
}

/// <summary>Nhật ký Tiến độ & Tích lũy Hoạt động Bán hàng Hàng ngày (DMS.NP.Biz / SP_KPIMonth_Daily / SalesKpiDailyLog): ghi nhận tiến độ thực hiện các chỉ số trong tháng (cuộc gọi, tiếp khách showroom, lái thử, báo giá, ký hợp đồng, bàn giao xe kèm số khung VIN).</summary>
public sealed class SalesKpiDailyLog
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesTargetKpiId { get; set; }
    public string TargetCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public DateTime LogDate { get; set; } = DateTime.Now; // Ngày phát sinh hoạt động
    public string KPICode { get; set; } = "KPI_RETAIL_DELIVERY"; // Mã chỉ số KPI liên quan
    public string KPIName { get; set; } = "Bàn giao xe bán lẻ cho khách";
    public decimal TargetDailyQty { get; set; } = 1;      // Chỉ tiêu tiến độ trong ngày
    public decimal ActualDailyQty { get; set; } = 1;      // Thực tế đạt được trong ngày
    public string? LinkedVin { get; set; }                // Số khung xe VIN liên quan nếu phát sinh ký cọc / giao xe
    public string? LinkedRefNo { get; set; }              // Mã số chứng từ liên quan (Mã HĐ Deal, Mã phiếu lái thử, Mã báo giá...)
    public string Notes { get; set; } = "";               // Diễn giải chi tiết hoạt động đạt được
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// ===== DTOs cho Quản lý Chỉ tiêu Bán hàng & KPI Doanh số Xe (SalesTargetKpi) =====

public sealed record CreateSalesKpiIndicatorDto(
    string KPICode,
    string KPIName,
    string? KPICategory,
    string? Unit,
    decimal? Weight,
    decimal? TargetDefault,
    bool? FlagActive,
    string? Remark
);

public sealed record UpdateSalesKpiIndicatorDto(
    string? KPIName,
    string? KPICategory,
    string? Unit,
    decimal? Weight,
    decimal? TargetDefault,
    bool? FlagActive,
    string? Remark
);

public sealed record CreateSalesTargetKpiDto(
    string? TargetCode,
    string? TargetCodeUser,
    string PeriodMonth,
    string? PeriodQuarter,
    int? PeriodYear,
    string DealerCode,
    string? DealerName,
    string? UserCode,
    string? UserName,
    string? Position,
    int? TargetCarCount,
    decimal? TargetRevenue,
    int? TargetTestDriveCount,
    int? TargetContractCount,
    int? TargetInsuranceCount,
    decimal? TargetAccessoriesRevenue,
    decimal? BonusRate,
    string? Remark,
    string? CreatedBy,
    List<SalesTargetKpiLineInputDto>? Lines,
    List<SalesKpiDailyLogInputDto>? DailyLogs
);

public sealed record SalesTargetKpiLineInputDto(
    string Model,
    string? SpecCode,
    int TargetQty,
    decimal? TargetRevenue,
    decimal? CommissionPerCar,
    string? Remark
);

public sealed record SalesKpiDailyLogInputDto(
    DateTime? LogDate,
    string KPICode,
    string? KPIName,
    decimal? TargetDailyQty,
    decimal ActualDailyQty,
    string? LinkedVin,
    string? LinkedRefNo,
    string? Notes
);

public sealed record UpdateSalesTargetKpiHeaderDto(
    string? TargetCodeUser,
    string? PeriodQuarter,
    int? PeriodYear,
    string? DealerName,
    string? UserName,
    string? Position,
    int? TargetCarCount,
    decimal? TargetRevenue,
    int? TargetTestDriveCount,
    int? TargetContractCount,
    int? TargetInsuranceCount,
    decimal? TargetAccessoriesRevenue,
    decimal? BonusRate,
    string? Remark
);

public sealed record UpdateSalesTargetKpiLineDto(
    string? Model,
    string? SpecCode,
    int? TargetQty,
    int? ActualQty,
    decimal? TargetRevenue,
    decimal? ActualRevenue,
    decimal? CommissionPerCar,
    string? Status,
    string? Remark
);

public sealed record SalesTargetKpiTransitionDto(
    string? Note,
    string? Actor,
    string? Reason,
    DateTime? TransitionDate
);

public sealed record EvaluateSalesTargetKpiDto(
    decimal? OverallScore,
    string? KpiGrade,
    decimal? BonusRate,
    decimal? BonusAmount,
    string? EvaluatedBy,
    string? Note
);

public sealed record AddSalesKpiDailyLogDto(
    DateTime? LogDate,
    string KPICode,
    string? KPIName,
    decimal? TargetDailyQty,
    decimal ActualDailyQty,
    string? LinkedVin,
    string? LinkedRefNo,
    string Notes,
    string? CreatedBy
);

public sealed record SalesKpiSummaryDto(
    int TotalPlans,
    int TotalDraft,
    int TotalSubmitted,
    int TotalApproved,
    int TotalEvaluated,
    int TotalCancelled,
    int TotalTargetCarCount,
    int TotalActualCarCount,
    decimal OverallCarCompletionRatePercent,
    decimal TotalTargetRevenue,
    decimal TotalActualRevenue,
    decimal OverallRevenueCompletionRatePercent,
    int TotalTestDrives,
    int TotalContracts,
    int TotalInsurances,
    decimal TotalAccessoriesRevenue,
    decimal TotalBonusAmount,
    List<SalesKpiDealerStatsDto> ByDealer,
    List<SalesKpiModelStatsDto> ByModel,
    List<SalesKpiConsultantStatsDto> ByConsultant,
    List<SalesKpiMonthStatsDto> ByPeriodMonth
);

public sealed record SalesKpiDealerStatsDto(string DealerCode, string DealerName, int PlanCount, int TargetCars, int ActualCars, decimal CompletionRate, decimal TargetRevenue, decimal ActualRevenue, decimal BonusAmount);
public sealed record SalesKpiModelStatsDto(string Model, int TargetQty, int ActualQty, decimal CompletionRate, decimal ActualRevenue, decimal BonusAmount);
public sealed record SalesKpiConsultantStatsDto(string UserCode, string UserName, string DealerCode, string Position, int TargetCars, int ActualCars, decimal CompletionRate, decimal OverallScore, string KpiGrade, decimal BonusAmount);
public sealed record SalesKpiMonthStatsDto(string PeriodMonth, int PlanCount, int TargetCars, int ActualCars, decimal CompletionRate, decimal TotalRevenue, decimal BonusAmount);

public sealed record SalesLeaderboardDto(
    string? PeriodMonth,
    string? DealerCode,
    DateTime GeneratedAt,
    List<SalesLeaderboardRankDto> TopConsultants,
    List<SalesLeaderboardDealerRankDto> TopDealers
);

public sealed record SalesLeaderboardRankDto(
    int Rank,
    string UserCode,
    string UserName,
    string DealerCode,
    string DealerName,
    int ActualCars,
    int TargetCars,
    decimal CompletionRatePercent,
    decimal ActualRevenue,
    decimal OverallScore,
    string KpiGrade,
    decimal BonusAmount
);

public sealed record SalesLeaderboardDealerRankDto(
    int Rank,
    string DealerCode,
    string DealerName,
    int ActualCars,
    int TargetCars,
    decimal CompletionRatePercent,
    decimal ActualRevenue,
    decimal TotalBonusAmount
);

public sealed record VehicleSalesKpiInfoDto(
    string Vin,
    string Model,
    string? EngineNo,
    string? Color,
    string? StorageCode,
    string? DealerCode,
    string? LastSalesKpiNo,
    DateTime? LastSalesKpiDate,
    int SalesKpiCount,
    SalesTargetKpi? TargetKpi,
    List<SalesKpiDailyLog> RelatedKpiLogs
);







/// <summary>Vi phạm chế tài nhân sự TVBH (BizHTC.WH / HR_SalesManViolate): ghi nhận vi phạm của nhân viên bán hàng đại lý, chế tài Tạm thời (TT) hoặc Vĩnh viễn (VV).</summary>
public sealed class SalesManViolation
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SMCode { get; set; } = "";              // Mã nhân viên bán hàng (TVBH)
    public int ViolateNumber { get; set; } = 1;           // Số thứ tự lần vi phạm của nhân sự (tăng dần)
    public string DealerCode { get; set; } = "";          // Đại lý quản lý nhân sự
    public DateTime ViolateDateStart { get; set; }        // Ngày bắt đầu hiệu lực chế tài
    public DateTime? ViolateDateEnd { get; set; }         // Ngày kết thúc chế tài (bắt buộc với loại Tạm thời)
    public string ViolateTypeId { get; set; } = "TT";     // Loại chế tài: TT (Tạm thời), VV (Vĩnh viễn)
    public string? SMHyundaiCode { get; set; }            // Mã nhân sự Hyundai (SMHyundaiCode)
    public string? SMName { get; set; }                   // Tên nhân sự
    public DateTime? SMDateOfBirth { get; set; }          // Ngày sinh nhân sự
    public string? IdentityCardNo { get; set; }           // Số CCCD/Hộ chiếu
    public string? SMPhoneNo { get; set; }                // SĐT nhân sự
    public string? SMType { get; set; }                   // Loại nhân sự (TVBH, CVDV, KTV...)
    public string? SMStatus { get; set; }                 // Trạng thái làm việc (CHINGTHUC, THUVIEC, CTVIEN, NGHIVIEC)
    public bool FlagActive { get; set; } = true;          // Còn hiệu lực
    public string? Remark { get; set; }                   // Ghi chú lý do vi phạm
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdateDTime { get; set; }
    public string? UpdateBy { get; set; }
}

/// <summary>Đề nghị giao tài liệu xe (BizHTC.WH.Car_DocReqList / Car_DocReqList): phiếu đề nghị giao hồ sơ gốc (COC/hóa đơn/đăng ký) cho đại lý theo lô nhiều VIN, phê duyệt 2 cấp (Pending → Approved1 → Approved2 → Finished hoặc Rejected/Cancelled).</summary>
public sealed class DocRequestList
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DRListCode { get; set; } = "";          // Mã phiếu đề nghị giao tài liệu (DRL...)
    public string TypeCRR { get; set; } = "NORMAL";       // Loại đề nghị: NORMAL (Thường), SPECIAL (Đặc biệt), DEALER (Đại lý tạo), DEALERTCG (Đại lý TCG)
    public string? DealerCode { get; set; }               // Đại lý nhận tài liệu
    public string Status { get; set; } = "Pending";       // Pending → Approved1 → Approved2 → Finished (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }                   // Ghi chú đề nghị
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy1 { get; set; }              // Người duyệt cấp 1
    public DateTime? ApprovedAt1 { get; set; }
    public string? ApprovedBy2 { get; set; }              // Người duyệt cấp 2
    public DateTime? ApprovedAt2 { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
}

/// <summary>Chi tiết xe trong phiếu đề nghị giao tài liệu (BizHTC.WH.Car_DocReqDtl / Car_DocReqDtl): từng VIN kèm trạng thái duyệt riêng (Pending → Approved1 → Approved2 → Finished hoặc Rejected/Cancelled).</summary>
public sealed class DocRequestListLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long DocRequestListId { get; set; }
    public string DRListCode { get; set; } = "";
    public int LineIndex { get; set; } = 1;
    public string Vin { get; set; } = "";
    public string? Model { get; set; }
    public string? EngineNo { get; set; }
    public string? Color { get; set; }
    public string? DealerCode { get; set; }
    public string Status { get; set; } = "Pending";       // Pending → Approved1 → Approved2 → Finished (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
    public string? ApprovedBy1 { get; set; }
    public DateTime? ApprovedAt1 { get; set; }
    public string? ApprovedBy2 { get; set; }
    public DateTime? ApprovedAt2 { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Hợp đồng phụ kiện xe ô tô của Đại lý (HCare.idocNet Dlr_ContractMstPart / Dlr_ContractMstPartController): phụ lục hợp đồng mua bán phụ kiện chính hãng giữa Hãng OEM và Đại lý, gồm số phụ lục, loại phụ lục (PHUKIEN), ngày ký, tiền đặt cọc, đại lý, khách hàng, tài khoản ngân hàng và danh mục phụ kiện kèm số lượng/đơn giá.</summary>
public sealed class AccessoryContract
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DlrContractPartNo { get; set; } = "";     // Số phụ lục hợp đồng phụ kiện (PLHĐ...)
    public string DlrCtrPartType { get; set; } = "PHUKIEN"; // Loại phụ lục (PHUKIEN = Phụ kiện)
    public DateTime? ContractPartDate { get; set; }         // Ngày ký phụ lục hợp đồng
    public decimal DepositVal { get; set; } = 0;            // Số tiền đặt cọc (VNĐ)
    public string? UserCodeOwner { get; set; }              // Mã nhân viên phụ trách (TVBH)
    public string? UserNameOwner { get; set; }              // Tên nhân viên phụ trách
    public string DealerCode { get; set; } = "";            // Mã đại lý ký phụ lục
    public string? BankAccountNo { get; set; }              // Số tài khoản ngân hàng thanh toán
    public string? CustomerCode { get; set; }               // Mã khách hàng (nếu phụ lục gắn khách)
    public string? CustomerName { get; set; }               // Tên khách hàng
    public decimal TotalValBeforeVAT { get; set; } = 0;     // Tổng giá trị phụ kiện trước thuế VAT (VNĐ)
    public string Status { get; set; } = "Pending";         // Pending (P) → Approved (A) → Cancelled (C)
    public string? Remark { get; set; }                     // Ghi chú phụ lục
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
}

/// <summary>Chi tiết phụ kiện trong phụ lục hợp đồng phụ kiện (HCare.idocNet Dlr_ContractMstPartDtl): dòng phụ kiện chính hãng (PartCode), số lượng, đơn giá và thành tiền trước VAT.</summary>
public sealed class AccessoryContractLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long AccessoryContractId { get; set; }
    public string DlrContractPartNo { get; set; } = "";
    public string PartCode { get; set; } = "";              // Mã phụ kiện chính hãng
    public string? PartName { get; set; }                   // Tên phụ kiện
    public string? PartUnitCode { get; set; }               // Đơn vị tính
    public decimal Qty { get; set; } = 1;                   // Số lượng
    public decimal UnitPrice { get; set; } = 0;             // Đơn giá phụ kiện (VNĐ)
    public decimal ValABeforeVAT { get; set; } = 0;         // Thành tiền trước VAT = Qty * UnitPrice
    public string Status { get; set; } = "Pending";         // Pending → Approved → Cancelled
    public string? Remark { get; set; }
}

/// <summary>Tiến trình bán hàng / Phễu bán hàng khách hàng (HCare.idocNet SP_SalesProcess): theo dõi hành trình khách hàng từ tham khảo → quan tâm → đàm phán → lái thử → ký hợp đồng, kèm cấp phê duyệt (SPLevel) và ngân sách dự kiến.</summary>
public sealed class SalesProcess
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SalesID { get; set; } = "";              // Mã tiến trình bán hàng (SP...)
    public string? CustomerCode { get; set; }              // Mã khách hàng (Dls_DealerCustomer)
    public string? CustomerTypeCode { get; set; }          // Loại khách hàng (Cá nhân / Doanh nghiệp)
    public string DealerCode { get; set; } = "";           // Đại lý phụ trách (suy ra từ nhân sự sở hữu)
    public string? UserCodeOwner { get; set; }             // Mã nhân sự TVBH sở hữu tiến trình
    public string? CampaignCode { get; set; }              // Mã chiến dịch marketing liên quan (Mkt_Campaign)
    public decimal BudgetVal { get; set; } = 0;            // Ngân sách dự kiến khách hàng dành cho xe (VNĐ)
    public string CarModelType { get; set; } = "";         // Loại dòng xe khách quan tâm (Sedan, SUV, MPV, Commercial, EV...)
    public DateTime? ContractExpectedDate { get; set; }    // Ngày dự kiến ký hợp đồng (bắt buộc khi SPLevel 3/4/5)
    public string SPLevelCode { get; set; } = "0";         // Cấp phê duyệt tiến trình (0..5)
    public string SPLevelStatus { get; set; } = "P";       // Trạng thái cấp phê duyệt: P (Pending) / A (Approved)
    public DateTime? SPLevelDTime { get; set; }            // Thời điểm phê duyệt cấp
    public string? SPLevelBy { get; set; }                 // Người phê duyệt cấp
    public string SPStatus { get; set; } = "THAMKHAO";     // Trạng thái phễu: THAMKHAO → QUANTAM → DAMPHAN → LAITHU → KYHOPDONG (hoặc HUY)
    public string? Remark { get; set; }                    // Ghi chú tiến trình
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>Chi tiết dòng xe quan tâm trong tiến trình bán hàng (HCare.idocNet SP_SalesProcessDtl): dòng model/phiên bản/màu và số lượng khách dự kiến mua.</summary>
public sealed class SalesProcessLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesProcessId { get; set; }
    public string SalesID { get; set; } = "";
    public string ModelCode { get; set; } = "";            // Dòng xe (SantaFe, Tucson, Accent, Creta...)
    public string? ColorCode { get; set; }                 // Màu xe
    public string? SpecCode { get; set; }                  // Phiên bản xe
    public int Qty { get; set; } = 1;                      // Số lượng xe khách dự kiến mua
    public string SPStatusDtl { get; set; } = "THAMKHAO";  // Trạng thái phễu của dòng xe
    public string? Remark { get; set; }
}

/// <summary>Chỉ số KPI gắn với tiến trình bán hàng (HCare.idocNet SP_SalesProcessPKI): đánh dấu tiến trình đóng góp cho các chỉ tiêu KPI bán hàng.</summary>
public sealed class SalesProcessKpi
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long SalesProcessId { get; set; }
    public string SalesID { get; set; } = "";
    public string KPICode { get; set; } = "";              // Mã chỉ tiêu KPI (NEWCTM, NEWPK, NEWISR, NEWNH, NEWDRT...)
    public DateTime? CreatedDate { get; set; }             // Ngày ghi nhận KPI
    public string? Remark { get; set; }
}
/// <summary>Đề nghị nhận xe &amp; kiểm tra PDI tại nhà máy OEM (BizHTC.HTMV.HTMV_PDI / HTMV_PDI): hãng xe lập đề nghị nhận lô xe từ nhà máy sản xuất (HTMV Ninh Bình) để kiểm tra chất lượng tiền xuất xưởng theo từng VIN trước khi nhập kho PDI.</summary>
public sealed class HtmvPdi
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string PDINo { get; set; } = "";               // Mã đề nghị nhận xe PDI (PDI...)
    public string Status { get; set; } = "P";             // Trạng thái header (HTMV_PDI.PDIStatus, TConst.Stage): P (Pending) → F (Finished) / C (Cancelled)
    public string? Remark { get; set; }                   // Ghi chú đề nghị nhận xe
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }               // Người duyệt (HTMV_PDI.ApprovedBy)
    public DateTime? ApprovedAt { get; set; }             // Ngày duyệt (HTMV_PDI.ApprovedDate)
}

/// <summary>Chi tiết xe VIN trong đề nghị nhận xe PDI nhà máy (BizHTC.HTMV.HTMV_PDIDtl / HTMV_PDIDtl): mỗi dòng 1 VIN với 2 trục trạng thái độc lập — trạng thái dòng (PDIDtlStatus) và trạng thái kho PDI (PDIStorageStatus).</summary>
public sealed class HtmvPdiDtl
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long HtmvPdiId { get; set; }
    public string PDINo { get; set; } = "";
    public string VIN { get; set; } = "";
    public string? RefNo { get; set; }                    // Số Proforma Invoice / PI liên quan (Ord_PI)
    public string? LCTemp { get; set; }                   // Mã L/C tạm (đối chiếu PI)
    public string? SpecCode { get; set; }                 // Phiên bản xe
    public string? ModelCode { get; set; }                // Dòng xe (suy ra từ SpecCode)
    public string? ColorCode { get; set; }                // Màu xe
    public string? ProductionMonth { get; set; }          // Tháng sản xuất (yyyy-MM)
    public string? EngineNo { get; set; }                 // Số máy
    public string PDIDtlStatus { get; set; } = "P";       // Trạng thái DÒNG (TConst.Stage): P → F (đã duyệt) / C (đã hủy)
    public string PDIStorageStatus { get; set; } = "P";   // Trạng thái KHO PDI (TConst.Stage): P → F (đã nhập kho PDI)
    public string PdiResult { get; set; } = "Pending";    // Kết quả kiểm PDI: Pending → Passed / Failed
    public string? FlagRepair { get; set; }               // Cờ cần sửa chữa sau kiểm PDI (HTMV_PDIDtl.FlagRepair)
    public string? RepairRemark { get; set; }             // Ghi chú sửa chữa (HTMV_PDIDtl.RepairRemark)
    public DateTime? PDIDate { get; set; }                // Ngày đề nghị nhận xe / ngày nhập kho PDI (HTMV_PDIDtl.PDIDate)
    public string? Remark { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Xe nhập kho PDI (BizHTC.HTMV.PDI_VIN / PDI_VIN): danh mục xe đã hoàn tất kiểm tra PDI và nhập kho PDI nhà máy, lưu model/spec/màu, số chìa khóa, serial AVN và số ắc quy.</summary>
public sealed class StoragePdiVin
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string VIN { get; set; } = "";
    public string? ModelCode { get; set; }
    public string? SpecCode { get; set; }
    public string? ColorCode { get; set; }
    public string? OrderNoMMS { get; set; }               // Số lệnh sản xuất MMS
    public string? OrderNoMMSDelivery { get; set; }       // Số lệnh giao MMS
    public string? EngineNo { get; set; }
    public string? KeyNo { get; set; }                    // Số chìa khóa
    public string? AVNSerialNo { get; set; }              // Serial màn hình AVN
    public string? BatteryNo { get; set; }                // Số ắc quy
    public string FlagActive { get; set; } = "1";         // Còn hiệu lực (1) / đã xóa (0)
    public string? PDIStorageStatus { get; set; }         // Trạng thái kho PDI của VIN (PDI_VIN.PDIStorageStatus)
    public DateTime? FinishDTime { get; set; }            // Thời điểm hoàn tất PDI = thời gian nhập kho
    public string? Remark { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
/// <summary>Biên bản hủy hợp đồng mua bán xe của Đại lý (DMS40.DMS40_DlrCtr_CancelMinutes / DMS40_DlrCtr_CancelMinutes): biên bản xác nhận hủy hợp đồng bán buôn đã ký giữa Hãng OEM và Đại lý, có 2 chữ ký số độc lập (Đại lý ký DlrSignCcMnStatus và Hãng ký HTCSignCcMnStatus) và trạng thái biên bản CancelMinutesStatus (NS → S).</summary>
public sealed class DealerContractCancelMinutes
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CancelMinutesNo { get; set; } = "";      // Mã biên bản hủy hợp đồng (CCM...)
    public string DlrCtrNo { get; set; } = "";             // Hợp đồng đại lý bị hủy (DMS40_CT_DealerContract.DlrCtrNo)
    public string DealerCode { get; set; } = "";           // Đại lý ký biên bản hủy
    public string? FilePath { get; set; }                  // Đường dẫn file biên bản scan/ký số
    public string? Remark { get; set; }                    // Ghi chú / lý do hủy hợp đồng
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? DlrApprBy { get; set; }                 // Người đại diện Đại lý ký duyệt
    public DateTime? DlrApprAt { get; set; }
    public string? HTCAppr1By { get; set; }                // Người duyệt cấp 1 phía Hãng OEM
    public DateTime? HTCAppr1At { get; set; }
    public string? HTCAppr2By { get; set; }                // Người duyệt cấp 2 phía Hãng OEM (chốt hủy hợp đồng)
    public DateTime? HTCAppr2At { get; set; }
    public string? RejectBy { get; set; }
    public DateTime? RejectAt { get; set; }
    public string? CancelBy { get; set; }
    public DateTime? CancelAt { get; set; }
    public string DlrSignCcMnStatus { get; set; } = "P";   // Trạng thái ký của Đại lý (TConst.DlrSignCcMnStatus): P (Pending) → A (Approved) / C (Cancel)
    public string HTCSignCcMnStatus { get; set; } = "P";   // Trạng thái ký của Hãng (TConst.HTCSignCcMnStatus): P → A1 (Approved1) → A2 (Approved2) / C
    public string CancelMinutesStatus { get; set; } = "NS"; // Trạng thái biên bản (TConst.CancelMinutesStatus): NS (NotSign) → S (Signed) / AJ (Adjusted) / C (Cancel)
}

/// <summary>Điều chuyển lại yêu cầu vận chuyển xe ô tô (BizHTC.Storage.Sto_RearrangeTranspReq / Sto_RearrangeTranspReq): gom nhóm các xe VIN (mỗi VIN gắn 1 lệnh tái sắp xếp kho StorageRearrange) vào 1 yêu cầu vận chuyển lại theo nhà xe + hợp đồng vận tải, phục vụ điều độ lại xe lồng khi thay đổi kế hoạch.</summary>
public sealed class RearrangeTransportRequest
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SRTReqNo { get; set; } = "";              // Mã yêu cầu điều chuyển lại vận chuyển (SRT...)
    public string? TransporterCode { get; set; }            // Đơn vị / Nhà xe vận chuyển (NYK, Traco, Vinafco...)
    public string? TransportContractNo { get; set; }        // Số hợp đồng vận chuyển
    public string? TruckPlateNo { get; set; }               // Biển số xe tải / xe lồng chuyên dụng
    public string? DriverName { get; set; }                 // Tên lái xe lồng
    public string? DriverPhone { get; set; }                // SĐT lái xe
    public string? FromStorage { get; set; }                // Kho bãi xuất phát
    public string? ToStorage { get; set; }                  // Kho / Điểm hạ tải đích
    public DateTime? EstimatedDeparture { get; set; }       // Ngày dự kiến xuất bến
    public DateTime? EstimatedArrival { get; set; }         // Ngày dự kiến đến nơi
    public string Status { get; set; } = "P";               // Trạng thái (TConst.Stage): P (Pending) → A (Approved) / R (Rejected)
    public string? Remark { get; set; }                     // Ghi chú điều vận
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectBy { get; set; }
    public DateTime? RejectAt { get; set; }
}

/// <summary>Chi tiết xe trong yêu cầu điều chuyển lại vận chuyển (Sto_RearrangeTranspReqDtl): danh sách VIN và liên kết lệnh tái sắp xếp kho nguồn.</summary>
public sealed class RearrangeTransportRequestLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long RearrangeTransportRequestId { get; set; }
    public string SRTReqNo { get; set; } = "";
    public string Vin { get; set; } = "";
    public string? StorageRearrangeNo { get; set; }          // Lệnh tái sắp xếp kho nguồn của xe (Sto_StorageRearrange)
    public string? StorageCodeFrom { get; set; }             // Vị trí/bãi đỗ cũ
    public string? StorageCodeTo { get; set; }               // Vị trí/bãi đỗ mới
    public string Status { get; set; } = "P";                // Trạng thái dòng (TConst.Stage): P → A / R
    public string? Remark { get; set; }
}

/// <summary>Kế hoạch xe về / Kế hoạch nhập xe theo đại lý (BizHTC.Car.Car_Plan / CarPlan): đại lý đăng ký kế hoạch nhận xe theo tháng (số đơn hàng, model, spec, màu, số lượng đặt/duyệt), theo dõi số lượng xe đã về kho và còn lại.</summary>
public sealed class CarPlan
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CPCode { get; set; } = "";              // Mã kế hoạch xe về (CP...)
    public string DealerCode { get; set; } = "";          // Đại lý đăng ký kế hoạch nhận xe
    public string? OrderNo { get; set; }                  // Số đơn hàng / lô xe liên quan
    public string? PlanMonth { get; set; }                // Tháng kế hoạch (yyyy-MM)
    public int TotalQtyOrder { get; set; } = 0;           // Tổng số lượng đặt
    public int TotalQty { get; set; } = 0;                // Tổng số lượng duyệt
    public int TotalArrivedQty { get; set; } = 0;         // Tổng số lượng xe đã về kho
    public int TotalPendingQty { get; set; } = 0;         // Tổng số lượng xe còn lại (chưa về)
    public string Status { get; set; } = "Draft";         // Draft → Submitted → Approved → Completed (hoặc Rejected / Cancelled)
    public string? CreatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Chi tiết dòng xe trong kế hoạch xe về (Car_PlanDetail): model, spec, màu, số lượng đặt/duyệt và số lượng đã về theo từng dòng.</summary>
public sealed class CarPlanLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long CarPlanId { get; set; }
    public string CPCode { get; set; } = "";
    public string Model { get; set; } = "";               // Dòng xe (Accent, Creta, Tucson...)
    public string? SpecCode { get; set; }                 // Phiên bản xe
    public string? Color { get; set; }                    // Màu xe
    public int QtyOrder { get; set; } = 0;                // Số lượng đặt
    public int Qty { get; set; } = 0;                     // Số lượng duyệt
    public int ArrivedQty { get; set; } = 0;              // Số lượng xe đã về kho
    public int PendingQty { get; set; } = 0;              // Số lượng xe còn lại (chưa về)
    public string Status { get; set; } = "Pending";       // Pending → Approved → Arrived (hoặc Rejected / Cancelled)
    public string? Remark { get; set; }
}

/// <summary>Danh mục kho bãi toàn cục của Hãng OEM (BizHTC.DMS40.Mst_StorageGlobal): mã vị trí kho/bãi đỗ xe
/// theo từng dòng xe (ModelCode). Dùng để gán vị trí lưu kho (StorageCode) cho hồ sơ số khung VIN.</summary>
public sealed class StorageGlobal
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string StorageCode { get; set; } = "";   // Mã vị trí kho/bãi đỗ (YARD-A1, BODY-SHOP-01...)
    public string ModelCode { get; set; } = "";     // Dòng xe áp dụng cho vị trí kho này
    public bool FlagActive { get; set; } = true;    // Còn hiệu lực sử dụng (FlagActive = '1')
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }    // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }            // Người cập nhật gần nhất
}

/// <summary>Danh mục kho bãi cục bộ của Đại lý (BizHTC.DMS40.Dlr_StorageLocal): mã vị trí kho/bãi đỗ xe
/// theo từng đại lý (DealerCode). Dùng để quản lý vị trí lưu kho xe tại showroom/đại lý.</summary>
public sealed class StorageLocal
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DealerCode { get; set; } = "";    // Đại lý sở hữu vị trí kho
    public string StorageCode { get; set; } = "";   // Mã vị trí kho/bãi đỗ của đại lý
    public bool FlagActive { get; set; } = true;    // Còn hiệu lực sử dụng (FlagActive = '1')
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }    // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }            // Người cập nhật gần nhất
}

/// <summary>Danh mục Giá xe tồn kho theo phiên bản (BizHTC.DMS40.Mst_CarPriceInStock): giá nhập/giá tồn kho
/// áp dụng cho từng phiên bản xe (SpecCode) theo ngày hiệu lực (EffectiveDate). Khóa nghiệp vụ = (SpecCode, EffectiveDate).</summary>
public sealed class CarPriceInStock
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string SpecCode { get; set; } = "";       // Mã phiên bản xe (SpecCode) — phải tồn tại & đang hiệu lực
    public DateTime EffectiveDate { get; set; }       // Ngày hiệu lực áp dụng giá (không được ở quá khứ khi tạo)
    public decimal UnitPriceIn { get; set; } = 0;     // Đơn giá tồn kho (VNĐ), >= 0
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }            // Người tạo bản ghi
    public DateTime? LogLUDateTime { get; set; }      // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }              // Người cập nhật gần nhất
}

/// <summary>Trang thiết bị gắn trên xe (BizHTC.WH.Mng_Device_Car): quản lý thiết bị/phụ kiện lắp trên từng
/// số khung VIN (AVN, GPS, camera hành trình, bệ bước, giá nóc...) kèm hóa đơn đầu vào (InputInvoiceNo/Date).
/// Khóa nghiệp vụ = (VIN, DeviceTypeCode, SpecCode).</summary>
public sealed class VehicleDevice
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string Vin { get; set; } = "";              // Số khung xe gắn thiết bị
    public string DeviceTypeCode { get; set; } = "";   // Mã loại thiết bị (AVN, GPS, CAMERA, BEBUOC...)
    public string SpecCode { get; set; } = "";         // Mã phiên bản xe (SpecCode) tại thời điểm gắn
    public string? ModelCode { get; set; }             // Dòng xe (suy ra từ hồ sơ VIN)
    public string? ColorCode { get; set; }             // Màu xe (suy ra từ hồ sơ VIN)
    public string? InputInvoiceNo { get; set; }        // Số hóa đơn đầu vào của thiết bị
    public DateTime? InputInvoiceDate { get; set; }    // Ngày hóa đơn đầu vào
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }       // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }               // Người cập nhật gần nhất
}

/// <summary>Kế hoạch kinh doanh năm của Đại lý (BizHTC.DMS40.BPL_BusinessPlan): đại lý lập kế hoạch
/// sản lượng bán lẻ / đặt hàng / back-order theo từng dòng xe cho cả năm (12 tháng), trình Hãng OEM
/// phê duyệt 2 cấp. Trạng thái: P (Pending) → A1 (Approve1) → A2 (Approve2). Version: INIT (bản nháp) → ACTUAL (bản chốt).</summary>
public sealed class BusinessPlan
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string BusinessPlanCode { get; set; } = "";   // Mã kế hoạch (yyMMdd-xxx/BPL/DLR-...)
    public string DealerCode { get; set; } = "";         // Đại lý lập kế hoạch
    public string YearPlan { get; set; } = "";           // Năm kế hoạch (yyyy)
    public string? MonthPlan { get; set; }               // Tháng kế hoạch (nếu lập theo tháng)
    public string PlanType { get; set; } = "Year";       // Loại kế hoạch: Year (cả năm), Month (theo tháng)
    public string BusinessPlanStatus { get; set; } = "P"; // P (Pending) → A1 (Approve1) → A2 (Approve2)
    public string Version { get; set; } = "INIT";        // INIT (bản nháp) → ACTUAL (bản chốt thực tế)
    public int TimesPlan { get; set; } = 0;              // Số lần trình duyệt (TimesPlan)
    public string? AreaCodeDealer { get; set; }          // Mã vùng/khu vực đại lý
    public string? AreaNameDealer { get; set; }          // Tên vùng/khu vực đại lý
    public string? HTCStaffInCharge { get; set; }        // Nhân viên Hãng OEM phụ trách theo dõi
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? Appr1DTime { get; set; }            // Thời điểm duyệt cấp 1
    public string? Appr1By { get; set; }
    public DateTime? Appr2DTime { get; set; }            // Thời điểm duyệt cấp 2 (chốt kế hoạch)
    public string? Appr2By { get; set; }
    public string? Remark { get; set; }
}

/// <summary>Chi tiết dòng xe trong kế hoạch kinh doanh (BizHTC.DMS40.BPL_BusinessPlanDtl): sản lượng
/// bán lẻ (Rtl), đặt hàng (Ord) và back-order (BO) theo từng tháng M1..M12 cho một dòng xe.</summary>
public sealed class BusinessPlanLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long BusinessPlanId { get; set; }
    public string BusinessPlanCode { get; set; } = "";
    public string YearPlan { get; set; } = "";
    public string ModelCode { get; set; } = "";          // Dòng xe
    public string BusinessPlanDtlStatus { get; set; } = "P"; // Trạng thái dòng: P → A1 → A2
    public string VersionDtl { get; set; } = "INIT";     // Phiên bản dòng: INIT → ACTUAL

    // Bán lẻ (Retail)
    public int Rtl_TotalQtyDeal { get; set; } = 0;       // Tổng sản lượng bán lẻ cả năm
    public int Rtl_QtyM1 { get; set; } = 0;
    public int Rtl_QtyM2 { get; set; } = 0;
    public int Rtl_QtyM3 { get; set; } = 0;
    public int Rtl_QtyM4 { get; set; } = 0;
    public int Rtl_QtyM5 { get; set; } = 0;
    public int Rtl_QtyM6 { get; set; } = 0;
    public int Rtl_QtyM7 { get; set; } = 0;
    public int Rtl_QtyM8 { get; set; } = 0;
    public int Rtl_QtyM9 { get; set; } = 0;
    public int Rtl_QtyM10 { get; set; } = 0;
    public int Rtl_QtyM11 { get; set; } = 0;
    public int Rtl_QtyM12 { get; set; } = 0;

    // Đặt hàng (Order)
    public int Ord_QtyM1 { get; set; } = 0;
    public int Ord_QtyM2 { get; set; } = 0;
    public int Ord_QtyM3 { get; set; } = 0;
    public int Ord_QtyM4 { get; set; } = 0;
    public int Ord_QtyM5 { get; set; } = 0;
    public int Ord_QtyM6 { get; set; } = 0;
    public int Ord_QtyM7 { get; set; } = 0;
    public int Ord_QtyM8 { get; set; } = 0;
    public int Ord_QtyM9 { get; set; } = 0;
    public int Ord_QtyM10 { get; set; } = 0;
    public int Ord_QtyM11 { get; set; } = 0;
    public int Ord_QtyM12 { get; set; } = 0;

    // Back-order (BO)
    public int BO_TotalQtyBO { get; set; } = 0;          // Tổng back-order cả năm
    public int BO_QtyM1 { get; set; } = 0;
    public int BO_QtyM2 { get; set; } = 0;
    public int BO_QtyM3 { get; set; } = 0;
    public int BO_QtyM4 { get; set; } = 0;
    public int BO_QtyM5 { get; set; } = 0;
    public int BO_QtyM6 { get; set; } = 0;
    public int BO_QtyM7 { get; set; } = 0;
    public int BO_QtyM8 { get; set; } = 0;
    public int BO_QtyM9 { get; set; } = 0;
    public int BO_QtyM10 { get; set; } = 0;
    public int BO_QtyM11 { get; set; } = 0;
    public int BO_QtyM12 { get; set; } = 0;

    public string? Remark { get; set; }
}

/// <summary>Lệnh sản xuất / Work Order nhà máy OEM (BizHTC.MMSIntergration / Mnf_WorkOrder): 1 lệnh sản xuất gom nhiều số khung VIN theo lô (Lot) và PI.</summary>
public sealed class WorkOrder
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string WorkOrderNo { get; set; } = "";      // Mã lệnh sản xuất (WORKORDERNO)
    public string? OrderNo { get; set; }               // Số đơn đặt hàng sản xuất (ORDERNO)
    public string? OrderNoUser { get; set; }           // Số đơn hàng do người dùng nhập (ORDERNOUSER)
    public string? PINo { get; set; }                  // Proforma Invoice liên quan (PINO)
    public string? Lot { get; set; }                   // Lô sản xuất (LOT)
    public string Status { get; set; } = "Draft";      // Draft → InProduction → Completed (hoặc Cancelled)
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
}

/// <summary>Dòng số khung VIN trong lệnh sản xuất (Mnf_VIN): theo dõi tiến độ qua các xưởng Hàn (BS) → Sơn (PS) → Lắp ráp (AS) → KCS (QA) → Hoàn tất.</summary>
public sealed class WorkOrderLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long WorkOrderId { get; set; }
    public string WorkOrderNo { get; set; } = "";
    public string Vin { get; set; } = "";              // Số khung (VIN)
    public string? SpecCode { get; set; }              // Phiên bản (SPECCODE)
    public string? ModelCode { get; set; }             // Dòng xe (MODELCODE)
    public string? ColorCodeInit { get; set; }         // Màu sơn kế hoạch ban đầu (COLORCODEINIT)
    public string? ColorCode { get; set; }             // Màu sơn thực tế (COLORCODE)
    public string? EngineNoInit { get; set; }          // Số máy kế hoạch (ENGINENOINIT)
    public string? EngineNo { get; set; }              // Số máy thực tế (ENGINENO)
    public int? VinYear { get; set; }                  // Năm sản xuất (VINYEAR)
    public string? ShopCCCode { get; set; }            // Xưởng hiện tại: XH (Hàn), XS (Sơn), XLR (Lắp ráp), XKTCL (KCS)
    public string? StationCCCode { get; set; }         // Trạm hiện tại (STATIONCCCODE)
    public string? ConvertRuleCode { get; set; }       // Quy tắc chuyển đổi thời gian công đoạn áp dụng (CONVERTRULECODE)
    public string? WOStatusDtl { get; set; }           // Trạng thái chi tiết công đoạn (WOSTATUSDTL)
    public string VinStatus { get; set; } = "Pending"; // Pending → InProduction → Finished (hoặc Cancelled)
    public string? VinShopStatus { get; set; }         // Trạng thái theo xưởng (VINSHOPSTATUS)
    public DateTime? WkDTime { get; set; }             // Thời điểm bắt đầu vào xưởng (WKDTIME)
    public string? WkBy { get; set; }                  // Người bắt đầu (WKBY)
    public DateTime? FinishDTime { get; set; }         // Thời điểm hoàn tất xuất xưởng (FINISHDTIME)
    public string? FinishBy { get; set; }              // Người hoàn tất (FINISHBY)
    public DateTime? EffDTimeStart_BS { get; set; }    // Bắt đầu xưởng Hàn (EFFDTIMESTART_BS)
    public DateTime? EffDTimeEnd_BS { get; set; }      // Kết thúc xưởng Hàn (EFFDTIMEEND_BS)
    public DateTime? EffDTimeStart_PS { get; set; }    // Bắt đầu xưởng Sơn (EFFDTIMESTART_PS)
    public DateTime? EffDTimeEnd_PS { get; set; }      // Kết thúc xưởng Sơn (EFFDTIMEEND_PS)
    public DateTime? EffDTimeStart_AS { get; set; }    // Bắt đầu xưởng Lắp ráp (EFFDTIMESTART_AS)
    public DateTime? EffDTimeEnd_AS { get; set; }      // Kết thúc xưởng Lắp ráp (EFFDTIMEEND_AS)
    public DateTime? EffDTimeEnd_QA { get; set; }      // Kết thúc kiểm tra KCS (EFFDTIMEEND_QA)
    public string? FlagRepair { get; set; }            // Cờ sửa chữa lại (FLAGREPAIR)
    public string? Remark { get; set; }
    public string? RemarkSub { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }
    public string? LogLUBy { get; set; }
}

/// <summary>Quy tắc chuyển đổi thời gian công đoạn sản xuất (Mnf_ConvertRule): định mức thời gian chuẩn (phút) cho từng trạm theo xưởng.</summary>
public sealed class ConvertRule
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ConvertRuleCode { get; set; } = "";  // Mã quy tắc (CONVERTRULECODE)
    public string? ConvertRuleDesc { get; set; }       // Mô tả quy tắc (CONVERTRULEDESC)
    public DateTime? EffDateStart { get; set; }        // Ngày hiệu lực bắt đầu (EFFDATESTART)
    public DateTime? EffDateEnd { get; set; }          // Ngày hiệu lực kết thúc (EFFDATEEND)
    public bool FlagActive { get; set; } = true;       // Còn hiệu lực (FLAGACTIVE)
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }
    public string? LogLUBy { get; set; }
}

/// <summary>Dòng định mức thời gian công đoạn theo trạm (Mnf_ConvertRuleDtl): thời gian sản xuất chuẩn (phút) cho từng trạm trong xưởng.</summary>
public sealed class ConvertRuleLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long ConvertRuleId { get; set; }
    public string ConvertRuleCode { get; set; } = "";
    public string ShopCCCode { get; set; } = "";       // Xưởng (SHOPCCCODE): XH, XS, XLR, XKTCL
    public string StationCCCode { get; set; } = "";    // Trạm (STATIONCCCODE)
    public decimal PrdTime { get; set; } = 0;          // Thời gian sản xuất chuẩn (phút) (PRDTIME)
    public decimal StationOfShopRate { get; set; } = 0; // Tỷ lệ trạm/xưởng (STATIONOFSHOPRATE)
    public decimal StationOfMnfRate { get; set; } = 0;  // Tỷ lệ trạm/nhà máy (STATIONOFMNFRATE)
    public int Seq { get; set; } = 0;                  // Thứ tự trạm (SEQ)
    public string? Remark { get; set; }
}

/// <summary>Nhật ký cập nhật trạng thái hồ sơ xe (BizHTC.Car.Car_VIN.DOCUMENTSTATUS / FULLDOCDATE / REMARKDETAIL):
/// ghi vết mỗi lần đại lý/hãng cập nhật tình trạng giấy tờ pháp lý của xe theo VIN.
/// Quy tắc nghiệp vụ nguồn: khi xe đã giao bán (SellStatus = "A") thì cập nhật "Ngày đủ hồ sơ" (FullDocDate);
/// ngược lại cập nhật "Trạng thái hồ sơ" (DocumentStatus).</summary>
public sealed class VehicleDocumentStatusLog
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DocStatusNo { get; set; } = "";        // Mã lần cập nhật (DSL...)
    public string Vin { get; set; } = "";                // Số khung xe
    public string? Model { get; set; }                   // Dòng xe (snapshot)
    public string? DealerCode { get; set; }              // Đại lý quản lý xe
    public string? DocumentStatus { get; set; }          // Trạng thái hồ sơ sau cập nhật
    public DateTime? FullDocDate { get; set; }           // Ngày đủ hồ sơ sau cập nhật
    public string? RemarkDetail { get; set; }            // Ghi chú chi tiết tình trạng hồ sơ
    public bool IsDelivered { get; set; } = false;       // Xe đã giao bán tại thời điểm cập nhật (SellStatus = "A")
    public string? UpdatedBy { get; set; }               // Người thực hiện cập nhật
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>Bảng kê tính Chi phí tài chính (CPTC) &amp; Chiết khấu thanh toán (CKTT) cho xe ô tô theo số ngày đặt cọc / bảo lãnh
/// (BizHTC.DMS40 / DMS40_FnExp_Calc_FnExp_PmDc): hãng OEM tính chi phí tài chính cho đại lý theo thời gian đại lý giữ
/// tiền đặt cọc và bảo lãnh ngân hàng, đồng thời tính chiết khấu thanh toán trả sớm. Luồng ký 2 bên:
/// Đại lý ký cấp 1 (A1) → cấp 2 (A2), Hãng ký cấp 1 (A1) → cấp 2 (A2) chốt FnExpStatus = S (Signed).</summary>
public sealed class FnExpCalc
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CaNo { get; set; } = "";                 // Mã bảng kê tính CPTC/CKTT (FNEXPCANO: {0}-{1}/{2}/{3})
    public string DealerCode { get; set; } = "";           // Đại lý được tính chi phí tài chính
    public DateTime? TermFrom { get; set; }                // Kỳ tính hiện tại - từ ngày
    public DateTime? TermTo { get; set; }                  // Kỳ tính hiện tại - đến ngày
    public DateTime? TermPrevFrom { get; set; }            // Kỳ trước - từ ngày
    public DateTime? TermPrevTo { get; set; }              // Kỳ trước - đến ngày
    public decimal FnExpPercent { get; set; } = 0;         // Tỷ lệ % chi phí tài chính (CPTC) áp dụng (0-100)
    public decimal PmtDsTCGPercent { get; set; } = 0;      // Tỷ lệ % chiết khấu thanh toán (CKTT) áp dụng (0-100)
    public decimal TotalFnDepositAmount { get; set; } = 0; // Tổng tiền CPTC phần đặt cọc
    public decimal TotalFnGrtAmount { get; set; } = 0;     // Tổng tiền CPTC phần bảo lãnh
    public decimal TotalFnAmount { get; set; } = 0;        // Tổng tiền CPTC (cọc + bảo lãnh)
    public decimal TotalPDAmount { get; set; } = 0;        // Tổng tiền chiết khấu thanh toán (CKTT)
    public int TotalVinCount { get; set; } = 0;            // Tổng số xe VIN trong bảng kê
    public string DlrSignStatus { get; set; } = "P";       // Trạng thái ký của Đại lý: P (chưa ký) → A1 → A2
    public string HTCSignStatus { get; set; } = "P";       // Trạng thái ký của Hãng: P (chưa ký) → A1 → A2
    public string FnExpStatus { get; set; } = "NS";        // Trạng thái bảng kê: NS (chưa ký) → S (đã ký) / C (hủy)
    public string? DlrAppr1By { get; set; }
    public DateTime? DlrAppr1At { get; set; }
    public string? DlrAppr2By { get; set; }
    public DateTime? DlrAppr2At { get; set; }
    public string? HTCAppr1By { get; set; }
    public DateTime? HTCAppr1At { get; set; }
    public string? HTCAppr2By { get; set; }
    public DateTime? HTCAppr2At { get; set; }
    public string? CancelBy { get; set; }
    public DateTime? CancelAt { get; set; }
    public string? CancelReason { get; set; }
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }
    public string? LogLUBy { get; set; }
}

/// <summary>Chi tiết xe VIN trong bảng kê tính CPTC/CKTT (DMS40_FnExp_Calc_FnExp_PmDcDtl):
/// số ngày và số tiền chi phí tài chính phần đặt cọc / bảo lãnh, số ngày và số tiền chiết khấu thanh toán.
/// Công thức nguồn: Amount = Rate * UnitPriceActual * Percent * CountDate / 360, với Rate phụ thuộc AssemblyStatus
/// (CKD: cọc 0.15 / bảo lãnh 0.85 / CKTT 0.85; CBU: cọc 0.30 / bảo lãnh 0.70 / CKTT 0.70).</summary>
public sealed class FnExpCalcLine
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public long FnExpCalcId { get; set; }
    public string CaNo { get; set; } = "";
    public string Vin { get; set; } = "";                  // Số khung xe
    public string? CarId { get; set; }                     // Mã xe nội bộ (Car_Car.CarID)
    public string? ModelCode { get; set; }                 // Dòng xe
    public string? SpecCode { get; set; }                  // Phiên bản
    public string AssemblyStatus { get; set; } = "CBU";    // Hình thức lắp ráp: CBU (nguyên chiếc) / CKD (bộ linh kiện)
    public decimal UnitPriceActual { get; set; } = 0;      // Giá xe thực tế dùng để tính (VNĐ)
    public string? SOCode { get; set; }                    // Đơn đặt hàng liên quan
    public string? SPCode { get; set; }                    // Chính sách đơn hàng
    public int FnDepositCountDate { get; set; } = 0;       // Số ngày tính CPTC phần đặt cọc
    public decimal FnDepositAmount { get; set; } = 0;      // Số tiền CPTC phần đặt cọc
    public int FnGrtCountDate { get; set; } = 0;           // Số ngày tính CPTC phần bảo lãnh
    public decimal FnGrtAmount { get; set; } = 0;          // Số tiền CPTC phần bảo lãnh
    public decimal FnTotalAmount { get; set; } = 0;        // Tổng CPTC = FnDepositAmount + FnGrtAmount
    public int PDCountDate { get; set; } = 0;              // Số ngày tính chiết khấu thanh toán (CKTT)
    public decimal PDAmount { get; set; } = 0;             // Số tiền chiết khấu thanh toán (CKTT)
    public string Status { get; set; } = "Pending";        // Pending → Signed (theo bảng kê) / Cancelled
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }
    public string? LogLUBy { get; set; }
}

/// <summary>Danh mục mẫu hợp đồng mua bán xe của Đại lý (BizHTC.RetailContract.Dlr_Mst_ContractForm / Dlr_Mst_ContractForm): catalog các mẫu hợp đồng bán lẻ/bán buôn xe ô tô do Hãng OEM ban hành, mỗi mẫu có mã (ContractFNo) và tên mẫu (ContractFName), dùng làm khuôn để gán điều khoản cho từng đại lý.</summary>
public sealed class DealerContractForm
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string ContractFNo { get; set; } = "";          // Mã mẫu hợp đồng (ContractFNo)
    public string ContractFName { get; set; } = "";        // Tên mẫu hợp đồng (ContractFName)
    public string? ContractFType { get; set; }             // Loại mẫu: Retail (bán lẻ) / Wholesale (bán buôn)
    public string? Remark { get; set; }
    public string FlagActive { get; set; } = "1";          // Đang áp dụng (FlagActive)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LogLUDateTime { get; set; }           // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }                   // Người cập nhật gần nhất
}

/// <summary>Mẫu hợp đồng mua bán xe gán theo từng Đại lý (BizHTC.RetailContract.Dlr_Mst_DealerContractForm / Dlr_Mst_DealerContractForm): bộ điều khoản hợp đồng cụ thể (khuyến mãi, thời hạn & phương thức thanh toán, thời gian/địa điểm giao xe, thời điểm sang tên, quyền & trách nhiệm bên bán/bên mua, bảo hành, điều khoản khác) áp dụng cho 1 đại lý theo 1 mẫu hợp đồng. Khóa nghiệp vụ = (DealerCode, ContractFNo).</summary>
public sealed class DealerContractFormTerm
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string DealerCode { get; set; } = "";           // Đại lý áp dụng mẫu hợp đồng
    public string ContractFNo { get; set; } = "";          // Mã mẫu hợp đồng (FK DealerContractForm)
    public string? Note { get; set; }                      // Ghi chú chung
    public string? Promotion { get; set; }                 // Khuyến mãi / ưu đãi
    public string? TimePayment { get; set; }               // Thời hạn thanh toán
    public string? MethodPayment { get; set; }             // Phương thức thanh toán
    public string? TimeAndAddressDelivery { get; set; }    // Thời gian & địa điểm giao xe
    public string? TimeOwnerTransfer { get; set; }         // Thời điểm sang tên chủ sở hữu
    public string? RightAndResponsibilityPartySeller { get; set; } // Quyền & trách nhiệm bên bán
    public string? RightAndResponsibilityPartyBuyer { get; set; }  // Quyền & trách nhiệm bên mua
    public string? Warrantly { get; set; }                 // Điều khoản bảo hành
    public string? OtherTerms { get; set; }                // Điều khoản khác
    public string? Remark { get; set; }
    public string FlagActive { get; set; } = "1";          // Đang áp dụng (FlagActive)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreateBy { get; set; }                  // Người tạo
    public DateTime? LogLUDateTime { get; set; }           // Thời điểm cập nhật gần nhất
    public string? LogLUBy { get; set; }                   // Người cập nhật gần nhất
}

/// <summary>Biên bản hủy hợp đồng thanh toán qua ngân hàng (BizHTC.DMS40.DMS40_DlrCtr_CancelBankMD / DMS40_DlrCtr_CancelBankMD): đại lý đề nghị hủy phương thức thanh toán qua ngân hàng (BankCodeMD) của một hợp đồng mua bán xe đã ký, trình hãng OEM duyệt, sau đó đại lý xác nhận hoàn tất để gỡ bỏ ràng buộc ngân hàng thanh toán trên hợp đồng. Luồng trạng thái: P (Pending) → A (Approved) → F (Finished), hoặc R (Rejected) / C (Cancelled).</summary>
public sealed class CancelBankMD
{
    public long Id { get; set; }
    public Guid OrgId { get; set; }
    public string CancelBankMDNo { get; set; } = "";       // Mã biên bản hủy thanh toán qua NH (CancelBankMDNo)
    public string DlrCtrNo { get; set; } = "";             // Hợp đồng mua bán xe bị hủy thanh toán qua NH (DlrCtrNo)
    public string DealerCode { get; set; } = "";           // Đại lý đề nghị (DealerCode)
    public string? BankCodeMD { get; set; }                // Mã ngân hàng thanh toán bị hủy (BankCodeMD)
    public string Status { get; set; } = "P";              // P (Pending) → A (Approved) → F (Finished); hoặc R (Rejected) / C (Cancelled)
    public string? RemarkDlr { get; set; }                 // Ghi chú của đại lý (RemarkDlr)
    public string? RemarkBank { get; set; }                // Ghi chú của hãng/ngân hàng (RemarkBank)
    public string? CreateBy { get; set; }                  // Người tạo biên bản (CreateBy)
    public string? ApproveBy { get; set; }                 // Người duyệt (ApproveBy)
    public string? FinishBy { get; set; }                  // Người xác nhận hoàn tất (FinishBy)
    public string? CancelBy { get; set; }                  // Người hủy (CancelBy)
    public string? RejectBy { get; set; }                  // Người từ chối (RejectBy)
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Ngày tạo (CreateDTime)
    public DateTime? ApproveAt { get; set; }               // Ngày duyệt (ApproveDateTime)
    public DateTime? FinishAt { get; set; }                // Ngày hoàn tất (FinishDTime)
    public DateTime? CancelAt { get; set; }                // Ngày hủy (CancelDTime)
    public DateTime? RejectAt { get; set; }                // Ngày từ chối (RejectDTime)
    public DateTime? LogLUDateTime { get; set; }           // Thời điểm cập nhật gần nhất (LogLUDateTime)
    public string? LogLUBy { get; set; }                   // Người cập nhật gần nhất (LogLUBy)
}
