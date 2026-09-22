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
public record CreateTestCarDto(string DealerCode, List<string> Vins, string? EventName = null, string? Purpose = null, DateTime? StartDate = null, DateTime? EndDate = null, string? Remark = null, string? TestCarCode = null);
public record TestCarTransitionDto(string? Note = null);
public record FinishTestCarLineDto(int? OdoEnd = null, string? ConditionEnd = null, string? Remark = null, DateTime? ReturnDate = null);

public record PdiItemInputDto(string Vin, string? DlrContractNo = null, string? Remark = null);
public record CreatePdiRequestDto(string DealerCode, List<string>? Vins = null, List<PdiItemInputDto>? Items = null, string? InspectorName = null, string? Remark = null, string? PdiReqNo = null);
public record PdiRequestTransitionDto(string? Note = null, string? InspectorName = null, string? ApprovedBy = null);
public record InspectPdiLineDto(
    double? BatteryVoltage = 12.6,
    bool? TirePressureOk = true,
    bool? FluidsOk = true,
    bool? ElectronicsOk = true,
    bool? ExteriorOk = true,
    bool? InteriorCleanOk = true,
    bool? DiagnosticScanOk = true,
    bool? Passed = true,
    string? RoNo = null,
    string? RoStatus = null,
    string? InspectorName = null,
    string? DefectNotes = null,
    string? Remark = null
);

public record MortgageItemInputDto(string Vin, decimal MortgageAmount = 0, string? Remark = null);
public record CreateMortgageRequestDto(string BankCode, List<string>? Vins = null, List<MortgageItemInputDto>? Items = null, DateTime? MortgageDate = null, string? Remark = null, string? ReqMortgageNo = null);
public record MortgageRequestTransitionDto(string? Note = null);

public record RedeemItemInputDto(string Vin, string? ReleaseDocType = "All", string? Remark = null);
public record CreateRedeemRequestDto(string DealerCode, string BankCode, List<string>? Vins = null, List<RedeemItemInputDto>? Items = null, string? ReqMortgageNo = null, string? Reason = null, string? Remark = null, string? RedeemReqNo = null);
public record RedeemRequestTransitionDto(string? Note = null);

public record SalesOrderItemInputDto(string Model, string? SpecCode = null, string? Color = null, int OrderQty = 1, decimal UnitPrice = 0, string? Remark = null);
public record CreateSalesOrderDto(string DealerCode, List<SalesOrderItemInputDto> Items, string? SOType = "Normal", string? SPCode = null, string? OrderMonth = null, string? ProductionMonth = null, string? ExpectedMonth = null, string? Remark = null, string? SOCode = null, string? CreatedBy = null);
public record SalesOrderApproveItemDto(long? LineId = null, string? Model = null, string? SpecCode = null, string? Color = null, int? ApprovedQty = null, decimal? UnitPrice = null);
public record SalesOrderTransitionDto(string? Note = null, string? ApprovedBy = null, string? ProductionMonth = null, string? ExpectedMonth = null, List<SalesOrderApproveItemDto>? Items = null);
public record AllocateSoVinDto(string Vin, long? LineId = null, string? Remark = null);

public record DealerDealItemInputDto(string Vin, decimal? UnitPrice = null, decimal? Discount = null, string? PlateNo = null, string? SBHOnlineNo = null, int? WarrantyMonths = null, int? DeliveryOdoKm = null, string? Remark = null);
public record CreateDealerDealDto(string DealerCode, string CustomerName, string CustomerPhone, List<DealerDealItemInputDto> Items, string? CustomerCode = null, string? CustomerType = "Individual", string? IdNo = null, string? Address = null, string? SalesManCode = null, string? SalesManName = null, string? SalesType = "Retail", string? PaymentType = "Cash", string? BankCode = null, decimal LoanAmount = 0, decimal DepositAmount = 0, DateTime? DealDate = null, string? DealNo = null, string? DealNoUser = null, string? Remark = null, string? CreatedBy = null);
public record DealerDealTransitionDto(string? Note = null, string? ApprovedBy = null, DateTime? DeliveryDate = null);
public record UpdateDealerDealLineDto(string? PlateNo = null, string? SBHOnlineNo = null, int? DeliveryOdoKm = null, DateTime? WarrantyStartDate = null, int? WarrantyMonths = null, DateTime? DeliveryDate = null, string? Remark = null);

public record GuaranteeItemInputDto(string Vin, decimal? GuaranteeValue = null, decimal GuaranteePercent = 100, DateTime? DateStart = null, string? Remark = null);
public record CreatePaymentGuaranteeDto(string BankGuaranteeNo, string BankCode, string DealerCode, decimal TotalAmount, DateTime DateOpen, DateTime DateExpired, List<GuaranteeItemInputDto>? Items = null, List<string>? Vins = null, string? BankName = null, int Term = 30, int TermActual = 30, string? Remark = null, string? GuaranteeNo = null, string? CreatedBy = null);
public record PaymentGuaranteeTransitionDto(string? Note = null, string? ApprovedBy = null, DateTime? DateExpired = null, int? Term = null, int? TermActual = null);
public record UpdatePaymentGuaranteeDto(string? BankGuaranteeNo = null, string? BankName = null, DateTime? DateExpired = null, int? Term = null, int? TermActual = null, decimal? TotalAmount = null, string? Remark = null);
public record CancelGuaranteeLineDto(string? Reason = null);

public record DealerContractItemInputDto(string Vin, decimal? UnitPrice = null, decimal? Discount = null, string? Remark = null);
public record CreateDealerContractDto(string DealerCode, List<DealerContractItemInputDto> Items, string? ContractNo = null, string? ContractNoUser = null, string? SOCode = null, string? ContractType = "Wholesale", DateTime? ContractDate = null, DateTime? DeliveryDeadline = null, int PaymentTermDays = 30, decimal DepositAmount = 0, string? Remark = null, string? CreatedBy = null);
public record DealerContractTransitionDto(string? Note = null, string? ApprovedBy = null, DateTime? DeliveryDeadline = null);
public record UpdateDealerContractLineDto(decimal? UnitPrice = null, decimal? Discount = null, string? Remark = null);

public record PaymentDiscountPhaseInputDto(DateTime? PaymentEndDate = null, decimal Amount = 0, int DiscountDateNumber = 0, decimal DiscountPercent = 0, decimal? DiscountPrice = null);
public record PaymentDiscountItemInputDto(string Vin, decimal? UnitPrice = null, string? GuaranteeNo = null, DateTime? PG_DateEnd = null, PaymentDiscountPhaseInputDto? Phase1 = null, PaymentDiscountPhaseInputDto? Phase2 = null, PaymentDiscountPhaseInputDto? Phase3 = null, decimal? TotalAmount = null, decimal? TotalDiscountPrice = null, string? Remark = null);
public record CreatePaymentDiscountDto(string DealerCode, List<PaymentDiscountItemInputDto> Items, DateTime? DateEndFrom = null, DateTime? DateEndTo = null, decimal DiscountPercent = 0, decimal PenaltyPercent = 0, string? FilePath = null, string? Remark = null, string? PaymentDiscountNo = null, string? CreatedBy = null);
public record PaymentDiscountTransitionDto(string? Note = null, string? User = null, string? FilePath = null);
public record UpdatePaymentDiscountLineDto(PaymentDiscountPhaseInputDto? Phase1 = null, PaymentDiscountPhaseInputDto? Phase2 = null, PaymentDiscountPhaseInputDto? Phase3 = null, string? GuaranteeNo = null, DateTime? PG_DateEnd = null, string? Remark = null);

public record InsuranceItemInputDto(string Vin, decimal? InsuredValue = null, decimal? PremiumRate = null, decimal? PremiumAmount = null, int InsuranceDays = 30, string? FromStorage = null, string? ToStorage = null, string? CertificateNo = null, string? Remark = null);
public record CreateInsuranceRequestDto(string InsCompanyCode, List<InsuranceItemInputDto>? Items = null, List<string>? Vins = null, string? InsCompanyName = null, string? InsTypeCode = "CARGO", string? PolicyNo = null, DateTime? EffectiveDate = null, DateTime? ExpireDate = null, decimal? PremiumRate = null, string? Remark = null, string? InsReqNo = null, string? CreatedBy = null);
public record InsuranceRequestTransitionDto(string? Note = null, string? ApprovedBy = null, string? PolicyNo = null);
public record UpdateInsuranceRequestLineDto(decimal? InsuredValue = null, decimal? PremiumRate = null, decimal? PremiumAmount = null, int? InsuranceDays = null, string? FromStorage = null, string? ToStorage = null, string? CertificateNo = null, string? Remark = null);

public record TransportMinutesItemInputDto(string Vin, string? DeliveryOrderNo = null, string? TransportReqNo = null, string? FromStorage = null, string? ToStorage = null, int OdoDeparture = 0, int OdoArrival = 0, decimal FreightAmount = 0, decimal Surcharge = 0, string? CargoCondition = "Good", bool IsInspectionPassed = true, string? Remark = null);
public record CreateTransportMinutesDto(string DealerCode, string TransporterCode, List<TransportMinutesItemInputDto>? Items = null, List<string>? Vins = null, string? TransporterName = null, string? TruckPlateNo = null, string? DriverName = null, string? DriverPhone = null, string? TransportReqNo = null, string? DeliveryOrderNo = null, DateTime? TransportMinutesDate = null, string? FilePath = null, string? Remark = null, string? TransportMinutesNo = null, string? CreatedBy = null);
public record TransportMinutesTransitionDto(string? Note = null, string? User = null, string? FilePath = null);
public record UpdateTransportMinutesLineDto(int? OdoDeparture = null, int? OdoArrival = null, decimal? FreightAmount = null, decimal? Surcharge = null, string? CargoCondition = null, bool? IsInspectionPassed = null, string? Remark = null);

public record DealerPaymentItemInputDto(string Vin, decimal? Amount = null, string? GuaranteeNo = null, string? Remark = null);
public record CreateDealerPaymentDto(
    string DealerCode,
    List<DealerPaymentItemInputDto>? Items = null,
    List<string>? Vins = null,
    decimal? TotalAmount = null,
    string? PaymentType = "Payment",
    string? BankNameSend = null,
    string? BankNameReceive = null,
    string? BankPaymentNo = null,
    string? Remark = null,
    string? PaymentNo = null,
    string? CreatedBy = null
);
public record DealerPaymentTransitionDto(
    string? Note = null,
    string? User = null,
    string? AccountingRecordNo = null,
    DateTime? PaymentEndDate = null
);
public record UpdateDealerPaymentLineDto(
    decimal? Amount = null,
    string? GuaranteeNo = null,
    string? Remark = null
);

public record StorageMaintenanceItemInputDto(
    string Vin,
    string? StorageCode = null,
    double? BatteryVoltage = 12.6,
    bool? ChargeBatteryOk = true,
    bool? EngineStartCheckOk = true,
    bool? TirePressureCheckOk = true,
    bool? TireRotationOk = true,
    bool? FluidLevelsCheckOk = true,
    bool? ElectricalSystemsOk = true,
    bool? BodyCleanOk = true,
    string? DefectNotes = null,
    string? Remark = null
);

public record CreateStorageMaintenanceDto(
    string StorageCode,
    List<StorageMaintenanceItemInputDto>? Items = null,
    List<string>? Vins = null,
    string? MtnType = "Periodic",
    DateTime? PlanDate = null,
    string? TechnicianCode = null,
    string? TechnicianName = null,
    string? SupervisorCode = null,
    string? SupervisorName = null,
    string? Remark = null,
    string? MtnNo = null,
    string? CreatedBy = null
);

public record StorageMaintenanceTransitionDto(
    string? Note = null,
    string? User = null,
    string? TechnicianName = null,
    string? SupervisorName = null
);

public record InspectStorageMaintenanceLineDto(
    double? BatteryVoltage = 12.6,
    bool? ChargeBatteryOk = true,
    bool? EngineStartCheckOk = true,
    bool? TirePressureCheckOk = true,
    bool? TireRotationOk = true,
    bool? FluidLevelsCheckOk = true,
    bool? ElectricalSystemsOk = true,
    bool? BodyCleanOk = true,
    bool? Passed = true,
    string? Technician = null,
    string? DefectNotes = null,
    string? Remark = null
);

public record UpdateStorageMaintenanceLineDto(
    double? BatteryVoltage = null,
    bool? ChargeBatteryOk = null,
    bool? EngineStartCheckOk = null,
    bool? TirePressureCheckOk = null,
    bool? TireRotationOk = null,
    bool? FluidLevelsCheckOk = null,
    bool? ElectricalSystemsOk = null,
    bool? BodyCleanOk = null,
    string? Technician = null,
    string? DefectNotes = null,
    string? Remark = null
);

public record PackingListItemInputDto(
    string Vin,
    string Model,
    string? SpecCode = null,
    string? EngineNo = null,
    string? Color = null,
    int? ModelYear = 2026,
    string? KeyNo = null,
    DateTime? ProductionDate = null,
    decimal UnitPrice = 0,
    string? Remark = null
);

public record CreatePackingListDto(
    string? PortCode = "NHA_MAY_NINH_BINH",
    List<PackingListItemInputDto>? Items = null,
    List<string>? Vins = null,
    string? ContractNo = null,
    string? LCNo = null,
    string? VesselName = null,
    string? VoyageNo = null,
    DateTime? ShippingDateStart = null,
    DateTime? ShippingDateEndExpected = null,
    string? Remark = null,
    string? PackingListNo = null,
    string? CreatedBy = null
);

public record PackingListTransitionDto(
    string? Note = null,
    string? User = null,
    string? ApprovedBy = null,
    DateTime? ShippingDateEnd = null
);

public record UpdatePackingListLineDto(
    string? Model = null,
    string? SpecCode = null,
    string? EngineNo = null,
    string? Color = null,
    int? ModelYear = null,
    string? KeyNo = null,
    DateTime? ProductionDate = null,
    decimal? UnitPrice = null,
    string? Remark = null
);

public record CustomsDeclarationItemInputDto(
    string Vin,
    string? Model = null,
    string? SpecCode = null,
    string? EngineNo = null,
    string? Color = null,
    int? ModelYear = 2026,
    string? PackingListNo = null,
    decimal? TaxValue = null,
    decimal? ImportTaxRate = 50,
    decimal? ImportTax = null,
    decimal? ExciseTaxRate = 35,
    decimal? ExciseTax = null,
    decimal? VatRate = 10,
    decimal? VatTax = null,
    string? Remark = null
);

public record CreateCustomsDeclarationDto(
    string? PortCode = "HQ_HAI_PHONG",
    string? PortName = null,
    List<CustomsDeclarationItemInputDto>? Items = null,
    List<string>? Vins = null,
    string? ContractNo = null,
    string? LCNo = null,
    string? BillOfLadingNo = null,
    string? DeclarationType = "CBU",
    DateTime? OpenDate = null,
    string? CustomsOfficer = null,
    string? DeclarantName = null,
    string? Remark = null,
    string? DeclarationNo = null,
    string? CreatedBy = null
);

public record CustomsDeclarationTransitionDto(
    string? Note = null,
    string? User = null,
    string? CustomsOfficer = null,
    DateTime? TaxPaymentDate = null,
    DateTime? ClearanceDate = null
);

public record UpdateCustomsDeclarationLineDto(
    decimal? TaxValue = null,
    decimal? ImportTaxRate = null,
    decimal? ImportTax = null,
    decimal? ExciseTaxRate = null,
    decimal? ExciseTax = null,
    decimal? VatRate = null,
    decimal? VatTax = null,
    DateTime? TaxPaymentDate = null,
    DateTime? ClearanceDate = null,
    string? Remark = null
);

public record UpdateCustomsDeclarationTaxPaymentDto(
    DateTime? TaxPaymentDate = null,
    string? Note = null,
    string? User = null
);

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
    Task<object> CreateTestCarAsync(CreateTestCarDto dto);
    Task<object> ListTestCarsAsync(string? status, string? dealer, string? vin);
    Task<object?> GetTestCarAsync(string testCarCode);
    Task<object?> TestCarTransitionAsync(string testCarCode, string action, TestCarTransitionDto? dto);
    Task<object?> FinishTestCarLineAsync(string testCarCode, string vin, FinishTestCarLineDto? dto);
    Task<object> CreatePdiRequestAsync(CreatePdiRequestDto dto);
    Task<object> ListPdiRequestsAsync(string? status, string? dealer, string? vin);
    Task<object?> GetPdiRequestAsync(string pdiReqNo);
    Task<object?> PdiRequestTransitionAsync(string pdiReqNo, string action, PdiRequestTransitionDto? dto);
    Task<object?> InspectPdiLineAsync(string pdiReqNo, string vin, InspectPdiLineDto dto);
    Task<object> CreateMortgageRequestAsync(CreateMortgageRequestDto dto);
    Task<object> ListMortgageRequestsAsync(string? status, string? bank, string? vin);
    Task<object?> GetMortgageRequestAsync(string reqMortgageNo);
    Task<object?> MortgageRequestTransitionAsync(string reqMortgageNo, string action, MortgageRequestTransitionDto? dto);
    Task<object> CreateRedeemRequestAsync(CreateRedeemRequestDto dto);
    Task<object> ListRedeemRequestsAsync(string? status, string? dealer, string? bank, string? vin);
    Task<object?> GetRedeemRequestAsync(string redeemReqNo);
    Task<object?> RedeemRequestTransitionAsync(string redeemReqNo, string action, RedeemRequestTransitionDto? dto);
    Task<object> CreateSalesOrderAsync(CreateSalesOrderDto dto);
    Task<object> ListSalesOrdersAsync(string? status, string? dealer, string? orderMonth, string? model);
    Task<object?> GetSalesOrderAsync(string soCode);
    Task<object?> SalesOrderTransitionAsync(string soCode, string action, SalesOrderTransitionDto? dto);
    Task<object?> AllocateSoVinAsync(string soCode, AllocateSoVinDto dto);
    Task<object?> DeallocateSoVinAsync(string soCode, string vin);
    Task<object> CreateDealerDealAsync(CreateDealerDealDto dto);
    Task<object> ListDealerDealsAsync(string? status, string? dealer, string? customer, string? salesMan, string? paymentType, string? vin);
    Task<object?> GetDealerDealAsync(string dealNo);
    Task<object?> DealerDealTransitionAsync(string dealNo, string action, DealerDealTransitionDto? dto);
    Task<object?> UpdateDealerDealLineAsync(string dealNo, string vin, UpdateDealerDealLineDto dto);
    Task<object> CreatePaymentGuaranteeAsync(CreatePaymentGuaranteeDto dto);
    Task<object> ListPaymentGuaranteesAsync(string? status, string? dealer, string? bank, string? vin);
    Task<object?> GetPaymentGuaranteeAsync(string guaranteeNo);
    Task<object?> PaymentGuaranteeTransitionAsync(string guaranteeNo, string action, PaymentGuaranteeTransitionDto? dto);
    Task<object?> CancelPaymentGuaranteeLineAsync(string guaranteeNo, string vin, string? reason);
    Task<object?> UpdatePaymentGuaranteeAsync(string guaranteeNo, UpdatePaymentGuaranteeDto dto);
    Task<object> CreateDealerContractAsync(CreateDealerContractDto dto);
    Task<object> ListDealerContractsAsync(string? status, string? dealer, string? contractNo, string? soCode, string? vin);
    Task<object?> GetDealerContractAsync(string contractNo);
    Task<object?> DealerContractTransitionAsync(string contractNo, string action, DealerContractTransitionDto? dto);
    Task<object?> UpdateDealerContractLineAsync(string contractNo, string vin, UpdateDealerContractLineDto dto);
    Task<object> CreatePaymentDiscountAsync(CreatePaymentDiscountDto dto);
    Task<object> ListPaymentDiscountsAsync(string? status, string? dealer, string? paymentDiscountNo, string? vin);
    Task<object?> GetPaymentDiscountAsync(string paymentDiscountNo);
    Task<object?> PaymentDiscountTransitionAsync(string paymentDiscountNo, string action, PaymentDiscountTransitionDto? dto);
    Task<object?> UpdatePaymentDiscountLineAsync(string paymentDiscountNo, string vin, UpdatePaymentDiscountLineDto dto);
    Task<object> CreateInsuranceRequestAsync(CreateInsuranceRequestDto dto);
    Task<object> ListInsuranceRequestsAsync(string? status, string? insCompanyCode, string? insTypeCode, string? vin);
    Task<object?> GetInsuranceRequestAsync(string insReqNo);
    Task<object?> InsuranceRequestTransitionAsync(string insReqNo, string action, InsuranceRequestTransitionDto? dto);
    Task<object?> UpdateInsuranceRequestLineAsync(string insReqNo, string vin, UpdateInsuranceRequestLineDto dto);
    Task<object?> AddInsuranceRequestLinesAsync(string insReqNo, List<InsuranceItemInputDto> items);
    Task<object?> RemoveInsuranceRequestLineAsync(string insReqNo, string vin);
    Task<object> CreateTransportMinutesAsync(CreateTransportMinutesDto dto);
    Task<object> ListTransportMinutesAsync(string? status, string? dealer, string? transporter, string? vin);
    Task<object?> GetTransportMinutesAsync(string transportMinutesNo);
    Task<object?> TransportMinutesTransitionAsync(string transportMinutesNo, string action, TransportMinutesTransitionDto? dto);
    Task<object?> UpdateTransportMinutesLineAsync(string transportMinutesNo, string vin, UpdateTransportMinutesLineDto dto);
    Task<object?> AddTransportMinutesLinesAsync(string transportMinutesNo, List<TransportMinutesItemInputDto> items);
    Task<object?> RemoveTransportMinutesLineAsync(string transportMinutesNo, string vin);
    Task<object> CreateDealerPaymentAsync(CreateDealerPaymentDto dto);
    Task<object> ListDealerPaymentsAsync(string? status, string? dealer, string? paymentType, string? paymentNo, string? vin);
    Task<object?> GetDealerPaymentAsync(string paymentNo);
    Task<object?> DealerPaymentTransitionAsync(string paymentNo, string action, DealerPaymentTransitionDto? dto);
    Task<object?> UpdateDealerPaymentLineAsync(string paymentNo, string vin, UpdateDealerPaymentLineDto dto);
    Task<object?> AddDealerPaymentLinesAsync(string paymentNo, List<DealerPaymentItemInputDto> items);
    Task<object?> RemoveDealerPaymentLineAsync(string paymentNo, string vin);
    Task<object> CreateStorageMaintenanceAsync(CreateStorageMaintenanceDto dto);
    Task<object> ListStorageMaintenancesAsync(string? status, string? storageCode, string? mtnType, string? mtnNo, string? vin);
    Task<object?> GetStorageMaintenanceAsync(string mtnNo);
    Task<object?> StorageMaintenanceTransitionAsync(string mtnNo, string action, StorageMaintenanceTransitionDto? dto);
    Task<object?> InspectStorageMaintenanceLineAsync(string mtnNo, string vin, InspectStorageMaintenanceLineDto dto);
    Task<object?> UpdateStorageMaintenanceLineAsync(string mtnNo, string vin, UpdateStorageMaintenanceLineDto dto);
    Task<object?> AddStorageMaintenanceLinesAsync(string mtnNo, List<StorageMaintenanceItemInputDto> items);
    Task<object?> RemoveStorageMaintenanceLineAsync(string mtnNo, string vin);
    Task<object> GetDueMaintenanceVehiclesAsync(string? storageCode, int dueWithinDays = 7);
    Task<object?> GetVehicleMaintenanceHistoryAsync(string vin);
    Task<object> CreatePackingListAsync(CreatePackingListDto dto);
    Task<object> ListPackingListsAsync(string? status, string? portCode, string? contractNo, string? vesselName, string? vin);
    Task<object?> GetPackingListAsync(string packingListNo);
    Task<object?> PackingListTransitionAsync(string packingListNo, string action, PackingListTransitionDto? dto);
    Task<object?> UpdatePackingListLineAsync(string packingListNo, string vin, UpdatePackingListLineDto dto);
    Task<object?> AddPackingListLinesAsync(string packingListNo, List<PackingListItemInputDto> items);
    Task<object?> RemovePackingListLineAsync(string packingListNo, string vin);
    Task<object> CreateCustomsDeclarationAsync(CreateCustomsDeclarationDto dto);
    Task<object> ListCustomsDeclarationsAsync(string? status, string? portCode, string? contractNo, string? declarationType, string? declarationNo, string? vin);
    Task<object?> GetCustomsDeclarationAsync(string declarationNo);
    Task<object?> CustomsDeclarationTransitionAsync(string declarationNo, string action, CustomsDeclarationTransitionDto? dto);
    Task<object?> UpdateCustomsDeclarationLineAsync(string declarationNo, string vin, UpdateCustomsDeclarationLineDto dto);
    Task<object?> AddCustomsDeclarationLinesAsync(string declarationNo, List<CustomsDeclarationItemInputDto> items);
    Task<object?> RemoveCustomsDeclarationLineAsync(string declarationNo, string vin);
    Task<object?> UpdateCustomsDeclarationTaxPaymentAsync(string declarationNo, UpdateCustomsDeclarationTaxPaymentDto dto);
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
            v.IsTestCar, v.IsMortgaged, v.MortgageBankCode,
            v.IsPaid, v.PaidAmount, v.PaidAt,
            v.StorageCode, v.PackingListNo, v.DealerCode, v.OwnerName, v.PlateNo, v.DeliveredAt, v.WarrantyEnd
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
            v.StorageCode, v.IsTestCar, v.IsMortgaged, v.MortgageBankCode, v.MortgageDate, v.RedeemDate,
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
            v.IsTestCar, v.IsMortgaged, v.MortgageBankCode,
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

    // ===== Đăng ký / Mượn xe chạy thử - lái thử xe (BizHTC.Car.Car_TestCar / TestCar) =====
    public async Task<object> CreateTestCarAsync(CreateTestCarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode) || dto.Vins is null || dto.Vins.Count == 0)
            throw new InvalidOperationException("Cần DealerCode và ít nhất 1 VIN để đăng ký xe lái thử.");

        var dealer = dto.DealerCode.Trim();
        var vins = dto.Vins.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Kiểm tra xe không được ở trạng thái đã giao cho khách hàng lẻ (Delivered)
        var invalidDelivered = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (invalidDelivered.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng cuối (Delivered) không thể đăng ký lái thử: " + string.Join(", ", invalidDelivered));

        // Kiểm tra quy tắc 2010.HTC: 1 VIN chỉ thuộc 1 đề nghị lái thử đang hoạt động (Pending, Approved, InUse)
        var activeLines = await db.TestCarLines
            .Where(l => l.OrgId == Org && vins.Contains(l.Vin) && (l.Status == "Pending" || l.Status == "Approved" || l.Status == "InUse"))
            .ToListAsync();
        if (activeLines.Count > 0)
        {
            var conflict = activeLines.First();
            throw new InvalidOperationException($"VIN {conflict.Vin} đang nằm trong lệnh lái thử khác chưa hoàn tất ({conflict.TestCarCode}).");
        }

        var reqCode = string.IsNullOrWhiteSpace(dto.TestCarCode)
            ? "TC" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.TestCarCode.Trim().ToUpperInvariant();

        if (await db.TestCars.AnyAsync(r => r.OrgId == Org && r.TestCarCode == reqCode))
            throw new InvalidOperationException($"Mã phiếu lái thử {reqCode} đã tồn tại.");

        var tc = new TestCarRequest
        {
            OrgId = Org,
            TestCarCode = reqCode,
            DealerCode = dealer,
            EventName = dto.EventName?.Trim(),
            Purpose = dto.Purpose?.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Pending",
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.TestCars.Add(tc);
        await db.SaveChangesAsync();

        foreach (var v in vehicles)
        {
            db.TestCarLines.Add(new TestCarLine
            {
                OrgId = Org,
                TestCarRequestId = tc.Id,
                TestCarCode = reqCode,
                Vin = v.Vin,
                OdoStart = 0,
                Status = "Pending",
                Remark = dto.Remark?.Trim()
            });
            Log(v.Vin, "TestCarRequested", $"{reqCode} ĐL:{dealer} Mục đích:{dto.Purpose ?? dto.EventName ?? "Lái thử xe"}");
        }
        await db.SaveChangesAsync();

        return new
        {
            tc.TestCarCode,
            tc.DealerCode,
            tc.EventName,
            tc.Purpose,
            tc.Status,
            totalVins = vins.Count,
            vins
        };
    }

    public async Task<object> ListTestCarsAsync(string? status, string? dealer, string? vin)
    {
        var q = db.TestCars.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(r => r.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedCodes = await db.TestCarLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.TestCarCode).Distinct().ToListAsync();
            q = q.Where(r => matchedCodes.Contains(r.TestCarCode));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.TestCarCode,
            r.DealerCode,
            r.EventName,
            r.Purpose,
            r.StartDate,
            r.EndDate,
            r.Status,
            r.Remark,
            r.CreatedAt,
            r.ApprovedAt,
            r.FinishedAt,
            vinCount = db.TestCarLines.Count(l => l.OrgId == Org && l.TestCarRequestId == r.Id),
            finishedCount = db.TestCarLines.Count(l => l.OrgId == Org && l.TestCarRequestId == r.Id && l.Status == "Finished")
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetTestCarAsync(string testCarCode)
    {
        testCarCode = testCarCode.Trim().ToUpperInvariant();
        var tc = await db.TestCars.FirstOrDefaultAsync(r => r.OrgId == Org && r.TestCarCode == testCarCode);
        if (tc is null) return null;

        var lines = await db.TestCarLines.Where(l => l.OrgId == Org && l.TestCarRequestId == tc.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.OdoStart,
            l.OdoEnd,
            l.ConditionStart,
            l.ConditionEnd,
            l.HandoverDate,
            l.ReturnDate,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.StorageCode, v.DealerCode, v.IsTestCar } : null
        }).ToList();

        return new
        {
            tc.TestCarCode,
            tc.DealerCode,
            tc.EventName,
            tc.Purpose,
            tc.StartDate,
            tc.EndDate,
            tc.Status,
            tc.Remark,
            tc.CreatedAt,
            tc.ApprovedAt,
            tc.FinishedAt,
            vins = details
        };
    }

    public async Task<object?> TestCarTransitionAsync(string testCarCode, string action, TestCarTransitionDto? dto)
    {
        testCarCode = testCarCode.Trim().ToUpperInvariant();
        var tc = await db.TestCars.FirstOrDefaultAsync(r => r.OrgId == Org && r.TestCarCode == testCarCode);
        if (tc is null) return null;

        var now = DateTime.Now;
        var lines = await db.TestCarLines.Where(l => l.OrgId == Org && l.TestCarRequestId == tc.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (tc.Status != "Pending") return null;
                tc.Status = "Approved";
                tc.ApprovedAt = now;
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles)
                {
                    v.IsTestCar = true;
                    Log(v.Vin, "TestCarApproved", testCarCode);
                }
                break;

            case "start":
            case "handover":
            case "inuse":
                if (tc.Status is not ("Pending" or "Approved")) return null;
                tc.Status = "InUse";
                tc.ApprovedAt ??= now;
                foreach (var l in lines)
                {
                    if (l.Status is "Pending" or "Approved")
                    {
                        l.Status = "InUse";
                        l.HandoverDate ??= now;
                    }
                }
                foreach (var v in vehicles)
                {
                    v.IsTestCar = true;
                    Log(v.Vin, "TestCarInUse", $"{testCarCode} Bàn giao xe lái thử cho ĐL:{tc.DealerCode}");
                }
                break;

            case "finish":
            case "complete":
                if (tc.Status is not ("Approved" or "InUse")) return null;
                tc.Status = "Finished";
                tc.FinishedAt = now;
                foreach (var l in lines)
                {
                    if (l.Status != "Finished")
                    {
                        l.Status = "Finished";
                        l.HandoverDate ??= now;
                        l.ReturnDate ??= now;
                    }
                }
                foreach (var v in vehicles)
                {
                    v.IsTestCar = false;
                    Log(v.Vin, "TestCarFinished", $"{testCarCode} Hoàn tất chương trình lái thử, trả lại xe.");
                }
                break;

            case "reject":
                if (tc.Status != "Pending") return null;
                tc.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tc.Remark = string.IsNullOrWhiteSpace(tc.Remark) ? dto.Note : $"{tc.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles)
                {
                    v.IsTestCar = false;
                    Log(v.Vin, "TestCarRejected", $"{testCarCode} Lý do: {dto?.Note ?? "N/A"}");
                }
                break;

            case "cancel":
                if (tc.Status is not ("Pending" or "Approved" or "InUse")) return null;
                tc.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tc.Remark = string.IsNullOrWhiteSpace(tc.Remark) ? dto.Note : $"{tc.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) if (l.Status != "Finished") l.Status = "Cancelled";
                foreach (var v in vehicles)
                {
                    v.IsTestCar = false;
                    Log(v.Vin, "TestCarCancelled", $"{testCarCode} Lý do: {dto?.Note ?? "N/A"}");
                }
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { tc.TestCarCode, tc.DealerCode, status = tc.Status, tc.ApprovedAt, tc.FinishedAt };
    }

    public async Task<object?> FinishTestCarLineAsync(string testCarCode, string vin, FinishTestCarLineDto? dto)
    {
        testCarCode = testCarCode.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var tc = await db.TestCars.FirstOrDefaultAsync(r => r.OrgId == Org && r.TestCarCode == testCarCode);
        if (tc is null || tc.Status is "Cancelled" or "Rejected") return null;

        var line = await db.TestCarLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.TestCarRequestId == tc.Id && l.Vin == vin);
        if (line is null) return null;

        var now = DateTime.Now;
        line.Status = "Finished";
        line.HandoverDate ??= now;
        line.ReturnDate = dto?.ReturnDate ?? now;
        if (dto?.OdoEnd.HasValue == true) line.OdoEnd = dto.OdoEnd.Value;
        if (!string.IsNullOrWhiteSpace(dto?.ConditionEnd)) line.ConditionEnd = dto.ConditionEnd.Trim();
        if (!string.IsNullOrWhiteSpace(dto?.Remark))
            line.Remark = string.IsNullOrWhiteSpace(line.Remark) ? dto.Remark : $"{line.Remark} | {dto.Remark}";

        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == Org && v.Vin == vin);
        if (vehicle != null)
        {
            vehicle.IsTestCar = false;
            Log(vin, "TestCarLineFinished", $"{testCarCode} Hoàn trả xe lái thử. ODO={line.OdoEnd} Tình trạng={line.ConditionEnd ?? "OK"}");
        }

        // Cập nhật trạng thái tổng thể của phiếu
        var allLines = await db.TestCarLines.Where(l => l.OrgId == Org && l.TestCarRequestId == tc.Id).ToListAsync();
        if (allLines.All(l => l.Status == "Finished"))
        {
            tc.Status = "Finished";
            tc.FinishedAt = now;
        }
        else if (tc.Status == "Approved" || tc.Status == "Pending")
        {
            tc.Status = "InUse";
        }

        await db.SaveChangesAsync();
        return new
        {
            tc.TestCarCode,
            line.Vin,
            line.OdoStart,
            line.OdoEnd,
            line.ConditionEnd,
            line.Status,
            line.ReturnDate,
            headerStatus = tc.Status
        };
    }

    // ===== Yêu cầu & Kiểm tra chất lượng tiền bàn giao xe PDI (BizHTC.WH.DlrPDIRequest / Dlr_PDIRequest) =====
    public async Task<object> CreatePdiRequestAsync(CreatePdiRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần mã đại lý DealerCode để tạo yêu cầu PDI.");

        var dealer = dto.DealerCode.Trim();

        // Thu thập danh sách xe (từ Items hoặc Vins)
        var itemList = new List<PdiItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0)
        {
            itemList.AddRange(dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)));
        }
        else if (dto.Vins != null && dto.Vins.Count > 0)
        {
            itemList.AddRange(dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => new PdiItemInputDto(v.Trim())));
        }

        if (itemList.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe (VIN) để tạo phiếu yêu cầu PDI.");

        // Khử trùng lặp VIN trong cùng 1 phiếu
        var distinctItems = itemList
            .GroupBy(i => i.Vin.Trim().ToUpperInvariant())
            .Select(g => g.First())
            .ToList();

        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Kiểm tra xe không được ở trạng thái đã giao cho khách hàng (Delivered)
        var invalidDelivered = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (invalidDelivered.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng cuối (Delivered) không thể tạo kiểm tra tiền bàn giao PDI: " + string.Join(", ", invalidDelivered));

        var reqNo = string.IsNullOrWhiteSpace(dto.PdiReqNo)
            ? "PDI" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.PdiReqNo.Trim().ToUpperInvariant();

        if (await db.PdiRequests.AnyAsync(r => r.OrgId == Org && r.PdiReqNo == reqNo))
            throw new InvalidOperationException($"Mã phiếu PDI {reqNo} đã tồn tại.");

        var pdi = new PdiRequest
        {
            OrgId = Org,
            PdiReqNo = reqNo,
            DealerCode = dealer,
            InspectorName = dto.InspectorName?.Trim(),
            Remark = dto.Remark?.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.Now
        };
        db.PdiRequests.Add(pdi);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            db.PdiRequestLines.Add(new PdiRequestLine
            {
                OrgId = Org,
                PdiRequestId = pdi.Id,
                PdiReqNo = reqNo,
                Vin = vin,
                DlrContractNo = item.DlrContractNo?.Trim(),
                RoStatus = "NORE",
                BatteryVoltage = 12.6,
                TirePressureOk = true,
                FluidsOk = true,
                ElectronicsOk = true,
                ExteriorOk = true,
                InteriorCleanOk = true,
                DiagnosticScanOk = true,
                PdiResult = "Pending",
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "PdiRequested", $"{reqNo} ĐL:{dealer} KTV:{dto.InspectorName ?? "Chưa phân công"}");
        }
        await db.SaveChangesAsync();

        return new
        {
            pdi.PdiReqNo,
            pdi.DealerCode,
            pdi.InspectorName,
            pdi.Status,
            totalVins = distinctItems.Count,
            vins = distinctItems.Select(i => new { i.Vin, i.DlrContractNo })
        };
    }

    public async Task<object> ListPdiRequestsAsync(string? status, string? dealer, string? vin)
    {
        var q = db.PdiRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(r => r.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.PdiRequestLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.PdiReqNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.PdiReqNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.PdiReqNo,
            r.DealerCode,
            r.InspectorName,
            r.ApprovedBy,
            r.Status,
            r.Remark,
            r.CreatedAt,
            r.ApprovedAt,
            r.CompletedAt,
            vinCount = db.PdiRequestLines.Count(l => l.OrgId == Org && l.PdiRequestId == r.Id),
            passedCount = db.PdiRequestLines.Count(l => l.OrgId == Org && l.PdiRequestId == r.Id && l.PdiResult == "Passed"),
            failedCount = db.PdiRequestLines.Count(l => l.OrgId == Org && l.PdiRequestId == r.Id && l.PdiResult == "Failed")
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetPdiRequestAsync(string pdiReqNo)
    {
        pdiReqNo = pdiReqNo.Trim().ToUpperInvariant();
        var pdi = await db.PdiRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.PdiReqNo == pdiReqNo);
        if (pdi is null) return null;

        var lines = await db.PdiRequestLines.Where(l => l.OrgId == Org && l.PdiRequestId == pdi.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.DlrContractNo,
            l.RoNo,
            l.RoStatus,
            l.BatteryVoltage,
            l.TirePressureOk,
            l.FluidsOk,
            l.ElectronicsOk,
            l.ExteriorOk,
            l.InteriorCleanOk,
            l.DiagnosticScanOk,
            l.PdiResult,
            l.Status,
            l.InspectedAt,
            l.InspectedBy,
            l.DefectNotes,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.StorageCode, v.DealerCode } : null
        }).ToList();

        return new
        {
            pdi.PdiReqNo,
            pdi.DealerCode,
            pdi.InspectorName,
            pdi.ApprovedBy,
            pdi.Status,
            pdi.Remark,
            pdi.CreatedAt,
            pdi.ApprovedAt,
            pdi.CompletedAt,
            vins = details
        };
    }

    public async Task<object?> PdiRequestTransitionAsync(string pdiReqNo, string action, PdiRequestTransitionDto? dto)
    {
        pdiReqNo = pdiReqNo.Trim().ToUpperInvariant();
        var pdi = await db.PdiRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.PdiReqNo == pdiReqNo);
        if (pdi is null) return null;

        var now = DateTime.Now;
        var lines = await db.PdiRequestLines.Where(l => l.OrgId == Org && l.PdiRequestId == pdi.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (pdi.Status != "Pending") return null;
                pdi.Status = "Approved";
                pdi.ApprovedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.ApprovedBy)) pdi.ApprovedBy = dto.ApprovedBy.Trim();
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "PdiApproved", $"{pdiReqNo} Duyệt bởi:{pdi.ApprovedBy ?? "Manager"}");
                break;

            case "start":
            case "inspect":
                if (pdi.Status is not ("Pending" or "Approved")) return null;
                pdi.Status = "InProgress";
                pdi.ApprovedAt ??= now;
                if (!string.IsNullOrWhiteSpace(dto?.InspectorName)) pdi.InspectorName = dto.InspectorName.Trim();
                foreach (var l in lines)
                {
                    if (l.Status is "Pending" or "Approved")
                        l.Status = "Inspected";
                }
                foreach (var v in vehicles) Log(v.Vin, "PdiInProgress", $"{pdiReqNo} Tiến hành kiểm tra PDI");
                break;

            case "complete":
            case "pass":
                if (pdi.Status is not ("Approved" or "InProgress" or "Pending")) return null;
                pdi.Status = "Completed";
                pdi.CompletedAt = now;
                foreach (var l in lines)
                {
                    if (l.Status != "Completed")
                    {
                        l.Status = "Completed";
                        l.PdiResult = "Passed";
                        l.InspectedAt ??= now;
                        l.InspectedBy ??= dto?.InspectorName ?? pdi.InspectorName ?? "Inspector";
                    }
                }
                foreach (var v in vehicles) Log(v.Vin, "PdiCompleted", $"{pdiReqNo} Hoàn tất kiểm tra PDI đạt chuẩn tiền bàn giao.");
                break;

            case "reject":
                if (pdi.Status != "Pending") return null;
                pdi.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pdi.Remark = string.IsNullOrWhiteSpace(pdi.Remark) ? dto.Note : $"{pdi.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines)
                {
                    l.Status = "Rejected";
                    l.PdiResult = "Failed";
                }
                foreach (var v in vehicles) Log(v.Vin, "PdiRejected", $"{pdiReqNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (pdi.Status is not ("Pending" or "Approved" or "InProgress")) return null;
                pdi.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pdi.Remark = string.IsNullOrWhiteSpace(pdi.Remark) ? dto.Note : $"{pdi.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) if (l.Status != "Completed") l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "PdiCancelled", $"{pdiReqNo} Lý do: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { pdi.PdiReqNo, pdi.DealerCode, status = pdi.Status, pdi.ApprovedAt, pdi.CompletedAt };
    }

    public async Task<object?> InspectPdiLineAsync(string pdiReqNo, string vin, InspectPdiLineDto dto)
    {
        pdiReqNo = pdiReqNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var pdi = await db.PdiRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.PdiReqNo == pdiReqNo);
        if (pdi is null || pdi.Status is "Cancelled" or "Rejected") return null;

        var line = await db.PdiRequestLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.PdiRequestId == pdi.Id && l.Vin == vin);
        if (line is null) return null;

        var now = DateTime.Now;
        line.InspectedAt = now;
        if (!string.IsNullOrWhiteSpace(dto.InspectorName)) line.InspectedBy = dto.InspectorName.Trim();
        else if (string.IsNullOrWhiteSpace(line.InspectedBy)) line.InspectedBy = pdi.InspectorName ?? "Inspector";

        if (dto.BatteryVoltage.HasValue) line.BatteryVoltage = dto.BatteryVoltage.Value;
        if (dto.TirePressureOk.HasValue) line.TirePressureOk = dto.TirePressureOk.Value;
        if (dto.FluidsOk.HasValue) line.FluidsOk = dto.FluidsOk.Value;
        if (dto.ElectronicsOk.HasValue) line.ElectronicsOk = dto.ElectronicsOk.Value;
        if (dto.ExteriorOk.HasValue) line.ExteriorOk = dto.ExteriorOk.Value;
        if (dto.InteriorCleanOk.HasValue) line.InteriorCleanOk = dto.InteriorCleanOk.Value;
        if (dto.DiagnosticScanOk.HasValue) line.DiagnosticScanOk = dto.DiagnosticScanOk.Value;

        if (!string.IsNullOrWhiteSpace(dto.RoNo)) line.RoNo = dto.RoNo.Trim();
        if (!string.IsNullOrWhiteSpace(dto.RoStatus)) line.RoStatus = dto.RoStatus.Trim();
        if (!string.IsNullOrWhiteSpace(dto.DefectNotes)) line.DefectNotes = dto.DefectNotes.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Remark))
            line.Remark = string.IsNullOrWhiteSpace(line.Remark) ? dto.Remark : $"{line.Remark} | {dto.Remark}";

        bool isPassed = dto.Passed ?? (
            line.BatteryVoltage >= 12.0 &&
            line.TirePressureOk &&
            line.FluidsOk &&
            line.ElectronicsOk &&
            line.ExteriorOk &&
            line.InteriorCleanOk &&
            line.DiagnosticScanOk &&
            string.IsNullOrWhiteSpace(line.DefectNotes)
        );

        if (isPassed)
        {
            line.PdiResult = "Passed";
            line.Status = "Completed";
            Log(vin, "PdiLinePassed", $"{pdiReqNo} Kiểm tra PDI ĐẠT. KTV: {line.InspectedBy}. Ắc quy={line.BatteryVoltage}V");
        }
        else
        {
            line.PdiResult = "Failed";
            line.Status = "Inspected";
            Log(vin, "PdiLineFailed", $"{pdiReqNo} Kiểm tra PDI KHÔNG ĐẠT. KTV: {line.InspectedBy}. Lỗi: {line.DefectNotes ?? "Hạng mục không đạt chuẩn"}");
        }

        // Cập nhật trạng thái tổng thể của phiếu PDI
        var allLines = await db.PdiRequestLines.Where(l => l.OrgId == Org && l.PdiRequestId == pdi.Id).ToListAsync();
        if (allLines.All(l => l.Status == "Completed" || l.PdiResult == "Passed"))
        {
            pdi.Status = "Completed";
            pdi.CompletedAt = now;
        }
        else if (pdi.Status == "Approved" || pdi.Status == "Pending")
        {
            pdi.Status = "InProgress";
        }

        await db.SaveChangesAsync();
        return new
        {
            pdi.PdiReqNo,
            line.Vin,
            line.PdiResult,
            line.Status,
            line.BatteryVoltage,
            line.TirePressureOk,
            line.FluidsOk,
            line.ElectronicsOk,
            line.ExteriorOk,
            line.InteriorCleanOk,
            line.DiagnosticScanOk,
            line.RoNo,
            line.RoStatus,
            line.InspectedBy,
            line.InspectedAt,
            line.DefectNotes,
            headerStatus = pdi.Status
        };
    }

    // ===== Thế chấp xe ngân hàng (BizHTC.GiaiChap.RM_ReqMortgage / RM_ReqMortgage) =====
    public async Task<object> CreateMortgageRequestAsync(CreateMortgageRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BankCode))
            throw new InvalidOperationException("Cần mã Ngân hàng thế chấp (BankCode).");

        var items = new List<MortgageItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0) items.AddRange(dto.Items);
        else if (dto.Vins != null && dto.Vins.Count > 0) items.AddRange(dto.Vins.Select(v => new MortgageItemInputDto(v)));

        if (items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 VIN trong yêu cầu thế chấp.");

        var distinctItems = items.GroupBy(i => i.Vin.Trim().ToUpperInvariant()).Select(g => g.First()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var reqNo = string.IsNullOrWhiteSpace(dto.ReqMortgageNo)
            ? "RM" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.ReqMortgageNo!.Trim().ToUpperInvariant();

        if (await db.MortgageRequests.AnyAsync(r => r.OrgId == Org && r.ReqMortgageNo == reqNo))
            throw new InvalidOperationException($"Mã yêu cầu thế chấp {reqNo} đã tồn tại.");

        var bankCode = dto.BankCode.Trim().ToUpperInvariant();
        var m = new MortgageRequest
        {
            OrgId = Org,
            ReqMortgageNo = reqNo,
            BankCode = bankCode,
            MortgageDate = dto.MortgageDate ?? DateTime.Now,
            Remark = dto.Remark?.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.Now
        };
        db.MortgageRequests.Add(m);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            db.MortgageRequestLines.Add(new MortgageRequestLine
            {
                OrgId = Org,
                MortgageRequestId = m.Id,
                ReqMortgageNo = reqNo,
                Vin = vin,
                MortgageAmount = item.MortgageAmount,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "MortgageRequested", $"{reqNo} Ngân hàng: {bankCode}. Định giá: {item.MortgageAmount:N0}");
        }
        await db.SaveChangesAsync();

        return new
        {
            m.ReqMortgageNo,
            m.BankCode,
            m.MortgageDate,
            m.Status,
            totalVins = distinctItems.Count,
            totalAmount = distinctItems.Sum(i => i.MortgageAmount),
            vins = distinctItems.Select(i => new { i.Vin, i.MortgageAmount })
        };
    }

    public async Task<object> ListMortgageRequestsAsync(string? status, string? bank, string? vin)
    {
        var q = db.MortgageRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(bank)) { var b = bank.Trim().ToUpperInvariant(); q = q.Where(r => r.BankCode == b); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.MortgageRequestLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.ReqMortgageNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.ReqMortgageNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.ReqMortgageNo,
            r.BankCode,
            r.MortgageDate,
            r.Status,
            r.Remark,
            r.CreatedAt,
            r.ApprovedAt,
            vinCount = db.MortgageRequestLines.Count(l => l.OrgId == Org && l.MortgageRequestId == r.Id),
            totalAmount = db.MortgageRequestLines.Where(l => l.OrgId == Org && l.MortgageRequestId == r.Id).Sum(l => l.MortgageAmount)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetMortgageRequestAsync(string reqMortgageNo)
    {
        reqMortgageNo = reqMortgageNo.Trim().ToUpperInvariant();
        var m = await db.MortgageRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.ReqMortgageNo == reqMortgageNo);
        if (m is null) return null;

        var lines = await db.MortgageRequestLines.Where(l => l.OrgId == Org && l.MortgageRequestId == m.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.MortgageAmount,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.StorageCode, v.DealerCode, v.IsMortgaged, v.MortgageBankCode } : null
        }).ToList();

        return new
        {
            m.ReqMortgageNo,
            m.BankCode,
            m.MortgageDate,
            m.Status,
            m.Remark,
            m.CreatedAt,
            m.ApprovedAt,
            totalAmount = lines.Sum(l => l.MortgageAmount),
            vins = details
        };
    }

    public async Task<object?> MortgageRequestTransitionAsync(string reqMortgageNo, string action, MortgageRequestTransitionDto? dto)
    {
        reqMortgageNo = reqMortgageNo.Trim().ToUpperInvariant();
        var m = await db.MortgageRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.ReqMortgageNo == reqMortgageNo);
        if (m is null) return null;

        var now = DateTime.Now;
        var lines = await db.MortgageRequestLines.Where(l => l.OrgId == Org && l.MortgageRequestId == m.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (m.Status != "Pending") return null;
                m.Status = "Approved";
                m.ApprovedAt = now;
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles)
                {
                    v.IsMortgaged = true;
                    v.MortgageBankCode = m.BankCode;
                    v.MortgageDate = m.MortgageDate ?? now;
                    Log(v.Vin, "Mortgaged", $"{reqMortgageNo} Thế chấp ngân hàng {m.BankCode}");
                }
                break;

            case "reject":
                if (m.Status != "Pending") return null;
                m.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    m.Remark = string.IsNullOrWhiteSpace(m.Remark) ? dto.Note : $"{m.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "MortgageRejected", $"{reqMortgageNo} Từ chối thế chấp: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (m.Status is not ("Pending" or "Approved")) return null;
                var prevStatus = m.Status;
                m.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    m.Remark = string.IsNullOrWhiteSpace(m.Remark) ? dto.Note : $"{m.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                if (prevStatus == "Approved")
                {
                    foreach (var v in vehicles)
                    {
                        v.IsMortgaged = false;
                        Log(v.Vin, "MortgageCancelled", $"{reqMortgageNo} Hủy thế chấp");
                    }
                }
                else
                {
                    foreach (var v in vehicles) Log(v.Vin, "MortgageCancelled", $"{reqMortgageNo} Hủy yêu cầu thế chấp");
                }
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { m.ReqMortgageNo, m.BankCode, status = m.Status, m.ApprovedAt };
    }

    // ===== Giải chấp xe ngân hàng (BizHTC.GiaiChap.RD_ReqRedeem / RD_ReqRedeem) =====
    public async Task<object> CreateRedeemRequestAsync(CreateRedeemRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode) || string.IsNullOrWhiteSpace(dto.BankCode))
            throw new InvalidOperationException("Cần DealerCode và BankCode để tạo yêu cầu giải chấp.");

        var items = new List<RedeemItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0) items.AddRange(dto.Items);
        else if (dto.Vins != null && dto.Vins.Count > 0) items.AddRange(dto.Vins.Select(v => new RedeemItemInputDto(v)));

        if (items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 VIN trong yêu cầu giải chấp.");

        var distinctItems = items.GroupBy(i => i.Vin.Trim().ToUpperInvariant()).Select(g => g.First()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var reqNo = string.IsNullOrWhiteSpace(dto.RedeemReqNo)
            ? "RD" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.RedeemReqNo!.Trim().ToUpperInvariant();

        if (await db.RedeemRequests.AnyAsync(r => r.OrgId == Org && r.RedeemReqNo == reqNo))
            throw new InvalidOperationException($"Mã yêu cầu giải chấp {reqNo} đã tồn tại.");

        var dealer = dto.DealerCode.Trim().ToUpperInvariant();
        var bankCode = dto.BankCode.Trim().ToUpperInvariant();

        var rd = new RedeemRequest
        {
            OrgId = Org,
            RedeemReqNo = reqNo,
            DealerCode = dealer,
            BankCode = bankCode,
            ReqMortgageNo = dto.ReqMortgageNo?.Trim().ToUpperInvariant(),
            Reason = dto.Reason?.Trim(),
            Remark = dto.Remark?.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.Now
        };
        db.RedeemRequests.Add(rd);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            db.RedeemRequestLines.Add(new RedeemRequestLine
            {
                OrgId = Org,
                RedeemRequestId = rd.Id,
                RedeemReqNo = reqNo,
                Vin = vin,
                ReleaseDocType = string.IsNullOrWhiteSpace(item.ReleaseDocType) ? "All" : item.ReleaseDocType.Trim(),
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "RedeemRequested", $"{reqNo} ĐL:{dealer} Ngân hàng:{bankCode} Loại giấy tờ:{item.ReleaseDocType ?? "All"}");
        }
        await db.SaveChangesAsync();

        return new
        {
            rd.RedeemReqNo,
            rd.DealerCode,
            rd.BankCode,
            rd.ReqMortgageNo,
            rd.Reason,
            rd.Status,
            totalVins = distinctItems.Count,
            vins = distinctItems.Select(i => new { i.Vin, i.ReleaseDocType })
        };
    }

    public async Task<object> ListRedeemRequestsAsync(string? status, string? dealer, string? bank, string? vin)
    {
        var q = db.RedeemRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) { var d = dealer.Trim().ToUpperInvariant(); q = q.Where(r => r.DealerCode == d); }
        if (!string.IsNullOrWhiteSpace(bank)) { var b = bank.Trim().ToUpperInvariant(); q = q.Where(r => r.BankCode == b); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.RedeemRequestLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.RedeemReqNo).Distinct().ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.RedeemReqNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.RedeemReqNo,
            r.DealerCode,
            r.BankCode,
            r.ReqMortgageNo,
            r.Reason,
            r.Status,
            r.Remark,
            r.CreatedAt,
            r.ApprovedAt,
            r.CompletedAt,
            vinCount = db.RedeemRequestLines.Count(l => l.OrgId == Org && l.RedeemRequestId == r.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetRedeemRequestAsync(string redeemReqNo)
    {
        redeemReqNo = redeemReqNo.Trim().ToUpperInvariant();
        var rd = await db.RedeemRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.RedeemReqNo == redeemReqNo);
        if (rd is null) return null;

        var lines = await db.RedeemRequestLines.Where(l => l.OrgId == Org && l.RedeemRequestId == rd.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Vin,
            l.ReleaseDocType,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { v.Model, v.Color, v.EngineNo, status = v.Status.ToString(), v.StorageCode, v.DealerCode, v.IsMortgaged, v.MortgageBankCode, v.RedeemDate } : null
        }).ToList();

        return new
        {
            rd.RedeemReqNo,
            rd.DealerCode,
            rd.BankCode,
            rd.ReqMortgageNo,
            rd.Reason,
            rd.Status,
            rd.Remark,
            rd.CreatedAt,
            rd.ApprovedAt,
            rd.CompletedAt,
            vins = details
        };
    }

    public async Task<object?> RedeemRequestTransitionAsync(string redeemReqNo, string action, RedeemRequestTransitionDto? dto)
    {
        redeemReqNo = redeemReqNo.Trim().ToUpperInvariant();
        var rd = await db.RedeemRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.RedeemReqNo == redeemReqNo);
        if (rd is null) return null;

        var now = DateTime.Now;
        var lines = await db.RedeemRequestLines.Where(l => l.OrgId == Org && l.RedeemRequestId == rd.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (rd.Status != "Pending") return null;
                rd.Status = "Approved";
                rd.ApprovedAt = now;
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "RedeemApproved", $"{redeemReqNo} Ngân hàng {rd.BankCode} chấp thuận giải chấp");
                break;

            case "complete":
                if (rd.Status is not ("Approved" or "Pending")) return null;
                rd.Status = "Completed";
                rd.ApprovedAt ??= now;
                rd.CompletedAt = now;
                foreach (var l in lines) l.Status = "Completed";
                foreach (var v in vehicles)
                {
                    v.IsMortgaged = false;
                    v.RedeemDate = now;
                    Log(v.Vin, "Redeemed", $"{redeemReqNo} Hoàn tất giải chấp ngân hàng {rd.BankCode}. Giải phóng toàn bộ chứng từ xuất xưởng.");
                }
                break;

            case "reject":
                if (rd.Status != "Pending") return null;
                rd.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    rd.Remark = string.IsNullOrWhiteSpace(rd.Remark) ? dto.Note : $"{rd.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "RedeemRejected", $"{redeemReqNo} Từ chối giải chấp: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (rd.Status is not ("Pending" or "Approved")) return null;
                rd.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    rd.Remark = string.IsNullOrWhiteSpace(rd.Remark) ? dto.Note : $"{rd.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "RedeemCancelled", $"{redeemReqNo} Hủy yêu cầu giải chấp: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new { rd.RedeemReqNo, rd.DealerCode, rd.BankCode, status = rd.Status, rd.ApprovedAt, rd.CompletedAt };
    }

    // ===== Đơn đặt hàng xe ô tô của Đại lý (BizHTC.Order.Ord_SalesOrder / SalesOrder) =====
    public async Task<object> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần DealerCode để tạo đơn đặt hàng.");
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 dòng chi tiết xe đặt hàng.");

        foreach (var item in dto.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Model))
                throw new InvalidOperationException("Mỗi dòng đặt hàng phải có Model xe.");
            if (item.OrderQty <= 0)
                throw new InvalidOperationException("Số lượng đặt hàng OrderQty phải lớn hơn 0.");
        }

        var soCode = string.IsNullOrWhiteSpace(dto.SOCode)
            ? "SO" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.SOCode!.Trim().ToUpperInvariant();

        if (await db.SalesOrders.AnyAsync(s => s.OrgId == Org && s.SOCode == soCode))
            throw new InvalidOperationException($"Mã đơn đặt hàng {soCode} đã tồn tại.");

        var dealer = dto.DealerCode.Trim();
        var totalOrderQty = dto.Items.Sum(i => i.OrderQty);
        var totalAmount = dto.Items.Sum(i => i.OrderQty * i.UnitPrice);

        var so = new SalesOrder
        {
            OrgId = Org,
            SOCode = soCode,
            SOType = string.IsNullOrWhiteSpace(dto.SOType) ? "Normal" : dto.SOType.Trim(),
            DealerCode = dealer,
            SPCode = dto.SPCode?.Trim(),
            OrderMonth = dto.OrderMonth?.Trim() ?? DateTime.Now.ToString("yyyy-MM"),
            ProductionMonth = dto.ProductionMonth?.Trim(),
            ExpectedMonth = dto.ExpectedMonth?.Trim(),
            TotalOrderQty = totalOrderQty,
            TotalApprovedQty = 0,
            TotalAllocatedQty = 0,
            TotalAmount = totalAmount,
            Status = "Draft",
            CreatedBy = dto.CreatedBy?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.SalesOrders.Add(so);
        await db.SaveChangesAsync();

        foreach (var item in dto.Items)
        {
            var lineAmount = item.OrderQty * item.UnitPrice;
            db.SalesOrderLines.Add(new SalesOrderLine
            {
                OrgId = Org,
                SalesOrderId = so.Id,
                SOCode = soCode,
                Model = item.Model.Trim(),
                SpecCode = item.SpecCode?.Trim(),
                Color = item.Color?.Trim(),
                OrderQty = item.OrderQty,
                ApprovedQty = 0,
                AllocatedQty = 0,
                UnitPrice = item.UnitPrice,
                TotalAmount = lineAmount,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });
        }
        await db.SaveChangesAsync();

        return new
        {
            so.SOCode,
            so.DealerCode,
            so.SOType,
            so.OrderMonth,
            so.TotalOrderQty,
            so.TotalAmount,
            status = so.Status,
            linesCount = dto.Items.Count
        };
    }

    public async Task<object> ListSalesOrdersAsync(string? status, string? dealer, string? orderMonth, string? model)
    {
        var q = db.SalesOrders.Where(s => s.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(s => s.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(s => s.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(orderMonth)) q = q.Where(s => s.OrderMonth == orderMonth);
        if (!string.IsNullOrWhiteSpace(model))
        {
            var m = model.Trim();
            var matchedCodes = await db.SalesOrderLines
                .Where(l => l.OrgId == Org && l.Model.Contains(m))
                .Select(l => l.SOCode)
                .Distinct()
                .ToListAsync();
            q = q.Where(s => matchedCodes.Contains(s.SOCode));
        }

        var items = await q.OrderByDescending(s => s.Id).Take(500).Select(s => new
        {
            s.SOCode,
            s.SOType,
            s.DealerCode,
            s.SPCode,
            s.OrderMonth,
            s.ProductionMonth,
            s.ExpectedMonth,
            s.TotalOrderQty,
            s.TotalApprovedQty,
            s.TotalAllocatedQty,
            s.TotalAmount,
            s.Status,
            s.CreatedBy,
            s.ApprovedBy1,
            s.ApprovedAt1,
            s.ApprovedBy2,
            s.ApprovedAt2,
            s.CreatedAt,
            linesCount = db.SalesOrderLines.Count(l => l.OrgId == Org && l.SalesOrderId == s.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetSalesOrderAsync(string soCode)
    {
        soCode = soCode.Trim().ToUpperInvariant();
        var so = await db.SalesOrders.FirstOrDefaultAsync(s => s.OrgId == Org && s.SOCode == soCode);
        if (so is null) return null;

        var lines = await db.SalesOrderLines.Where(l => l.OrgId == Org && l.SalesOrderId == so.Id).ToListAsync();
        var allocatedVehicles = await db.Vehicles
            .Where(v => v.OrgId == Org && v.SOCode == soCode)
            .Select(v => new
            {
                v.Vin,
                v.Model,
                v.Color,
                v.EngineNo,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString()
            })
            .ToListAsync();

        var details = lines.Select(l => new
        {
            l.Id,
            l.Model,
            l.SpecCode,
            l.Color,
            l.OrderQty,
            l.ApprovedQty,
            l.AllocatedQty,
            l.UnitPrice,
            l.TotalAmount,
            l.Status,
            l.Remark
        }).ToList();

        return new
        {
            so.SOCode,
            so.SOType,
            so.DealerCode,
            so.SPCode,
            so.OrderMonth,
            so.ProductionMonth,
            so.ExpectedMonth,
            so.TotalOrderQty,
            so.TotalApprovedQty,
            so.TotalAllocatedQty,
            so.TotalAmount,
            so.Status,
            so.CreatedBy,
            so.ApprovedBy1,
            so.ApprovedAt1,
            so.ApprovedBy2,
            so.ApprovedAt2,
            so.Remark,
            so.CreatedAt,
            lines = details,
            allocatedVehicles
        };
    }

    public async Task<object?> SalesOrderTransitionAsync(string soCode, string action, SalesOrderTransitionDto? dto)
    {
        soCode = soCode.Trim().ToUpperInvariant();
        var so = await db.SalesOrders.FirstOrDefaultAsync(s => s.OrgId == Org && s.SOCode == soCode);
        if (so is null) return null;

        var now = DateTime.Now;
        var lines = await db.SalesOrderLines.Where(l => l.OrgId == Org && l.SalesOrderId == so.Id).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (so.Status != "Draft") return null;
                so.Status = "Submitted";
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Submitted";
                break;

            case "approve1":
                if (so.Status is not ("Draft" or "Submitted")) return null;
                so.Status = "Approved1";
                so.ApprovedBy1 = dto?.ApprovedBy?.Trim() ?? "PlanningDept";
                so.ApprovedAt1 = now;
                if (!string.IsNullOrWhiteSpace(dto?.ProductionMonth)) so.ProductionMonth = dto.ProductionMonth.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.ExpectedMonth)) so.ExpectedMonth = dto.ExpectedMonth.Trim();

                if (dto?.Items != null && dto.Items.Count > 0)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var line = lines.FirstOrDefault(l => (itemDto.LineId.HasValue && l.Id == itemDto.LineId.Value) || (l.Model == itemDto.Model && (itemDto.Color == null || l.Color == itemDto.Color)));
                        if (line != null)
                        {
                            if (itemDto.ApprovedQty.HasValue) line.ApprovedQty = Math.Max(0, itemDto.ApprovedQty.Value);
                            if (itemDto.UnitPrice.HasValue && itemDto.UnitPrice.Value > 0) line.UnitPrice = itemDto.UnitPrice.Value;
                            line.TotalAmount = line.ApprovedQty * line.UnitPrice;
                            line.Status = "Approved1";
                        }
                    }
                }
                else
                {
                    foreach (var l in lines)
                    {
                        if (l.ApprovedQty == 0) l.ApprovedQty = l.OrderQty;
                        l.TotalAmount = l.ApprovedQty * l.UnitPrice;
                        l.Status = "Approved1";
                    }
                }
                so.TotalApprovedQty = lines.Sum(l => l.ApprovedQty);
                so.TotalAmount = lines.Sum(l => l.TotalAmount);
                break;

            case "approve2":
            case "approve":
                if (so.Status is not ("Draft" or "Submitted" or "Approved1")) return null;
                so.Status = "Approved";
                so.ApprovedBy2 = dto?.ApprovedBy?.Trim() ?? "Director";
                so.ApprovedAt2 = now;
                if (string.IsNullOrWhiteSpace(so.ApprovedBy1))
                {
                    so.ApprovedBy1 = so.ApprovedBy2;
                    so.ApprovedAt1 = now;
                }
                if (!string.IsNullOrWhiteSpace(dto?.ProductionMonth)) so.ProductionMonth = dto.ProductionMonth.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.ExpectedMonth)) so.ExpectedMonth = dto.ExpectedMonth.Trim();

                foreach (var l in lines)
                {
                    if (l.ApprovedQty == 0) l.ApprovedQty = l.OrderQty;
                    l.TotalAmount = l.ApprovedQty * l.UnitPrice;
                    l.Status = l.AllocatedQty >= l.ApprovedQty ? "FullyAllocated" : (l.AllocatedQty > 0 ? "PartiallyAllocated" : "Approved");
                }
                so.TotalApprovedQty = lines.Sum(l => l.ApprovedQty);
                so.TotalAmount = lines.Sum(l => l.TotalAmount);
                break;

            case "reject":
                if (so.Status is "Approved" or "Cancelled") return null;
                so.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    so.Remark = string.IsNullOrWhiteSpace(so.Remark) ? dto.Note : $"{so.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                break;

            case "cancel":
                if (so.Status is "Cancelled" or "Rejected") return null;
                so.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    so.Remark = string.IsNullOrWhiteSpace(so.Remark) ? dto.Note : $"{so.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";

                // Deallocate any currently allocated vehicles
                var allocatedVehicles = await db.Vehicles.Where(v => v.OrgId == Org && v.SOCode == soCode).ToListAsync();
                foreach (var v in allocatedVehicles)
                {
                    if (v.Status != VehicleStatus.Delivered)
                    {
                        v.SOCode = null;
                        v.DealerCode = null;
                        v.Status = VehicleStatus.InStock;
                        Log(v.Vin, "DeallocatedFromSO", $"{soCode} Hủy phân bổ do hủy đơn đặt hàng SO.");
                    }
                }
                so.TotalAllocatedQty = 0;
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            so.SOCode,
            so.DealerCode,
            status = so.Status,
            so.TotalOrderQty,
            so.TotalApprovedQty,
            so.TotalAllocatedQty,
            so.TotalAmount,
            so.ApprovedAt1,
            so.ApprovedAt2
        };
    }

    public async Task<object?> AllocateSoVinAsync(string soCode, AllocateSoVinDto dto)
    {
        soCode = soCode.Trim().ToUpperInvariant();
        var so = await db.SalesOrders.FirstOrDefaultAsync(s => s.OrgId == Org && s.SOCode == soCode);
        if (so is null) return null;
        if (so.Status is not ("Approved" or "Approved1" or "Approved2"))
            throw new InvalidOperationException($"Đơn hàng {soCode} đang ở trạng thái '{so.Status}', chỉ có thể phân bổ VIN khi đơn hàng đã được phê duyệt.");

        var vin = dto.Vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null) throw new InvalidOperationException($"Không tìm thấy xe VIN {vin}.");
        if (v.Status != VehicleStatus.InStock)
            throw new InvalidOperationException($"Xe {vin} đang ở trạng thái '{v.Status}', chỉ có thể phân bổ xe trong kho (InStock).");

        var lines = await db.SalesOrderLines.Where(l => l.OrgId == Org && l.SalesOrderId == so.Id).ToListAsync();
        SalesOrderLine? targetLine = null;

        if (dto.LineId.HasValue)
        {
            targetLine = lines.FirstOrDefault(l => l.Id == dto.LineId.Value);
            if (targetLine is null)
                throw new InvalidOperationException($"Không tìm thấy dòng chi tiết đơn hàng ID {dto.LineId}.");
        }
        else
        {
            // Tự động tìm dòng khớp Model và Color (hoặc dòng khớp Model nếu dòng không chỉ định màu)
            targetLine = lines.FirstOrDefault(l =>
                l.Model.Equals(v.Model, StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(l.Color) || (v.Color != null && l.Color.Equals(v.Color, StringComparison.OrdinalIgnoreCase))) &&
                l.AllocatedQty < (l.ApprovedQty > 0 ? l.ApprovedQty : l.OrderQty)
            );

            targetLine ??= lines.FirstOrDefault(l =>
                l.Model.Equals(v.Model, StringComparison.OrdinalIgnoreCase) &&
                l.AllocatedQty < (l.ApprovedQty > 0 ? l.ApprovedQty : l.OrderQty)
            );
        }

        if (targetLine is null)
            throw new InvalidOperationException($"Không tìm thấy dòng đặt hàng phù hợp với xe {v.Model} ({v.Color ?? "N/A"}) hoặc dòng đã phân bổ đủ số lượng.");

        // Phân bổ xe
        v.DealerCode = so.DealerCode;
        v.Status = VehicleStatus.Allocated;
        v.SOCode = so.SOCode;

        targetLine.AllocatedQty++;
        var targetCapacity = targetLine.ApprovedQty > 0 ? targetLine.ApprovedQty : targetLine.OrderQty;
        targetLine.Status = targetLine.AllocatedQty >= targetCapacity ? "FullyAllocated" : "PartiallyAllocated";

        so.TotalAllocatedQty = lines.Sum(l => l.AllocatedQty);

        Log(v.Vin, "AllocatedBySO", $"{soCode} Phân bổ cho ĐL {so.DealerCode} theo đơn hàng SO (Dòng: {targetLine.Model}{(string.IsNullOrEmpty(targetLine.SpecCode) ? "" : $" - {targetLine.SpecCode}")}). {dto.Remark ?? ""}".Trim());

        await db.SaveChangesAsync();

        return new
        {
            so.SOCode,
            v.Vin,
            v.Model,
            v.Color,
            v.DealerCode,
            status = v.Status.ToString(),
            lineId = targetLine.Id,
            lineAllocatedQty = targetLine.AllocatedQty,
            lineApprovedQty = targetLine.ApprovedQty,
            soTotalAllocated = so.TotalAllocatedQty,
            soTotalApproved = so.TotalApprovedQty
        };
    }

    public async Task<object?> DeallocateSoVinAsync(string soCode, string vin)
    {
        soCode = soCode.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var so = await db.SalesOrders.FirstOrDefaultAsync(s => s.OrgId == Org && s.SOCode == soCode);
        if (so is null) return null;

        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null) throw new InvalidOperationException($"Không tìm thấy xe VIN {vin}.");
        if (v.SOCode != soCode)
            throw new InvalidOperationException($"Xe {vin} không thuộc đơn hàng {soCode}.");
        if (v.Status == VehicleStatus.Delivered)
            throw new InvalidOperationException($"Xe {vin} đã giao cho khách hàng cuối (Delivered), không thể hủy phân bổ.");

        var lines = await db.SalesOrderLines.Where(l => l.OrgId == Org && l.SalesOrderId == so.Id).ToListAsync();
        var targetLine = lines.FirstOrDefault(l => l.Model.Equals(v.Model, StringComparison.OrdinalIgnoreCase) && l.AllocatedQty > 0)
            ?? lines.FirstOrDefault(l => l.AllocatedQty > 0);

        if (targetLine != null)
        {
            targetLine.AllocatedQty = Math.Max(0, targetLine.AllocatedQty - 1);
            var targetCapacity = targetLine.ApprovedQty > 0 ? targetLine.ApprovedQty : targetLine.OrderQty;
            targetLine.Status = targetLine.AllocatedQty == 0 ? "Approved" : (targetLine.AllocatedQty >= targetCapacity ? "FullyAllocated" : "PartiallyAllocated");
        }

        v.SOCode = null;
        v.DealerCode = null;
        v.Status = VehicleStatus.InStock;

        so.TotalAllocatedQty = lines.Sum(l => l.AllocatedQty);

        Log(v.Vin, "DeallocatedFromSO", $"{soCode} Hủy phân bổ khỏi đơn đặt hàng SO.");

        await db.SaveChangesAsync();

        return new
        {
            so.SOCode,
            v.Vin,
            status = v.Status.ToString(),
            soTotalAllocated = so.TotalAllocatedQty,
            soTotalApproved = so.TotalApprovedQty
        };
    }

    // ===== Giao dịch bán lẻ ô tô của Đại lý cho Khách hàng & Kích hoạt Sổ Bảo Hành Online (BizHTC.DealerSales / DLS_Deal / DLS_DealDetail) =====
    public async Task<object> CreateDealerDealAsync(CreateDealerDealDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần DealerCode để tạo giao dịch bán lẻ Deal.");
        if (string.IsNullOrWhiteSpace(dto.CustomerName) || string.IsNullOrWhiteSpace(dto.CustomerPhone))
            throw new InvalidOperationException("Cần tên khách hàng CustomerName và số điện thoại CustomerPhone.");
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe VIN trong giao dịch bán lẻ Deal.");

        var distinctItems = dto.Items.GroupBy(i => i.Vin.Trim().ToUpperInvariant()).Select(g => g.First()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var deliveredVins = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (deliveredVins.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng (Delivered) không thể tạo deal mới: " + string.Join(", ", deliveredVins));

        // Kiểm tra xe đang trong deal khác chưa hoàn tất
        var activeLines = await db.DealerDealLines
            .Where(l => l.OrgId == Org && vins.Contains(l.Vin) && (l.Status == "Pending" || l.Status == "Submitted" || l.Status == "Approved"))
            .ToListAsync();
        if (activeLines.Count > 0)
        {
            var conflict = activeLines.First();
            throw new InvalidOperationException($"VIN {conflict.Vin} đang nằm trong giao dịch bán lẻ khác chưa kết thúc ({conflict.DealNo}).");
        }

        var dealNo = string.IsNullOrWhiteSpace(dto.DealNo)
            ? "DEAL" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.DealNo!.Trim().ToUpperInvariant();

        if (await db.DealerDeals.AnyAsync(d => d.OrgId == Org && d.DealNo == dealNo))
            throw new InvalidOperationException($"Mã giao dịch {dealNo} đã tồn tại.");

        var dealer = dto.DealerCode.Trim();
        var vMap = vehicles.ToDictionary(v => v.Vin);

        decimal totalAmount = 0;
        decimal totalDiscount = 0;

        foreach (var item in distinctItems)
        {
            var v = vMap[item.Vin.Trim().ToUpperInvariant()];
            var unitPrice = item.UnitPrice ?? 0;
            var discount = item.Discount ?? 0;
            totalAmount += unitPrice;
            totalDiscount += discount;
        }

        var finalAmount = Math.Max(0, totalAmount - totalDiscount);

        var deal = new DealerDeal
        {
            OrgId = Org,
            DealNo = dealNo,
            DealNoUser = dto.DealNoUser?.Trim(),
            DealerCode = dealer,
            CustomerCode = dto.CustomerCode?.Trim(),
            CustomerName = dto.CustomerName.Trim(),
            CustomerPhone = dto.CustomerPhone.Trim(),
            CustomerType = string.IsNullOrWhiteSpace(dto.CustomerType) ? "Individual" : dto.CustomerType.Trim(),
            IdNo = dto.IdNo?.Trim(),
            Address = dto.Address?.Trim(),
            SalesManCode = dto.SalesManCode?.Trim(),
            SalesManName = dto.SalesManName?.Trim(),
            SalesType = string.IsNullOrWhiteSpace(dto.SalesType) ? "Retail" : dto.SalesType.Trim(),
            PaymentType = string.IsNullOrWhiteSpace(dto.PaymentType) ? "Cash" : dto.PaymentType.Trim(),
            BankCode = dto.BankCode?.Trim()?.ToUpperInvariant(),
            LoanAmount = dto.LoanAmount,
            DepositAmount = dto.DepositAmount,
            TotalAmount = totalAmount,
            DiscountAmount = totalDiscount,
            FinalAmount = finalAmount,
            DealDate = dto.DealDate ?? DateTime.Now,
            Status = "Draft",
            CreatedBy = dto.CreatedBy?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };

        db.DealerDeals.Add(deal);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var v = vMap[vin];
            var unitPrice = item.UnitPrice ?? 0;
            var discount = item.Discount ?? 0;
            var price = Math.Max(0, unitPrice - discount);

            db.DealerDealLines.Add(new DealerDealLine
            {
                OrgId = Org,
                DealerDealId = deal.Id,
                DealNo = dealNo,
                Vin = vin,
                Model = v.Model,
                Color = v.Color,
                UnitPrice = unitPrice,
                Discount = discount,
                Price = price,
                PlateNo = item.PlateNo?.Trim(),
                SBHOnlineNo = item.SBHOnlineNo?.Trim(),
                DeliveryOdoKm = item.DeliveryOdoKm ?? 10,
                WarrantyMonths = item.WarrantyMonths ?? v.WarrantyMonths,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "DealerDealCreated", $"{dealNo} ĐL:{dealer} KH:{deal.CustomerName} ({deal.CustomerPhone}) Giá bán:{price:N0}đ");
        }

        await db.SaveChangesAsync();

        return new
        {
            deal.DealNo,
            deal.DealNoUser,
            deal.DealerCode,
            deal.CustomerName,
            deal.CustomerPhone,
            deal.SalesType,
            deal.PaymentType,
            deal.TotalAmount,
            deal.DiscountAmount,
            deal.FinalAmount,
            deal.Status,
            vinsCount = distinctItems.Count
        };
    }

    public async Task<object> ListDealerDealsAsync(string? status, string? dealer, string? customer, string? salesMan, string? paymentType, string? vin)
    {
        var q = db.DealerDeals.Where(d => d.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(d => d.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) q = q.Where(d => d.DealerCode == dealer);
        if (!string.IsNullOrWhiteSpace(customer))
        {
            var c = customer.Trim().ToLowerInvariant();
            q = q.Where(d => d.CustomerName.ToLower().Contains(c) || d.CustomerPhone.Contains(c));
        }
        if (!string.IsNullOrWhiteSpace(salesMan))
        {
            var sm = salesMan.Trim().ToLowerInvariant();
            q = q.Where(d => (d.SalesManCode != null && d.SalesManCode.ToLower().Contains(sm)) || (d.SalesManName != null && d.SalesManName.ToLower().Contains(sm)));
        }
        if (!string.IsNullOrWhiteSpace(paymentType)) q = q.Where(d => d.PaymentType == paymentType);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.DealerDealLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.DealNo).Distinct().ToListAsync();
            q = q.Where(d => matchedNos.Contains(d.DealNo));
        }

        var items = await q.OrderByDescending(d => d.Id).Take(500).Select(d => new
        {
            d.DealNo,
            d.DealNoUser,
            d.DealerCode,
            d.CustomerName,
            d.CustomerPhone,
            d.CustomerType,
            d.SalesManName,
            d.SalesType,
            d.PaymentType,
            d.BankCode,
            d.TotalAmount,
            d.DiscountAmount,
            d.FinalAmount,
            d.DepositAmount,
            d.DealDate,
            d.Status,
            d.CreatedAt,
            d.ApprovedAt,
            d.DeliveredAt,
            vinCount = db.DealerDealLines.Count(l => l.OrgId == Org && l.DealerDealId == d.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetDealerDealAsync(string dealNo)
    {
        dealNo = dealNo.Trim().ToUpperInvariant();
        var deal = await db.DealerDeals.FirstOrDefaultAsync(d => d.OrgId == Org && d.DealNo == dealNo);
        if (deal is null) return null;

        var lines = await db.DealerDealLines.Where(l => l.OrgId == Org && l.DealerDealId == deal.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.Color,
            l.UnitPrice,
            l.Discount,
            l.Price,
            l.PlateNo,
            l.SBHOnlineNo,
            l.DeliveryOdoKm,
            l.WarrantyStartDate,
            l.WarrantyMonths,
            l.DeliveryDate,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString(),
                v.OwnerName,
                v.OwnerPhone,
                v.PlateNo,
                v.WarrantyStart,
                v.WarrantyEnd
            } : null
        }).ToList();

        return new
        {
            deal.DealNo,
            deal.DealNoUser,
            deal.DealerCode,
            deal.CustomerCode,
            deal.CustomerName,
            deal.CustomerPhone,
            deal.CustomerType,
            deal.IdNo,
            deal.Address,
            deal.SalesManCode,
            deal.SalesManName,
            deal.SalesType,
            deal.PaymentType,
            deal.BankCode,
            deal.LoanAmount,
            deal.DepositAmount,
            deal.TotalAmount,
            deal.DiscountAmount,
            deal.FinalAmount,
            deal.DealDate,
            deal.Status,
            deal.CreatedBy,
            deal.ApprovedBy,
            deal.Remark,
            deal.CreatedAt,
            deal.ApprovedAt,
            deal.DeliveredAt,
            lines = details
        };
    }

    public async Task<object?> DealerDealTransitionAsync(string dealNo, string action, DealerDealTransitionDto? dto)
    {
        dealNo = dealNo.Trim().ToUpperInvariant();
        var deal = await db.DealerDeals.FirstOrDefaultAsync(d => d.OrgId == Org && d.DealNo == dealNo);
        if (deal is null) return null;

        var now = DateTime.Now;
        var lines = await db.DealerDealLines.Where(l => l.OrgId == Org && l.DealerDealId == deal.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (deal.Status != "Draft") return null;
                deal.Status = "Submitted";
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Submitted";
                foreach (var v in vehicles) Log(v.Vin, "DealSubmitted", $"{dealNo} Giao dịch bán lẻ đã nộp chờ duyệt.");
                break;

            case "approve":
                if (deal.Status is not ("Draft" or "Submitted")) return null;
                deal.Status = "Approved";
                deal.ApprovedBy = dto?.ApprovedBy?.Trim() ?? "SalesManager";
                deal.ApprovedAt = now;
                foreach (var l in lines) if (l.Status is "Pending" or "Submitted") l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "DealApproved", $"{dealNo} Giao dịch bán lẻ được phê duyệt bởi {deal.ApprovedBy}.");
                break;

            case "deliver":
                if (deal.Status is not ("Approved" or "Submitted")) return null;
                deal.Status = "Delivered";
                deal.DeliveredAt = dto?.DeliveryDate ?? now;

                foreach (var line in lines)
                {
                    line.Status = "Delivered";
                    line.DeliveryDate = dto?.DeliveryDate ?? line.DeliveryDate ?? now;
                    line.WarrantyStartDate ??= line.DeliveryDate;

                    if (string.IsNullOrWhiteSpace(line.SBHOnlineNo))
                    {
                        var shortVin = line.Vin.Length >= 6 ? line.Vin[^6..] : line.Vin;
                        line.SBHOnlineNo = $"SBH-{now:yyyyMMdd}-{shortVin}";
                    }

                    var vehicle = vehicles.FirstOrDefault(v => v.Vin == line.Vin);
                    if (vehicle != null)
                    {
                        vehicle.Status = VehicleStatus.Delivered;
                        vehicle.DealerCode = deal.DealerCode;
                        vehicle.OwnerName = deal.CustomerName;
                        vehicle.OwnerPhone = deal.CustomerPhone;
                        if (!string.IsNullOrWhiteSpace(line.PlateNo)) vehicle.PlateNo = line.PlateNo;
                        vehicle.DeliveredAt = line.DeliveryDate;
                        vehicle.WarrantyStart = line.WarrantyStartDate;
                        vehicle.WarrantyMonths = line.WarrantyMonths > 0 ? line.WarrantyMonths : (vehicle.WarrantyMonths > 0 ? vehicle.WarrantyMonths : 36);
                        vehicle.WarrantyEnd = vehicle.WarrantyStart.Value.AddMonths(vehicle.WarrantyMonths);

                        Log(vehicle.Vin, "DealDelivered", $"{dealNo} Bàn giao xe bán lẻ cho KH {deal.CustomerName} ({deal.CustomerPhone}). Biển số: {vehicle.PlateNo ?? "Chưa đăng ký"}. Sổ BH: {line.SBHOnlineNo}. BH đến: {vehicle.WarrantyEnd:yyyy-MM-dd}");
                    }
                }
                break;

            case "reject":
                if (deal.Status is "Delivered" or "Cancelled") return null;
                deal.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    deal.Remark = string.IsNullOrWhiteSpace(deal.Remark) ? dto.Note : $"{deal.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "DealRejected", $"{dealNo} Từ chối giao dịch: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (deal.Status is "Cancelled" or "Delivered") return null;
                deal.Status = "Cancelled";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    deal.Remark = string.IsNullOrWhiteSpace(deal.Remark) ? dto.Note : $"{deal.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "DealCancelled", $"{dealNo} Hủy giao dịch: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            deal.DealNo,
            deal.DealerCode,
            status = deal.Status,
            deal.ApprovedAt,
            deal.DeliveredAt
        };
    }

    public async Task<object?> UpdateDealerDealLineAsync(string dealNo, string vin, UpdateDealerDealLineDto dto)
    {
        dealNo = dealNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var deal = await db.DealerDeals.FirstOrDefaultAsync(d => d.OrgId == Org && d.DealNo == dealNo);
        if (deal is null) return null;

        var line = await db.DealerDealLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.DealerDealId == deal.Id && l.Vin == vin);
        if (line is null) return null;

        if (!string.IsNullOrWhiteSpace(dto.PlateNo)) line.PlateNo = dto.PlateNo.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(dto.SBHOnlineNo)) line.SBHOnlineNo = dto.SBHOnlineNo.Trim().ToUpperInvariant();
        if (dto.DeliveryOdoKm.HasValue && dto.DeliveryOdoKm.Value >= 0) line.DeliveryOdoKm = dto.DeliveryOdoKm.Value;
        if (dto.WarrantyStartDate.HasValue) line.WarrantyStartDate = dto.WarrantyStartDate.Value;
        if (dto.WarrantyMonths.HasValue && dto.WarrantyMonths.Value > 0) line.WarrantyMonths = dto.WarrantyMonths.Value;
        if (dto.DeliveryDate.HasValue) line.DeliveryDate = dto.DeliveryDate.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark))
            line.Remark = string.IsNullOrWhiteSpace(line.Remark) ? dto.Remark : $"{line.Remark} | {dto.Remark}";

        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == Org && v.Vin == vin);
        if (vehicle != null && (deal.Status == "Delivered" || line.Status == "Delivered"))
        {
            if (!string.IsNullOrWhiteSpace(line.PlateNo)) vehicle.PlateNo = line.PlateNo;
            if (line.WarrantyStartDate.HasValue)
            {
                vehicle.WarrantyStart = line.WarrantyStartDate.Value;
                vehicle.WarrantyMonths = line.WarrantyMonths;
                vehicle.WarrantyEnd = vehicle.WarrantyStart.Value.AddMonths(vehicle.WarrantyMonths);
            }
            if (line.DeliveryDate.HasValue) vehicle.DeliveredAt = line.DeliveryDate.Value;
            Log(vin, "DealLineUpdated", $"{dealNo} Cập nhật thông tin giao xe: Biển số={line.PlateNo ?? "N/A"}, SBH={line.SBHOnlineNo ?? "N/A"}, ODO={line.DeliveryOdoKm}km");
        }

        await db.SaveChangesAsync();

        return new
        {
            deal.DealNo,
            line.Vin,
            line.Model,
            line.PlateNo,
            line.SBHOnlineNo,
            line.DeliveryOdoKm,
            line.WarrantyStartDate,
            line.WarrantyMonths,
            line.DeliveryDate,
            line.Status,
            line.Remark
        };
    }

    // ===== Bảo lãnh thanh toán mua xe ô tô của Ngân hàng cho Đại lý (BizHTC.Payment / Pmt_Guarantee / Pmt_GuaranteeDetail) =====
    public async Task<object> CreatePaymentGuaranteeAsync(CreatePaymentGuaranteeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BankGuaranteeNo))
            throw new InvalidOperationException("Cần số chứng thư thư bảo lãnh ngân hàng (BankGuaranteeNo).");
        if (string.IsNullOrWhiteSpace(dto.BankCode))
            throw new InvalidOperationException("Cần mã Ngân hàng bảo lãnh (BankCode).");
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần mã Đại lý được bảo lãnh (DealerCode).");
        if (dto.TotalAmount <= 0)
            throw new InvalidOperationException("Tổng hạn mức bảo lãnh TotalAmount phải lớn hơn 0.");
        if (dto.DateExpired <= dto.DateOpen)
            throw new InvalidOperationException("Ngày hết hạn bảo lãnh DateExpired phải sau ngày mở DateOpen.");

        var distinctItems = new List<GuaranteeItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0)
        {
            distinctItems = dto.Items.GroupBy(i => i.Vin.Trim().ToUpperInvariant()).Select(g => g.First()).ToList();
        }
        else if (dto.Vins != null && dto.Vins.Count > 0)
        {
            var unitVal = dto.TotalAmount / dto.Vins.Count;
            distinctItems = dto.Vins.Select(v => v.Trim().ToUpperInvariant()).Distinct()
                .Select(v => new GuaranteeItemInputDto(v, unitVal, 100, dto.DateOpen, null)).ToList();
        }

        if (distinctItems.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe VIN trong chứng thư bảo lãnh.");

        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        // Kiểm tra VIN đang thuộc bảo lãnh khác chưa quyết toán / chưa hủy
        var activeLines = await db.GuaranteeLines
            .Where(l => l.OrgId == Org && vins.Contains(l.Vin) && (l.Status == "Pending" || l.Status == "Approved"))
            .ToListAsync();
        if (activeLines.Count > 0)
        {
            var conflict = activeLines.First();
            throw new InvalidOperationException($"VIN {conflict.Vin} đang nằm trong chứng thư bảo lãnh khác chưa tất toán ({conflict.GuaranteeNo}).");
        }

        var grtNo = string.IsNullOrWhiteSpace(dto.GuaranteeNo)
            ? "GRT" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.GuaranteeNo!.Trim().ToUpperInvariant();

        if (await db.Guarantees.AnyAsync(g => g.OrgId == Org && g.GuaranteeNo == grtNo))
            throw new InvalidOperationException($"Mã chứng thư bảo lãnh {grtNo} đã tồn tại.");

        var bankCode = dto.BankCode.Trim().ToUpperInvariant();
        var bankName = !string.IsNullOrWhiteSpace(dto.BankName) ? dto.BankName.Trim() : bankCode switch
        {
            "VCB" => "Ngân hàng Ngoại thương Việt Nam (Vietcombank)",
            "VPB" => "Ngân hàng TMCP Việt Nam Thịnh Vượng (VPBank)",
            "TCB" => "Ngân hàng Kỹ thương Việt Nam (Techcombank)",
            "BIDV" => "Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)",
            "CTG" => "Ngân hàng Công thương Việt Nam (VietinBank)",
            "MB" => "Ngân hàng Quân đội (MB Bank)",
            _ => $"Ngân hàng {bankCode}"
        };

        var term = dto.Term > 0 ? dto.Term : 30;
        var termActual = dto.TermActual > 0 ? dto.TermActual : term;

        var grt = new PaymentGuarantee
        {
            OrgId = Org,
            GuaranteeNo = grtNo,
            BankGuaranteeNo = dto.BankGuaranteeNo.Trim().ToUpperInvariant(),
            BankCode = bankCode,
            BankName = bankName,
            DealerCode = dto.DealerCode.Trim().ToUpperInvariant(),
            DateOpen = dto.DateOpen,
            DateExpired = dto.DateExpired,
            Term = term,
            TermActual = termActual,
            TotalAmount = dto.TotalAmount,
            TotalVehicleCount = distinctItems.Count,
            Status = "Pending",
            Remark = dto.Remark?.Trim(),
            CreatedBy = dto.CreatedBy?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.Guarantees.Add(grt);
        await db.SaveChangesAsync();

        var vMap = vehicles.ToDictionary(v => v.Vin);
        var dateWarning = dto.DateExpired.AddDays(-7); // cảnh báo trước 7 ngày

        foreach (var item in distinctItems)
        {
            var v = vMap[item.Vin.Trim().ToUpperInvariant()];
            var val = item.GuaranteeValue.HasValue && item.GuaranteeValue.Value > 0
                ? item.GuaranteeValue.Value
                : (dto.TotalAmount / distinctItems.Count);

            db.GuaranteeLines.Add(new PaymentGuaranteeLine
            {
                OrgId = Org,
                PaymentGuaranteeId = grt.Id,
                GuaranteeNo = grt.GuaranteeNo,
                Vin = v.Vin,
                Model = v.Model,
                GuaranteePercent = item.GuaranteePercent > 0 ? item.GuaranteePercent : 100,
                GuaranteeValue = val,
                DateStart = item.DateStart ?? dto.DateOpen,
                DateWarning = dateWarning,
                DateExpired = dto.DateExpired,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(v.Vin, "GuaranteeCreated", $"{grtNo} Tiếp nhận chứng thư bảo lãnh NH {bankCode} ({grt.BankGuaranteeNo}) cho ĐL {grt.DealerCode}. Hạn mức xe: {val:N0} VNĐ");
        }

        await db.SaveChangesAsync();

        return new
        {
            grt.GuaranteeNo,
            grt.BankGuaranteeNo,
            grt.BankCode,
            grt.BankName,
            grt.DealerCode,
            grt.DateOpen,
            grt.DateExpired,
            grt.TotalAmount,
            grt.Status,
            totalVehicles = distinctItems.Count,
            vins = distinctItems.Select(i => i.Vin)
        };
    }

    public async Task<object> ListPaymentGuaranteesAsync(string? status, string? dealer, string? bank, string? vin)
    {
        var q = db.Guarantees.Where(g => g.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(g => g.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) { var d = dealer.Trim().ToUpperInvariant(); q = q.Where(g => g.DealerCode == d); }
        if (!string.IsNullOrWhiteSpace(bank)) { var b = bank.Trim().ToUpperInvariant(); q = q.Where(g => g.BankCode == b); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.GuaranteeLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.GuaranteeNo).Distinct().ToListAsync();
            q = q.Where(g => matchedNos.Contains(g.GuaranteeNo));
        }

        var today = DateTime.Today;
        var items = await q.OrderByDescending(g => g.Id).Take(500).Select(g => new
        {
            g.GuaranteeNo,
            g.BankGuaranteeNo,
            g.BankCode,
            g.BankName,
            g.DealerCode,
            g.DateOpen,
            g.DateExpired,
            g.Term,
            g.TermActual,
            g.TotalAmount,
            g.TotalVehicleCount,
            g.Status,
            g.Remark,
            g.CreatedBy,
            g.ApprovedBy,
            g.CreatedAt,
            g.ApprovedAt,
            g.SettledAt,
            daysLeft = (int)(g.DateExpired.Date - today).TotalDays,
            isExpiringSoon = g.Status == "Approved" && (g.DateExpired.Date - today).TotalDays <= 7 && (g.DateExpired.Date - today).TotalDays >= 0,
            isExpired = g.Status == "Approved" && g.DateExpired.Date < today,
            vinCount = db.GuaranteeLines.Count(l => l.OrgId == Org && l.PaymentGuaranteeId == g.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetPaymentGuaranteeAsync(string guaranteeNo)
    {
        guaranteeNo = guaranteeNo.Trim().ToUpperInvariant();
        var grt = await db.Guarantees.FirstOrDefaultAsync(g => g.OrgId == Org && g.GuaranteeNo == guaranteeNo);
        if (grt is null) return null;

        var lines = await db.GuaranteeLines.Where(l => l.OrgId == Org && l.PaymentGuaranteeId == grt.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var today = DateTime.Today;
        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.GuaranteePercent,
            l.GuaranteeValue,
            l.DateStart,
            l.DateWarning,
            l.DateExpired,
            l.Status,
            l.Remark,
            daysLeft = l.DateExpired.HasValue ? (int)(l.DateExpired.Value.Date - today).TotalDays : 0,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.Color,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString(),
                v.OwnerName,
                v.PlateNo,
                v.DeliveredAt,
                v.IsMortgaged
            } : null
        }).ToList();

        return new
        {
            grt.GuaranteeNo,
            grt.BankGuaranteeNo,
            grt.BankCode,
            grt.BankName,
            grt.DealerCode,
            grt.DateOpen,
            grt.DateExpired,
            grt.Term,
            grt.TermActual,
            grt.TotalAmount,
            grt.TotalVehicleCount,
            grt.Status,
            grt.Remark,
            grt.CreatedBy,
            grt.ApprovedBy,
            grt.CreatedAt,
            grt.ApprovedAt,
            grt.SettledAt,
            grt.CancelledAt,
            daysLeft = (int)(grt.DateExpired.Date - today).TotalDays,
            isExpiringSoon = grt.Status == "Approved" && (grt.DateExpired.Date - today).TotalDays <= 7 && (grt.DateExpired.Date - today).TotalDays >= 0,
            lines = details
        };
    }

    public async Task<object?> PaymentGuaranteeTransitionAsync(string guaranteeNo, string action, PaymentGuaranteeTransitionDto? dto)
    {
        guaranteeNo = guaranteeNo.Trim().ToUpperInvariant();
        var grt = await db.Guarantees.FirstOrDefaultAsync(g => g.OrgId == Org && g.GuaranteeNo == guaranteeNo);
        if (grt is null) return null;

        var now = DateTime.Now;
        var lines = await db.GuaranteeLines.Where(l => l.OrgId == Org && l.PaymentGuaranteeId == grt.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (grt.Status != "Pending") return null;
                grt.Status = "Approved";
                grt.ApprovedBy = dto?.ApprovedBy?.Trim() ?? "AccountantManager";
                grt.ApprovedAt = now;
                if (dto?.DateExpired.HasValue == true) grt.DateExpired = dto.DateExpired.Value;
                if (dto?.Term.HasValue == true && dto.Term.Value > 0) grt.Term = dto.Term.Value;
                if (dto?.TermActual.HasValue == true && dto.TermActual.Value > 0) grt.TermActual = dto.TermActual.Value;

                var dateWarning = grt.DateExpired.AddDays(-7);
                foreach (var l in lines)
                {
                    if (l.Status == "Pending")
                    {
                        l.Status = "Approved";
                        l.DateStart ??= now;
                        l.DateWarning = dateWarning;
                        l.DateExpired = grt.DateExpired;
                    }
                }
                foreach (var v in vehicles)
                {
                    Log(v.Vin, "GuaranteeApproved", $"{guaranteeNo} Phê duyệt bảo lãnh thanh toán NH {grt.BankCode} ({grt.BankGuaranteeNo}). Người duyệt: {grt.ApprovedBy}. Hạn bảo lãnh: {grt.DateExpired:yyyy-MM-dd}");
                }
                break;

            case "settle":
                if (grt.Status != "Approved") return null;
                grt.Status = "Settled";
                grt.SettledAt = now;
                foreach (var l in lines)
                {
                    if (l.Status == "Approved") l.Status = "Settled";
                }
                foreach (var v in vehicles)
                {
                    Log(v.Vin, "GuaranteeSettled", $"{guaranteeNo} Tất toán hoàn tất nghĩa vụ bảo lãnh thanh toán NH {grt.BankCode}. Giải phóng hạn mức bảo lãnh.");
                }
                break;

            case "reject":
                if (grt.Status != "Pending") return null;
                grt.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    grt.Remark = string.IsNullOrWhiteSpace(grt.Remark) ? dto.Note : $"{grt.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "GuaranteeRejected", $"{guaranteeNo} Từ chối thư bảo lãnh: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (grt.Status is not ("Pending" or "Approved")) return null;
                grt.Status = "Cancelled";
                grt.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    grt.Remark = string.IsNullOrWhiteSpace(grt.Remark) ? dto.Note : $"{grt.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) if (l.Status != "Settled") l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "GuaranteeCancelled", $"{guaranteeNo} Hủy bảo lãnh thanh toán: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            grt.GuaranteeNo,
            grt.BankGuaranteeNo,
            grt.DealerCode,
            status = grt.Status,
            grt.ApprovedAt,
            grt.SettledAt,
            grt.CancelledAt
        };
    }

    public async Task<object?> CancelPaymentGuaranteeLineAsync(string guaranteeNo, string vin, string? reason)
    {
        guaranteeNo = guaranteeNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var grt = await db.Guarantees.FirstOrDefaultAsync(g => g.OrgId == Org && g.GuaranteeNo == guaranteeNo);
        if (grt is null || grt.Status is "Cancelled" or "Rejected") return null;

        var line = await db.GuaranteeLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.PaymentGuaranteeId == grt.Id && l.Vin == vin);
        if (line is null) return null;

        line.Status = "Cancelled";
        if (!string.IsNullOrWhiteSpace(reason))
            line.Remark = string.IsNullOrWhiteSpace(line.Remark) ? reason : $"{line.Remark} | Hủy: {reason}";

        Log(vin, "GuaranteeLineCancelled", $"{guaranteeNo} Hủy / giải tỏa bảo lãnh cho xe VIN {vin}. Lý do: {reason ?? "Thanh toán trực tiếp / thay thế"}");

        // Nếu tất cả các dòng đều đã Settled hoặc Cancelled, cập nhật trạng thái master
        var allLines = await db.GuaranteeLines.Where(l => l.OrgId == Org && l.PaymentGuaranteeId == grt.Id).ToListAsync();
        if (allLines.All(l => l.Status == "Settled"))
        {
            grt.Status = "Settled";
            grt.SettledAt = DateTime.Now;
        }
        else if (allLines.All(l => l.Status == "Cancelled"))
        {
            grt.Status = "Cancelled";
            grt.CancelledAt = DateTime.Now;
        }

        await db.SaveChangesAsync();

        return new
        {
            grt.GuaranteeNo,
            line.Vin,
            line.Status,
            line.GuaranteeValue,
            masterStatus = grt.Status
        };
    }

    public async Task<object?> UpdatePaymentGuaranteeAsync(string guaranteeNo, UpdatePaymentGuaranteeDto dto)
    {
        guaranteeNo = guaranteeNo.Trim().ToUpperInvariant();
        var grt = await db.Guarantees.FirstOrDefaultAsync(g => g.OrgId == Org && g.GuaranteeNo == guaranteeNo);
        if (grt is null || grt.Status is "Cancelled" or "Settled") return null;

        if (!string.IsNullOrWhiteSpace(dto.BankGuaranteeNo)) grt.BankGuaranteeNo = dto.BankGuaranteeNo.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(dto.BankName)) grt.BankName = dto.BankName.Trim();
        if (dto.DateExpired.HasValue && dto.DateExpired.Value > grt.DateOpen)
        {
            grt.DateExpired = dto.DateExpired.Value;
            var lines = await db.GuaranteeLines.Where(l => l.OrgId == Org && l.PaymentGuaranteeId == grt.Id).ToListAsync();
            var dateWarning = grt.DateExpired.AddDays(-7);
            foreach (var l in lines)
            {
                l.DateExpired = grt.DateExpired;
                l.DateWarning = dateWarning;
            }
        }
        if (dto.Term.HasValue && dto.Term.Value > 0) grt.Term = dto.Term.Value;
        if (dto.TermActual.HasValue && dto.TermActual.Value > 0) grt.TermActual = dto.TermActual.Value;
        if (dto.TotalAmount.HasValue && dto.TotalAmount.Value > 0) grt.TotalAmount = dto.TotalAmount.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark))
            grt.Remark = string.IsNullOrWhiteSpace(grt.Remark) ? dto.Remark : $"{grt.Remark} | {dto.Remark}";

        await db.SaveChangesAsync();

        return new
        {
            grt.GuaranteeNo,
            grt.BankGuaranteeNo,
            grt.BankName,
            grt.DateExpired,
            grt.Term,
            grt.TermActual,
            grt.TotalAmount,
            grt.Status,
            grt.Remark
        };
    }

    // ===== Hợp đồng mua bán xe ô tô giữa Hãng OEM và Đại lý phân phối (BizHTC.Contract.DealerContract / CT_DealerContract / CT_DealerContractDetail) =====
    public async Task<object> CreateDealerContractAsync(CreateDealerContractDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần mã Đại lý (DealerCode) để lập hợp đồng mua bán.");
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe (VIN) trong hợp đồng mua bán đại lý.");

        var distinctItems = dto.Items
            .Where(i => !string.IsNullOrWhiteSpace(i.Vin))
            .GroupBy(i => i.Vin.Trim().ToUpperInvariant())
            .Select(g => g.First())
            .ToList();

        if (distinctItems.Count == 0)
            throw new InvalidOperationException("Danh sách xe VIN hợp lệ không được rỗng.");

        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var deliveredVins = vehicles.Where(v => v.Status == VehicleStatus.Delivered).Select(v => v.Vin).ToList();
        if (deliveredVins.Count > 0)
            throw new InvalidOperationException("Xe đã giao cho khách hàng cuối (Delivered) không thể đưa vào hợp đồng mua bán đại lý: " + string.Join(", ", deliveredVins));

        var dealer = dto.DealerCode.Trim().ToUpperInvariant();
        var wrongDealerVins = vehicles.Where(v => !string.IsNullOrWhiteSpace(v.DealerCode) && !v.DealerCode.Equals(dealer, StringComparison.OrdinalIgnoreCase)).Select(v => $"{v.Vin} (thuộc {v.DealerCode})").ToList();
        if (wrongDealerVins.Count > 0)
            throw new InvalidOperationException($"Xe đã phân bổ cho đại lý khác, không thể đưa vào hợp đồng của {dealer}: " + string.Join(", ", wrongDealerVins));

        // Kiểm tra xe đang nằm trong hợp đồng mua bán khác chưa kết thúc
        var activeLines = await db.DealerContractLines
            .Where(l => l.OrgId == Org && vins.Contains(l.Vin) && (l.Status == "Pending" || l.Status == "Draft" || l.Status == "Submitted" || l.Status == "Approved"))
            .ToListAsync();
        if (activeLines.Count > 0)
        {
            var conflict = activeLines.First();
            throw new InvalidOperationException($"VIN {conflict.Vin} đang nằm trong hợp đồng mua bán khác chưa kết thúc ({conflict.ContractNo}).");
        }

        var contractNo = string.IsNullOrWhiteSpace(dto.ContractNo)
            ? "CTR" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.ContractNo!.Trim().ToUpperInvariant();

        if (await db.DealerContracts.AnyAsync(c => c.OrgId == Org && c.ContractNo == contractNo))
            throw new InvalidOperationException($"Mã hợp đồng {contractNo} đã tồn tại.");

        var vMap = vehicles.ToDictionary(v => v.Vin);
        decimal totalAmount = 0;
        decimal totalDiscount = 0;

        foreach (var item in distinctItems)
        {
            var unitPrice = item.UnitPrice ?? 0;
            var discount = item.Discount ?? 0;
            totalAmount += unitPrice;
            totalDiscount += discount;
        }

        var finalAmount = Math.Max(0, totalAmount - totalDiscount);

        var ctr = new DealerContract
        {
            OrgId = Org,
            ContractNo = contractNo,
            ContractNoUser = dto.ContractNoUser?.Trim(),
            DealerCode = dealer,
            SOCode = dto.SOCode?.Trim()?.ToUpperInvariant(),
            ContractType = string.IsNullOrWhiteSpace(dto.ContractType) ? "Wholesale" : dto.ContractType.Trim(),
            ContractDate = dto.ContractDate ?? DateTime.Now,
            DeliveryDeadline = dto.DeliveryDeadline,
            PaymentTermDays = dto.PaymentTermDays > 0 ? dto.PaymentTermDays : 30,
            TotalQuantity = distinctItems.Count,
            TotalAmount = totalAmount,
            DiscountAmount = totalDiscount,
            FinalAmount = finalAmount,
            DepositAmount = dto.DepositAmount,
            Status = "Draft",
            CreatedBy = dto.CreatedBy?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };

        db.DealerContracts.Add(ctr);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var v = vMap[vin];
            var unitPrice = item.UnitPrice ?? 0;
            var discount = item.Discount ?? 0;
            var actualPrice = Math.Max(0, unitPrice - discount);

            db.DealerContractLines.Add(new DealerContractLine
            {
                OrgId = Org,
                DealerContractId = ctr.Id,
                ContractNo = contractNo,
                Vin = vin,
                Model = v.Model,
                Color = v.Color,
                UnitPrice = unitPrice,
                Discount = discount,
                ActualPrice = actualPrice,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "DealerContractCreated", $"{contractNo} Lập hợp đồng mua bán đại lý {dealer} ({ctr.ContractNoUser ?? "N/A"}). Đơn giá: {actualPrice:N0} VNĐ");
        }

        await db.SaveChangesAsync();

        return new
        {
            ctr.ContractNo,
            ctr.ContractNoUser,
            ctr.DealerCode,
            ctr.ContractType,
            ctr.SOCode,
            ctr.ContractDate,
            ctr.DeliveryDeadline,
            ctr.PaymentTermDays,
            ctr.TotalQuantity,
            ctr.TotalAmount,
            ctr.DiscountAmount,
            ctr.FinalAmount,
            ctr.DepositAmount,
            ctr.Status,
            linesCount = distinctItems.Count
        };
    }

    public async Task<object> ListDealerContractsAsync(string? status, string? dealer, string? contractNo, string? soCode, string? vin)
    {
        var q = db.DealerContracts.Where(c => c.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(c => c.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) { var d = dealer.Trim().ToUpperInvariant(); q = q.Where(c => c.DealerCode == d); }
        if (!string.IsNullOrWhiteSpace(contractNo)) { var cn = contractNo.Trim().ToUpperInvariant(); q = q.Where(c => c.ContractNo.Contains(cn) || (c.ContractNoUser != null && c.ContractNoUser.Contains(cn))); }
        if (!string.IsNullOrWhiteSpace(soCode)) { var so = soCode.Trim().ToUpperInvariant(); q = q.Where(c => c.SOCode == so); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.DealerContractLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.ContractNo).Distinct().ToListAsync();
            q = q.Where(c => matchedNos.Contains(c.ContractNo));
        }

        var today = DateTime.Today;
        var items = await q.OrderByDescending(c => c.Id).Take(500).Select(c => new
        {
            c.ContractNo,
            c.ContractNoUser,
            c.DealerCode,
            c.SOCode,
            c.ContractType,
            c.ContractDate,
            c.DeliveryDeadline,
            c.PaymentTermDays,
            c.TotalQuantity,
            c.TotalAmount,
            c.DiscountAmount,
            c.FinalAmount,
            c.DepositAmount,
            c.Status,
            c.CreatedBy,
            c.ApprovedBy,
            c.CreatedAt,
            c.ApprovedAt,
            c.CompletedAt,
            c.CancelledAt,
            daysUntilDeadline = c.DeliveryDeadline.HasValue ? (int)(c.DeliveryDeadline.Value.Date - today).TotalDays : (int?)null,
            isOverdue = c.Status == "Approved" && c.DeliveryDeadline.HasValue && c.DeliveryDeadline.Value.Date < today,
            lineCount = db.DealerContractLines.Count(l => l.OrgId == Org && l.DealerContractId == c.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetDealerContractAsync(string contractNo)
    {
        contractNo = contractNo.Trim().ToUpperInvariant();
        var ctr = await db.DealerContracts.FirstOrDefaultAsync(c => c.OrgId == Org && c.ContractNo == contractNo);
        if (ctr is null) return null;

        var lines = await db.DealerContractLines.Where(l => l.OrgId == Org && l.DealerContractId == ctr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var today = DateTime.Today;
        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.SpecCode,
            l.Color,
            l.UnitPrice,
            l.Discount,
            l.ActualPrice,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString(),
                v.IsMortgaged,
                v.MortgageBankCode,
                v.SOCode
            } : null
        }).ToList();

        return new
        {
            ctr.ContractNo,
            ctr.ContractNoUser,
            ctr.DealerCode,
            ctr.SOCode,
            ctr.ContractType,
            ctr.ContractDate,
            ctr.DeliveryDeadline,
            ctr.PaymentTermDays,
            ctr.TotalQuantity,
            ctr.TotalAmount,
            ctr.DiscountAmount,
            ctr.FinalAmount,
            ctr.DepositAmount,
            ctr.Status,
            ctr.CreatedBy,
            ctr.ApprovedBy,
            ctr.Remark,
            ctr.CreatedAt,
            ctr.ApprovedAt,
            ctr.CompletedAt,
            ctr.CancelledAt,
            daysUntilDeadline = ctr.DeliveryDeadline.HasValue ? (int)(ctr.DeliveryDeadline.Value.Date - today).TotalDays : (int?)null,
            isOverdue = ctr.Status == "Approved" && ctr.DeliveryDeadline.HasValue && ctr.DeliveryDeadline.Value.Date < today,
            lines = details
        };
    }

    public async Task<object?> DealerContractTransitionAsync(string contractNo, string action, DealerContractTransitionDto? dto)
    {
        contractNo = contractNo.Trim().ToUpperInvariant();
        var ctr = await db.DealerContracts.FirstOrDefaultAsync(c => c.OrgId == Org && c.ContractNo == contractNo);
        if (ctr is null) return null;

        var now = DateTime.Now;
        var lines = await db.DealerContractLines.Where(l => l.OrgId == Org && l.DealerContractId == ctr.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (ctr.Status != "Draft") return null;
                ctr.Status = "Submitted";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    ctr.Remark = string.IsNullOrWhiteSpace(ctr.Remark) ? dto.Note : $"{ctr.Remark} | Trình duyệt: {dto.Note}";
                foreach (var l in lines) l.Status = "Submitted";
                foreach (var v in vehicles) Log(v.Vin, "DealerContractSubmitted", $"{contractNo} Trình duyệt hợp đồng mua bán xe cho đại lý {ctr.DealerCode}");
                break;

            case "approve":
                if (ctr.Status is not ("Draft" or "Submitted")) return null;
                ctr.Status = "Approved";
                ctr.ApprovedBy = dto?.ApprovedBy?.Trim() ?? "SalesDirector";
                ctr.ApprovedAt = now;
                if (dto?.DeliveryDeadline.HasValue == true) ctr.DeliveryDeadline = dto.DeliveryDeadline.Value;

                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles)
                {
                    if (v.Status == VehicleStatus.InStock)
                    {
                        v.Status = VehicleStatus.Allocated;
                        v.DealerCode = ctr.DealerCode;
                    }
                    Log(v.Vin, "DealerContractApproved", $"{contractNo} Phê duyệt hợp đồng mua bán xe cho đại lý {ctr.DealerCode}. Người duyệt: {ctr.ApprovedBy}. Hạn giao: {ctr.DeliveryDeadline:yyyy-MM-dd}");
                }
                break;

            case "complete":
                if (ctr.Status != "Approved") return null;
                ctr.Status = "Completed";
                ctr.CompletedAt = now;
                foreach (var l in lines) if (l.Status == "Approved") l.Status = "Delivered";
                foreach (var v in vehicles) Log(v.Vin, "DealerContractCompleted", $"{contractNo} Hoàn tất thanh lý & bàn giao toàn bộ xe theo hợp đồng mua bán ĐL {ctr.DealerCode}");
                break;

            case "reject":
                if (ctr.Status is not ("Draft" or "Submitted")) return null;
                ctr.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    ctr.Remark = string.IsNullOrWhiteSpace(ctr.Remark) ? dto.Note : $"{ctr.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "DealerContractRejected", $"{contractNo} Từ chối hợp đồng mua bán: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (ctr.Status is not ("Draft" or "Submitted" or "Approved")) return null;
                ctr.Status = "Cancelled";
                ctr.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    ctr.Remark = string.IsNullOrWhiteSpace(ctr.Remark) ? dto.Note : $"{ctr.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "DealerContractCancelled", $"{contractNo} Hủy hợp đồng mua bán: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            ctr.ContractNo,
            ctr.ContractNoUser,
            ctr.DealerCode,
            status = ctr.Status,
            ctr.ApprovedAt,
            ctr.CompletedAt,
            ctr.CancelledAt
        };
    }

    public async Task<object?> UpdateDealerContractLineAsync(string contractNo, string vin, UpdateDealerContractLineDto dto)
    {
        contractNo = contractNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var ctr = await db.DealerContracts.FirstOrDefaultAsync(c => c.OrgId == Org && c.ContractNo == contractNo);
        if (ctr is null || ctr.Status is "Approved" or "Completed" or "Cancelled" or "Rejected") return null;

        var line = await db.DealerContractLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.DealerContractId == ctr.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.UnitPrice.HasValue && dto.UnitPrice.Value >= 0) line.UnitPrice = dto.UnitPrice.Value;
        if (dto.Discount.HasValue && dto.Discount.Value >= 0) line.Discount = dto.Discount.Value;
        line.ActualPrice = Math.Max(0, line.UnitPrice - line.Discount);
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.DealerContractLines.Where(l => l.OrgId == Org && l.DealerContractId == ctr.Id).ToListAsync();
        ctr.TotalAmount = allLines.Sum(l => l.UnitPrice);
        ctr.DiscountAmount = allLines.Sum(l => l.Discount);
        ctr.FinalAmount = Math.Max(0, ctr.TotalAmount - ctr.DiscountAmount);

        await db.SaveChangesAsync();

        return new
        {
            ctr.ContractNo,
            line.Vin,
            line.UnitPrice,
            line.Discount,
            line.ActualPrice,
            line.Remark,
            contractTotalAmount = ctr.TotalAmount,
            contractDiscountAmount = ctr.DiscountAmount,
            contractFinalAmount = ctr.FinalAmount
        };
    }

    // ===== Yêu cầu & Quyết toán Chiết khấu thanh toán mua xe ô tô cho Đại lý (BizHTC.PaymentDiscount / Req_PaymentDiscount) =====
    public async Task<object> CreatePaymentDiscountAsync(CreatePaymentDiscountDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần mã đại lý DealerCode.");
        if (dto.Items is null || dto.Items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 dòng chi tiết xe yêu cầu chiết khấu thanh toán.");

        var dealer = dto.DealerCode.Trim().ToUpperInvariant();
        var distinctItems = dto.Items.DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();

        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();
        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var vMap = vehicles.ToDictionary(v => v.Vin);

        var today = DateTime.Today;
        var discountNo = string.IsNullOrWhiteSpace(dto.PaymentDiscountNo)
            ? $"{today:yyyyMMdd}-{(await db.PaymentDiscounts.CountAsync(d => d.OrgId == Org && d.CreatedAt.Date == today) + 1):000}/DNCK/{dealer}"
            : dto.PaymentDiscountNo!.Trim().ToUpperInvariant();

        if (await db.PaymentDiscounts.AnyAsync(d => d.OrgId == Org && d.PaymentDiscountNo == discountNo))
            throw new InvalidOperationException($"Mã đề nghị chiết khấu {discountNo} đã tồn tại.");

        decimal totalPayment = 0;
        decimal totalDiscount = 0;

        var lineList = new List<PaymentDiscountLine>();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var v = vMap[vin];
            var unitPrice = item.UnitPrice ?? 0;

            // Phase 1
            var p1Amount = item.Phase1?.Amount ?? 0;
            var p1Days = item.Phase1?.DiscountDateNumber ?? 0;
            var p1Pct = item.Phase1?.DiscountPercent ?? (dto.DiscountPercent > 0 ? dto.DiscountPercent : 0);
            var p1Price = item.Phase1?.DiscountPrice ?? (p1Amount > 0 && p1Pct > 0 ? Math.Round(p1Amount * (p1Pct / 100m), 0) : 0);

            // Phase 2
            var p2Amount = item.Phase2?.Amount ?? 0;
            var p2Days = item.Phase2?.DiscountDateNumber ?? 0;
            var p2Pct = item.Phase2?.DiscountPercent ?? 0;
            var p2Price = item.Phase2?.DiscountPrice ?? (p2Amount > 0 && p2Pct > 0 ? Math.Round(p2Amount * (p2Pct / 100m), 0) : 0);

            // Phase 3
            var p3Amount = item.Phase3?.Amount ?? 0;
            var p3Days = item.Phase3?.DiscountDateNumber ?? 0;
            var p3Pct = item.Phase3?.DiscountPercent ?? 0;
            var p3Price = item.Phase3?.DiscountPrice ?? (p3Amount > 0 && p3Pct > 0 ? Math.Round(p3Amount * (p3Pct / 100m), 0) : 0);

            var lineTotalAmount = item.TotalAmount ?? (p1Amount + p2Amount + p3Amount);
            if (lineTotalAmount == 0 && unitPrice > 0) lineTotalAmount = unitPrice;

            var lineTotalDiscount = item.TotalDiscountPrice ?? (p1Price + p2Price + p3Price);

            totalPayment += lineTotalAmount;
            totalDiscount += lineTotalDiscount;

            lineList.Add(new PaymentDiscountLine
            {
                OrgId = Org,
                PaymentDiscountNo = discountNo,
                Vin = vin,
                Model = v.Model,
                GuaranteeNo = item.GuaranteeNo?.Trim().ToUpperInvariant(),
                UnitPrice = unitPrice,
                PaymentEndDatePhase1 = item.Phase1?.PaymentEndDate,
                AmountPhase1 = p1Amount,
                DiscountDateNumberPhase1 = p1Days,
                DiscountPercentPhase1 = p1Pct,
                DiscountPricePhase1 = p1Price,
                PaymentEndDatePhase2 = item.Phase2?.PaymentEndDate,
                AmountPhase2 = p2Amount,
                DiscountDateNumberPhase2 = p2Days,
                DiscountPercentPhase2 = p2Pct,
                DiscountPricePhase2 = p2Price,
                PaymentEndDatePhase3 = item.Phase3?.PaymentEndDate,
                AmountPhase3 = p3Amount,
                DiscountDateNumberPhase3 = p3Days,
                DiscountPercentPhase3 = p3Pct,
                DiscountPricePhase3 = p3Price,
                TotalAmount = lineTotalAmount,
                TotalDiscountPrice = lineTotalDiscount,
                PG_DateEnd = item.PG_DateEnd,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });
        }

        var pd = new PaymentDiscount
        {
            OrgId = Org,
            PaymentDiscountNo = discountNo,
            DealerCode = dealer,
            DateEndFrom = dto.DateEndFrom,
            DateEndTo = dto.DateEndTo,
            TotalVehicleCount = distinctItems.Count,
            TotalPaymentAmount = totalPayment,
            TotalDiscountAmount = totalDiscount,
            DiscountPercent = dto.DiscountPercent,
            PenaltyPercent = dto.PenaltyPercent,
            FilePath = dto.FilePath?.Trim(),
            PmtDctStatus = "Draft",
            DlrSignStatus = "Pending",
            HTCSignStatus = "Pending",
            CreatedBy = dto.CreatedBy?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };

        db.PaymentDiscounts.Add(pd);
        await db.SaveChangesAsync();

        foreach (var line in lineList)
        {
            line.PaymentDiscountId = pd.Id;
            db.PaymentDiscountLines.Add(line);
            Log(line.Vin, "PaymentDiscountCreated", $"{discountNo} Lập đề nghị chiết khấu thanh toán ĐL {dealer}. Tiền TT: {line.TotalAmount:N0} VNĐ, Tiền CK: {line.TotalDiscountPrice:N0} VNĐ");
        }

        await db.SaveChangesAsync();

        return new
        {
            pd.PaymentDiscountNo,
            pd.DealerCode,
            pd.DateEndFrom,
            pd.DateEndTo,
            pd.TotalVehicleCount,
            pd.TotalPaymentAmount,
            pd.TotalDiscountAmount,
            pd.DiscountPercent,
            pd.PmtDctStatus,
            pd.DlrSignStatus,
            pd.HTCSignStatus,
            linesCount = lineList.Count
        };
    }

    public async Task<object> ListPaymentDiscountsAsync(string? status, string? dealer, string? paymentDiscountNo, string? vin)
    {
        var q = db.PaymentDiscounts.Where(d => d.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(d => d.PmtDctStatus == status);
        if (!string.IsNullOrWhiteSpace(dealer)) { var dl = dealer.Trim().ToUpperInvariant(); q = q.Where(d => d.DealerCode == dl); }
        if (!string.IsNullOrWhiteSpace(paymentDiscountNo)) { var no = paymentDiscountNo.Trim().ToUpperInvariant(); q = q.Where(d => d.PaymentDiscountNo.Contains(no)); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.PaymentDiscountLines.Where(l => l.OrgId == Org && l.Vin == vv).Select(l => l.PaymentDiscountNo).Distinct().ToListAsync();
            q = q.Where(d => matchedNos.Contains(d.PaymentDiscountNo));
        }

        var items = await q.OrderByDescending(d => d.Id).Take(500).Select(d => new
        {
            d.PaymentDiscountNo,
            d.DealerCode,
            d.DateEndFrom,
            d.DateEndTo,
            d.TotalVehicleCount,
            d.TotalPaymentAmount,
            d.TotalDiscountAmount,
            d.DiscountPercent,
            d.PenaltyPercent,
            d.FilePath,
            d.PmtDctStatus,
            d.DlrSignStatus,
            d.HTCSignStatus,
            d.CreatedBy,
            d.CreatedAt,
            d.HTCApprBy,
            d.HTCApprAt,
            d.DlrSignBy,
            d.DlrSignAt,
            d.HTCSignBy,
            d.HTCSignAt,
            d.RejectBy,
            d.RejectAt,
            d.CancelBy,
            d.CancelAt,
            lineCount = db.PaymentDiscountLines.Count(l => l.OrgId == Org && l.PaymentDiscountId == d.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetPaymentDiscountAsync(string paymentDiscountNo)
    {
        paymentDiscountNo = paymentDiscountNo.Trim().ToUpperInvariant();
        var pd = await db.PaymentDiscounts.FirstOrDefaultAsync(d => d.OrgId == Org && d.PaymentDiscountNo == paymentDiscountNo);
        if (pd is null) return null;

        var lines = await db.PaymentDiscountLines.Where(l => l.OrgId == Org && l.PaymentDiscountId == pd.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.GuaranteeNo,
            l.UnitPrice,
            phase1 = new
            {
                paymentEndDate = l.PaymentEndDatePhase1,
                amount = l.AmountPhase1,
                discountDateNumber = l.DiscountDateNumberPhase1,
                discountPercent = l.DiscountPercentPhase1,
                discountPrice = l.DiscountPricePhase1
            },
            phase2 = new
            {
                paymentEndDate = l.PaymentEndDatePhase2,
                amount = l.AmountPhase2,
                discountDateNumber = l.DiscountDateNumberPhase2,
                discountPercent = l.DiscountPercentPhase2,
                discountPrice = l.DiscountPricePhase2
            },
            phase3 = new
            {
                paymentEndDate = l.PaymentEndDatePhase3,
                amount = l.AmountPhase3,
                discountDateNumber = l.DiscountDateNumberPhase3,
                discountPercent = l.DiscountPercentPhase3,
                discountPrice = l.DiscountPricePhase3
            },
            l.TotalAmount,
            l.TotalDiscountPrice,
            l.PG_DateEnd,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.Color,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString(),
                v.IsMortgaged,
                v.MortgageBankCode
            } : null
        }).ToList();

        return new
        {
            pd.PaymentDiscountNo,
            pd.DealerCode,
            pd.DateEndFrom,
            pd.DateEndTo,
            pd.TotalVehicleCount,
            pd.TotalPaymentAmount,
            pd.TotalDiscountAmount,
            pd.DiscountPercent,
            pd.PenaltyPercent,
            pd.FilePath,
            pd.PmtDctStatus,
            pd.DlrSignStatus,
            pd.HTCSignStatus,
            pd.CreatedBy,
            pd.CreatedAt,
            pd.HTCApprBy,
            pd.HTCApprAt,
            pd.DlrSignBy,
            pd.DlrSignAt,
            pd.HTCSignBy,
            pd.HTCSignAt,
            pd.RejectBy,
            pd.RejectAt,
            pd.CancelBy,
            pd.CancelAt,
            pd.Remark,
            lines = details
        };
    }

    public async Task<object?> PaymentDiscountTransitionAsync(string paymentDiscountNo, string action, PaymentDiscountTransitionDto? dto)
    {
        paymentDiscountNo = paymentDiscountNo.Trim().ToUpperInvariant();
        var pd = await db.PaymentDiscounts.FirstOrDefaultAsync(d => d.OrgId == Org && d.PaymentDiscountNo == paymentDiscountNo);
        if (pd is null) return null;

        var now = DateTime.Now;
        var lines = await db.PaymentDiscountLines.Where(l => l.OrgId == Org && l.PaymentDiscountId == pd.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "approve":
                if (pd.PmtDctStatus is not ("Draft" or "Pending" or "NotSign")) return null;
                pd.PmtDctStatus = "Approved";
                pd.HTCSignStatus = "Approved";
                pd.HTCApprBy = dto?.User?.Trim() ?? "HTCSalesManager";
                pd.HTCApprAt = now;
                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "PaymentDiscountApproved", $"{paymentDiscountNo} Hãng OEM sơ duyệt chiết khấu thanh toán ĐL {pd.DealerCode}. Người duyệt: {pd.HTCApprBy}");
                break;

            case "dlr-sign":
            case "dlrsign":
            case "sign-dlr":
                if (pd.PmtDctStatus is "Rejected" or "Cancelled" or "Signed" or "Sign") return null;
                pd.DlrSignStatus = "Signed";
                pd.DlrSignBy = dto?.User?.Trim() ?? "DealerDirector";
                pd.DlrSignAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.FilePath)) pd.FilePath = dto.FilePath.Trim();
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "PaymentDiscountDlrSigned", $"{paymentDiscountNo} Đại lý {pd.DealerCode} ký số xác nhận chiết khấu thanh toán. Người ký: {pd.DlrSignBy}");
                break;

            case "htc-sign":
            case "htcsign":
            case "sign-htc":
            case "settle":
                if (pd.DlrSignStatus != "Signed" || pd.PmtDctStatus is "Rejected" or "Cancelled") return null;
                pd.HTCSignStatus = "Signed";
                pd.PmtDctStatus = "Signed";
                pd.HTCSignBy = dto?.User?.Trim() ?? "HTCFinanceDirector";
                pd.HTCSignAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.FilePath)) pd.FilePath = dto.FilePath.Trim();
                foreach (var l in lines) l.Status = "Signed";
                foreach (var v in vehicles) Log(v.Vin, "PaymentDiscountHTCSigned", $"{paymentDiscountNo} Hãng OEM ký số phê duyệt quyết toán chiết khấu thanh toán ĐL {pd.DealerCode}. Tổng chiết khấu: {pd.TotalDiscountAmount:N0} VNĐ. Người ký: {pd.HTCSignBy}");
                break;

            case "reject":
                if (pd.PmtDctStatus is "Signed" or "Sign") return null;
                pd.PmtDctStatus = "Rejected";
                pd.RejectBy = dto?.User?.Trim() ?? "HTCOfficer";
                pd.RejectAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pd.Remark = string.IsNullOrWhiteSpace(pd.Remark) ? dto.Note : $"{pd.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "PaymentDiscountRejected", $"{paymentDiscountNo} Từ chối chiết khấu thanh toán: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (pd.PmtDctStatus is "Signed" or "Sign") return null;
                pd.PmtDctStatus = "Cancelled";
                pd.CancelBy = dto?.User?.Trim() ?? "Operator";
                pd.CancelAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pd.Remark = string.IsNullOrWhiteSpace(pd.Remark) ? dto.Note : $"{pd.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "PaymentDiscountCancelled", $"{paymentDiscountNo} Hủy đề nghị chiết khấu thanh toán: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            pd.PaymentDiscountNo,
            pd.DealerCode,
            status = pd.PmtDctStatus,
            dlrSignStatus = pd.DlrSignStatus,
            htcSignStatus = pd.HTCSignStatus,
            pd.HTCApprAt,
            pd.DlrSignAt,
            pd.HTCSignAt,
            pd.RejectAt,
            pd.CancelAt
        };
    }

    public async Task<object?> UpdatePaymentDiscountLineAsync(string paymentDiscountNo, string vin, UpdatePaymentDiscountLineDto dto)
    {
        paymentDiscountNo = paymentDiscountNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var pd = await db.PaymentDiscounts.FirstOrDefaultAsync(d => d.OrgId == Org && d.PaymentDiscountNo == paymentDiscountNo);
        if (pd is null || pd.PmtDctStatus is "Signed" or "Sign" or "Cancelled" or "Rejected") return null;

        var line = await db.PaymentDiscountLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.PaymentDiscountId == pd.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.Phase1 != null)
        {
            if (dto.Phase1.PaymentEndDate.HasValue) line.PaymentEndDatePhase1 = dto.Phase1.PaymentEndDate;
            if (dto.Phase1.Amount >= 0) line.AmountPhase1 = dto.Phase1.Amount;
            if (dto.Phase1.DiscountDateNumber >= 0) line.DiscountDateNumberPhase1 = dto.Phase1.DiscountDateNumber;
            if (dto.Phase1.DiscountPercent >= 0) line.DiscountPercentPhase1 = dto.Phase1.DiscountPercent;
            line.DiscountPricePhase1 = dto.Phase1.DiscountPrice ?? (line.AmountPhase1 > 0 && line.DiscountPercentPhase1 > 0 ? Math.Round(line.AmountPhase1 * (line.DiscountPercentPhase1 / 100m), 0) : 0);
        }

        if (dto.Phase2 != null)
        {
            if (dto.Phase2.PaymentEndDate.HasValue) line.PaymentEndDatePhase2 = dto.Phase2.PaymentEndDate;
            if (dto.Phase2.Amount >= 0) line.AmountPhase2 = dto.Phase2.Amount;
            if (dto.Phase2.DiscountDateNumber >= 0) line.DiscountDateNumberPhase2 = dto.Phase2.DiscountDateNumber;
            if (dto.Phase2.DiscountPercent >= 0) line.DiscountPercentPhase2 = dto.Phase2.DiscountPercent;
            line.DiscountPricePhase2 = dto.Phase2.DiscountPrice ?? (line.AmountPhase2 > 0 && line.DiscountPercentPhase2 > 0 ? Math.Round(line.AmountPhase2 * (line.DiscountPercentPhase2 / 100m), 0) : 0);
        }

        if (dto.Phase3 != null)
        {
            if (dto.Phase3.PaymentEndDate.HasValue) line.PaymentEndDatePhase3 = dto.Phase3.PaymentEndDate;
            if (dto.Phase3.Amount >= 0) line.AmountPhase3 = dto.Phase3.Amount;
            if (dto.Phase3.DiscountDateNumber >= 0) line.DiscountDateNumberPhase3 = dto.Phase3.DiscountDateNumber;
            if (dto.Phase3.DiscountPercent >= 0) line.DiscountPercentPhase3 = dto.Phase3.DiscountPercent;
            line.DiscountPricePhase3 = dto.Phase3.DiscountPrice ?? (line.AmountPhase3 > 0 && line.DiscountPercentPhase3 > 0 ? Math.Round(line.AmountPhase3 * (line.DiscountPercentPhase3 / 100m), 0) : 0);
        }

        if (!string.IsNullOrWhiteSpace(dto.GuaranteeNo)) line.GuaranteeNo = dto.GuaranteeNo.Trim().ToUpperInvariant();
        if (dto.PG_DateEnd.HasValue) line.PG_DateEnd = dto.PG_DateEnd.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        line.TotalAmount = line.AmountPhase1 + line.AmountPhase2 + line.AmountPhase3;
        if (line.TotalAmount == 0 && line.UnitPrice > 0) line.TotalAmount = line.UnitPrice;
        line.TotalDiscountPrice = line.DiscountPricePhase1 + line.DiscountPricePhase2 + line.DiscountPricePhase3;

        var allLines = await db.PaymentDiscountLines.Where(l => l.OrgId == Org && l.PaymentDiscountId == pd.Id).ToListAsync();
        pd.TotalPaymentAmount = allLines.Sum(l => l.TotalAmount);
        pd.TotalDiscountAmount = allLines.Sum(l => l.TotalDiscountPrice);

        await db.SaveChangesAsync();

        return new
        {
            pd.PaymentDiscountNo,
            line.Vin,
            line.TotalAmount,
            line.TotalDiscountPrice,
            line.Remark,
            discountTotalPaymentAmount = pd.TotalPaymentAmount,
            discountTotalDiscountAmount = pd.TotalDiscountAmount
        };
    }

    // ===== Yêu cầu & Quản lý Bảo hiểm lô xe vận chuyển & lưu kho (BizHTC.WH.Ins_InsuranceReq / Ins_InsuranceReq) =====
    public async Task<object> CreateInsuranceRequestAsync(CreateInsuranceRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.InsCompanyCode))
            throw new InvalidOperationException("Cần mã hãng bảo hiểm InsCompanyCode.");

        var items = new List<InsuranceItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0)
        {
            items.AddRange(dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)));
        }
        else if (dto.Vins != null && dto.Vins.Count > 0)
        {
            items.AddRange(dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => new InsuranceItemInputDto(v)));
        }

        if (items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe (VIN) trong yêu cầu bảo hiểm.");

        var distinctItems = items.DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();

        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();
        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var vMap = vehicles.ToDictionary(v => v.Vin);

        var companyCode = dto.InsCompanyCode.Trim().ToUpperInvariant();
        var companyName = !string.IsNullOrWhiteSpace(dto.InsCompanyName)
            ? dto.InsCompanyName.Trim()
            : companyCode switch
            {
                "PVI" => "Tổng công ty Bảo hiểm PVI",
                "BAOVIET" or "BV" => "Tổng công ty Bảo hiểm Bảo Việt",
                "PJICO" => "Tổng công ty Cổ phần Bảo hiểm Petrolimex (PJICO)",
                "PTI" => "Tổng công ty Cổ phần Bảo hiểm Bưu điện (PTI)",
                "BMI" or "BAOMINH" => "Tổng công ty Cổ phần Bảo Minh",
                "BIC" => "Tổng công ty Bảo hiểm BIDV (BIC)",
                "MIC" => "Tổng công ty Cổ phần Bảo hiểm Quân đội (MIC)",
                _ => companyCode
            };

        var reqNo = string.IsNullOrWhiteSpace(dto.InsReqNo)
            ? "INS" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.InsReqNo.Trim().ToUpperInvariant();

        if (await db.InsuranceRequests.AnyAsync(r => r.OrgId == Org && r.InsReqNo == reqNo))
            throw new InvalidOperationException($"Mã yêu cầu bảo hiểm {reqNo} đã tồn tại.");

        var defaultRate = dto.PremiumRate is > 0 ? dto.PremiumRate.Value : 0.15m;
        var effectiveDate = dto.EffectiveDate ?? DateTime.Now;

        decimal totalInsuredValue = 0;
        decimal totalPremiumAmount = 0;

        var lineList = new List<InsuranceRequestLine>();
        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var v = vMap[vin];

            var lineInsuredValue = item.InsuredValue.HasValue && item.InsuredValue.Value > 0
                ? item.InsuredValue.Value
                : 500000000m; // Định giá chuẩn nếu không chỉ định

            var lineRate = item.PremiumRate is > 0 ? item.PremiumRate.Value : defaultRate;
            var linePremium = item.PremiumAmount.HasValue && item.PremiumAmount.Value >= 0
                ? item.PremiumAmount.Value
                : Math.Round(lineInsuredValue * (lineRate / 100m), 0);

            var days = item.InsuranceDays > 0 ? item.InsuranceDays : 30;
            var fromStorage = item.FromStorage?.Trim() ?? v.StorageCode ?? "YARD-DEFAULT";
            var toStorage = item.ToStorage?.Trim() ?? v.DealerCode ?? "DLR-DEST";

            totalInsuredValue += lineInsuredValue;
            totalPremiumAmount += linePremium;

            lineList.Add(new InsuranceRequestLine
            {
                OrgId = Org,
                InsReqNo = reqNo,
                Vin = vin,
                Model = v.Model,
                EngineNo = v.EngineNo,
                Color = v.Color,
                InsuredValue = lineInsuredValue,
                PremiumRate = lineRate,
                PremiumAmount = linePremium,
                InsuranceDays = days,
                FromStorage = fromStorage,
                ToStorage = toStorage,
                CertificateNo = item.CertificateNo?.Trim().ToUpperInvariant(),
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });
        }

        var ins = new InsuranceRequest
        {
            OrgId = Org,
            InsReqNo = reqNo,
            InsCompanyCode = companyCode,
            InsCompanyName = companyName,
            InsTypeCode = string.IsNullOrWhiteSpace(dto.InsTypeCode) ? "CARGO" : dto.InsTypeCode.Trim().ToUpperInvariant(),
            PolicyNo = dto.PolicyNo?.Trim().ToUpperInvariant(),
            EffectiveDate = effectiveDate,
            ExpireDate = dto.ExpireDate ?? effectiveDate.AddDays(30),
            TotalVehicleCount = distinctItems.Count,
            TotalInsuredValue = totalInsuredValue,
            PremiumRate = defaultRate,
            TotalPremiumAmount = totalPremiumAmount,
            Status = "Draft",
            CreatedBy = dto.CreatedBy?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedAt = DateTime.Now
        };

        db.InsuranceRequests.Add(ins);
        await db.SaveChangesAsync();

        foreach (var l in lineList)
        {
            l.InsuranceRequestId = ins.Id;
            db.InsuranceRequestLines.Add(l);
            Log(l.Vin, "InsuranceReqCreated", $"{reqNo} Hãng BH: {ins.InsCompanyCode} ({ins.InsTypeCode}). Giá trị định giá: {l.InsuredValue:N0} VNĐ, Phí: {l.PremiumAmount:N0} VNĐ");
        }
        await db.SaveChangesAsync();

        return new
        {
            ins.InsReqNo,
            ins.InsCompanyCode,
            ins.InsCompanyName,
            ins.InsTypeCode,
            ins.EffectiveDate,
            ins.ExpireDate,
            ins.TotalVehicleCount,
            ins.TotalInsuredValue,
            ins.TotalPremiumAmount,
            ins.Status,
            linesCount = lineList.Count
        };
    }

    public async Task<object> ListInsuranceRequestsAsync(string? status, string? insCompanyCode, string? insTypeCode, string? vin)
    {
        var q = db.InsuranceRequests.Where(r => r.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(r => r.Status == status);
        if (!string.IsNullOrWhiteSpace(insCompanyCode)) q = q.Where(r => r.InsCompanyCode == insCompanyCode);
        if (!string.IsNullOrWhiteSpace(insTypeCode)) q = q.Where(r => r.InsTypeCode == insTypeCode);
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.InsuranceRequestLines
                .Where(l => l.OrgId == Org && l.Vin == vv)
                .Select(l => l.InsReqNo)
                .Distinct()
                .ToListAsync();
            q = q.Where(r => matchedNos.Contains(r.InsReqNo));
        }

        var items = await q.OrderByDescending(r => r.Id).Take(500).Select(r => new
        {
            r.InsReqNo,
            r.InsCompanyCode,
            r.InsCompanyName,
            r.InsTypeCode,
            r.PolicyNo,
            r.EffectiveDate,
            r.ExpireDate,
            r.TotalVehicleCount,
            r.TotalInsuredValue,
            r.PremiumRate,
            r.TotalPremiumAmount,
            r.Status,
            r.CreatedBy,
            r.ApprovedBy,
            r.CreatedAt,
            r.ApprovedAt,
            r.CompletedAt,
            r.CancelledAt,
            linesCount = db.InsuranceRequestLines.Count(l => l.OrgId == Org && l.InsuranceRequestId == r.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetInsuranceRequestAsync(string insReqNo)
    {
        insReqNo = insReqNo.Trim().ToUpperInvariant();
        var ins = await db.InsuranceRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.InsReqNo == insReqNo);
        if (ins is null) return null;

        var lines = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.EngineNo,
            l.Color,
            l.InsuredValue,
            l.PremiumRate,
            l.PremiumAmount,
            l.InsuranceDays,
            l.FromStorage,
            l.ToStorage,
            l.CertificateNo,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString(),
                v.OwnerName,
                v.PlateNo
            } : null
        }).ToList();

        return new
        {
            ins.InsReqNo,
            ins.InsCompanyCode,
            ins.InsCompanyName,
            ins.InsTypeCode,
            ins.PolicyNo,
            ins.EffectiveDate,
            ins.ExpireDate,
            ins.TotalVehicleCount,
            ins.TotalInsuredValue,
            ins.PremiumRate,
            ins.TotalPremiumAmount,
            ins.Status,
            ins.CreatedBy,
            ins.CreatedAt,
            ins.ApprovedBy,
            ins.ApprovedAt,
            ins.CompletedAt,
            ins.CancelledAt,
            ins.Remark,
            lines = details
        };
    }

    public async Task<object?> InsuranceRequestTransitionAsync(string insReqNo, string action, InsuranceRequestTransitionDto? dto)
    {
        insReqNo = insReqNo.Trim().ToUpperInvariant();
        var ins = await db.InsuranceRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.InsReqNo == insReqNo);
        if (ins is null) return null;

        var now = DateTime.Now;
        var lines = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (ins.Status != "Draft") return null;
                ins.Status = "Submitted";
                foreach (var l in lines) l.Status = "Submitted";
                foreach (var v in vehicles) Log(v.Vin, "InsuranceReqSubmitted", $"{insReqNo} Trình duyệt bảo hiểm tới hãng {ins.InsCompanyCode}");
                break;

            case "approve":
                if (ins.Status is not ("Draft" or "Submitted")) return null;
                ins.Status = "Approved";
                ins.ApprovedBy = dto?.ApprovedBy?.Trim() ?? "InsManager";
                ins.ApprovedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.PolicyNo)) ins.PolicyNo = dto.PolicyNo.Trim().ToUpperInvariant();
                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "InsuranceReqApproved", $"{insReqNo} Phê duyệt bảo hiểm {ins.InsCompanyCode} ({ins.InsTypeCode}). Số HĐ: {ins.PolicyNo ?? "N/A"}. Duyệt bởi: {ins.ApprovedBy}");
                break;

            case "complete":
                if (ins.Status != "Approved") return null;
                ins.Status = "Completed";
                ins.CompletedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.PolicyNo)) ins.PolicyNo = dto.PolicyNo.Trim().ToUpperInvariant();
                foreach (var l in lines)
                {
                    l.Status = "Completed";
                    if (string.IsNullOrWhiteSpace(l.CertificateNo))
                    {
                        var shortVin = l.Vin.Length >= 6 ? l.Vin[^6..] : l.Vin;
                        l.CertificateNo = $"GCN-{ins.InsCompanyCode}-{now:yyyyMMdd}-{shortVin}";
                    }
                }
                foreach (var v in vehicles) Log(v.Vin, "InsuranceReqCompleted", $"{insReqNo} Hoàn tất & xuất GCN bảo hiểm điện tử {ins.InsCompanyCode} ({ins.InsTypeCode}). Số HĐ: {ins.PolicyNo ?? "N/A"}");
                break;

            case "reject":
                if (ins.Status is "Completed" or "Cancelled") return null;
                ins.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    ins.Remark = string.IsNullOrWhiteSpace(ins.Remark) ? dto.Note : $"{ins.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "InsuranceReqRejected", $"{insReqNo} Từ chối yêu cầu bảo hiểm: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (ins.Status is "Completed" or "Cancelled") return null;
                ins.Status = "Cancelled";
                ins.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    ins.Remark = string.IsNullOrWhiteSpace(ins.Remark) ? dto.Note : $"{ins.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "InsuranceReqCancelled", $"{insReqNo} Hủy yêu cầu bảo hiểm: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            ins.InsReqNo,
            ins.InsCompanyCode,
            ins.PolicyNo,
            status = ins.Status,
            ins.ApprovedAt,
            ins.CompletedAt,
            ins.CancelledAt
        };
    }

    public async Task<object?> UpdateInsuranceRequestLineAsync(string insReqNo, string vin, UpdateInsuranceRequestLineDto dto)
    {
        insReqNo = insReqNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var ins = await db.InsuranceRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.InsReqNo == insReqNo);
        if (ins is null || ins.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var line = await db.InsuranceRequestLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.InsuredValue.HasValue && dto.InsuredValue.Value > 0) line.InsuredValue = dto.InsuredValue.Value;
        if (dto.PremiumRate.HasValue && dto.PremiumRate.Value > 0) line.PremiumRate = dto.PremiumRate.Value;
        if (dto.PremiumAmount.HasValue && dto.PremiumAmount.Value >= 0) line.PremiumAmount = dto.PremiumAmount.Value;
        else if (dto.InsuredValue.HasValue || dto.PremiumRate.HasValue)
            line.PremiumAmount = Math.Round(line.InsuredValue * (line.PremiumRate / 100m), 0);

        if (dto.InsuranceDays.HasValue && dto.InsuranceDays.Value > 0) line.InsuranceDays = dto.InsuranceDays.Value;
        if (!string.IsNullOrWhiteSpace(dto.FromStorage)) line.FromStorage = dto.FromStorage.Trim();
        if (!string.IsNullOrWhiteSpace(dto.ToStorage)) line.ToStorage = dto.ToStorage.Trim();
        if (!string.IsNullOrWhiteSpace(dto.CertificateNo)) line.CertificateNo = dto.CertificateNo.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id).ToListAsync();
        ins.TotalInsuredValue = allLines.Sum(l => l.InsuredValue);
        ins.TotalPremiumAmount = allLines.Sum(l => l.PremiumAmount);

        await db.SaveChangesAsync();

        return new
        {
            ins.InsReqNo,
            line.Vin,
            line.InsuredValue,
            line.PremiumRate,
            line.PremiumAmount,
            line.CertificateNo,
            line.Remark,
            totalInsuredValue = ins.TotalInsuredValue,
            totalPremiumAmount = ins.TotalPremiumAmount
        };
    }

    public async Task<object?> AddInsuranceRequestLinesAsync(string insReqNo, List<InsuranceItemInputDto> items)
    {
        insReqNo = insReqNo.Trim().ToUpperInvariant();
        var ins = await db.InsuranceRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.InsReqNo == insReqNo);
        if (ins is null || ins.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var distinctItems = items.Where(i => !string.IsNullOrWhiteSpace(i.Vin))
            .DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        if (distinctItems.Count == 0) return null;

        var existingVins = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id)
            .Select(l => l.Vin).ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin.Trim().ToUpperInvariant())).ToList();
        if (newItems.Count == 0) return null;

        var newVins = newItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && newVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        foreach (var item in newItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            if (!vehicles.TryGetValue(vin, out var v)) continue;

            var lineVal = item.InsuredValue is > 0 ? item.InsuredValue.Value : 500000000m;
            var lineRate = item.PremiumRate is > 0 ? item.PremiumRate.Value : ins.PremiumRate;
            var linePremium = item.PremiumAmount is >= 0 ? item.PremiumAmount.Value : Math.Round(lineVal * (lineRate / 100m), 0);

            db.InsuranceRequestLines.Add(new InsuranceRequestLine
            {
                OrgId = Org,
                InsuranceRequestId = ins.Id,
                InsReqNo = ins.InsReqNo,
                Vin = vin,
                Model = v.Model,
                EngineNo = v.EngineNo,
                Color = v.Color,
                InsuredValue = lineVal,
                PremiumRate = lineRate,
                PremiumAmount = linePremium,
                InsuranceDays = item.InsuranceDays > 0 ? item.InsuranceDays : 30,
                FromStorage = item.FromStorage?.Trim() ?? v.StorageCode ?? "YARD-DEFAULT",
                ToStorage = item.ToStorage?.Trim() ?? v.DealerCode ?? "DLR-DEST",
                CertificateNo = item.CertificateNo?.Trim().ToUpperInvariant(),
                Status = ins.Status == "Approved" ? "Approved" : "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "InsuranceReqLineAdded", $"{insReqNo} Bổ sung xe vào yêu cầu bảo hiểm {ins.InsCompanyCode}");
        }

        await db.SaveChangesAsync();

        var allLines = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id).ToListAsync();
        ins.TotalVehicleCount = allLines.Count;
        ins.TotalInsuredValue = allLines.Sum(l => l.InsuredValue);
        ins.TotalPremiumAmount = allLines.Sum(l => l.PremiumAmount);
        await db.SaveChangesAsync();

        return new
        {
            ins.InsReqNo,
            addedCount = newItems.Count,
            ins.TotalVehicleCount,
            ins.TotalInsuredValue,
            ins.TotalPremiumAmount
        };
    }

    public async Task<object?> RemoveInsuranceRequestLineAsync(string insReqNo, string vin)
    {
        insReqNo = insReqNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var ins = await db.InsuranceRequests.FirstOrDefaultAsync(r => r.OrgId == Org && r.InsReqNo == insReqNo);
        if (ins is null || ins.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var line = await db.InsuranceRequestLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id && l.Vin == vin);
        if (line is null) return null;

        db.InsuranceRequestLines.Remove(line);
        Log(vin, "InsuranceReqLineRemoved", $"{insReqNo} Rút xe khỏi yêu cầu bảo hiểm {ins.InsCompanyCode}");
        await db.SaveChangesAsync();

        var allLines = await db.InsuranceRequestLines.Where(l => l.OrgId == Org && l.InsuranceRequestId == ins.Id).ToListAsync();
        ins.TotalVehicleCount = allLines.Count;
        ins.TotalInsuredValue = allLines.Sum(l => l.InsuredValue);
        ins.TotalPremiumAmount = allLines.Sum(l => l.PremiumAmount);
        await db.SaveChangesAsync();

        return new
        {
            ins.InsReqNo,
            vin,
            ins.TotalVehicleCount,
            ins.TotalInsuredValue,
            ins.TotalPremiumAmount
        };
    }

    public async Task<object> CreateTransportMinutesAsync(CreateTransportMinutesDto dto)
    {
        var dealerCode = dto.DealerCode.Trim().ToUpperInvariant();
        var transporterCode = dto.TransporterCode.Trim().ToUpperInvariant();

        var tmNo = string.IsNullOrWhiteSpace(dto.TransportMinutesNo)
            ? $"TM{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}"
            : dto.TransportMinutesNo.Trim().ToUpperInvariant();

        if (await db.TransportMinutes.AnyAsync(m => m.OrgId == Org && m.TransportMinutesNo == tmNo))
            throw new InvalidOperationException($"Mã biên bản vận chuyển {tmNo} đã tồn tại.");

        var inputItems = new List<TransportMinutesItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0)
        {
            inputItems.AddRange(dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)));
        }
        else if (dto.Vins != null && dto.Vins.Count > 0)
        {
            inputItems.AddRange(dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => new TransportMinutesItemInputDto(v.Trim().ToUpperInvariant(), dto.DeliveryOrderNo, dto.TransportReqNo, null, null, 10, 15, 2500000m, 0m, "Good", true, null)));
        }

        if (inputItems.Count == 0)
            throw new InvalidOperationException("Cần danh sách xe (Items hoặc Vins) để lập biên bản vận chuyển.");

        var distinctItems = inputItems.DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var now = DateTime.Now;
        var tm = new TransportMinutes
        {
            OrgId = Org,
            TransportMinutesNo = tmNo,
            DealerCode = dealerCode,
            TransporterCode = transporterCode,
            TransporterName = dto.TransporterName?.Trim() ?? transporterCode switch
            {
                "NYK" => "NYK Auto Logistics Việt Nam",
                "TRACO" => "Công ty CP Vận tải Ô tô Traco",
                "VINAFCO" => "Công ty Cổ phần Vinafco Logistics",
                "KCTC" => "KCTC Vina Logistics",
                _ => transporterCode
            },
            TruckPlateNo = dto.TruckPlateNo?.Trim().ToUpperInvariant() ?? "29C-" + new Random().Next(10000, 99999),
            DriverName = dto.DriverName?.Trim() ?? "Nguyễn Văn Lái",
            DriverPhone = dto.DriverPhone?.Trim() ?? "098" + new Random().Next(1000000, 9999999),
            TransportReqNo = dto.TransportReqNo?.Trim().ToUpperInvariant(),
            DeliveryOrderNo = dto.DeliveryOrderNo?.Trim().ToUpperInvariant(),
            TransportMinutesDate = dto.TransportMinutesDate ?? now,
            TotalVehicleCount = distinctItems.Count,
            FilePath = dto.FilePath?.Trim(),
            Status = "Draft",
            CreatedBy = dto.CreatedBy?.Trim() ?? "transporter.dispatcher",
            CreatedAt = now,
            Remark = dto.Remark?.Trim()
        };

        db.TransportMinutes.Add(tm);
        await db.SaveChangesAsync();

        decimal totalFreight = 0;
        decimal totalSurcharge = 0;

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            vehicles.TryGetValue(vin, out var v);

            var freight = item.FreightAmount > 0 ? item.FreightAmount : 2500000m;
            var surcharge = item.Surcharge >= 0 ? item.Surcharge : 0m;
            var lineTotal = freight + surcharge;

            totalFreight += freight;
            totalSurcharge += surcharge;

            db.TransportMinutesLines.Add(new TransportMinutesLine
            {
                OrgId = Org,
                TransportMinutesId = tm.Id,
                TransportMinutesNo = tm.TransportMinutesNo,
                Vin = vin,
                Model = v?.Model ?? "N/A",
                DeliveryOrderNo = item.DeliveryOrderNo?.Trim() ?? tm.DeliveryOrderNo,
                TransportReqNo = item.TransportReqNo?.Trim() ?? tm.TransportReqNo,
                FromStorage = item.FromStorage?.Trim() ?? v?.StorageCode ?? "YARD-MAIN",
                ToStorage = item.ToStorage?.Trim() ?? tm.DealerCode,
                OdoDeparture = item.OdoDeparture >= 0 ? item.OdoDeparture : 10,
                OdoArrival = item.OdoArrival >= 0 ? item.OdoArrival : (item.OdoDeparture >= 0 ? item.OdoDeparture + 5 : 15),
                FreightAmount = freight,
                Surcharge = surcharge,
                TotalAmount = lineTotal,
                CargoCondition = item.CargoCondition?.Trim() ?? "Good",
                IsInspectionPassed = item.IsInspectionPassed,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "TransportMinutesCreated", $"{tm.TransportMinutesNo} Lập biên bản vận chuyển xe lồng: {tm.TransporterCode} ({tm.TruckPlateNo}). Lái xe: {tm.DriverName}");
        }

        tm.TotalFreightAmount = totalFreight;
        tm.TotalSurchargeAmount = totalSurcharge;
        tm.TotalAmount = totalFreight + totalSurcharge;

        await db.SaveChangesAsync();

        return new
        {
            tm.Id,
            tm.TransportMinutesNo,
            tm.DealerCode,
            tm.TransporterCode,
            tm.TransporterName,
            tm.TruckPlateNo,
            tm.DriverName,
            tm.TotalVehicleCount,
            tm.TotalFreightAmount,
            tm.TotalSurchargeAmount,
            tm.TotalAmount,
            status = tm.Status
        };
    }

    public async Task<object> ListTransportMinutesAsync(string? status, string? dealer, string? transporter, string? vin)
    {
        var q = db.TransportMinutes.Where(m => m.OrgId == Org);

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(m => m.Status.ToLower() == status.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(dealer))
            q = q.Where(m => m.DealerCode.Contains(dealer.Trim().ToUpperInvariant()));
        if (!string.IsNullOrWhiteSpace(transporter))
            q = q.Where(m => m.TransporterCode.Contains(transporter.Trim().ToUpperInvariant()));

        if (!string.IsNullOrWhiteSpace(vin))
        {
            var targetVin = vin.Trim().ToUpperInvariant();
            var tmIds = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.Vin.Contains(targetVin))
                .Select(l => l.TransportMinutesId).Distinct().ToListAsync();
            q = q.Where(m => tmIds.Contains(m.Id));
        }

        var items = await q.OrderByDescending(m => m.CreatedAt).Select(m => new
        {
            m.Id,
            m.TransportMinutesNo,
            m.DealerCode,
            m.TransporterCode,
            m.TransporterName,
            m.TruckPlateNo,
            m.DriverName,
            m.DriverPhone,
            m.TransportReqNo,
            m.DeliveryOrderNo,
            m.TransportMinutesDate,
            m.TotalVehicleCount,
            m.TotalFreightAmount,
            m.TotalSurchargeAmount,
            m.TotalAmount,
            m.FilePath,
            m.Status,
            m.CreatedBy,
            m.CreatedAt,
            m.DLApprBy,
            m.DLApprAt,
            m.DLApprNote,
            m.HTCAppr1By,
            m.HTCAppr1At,
            m.HTCAppr2By,
            m.HTCAppr2At,
            m.Remark,
            linesCount = db.TransportMinutesLines.Count(l => l.OrgId == Org && l.TransportMinutesId == m.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetTransportMinutesAsync(string transportMinutesNo)
    {
        transportMinutesNo = transportMinutesNo.Trim().ToUpperInvariant();
        var tm = await db.TransportMinutes.FirstOrDefaultAsync(m => m.OrgId == Org && m.TransportMinutesNo == transportMinutesNo);
        if (tm is null) return null;

        var lines = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.DeliveryOrderNo,
            l.TransportReqNo,
            l.FromStorage,
            l.ToStorage,
            l.OdoDeparture,
            l.OdoArrival,
            odoDistance = Math.Max(0, l.OdoArrival - l.OdoDeparture),
            l.FreightAmount,
            l.Surcharge,
            l.TotalAmount,
            l.CargoCondition,
            l.IsInspectionPassed,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.Color,
                v.ModelYear,
                v.StorageCode,
                v.DealerCode,
                status = v.Status.ToString()
            } : null
        }).ToList();

        return new
        {
            tm.Id,
            tm.TransportMinutesNo,
            tm.DealerCode,
            tm.TransporterCode,
            tm.TransporterName,
            tm.TruckPlateNo,
            tm.DriverName,
            tm.DriverPhone,
            tm.TransportReqNo,
            tm.DeliveryOrderNo,
            tm.TransportMinutesDate,
            tm.TotalVehicleCount,
            tm.TotalFreightAmount,
            tm.TotalSurchargeAmount,
            tm.TotalAmount,
            tm.FilePath,
            tm.Status,
            tm.CreatedBy,
            tm.CreatedAt,
            tm.DLApprBy,
            tm.DLApprAt,
            tm.DLApprNote,
            tm.HTCAppr1By,
            tm.HTCAppr1At,
            tm.HTCAppr2By,
            tm.HTCAppr2At,
            tm.CancelledAt,
            tm.Remark,
            lines = details
        };
    }

    public async Task<object?> TransportMinutesTransitionAsync(string transportMinutesNo, string action, TransportMinutesTransitionDto? dto)
    {
        transportMinutesNo = transportMinutesNo.Trim().ToUpperInvariant();
        var tm = await db.TransportMinutes.FirstOrDefaultAsync(m => m.OrgId == Org && m.TransportMinutesNo == transportMinutesNo);
        if (tm is null) return null;

        var now = DateTime.Now;
        var lines = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (tm.Status != "Draft") return null;
                tm.Status = "Pending";
                foreach (var l in lines) if (l.Status == "Pending") l.Status = "Pending";
                foreach (var v in vehicles) Log(v.Vin, "TransportMinutesSubmitted", $"{transportMinutesNo} Gửi biên bản vận chuyển tới đại lý {tm.DealerCode} và Hãng OEM");
                break;

            case "dl-appr":
            case "dlappr":
            case "dl_appr":
            case "sign-dlr":
                if (tm.Status is not ("Draft" or "Pending")) return null;
                tm.Status = "DLAppr";
                tm.DLApprBy = dto?.User?.Trim() ?? "DealerInspector";
                tm.DLApprAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note)) tm.DLApprNote = dto.Note.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.FilePath)) tm.FilePath = dto.FilePath.Trim();
                foreach (var l in lines) l.Status = "DLAppr";
                foreach (var v in vehicles) Log(v.Vin, "TransportMinutesDLAppr", $"{transportMinutesNo} Đại lý {tm.DealerCode} ký nhận nghiệm thu vận chuyển tại hiện trường. Người ký: {tm.DLApprBy}");
                break;

            case "htc-appr1":
            case "htcappr1":
            case "htc_appr1":
            case "logistics-appr":
                if (tm.Status is not ("Pending" or "DLAppr")) return null;
                tm.Status = "HTCAppr1";
                tm.HTCAppr1By = dto?.User?.Trim() ?? "LogisticsSpecialist";
                tm.HTCAppr1At = now;
                if (!string.IsNullOrWhiteSpace(dto?.FilePath)) tm.FilePath = dto.FilePath.Trim();
                foreach (var v in vehicles) Log(v.Vin, "TransportMinutesHTCAppr1", $"{transportMinutesNo} Bộ phận Vận tải Logistics OEM sơ duyệt đối soát lộ trình & cước phí xe lồng {tm.TransporterCode}. Người duyệt: {tm.HTCAppr1By}");
                break;

            case "htc-appr2":
            case "htcappr2":
            case "htc_appr2":
            case "approve":
            case "complete":
            case "settle":
                if (tm.Status is not ("Pending" or "DLAppr" or "HTCAppr1")) return null;
                tm.Status = "Approved";
                tm.HTCAppr2By = dto?.User?.Trim() ?? "FinanceDirector";
                tm.HTCAppr2At = now;
                if (string.IsNullOrWhiteSpace(tm.HTCAppr1By))
                {
                    tm.HTCAppr1By = tm.HTCAppr2By;
                    tm.HTCAppr1At = now;
                }
                if (string.IsNullOrWhiteSpace(tm.DLApprBy))
                {
                    tm.DLApprBy = "DealerAutoConfirm";
                    tm.DLApprAt = now;
                }
                if (!string.IsNullOrWhiteSpace(dto?.FilePath)) tm.FilePath = dto.FilePath.Trim();

                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles)
                {
                    Log(v.Vin, "TransportMinutesApproved", $"{transportMinutesNo} Hãng OEM duyệt quyết toán cước vận tải cho nhà xe {tm.TransporterCode} ({tm.TruckPlateNo}). Tổng cước: {tm.TotalAmount:N0} VNĐ. Người duyệt: {tm.HTCAppr2By}");
                }
                break;

            case "reject":
                if (tm.Status is "Approved" or "Cancelled") return null;
                tm.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tm.Remark = string.IsNullOrWhiteSpace(tm.Remark) ? dto.Note : $"{tm.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "TransportMinutesRejected", $"{transportMinutesNo} Từ chối biên bản vận chuyển: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (tm.Status is "Approved" or "Cancelled") return null;
                tm.Status = "Cancelled";
                tm.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    tm.Remark = string.IsNullOrWhiteSpace(tm.Remark) ? dto.Note : $"{tm.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "TransportMinutesCancelled", $"{transportMinutesNo} Hủy biên bản vận chuyển: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            tm.TransportMinutesNo,
            tm.DealerCode,
            tm.TransporterCode,
            status = tm.Status,
            tm.DLApprAt,
            tm.HTCAppr1At,
            tm.HTCAppr2At,
            tm.CancelledAt
        };
    }

    public async Task<object?> UpdateTransportMinutesLineAsync(string transportMinutesNo, string vin, UpdateTransportMinutesLineDto dto)
    {
        transportMinutesNo = transportMinutesNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var tm = await db.TransportMinutes.FirstOrDefaultAsync(m => m.OrgId == Org && m.TransportMinutesNo == transportMinutesNo);
        if (tm is null || tm.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var line = await db.TransportMinutesLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.TransportMinutesId == tm.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.OdoDeparture.HasValue && dto.OdoDeparture.Value >= 0) line.OdoDeparture = dto.OdoDeparture.Value;
        if (dto.OdoArrival.HasValue && dto.OdoArrival.Value >= 0) line.OdoArrival = dto.OdoArrival.Value;
        if (dto.FreightAmount.HasValue && dto.FreightAmount.Value >= 0) line.FreightAmount = dto.FreightAmount.Value;
        if (dto.Surcharge.HasValue && dto.Surcharge.Value >= 0) line.Surcharge = dto.Surcharge.Value;
        line.TotalAmount = line.FreightAmount + line.Surcharge;

        if (!string.IsNullOrWhiteSpace(dto.CargoCondition)) line.CargoCondition = dto.CargoCondition.Trim();
        if (dto.IsInspectionPassed.HasValue) line.IsInspectionPassed = dto.IsInspectionPassed.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id).ToListAsync();
        tm.TotalFreightAmount = allLines.Sum(l => l.FreightAmount);
        tm.TotalSurchargeAmount = allLines.Sum(l => l.Surcharge);
        tm.TotalAmount = tm.TotalFreightAmount + tm.TotalSurchargeAmount;

        await db.SaveChangesAsync();

        return new
        {
            tm.TransportMinutesNo,
            line.Vin,
            line.OdoDeparture,
            line.OdoArrival,
            line.FreightAmount,
            line.Surcharge,
            line.TotalAmount,
            line.CargoCondition,
            line.IsInspectionPassed,
            line.Remark,
            totalFreightAmount = tm.TotalFreightAmount,
            totalSurchargeAmount = tm.TotalSurchargeAmount,
            totalAmount = tm.TotalAmount
        };
    }

    public async Task<object?> AddTransportMinutesLinesAsync(string transportMinutesNo, List<TransportMinutesItemInputDto> items)
    {
        transportMinutesNo = transportMinutesNo.Trim().ToUpperInvariant();
        var tm = await db.TransportMinutes.FirstOrDefaultAsync(m => m.OrgId == Org && m.TransportMinutesNo == transportMinutesNo);
        if (tm is null || tm.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var distinctItems = items.Where(i => !string.IsNullOrWhiteSpace(i.Vin))
            .DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        if (distinctItems.Count == 0) return null;

        var existingVins = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id)
            .Select(l => l.Vin).ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin.Trim().ToUpperInvariant())).ToList();
        if (newItems.Count == 0) return null;

        var newVins = newItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && newVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        foreach (var item in newItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            vehicles.TryGetValue(vin, out var v);

            var freight = item.FreightAmount > 0 ? item.FreightAmount : 2500000m;
            var surcharge = item.Surcharge >= 0 ? item.Surcharge : 0m;
            var lineTotal = freight + surcharge;

            db.TransportMinutesLines.Add(new TransportMinutesLine
            {
                OrgId = Org,
                TransportMinutesId = tm.Id,
                TransportMinutesNo = tm.TransportMinutesNo,
                Vin = vin,
                Model = v?.Model ?? "N/A",
                DeliveryOrderNo = item.DeliveryOrderNo?.Trim() ?? tm.DeliveryOrderNo,
                TransportReqNo = item.TransportReqNo?.Trim() ?? tm.TransportReqNo,
                FromStorage = item.FromStorage?.Trim() ?? v?.StorageCode ?? "YARD-MAIN",
                ToStorage = item.ToStorage?.Trim() ?? tm.DealerCode,
                OdoDeparture = item.OdoDeparture >= 0 ? item.OdoDeparture : 10,
                OdoArrival = item.OdoArrival >= 0 ? item.OdoArrival : (item.OdoDeparture >= 0 ? item.OdoDeparture + 5 : 15),
                FreightAmount = freight,
                Surcharge = surcharge,
                TotalAmount = lineTotal,
                CargoCondition = item.CargoCondition?.Trim() ?? "Good",
                IsInspectionPassed = item.IsInspectionPassed,
                Status = tm.Status == "DLAppr" ? "DLAppr" : "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "TransportMinutesLineAdded", $"{transportMinutesNo} Bổ sung xe vào biên bản vận chuyển {tm.TransporterCode}");
        }

        await db.SaveChangesAsync();

        var allLines = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id).ToListAsync();
        tm.TotalVehicleCount = allLines.Count;
        tm.TotalFreightAmount = allLines.Sum(l => l.FreightAmount);
        tm.TotalSurchargeAmount = allLines.Sum(l => l.Surcharge);
        tm.TotalAmount = tm.TotalFreightAmount + tm.TotalSurchargeAmount;
        await db.SaveChangesAsync();

        return new
        {
            tm.TransportMinutesNo,
            addedCount = newItems.Count,
            tm.TotalVehicleCount,
            tm.TotalFreightAmount,
            tm.TotalSurchargeAmount,
            tm.TotalAmount
        };
    }

    public async Task<object?> RemoveTransportMinutesLineAsync(string transportMinutesNo, string vin)
    {
        transportMinutesNo = transportMinutesNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var tm = await db.TransportMinutes.FirstOrDefaultAsync(m => m.OrgId == Org && m.TransportMinutesNo == transportMinutesNo);
        if (tm is null || tm.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var line = await db.TransportMinutesLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.TransportMinutesId == tm.Id && l.Vin == vin);
        if (line is null) return null;

        db.TransportMinutesLines.Remove(line);
        Log(vin, "TransportMinutesLineRemoved", $"{transportMinutesNo} Rút xe khỏi biên bản vận chuyển {tm.TransporterCode}");
        await db.SaveChangesAsync();

        var allLines = await db.TransportMinutesLines.Where(l => l.OrgId == Org && l.TransportMinutesId == tm.Id).ToListAsync();
        tm.TotalVehicleCount = allLines.Count;
        tm.TotalFreightAmount = allLines.Sum(l => l.FreightAmount);
        tm.TotalSurchargeAmount = allLines.Sum(l => l.Surcharge);
        tm.TotalAmount = tm.TotalFreightAmount + tm.TotalSurchargeAmount;
        await db.SaveChangesAsync();

        return new
        {
            tm.TransportMinutesNo,
            vin,
            tm.TotalVehicleCount,
            tm.TotalFreightAmount,
            tm.TotalSurchargeAmount,
            tm.TotalAmount
        };
    }

    // ===== Chứng từ / Phiếu thanh toán tiền mua xe ô tô của Đại lý (BizHTC.Payment.Pmt_Payment / DealerPayment) =====
    public async Task<object> CreateDealerPaymentAsync(CreateDealerPaymentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DealerCode))
            throw new InvalidOperationException("Cần mã đại lý DealerCode nộp tiền mua xe.");

        var dealer = dto.DealerCode.Trim().ToUpperInvariant();

        var inputItems = new List<DealerPaymentItemInputDto>();
        if (dto.Items is { Count: > 0 })
        {
            inputItems.AddRange(dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)));
        }
        else if (dto.Vins is { Count: > 0 })
        {
            inputItems.AddRange(dto.Vins.Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(v => new DealerPaymentItemInputDto(v.Trim().ToUpperInvariant(), null, null, null)));
        }

        if (inputItems.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe VIN trong chứng từ thanh toán tiền xe.");

        var distinctItems = inputItems.DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vins = distinctItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();

        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();
        var missing = vins.Except(vehicles.Select(v => v.Vin)).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("VIN không tồn tại trong hệ thống: " + string.Join(", ", missing));

        var today = DateTime.Today;
        var pmtNo = string.IsNullOrWhiteSpace(dto.PaymentNo)
            ? $"PMT{today:yyyyMMdd}-{(await db.Payments.CountAsync(p => p.OrgId == Org && p.CreatedAt.Date == today) + 1):000}"
            : dto.PaymentNo!.Trim().ToUpperInvariant();

        if (await db.Payments.AnyAsync(p => p.OrgId == Org && p.PaymentNo == pmtNo))
            throw new InvalidOperationException($"Mã phiếu thanh toán {pmtNo} đã tồn tại.");

        var vMap = vehicles.ToDictionary(v => v.Vin);
        var pmtType = string.IsNullOrWhiteSpace(dto.PaymentType) ? "Payment" : dto.PaymentType.Trim();

        decimal totalAmount = 0;
        var lineList = new List<DealerPaymentLine>();

        foreach (var item in distinctItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            var v = vMap[vin];

            decimal amount = 0;
            if (item.Amount.HasValue && item.Amount.Value > 0)
            {
                amount = item.Amount.Value;
            }
            else if (dto.TotalAmount.HasValue && dto.TotalAmount.Value > 0)
            {
                amount = Math.Round(dto.TotalAmount.Value / distinctItems.Count, 0);
            }
            else
            {
                // Mặc định định mức chuẩn xe ô tô
                amount = v.Model.Contains("Accent", StringComparison.OrdinalIgnoreCase) ? 550000000m : 700000000m;
            }

            totalAmount += amount;
            lineList.Add(new DealerPaymentLine
            {
                OrgId = Org,
                PaymentNo = pmtNo,
                Vin = vin,
                Model = v.Model,
                GuaranteeNo = item.GuaranteeNo?.Trim().ToUpperInvariant(),
                Amount = amount,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });
        }

        var pmt = new DealerPayment
        {
            OrgId = Org,
            PaymentNo = pmtNo,
            DealerCode = dealer,
            PaymentType = pmtType,
            BankNameSend = dto.BankNameSend?.Trim(),
            BankNameReceive = dto.BankNameReceive?.Trim() ?? "Vietcombank Sở Giao Dịch Hà Nội",
            BankPaymentNo = dto.BankPaymentNo?.Trim(),
            AccountingRecordNo = null,
            PaymentEndDate = null,
            TotalAmount = totalAmount,
            TotalVehicleCount = distinctItems.Count,
            Status = "Draft",
            Remark = dto.Remark?.Trim(),
            CreatedBy = dto.CreatedBy?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.Payments.Add(pmt);
        await db.SaveChangesAsync();

        foreach (var line in lineList)
        {
            line.DealerPaymentId = pmt.Id;
            db.PaymentLines.Add(line);

            Log(line.Vin, "PaymentCreated", $"{pmtNo} Lập phiếu thanh toán tiền xe ĐL {dealer}. Số tiền: {line.Amount:N0} VNĐ. Ngân hàng: {pmt.BankNameSend ?? "N/A"} - UNC: {pmt.BankPaymentNo ?? "N/A"}");
        }

        await db.SaveChangesAsync();

        return new
        {
            pmt.PaymentNo,
            pmt.DealerCode,
            pmt.PaymentType,
            pmt.BankNameSend,
            pmt.BankNameReceive,
            pmt.BankPaymentNo,
            pmt.TotalAmount,
            pmt.TotalVehicleCount,
            pmt.Status,
            linesCount = lineList.Count
        };
    }

    public async Task<object> ListDealerPaymentsAsync(string? status, string? dealer, string? paymentType, string? paymentNo, string? vin)
    {
        var q = db.Payments.Where(p => p.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(p => p.Status == status);
        if (!string.IsNullOrWhiteSpace(dealer)) { var d = dealer.Trim().ToUpperInvariant(); q = q.Where(p => p.DealerCode == d); }
        if (!string.IsNullOrWhiteSpace(paymentType)) q = q.Where(p => p.PaymentType == paymentType);
        if (!string.IsNullOrWhiteSpace(paymentNo)) { var no = paymentNo.Trim().ToUpperInvariant(); q = q.Where(p => p.PaymentNo == no); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.PaymentLines
                .Where(l => l.OrgId == Org && l.Vin == vv)
                .Select(l => l.PaymentNo)
                .Distinct()
                .ToListAsync();
            q = q.Where(p => matchedNos.Contains(p.PaymentNo));
        }

        var items = await q.OrderByDescending(p => p.Id).Take(500).Select(p => new
        {
            p.PaymentNo,
            p.DealerCode,
            p.PaymentType,
            p.BankNameSend,
            p.BankNameReceive,
            p.BankPaymentNo,
            p.AccountingRecordNo,
            p.PaymentEndDate,
            p.TotalAmount,
            p.TotalVehicleCount,
            p.Status,
            p.CreatedBy,
            p.CreatedAt,
            p.ApprovedBy,
            p.ApprovedAt,
            p.ConfirmBy,
            p.ConfirmedAt,
            p.CancelledAt,
            p.Remark,
            linesCount = db.PaymentLines.Count(l => l.OrgId == Org && l.DealerPaymentId == p.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetDealerPaymentAsync(string paymentNo)
    {
        paymentNo = paymentNo.Trim().ToUpperInvariant();
        var pmt = await db.Payments.FirstOrDefaultAsync(p => p.OrgId == Org && p.PaymentNo == paymentNo);
        if (pmt is null) return null;

        var lines = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.GuaranteeNo,
            l.Amount,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                v.EngineNo,
                v.Color,
                v.ModelYear,
                status = v.Status.ToString(),
                v.StorageCode,
                v.DealerCode,
                v.IsPaid,
                v.PaidAmount,
                v.PaidAt
            } : null
        }).ToList();

        return new
        {
            pmt.Id,
            pmt.PaymentNo,
            pmt.DealerCode,
            pmt.PaymentType,
            pmt.BankNameSend,
            pmt.BankNameReceive,
            pmt.BankPaymentNo,
            pmt.AccountingRecordNo,
            pmt.PaymentEndDate,
            pmt.TotalAmount,
            pmt.TotalVehicleCount,
            pmt.Status,
            pmt.CreatedBy,
            pmt.CreatedAt,
            pmt.ApprovedBy,
            pmt.ApprovedAt,
            pmt.ConfirmBy,
            pmt.ConfirmedAt,
            pmt.CancelledAt,
            pmt.Remark,
            lines = details
        };
    }

    public async Task<object?> DealerPaymentTransitionAsync(string paymentNo, string action, DealerPaymentTransitionDto? dto)
    {
        paymentNo = paymentNo.Trim().ToUpperInvariant();
        var pmt = await db.Payments.FirstOrDefaultAsync(p => p.OrgId == Org && p.PaymentNo == paymentNo);
        if (pmt is null) return null;

        var now = DateTime.Now;
        var lines = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToListAsync();

        switch (action.ToLowerInvariant())
        {
            case "submit":
                if (pmt.Status != "Draft") return null;
                pmt.Status = "Pending";
                foreach (var l in lines) l.Status = "Pending";
                foreach (var v in vehicles) Log(v.Vin, "PaymentSubmitted", $"{paymentNo} Gửi phiếu thanh toán tiền xe đại lý {pmt.DealerCode} sang Kế toán OEM.");
                break;

            case "approve":
                if (pmt.Status is not ("Draft" or "Pending")) return null;
                pmt.Status = "Approved";
                pmt.ApprovedBy = dto?.User?.Trim() ?? "Accountant";
                pmt.ApprovedAt = now;
                foreach (var l in lines) l.Status = "Approved";
                foreach (var v in vehicles) Log(v.Vin, "PaymentApproved", $"{paymentNo} Kế toán công nợ OEM sơ duyệt phiếu thanh toán tiền xe {pmt.DealerCode}. Người duyệt: {pmt.ApprovedBy}");
                break;

            case "confirm":
            case "complete":
            case "finish":
            case "settle":
                if (pmt.Status is not ("Pending" or "Approved")) return null;
                pmt.Status = "Confirmed";
                pmt.ConfirmBy = dto?.User?.Trim() ?? "ChiefAccountant";
                pmt.ConfirmedAt = now;
                pmt.PaymentEndDate = dto?.PaymentEndDate ?? now;
                pmt.AccountingRecordNo = !string.IsNullOrWhiteSpace(dto?.AccountingRecordNo)
                    ? dto.AccountingRecordNo.Trim().ToUpperInvariant()
                    : (!string.IsNullOrWhiteSpace(pmt.AccountingRecordNo) ? pmt.AccountingRecordNo : $"PT-{now:yyyyMMdd}-{(pmt.Id):0000}");

                if (string.IsNullOrWhiteSpace(pmt.ApprovedBy))
                {
                    pmt.ApprovedBy = pmt.ConfirmBy;
                    pmt.ApprovedAt = now;
                }

                foreach (var l in lines) l.Status = "Confirmed";

                var vMap = vehicles.ToDictionary(v => v.Vin);
                foreach (var l in lines)
                {
                    if (vMap.TryGetValue(l.Vin, out var v))
                    {
                        v.IsPaid = true;
                        v.PaidAmount += l.Amount;
                        v.PaidAt = now;

                        // Nếu xe có gắn mã bảo lãnh ngân hàng -> giải tỏa bảo lãnh
                        if (!string.IsNullOrWhiteSpace(l.GuaranteeNo))
                        {
                            var grtLines = await db.GuaranteeLines.Where(gl => gl.OrgId == Org && gl.GuaranteeNo == l.GuaranteeNo && gl.Vin == l.Vin).ToListAsync();
                            foreach (var gl in grtLines) gl.Status = "Settled";
                        }

                        Log(v.Vin, "PaymentConfirmed", $"{paymentNo} Kế toán OEM xác nhận khớp tiền thanh toán: {l.Amount:N0} VNĐ. UNC: {pmt.BankPaymentNo ?? "N/A"}. Hạch toán ERP: {pmt.AccountingRecordNo}. Người xác nhận: {pmt.ConfirmBy}");
                    }
                }
                break;

            case "reject":
                if (pmt.Status is "Confirmed" or "Cancelled") return null;
                pmt.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pmt.Remark = string.IsNullOrWhiteSpace(pmt.Remark) ? dto.Note : $"{pmt.Remark} | Từ chối: {dto.Note}";
                foreach (var l in lines) l.Status = "Rejected";
                foreach (var v in vehicles) Log(v.Vin, "PaymentRejected", $"{paymentNo} Từ chối phiếu thanh toán: {dto?.Note ?? "N/A"}");
                break;

            case "cancel":
                if (pmt.Status is "Confirmed" or "Cancelled") return null;
                pmt.Status = "Cancelled";
                pmt.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note))
                    pmt.Remark = string.IsNullOrWhiteSpace(pmt.Remark) ? dto.Note : $"{pmt.Remark} | Hủy: {dto.Note}";
                foreach (var l in lines) l.Status = "Cancelled";
                foreach (var v in vehicles) Log(v.Vin, "PaymentCancelled", $"{paymentNo} Hủy phiếu thanh toán: {dto?.Note ?? "N/A"}");
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();
        return new
        {
            pmt.PaymentNo,
            pmt.DealerCode,
            pmt.Status,
            pmt.AccountingRecordNo,
            pmt.PaymentEndDate,
            pmt.ApprovedAt,
            pmt.ConfirmedAt,
            pmt.CancelledAt
        };
    }

    public async Task<object?> UpdateDealerPaymentLineAsync(string paymentNo, string vin, UpdateDealerPaymentLineDto dto)
    {
        paymentNo = paymentNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var pmt = await db.Payments.FirstOrDefaultAsync(p => p.OrgId == Org && p.PaymentNo == paymentNo);
        if (pmt is null || pmt.Status is "Confirmed" or "Cancelled" or "Rejected") return null;

        var line = await db.PaymentLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.Amount.HasValue && dto.Amount.Value > 0) line.Amount = dto.Amount.Value;
        if (!string.IsNullOrWhiteSpace(dto.GuaranteeNo)) line.GuaranteeNo = dto.GuaranteeNo.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id).ToListAsync();
        pmt.TotalAmount = allLines.Sum(l => l.Amount);
        pmt.TotalVehicleCount = allLines.Count;

        await db.SaveChangesAsync();

        return new
        {
            pmt.PaymentNo,
            line.Vin,
            line.Amount,
            line.GuaranteeNo,
            line.Remark,
            paymentTotalAmount = pmt.TotalAmount,
            paymentTotalVehicleCount = pmt.TotalVehicleCount
        };
    }

    public async Task<object?> AddDealerPaymentLinesAsync(string paymentNo, List<DealerPaymentItemInputDto> items)
    {
        paymentNo = paymentNo.Trim().ToUpperInvariant();
        var pmt = await db.Payments.FirstOrDefaultAsync(p => p.OrgId == Org && p.PaymentNo == paymentNo);
        if (pmt is null || pmt.Status is "Confirmed" or "Cancelled" or "Rejected") return null;

        var distinctItems = items.Where(i => !string.IsNullOrWhiteSpace(i.Vin))
            .DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        if (distinctItems.Count == 0) return null;

        var existingVins = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id)
            .Select(l => l.Vin).ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin.Trim().ToUpperInvariant())).ToList();
        if (newItems.Count == 0) return null;

        var newVins = newItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && newVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        foreach (var item in newItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            vehicles.TryGetValue(vin, out var v);

            var amount = item.Amount.HasValue && item.Amount.Value > 0
                ? item.Amount.Value
                : (v?.Model.Contains("Accent", StringComparison.OrdinalIgnoreCase) == true ? 550000000m : 700000000m);

            db.PaymentLines.Add(new DealerPaymentLine
            {
                OrgId = Org,
                DealerPaymentId = pmt.Id,
                PaymentNo = pmt.PaymentNo,
                Vin = vin,
                Model = v?.Model ?? "N/A",
                GuaranteeNo = item.GuaranteeNo?.Trim().ToUpperInvariant(),
                Amount = amount,
                Status = pmt.Status == "Approved" ? "Approved" : "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(vin, "PaymentLineAdded", $"{paymentNo} Bổ sung xe vào phiếu thanh toán ĐL {pmt.DealerCode}. Số tiền: {amount:N0} VNĐ");
        }

        await db.SaveChangesAsync();

        var allLines = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id).ToListAsync();
        pmt.TotalAmount = allLines.Sum(l => l.Amount);
        pmt.TotalVehicleCount = allLines.Count;
        await db.SaveChangesAsync();

        return new
        {
            pmt.PaymentNo,
            addedCount = newItems.Count,
            pmt.TotalVehicleCount,
            pmt.TotalAmount
        };
    }

    public async Task<object?> RemoveDealerPaymentLineAsync(string paymentNo, string vin)
    {
        paymentNo = paymentNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var pmt = await db.Payments.FirstOrDefaultAsync(p => p.OrgId == Org && p.PaymentNo == paymentNo);
        if (pmt is null || pmt.Status is "Confirmed" or "Cancelled" or "Rejected") return null;

        var line = await db.PaymentLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id && l.Vin == vin);
        if (line is null) return null;

        db.PaymentLines.Remove(line);
        Log(vin, "PaymentLineRemoved", $"{paymentNo} Rút xe khỏi phiếu thanh toán ĐL {pmt.DealerCode}");
        await db.SaveChangesAsync();

        var allLines = await db.PaymentLines.Where(l => l.OrgId == Org && l.DealerPaymentId == pmt.Id).ToListAsync();
        pmt.TotalAmount = allLines.Sum(l => l.Amount);
        pmt.TotalVehicleCount = allLines.Count;
        await db.SaveChangesAsync();

        return new
        {
            pmt.PaymentNo,
            vin,
            pmt.TotalVehicleCount,
            pmt.TotalAmount
        };
    }

    public async Task<object> CreateStorageMaintenanceAsync(CreateStorageMaintenanceDto dto)
    {
        var mtnNo = string.IsNullOrWhiteSpace(dto.MtnNo)
            ? "MTN-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()
            : dto.MtnNo.Trim().ToUpperInvariant();

        if (await db.StorageMaintenances.AnyAsync(m => m.OrgId == Org && m.MtnNo == mtnNo))
            throw new InvalidOperationException($"Số phiếu bảo dưỡng {mtnNo} đã tồn tại.");

        var distinctVins = new List<string>();
        var itemMap = new Dictionary<string, StorageMaintenanceItemInputDto>(StringComparer.OrdinalIgnoreCase);

        if (dto.Items is { Count: > 0 })
        {
            foreach (var it in dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)))
            {
                var cleanVin = it.Vin.Trim().ToUpperInvariant();
                if (!itemMap.ContainsKey(cleanVin))
                {
                    itemMap[cleanVin] = it;
                    distinctVins.Add(cleanVin);
                }
            }
        }
        else if (dto.Vins is { Count: > 0 })
        {
            distinctVins = dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim().ToUpperInvariant())
                .Distinct().ToList();
        }

        if (distinctVins.Count == 0)
            throw new InvalidOperationException("Cần ít nhất một số khung VIN để lập phiếu bảo dưỡng kho.");

        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && distinctVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var missingVins = distinctVins.Where(vin => !vehicles.ContainsKey(vin)).ToList();
        if (missingVins.Count > 0)
            throw new InvalidOperationException($"Các số khung VIN không tồn tại: {string.Join(", ", missingVins)}");

        var storageCode = !string.IsNullOrWhiteSpace(dto.StorageCode) ? dto.StorageCode.Trim().ToUpperInvariant() : "YARD-A1";

        var mtn = new StorageMaintenance
        {
            OrgId = Org,
            MtnNo = mtnNo,
            StorageCode = storageCode,
            MtnType = string.IsNullOrWhiteSpace(dto.MtnType) ? "Periodic" : dto.MtnType.Trim(),
            PlanDate = dto.PlanDate ?? DateTime.Now,
            TotalVehicleCount = distinctVins.Count,
            PassedVehicleCount = 0,
            FailedVehicleCount = 0,
            Status = "Draft",
            TechnicianCode = dto.TechnicianCode?.Trim(),
            TechnicianName = dto.TechnicianName?.Trim(),
            SupervisorCode = dto.SupervisorCode?.Trim(),
            SupervisorName = dto.SupervisorName?.Trim(),
            Remark = dto.Remark?.Trim(),
            CreatedBy = dto.CreatedBy?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.StorageMaintenances.Add(mtn);

        foreach (var vin in distinctVins)
        {
            vehicles.TryGetValue(vin, out var v);
            itemMap.TryGetValue(vin, out var item);

            var line = new StorageMaintenanceLine
            {
                OrgId = Org,
                StorageMaintenanceId = mtn.Id,
                MtnNo = mtnNo,
                Vin = vin,
                Model = v?.Model,
                StorageCode = item?.StorageCode?.Trim() ?? v?.StorageCode ?? storageCode,
                MtnTimes = v?.StorageMtnTimes ?? 0,
                BatteryVoltage = item?.BatteryVoltage ?? 12.6,
                ChargeBatteryOk = item?.ChargeBatteryOk ?? true,
                EngineStartCheckOk = item?.EngineStartCheckOk ?? true,
                TirePressureCheckOk = item?.TirePressureCheckOk ?? true,
                TireRotationOk = item?.TireRotationOk ?? true,
                FluidLevelsCheckOk = item?.FluidLevelsCheckOk ?? true,
                ElectricalSystemsOk = item?.ElectricalSystemsOk ?? true,
                BodyCleanOk = item?.BodyCleanOk ?? true,
                InspectionResult = "Pending",
                Status = "Pending",
                DefectNotes = item?.DefectNotes?.Trim(),
                Remark = item?.Remark?.Trim()
            };
            db.StorageMaintenanceLines.Add(line);

            Log(vin, "StorageMaintenancePlanned", $"{mtnNo} Lập kế hoạch bảo dưỡng định kỳ xe tồn kho ({mtn.MtnType}) tại bãi {storageCode}");
        }

        await db.SaveChangesAsync();

        return new
        {
            mtn.Id,
            mtn.MtnNo,
            mtn.StorageCode,
            mtn.MtnType,
            mtn.PlanDate,
            mtn.TotalVehicleCount,
            mtn.Status,
            mtn.TechnicianName,
            mtn.SupervisorName,
            mtn.CreatedAt,
            vehicles = distinctVins
        };
    }

    public async Task<object> ListStorageMaintenancesAsync(string? status, string? storageCode, string? mtnType, string? mtnNo, string? vin)
    {
        var q = db.StorageMaintenances.Where(m => m.OrgId == Org);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim();
            q = q.Where(m => m.Status.ToLower() == s.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(storageCode))
        {
            var sc = storageCode.Trim().ToLower();
            q = q.Where(m => m.StorageCode.ToLower().Contains(sc));
        }

        if (!string.IsNullOrWhiteSpace(mtnType))
        {
            var t = mtnType.Trim().ToLower();
            q = q.Where(m => m.MtnType.ToLower() == t);
        }

        if (!string.IsNullOrWhiteSpace(mtnNo))
        {
            var no = mtnNo.Trim().ToLower();
            q = q.Where(m => m.MtnNo.ToLower().Contains(no));
        }

        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vClean = vin.Trim().ToUpperInvariant();
            var mtnNosWithVin = await db.StorageMaintenanceLines
                .Where(l => l.OrgId == Org && l.Vin.Contains(vClean))
                .Select(l => l.MtnNo)
                .Distinct()
                .ToListAsync();
            q = q.Where(m => mtnNosWithVin.Contains(m.MtnNo));
        }

        var list = await q.OrderByDescending(m => m.CreatedAt).ToListAsync();
        var mtnIds = list.Select(m => m.Id).ToList();

        var lineSummaries = await db.StorageMaintenanceLines
            .Where(l => l.OrgId == Org && mtnIds.Contains(l.StorageMaintenanceId))
            .GroupBy(l => l.StorageMaintenanceId)
            .Select(g => new
            {
                MtnId = g.Key,
                Count = g.Count(),
                PassedCount = g.Count(x => x.InspectionResult == "Passed"),
                FailedCount = g.Count(x => x.InspectionResult == "Failed"),
                PendingCount = g.Count(x => x.InspectionResult == "Pending"),
                Vins = g.Select(x => x.Vin).Take(5).ToList()
            })
            .ToDictionaryAsync(g => g.MtnId);

        return list.Select(m =>
        {
            lineSummaries.TryGetValue(m.Id, out var s);
            return new
            {
                m.Id,
                m.MtnNo,
                m.StorageCode,
                m.MtnType,
                m.PlanDate,
                TotalVehicleCount = s?.Count ?? m.TotalVehicleCount,
                PassedVehicleCount = s?.PassedCount ?? m.PassedVehicleCount,
                FailedVehicleCount = s?.FailedCount ?? m.FailedVehicleCount,
                PendingVehicleCount = s?.PendingCount ?? 0,
                m.Status,
                m.TechnicianCode,
                m.TechnicianName,
                m.SupervisorCode,
                m.SupervisorName,
                m.Remark,
                m.CreatedBy,
                m.CreatedAt,
                m.ApprovedBy,
                m.ApprovedAt,
                m.CompletedAt,
                m.CancelledAt,
                sampleVins = s?.Vins ?? new List<string>()
            };
        });
    }

    public async Task<object?> GetStorageMaintenanceAsync(string mtnNo)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null) return null;

        var lines = await db.StorageMaintenanceLines
            .Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id)
            .OrderBy(l => l.Id)
            .ToListAsync();

        return new
        {
            mtn.Id,
            mtn.MtnNo,
            mtn.StorageCode,
            mtn.MtnType,
            mtn.PlanDate,
            mtn.TotalVehicleCount,
            mtn.PassedVehicleCount,
            mtn.FailedVehicleCount,
            mtn.Status,
            mtn.TechnicianCode,
            mtn.TechnicianName,
            mtn.SupervisorCode,
            mtn.SupervisorName,
            mtn.Remark,
            mtn.CreatedBy,
            mtn.CreatedAt,
            mtn.ApprovedBy,
            mtn.ApprovedAt,
            mtn.CompletedAt,
            mtn.CancelledAt,
            lines = lines.Select(l => new
            {
                l.Id,
                l.Vin,
                l.Model,
                l.StorageCode,
                l.MtnTimes,
                l.BatteryVoltage,
                l.ChargeBatteryOk,
                l.EngineStartCheckOk,
                l.TirePressureCheckOk,
                l.TireRotationOk,
                l.FluidLevelsCheckOk,
                l.ElectricalSystemsOk,
                l.BodyCleanOk,
                l.InspectionResult,
                l.MtnDate,
                l.NextMtnDate,
                l.Technician,
                l.DefectNotes,
                l.Status,
                l.Remark
            })
        };
    }

    public async Task<object?> StorageMaintenanceTransitionAsync(string mtnNo, string action, StorageMaintenanceTransitionDto? dto)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        var act = action.Trim().ToLowerInvariant();

        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null) return null;

        var lines = await db.StorageMaintenanceLines
            .Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id)
            .ToListAsync();

        var lineVins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && lineVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var now = DateTime.Now;

        switch (act)
        {
            case "submit" or "request":
                if (mtn.Status != "Draft") return null;
                mtn.Status = "Pending";
                if (!string.IsNullOrWhiteSpace(dto?.Note)) mtn.Remark = (mtn.Remark + " | " + dto.Note).Trim(' ', '|');
                break;

            case "approve":
                if (mtn.Status is not ("Draft" or "Pending")) return null;
                mtn.Status = "InProgress";
                mtn.ApprovedBy = dto?.User ?? dto?.SupervisorName ?? "Supervisor";
                mtn.ApprovedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.SupervisorName)) mtn.SupervisorName = dto.SupervisorName.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.TechnicianName)) mtn.TechnicianName = dto.TechnicianName.Trim();
                foreach (var line in lines)
                {
                    if (line.Status == "Pending") line.Status = "InProgress";
                }
                break;

            case "start" or "in-progress" or "inprogress":
                if (mtn.Status is not ("Draft" or "Pending" or "Approved")) return null;
                mtn.Status = "InProgress";
                if (!string.IsNullOrWhiteSpace(dto?.TechnicianName)) mtn.TechnicianName = dto.TechnicianName.Trim();
                foreach (var line in lines)
                {
                    if (line.Status == "Pending") line.Status = "InProgress";
                }
                break;

            case "complete" or "finish":
                if (mtn.Status is not ("InProgress" or "Pending" or "Draft")) return null;

                foreach (var line in lines)
                {
                    if (line.Status != "Completed")
                    {
                        line.Status = "Completed";
                        if (line.InspectionResult == "Pending") line.InspectionResult = "Passed";
                        line.MtnDate = now;
                        line.NextMtnDate = now.AddDays(30);
                        line.MtnTimes += 1;
                        if (!string.IsNullOrWhiteSpace(dto?.TechnicianName) && string.IsNullOrWhiteSpace(line.Technician))
                            line.Technician = dto.TechnicianName.Trim();
                    }

                    if (vehicles.TryGetValue(line.Vin, out var v))
                    {
                        v.LastStorageMtnDate = line.MtnDate ?? now;
                        v.NextStorageMtnDate = line.NextMtnDate ?? now.AddDays(30);
                        v.StorageMtnTimes = line.MtnTimes;
                    }

                    Log(line.Vin, "StorageMaintenanceCompleted",
                        $"{mtnNo} Hoàn tất bảo dưỡng xe tồn kho (lần {line.MtnTimes}, kết quả: {line.InspectionResult}). Hạn tiếp theo: {line.NextMtnDate:yyyy-MM-dd}");
                }

                mtn.Status = "Completed";
                mtn.CompletedAt = now;
                mtn.PassedVehicleCount = lines.Count(l => l.InspectionResult == "Passed");
                mtn.FailedVehicleCount = lines.Count(l => l.InspectionResult == "Failed");
                break;

            case "reject":
                if (mtn.Status is "Completed" or "Cancelled") return null;
                mtn.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note)) mtn.Remark = (mtn.Remark + " | Từ chối: " + dto.Note).Trim(' ', '|');
                foreach (var line in lines) line.Status = "Rejected";
                break;

            case "cancel":
                if (mtn.Status is "Completed" or "Cancelled") return null;
                mtn.Status = "Cancelled";
                mtn.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note)) mtn.Remark = (mtn.Remark + " | Hủy: " + dto.Note).Trim(' ', '|');
                foreach (var line in lines) line.Status = "Cancelled";
                break;

            default:
                return null;
        }

        mtn.TotalVehicleCount = lines.Count;
        mtn.PassedVehicleCount = lines.Count(l => l.InspectionResult == "Passed");
        mtn.FailedVehicleCount = lines.Count(l => l.InspectionResult == "Failed");

        await db.SaveChangesAsync();

        return new
        {
            mtn.MtnNo,
            mtn.Status,
            mtn.TotalVehicleCount,
            mtn.PassedVehicleCount,
            mtn.FailedVehicleCount,
            mtn.ApprovedBy,
            mtn.ApprovedAt,
            mtn.CompletedAt,
            mtn.CancelledAt,
            action = act
        };
    }

    public async Task<object?> InspectStorageMaintenanceLineAsync(string mtnNo, string vin, InspectStorageMaintenanceLineDto dto)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null || mtn.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var line = await db.StorageMaintenanceLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id && l.Vin == vin);
        if (line is null) return null;

        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);

        var now = DateTime.Now;
        var passed = dto.Passed ?? true;

        line.BatteryVoltage = dto.BatteryVoltage ?? line.BatteryVoltage;
        line.ChargeBatteryOk = dto.ChargeBatteryOk ?? line.ChargeBatteryOk;
        line.EngineStartCheckOk = dto.EngineStartCheckOk ?? line.EngineStartCheckOk;
        line.TirePressureCheckOk = dto.TirePressureCheckOk ?? line.TirePressureCheckOk;
        line.TireRotationOk = dto.TireRotationOk ?? line.TireRotationOk;
        line.FluidLevelsCheckOk = dto.FluidLevelsCheckOk ?? line.FluidLevelsCheckOk;
        line.ElectricalSystemsOk = dto.ElectricalSystemsOk ?? line.ElectricalSystemsOk;
        line.BodyCleanOk = dto.BodyCleanOk ?? line.BodyCleanOk;
        line.InspectionResult = passed ? "Passed" : "Failed";
        line.Status = "Completed";
        line.MtnDate = now;
        line.NextMtnDate = now.AddDays(30);
        if (!string.IsNullOrWhiteSpace(dto.Technician)) line.Technician = dto.Technician.Trim();
        if (!string.IsNullOrWhiteSpace(dto.DefectNotes)) line.DefectNotes = dto.DefectNotes.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        // Increment MtnTimes
        line.MtnTimes = (v?.StorageMtnTimes ?? line.MtnTimes) + 1;

        if (v is not null)
        {
            v.LastStorageMtnDate = line.MtnDate;
            v.NextStorageMtnDate = line.NextMtnDate;
            v.StorageMtnTimes = line.MtnTimes;
        }

        if (mtn.Status is "Draft" or "Pending") mtn.Status = "InProgress";

        var allLines = await db.StorageMaintenanceLines.Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id).ToListAsync();
        mtn.PassedVehicleCount = allLines.Count(l => l.InspectionResult == "Passed");
        mtn.FailedVehicleCount = allLines.Count(l => l.InspectionResult == "Failed");

        Log(vin, "StorageMaintenanceInspected",
            $"{mtnNo} Kiểm tra bảo dưỡng xe: {(passed ? "ĐẠT CHUẨN" : "KHÔNG ĐẠT - " + line.DefectNotes)} (Ắc quy: {line.BatteryVoltage}V, Lần: {line.MtnTimes})");

        await db.SaveChangesAsync();

        return new
        {
            mtn.MtnNo,
            line.Vin,
            line.Model,
            line.StorageCode,
            line.MtnTimes,
            line.BatteryVoltage,
            line.ChargeBatteryOk,
            line.EngineStartCheckOk,
            line.TirePressureCheckOk,
            line.TireRotationOk,
            line.FluidLevelsCheckOk,
            line.ElectricalSystemsOk,
            line.BodyCleanOk,
            line.InspectionResult,
            line.MtnDate,
            line.NextMtnDate,
            line.Technician,
            line.DefectNotes,
            mtnPassedCount = mtn.PassedVehicleCount,
            mtnFailedCount = mtn.FailedVehicleCount
        };
    }

    public async Task<object?> UpdateStorageMaintenanceLineAsync(string mtnNo, string vin, UpdateStorageMaintenanceLineDto dto)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null || mtn.Status is "Cancelled" or "Rejected") return null;

        var line = await db.StorageMaintenanceLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.BatteryVoltage.HasValue) line.BatteryVoltage = dto.BatteryVoltage.Value;
        if (dto.ChargeBatteryOk.HasValue) line.ChargeBatteryOk = dto.ChargeBatteryOk.Value;
        if (dto.EngineStartCheckOk.HasValue) line.EngineStartCheckOk = dto.EngineStartCheckOk.Value;
        if (dto.TirePressureCheckOk.HasValue) line.TirePressureCheckOk = dto.TirePressureCheckOk.Value;
        if (dto.TireRotationOk.HasValue) line.TireRotationOk = dto.TireRotationOk.Value;
        if (dto.FluidLevelsCheckOk.HasValue) line.FluidLevelsCheckOk = dto.FluidLevelsCheckOk.Value;
        if (dto.ElectricalSystemsOk.HasValue) line.ElectricalSystemsOk = dto.ElectricalSystemsOk.Value;
        if (dto.BodyCleanOk.HasValue) line.BodyCleanOk = dto.BodyCleanOk.Value;
        if (!string.IsNullOrWhiteSpace(dto.Technician)) line.Technician = dto.Technician.Trim();
        if (!string.IsNullOrWhiteSpace(dto.DefectNotes)) line.DefectNotes = dto.DefectNotes.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        await db.SaveChangesAsync();

        return new
        {
            mtn.MtnNo,
            line.Vin,
            line.BatteryVoltage,
            line.ChargeBatteryOk,
            line.EngineStartCheckOk,
            line.TirePressureCheckOk,
            line.TireRotationOk,
            line.FluidLevelsCheckOk,
            line.ElectricalSystemsOk,
            line.BodyCleanOk,
            line.InspectionResult,
            line.Technician,
            line.DefectNotes,
            line.Remark
        };
    }

    public async Task<object?> AddStorageMaintenanceLinesAsync(string mtnNo, List<StorageMaintenanceItemInputDto> items)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null || mtn.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var distinctItems = items.Where(i => !string.IsNullOrWhiteSpace(i.Vin))
            .DistinctBy(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        if (distinctItems.Count == 0) return null;

        var existingVins = await db.StorageMaintenanceLines.Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id)
            .Select(l => l.Vin).ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin.Trim().ToUpperInvariant())).ToList();
        if (newItems.Count == 0) return null;

        var newVins = newItems.Select(i => i.Vin.Trim().ToUpperInvariant()).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && newVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        foreach (var item in newItems)
        {
            var vin = item.Vin.Trim().ToUpperInvariant();
            vehicles.TryGetValue(vin, out var v);

            db.StorageMaintenanceLines.Add(new StorageMaintenanceLine
            {
                OrgId = Org,
                StorageMaintenanceId = mtn.Id,
                MtnNo = mtn.MtnNo,
                Vin = vin,
                Model = v?.Model,
                StorageCode = item.StorageCode?.Trim() ?? v?.StorageCode ?? mtn.StorageCode,
                MtnTimes = v?.StorageMtnTimes ?? 0,
                BatteryVoltage = item.BatteryVoltage ?? 12.6,
                ChargeBatteryOk = item.ChargeBatteryOk ?? true,
                EngineStartCheckOk = item.EngineStartCheckOk ?? true,
                TirePressureCheckOk = item.TirePressureCheckOk ?? true,
                TireRotationOk = item.TireRotationOk ?? true,
                FluidLevelsCheckOk = item.FluidLevelsCheckOk ?? true,
                ElectricalSystemsOk = item.ElectricalSystemsOk ?? true,
                BodyCleanOk = item.BodyCleanOk ?? true,
                InspectionResult = "Pending",
                Status = mtn.Status == "InProgress" ? "InProgress" : "Pending",
                DefectNotes = item.DefectNotes?.Trim(),
                Remark = item.Remark?.Trim()
            });

            Log(vin, "StorageMaintenanceLineAdded", $"{mtnNo} Bổ sung xe vào phiếu bảo dưỡng kho {mtn.StorageCode}");
        }

        await db.SaveChangesAsync();

        var allLines = await db.StorageMaintenanceLines.Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id).ToListAsync();
        mtn.TotalVehicleCount = allLines.Count;
        await db.SaveChangesAsync();

        return new
        {
            mtn.MtnNo,
            addedCount = newItems.Count,
            mtn.TotalVehicleCount
        };
    }

    public async Task<object?> RemoveStorageMaintenanceLineAsync(string mtnNo, string vin)
    {
        mtnNo = mtnNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var mtn = await db.StorageMaintenances.FirstOrDefaultAsync(m => m.OrgId == Org && m.MtnNo == mtnNo);
        if (mtn is null || mtn.Status is "Completed" or "Cancelled" or "Rejected") return null;

        var line = await db.StorageMaintenanceLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id && l.Vin == vin);
        if (line is null) return null;

        db.StorageMaintenanceLines.Remove(line);
        Log(vin, "StorageMaintenanceLineRemoved", $"{mtnNo} Rút xe khỏi phiếu bảo dưỡng kho {mtn.StorageCode}");
        await db.SaveChangesAsync();

        var allLines = await db.StorageMaintenanceLines.Where(l => l.OrgId == Org && l.StorageMaintenanceId == mtn.Id).ToListAsync();
        mtn.TotalVehicleCount = allLines.Count;
        mtn.PassedVehicleCount = allLines.Count(l => l.InspectionResult == "Passed");
        mtn.FailedVehicleCount = allLines.Count(l => l.InspectionResult == "Failed");
        await db.SaveChangesAsync();

        return new
        {
            mtn.MtnNo,
            vin,
            mtn.TotalVehicleCount,
            mtn.PassedVehicleCount,
            mtn.FailedVehicleCount
        };
    }

    public async Task<object> GetDueMaintenanceVehiclesAsync(string? storageCode, int dueWithinDays = 7)
    {
        var q = db.Vehicles.Where(v => v.OrgId == Org && v.Status == VehicleStatus.InStock);

        if (!string.IsNullOrWhiteSpace(storageCode))
        {
            var sc = storageCode.Trim().ToLower();
            q = q.Where(v => v.StorageCode != null && v.StorageCode.ToLower().Contains(sc));
        }

        var stockVehicles = await q.OrderBy(v => v.NextStorageMtnDate ?? DateTime.MinValue).ToListAsync();
        var cutoff = DateTime.Now.AddDays(dueWithinDays);
        var now = DateTime.Now;

        var dueVehicles = stockVehicles.Where(v =>
            !v.NextStorageMtnDate.HasValue || v.NextStorageMtnDate.Value <= cutoff
        ).Select(v =>
        {
            string urgency;
            if (!v.NextStorageMtnDate.HasValue)
            {
                urgency = (now - v.CreatedAt).TotalDays > 30 ? "OverdueInitial" : "FirstInspectionPending";
            }
            else if (v.NextStorageMtnDate.Value < now)
            {
                urgency = "Overdue";
            }
            else
            {
                urgency = "DueSoon";
            }

            var daysUntilDue = v.NextStorageMtnDate.HasValue ? (int)(v.NextStorageMtnDate.Value - now).TotalDays : 0;
            var daysSinceLastMtn = v.LastStorageMtnDate.HasValue ? (int)(now - v.LastStorageMtnDate.Value).TotalDays : (int)(now - v.CreatedAt).TotalDays;

            return new
            {
                v.Vin,
                v.Model,
                v.Color,
                v.StorageCode,
                v.StorageMtnTimes,
                v.LastStorageMtnDate,
                v.NextStorageMtnDate,
                daysUntilDue,
                daysSinceLastMtn,
                urgency
            };
        }).ToList();

        return new
        {
            totalDueCount = dueVehicles.Count,
            dueWithinDays,
            vehicles = dueVehicles
        };
    }

    public async Task<object?> GetVehicleMaintenanceHistoryAsync(string vin)
    {
        vin = vin.Trim().ToUpperInvariant();
        var v = await db.Vehicles.FirstOrDefaultAsync(x => x.OrgId == Org && x.Vin == vin);
        if (v is null) return null;

        var historyLines = await db.StorageMaintenanceLines
            .Where(l => l.OrgId == Org && l.Vin == vin)
            .OrderByDescending(l => l.MtnDate ?? (l.Status == "Completed" ? DateTime.MaxValue : DateTime.MinValue))
            .ThenByDescending(l => l.Id)
            .ToListAsync();

        return new
        {
            v.Vin,
            v.Model,
            v.Color,
            v.EngineNo,
            v.Status,
            v.StorageCode,
            v.StorageMtnTimes,
            v.LastStorageMtnDate,
            v.NextStorageMtnDate,
            history = historyLines.Select(h => new
            {
                h.Id,
                h.MtnNo,
                h.StorageCode,
                h.MtnTimes,
                h.BatteryVoltage,
                h.ChargeBatteryOk,
                h.EngineStartCheckOk,
                h.TirePressureCheckOk,
                h.TireRotationOk,
                h.FluidLevelsCheckOk,
                h.ElectricalSystemsOk,
                h.BodyCleanOk,
                h.InspectionResult,
                h.MtnDate,
                h.NextMtnDate,
                h.Technician,
                h.DefectNotes,
                h.Status,
                h.Remark
            })
        };
    }

    // ===== Packing List xuất xưởng nhà máy & Vận đơn nhập khẩu CBU/CKD (BizHTC.Contract.ContractPackingList / CT_PackingList) =====
    public async Task<object> CreatePackingListAsync(CreatePackingListDto dto)
    {
        var today = DateTime.Today;
        var plNo = string.IsNullOrWhiteSpace(dto.PackingListNo)
            ? $"PL{today:yyyyMMdd}-{(await db.PackingLists.CountAsync(p => p.OrgId == Org && p.CreatedAt.Date == today) + 1):000}"
            : dto.PackingListNo.Trim().ToUpperInvariant();

        if (await db.PackingLists.AnyAsync(p => p.OrgId == Org && p.PackingListNo == plNo))
            throw new InvalidOperationException($"Số Packing List {plNo} đã tồn tại.");

        var distinctItems = new List<PackingListItemInputDto>();
        var seenVins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (dto.Items is { Count: > 0 })
        {
            foreach (var it in dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)))
            {
                var cleanVin = it.Vin.Trim().ToUpperInvariant();
                if (cleanVin.Length != 17)
                    throw new InvalidOperationException($"Số khung VIN '{cleanVin}' không hợp lệ (phải đúng 17 ký tự tiêu chuẩn ISO 3779).");

                if (seenVins.Add(cleanVin))
                {
                    distinctItems.Add(it with { Vin = cleanVin, Model = it.Model?.Trim() ?? "Hyundai" });
                }
            }
        }
        else if (dto.Vins is { Count: > 0 })
        {
            foreach (var rawVin in dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v)))
            {
                var cleanVin = rawVin.Trim().ToUpperInvariant();
                if (cleanVin.Length != 17)
                    throw new InvalidOperationException($"Số khung VIN '{cleanVin}' không hợp lệ (phải đúng 17 ký tự tiêu chuẩn ISO 3779).");

                if (seenVins.Add(cleanVin))
                {
                    distinctItems.Add(new PackingListItemInputDto(cleanVin, "Hyundai", null, null, null, 2026, null, null, 600000000m, null));
                }
            }
        }

        if (distinctItems.Count == 0)
            throw new InvalidOperationException("Cần ít nhất một số khung VIN trong danh sách Packing List.");

        var portCode = !string.IsNullOrWhiteSpace(dto.PortCode) ? dto.PortCode.Trim().ToUpperInvariant() : "NHA_MAY_NINH_BINH";
        var totalAmount = distinctItems.Sum(i => i.UnitPrice);

        var pl = new PackingList
        {
            OrgId = Org,
            PackingListNo = plNo,
            ContractNo = dto.ContractNo?.Trim(),
            LCNo = dto.LCNo?.Trim(),
            PortCode = portCode,
            VesselName = dto.VesselName?.Trim(),
            VoyageNo = dto.VoyageNo?.Trim(),
            ShippingDateStart = dto.ShippingDateStart ?? DateTime.Now,
            ShippingDateEndExpected = dto.ShippingDateEndExpected ?? DateTime.Now.AddDays(7),
            ShippingDateEnd = null,
            TotalQuantity = distinctItems.Count,
            TotalAmount = totalAmount,
            Status = "Draft",
            Remark = dto.Remark?.Trim(),
            CreatedBy = dto.CreatedBy?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.PackingLists.Add(pl);
        await db.SaveChangesAsync();

        foreach (var item in distinctItems)
        {
            var line = new PackingListLine
            {
                OrgId = Org,
                PackingListId = pl.Id,
                PackingListNo = plNo,
                Vin = item.Vin,
                Model = string.IsNullOrWhiteSpace(item.Model) ? "Hyundai" : item.Model.Trim(),
                SpecCode = item.SpecCode?.Trim(),
                EngineNo = item.EngineNo?.Trim(),
                Color = item.Color?.Trim(),
                ModelYear = item.ModelYear ?? 2026,
                KeyNo = item.KeyNo?.Trim(),
                ProductionDate = item.ProductionDate ?? DateTime.Now,
                UnitPrice = item.UnitPrice,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            };
            db.PackingListLines.Add(line);

            Log(item.Vin, "PackingListCreated", $"{plNo} Đóng gói xuất xưởng / Lập vận đơn Packing List tại {portCode}. Tàu/Đoàn xe: {pl.VesselName ?? "N/A"}. Đơn giá: {item.UnitPrice:N0} VNĐ");
        }

        await db.SaveChangesAsync();

        return new
        {
            pl.PackingListNo,
            pl.ContractNo,
            pl.LCNo,
            pl.PortCode,
            pl.VesselName,
            pl.VoyageNo,
            pl.ShippingDateStart,
            pl.ShippingDateEndExpected,
            pl.TotalQuantity,
            pl.TotalAmount,
            pl.Status,
            linesCount = distinctItems.Count
        };
    }

    public async Task<object> ListPackingListsAsync(string? status, string? portCode, string? contractNo, string? vesselName, string? vin)
    {
        var q = db.PackingLists.Where(p => p.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(p => p.Status == status);
        if (!string.IsNullOrWhiteSpace(portCode)) { var p = portCode.Trim().ToUpperInvariant(); q = q.Where(x => x.PortCode == p); }
        if (!string.IsNullOrWhiteSpace(contractNo)) { var c = contractNo.Trim().ToUpperInvariant(); q = q.Where(x => x.ContractNo != null && x.ContractNo.Contains(c)); }
        if (!string.IsNullOrWhiteSpace(vesselName)) { var v = vesselName.Trim().ToLower(); q = q.Where(x => x.VesselName != null && x.VesselName.ToLower().Contains(v)); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.PackingListLines
                .Where(l => l.OrgId == Org && l.Vin == vv)
                .Select(l => l.PackingListNo)
                .Distinct()
                .ToListAsync();
            q = q.Where(p => matchedNos.Contains(p.PackingListNo));
        }

        var items = await q.OrderByDescending(p => p.Id).Take(500).Select(p => new
        {
            p.PackingListNo,
            p.ContractNo,
            p.LCNo,
            p.PortCode,
            p.VesselName,
            p.VoyageNo,
            p.ShippingDateStart,
            p.ShippingDateEndExpected,
            p.ShippingDateEnd,
            p.TotalQuantity,
            p.TotalAmount,
            p.Status,
            p.Remark,
            p.CreatedBy,
            p.CreatedAt,
            p.ApprovedBy,
            p.ApprovedAt,
            p.CancelledAt
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetPackingListAsync(string packingListNo)
    {
        var plNo = packingListNo.Trim().ToUpperInvariant();
        var pl = await db.PackingLists.FirstOrDefaultAsync(p => p.OrgId == Org && p.PackingListNo == plNo);
        if (pl is null) return null;

        var lines = await db.PackingListLines
            .Where(l => l.OrgId == Org && l.PackingListId == pl.Id)
            .OrderBy(l => l.Id)
            .ToListAsync();

        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.PackingListNo,
            l.Vin,
            l.Model,
            l.SpecCode,
            l.EngineNo,
            l.Color,
            l.ModelYear,
            l.KeyNo,
            l.ProductionDate,
            l.UnitPrice,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new { status = v.Status.ToString(), v.StorageCode, v.DealerCode, v.OwnerName, v.PlateNo } : null
        }).ToList();

        return new
        {
            pl.PackingListNo,
            pl.ContractNo,
            pl.LCNo,
            pl.PortCode,
            pl.VesselName,
            pl.VoyageNo,
            pl.ShippingDateStart,
            pl.ShippingDateEndExpected,
            pl.ShippingDateEnd,
            pl.TotalQuantity,
            pl.TotalAmount,
            pl.Status,
            pl.Remark,
            pl.CreatedBy,
            pl.CreatedAt,
            pl.ApprovedBy,
            pl.ApprovedAt,
            pl.CancelledAt,
            lines = details
        };
    }

    public async Task<object?> PackingListTransitionAsync(string packingListNo, string action, PackingListTransitionDto? dto)
    {
        var plNo = packingListNo.Trim().ToUpperInvariant();
        var pl = await db.PackingLists.FirstOrDefaultAsync(p => p.OrgId == Org && p.PackingListNo == plNo);
        if (pl is null) return null;

        var lines = await db.PackingListLines.Where(l => l.OrgId == Org && l.PackingListId == pl.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var act = action.Trim().ToLowerInvariant();
        var now = DateTime.Now;

        switch (act)
        {
            case "submit" or "request":
                if (pl.Status != "Draft") return null;
                pl.Status = "Submitted";
                if (!string.IsNullOrWhiteSpace(dto?.Note)) pl.Remark = (pl.Remark + " | " + dto.Note).Trim(' ', '|');
                break;

            case "approve" or "confirm":
                if (pl.Status is not ("Draft" or "Submitted")) return null;
                pl.Status = "Approved";
                pl.ApprovedBy = dto?.ApprovedBy ?? dto?.User ?? "Admin";
                pl.ApprovedAt = now;
                pl.ShippingDateEnd = dto?.ShippingDateEnd ?? now;

                foreach (var line in lines)
                {
                    line.Status = "Approved";

                    if (!vehicles.TryGetValue(line.Vin, out var v))
                    {
                        v = new Vehicle
                        {
                            OrgId = Org,
                            Vin = line.Vin,
                            Model = line.Model,
                            EngineNo = line.EngineNo,
                            Color = line.Color,
                            ModelYear = line.ModelYear,
                            StorageCode = pl.PortCode,
                            PackingListNo = pl.PackingListNo,
                            Status = VehicleStatus.InStock,
                            WarrantyMonths = 36,
                            CreatedAt = now
                        };
                        db.Vehicles.Add(v);
                        vehicles[line.Vin] = v;
                    }
                    else
                    {
                        v.StorageCode ??= pl.PortCode;
                        v.PackingListNo ??= pl.PackingListNo;
                        v.EngineNo ??= line.EngineNo;
                        v.Color ??= line.Color;
                        v.ModelYear ??= line.ModelYear;
                    }

                    Log(line.Vin, "PackingListApproved",
                        $"{plNo} Phê duyệt nhập kho OEM từ Packing List (Cảng/Kho: {pl.PortCode}, Tàu/Chuyến: {pl.VesselName ?? "N/A"}). Trạng thái xe: InStock");
                }
                break;

            case "reject":
                if (pl.Status is "Approved" or "Cancelled") return null;
                pl.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note)) pl.Remark = (pl.Remark + " | Từ chối: " + dto.Note).Trim(' ', '|');
                foreach (var line in lines) line.Status = "Rejected";
                break;

            case "cancel":
                if (pl.Status is "Approved" or "Cancelled") return null;
                pl.Status = "Cancelled";
                pl.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note)) pl.Remark = (pl.Remark + " | Hủy: " + dto.Note).Trim(' ', '|');
                foreach (var line in lines) line.Status = "Cancelled";
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();

        return new
        {
            pl.PackingListNo,
            pl.Status,
            pl.TotalQuantity,
            pl.TotalAmount,
            pl.ApprovedBy,
            pl.ApprovedAt,
            pl.ShippingDateEnd,
            pl.CancelledAt,
            action = act
        };
    }

    public async Task<object?> UpdatePackingListLineAsync(string packingListNo, string vin, UpdatePackingListLineDto dto)
    {
        var plNo = packingListNo.Trim().ToUpperInvariant();
        var vVin = vin.Trim().ToUpperInvariant();

        var pl = await db.PackingLists.FirstOrDefaultAsync(p => p.OrgId == Org && p.PackingListNo == plNo);
        if (pl is null || pl.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var line = await db.PackingListLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.PackingListId == pl.Id && l.Vin == vVin);
        if (line is null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Model)) line.Model = dto.Model.Trim();
        if (!string.IsNullOrWhiteSpace(dto.SpecCode)) line.SpecCode = dto.SpecCode.Trim();
        if (!string.IsNullOrWhiteSpace(dto.EngineNo)) line.EngineNo = dto.EngineNo.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Color)) line.Color = dto.Color.Trim();
        if (dto.ModelYear.HasValue && dto.ModelYear.Value > 1990) line.ModelYear = dto.ModelYear.Value;
        if (!string.IsNullOrWhiteSpace(dto.KeyNo)) line.KeyNo = dto.KeyNo.Trim();
        if (dto.ProductionDate.HasValue) line.ProductionDate = dto.ProductionDate.Value;
        if (dto.UnitPrice.HasValue && dto.UnitPrice.Value >= 0) line.UnitPrice = dto.UnitPrice.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.PackingListLines.Where(l => l.OrgId == Org && l.PackingListId == pl.Id).ToListAsync();
        pl.TotalAmount = allLines.Sum(l => l.UnitPrice);

        await db.SaveChangesAsync();

        return new
        {
            pl.PackingListNo,
            line.Vin,
            line.Model,
            line.SpecCode,
            line.EngineNo,
            line.Color,
            line.ModelYear,
            line.KeyNo,
            line.UnitPrice,
            line.Remark,
            totalAmount = pl.TotalAmount
        };
    }

    public async Task<object?> AddPackingListLinesAsync(string packingListNo, List<PackingListItemInputDto> items)
    {
        var plNo = packingListNo.Trim().ToUpperInvariant();
        var pl = await db.PackingLists.FirstOrDefaultAsync(p => p.OrgId == Org && p.PackingListNo == plNo);
        if (pl is null || pl.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var distinctItems = new List<PackingListItemInputDto>();
        var seenVins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var it in items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)))
        {
            var cleanVin = it.Vin.Trim().ToUpperInvariant();
            if (cleanVin.Length != 17)
                throw new InvalidOperationException($"Số khung VIN '{cleanVin}' không hợp lệ (phải đúng 17 ký tự tiêu chuẩn ISO 3779).");

            if (seenVins.Add(cleanVin))
            {
                distinctItems.Add(it with { Vin = cleanVin, Model = it.Model?.Trim() ?? "Hyundai" });
            }
        }

        if (distinctItems.Count == 0) return null;

        var existingVins = await db.PackingListLines
            .Where(l => l.OrgId == Org && l.PackingListId == pl.Id)
            .Select(l => l.Vin)
            .ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin)).ToList();
        if (newItems.Count == 0) return null;

        foreach (var item in newItems)
        {
            db.PackingListLines.Add(new PackingListLine
            {
                OrgId = Org,
                PackingListId = pl.Id,
                PackingListNo = pl.PackingListNo,
                Vin = item.Vin,
                Model = string.IsNullOrWhiteSpace(item.Model) ? "Hyundai" : item.Model.Trim(),
                SpecCode = item.SpecCode?.Trim(),
                EngineNo = item.EngineNo?.Trim(),
                Color = item.Color?.Trim(),
                ModelYear = item.ModelYear ?? 2026,
                KeyNo = item.KeyNo?.Trim(),
                ProductionDate = item.ProductionDate ?? DateTime.Now,
                UnitPrice = item.UnitPrice,
                Status = "Pending",
                Remark = item.Remark?.Trim()
            });

            Log(item.Vin, "PackingListLineAdded", $"{plNo} Bổ sung xe vào Packing List {pl.PortCode}");
        }

        await db.SaveChangesAsync();

        var allLines = await db.PackingListLines.Where(l => l.OrgId == Org && l.PackingListId == pl.Id).ToListAsync();
        pl.TotalQuantity = allLines.Count;
        pl.TotalAmount = allLines.Sum(l => l.UnitPrice);
        await db.SaveChangesAsync();

        return new
        {
            pl.PackingListNo,
            addedCount = newItems.Count,
            pl.TotalQuantity,
            pl.TotalAmount
        };
    }

    public async Task<object?> RemovePackingListLineAsync(string packingListNo, string vin)
    {
        var plNo = packingListNo.Trim().ToUpperInvariant();
        var vVin = vin.Trim().ToUpperInvariant();

        var pl = await db.PackingLists.FirstOrDefaultAsync(p => p.OrgId == Org && p.PackingListNo == plNo);
        if (pl is null || pl.Status is "Approved" or "Cancelled" or "Rejected") return null;

        var line = await db.PackingListLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.PackingListId == pl.Id && l.Vin == vVin);
        if (line is null) return null;

        db.PackingListLines.Remove(line);
        Log(vVin, "PackingListLineRemoved", $"{plNo} Rút xe khỏi Packing List {pl.PortCode}");
        await db.SaveChangesAsync();

        var allLines = await db.PackingListLines.Where(l => l.OrgId == Org && l.PackingListId == pl.Id).ToListAsync();
        pl.TotalQuantity = allLines.Count;
        pl.TotalAmount = allLines.Sum(l => l.UnitPrice);
        await db.SaveChangesAsync();

        return new
        {
            pl.PackingListNo,
            vin = vVin,
            pl.TotalQuantity,
            pl.TotalAmount
        };
    }

    // ===== Tờ khai Hải quan nhập khẩu CBU/CKD & Nộp thuế thông quan xe (BizHTC.Contract.ContractDeclaration & CT_TKHQ / CT_Declaration) =====
    public async Task<object> CreateCustomsDeclarationAsync(CreateCustomsDeclarationDto dto)
    {
        var items = new List<CustomsDeclarationItemInputDto>();
        if (dto.Items != null && dto.Items.Count > 0)
        {
            items.AddRange(dto.Items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)));
        }
        else if (dto.Vins != null && dto.Vins.Count > 0)
        {
            items.AddRange(dto.Vins.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => new CustomsDeclarationItemInputDto(v)));
        }

        if (items.Count == 0)
            throw new InvalidOperationException("Cần ít nhất 1 xe (VIN) trong Tờ khai hải quan.");

        var distinctItems = new List<CustomsDeclarationItemInputDto>();
        var seenVins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var it in items)
        {
            var cleanVin = it.Vin.Trim().ToUpperInvariant();
            if (cleanVin.Length != 17)
                throw new InvalidOperationException($"Số khung VIN '{cleanVin}' không hợp lệ (phải đúng 17 ký tự tiêu chuẩn ISO 3779).");

            if (seenVins.Add(cleanVin))
            {
                distinctItems.Add(it with { Vin = cleanVin });
            }
        }

        var declNo = string.IsNullOrWhiteSpace(dto.DeclarationNo)
            ? "TKHQ" + DateTime.Now.ToString("yyMMddHHmmss")
            : dto.DeclarationNo.Trim().ToUpperInvariant();

        if (await db.CustomsDeclarations.AnyAsync(d => d.OrgId == Org && d.DeclarationNo == declNo))
            throw new InvalidOperationException($"Số tờ khai Hải quan '{declNo}' đã tồn tại trong hệ thống.");

        var portCode = string.IsNullOrWhiteSpace(dto.PortCode) ? "HQ_HAI_PHONG" : dto.PortCode.Trim().ToUpperInvariant();
        var portName = !string.IsNullOrWhiteSpace(dto.PortName)
            ? dto.PortName.Trim()
            : portCode switch
            {
                "HQ_HAI_PHONG" => "Chi cục Hải quan Cửa khẩu Cảng Hải Phòng",
                "HQ_CAT_LAI" => "Chi cục Hải quan Cửa khẩu Cảng Sài Gòn KV1 (Cát Lái)",
                "HQ_CAI_MEP" => "Chi cục Hải quan Cửa khẩu Cảng Cái Mép - Bà Rịa Vũng Tàu",
                "HQ_NOI_BAI" => "Chi cục Hải quan Sân bay Quốc tế Nội Bài",
                "HQ_HUU_NGHI" => "Chi cục Hải quan Cửa khẩu Quốc tế Hữu Nghị",
                "NHA_MAY_NINH_BINH" => "Hải quan Quản lý Đầu tư Gia công Ninh Bình (Nhà máy HTMV)",
                _ => portCode
            };

        var declType = string.IsNullOrWhiteSpace(dto.DeclarationType) ? "CBU" : dto.DeclarationType.Trim().ToUpperInvariant();
        var openDate = dto.OpenDate ?? DateTime.Now;

        // Load existing vehicles if already registered in system
        var vins = distinctItems.Select(i => i.Vin).ToList();
        var existingVehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        decimal totalTaxValue = 0;
        decimal totalImportTax = 0;
        decimal totalExciseTax = 0;
        decimal totalVatTax = 0;

        var lineList = new List<CustomsDeclarationLine>();
        foreach (var it in distinctItems)
        {
            existingVehicles.TryGetValue(it.Vin, out var v);

            var model = !string.IsNullOrWhiteSpace(it.Model) ? it.Model.Trim() : (v?.Model ?? "Hyundai");
            var specCode = it.SpecCode?.Trim() ?? v?.Model;
            var engineNo = it.EngineNo?.Trim() ?? v?.EngineNo;
            var color = it.Color?.Trim() ?? v?.Color;
            var modelYear = it.ModelYear ?? v?.ModelYear ?? 2026;
            var plNo = it.PackingListNo?.Trim() ?? v?.PackingListNo;

            // Thuế hải quan ô tô:
            // 1. Trị giá tính thuế CIF/FOB (TaxValue)
            var taxValue = it.TaxValue.HasValue && it.TaxValue.Value > 0
                ? it.TaxValue.Value
                : (v != null && v.PaidAmount > 0 ? v.PaidAmount : 500000000m);

            // 2. Thuế nhập khẩu: TaxValue * (ImportTaxRate / 100)
            var impRate = it.ImportTaxRate.HasValue && it.ImportTaxRate.Value >= 0 ? it.ImportTaxRate.Value : (declType == "CKD" ? 10m : 50m);
            var impTax = it.ImportTax.HasValue && it.ImportTax.Value >= 0
                ? it.ImportTax.Value
                : Math.Round(taxValue * (impRate / 100m), 0);

            // 3. Thuế tiêu thụ đặc biệt (TTĐB): (TaxValue + ImportTax) * (ExciseTaxRate / 100)
            var excRate = it.ExciseTaxRate.HasValue && it.ExciseTaxRate.Value >= 0 ? it.ExciseTaxRate.Value : 35m;
            var excTax = it.ExciseTax.HasValue && it.ExciseTax.Value >= 0
                ? it.ExciseTax.Value
                : Math.Round((taxValue + impTax) * (excRate / 100m), 0);

            // 4. Thuế GTGT (VAT): (TaxValue + ImportTax + ExciseTax) * (VatRate / 100)
            var vatRate = it.VatRate.HasValue && it.VatRate.Value >= 0 ? it.VatRate.Value : 10m;
            var vatTax = it.VatTax.HasValue && it.VatTax.Value >= 0
                ? it.VatTax.Value
                : Math.Round((taxValue + impTax + excTax) * (vatRate / 100m), 0);

            var lineTotalTax = impTax + excTax + vatTax;

            totalTaxValue += taxValue;
            totalImportTax += impTax;
            totalExciseTax += excTax;
            totalVatTax += vatTax;

            lineList.Add(new CustomsDeclarationLine
            {
                OrgId = Org,
                DeclarationNo = declNo,
                Vin = it.Vin,
                Model = model,
                SpecCode = specCode,
                EngineNo = engineNo,
                Color = color,
                ModelYear = modelYear,
                PackingListNo = plNo,
                TaxValue = taxValue,
                ImportTaxRate = impRate,
                ImportTax = impTax,
                ExciseTaxRate = excRate,
                ExciseTax = excTax,
                VatRate = vatRate,
                VatTax = vatTax,
                TotalTax = lineTotalTax,
                Status = "Pending",
                Remark = it.Remark?.Trim()
            });
        }

        var totalTaxAmount = totalImportTax + totalExciseTax + totalVatTax;

        var cd = new CustomsDeclaration
        {
            OrgId = Org,
            DeclarationNo = declNo,
            PortCode = portCode,
            PortName = portName,
            ContractNo = dto.ContractNo?.Trim(),
            LCNo = dto.LCNo?.Trim(),
            BillOfLadingNo = dto.BillOfLadingNo?.Trim(),
            DeclarationType = declType,
            OpenDate = openDate,
            CustomsOfficer = dto.CustomsOfficer?.Trim(),
            DeclarantName = dto.DeclarantName?.Trim() ?? "Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam",
            TotalVehicleCount = distinctItems.Count,
            TotalTaxValue = totalTaxValue,
            ImportTaxAmount = totalImportTax,
            ExciseTaxAmount = totalExciseTax,
            VatAmount = totalVatTax,
            TotalTaxAmount = totalTaxAmount,
            Status = "Draft",
            Remark = dto.Remark?.Trim(),
            CreatedBy = dto.CreatedBy?.Trim(),
            CreatedAt = DateTime.Now
        };
        db.CustomsDeclarations.Add(cd);
        await db.SaveChangesAsync();

        foreach (var line in lineList)
        {
            line.CustomsDeclarationId = cd.Id;
            db.CustomsDeclarationLines.Add(line);

            Log(line.Vin, "CustomsDeclarationCreated",
                $"{declNo} Lập tờ khai hải quan {portCode} ({declType}). Trị giá: {line.TaxValue:N0} VNĐ, Thuế NK: {line.ImportTax:N0} VNĐ, TTĐB: {line.ExciseTax:N0} VNĐ, VAT: {line.VatTax:N0} VNĐ (Tổng thuế: {line.TotalTax:N0} VNĐ)");
        }

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            cd.PortCode,
            cd.PortName,
            cd.ContractNo,
            cd.LCNo,
            cd.BillOfLadingNo,
            cd.DeclarationType,
            cd.OpenDate,
            cd.TotalVehicleCount,
            cd.TotalTaxValue,
            cd.ImportTaxAmount,
            cd.ExciseTaxAmount,
            cd.VatAmount,
            cd.TotalTaxAmount,
            cd.Status,
            linesCount = lineList.Count
        };
    }

    public async Task<object> ListCustomsDeclarationsAsync(string? status, string? portCode, string? contractNo, string? declarationType, string? declarationNo, string? vin)
    {
        var q = db.CustomsDeclarations.Where(d => d.OrgId == Org);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(d => d.Status == status);
        if (!string.IsNullOrWhiteSpace(portCode)) { var p = portCode.Trim().ToUpperInvariant(); q = q.Where(d => d.PortCode == p); }
        if (!string.IsNullOrWhiteSpace(contractNo)) { var c = contractNo.Trim().ToUpperInvariant(); q = q.Where(d => d.ContractNo != null && d.ContractNo.ToUpper().Contains(c)); }
        if (!string.IsNullOrWhiteSpace(declarationType)) { var dt = declarationType.Trim().ToUpperInvariant(); q = q.Where(d => d.DeclarationType == dt); }
        if (!string.IsNullOrWhiteSpace(declarationNo)) { var dn = declarationNo.Trim().ToUpperInvariant(); q = q.Where(d => d.DeclarationNo.ToUpper().Contains(dn)); }
        if (!string.IsNullOrWhiteSpace(vin))
        {
            var vv = vin.Trim().ToUpperInvariant();
            var matchedNos = await db.CustomsDeclarationLines
                .Where(l => l.OrgId == Org && l.Vin == vv)
                .Select(l => l.DeclarationNo)
                .Distinct()
                .ToListAsync();
            q = q.Where(d => matchedNos.Contains(d.DeclarationNo));
        }

        var items = await q.OrderByDescending(d => d.Id).Take(500).Select(d => new
        {
            d.DeclarationNo,
            d.PortCode,
            d.PortName,
            d.ContractNo,
            d.LCNo,
            d.BillOfLadingNo,
            d.DeclarationType,
            d.OpenDate,
            d.TaxPaymentDate,
            d.ClearanceDate,
            d.CustomsOfficer,
            d.DeclarantName,
            d.TotalVehicleCount,
            d.TotalTaxValue,
            d.ImportTaxAmount,
            d.ExciseTaxAmount,
            d.VatAmount,
            d.TotalTaxAmount,
            d.Status,
            d.CreatedBy,
            d.CreatedAt,
            d.ApprovedBy,
            d.ApprovedAt,
            d.ClearedBy,
            d.ClearedAt,
            d.CancelledAt,
            d.Remark,
            linesCount = db.CustomsDeclarationLines.Count(l => l.OrgId == Org && l.CustomsDeclarationId == d.Id)
        }).ToListAsync();

        return new { count = items.Count, items };
    }

    public async Task<object?> GetCustomsDeclarationAsync(string declarationNo)
    {
        declarationNo = declarationNo.Trim().ToUpperInvariant();
        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declarationNo);
        if (cd is null) return null;

        var lines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        var vins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && vins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        var details = lines.Select(l => new
        {
            l.Id,
            l.Vin,
            l.Model,
            l.SpecCode,
            l.EngineNo,
            l.Color,
            l.ModelYear,
            l.PackingListNo,
            l.TaxValue,
            l.ImportTaxRate,
            l.ImportTax,
            l.ExciseTaxRate,
            l.ExciseTax,
            l.VatRate,
            l.VatTax,
            l.TotalTax,
            l.TaxPaymentDate,
            l.ClearanceDate,
            l.Status,
            l.Remark,
            vehicle = vehicles.TryGetValue(l.Vin, out var v) ? new
            {
                status = v.Status.ToString(),
                v.StorageCode,
                v.DealerCode,
                v.IsCustomsCleared,
                v.CustomsClearanceDate,
                v.DeclarationNo,
                v.TaxPaymentDate
            } : null
        }).ToList();

        return new
        {
            cd.DeclarationNo,
            cd.PortCode,
            cd.PortName,
            cd.ContractNo,
            cd.LCNo,
            cd.BillOfLadingNo,
            cd.DeclarationType,
            cd.OpenDate,
            cd.TaxPaymentDate,
            cd.ClearanceDate,
            cd.CustomsOfficer,
            cd.DeclarantName,
            cd.TotalVehicleCount,
            cd.TotalTaxValue,
            cd.ImportTaxAmount,
            cd.ExciseTaxAmount,
            cd.VatAmount,
            cd.TotalTaxAmount,
            cd.Status,
            cd.Remark,
            cd.CreatedBy,
            cd.CreatedAt,
            cd.ApprovedBy,
            cd.ApprovedAt,
            cd.ClearedBy,
            cd.ClearedAt,
            cd.CancelledAt,
            lines = details
        };
    }

    public async Task<object?> CustomsDeclarationTransitionAsync(string declarationNo, string action, CustomsDeclarationTransitionDto? dto)
    {
        declarationNo = declarationNo.Trim().ToUpperInvariant();
        var act = action.Trim().ToLowerInvariant();

        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declarationNo);
        if (cd is null) return null;

        var lines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        var lineVins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && lineVins.Contains(v.Vin)).ToListAsync();
        var vMap = vehicles.ToDictionary(v => v.Vin);

        var now = DateTime.Now;

        switch (act)
        {
            case "submit" or "register":
                if (cd.Status != "Draft") return null;
                cd.Status = "Registered";
                cd.ApprovedBy = dto?.User ?? "Declarant";
                cd.ApprovedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.CustomsOfficer)) cd.CustomsOfficer = dto.CustomsOfficer.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.Note)) cd.Remark = (cd.Remark + " | " + dto.Note).Trim(' ', '|');

                foreach (var line in lines)
                {
                    if (line.Status == "Pending") line.Status = "Registered";
                    Log(line.Vin, "CustomsRegistered", $"{declarationNo} Đã đăng ký tờ khai tại {cd.PortCode} ({cd.PortName}). Cán bộ tiếp nhận: {cd.CustomsOfficer ?? "N/A"}");
                }
                break;

            case "pay-tax" or "paytax":
                if (cd.Status is not ("Draft" or "Registered")) return null;
                cd.Status = "TaxPaid";
                cd.TaxPaymentDate = dto?.TaxPaymentDate ?? now;
                if (!string.IsNullOrWhiteSpace(dto?.Note)) cd.Remark = (cd.Remark + " | Nộp thuế: " + dto.Note).Trim(' ', '|');

                foreach (var line in lines)
                {
                    line.Status = "TaxPaid";
                    line.TaxPaymentDate = cd.TaxPaymentDate;

                    if (vMap.TryGetValue(line.Vin, out var v))
                    {
                        v.TaxPaymentDate = cd.TaxPaymentDate;
                    }

                    Log(line.Vin, "CustomsTaxPaid", $"{declarationNo} Hoàn thành nộp thuế hải quan {line.TotalTax:N0} VNĐ vào NSNN ngày {cd.TaxPaymentDate:yyyy-MM-dd}");
                }
                break;

            case "clear" or "clearance" or "approve":
                if (cd.Status is not ("Draft" or "Registered" or "TaxPaid")) return null;

                cd.Status = "Cleared";
                cd.TaxPaymentDate ??= dto?.TaxPaymentDate ?? now;
                cd.ClearanceDate = dto?.ClearanceDate ?? now;
                cd.ClearedBy = dto?.User ?? dto?.CustomsOfficer ?? "CustomsAuthority";
                cd.ClearedAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.CustomsOfficer)) cd.CustomsOfficer = dto.CustomsOfficer.Trim();
                if (!string.IsNullOrWhiteSpace(dto?.Note)) cd.Remark = (cd.Remark + " | Thông quan: " + dto.Note).Trim(' ', '|');

                foreach (var line in lines)
                {
                    line.Status = "Cleared";
                    line.TaxPaymentDate ??= cd.TaxPaymentDate;
                    line.ClearanceDate = cd.ClearanceDate;

                    if (vMap.TryGetValue(line.Vin, out var v))
                    {
                        v.DeclarationNo = cd.DeclarationNo;
                        v.TaxPaymentDate = cd.TaxPaymentDate;
                        v.IsCustomsCleared = true;
                        v.CustomsClearanceDate = cd.ClearanceDate;
                    }

                    Log(line.Vin, "CustomsCleared",
                        $"{declarationNo} Đã hoàn tất thủ tục thông quan hải quan tại {cd.PortCode}. Đủ điều kiện xuất xưởng/phân bổ và lưu thông.");
                }
                break;

            case "reject":
                if (cd.Status is "Cleared" or "Cancelled") return null;
                cd.Status = "Rejected";
                if (!string.IsNullOrWhiteSpace(dto?.Note)) cd.Remark = (cd.Remark + " | Từ chối: " + dto.Note).Trim(' ', '|');

                foreach (var line in lines)
                {
                    line.Status = "Rejected";
                    Log(line.Vin, "CustomsRejected", $"{declarationNo} Chi cục HQ từ chối thông quan: {dto?.Note ?? "N/A"}");
                }
                break;

            case "cancel":
                if (cd.Status is "Cleared" or "Cancelled") return null;
                cd.Status = "Cancelled";
                cd.CancelledAt = now;
                if (!string.IsNullOrWhiteSpace(dto?.Note)) cd.Remark = (cd.Remark + " | Hủy: " + dto.Note).Trim(' ', '|');

                foreach (var line in lines)
                {
                    line.Status = "Cancelled";
                    Log(line.Vin, "CustomsCancelled", $"{declarationNo} Hủy tờ khai hải quan: {dto?.Note ?? "N/A"}");
                }
                break;

            default:
                return null;
        }

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            cd.PortCode,
            cd.DeclarationType,
            status = cd.Status,
            cd.TaxPaymentDate,
            cd.ClearanceDate,
            cd.ApprovedAt,
            cd.ClearedAt,
            cd.CancelledAt,
            linesCount = lines.Count
        };
    }

    public async Task<object?> UpdateCustomsDeclarationLineAsync(string declarationNo, string vin, UpdateCustomsDeclarationLineDto dto)
    {
        declarationNo = declarationNo.Trim().ToUpperInvariant();
        vin = vin.Trim().ToUpperInvariant();

        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declarationNo);
        if (cd is null || cd.Status is "Cleared" or "Cancelled" or "Rejected") return null;

        var line = await db.CustomsDeclarationLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id && l.Vin == vin);
        if (line is null) return null;

        if (dto.TaxValue.HasValue && dto.TaxValue.Value >= 0) line.TaxValue = dto.TaxValue.Value;
        if (dto.ImportTaxRate.HasValue && dto.ImportTaxRate.Value >= 0) line.ImportTaxRate = dto.ImportTaxRate.Value;
        if (dto.ExciseTaxRate.HasValue && dto.ExciseTaxRate.Value >= 0) line.ExciseTaxRate = dto.ExciseTaxRate.Value;
        if (dto.VatRate.HasValue && dto.VatRate.Value >= 0) line.VatRate = dto.VatRate.Value;

        // Recalculate tax amounts
        line.ImportTax = dto.ImportTax.HasValue && dto.ImportTax.Value >= 0
            ? dto.ImportTax.Value
            : Math.Round(line.TaxValue * (line.ImportTaxRate / 100m), 0);

        line.ExciseTax = dto.ExciseTax.HasValue && dto.ExciseTax.Value >= 0
            ? dto.ExciseTax.Value
            : Math.Round((line.TaxValue + line.ImportTax) * (line.ExciseTaxRate / 100m), 0);

        line.VatTax = dto.VatTax.HasValue && dto.VatTax.Value >= 0
            ? dto.VatTax.Value
            : Math.Round((line.TaxValue + line.ImportTax + line.ExciseTax) * (line.VatRate / 100m), 0);

        line.TotalTax = line.ImportTax + line.ExciseTax + line.VatTax;

        if (dto.TaxPaymentDate.HasValue) line.TaxPaymentDate = dto.TaxPaymentDate.Value;
        if (dto.ClearanceDate.HasValue) line.ClearanceDate = dto.ClearanceDate.Value;
        if (!string.IsNullOrWhiteSpace(dto.Remark)) line.Remark = dto.Remark.Trim();

        var allLines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        cd.TotalTaxValue = allLines.Sum(l => l.TaxValue);
        cd.ImportTaxAmount = allLines.Sum(l => l.ImportTax);
        cd.ExciseTaxAmount = allLines.Sum(l => l.ExciseTax);
        cd.VatAmount = allLines.Sum(l => l.VatTax);
        cd.TotalTaxAmount = allLines.Sum(l => l.TotalTax);

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            line.Vin,
            line.TaxValue,
            line.ImportTaxRate,
            line.ImportTax,
            line.ExciseTaxRate,
            line.ExciseTax,
            line.VatRate,
            line.VatTax,
            line.TotalTax,
            line.TaxPaymentDate,
            line.ClearanceDate,
            line.Remark,
            declarationTotalTaxAmount = cd.TotalTaxAmount
        };
    }

    public async Task<object?> AddCustomsDeclarationLinesAsync(string declarationNo, List<CustomsDeclarationItemInputDto> items)
    {
        var declNo = declarationNo.Trim().ToUpperInvariant();
        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declNo);
        if (cd is null || cd.Status is "Cleared" or "Cancelled" or "Rejected") return null;

        var distinctItems = new List<CustomsDeclarationItemInputDto>();
        var seenVins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var it in items.Where(i => !string.IsNullOrWhiteSpace(i.Vin)))
        {
            var cleanVin = it.Vin.Trim().ToUpperInvariant();
            if (cleanVin.Length != 17)
                throw new InvalidOperationException($"Số khung VIN '{cleanVin}' không hợp lệ (phải đúng 17 ký tự tiêu chuẩn ISO 3779).");

            if (seenVins.Add(cleanVin))
            {
                distinctItems.Add(it with { Vin = cleanVin });
            }
        }

        if (distinctItems.Count == 0) return null;

        var existingVins = await db.CustomsDeclarationLines
            .Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id)
            .Select(l => l.Vin)
            .ToListAsync();

        var newItems = distinctItems.Where(i => !existingVins.Contains(i.Vin)).ToList();
        if (newItems.Count == 0) return null;

        var newVins = newItems.Select(i => i.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && newVins.Contains(v.Vin)).ToDictionaryAsync(v => v.Vin);

        foreach (var it in newItems)
        {
            vehicles.TryGetValue(it.Vin, out var v);

            var model = !string.IsNullOrWhiteSpace(it.Model) ? it.Model.Trim() : (v?.Model ?? "Hyundai");
            var specCode = it.SpecCode?.Trim() ?? v?.Model;
            var engineNo = it.EngineNo?.Trim() ?? v?.EngineNo;
            var color = it.Color?.Trim() ?? v?.Color;
            var modelYear = it.ModelYear ?? v?.ModelYear ?? 2026;
            var plNo = it.PackingListNo?.Trim() ?? v?.PackingListNo;

            var taxValue = it.TaxValue.HasValue && it.TaxValue.Value > 0
                ? it.TaxValue.Value
                : (v != null && v.PaidAmount > 0 ? v.PaidAmount : 500000000m);

            var impRate = it.ImportTaxRate.HasValue && it.ImportTaxRate.Value >= 0 ? it.ImportTaxRate.Value : (cd.DeclarationType == "CKD" ? 10m : 50m);
            var impTax = it.ImportTax.HasValue && it.ImportTax.Value >= 0
                ? it.ImportTax.Value
                : Math.Round(taxValue * (impRate / 100m), 0);

            var excRate = it.ExciseTaxRate.HasValue && it.ExciseTaxRate.Value >= 0 ? it.ExciseTaxRate.Value : 35m;
            var excTax = it.ExciseTax.HasValue && it.ExciseTax.Value >= 0
                ? it.ExciseTax.Value
                : Math.Round((taxValue + impTax) * (excRate / 100m), 0);

            var vatRate = it.VatRate.HasValue && it.VatRate.Value >= 0 ? it.VatRate.Value : 10m;
            var vatTax = it.VatTax.HasValue && it.VatTax.Value >= 0
                ? it.VatTax.Value
                : Math.Round((taxValue + impTax + excTax) * (vatRate / 100m), 0);

            var lineTotalTax = impTax + excTax + vatTax;

            db.CustomsDeclarationLines.Add(new CustomsDeclarationLine
            {
                OrgId = Org,
                CustomsDeclarationId = cd.Id,
                DeclarationNo = cd.DeclarationNo,
                Vin = it.Vin,
                Model = model,
                SpecCode = specCode,
                EngineNo = engineNo,
                Color = color,
                ModelYear = modelYear,
                PackingListNo = plNo,
                TaxValue = taxValue,
                ImportTaxRate = impRate,
                ImportTax = impTax,
                ExciseTaxRate = excRate,
                ExciseTax = excTax,
                VatRate = vatRate,
                VatTax = vatTax,
                TotalTax = lineTotalTax,
                Status = cd.Status == "Registered" ? "Registered" : "Pending",
                Remark = it.Remark?.Trim()
            });

            Log(it.Vin, "CustomsDeclarationLineAdded", $"{declNo} Bổ sung xe vào tờ khai hải quan {cd.PortCode}");
        }

        await db.SaveChangesAsync();

        var allLines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        cd.TotalVehicleCount = allLines.Count;
        cd.TotalTaxValue = allLines.Sum(l => l.TaxValue);
        cd.ImportTaxAmount = allLines.Sum(l => l.ImportTax);
        cd.ExciseTaxAmount = allLines.Sum(l => l.ExciseTax);
        cd.VatAmount = allLines.Sum(l => l.VatTax);
        cd.TotalTaxAmount = allLines.Sum(l => l.TotalTax);

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            addedCount = newItems.Count,
            cd.TotalVehicleCount,
            cd.TotalTaxValue,
            cd.TotalTaxAmount
        };
    }

    public async Task<object?> RemoveCustomsDeclarationLineAsync(string declarationNo, string vin)
    {
        var declNo = declarationNo.Trim().ToUpperInvariant();
        var vVin = vin.Trim().ToUpperInvariant();

        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declNo);
        if (cd is null || cd.Status is "Cleared" or "Cancelled" or "Rejected") return null;

        var line = await db.CustomsDeclarationLines.FirstOrDefaultAsync(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id && l.Vin == vVin);
        if (line is null) return null;

        db.CustomsDeclarationLines.Remove(line);
        Log(vVin, "CustomsDeclarationLineRemoved", $"{declNo} Rút xe khỏi tờ khai hải quan {cd.PortCode}");
        await db.SaveChangesAsync();

        var allLines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        cd.TotalVehicleCount = allLines.Count;
        cd.TotalTaxValue = allLines.Sum(l => l.TaxValue);
        cd.ImportTaxAmount = allLines.Sum(l => l.ImportTax);
        cd.ExciseTaxAmount = allLines.Sum(l => l.ExciseTax);
        cd.VatAmount = allLines.Sum(l => l.VatTax);
        cd.TotalTaxAmount = allLines.Sum(l => l.TotalTax);

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            vin = vVin,
            cd.TotalVehicleCount,
            cd.TotalTaxAmount
        };
    }

    public async Task<object?> UpdateCustomsDeclarationTaxPaymentAsync(string declarationNo, UpdateCustomsDeclarationTaxPaymentDto dto)
    {
        declarationNo = declarationNo.Trim().ToUpperInvariant();
        var cd = await db.CustomsDeclarations.FirstOrDefaultAsync(d => d.OrgId == Org && d.DeclarationNo == declarationNo);
        if (cd is null || cd.Status is "Cancelled" or "Rejected") return null;

        var payDate = dto.TaxPaymentDate ?? DateTime.Now;
        cd.TaxPaymentDate = payDate;
        if (cd.Status is "Draft" or "Registered") cd.Status = "TaxPaid";
        if (!string.IsNullOrWhiteSpace(dto.Note)) cd.Remark = (cd.Remark + " | Cập nhật nộp thuế: " + dto.Note).Trim(' ', '|');

        var lines = await db.CustomsDeclarationLines.Where(l => l.OrgId == Org && l.CustomsDeclarationId == cd.Id).ToListAsync();
        var lineVins = lines.Select(l => l.Vin).ToList();
        var vehicles = await db.Vehicles.Where(v => v.OrgId == Org && lineVins.Contains(v.Vin)).ToListAsync();

        foreach (var line in lines)
        {
            line.TaxPaymentDate = payDate;
            if (line.Status is "Pending" or "Registered") line.Status = "TaxPaid";
        }

        foreach (var v in vehicles)
        {
            v.TaxPaymentDate = payDate;
            v.DeclarationNo = cd.DeclarationNo;
            Log(v.Vin, "CustomsTaxPaymentUpdated", $"{declarationNo} Xác nhận nộp thuế hải quan hoàn tất ngày {payDate:yyyy-MM-dd}");
        }

        await db.SaveChangesAsync();

        return new
        {
            cd.DeclarationNo,
            cd.Status,
            cd.TaxPaymentDate,
            updatedVehiclesCount = vehicles.Count,
            cd.TotalTaxAmount
        };
    }
}
