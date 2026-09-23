using Microsoft.EntityFrameworkCore;
using MiniVehicle.Models;

namespace MiniVehicle.Data;

public static class Seeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        await MigrateAsync(db);   // bảng thêm sau (Recalls) — EnsureCreated KHÔNG tạo trên DB Postgres đã tồn tại
        if (!await db.Orgs.AnyAsync(o => o.Id == TenantContext.DefaultOrgId))
            db.Orgs.Add(new Org { Id = TenantContext.DefaultOrgId, Name = "Demo OEM", ApiKey = "demo-vehicle" });
        if (!await db.Vehicles.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            db.Vehicles.AddRange(
                new Vehicle { OrgId = org, Vin = "DEMOVIN00000001", Model = "Accent 1.4 AT", Color = "Trắng", ModelYear = 2026, EngineNo = "G4LC0001", StorageCode = "YARD-A1", Status = VehicleStatus.InStock },
                new Vehicle { OrgId = org, Vin = "DEMOVIN00000002", Model = "Creta 1.5 Cao cấp", Color = "Đen", ModelYear = 2026, EngineNo = "G4FL0002", StorageCode = "YARD-B2", Status = VehicleStatus.InStock },
                new Vehicle { OrgId = org, Vin = "DEMOVIN00000003", Model = "Hyundai New Porter H150", Color = "Trắng", ModelYear = 2026, EngineNo = "D4CB0003", StorageCode = "BODY-SHOP-01", Status = VehicleStatus.InStock, TypeCB = "1", LoaiThung = "ThungBat", CBReqNo = "CBR-202603-001" },
                new Vehicle { OrgId = org, Vin = "DEMOVIN00000004", Model = "Hyundai Mighty EX8 GTL", Color = "Xanh", ModelYear = 2026, EngineNo = "D4GA0004", StorageCode = "BODY-SHOP-01", Status = VehicleStatus.InStock, TypeCB = "1", LoaiThung = "ThungLanh", CBReqNo = "CBR-202603-001" }
            );
        }
        if (!await db.SalesOrders.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var so = new SalesOrder
            {
                OrgId = org,
                SOCode = "SO202603-001",
                SOType = "Normal",
                DealerCode = "DLR-HN01",
                OrderMonth = "2026-03",
                ProductionMonth = "2026-03",
                ExpectedMonth = "2026-04",
                TotalOrderQty = 5,
                TotalApprovedQty = 5,
                TotalAllocatedQty = 0,
                TotalAmount = 3750000000m,
                Status = "Approved",
                CreatedBy = "dealer.hn01",
                ApprovedBy1 = "plan.manager",
                ApprovedAt1 = DateTime.Now.AddDays(-2),
                ApprovedBy2 = "director.sale",
                ApprovedAt2 = DateTime.Now.AddDays(-1),
                Remark = "Đơn đặt hàng xe kế hoạch tháng 03/2026 đại lý Hà Nội 01"
            };
            db.SalesOrders.Add(so);
            await db.SaveChangesAsync();

            db.SalesOrderLines.AddRange(
                new SalesOrderLine
                {
                    OrgId = org,
                    SalesOrderId = so.Id,
                    SOCode = so.SOCode,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    Color = "Trắng",
                    OrderQty = 3,
                    ApprovedQty = 3,
                    AllocatedQty = 0,
                    UnitPrice = 550000000m,
                    TotalAmount = 1650000000m,
                    Status = "Approved",
                    Remark = "Giao đợt 1"
                },
                new SalesOrderLine
                {
                    OrgId = org,
                    SalesOrderId = so.Id,
                    SOCode = so.SOCode,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    Color = "Đen",
                    OrderQty = 2,
                    ApprovedQty = 2,
                    AllocatedQty = 0,
                    UnitPrice = 700000000m,
                    TotalAmount = 1400000000m,
                    Status = "Approved",
                    Remark = "Giao đợt 1"
                }
            );
        }
        if (!await db.DealerDeals.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var deal = new DealerDeal
            {
                OrgId = org,
                DealNo = "DEAL202603-001",
                DealNoUser = "HDBL-2026/03/HN01-001",
                DealerCode = "DLR-HN01",
                CustomerCode = "CUST-001",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerType = "Individual",
                IdNo = "001090012345",
                Address = "Số 12 phố Trần Duy Hưng, Cầu Giấy, Hà Nội",
                SalesManCode = "TVBH-01",
                SalesManName = "Hoàng Minh",
                SalesType = "Retail",
                PaymentType = "Cash",
                TotalAmount = 550000000m,
                DiscountAmount = 15000000m,
                FinalAmount = 535000000m,
                DepositAmount = 50000000m,
                DealDate = DateTime.Now.AddDays(-1),
                Status = "Approved",
                CreatedBy = "dealer.hn01",
                ApprovedBy = "sales.lead",
                ApprovedAt = DateTime.Now.AddHours(-12),
                Remark = "Hợp đồng bán lẻ xe Accent cho khách hàng cá nhân"
            };
            db.DealerDeals.Add(deal);
            await db.SaveChangesAsync();

            db.DealerDealLines.Add(new DealerDealLine
            {
                OrgId = org,
                DealerDealId = deal.Id,
                DealNo = deal.DealNo,
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                Color = "Trắng",
                UnitPrice = 550000000m,
                Discount = 15000000m,
                Price = 535000000m,
                PlateNo = "30K-988.66",
                SBHOnlineNo = "SBH-2026-HN001",
                DeliveryOdoKm = 12,
                WarrantyMonths = 36,
                Status = "Approved",
                Remark = "Khách chọn nhận xe tại showroom"
            });
        }
        if (!await db.Guarantees.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var grt = new PaymentGuarantee
            {
                OrgId = org,
                GuaranteeNo = "GRT202603-001",
                BankGuaranteeNo = "BL-VCB-2026/089",
                BankCode = "VCB",
                BankName = "Ngân hàng Ngoại thương Việt Nam (Vietcombank)",
                DealerCode = "DLR-HN01",
                DateOpen = DateTime.Now.AddDays(-5),
                DateExpired = DateTime.Now.AddDays(25),
                Term = 30,
                TermActual = 30,
                TotalAmount = 1250000000m,
                TotalVehicleCount = 2,
                Status = "Approved",
                CreatedBy = "dealer.hn01",
                ApprovedBy = "chief.accountant",
                ApprovedAt = DateTime.Now.AddDays(-4),
                Remark = "Thư bảo lãnh thanh toán mua lô xe Accent & Creta đại lý Hà Nội 01"
            };
            db.Guarantees.Add(grt);
            await db.SaveChangesAsync();

            db.GuaranteeLines.AddRange(
                new PaymentGuaranteeLine
                {
                    OrgId = org,
                    PaymentGuaranteeId = grt.Id,
                    GuaranteeNo = grt.GuaranteeNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    GuaranteePercent = 100,
                    GuaranteeValue = 550000000m,
                    DateStart = DateTime.Now.AddDays(-5),
                    DateWarning = DateTime.Now.AddDays(18),
                    DateExpired = DateTime.Now.AddDays(25),
                    Status = "Approved",
                    Remark = "Bảo lãnh 100% giá trị xe"
                },
                new PaymentGuaranteeLine
                {
                    OrgId = org,
                    PaymentGuaranteeId = grt.Id,
                    GuaranteeNo = grt.GuaranteeNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    GuaranteePercent = 100,
                    GuaranteeValue = 700000000m,
                    DateStart = DateTime.Now.AddDays(-5),
                    DateWarning = DateTime.Now.AddDays(18),
                    DateExpired = DateTime.Now.AddDays(25),
                    Status = "Approved",
                    Remark = "Bảo lãnh 100% giá trị xe"
                }
            );
        }
        if (!await db.DealerContracts.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ctr = new DealerContract
            {
                OrgId = org,
                ContractNo = "CTR202603-001",
                ContractNoUser = "HĐMB-2026/03/HN01-01",
                DealerCode = "DLR-HN01",
                SOCode = "SO202603-001",
                ContractType = "Wholesale",
                ContractDate = DateTime.Now.AddDays(-3),
                DeliveryDeadline = DateTime.Now.AddDays(15),
                PaymentTermDays = 30,
                TotalQuantity = 2,
                TotalAmount = 1250000000m,
                DiscountAmount = 20000000m,
                FinalAmount = 1230000000m,
                DepositAmount = 100000000m,
                Status = "Approved",
                CreatedBy = "sales.dealer",
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-2),
                Remark = "Hợp đồng mua bán lô xe Accent và Creta cho đại lý Hà Nội 01"
            };
            db.DealerContracts.Add(ctr);
            await db.SaveChangesAsync();

            db.DealerContractLines.AddRange(
                new DealerContractLine
                {
                    OrgId = org,
                    DealerContractId = ctr.Id,
                    ContractNo = ctr.ContractNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    Color = "Trắng",
                    UnitPrice = 550000000m,
                    Discount = 10000000m,
                    ActualPrice = 540000000m,
                    Status = "Approved",
                    Remark = "Bán buôn theo chính sách tháng 3"
                },
                new DealerContractLine
                {
                    OrgId = org,
                    DealerContractId = ctr.Id,
                    ContractNo = ctr.ContractNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    Color = "Đen",
                    UnitPrice = 700000000m,
                    Discount = 10000000m,
                    ActualPrice = 690000000m,
                    Status = "Approved",
                    Remark = "Bán buôn theo chính sách tháng 3"
                }
            );
        }
        if (!await db.PaymentDiscounts.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var pd = new PaymentDiscount
            {
                OrgId = org,
                PaymentDiscountNo = "20260315-001/DNCK/DLR-HN01",
                DealerCode = "DLR-HN01",
                DateEndFrom = DateTime.Now.AddDays(-10),
                DateEndTo = DateTime.Now.AddDays(20),
                TotalVehicleCount = 2,
                TotalPaymentAmount = 1250000000m,
                TotalDiscountAmount = 18750000m,
                DiscountPercent = 1.5m,
                PenaltyPercent = 0m,
                PmtDctStatus = "Approved",
                DlrSignStatus = "Signed",
                HTCSignStatus = "Approved",
                CreatedBy = "dealer.hn01",
                HTCApprBy = "sales.director",
                HTCApprAt = DateTime.Now.AddDays(-2),
                DlrSignBy = "dlr.director",
                DlrSignAt = DateTime.Now.AddDays(-1),
                Remark = "Đề nghị chiết khấu thanh toán trả sớm lô xe bảo lãnh VCB tháng 3"
            };
            db.PaymentDiscounts.Add(pd);
            await db.SaveChangesAsync();

            db.PaymentDiscountLines.AddRange(
                new PaymentDiscountLine
                {
                    OrgId = org,
                    PaymentDiscountId = pd.Id,
                    PaymentDiscountNo = pd.PaymentDiscountNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    GuaranteeNo = "GRT202603-001",
                    UnitPrice = 550000000m,
                    PaymentEndDatePhase1 = DateTime.Now.AddDays(-5),
                    AmountPhase1 = 550000000m,
                    DiscountDateNumberPhase1 = 15,
                    DiscountPercentPhase1 = 1.5m,
                    DiscountPricePhase1 = 8250000m,
                    TotalAmount = 550000000m,
                    TotalDiscountPrice = 8250000m,
                    PG_DateEnd = DateTime.Now.AddDays(25),
                    Status = "Approved",
                    Remark = "Thanh toán sớm đợt 1 hưởng 1.5% chiết khấu"
                },
                new PaymentDiscountLine
                {
                    OrgId = org,
                    PaymentDiscountId = pd.Id,
                    PaymentDiscountNo = pd.PaymentDiscountNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    GuaranteeNo = "GRT202603-001",
                    UnitPrice = 700000000m,
                    PaymentEndDatePhase1 = DateTime.Now.AddDays(-5),
                    AmountPhase1 = 700000000m,
                    DiscountDateNumberPhase1 = 15,
                    DiscountPercentPhase1 = 1.5m,
                    DiscountPricePhase1 = 10500000m,
                    TotalAmount = 700000000m,
                    TotalDiscountPrice = 10500000m,
                    PG_DateEnd = DateTime.Now.AddDays(25),
                    Status = "Approved",
                    Remark = "Thanh toán sớm đợt 1 hưởng 1.5% chiết khấu"
                }
            );
        }
        if (!await db.InsuranceRequests.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ins = new InsuranceRequest
            {
                OrgId = org,
                InsReqNo = "INS202603-001",
                InsCompanyCode = "PVI",
                InsCompanyName = "Tổng công ty Bảo hiểm PVI",
                InsTypeCode = "CARGO",
                PolicyNo = "PVI-CARGO-2026-0089",
                EffectiveDate = DateTime.Now.AddDays(-3),
                ExpireDate = DateTime.Now.AddDays(27),
                TotalVehicleCount = 2,
                TotalInsuredValue = 1250000000m,
                PremiumRate = 0.15m,
                TotalPremiumAmount = 1875000m,
                Status = "Approved",
                CreatedBy = "logistic.planner",
                ApprovedBy = "ins.manager",
                ApprovedAt = DateTime.Now.AddDays(-2),
                Remark = "Bảo hiểm vận chuyển hàng hóa đường bộ cho lô xe Accent & Creta chuyển kho đại lý Hà Nội 01"
            };
            db.InsuranceRequests.Add(ins);
            await db.SaveChangesAsync();

            db.InsuranceRequestLines.AddRange(
                new InsuranceRequestLine
                {
                    OrgId = org,
                    InsuranceRequestId = ins.Id,
                    InsReqNo = ins.InsReqNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    InsuredValue = 550000000m,
                    PremiumRate = 0.15m,
                    PremiumAmount = 825000m,
                    InsuranceDays = 30,
                    FromStorage = "YARD-A1",
                    ToStorage = "DLR-HN01",
                    CertificateNo = "GCN-PVI-202603-000001",
                    Status = "Approved",
                    Remark = "Bảo hiểm vận chuyển xe lồng hoàn tất"
                },
                new InsuranceRequestLine
                {
                    OrgId = org,
                    InsuranceRequestId = ins.Id,
                    InsReqNo = ins.InsReqNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    InsuredValue = 700000000m,
                    PremiumRate = 0.15m,
                    PremiumAmount = 1050000m,
                    InsuranceDays = 30,
                    FromStorage = "YARD-B2",
                    ToStorage = "DLR-HN01",
                    CertificateNo = "GCN-PVI-202603-000002",
                    Status = "Approved",
                    Remark = "Bảo hiểm vận chuyển xe lồng hoàn tất"
                }
            );
        }
        if (!await db.TransportMinutes.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var tm = new TransportMinutes
            {
                OrgId = org,
                TransportMinutesNo = "TM202603-001",
                DealerCode = "DLR-HN01",
                TransporterCode = "NYK",
                TransporterName = "NYK Auto Logistics Việt Nam",
                TruckPlateNo = "29C-928.34",
                DriverName = "Vũ Mạnh Hùng",
                DriverPhone = "0912345678",
                TransportReqNo = "CTR202603-001",
                DeliveryOrderNo = "DO202603-001",
                TransportMinutesDate = DateTime.Now.AddDays(-1),
                TotalVehicleCount = 2,
                TotalFreightAmount = 5000000m,
                TotalSurchargeAmount = 200000m,
                TotalAmount = 5200000m,
                FilePath = "https://storage.oem.com/minutes/TM202603-001-signed.pdf",
                Status = "Approved",
                CreatedBy = "transporter.nyk",
                DLApprBy = "dlr.inspector",
                DLApprAt = DateTime.Now.AddHours(-18),
                DLApprNote = "Nhận đủ 02 xe nguyên vẹn, ODO chuẩn, đã kiểm tra thân vỏ và phụ kiện",
                HTCAppr1By = "logistics.specialist",
                HTCAppr1At = DateTime.Now.AddHours(-12),
                HTCAppr2By = "finance.manager",
                HTCAppr2At = DateTime.Now.AddHours(-6),
                Remark = "Biên bản nghiệm thu hoàn tất vận chuyển đường bộ xe lồng tuyến Nhà máy - Hà Nội 01"
            };
            db.TransportMinutes.Add(tm);
            await db.SaveChangesAsync();

            db.TransportMinutesLines.AddRange(
                new TransportMinutesLine
                {
                    OrgId = org,
                    TransportMinutesId = tm.Id,
                    TransportMinutesNo = tm.TransportMinutesNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    DeliveryOrderNo = tm.DeliveryOrderNo,
                    TransportReqNo = tm.TransportReqNo,
                    FromStorage = "YARD-A1",
                    ToStorage = "DLR-HN01",
                    OdoDeparture = 10,
                    OdoArrival = 15,
                    FreightAmount = 2500000m,
                    Surcharge = 100000m,
                    TotalAmount = 2600000m,
                    CargoCondition = "Good",
                    IsInspectionPassed = true,
                    Status = "Approved",
                    Remark = "Xe nguyên vẹn, giao đủ chìa khóa và phụ kiện"
                },
                new TransportMinutesLine
                {
                    OrgId = org,
                    TransportMinutesId = tm.Id,
                    TransportMinutesNo = tm.TransportMinutesNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    DeliveryOrderNo = tm.DeliveryOrderNo,
                    TransportReqNo = tm.TransportReqNo,
                    FromStorage = "YARD-B2",
                    ToStorage = "DLR-HN01",
                    OdoDeparture = 12,
                    OdoArrival = 18,
                    FreightAmount = 2500000m,
                    Surcharge = 100000m,
                    TotalAmount = 2600000m,
                    CargoCondition = "Good",
                    IsInspectionPassed = true,
                    Status = "Approved",
                    Remark = "Xe nguyên vẹn, giao đủ chìa khóa và phụ kiện"
                }
            );
        }

        if (!await db.StorageMaintenances.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var mtn = new StorageMaintenance
            {
                OrgId = org,
                MtnNo = "MTN-202603-001",
                StorageCode = "YARD-A1",
                MtnType = "Periodic",
                PlanDate = DateTime.Now.AddDays(-3),
                TotalVehicleCount = 2,
                PassedVehicleCount = 2,
                FailedVehicleCount = 0,
                Status = "Completed",
                TechnicianCode = "TECH-01",
                TechnicianName = "Nguyễn Văn Kỹ Thuật",
                SupervisorCode = "SUP-01",
                SupervisorName = "Trần Văn Quản Đốc",
                Remark = "Đợt bảo dưỡng định kỳ 30 ngày cho các xe tồn bãi YARD-A1 tháng 03/2026",
                CreatedBy = "planner.oem",
                ApprovedBy = "Trần Văn Quản Đốc",
                ApprovedAt = DateTime.Now.AddDays(-3),
                CompletedAt = DateTime.Now.AddDays(-2)
            };
            db.StorageMaintenances.Add(mtn);
            await db.SaveChangesAsync();

            db.StorageMaintenanceLines.AddRange(
                new StorageMaintenanceLine
                {
                    OrgId = org,
                    StorageMaintenanceId = mtn.Id,
                    MtnNo = mtn.MtnNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    StorageCode = "YARD-A1",
                    MtnTimes = 1,
                    BatteryVoltage = 12.7,
                    ChargeBatteryOk = true,
                    EngineStartCheckOk = true,
                    TirePressureCheckOk = true,
                    TireRotationOk = true,
                    FluidLevelsCheckOk = true,
                    ElectricalSystemsOk = true,
                    BodyCleanOk = true,
                    InspectionResult = "Passed",
                    MtnDate = DateTime.Now.AddDays(-2),
                    NextMtnDate = DateTime.Now.AddDays(28),
                    Technician = "Nguyễn Văn Kỹ Thuật",
                    Status = "Completed",
                    Remark = "Đã kiểm tra điện áp bình 12.7V, nổ máy 15p, áp suất 4 lốp 2.3 bar, dịch chuyển bánh xe"
                },
                new StorageMaintenanceLine
                {
                    OrgId = org,
                    StorageMaintenanceId = mtn.Id,
                    MtnNo = mtn.MtnNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    StorageCode = "YARD-A1",
                    MtnTimes = 1,
                    BatteryVoltage = 12.6,
                    ChargeBatteryOk = true,
                    EngineStartCheckOk = true,
                    TirePressureCheckOk = true,
                    TireRotationOk = true,
                    FluidLevelsCheckOk = true,
                    ElectricalSystemsOk = true,
                    BodyCleanOk = true,
                    InspectionResult = "Passed",
                    MtnDate = DateTime.Now.AddDays(-2),
                    NextMtnDate = DateTime.Now.AddDays(28),
                    Technician = "Nguyễn Văn Kỹ Thuật",
                    Status = "Completed",
                    Remark = "Kiểm tra toàn diện đạt chuẩn, rửa xe sạch sẽ"
                }
            );
        }

        if (!await db.CustomsDeclarations.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var cd = new CustomsDeclaration
            {
                OrgId = org,
                DeclarationNo = "TKHQ202603-001",
                PortCode = "HQ_HAI_PHONG",
                PortName = "Chi cục Hải quan Cửa khẩu Cảng Hải Phòng",
                ContractNo = "CTR-OVERSEA-2026-089",
                LCNo = "LC-VCB-2026-901",
                BillOfLadingNo = "BL-MOL-2026-4412",
                DeclarationType = "CBU",
                OpenDate = DateTime.Now.AddDays(-6),
                TaxPaymentDate = DateTime.Now.AddDays(-4),
                ClearanceDate = DateTime.Now.AddDays(-3),
                CustomsOfficer = "Nguyễn Văn Hải Quan",
                DeclarantName = "Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam",
                TotalVehicleCount = 2,
                TotalTaxValue = 1250000000m,
                ImportTaxAmount = 625000000m,
                ExciseTaxAmount = 656250000m,
                VatAmount = 253125000m,
                TotalTaxAmount = 1534375000m,
                Status = "Cleared",
                Remark = "Tờ khai hải quan thông quan nhập khẩu lô xe nguyên chiếc Accent và Creta tại cảng Hải Phòng",
                CreatedBy = "declarant.htc",
                ApprovedBy = "Nguyễn Văn Hải Quan",
                ApprovedAt = DateTime.Now.AddDays(-5),
                ClearedBy = "Nguyễn Văn Hải Quan",
                ClearedAt = DateTime.Now.AddDays(-3)
            };
            db.CustomsDeclarations.Add(cd);
            await db.SaveChangesAsync();

            db.CustomsDeclarationLines.AddRange(
                new CustomsDeclarationLine
                {
                    OrgId = org,
                    CustomsDeclarationId = cd.Id,
                    DeclarationNo = cd.DeclarationNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    ModelYear = 2026,
                    TaxValue = 550000000m,
                    ImportTaxRate = 50,
                    ImportTax = 275000000m,
                    ExciseTaxRate = 35,
                    ExciseTax = 288750000m,
                    VatRate = 10,
                    VatTax = 111375000m,
                    TotalTax = 675125000m,
                    TaxPaymentDate = DateTime.Now.AddDays(-4),
                    ClearanceDate = DateTime.Now.AddDays(-3),
                    Status = "Cleared",
                    Remark = "Thông quan hợp lệ, đã nộp đủ thuế vào KBNN"
                },
                new CustomsDeclarationLine
                {
                    OrgId = org,
                    CustomsDeclarationId = cd.Id,
                    DeclarationNo = cd.DeclarationNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    ModelYear = 2026,
                    TaxValue = 700000000m,
                    ImportTaxRate = 50,
                    ImportTax = 350000000m,
                    ExciseTaxRate = 35,
                    ExciseTax = 367500000m,
                    VatRate = 10,
                    VatTax = 141750000m,
                    TotalTax = 859250000m,
                    TaxPaymentDate = DateTime.Now.AddDays(-4),
                    ClearanceDate = DateTime.Now.AddDays(-3),
                    Status = "Cleared",
                    Remark = "Thông quan hợp lệ, đã nộp đủ thuế vào KBNN"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.DeclarationNo = cd.DeclarationNo;
                v1.TaxPaymentDate = cd.TaxPaymentDate;
                v1.IsCustomsCleared = true;
                v1.CustomsClearanceDate = cd.ClearanceDate;
            }
            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.DeclarationNo = cd.DeclarationNo;
                v2.TaxPaymentDate = cd.TaxPaymentDate;
                v2.IsCustomsCleared = true;
                v2.CustomsClearanceDate = cd.ClearanceDate;
            }
        }

        if (!await db.CarBoxRequests.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var cbr = new CarBoxRequest
            {
                OrgId = org,
                CBReqNo = "CBR-202603-001",
                DealerCode = "DLR-HN01",
                BodyBuilder = "Hyundai Commercial Body Center Ninh Bình",
                RequestDate = DateTime.Now.AddDays(-5),
                ExpectedStartDate = DateTime.Now.AddDays(-5),
                ExpectedEndDate = DateTime.Now.AddDays(-1),
                TotalVehicleCount = 2,
                TotalAmount = 155000000m,
                Status = "Completed",
                Remark = "Lệnh đóng thùng xe tải đợt 1 tháng 03/2026 cho đại lý Hà Nội 01 (01 Thùng mui bạt H150 + 01 Thùng đông lạnh EX8)",
                CreatedBy = "cv.sales",
                ApprovedBy = "Kỹ Sư Trưởng Đỗ Văn Cường",
                ApprovedAt = DateTime.Now.AddDays(-5),
                CompletedBy = "Quản Đốc QC Lê Thanh Tùng",
                CompletedAt = DateTime.Now.AddDays(-1)
            };
            db.CarBoxRequests.Add(cbr);
            await db.SaveChangesAsync();

            db.CarBoxRequestLines.AddRange(
                new CarBoxRequestLine
                {
                    OrgId = org,
                    CarBoxRequestId = cbr.Id,
                    CBReqNo = cbr.CBReqNo,
                    Vin = "DEMOVIN00000003",
                    Model = "Hyundai New Porter H150",
                    StorageCodeFrom = "YARD-CHASSIS-01",
                    StorageCodeTo = "BODY-SHOP-01",
                    LoaiThung = "ThungBat",
                    TenLoaiThung = "Thùng mui bạt tiêu chuẩn (bửng nhôm/inox)",
                    BoxLengthMm = 3130,
                    BoxWidthMm = 1630,
                    BoxHeightMm = 1770,
                    PayloadKg = 1490,
                    BodyPrice = 35000000m,
                    BodyBuilder = "Hyundai Commercial Body Center Ninh Bình",
                    InspectionNo = "QC-BODY-202603-000003",
                    InspectionResult = "Passed",
                    InspectionDate = DateTime.Now.AddDays(-1),
                    InspectorName = "Quản Đốc QC Lê Thanh Tùng",
                    Status = "Completed",
                    CompletedDate = DateTime.Now.AddDays(-1),
                    Remark = "Kích thước lọt lòng và tải trọng đạt chuẩn Cục Đăng Kiểm"
                },
                new CarBoxRequestLine
                {
                    OrgId = org,
                    CarBoxRequestId = cbr.Id,
                    CBReqNo = cbr.CBReqNo,
                    Vin = "DEMOVIN00000004",
                    Model = "Hyundai Mighty EX8 GTL",
                    StorageCodeFrom = "YARD-CHASSIS-02",
                    StorageCodeTo = "BODY-SHOP-01",
                    LoaiThung = "ThungLanh",
                    TenLoaiThung = "Thùng đông lạnh Panel XPS máy lạnh Thermal Master -18°C",
                    BoxLengthMm = 5700,
                    BoxWidthMm = 2060,
                    BoxHeightMm = 2050,
                    PayloadKg = 6800,
                    BodyPrice = 120000000m,
                    BodyBuilder = "Hyundai Commercial Body Center Ninh Bình",
                    InspectionNo = "QC-BODY-202603-000004",
                    InspectionResult = "Passed",
                    InspectionDate = DateTime.Now.AddDays(-1),
                    InspectorName = "Quản Đốc QC Lê Thanh Tùng",
                    Status = "Completed",
                    CompletedDate = DateTime.Now.AddDays(-1),
                    Remark = "Đã test nhiệt độ thùng đạt -18.5°C sau 45 phút nổ máy, form panel cách nhiệt chuẩn"
                }
            );

            var v3 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000003");
            if (v3 != null)
            {
                v3.TypeCB = "1";
                v3.LoaiThung = "ThungBat";
                v3.CBReqNo = cbr.CBReqNo;
                v3.StorageCode = "BODY-SHOP-01";
            }
            var v4 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000004");
            if (v4 != null)
            {
                v4.TypeCB = "1";
                v4.LoaiThung = "ThungLanh";
                v4.CBReqNo = cbr.CBReqNo;
                v4.StorageCode = "BODY-SHOP-01";
            }
        }

        if (!await db.CarInvoices.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var inv1 = new CarInvoice
            {
                OrgId = org,
                InvoiceListCode = "IVL-202603-001",
                DealerCode = "DLR-HN01",
                InvoiceType = "VAT",
                InvoiceDate = DateTime.Now.AddDays(-2),
                TotalVehicleCount = 2,
                TotalTaxValue = 1250000000m,
                VatRate = 10,
                TotalVatAmount = 125000000m,
                TotalAmount = 1375000000m,
                Status = "Issued",
                Remark = "Bảng kê phát hành hóa đơn GTGT điện tử bán buôn xe Accent & Creta cho đại lý Hà Nội 01",
                CreatedBy = "ke.toan.thanhcong",
                IssuedBy = "Kế Toán Trưởng Trần Thị Mai",
                IssuedAt = DateTime.Now.AddDays(-2)
            };
            db.CarInvoices.Add(inv1);
            await db.SaveChangesAsync();

            db.CarInvoiceLines.AddRange(
                new CarInvoiceLine
                {
                    OrgId = org,
                    CarInvoiceId = inv1.Id,
                    InvoiceListCode = inv1.InvoiceListCode,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    InvoiceDealerCode = "DLR-HN01",
                    InvoiceNo = "HD26-0001001",
                    InvoiceDate = DateTime.Now.AddDays(-2),
                    TaxValue = 550000000m,
                    VatRate = 10,
                    VatAmount = 55000000m,
                    TotalAmount = 605000000m,
                    Status = "Issued",
                    Remark = "Đã phát hành HĐĐT thành công"
                },
                new CarInvoiceLine
                {
                    OrgId = org,
                    CarInvoiceId = inv1.Id,
                    InvoiceListCode = inv1.InvoiceListCode,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    InvoiceDealerCode = "DLR-HN01",
                    InvoiceNo = "HD26-0001002",
                    InvoiceDate = DateTime.Now.AddDays(-2),
                    TaxValue = 700000000m,
                    VatRate = 10,
                    VatAmount = 70000000m,
                    TotalAmount = 770000000m,
                    Status = "Issued",
                    Remark = "Đã phát hành HĐĐT thành công"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.IsInvoiced = true;
                v1.InvoiceNo = "HD26-0001001";
                v1.InvoiceDate = DateTime.Now.AddDays(-2);
                v1.InvoiceListCode = inv1.InvoiceListCode;
            }
            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.IsInvoiced = true;
                v2.InvoiceNo = "HD26-0001002";
                v2.InvoiceDate = DateTime.Now.AddDays(-2);
                v2.InvoiceListCode = inv1.InvoiceListCode;
            }
        }

        if (!await db.GuaranteeExtensions.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ext1 = new GuaranteeExtension
            {
                OrgId = org,
                GrtClaimExtNo = "GEXT-202603-001",
                DealerCode = "DLR-HN01",
                BankCode = "VCB",
                GuaranteeNo = "GRT202603-001",
                ExtensionDays = 30,
                FeeRate = 0.2m,
                TotalVehicleCount = 2,
                TotalGuaranteeAmount = 1250000000m,
                TotalFeeAmount = 2500000m,
                FileSigned = "https://doc.hyundai.thanhcong.vn/guarantee-ext/GEXT-202603-001.pdf",
                Status = "Completed",
                Remark = "Quyết định phê duyệt gia hạn bảo lãnh thanh toán 30 ngày cho lô xe Accent & Creta đại lý Hà Nội 01",
                CreatedBy = "dealer.hn01",
                ApprovedBy = "RiskManager.VuThanhNam",
                ApprovedAt = DateTime.Now.AddDays(-2),
                SignedBy = "GeneralDirector.NguyenAnhTuan",
                SignedAt = DateTime.Now.AddDays(-1)
            };
            db.GuaranteeExtensions.Add(ext1);
            await db.SaveChangesAsync();

            db.GuaranteeExtensionLines.AddRange(
                new GuaranteeExtensionLine
                {
                    OrgId = org,
                    GuaranteeExtensionId = ext1.Id,
                    GrtClaimExtNo = ext1.GrtClaimExtNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    GuaranteeNo = "GRT202603-001",
                    CurrentDateExpired = DateTime.Now.AddDays(-5),
                    NewDateExpired = DateTime.Now.AddDays(25),
                    ExtensionDays = 30,
                    GuaranteeValue = 550000000m,
                    FeeRate = 0.2m,
                    ExtensionFee = 1100000m,
                    Status = "Completed",
                    Remark = "Đã ký số văn bản gia hạn và thanh toán đủ phí gia hạn"
                },
                new GuaranteeExtensionLine
                {
                    OrgId = org,
                    GuaranteeExtensionId = ext1.Id,
                    GrtClaimExtNo = ext1.GrtClaimExtNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    GuaranteeNo = "GRT202603-001",
                    CurrentDateExpired = DateTime.Now.AddDays(-5),
                    NewDateExpired = DateTime.Now.AddDays(25),
                    ExtensionDays = 30,
                    GuaranteeValue = 700000000m,
                    FeeRate = 0.2m,
                    ExtensionFee = 1400000m,
                    Status = "Completed",
                    Remark = "Đã ký số văn bản gia hạn và thanh toán đủ phí gia hạn"
                }
            );

            var gl1 = await db.GuaranteeLines.FirstOrDefaultAsync(l => l.OrgId == org && l.Vin == "DEMOVIN00000001");
            if (gl1 != null)
            {
                gl1.LastGrtExtNo = ext1.GrtClaimExtNo;
                gl1.ExtensionTimes = 1;
            }
            var gl2 = await db.GuaranteeLines.FirstOrDefaultAsync(l => l.OrgId == org && l.Vin == "DEMOVIN00000002");
            if (gl2 != null)
            {
                gl2.LastGrtExtNo = ext1.GrtClaimExtNo;
                gl2.ExtensionTimes = 1;
            }
        }

        if (!await db.ContractCancels.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ccn1 = new ContractCancel
            {
                OrgId = org,
                ContractCNo = "CCN202603-001",
                DealerCode = "DLR-HN01",
                DlrContractNo = "CTR202603-001",
                CancelType = "Partial",
                CancelReason = "Khách hàng cá nhân rút cọc xe Accent, đại lý đề nghị hủy phân bổ 01 xe và rút cọc hợp đồng",
                TotalCancelQty = 1,
                TotalCancelAmount = 550000000m,
                DepositRefundAmount = 50000000m,
                Status = "Approved",
                Remark = "Đã phê duyệt đề nghị hủy phân bổ xe, hoàn trả xe về kho InStock của OEM",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-1),
                ApprovedBy = "SalesDirector.NguyenVanBinh",
                ApprovedAt = DateTime.Now.AddHours(-6)
            };
            db.ContractCancels.Add(ccn1);
            await db.SaveChangesAsync();

            db.ContractCancelLines.Add(
                new ContractCancelLine
                {
                    OrgId = org,
                    ContractCancelId = ccn1.Id,
                    ContractCNo = ccn1.ContractCNo,
                    DlrContractNo = "CTR202603-001",
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    Color = "Trắng",
                    ContractUpdateType = "CUSTOMER_CANCEL",
                    CancelQty = 1,
                    UnitPrice = 550000000m,
                    RefundAmount = 550000000m,
                    Status = "Approved",
                    Remark = "Khách hàng đổi sang dòng Creta, hủy hợp đồng Accent"
                }
            );

            var ccn2 = new ContractCancel
            {
                OrgId = org,
                ContractCNo = "CCN202603-002",
                DealerCode = "DLR-HN01",
                DlrContractNo = "CTR202603-001",
                CancelType = "Partial",
                CancelReason = "Đại lý xin giảm số lượng đặt mua 01 xe Creta do điều chỉnh kế hoạch tài chính showroom",
                TotalCancelQty = 1,
                TotalCancelAmount = 700000000m,
                DepositRefundAmount = 70000000m,
                Status = "Submitted",
                Remark = "Chờ ban giám đốc OEM phê duyệt giảm số lượng hợp đồng bán buôn",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddHours(-3)
            };
            db.ContractCancels.Add(ccn2);
            await db.SaveChangesAsync();

            db.ContractCancelLines.Add(
                new ContractCancelLine
                {
                    OrgId = org,
                    ContractCancelId = ccn2.Id,
                    ContractCNo = ccn2.ContractCNo,
                    DlrContractNo = "CTR202603-001",
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    Color = "Đen",
                    ContractUpdateType = "REDUCE_QTY",
                    CancelQty = 1,
                    UnitPrice = 700000000m,
                    RefundAmount = 700000000m,
                    Status = "Submitted",
                    Remark = "Đề nghị giảm 01 xe Creta khỏi hợp đồng tháng 3"
                }
            );
        }

        if (!await db.CarColorChanges.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ccc1 = new CarColorChange
            {
                OrgId = org,
                ChangeNo = "CCC202603-001",
                DealerCode = "DLR-HN01",
                ChangeType = "CustomerRequest",
                Reason = "Khách hàng mua xe DEMOVIN00000001 yêu cầu đổi màu sơn từ Bạc sang Trắng trước khi bàn giao",
                TotalVehicleCount = 1,
                Status = "Approved",
                Remark = "Đã phê duyệt đổi màu sơn thành công và cập nhật hồ sơ xe VIN",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-2),
                ApprovedBy = "ProductionManager.NguyenVanHung",
                ApprovedAt = DateTime.Now.AddDays(-1)
            };
            db.CarColorChanges.Add(ccc1);
            await db.SaveChangesAsync();

            db.CarColorChangeLines.Add(
                new CarColorChangeLine
                {
                    OrgId = org,
                    CarColorChangeId = ccc1.Id,
                    ChangeNo = ccc1.ChangeNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    OldColor = "Bạc",
                    NewColor = "Trắng",
                    OldColorCode = "T2X",
                    NewColorCode = "NWAC",
                    OldColorName = "Bạc ánh kim (Sleek Silver)",
                    NewColorName = "Trắng ngọc trai (Atlas White)",
                    Status = "Approved",
                    Remark = "Sơn lại theo yêu cầu phong thủy của khách hàng"
                }
            );

            var ccc2 = new CarColorChange
            {
                OrgId = org,
                ChangeNo = "CCC202603-002",
                DealerCode = "DLR-HN01",
                ChangeType = "DealerRequest",
                Reason = "Đại lý xin đổi màu xe Creta từ Đen sang Đỏ để kịp giao xe cho sự kiện ra mắt showroom",
                TotalVehicleCount = 1,
                Status = "Submitted",
                Remark = "Đang chờ quản đốc xưởng sơn OEM kiểm tra lịch buồng sơn và duyệt",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddHours(-4)
            };
            db.CarColorChanges.Add(ccc2);
            await db.SaveChangesAsync();

            db.CarColorChangeLines.Add(
                new CarColorChangeLine
                {
                    OrgId = org,
                    CarColorChangeId = ccc2.Id,
                    ChangeNo = ccc2.ChangeNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    OldColor = "Đen",
                    NewColor = "Đỏ",
                    OldColorCode = "SAW",
                    NewColorCode = "R2P",
                    OldColorName = "Đen bóng (Midnight Black)",
                    NewColorName = "Đỏ đô (Dragon Red)",
                    Status = "Submitted",
                    Remark = "Đề nghị đổi màu theo đơn đặt hàng showroom"
                }
            );
        }

        if (!await db.ContractOverseas.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var cto1 = new ContractOversea
            {
                OrgId = org,
                ContractNo = "CTO-HMC-2026-001",
                ContractNoUser = "HĐNT-2026/01/HMC-01",
                SupplierCode = "HMC",
                SupplierName = "Hyundai Motor Company (Korea)",
                IncotermsCode = "CIF_HAI_PHONG",
                Currency = "USD",
                ExchangeRate = 25450m,
                PaymentTerm = "LC",
                DeparturePort = "BUSAN",
                ArrivalPort = "CANG_HAI_PHONG",
                OrderMonth = "2026-01",
                ProductionMonth = "2026-02",
                ExpectedDeliveryMonth = "2026-03",
                ContractDate = DateTime.Now.AddDays(-60),
                DeliveryDeadline = DateTime.Now.AddDays(15),
                TotalQuantity = 2,
                TotalAmountForeign = 49115.91m,
                TotalAmount = 1250000000m,
                FileSigned = "https://doc.hyundai.thanhcong.vn/contracts-oversea/CTO-HMC-2026-001.pdf",
                Status = "Approved",
                Remark = "Hợp đồng ngoại thương nhập khẩu xe nguyên chiếc CBU Accent và Creta từ tập đoàn Hyundai Hàn Quốc về Cảng Hải Phòng",
                CreatedBy = "import.planner",
                CreatedAt = DateTime.Now.AddDays(-60),
                ApprovedBy = "Director.LeNgocDuc",
                ApprovedAt = DateTime.Now.AddDays(-55)
            };
            db.ContractOverseas.Add(cto1);
            await db.SaveChangesAsync();

            db.ContractOverseaLines.AddRange(
                new ContractOverseaLine
                {
                    OrgId = org,
                    ContractOverseaId = cto1.Id,
                    ContractNo = cto1.ContractNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    Color = "Trắng",
                    ColorCode = "NWAC",
                    ModelYear = 2026,
                    PlantCode = "ULSAN_PLANT_1",
                    PortCode = "BUSAN",
                    WorkOrderNo = "WO-HMC-2026-1001",
                    LCTemp = "LC-VCB-2026-901",
                    OrderQty = 1,
                    UnitPriceForeign = 21610.99m,
                    TotalAmountForeign = 21610.99m,
                    UnitPrice = 550000000m,
                    TotalAmount = 550000000m,
                    Status = "Approved",
                    Remark = "Tiêu chuẩn xuất xưởng CBU thị trường Việt Nam"
                },
                new ContractOverseaLine
                {
                    OrgId = org,
                    ContractOverseaId = cto1.Id,
                    ContractNo = cto1.ContractNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    Color = "Đen",
                    ColorCode = "SAW",
                    ModelYear = 2026,
                    PlantCode = "ULSAN_PLANT_2",
                    PortCode = "BUSAN",
                    WorkOrderNo = "WO-HMC-2026-1002",
                    LCTemp = "LC-VCB-2026-901",
                    OrderQty = 1,
                    UnitPriceForeign = 27504.92m,
                    TotalAmountForeign = 27504.92m,
                    UnitPrice = 700000000m,
                    TotalAmount = 700000000m,
                    Status = "Approved",
                    Remark = "Tiêu chuẩn xuất xưởng CBU thị trường Việt Nam"
                }
            );

            var cto2 = new ContractOversea
            {
                OrgId = org,
                ContractNo = "CTO-HMI-2026-002",
                ContractNoUser = "HĐNT-2026/02/HMI-02",
                SupplierCode = "HMI",
                SupplierName = "Hyundai Motor India",
                IncotermsCode = "CIF_HAI_PHONG",
                Currency = "USD",
                ExchangeRate = 25450m,
                PaymentTerm = "LC",
                DeparturePort = "CHENNAI",
                ArrivalPort = "CANG_HAI_PHONG",
                OrderMonth = "2026-02",
                ProductionMonth = "2026-03",
                ExpectedDeliveryMonth = "2026-04",
                ContractDate = DateTime.Now.AddDays(-20),
                DeliveryDeadline = DateTime.Now.AddDays(40),
                TotalQuantity = 2,
                TotalAmountForeign = 40000m,
                TotalAmount = 1018000000m,
                Status = "Submitted",
                Remark = "Hợp đồng ngoại thương nhập khẩu dòng xe đô thị Venue và Grand i10 từ nhà máy Hyundai Ấn Độ",
                CreatedBy = "import.planner",
                CreatedAt = DateTime.Now.AddDays(-20)
            };
            db.ContractOverseas.Add(cto2);
            await db.SaveChangesAsync();

            db.ContractOverseaLines.AddRange(
                new ContractOverseaLine
                {
                    OrgId = org,
                    ContractOverseaId = cto2.Id,
                    ContractNo = cto2.ContractNo,
                    Model = "Hyundai Venue 1.0 T-GDi",
                    SpecCode = "1.0 T-GDi Cao cấp",
                    Color = "Đỏ",
                    ColorCode = "R2P",
                    ModelYear = 2026,
                    PlantCode = "CHENNAI_PLANT_1",
                    PortCode = "CHENNAI",
                    WorkOrderNo = "WO-HMI-2026-2001",
                    OrderQty = 1,
                    UnitPriceForeign = 22000m,
                    TotalAmountForeign = 22000m,
                    UnitPrice = 559900000m,
                    TotalAmount = 559900000m,
                    Status = "Submitted",
                    Remark = "Kế hoạch nhập khẩu quý 2"
                },
                new ContractOverseaLine
                {
                    OrgId = org,
                    ContractOverseaId = cto2.Id,
                    ContractNo = cto2.ContractNo,
                    Model = "Hyundai Grand i10 Hatchback",
                    SpecCode = "1.2 AT Tiêu chuẩn",
                    Color = "Bạc",
                    ColorCode = "T2X",
                    ModelYear = 2026,
                    PlantCode = "CHENNAI_PLANT_2",
                    PortCode = "CHENNAI",
                    WorkOrderNo = "WO-HMI-2026-2002",
                    OrderQty = 1,
                    UnitPriceForeign = 18000m,
                    TotalAmountForeign = 18000m,
                    UnitPrice = 458100000m,
                    TotalAmount = 458100000m,
                    Status = "Submitted",
                    Remark = "Kế hoạch nhập khẩu quý 2"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null) v1.ContractNoOversea = cto1.ContractNo;

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null) v2.ContractNoOversea = cto1.ContractNo;
        }

        if (!await db.LettersOfCredit.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var lc1 = new LetterOfCredit
            {
                OrgId = org,
                LCNo = "LC-VCB-2026-901",
                LCNoUser = "LC/VCB/2026/03/901",
                ContractNo = "CTO-HMC-2026-001",
                BankCode = "VCB",
                BankName = "Ngân hàng TMCP Ngoại Thương Việt Nam (Vietcombank - Sở Giao Dịch)",
                BeneficiaryName = "Hyundai Motor Company (Korea)",
                ApplicantName = "Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam",
                Currency = "USD",
                ExchangeRate = 25450m,
                LCAmountForeign = 49115.91m,
                LCAmount = 1250000000m,
                MarginRate = 10m,
                MarginAmount = 125000000m,
                IssueDate = DateTime.Now.AddDays(-50),
                ExpiryDate = DateTime.Now.AddDays(40),
                LatestShipmentDate = DateTime.Now.AddDays(10),
                PaymentTerm = "AtSight",
                DeparturePort = "BUSAN",
                ArrivalPort = "CANG_HAI_PHONG",
                TotalVehicleCount = 2,
                UtilizedAmountForeign = 49115.91m,
                UtilizedAmount = 1250000000m,
                RemainingAmountForeign = 0m,
                RemainingAmount = 0m,
                SwiftCode = "MT700-VCB-202603-901001",
                FileSigned = "https://doc.hyundai.thanhcong.vn/letters-of-credit/LC-VCB-2026-901.pdf",
                Status = "Settled",
                Remark = "Thư tín dụng L/C không hủy ngang trả ngay (Irrevocable L/C at sight) thanh toán hợp đồng nhập khẩu lô xe Accent & Creta từ Hyundai Motor Hàn Quốc",
                CreatedBy = "import.finance",
                CreatedAt = DateTime.Now.AddDays(-50),
                ApprovedBy = "FinanceDirector.NguyenVanNam",
                ApprovedAt = DateTime.Now.AddDays(-48),
                UtilizedBy = "TradeFinance.TranThiHang",
                UtilizedAt = DateTime.Now.AddDays(-10),
                SettledBy = "ChiefAccountant.TranThiMai",
                SettledAt = DateTime.Now.AddDays(-2)
            };
            db.LettersOfCredit.Add(lc1);
            await db.SaveChangesAsync();

            db.LetterOfCreditLines.AddRange(
                new LetterOfCreditLine
                {
                    OrgId = org,
                    LetterOfCreditId = lc1.Id,
                    LCNo = lc1.LCNo,
                    ContractNo = lc1.ContractNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    OrderQty = 1,
                    UnitPriceForeign = 21610.99m,
                    TotalAmountForeign = 21610.99m,
                    UnitPrice = 550000000m,
                    TotalAmount = 550000000m,
                    PackingListNo = "PL202603-001",
                    DeclarationNo = "TKHQ202603-001",
                    Status = "Settled",
                    Remark = "Đã khớp bộ chứng từ gốc B/L, Packing List, Invoice và giải phóng L/C"
                },
                new LetterOfCreditLine
                {
                    OrgId = org,
                    LetterOfCreditId = lc1.Id,
                    LCNo = lc1.LCNo,
                    ContractNo = lc1.ContractNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    OrderQty = 1,
                    UnitPriceForeign = 27504.92m,
                    TotalAmountForeign = 27504.92m,
                    UnitPrice = 700000000m,
                    TotalAmount = 700000000m,
                    PackingListNo = "PL202603-001",
                    DeclarationNo = "TKHQ202603-001",
                    Status = "Settled",
                    Remark = "Đã khớp bộ chứng từ gốc B/L, Packing List, Invoice và giải phóng L/C"
                }
            );

            var lc2 = new LetterOfCredit
            {
                OrgId = org,
                LCNo = "LC-CTG-2026-902",
                LCNoUser = "LC/CTG/2026/02/902",
                ContractNo = "CTO-HMI-2026-002",
                BankCode = "CTG",
                BankName = "Ngân hàng TMCP Công Thương Việt Nam (VietinBank - Chi nhánh Hà Nội)",
                BeneficiaryName = "Hyundai Motor India",
                ApplicantName = "Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam",
                Currency = "USD",
                ExchangeRate = 25450m,
                LCAmountForeign = 40000m,
                LCAmount = 1018000000m,
                MarginRate = 10m,
                MarginAmount = 101800000m,
                IssueDate = DateTime.Now.AddDays(-15),
                ExpiryDate = DateTime.Now.AddDays(75),
                LatestShipmentDate = DateTime.Now.AddDays(30),
                PaymentTerm = "Usance30",
                DeparturePort = "CHENNAI",
                ArrivalPort = "CANG_HAI_PHONG",
                TotalVehicleCount = 2,
                UtilizedAmountForeign = 0m,
                UtilizedAmount = 0m,
                RemainingAmountForeign = 40000m,
                RemainingAmount = 1018000000m,
                SwiftCode = "MT700-CTG-202603-902002",
                FileSigned = "https://doc.hyundai.thanhcong.vn/letters-of-credit/LC-CTG-2026-902.pdf",
                Status = "Issued",
                Remark = "Thư tín dụng L/C trả chậm 30 ngày mở tại VietinBank phục vụ nhập khẩu lô xe Venue & Grand i10 từ Hyundai Ấn Độ",
                CreatedBy = "import.finance",
                CreatedAt = DateTime.Now.AddDays(-15),
                ApprovedBy = "FinanceDirector.NguyenVanNam",
                ApprovedAt = DateTime.Now.AddDays(-14)
            };
            db.LettersOfCredit.Add(lc2);
            await db.SaveChangesAsync();

            db.LetterOfCreditLines.AddRange(
                new LetterOfCreditLine
                {
                    OrgId = org,
                    LetterOfCreditId = lc2.Id,
                    LCNo = lc2.LCNo,
                    ContractNo = lc2.ContractNo,
                    Model = "Hyundai Venue 1.0 T-GDi",
                    SpecCode = "1.0 T-GDi Cao cấp",
                    Color = "Đỏ",
                    OrderQty = 1,
                    UnitPriceForeign = 22000m,
                    TotalAmountForeign = 22000m,
                    UnitPrice = 559900000m,
                    TotalAmount = 559900000m,
                    Status = "Issued",
                    Remark = "Đang chờ tàu bốc hàng từ cảng Chennai"
                },
                new LetterOfCreditLine
                {
                    OrgId = org,
                    LetterOfCreditId = lc2.Id,
                    LCNo = lc2.LCNo,
                    ContractNo = lc2.ContractNo,
                    Model = "Hyundai Grand i10 Hatchback",
                    SpecCode = "1.2 AT Tiêu chuẩn",
                    Color = "Bạc",
                    OrderQty = 1,
                    UnitPriceForeign = 18000m,
                    TotalAmountForeign = 18000m,
                    UnitPrice = 458100000m,
                    TotalAmount = 458100000m,
                    Status = "Issued",
                    Remark = "Đang chờ tàu bốc hàng từ cảng Chennai"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null) v1.LCNo = lc1.LCNo;

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null) v2.LCNo = lc1.LCNo;
        }

        if (!await db.RepairOrders.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var ro1 = new RepairOrder
            {
                OrgId = org,
                RoNo = "RO-HN01-2026-0001",
                RoNoUser = "RO/2026/03/HN01-001",
                DealerCode = "DLR-HN01",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                PlateNo = "30K-988.66",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                RoType = "PeriodicMaintenance",
                ServiceAdvisor = "CVDV Trần Quốc Tuấn",
                Technician = "KTV-Trưởng Phạm Văn Hưng",
                OdoKm = 5120,
                FuelLevel = "3/4",
                CarStatus = "Xe sạch sẽ, không trầy xước phát sinh, có thảm lót sàn cao su",
                CustomerRequest = "Bảo dưỡng định kỳ 5.000 km, thay dầu máy, lọc nhớt, kiểm tra phanh và hệ thống điện",
                DiagnosisNotes = "Ắc quy 12.6V tốt, má phanh trước/sau độ mòn chuẩn, lốp 2.3 bar, không có mã lỗi DTC",
                CheckInDate = DateTime.Now.AddDays(-6),
                ExpectedDeliveryDate = DateTime.Now.AddDays(-6).AddHours(3),
                ActualDeliveryDate = DateTime.Now.AddDays(-6).AddHours(2),
                TotalLaborAmount = 400000m,
                TotalPartAmount = 900000m,
                DiscountAmount = 50000m,
                VatRate = 10,
                TotalVatAmount = 125000m,
                TotalAmount = 1375000m,
                PaymentStatus = "Paid",
                PaymentMethod = "Cash",
                PaymentNotes = "Khách thanh toán tiền mặt tại quầy thu ngân đại lý",
                Status = "Paid",
                Remark = "Bảo dưỡng cấp 1 (5.000 km) hoàn tất đúng tiến độ, khách hài lòng",
                CreatedBy = "cvdv.tuan",
                CreatedAt = DateTime.Now.AddDays(-6),
                ApprovedBy = "Quản Đốc Xưởng Lê Văn Thắng",
                ApprovedAt = DateTime.Now.AddDays(-6).AddMinutes(15),
                RepairedBy = "QC Inspector Nguyễn Tuấn Anh",
                RepairedAt = DateTime.Now.AddDays(-6).AddHours(1).AddMinutes(45),
                DeliveredBy = "CVDV Trần Quốc Tuấn",
                DeliveredAt = DateTime.Now.AddDays(-6).AddHours(2),
                PaidBy = "Thu Ngân Phạm Thu Hương",
                PaidAt = DateTime.Now.AddDays(-6).AddHours(2).AddMinutes(10)
            };
            db.RepairOrders.Add(ro1);
            await db.SaveChangesAsync();

            db.RepairOrderServiceLines.AddRange(
                new RepairOrderServiceLine
                {
                    OrgId = org,
                    RepairOrderId = ro1.Id,
                    RoNo = ro1.RoNo,
                    SerCode = "BD-5K",
                    SerName = "Bảo dưỡng định kỳ cấp 5.000 km tiêu chuẩn",
                    ServiceType = "Maintenance",
                    StandardHours = 1.0m,
                    LaborPrice = 300000m,
                    Discount = 0,
                    LaborAmount = 300000m,
                    Technician = "KTV Phạm Văn Hưng",
                    Status = "Completed",
                    Remark = "Kiểm tra 24 hạng mục tiêu chuẩn Hyundai toàn cầu"
                },
                new RepairOrderServiceLine
                {
                    OrgId = org,
                    RepairOrderId = ro1.Id,
                    RoNo = ro1.RoNo,
                    SerCode = "KT-DIEN",
                    SerName = "Kiểm tra hệ thống điện, ắc quy & chẩn đoán ECU GDS-Mobile",
                    ServiceType = "Inspection",
                    StandardHours = 0.5m,
                    LaborPrice = 200000m,
                    Discount = 0,
                    LaborAmount = 100000m,
                    Technician = "KTV Điện Bùi Văn Khoa",
                    Status = "Completed",
                    Remark = "Quét chẩn đoán không phát hiện lỗi (No DTCs)"
                }
            );

            db.RepairOrderPartLines.AddRange(
                new RepairOrderPartLine
                {
                    OrgId = org,
                    RepairOrderId = ro1.Id,
                    RoNo = ro1.RoNo,
                    PartCode = "26300-35505",
                    PartName = "Lọc dầu động cơ chính hãng Mobis",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 120000m,
                    Discount = 0,
                    TotalAmount = 120000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Xuất kho xưởng dịch vụ"
                },
                new RepairOrderPartLine
                {
                    OrgId = org,
                    RepairOrderId = ro1.Id,
                    RoNo = ro1.RoNo,
                    PartCode = "05100-00441",
                    PartName = "Dầu nhờn động cơ cao cấp Hyundai Genuine Oil 5W-30 SN/CF",
                    Unit = "Lít",
                    Quantity = 3.5m,
                    UnitPrice = 160000m,
                    Discount = 0,
                    TotalAmount = 560000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Thay đủ 3.5 lít theo tài liệu kỹ thuật"
                },
                new RepairOrderPartLine
                {
                    OrgId = org,
                    RepairOrderId = ro1.Id,
                    RoNo = ro1.RoNo,
                    PartCode = "97133-2E210",
                    PartName = "Lọc gió điều hòa cabin kháng khuẩn",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 220000m,
                    Discount = 0,
                    TotalAmount = 220000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Thay mới theo yêu cầu khách hàng"
                }
            );

            var ro2 = new RepairOrder
            {
                OrgId = org,
                RoNo = "RO-HN01-2026-0002",
                RoNoUser = "RO/2026/03/HN01-002",
                DealerCode = "DLR-HN01",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                EngineNo = "G4FL0002",
                PlateNo = "30K-678.90",
                CustomerName = "Lê Thanh Bình",
                CustomerPhone = "0912345678",
                RoType = "BodyPaint",
                ServiceAdvisor = "CVDV Vũ Hồng Sơn",
                Technician = "Quản Đốc Sơn Đỗ Mạnh Cường",
                OdoKm = 3200,
                FuelLevel = "1/2",
                CarStatus = "Trầy xước và móp nhẹ mép cản trước bên phụ do quẹt vỉa hè",
                CustomerRequest = "Làm đồng sơn phục hồi cản trước theo hợp đồng bảo hiểm PJICO",
                DiagnosisNotes = "Cản trước biến dạng nhẹ, cần nẹp viền mạ crom và sơn hấp nhiệt màu Đen SAW",
                CheckInDate = DateTime.Now.AddDays(-1),
                ExpectedDeliveryDate = DateTime.Now.AddDays(1),
                TotalLaborAmount = 1425000m,
                TotalPartAmount = 2300000m,
                DiscountAmount = 100000m,
                VatRate = 10,
                TotalVatAmount = 362500m,
                TotalAmount = 3987500m,
                PaymentStatus = "InsuranceCovered",
                PaymentMethod = "Insurance",
                PaymentNotes = "Hồ sơ bồi thường Bảo hiểm PJICO Hà Nội - Giám định viên Trần Hải Long",
                Status = "InGarage",
                Remark = "Đang trong phòng sấy sơn nhiệt, dự kiến bàn giao chiều mai",
                CreatedBy = "cvdv.son",
                CreatedAt = DateTime.Now.AddDays(-1),
                ApprovedBy = "Quản Đốc Đồng Sơn Đỗ Mạnh Cường",
                ApprovedAt = DateTime.Now.AddDays(-1).AddHours(1)
            };
            db.RepairOrders.Add(ro2);
            await db.SaveChangesAsync();

            db.RepairOrderServiceLines.AddRange(
                new RepairOrderServiceLine
                {
                    OrgId = org,
                    RepairOrderId = ro2.Id,
                    RoNo = ro2.RoNo,
                    SerCode = "DS-CAN-TRUOC",
                    SerName = "Gò nắn căn chỉnh phục hồi form cản trước",
                    ServiceType = "BodyPaint",
                    StandardHours = 1.5m,
                    LaborPrice = 350000m,
                    Discount = 0,
                    LaborAmount = 525000m,
                    Technician = "KTV Đồng Nguyễn Hữu Toàn",
                    Status = "Completed",
                    Remark = "Phục hồi chuẩn khe hở cản và tai xe"
                },
                new RepairOrderServiceLine
                {
                    OrgId = org,
                    RepairOrderId = ro2.Id,
                    RoNo = ro2.RoNo,
                    SerCode = "SON-CAN-TRUOC",
                    SerName = "Sơn lót, sơn màu Đen SAW và sơn bóng hấp nhiệt cản trước",
                    ServiceType = "BodyPaint",
                    StandardHours = 2.0m,
                    LaborPrice = 450000m,
                    Discount = 0,
                    LaborAmount = 900000m,
                    Technician = "KTV Sơn Đỗ Mạnh Cường",
                    Status = "InProgress",
                    Remark = "Sơn trong buồng sấy nhiệt tiêu chuẩn OEM"
                }
            );

            db.RepairOrderPartLines.AddRange(
                new RepairOrderPartLine
                {
                    OrgId = org,
                    RepairOrderId = ro2.Id,
                    RoNo = ro2.RoNo,
                    PartCode = "86511-BW000",
                    PartName = "Vỏ cản trước xe Hyundai Creta chính hãng",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 1850000m,
                    Discount = 0,
                    TotalAmount = 1850000m,
                    PaymentType = "Insurance",
                    Status = "Issued",
                    Remark = "Bảo hiểm PJICO duyệt bồi thường 100%"
                },
                new RepairOrderPartLine
                {
                    OrgId = org,
                    RepairOrderId = ro2.Id,
                    RoNo = ro2.RoNo,
                    PartCode = "86519-BW000",
                    PartName = "Nẹp trang trí mạ crom cản trước Creta",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 450000m,
                    Discount = 0,
                    TotalAmount = 450000m,
                    PaymentType = "Insurance",
                    Status = "Issued",
                    Remark = "Bảo hiểm PJICO duyệt bồi thường 100%"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastRoNo = ro1.RoNo;
                v1.LastRoDate = ro1.CheckInDate;
                v1.LastOdoKm = ro1.OdoKm;
            }
            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastRoNo = ro2.RoNo;
                v2.LastRoDate = ro2.CheckInDate;
                v2.LastOdoKm = ro2.OdoKm;
            }
        }

        if (!await db.ServiceAppointments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var app1 = new ServiceAppointment
            {
                OrgId = org,
                AppNo = "APP-HN01-2026-0001",
                AppNoUser = "AP/2026/03/HN01-001",
                DealerCode = "DLR-HN01",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                PlateNo = "30K-988.66",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                ServiceType = "PeriodicMaintenance",
                AppointmentDate = DateTime.Now.AddDays(-6),
                AppointmentTime = "08:30",
                EstimatedDurationMinutes = 120,
                ServiceAdvisor = "CVDV Trần Quốc Tuấn",
                Technician = "KTV-Trưởng Phạm Văn Hưng",
                CustomerRequest = "Bảo dưỡng định kỳ 5.000 km, thay dầu máy, lọc nhớt, kiểm tra phanh và hệ thống điện",
                TotalEstimatedLabor = 400000m,
                TotalEstimatedParts = 900000m,
                TotalEstimatedAmount = 1300000m,
                Status = "InService",
                RoNo = "RO-HN01-2026-0001",
                Remark = "Khách đặt lịch hẹn qua tổng đài Hyundai Hotline, đã tiếp nhận chuyển sang lệnh sửa chữa RO",
                CreatedBy = "cskh.lananh",
                CreatedAt = DateTime.Now.AddDays(-7),
                ConfirmedBy = "CVDV Trần Quốc Tuấn",
                ConfirmedAt = DateTime.Now.AddDays(-7).AddHours(2),
                CheckedInBy = "CVDV Trần Quốc Tuấn",
                CheckedInAt = DateTime.Now.AddDays(-6).AddHours(0)
            };
            db.ServiceAppointments.Add(app1);
            await db.SaveChangesAsync();

            db.ServiceAppointmentServiceLines.AddRange(
                new ServiceAppointmentServiceLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app1.Id,
                    AppNo = app1.AppNo,
                    SerCode = "BD-5K",
                    SerName = "Bảo dưỡng định kỳ cấp 5.000 km tiêu chuẩn",
                    ServiceType = "Maintenance",
                    StandardHours = 1.0m,
                    LaborPrice = 300000m,
                    Discount = 0,
                    LaborAmount = 300000m,
                    Technician = "KTV-Trưởng Phạm Văn Hưng",
                    Status = "Completed",
                    Remark = "Bảo dưỡng cấp 1 tiêu chuẩn"
                },
                new ServiceAppointmentServiceLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app1.Id,
                    AppNo = app1.AppNo,
                    SerCode = "KT-DIEN",
                    SerName = "Kiểm tra hệ thống điện, ắc quy & chẩn đoán ECU GDS-Mobile",
                    ServiceType = "Inspection",
                    StandardHours = 0.5m,
                    LaborPrice = 200000m,
                    Discount = 0,
                    LaborAmount = 100000m,
                    Technician = "KTV Điện Bùi Văn Khoa",
                    Status = "Completed",
                    Remark = "Chẩn đoán OBD không lỗi"
                }
            );

            db.ServiceAppointmentPartLines.AddRange(
                new ServiceAppointmentPartLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app1.Id,
                    AppNo = app1.AppNo,
                    PartCode = "26300-35505",
                    PartName = "Lọc dầu động cơ chính hãng Mobis",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 120000m,
                    Discount = 0,
                    TotalAmount = 120000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Đặt trước theo lịch hẹn"
                },
                new ServiceAppointmentPartLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app1.Id,
                    AppNo = app1.AppNo,
                    PartCode = "05100-00441",
                    PartName = "Dầu nhờn động cơ cao cấp Hyundai Genuine Oil 5W-30 SN/CF",
                    Unit = "Lít",
                    Quantity = 3.5m,
                    UnitPrice = 160000m,
                    Discount = 0,
                    TotalAmount = 560000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Đặt trước 3.5 lít dầu máy"
                },
                new ServiceAppointmentPartLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app1.Id,
                    AppNo = app1.AppNo,
                    PartCode = "97133-2E210",
                    PartName = "Lọc gió điều hòa cabin kháng khuẩn",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 220000m,
                    Discount = 0,
                    TotalAmount = 220000m,
                    PaymentType = "Customer",
                    Status = "Issued",
                    Remark = "Đặt trước lọc gió"
                }
            );

            var app2 = new ServiceAppointment
            {
                OrgId = org,
                AppNo = "APP-HN01-2026-0002",
                AppNoUser = "AP/2026/03/HN01-002",
                DealerCode = "DLR-HN01",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                EngineNo = "G4FL0002",
                PlateNo = "30K-678.90",
                CustomerName = "Lê Thanh Bình",
                CustomerPhone = "0912345678",
                ServiceType = "BodyPaint",
                AppointmentDate = DateTime.Now.AddDays(2),
                AppointmentTime = "09:30",
                EstimatedDurationMinutes = 180,
                ServiceAdvisor = "CVDV Vũ Hồng Sơn",
                Technician = "Quản Đốc Sơn Đỗ Mạnh Cường",
                CustomerRequest = "Kiểm tra sơn phủ bóng nano và căn chỉnh khe hở nắp capo",
                TotalEstimatedLabor = 600000m,
                TotalEstimatedParts = 450000m,
                TotalEstimatedAmount = 1050000m,
                Status = "Confirmed",
                Remark = "Lịch hẹn phục vụ chăm sóc xe và phủ ceramic bóng sơn",
                CreatedBy = "cskh.lananh",
                CreatedAt = DateTime.Now.AddDays(-1),
                ConfirmedBy = "CVDV Vũ Hồng Sơn",
                ConfirmedAt = DateTime.Now.AddHours(-5)
            };
            db.ServiceAppointments.Add(app2);
            await db.SaveChangesAsync();

            db.ServiceAppointmentServiceLines.Add(
                new ServiceAppointmentServiceLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app2.Id,
                    AppNo = app2.AppNo,
                    SerCode = "DS-BONG-NANO",
                    SerName = "Đánh bóng phủ ceramic nano sơn thân vỏ",
                    ServiceType = "BodyPaint",
                    StandardHours = 1.5m,
                    LaborPrice = 400000m,
                    Discount = 0,
                    LaborAmount = 600000m,
                    Technician = "KTV Sơn Đỗ Mạnh Cường",
                    Status = "Confirmed",
                    Remark = "Đã xếp lịch phòng phủ Ceramic"
                }
            );

            db.ServiceAppointmentPartLines.Add(
                new ServiceAppointmentPartLine
                {
                    OrgId = org,
                    ServiceAppointmentId = app2.Id,
                    AppNo = app2.AppNo,
                    PartCode = "NANO-CER-01",
                    PartName = "Bộ dung dịch phủ ceramic Hyundai Care chính hãng",
                    Unit = "Bộ",
                    Quantity = 1,
                    UnitPrice = 450000m,
                    Discount = 0,
                    TotalAmount = 450000m,
                    PaymentType = "Customer",
                    Status = "Confirmed",
                    Remark = "Giữ sẵn vật tư trong kho"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastAppointmentNo = app1.AppNo;
                v1.LastAppointmentDate = app1.AppointmentDate;
            }
            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastAppointmentNo = app2.AppNo;
                v2.LastAppointmentDate = app2.AppointmentDate;
            }
        }

        if (!await db.TechnicalBulletins.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var tsb1 = new TechnicalBulletin
            {
                OrgId = org,
                BulletinNo = "TSB-2026-001",
                BulletinNoUser = "TSB/2026/03/TCU-01",
                Title = "Cập nhật phần mềm điều khiển hộp số tự động TCU 8 cấp và chống rung giật dải tốc độ thấp",
                Category = "SoftwareUpdate",
                Model = "Creta 1.5 Cao cấp",
                Severity = "High",
                ReleaseDate = DateTime.Now.AddDays(-10),
                ExpiryDate = DateTime.Now.AddDays(180),
                Description = "Một số xe ghi nhận hiện tượng chuyển số từ cấp 1 sang cấp 2 hơi giật khi vận hành ở tốc độ thấp dưới 20km/h trong điều kiện đường đô thị đông đúc.",
                Remedy = "Tiến hành kết nối máy chẩn đoán GDS Mobile, tải bản nâng cấp phần mềm ROM TCU phiên bản V2.4 và nạp vào hộp số, thực hiện hiệu chỉnh học lại vị trí ly hợp (Clutch Adaptation).",
                AttachmentFileName = "TSB-2026-001-TCU-UPDATE.pdf",
                AttachmentUrl = "https://doc.hyundai.thanhcong.vn/tsb/TSB-2026-001-TCU-UPDATE.pdf",
                TotalVehicleCount = 1,
                CompletedVehicleCount = 1,
                Status = "Published",
                Remark = "Áp dụng kiểm tra và cập nhật miễn phí cho toàn bộ xe trong dải số khung áp dụng",
                CreatedBy = "OEM.SeniorEngineer",
                CreatedAt = DateTime.Now.AddDays(-12),
                PublishedBy = "OEM.TechnicalDirector",
                PublishedAt = DateTime.Now.AddDays(-10)
            };
            db.TechnicalBulletins.Add(tsb1);
            await db.SaveChangesAsync();

            db.TechnicalBulletinLines.Add(
                new TechnicalBulletinLine
                {
                    OrgId = org,
                    TechnicalBulletinId = tsb1.Id,
                    BulletinNo = tsb1.BulletinNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    PlateNo = "30K-678.90",
                    DealerCode = "DLR-HN01",
                    Status = "Completed",
                    InspectedAt = DateTime.Now.AddDays(-3),
                    CompletedAt = DateTime.Now.AddDays(-3),
                    Technician = "KTV Chẩn đoán Nguyễn Hữu Toàn",
                    OdoKm = 3200,
                    RoNo = "RO-HN01-2026-0001",
                    ResultNotes = "Đã nạp bản ROM TCU V2.4, xóa mã lỗi và chạy thử xe chuyển số rất mượt mà",
                    Remark = "Nghiệm thu đạt yêu cầu kỹ thuật"
                }
            );

            var tsb2 = new TechnicalBulletin
            {
                OrgId = org,
                BulletinNo = "TSB-2026-002",
                BulletinNoUser = "TSB/2026/03/STEER-02",
                Title = "Kiểm tra siết lực bu-lông cụm thước lái điện MDPS và cân chỉnh góc đặt bánh xe",
                Category = "TechnicalGuideline",
                Model = "Accent 1.4 AT",
                Severity = "Medium",
                ReleaseDate = DateTime.Now.AddDays(-5),
                ExpiryDate = DateTime.Now.AddDays(120),
                Description = "Kiểm tra lực siết đai ốc trục lái trung gian và bu-lông cố định mô-tơ trợ lực lái MDPS nhằm loại trừ tiếng kêu lách cách khi đánh hết lái.",
                Remedy = "Sử dụng cờ-lê cân lực kiểm tra siết đạt mô-men 45-50 Nm, bôi mỡ chuyên dụng mỡ trắng chống rung và cân chỉnh độ chụm bánh xe.",
                AttachmentFileName = "TSB-2026-002-STEERING-TORQUE.pdf",
                AttachmentUrl = "https://doc.hyundai.thanhcong.vn/tsb/TSB-2026-002-STEERING-TORQUE.pdf",
                TotalVehicleCount = 1,
                CompletedVehicleCount = 0,
                Status = "Published",
                Remark = "Chiến dịch hướng dẫn kỹ thuật bảo dưỡng xưởng",
                CreatedBy = "OEM.SeniorEngineer",
                CreatedAt = DateTime.Now.AddDays(-6),
                PublishedBy = "OEM.TechnicalDirector",
                PublishedAt = DateTime.Now.AddDays(-5)
            };
            db.TechnicalBulletins.Add(tsb2);
            await db.SaveChangesAsync();

            db.TechnicalBulletinLines.Add(
                new TechnicalBulletinLine
                {
                    OrgId = org,
                    TechnicalBulletinId = tsb2.Id,
                    BulletinNo = tsb2.BulletinNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    PlateNo = "30K-123.45",
                    DealerCode = "DLR-HN01",
                    Status = "Notified",
                    Remark = "Đã phát thông báo nhắc nhở kiểm tra khi xe vào xưởng làm dịch vụ"
                }
            );

            var v2Vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Vehicle != null)
            {
                v2Vehicle.LastBulletinNo = tsb1.BulletinNo;
                v2Vehicle.LastBulletinDate = DateTime.Now.AddDays(-3);
            }
        }

        if (!await db.BankDisbursements.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var dis1 = new BankDisbursement
            {
                OrgId = org,
                RQ_BankingTransNo = "BDIS202603-001",
                RQ_BankingTransNoUser = "DNGN/2026/03/CTG-HN01-01",
                DealerCode = "DLR-HN01",
                BankCode = "VIETINBANK",
                BankName = "Ngân hàng TMCP Công thương Việt Nam - Chi nhánh Hà Nội",
                BizResNumber = "HĐTD-CTG-2026-HN01",
                BeneficiaryAccountNo = "110002899999",
                BeneficiaryAccountName = "Công ty Cổ phần Liên doanh Ô tô Hyundai Thành Công Việt Nam",
                BeneficiaryBankCode = "VIETINBANK",
                DisbursementType = "AutoLoan",
                TotalVehicleCount = 2,
                TotalCollateralValue = 1250000000m,
                DisbursementRate = 80m,
                TotalDisbursementAmount = 1000000000m,
                DisbursedAmount = 1000000000m,
                BkTransStatus = "Disbursed",
                BkTransBankStatus = "Disbursed",
                RefBankCode = "REF-CTG-20260315-8899",
                DisbursementDate = DateTime.Now.AddDays(-2),
                BankRemark = "Hạch toán giải ngân thành công từ HĐTD hạn mức số HĐTD-CTG-2026-HN01",
                FilePath = "https://doc.hyundai.thanhcong.vn/disbursements/BDIS202603-001.pdf",
                Remark = "Hồ sơ đề nghị giải ngân ngân hàng VietinBank tài trợ vốn lưu động mua lô xe Accent & Creta đại lý Hà Nội 01",
                CreatedBy = "credit.officer.ctg",
                CreatedAt = DateTime.Now.AddDays(-5),
                ApprovedBy = "FinanceDirector.TranMinhDuc",
                ApprovedAt = DateTime.Now.AddDays(-4),
                PushedBy = "System.CoreBankingGateway",
                PushedAt = DateTime.Now.AddDays(-3),
                DisbursedBy = "ChiefAccountant.NguyenThanhHa",
                DisbursedAt = DateTime.Now.AddDays(-2)
            };
            db.BankDisbursements.Add(dis1);
            await db.SaveChangesAsync();

            db.BankDisbursementLines.AddRange(
                new BankDisbursementLine
                {
                    OrgId = org,
                    BankDisbursementId = dis1.Id,
                    RQ_BankingTransNo = dis1.RQ_BankingTransNo,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    InvoiceNo = "HD26-0001001",
                    InvoiceDate = DateTime.Now.AddDays(-4),
                    GuaranteeNo = "GRT202603-001",
                    UnitPrice = 550000000m,
                    CollateralValue = 550000000m,
                    DisbursementPercent = 80m,
                    DisbursementAmount = 440000000m,
                    DisbursedAmount = 440000000m,
                    Status = "Disbursed",
                    Remark = "Giải ngân 80% giá trị xuất bán buôn"
                },
                new BankDisbursementLine
                {
                    OrgId = org,
                    BankDisbursementId = dis1.Id,
                    RQ_BankingTransNo = dis1.RQ_BankingTransNo,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    InvoiceNo = "HD26-0001002",
                    InvoiceDate = DateTime.Now.AddDays(-4),
                    GuaranteeNo = "GRT202603-001",
                    UnitPrice = 700000000m,
                    CollateralValue = 700000000m,
                    DisbursementPercent = 80m,
                    DisbursementAmount = 560000000m,
                    DisbursedAmount = 560000000m,
                    Status = "Disbursed",
                    Remark = "Giải ngân 80% giá trị xuất bán buôn"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastDisbursementNo = dis1.RQ_BankingTransNo;
                v1.LastDisbursementDate = dis1.DisbursementDate;
            }
            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastDisbursementNo = dis1.RQ_BankingTransNo;
                v2.LastDisbursementDate = dis1.DisbursementDate;
            }
        }

        if (!await db.ServiceCampaigns.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var cam1 = new ServiceCampaign
            {
                OrgId = org,
                CamMarketingNo = "CAM-2026-001",
                CamMarketingNoUser = "KM/2026/03/HE-01",
                CampaignName = "Chiến dịch Chăm sóc Toàn diện Xe Hyundai Đón Hè 2026 & Tri ân Khách hàng",
                CampaignType = "SeasonalService",
                Model = "All",
                DateStart = DateTime.Now.AddDays(-15),
                DateEnd = DateTime.Now.AddDays(45),
                DiscountLaborPercent = 20,
                DiscountPartPercent = 15,
                FreeInspectionItems = "Miễn phí kiểm tra 20 hạng mục an toàn: Hệ thống phanh, lốp xe, điện áp ắc quy, mức dầu máy, nước làm mát động cơ, hệ thống điều hòa nhiệt độ cabin và quét chẩn đoán mã lỗi ECU/GDS-Mobile",
                GiftDescription = "Ô dù cầm tay cao cấp Hyundai + Voucher khử khuẩn nội thất Nano Bạc",
                BudgetAmount = 250000000m,
                ActualAmount = 250000m,
                TotalVehicleCount = 2,
                AttendedVehicleCount = 1,
                Status = "InProgress",
                Remark = "Chương trình áp dụng tại tất cả Đại lý Ủy quyền Hyundai trên toàn quốc",
                CreatedBy = "MarketingOEM.NguyenHaMy",
                CreatedAt = DateTime.Now.AddDays(-16),
                ApprovedBy = "AfterSalesDirector.PhamQuocBao",
                ApprovedAt = DateTime.Now.AddDays(-15)
            };
            db.ServiceCampaigns.Add(cam1);
            await db.SaveChangesAsync();

            db.ServiceCampaignLines.AddRange(
                new ServiceCampaignLine
                {
                    OrgId = org,
                    ServiceCampaignId = cam1.Id,
                    CamMarketingNo = cam1.CamMarketingNo,
                    DealerCode = "DLR-HN01",
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    PlateNo = "30K-988.66",
                    CustomerName = "Nguyễn Văn An",
                    CustomerPhone = "0901234567",
                    ServiceDate = DateTime.Now.AddDays(-6),
                    RoNo = "RO-HN01-2026-0001",
                    DiscountLaborAmount = 100000m,
                    DiscountPartAmount = 150000m,
                    TotalDiscountAmount = 250000m,
                    IsGiftDelivered = true,
                    GiftName = "Ô dù cầm tay cao cấp Hyundai",
                    Technician = "KTV-Trưởng Phạm Văn Hưng",
                    ServiceAdvisor = "CVDV Trần Quốc Tuấn",
                    Status = "Completed",
                    Remark = "Khách hàng đã nhận đủ ưu đãi giảm 20% công + 15% dầu nhớt và quà tặng tri ân"
                },
                new ServiceCampaignLine
                {
                    OrgId = org,
                    ServiceCampaignId = cam1.Id,
                    CamMarketingNo = cam1.CamMarketingNo,
                    DealerCode = "DLR-HN01",
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    PlateNo = "30K-678.90",
                    CustomerName = "Lê Thanh Bình",
                    CustomerPhone = "0912345678",
                    ServiceDate = null,
                    RoNo = null,
                    DiscountLaborAmount = 0,
                    DiscountPartAmount = 0,
                    TotalDiscountAmount = 0,
                    IsGiftDelivered = false,
                    GiftName = null,
                    Technician = null,
                    ServiceAdvisor = "CVDV Vũ Hồng Sơn",
                    Status = "Registered",
                    Remark = "Đã gửi tin nhắn SMS mời khách hàng mang xe vào xưởng làm dịch vụ"
                }
            );

            var cam2 = new ServiceCampaign
            {
                OrgId = org,
                CamMarketingNo = "CAM-2026-002",
                CamMarketingNoUser = "KM/2026/03/SAFETY-02",
                CampaignName = "Chiến dịch Miễn phí Kiểm tra Ắc quy & Hệ thống Lái Trợ lực Điện MDPS",
                CampaignType = "SafetyCheck",
                Model = "Accent 1.4 AT",
                DateStart = DateTime.Now.AddDays(-5),
                DateEnd = DateTime.Now.AddDays(25),
                DiscountLaborPercent = 30,
                DiscountPartPercent = 10,
                FreeInspectionItems = "Kiểm tra đo dòng sạc/nạp bình ắc quy, kiểm tra lực siết khớp nối trục lái điện MDPS",
                GiftDescription = "Bình nước giữ nhiệt inox Lock&Lock Hyundai Motorsport",
                BudgetAmount = 120000000m,
                ActualAmount = 0,
                TotalVehicleCount = 1,
                AttendedVehicleCount = 0,
                Status = "Approved",
                Remark = "Chiến dịch kiểm tra an toàn theo bản tin kỹ thuật TSB-2026-002",
                CreatedBy = "TechnicalSupport.TranVanDuc",
                CreatedAt = DateTime.Now.AddDays(-6),
                ApprovedBy = "AfterSalesDirector.PhamQuocBao",
                ApprovedAt = DateTime.Now.AddDays(-5)
            };
            db.ServiceCampaigns.Add(cam2);
            await db.SaveChangesAsync();

            db.ServiceCampaignLines.Add(
                new ServiceCampaignLine
                {
                    OrgId = org,
                    ServiceCampaignId = cam2.Id,
                    CamMarketingNo = cam2.CamMarketingNo,
                    DealerCode = "DLR-HN01",
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    PlateNo = "30K-988.66",
                    CustomerName = "Nguyễn Văn An",
                    CustomerPhone = "0901234567",
                    Status = "Registered",
                    Remark = "Đã lên danh sách xe ưu tiên kiểm tra"
                }
            );

            var v1Vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Vehicle != null)
            {
                v1Vehicle.LastCampaignNo = cam1.CamMarketingNo;
                v1Vehicle.LastCampaignDate = DateTime.Now.AddDays(-6);
            }
        }

        if (!await db.WarrantyReports.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var wr1 = new WarrantyReport
            {
                OrgId = org,
                ROWNo = "WR-HN01-2026-0001",
                ROWNoUser = "WR/2026/03/HN01-01",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                RoNo = "RO-HN01-2026-0001",
                Vin = "DEMOVIN00000001",
                PlateNo = "30K-988.66",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                OdoKm = 12500,
                CheckInDate = DateTime.Now.AddDays(-10),
                StartDate = DateTime.Now.AddDays(-10).AddHours(1),
                FinishedDate = DateTime.Now.AddDays(-10).AddHours(4),
                WarrantyStartDate = DateTime.Now.AddMonths(-6),
                WarrantyEndDate = DateTime.Now.AddMonths(30),
                WarrantyMonths = 36,
                CusName = "Nguyễn Văn An",
                CusTel = "0901234567",
                CusAddress = "Số 12 phố Trần Duy Hưng, Cầu Giấy, Hà Nội",
                CusRequest = "Cần số chuyển số giật cục và có tiếng kêu lục cục ở hệ thống lái trợ lực điện MDPS",
                DiagnosticResult = "Hỏng vòng đệm cao su giảm chấn khớp nối trục lái điện MDPS (Coupling Flexible Steering Column) và cảm biến vị trí góc lái",
                NaturalCode = "C05",
                CauseCode = "M01",
                MainPartCode = "56315-2K000-FFF",
                MainPartName = "Khớp cao su giảm chấn trục lái điện MDPS (Coupling Flexible)",
                WarrantyType = "Standard",
                TotalLaborAmount = 600000m,
                TotalPartAmount = 1450000m,
                TotalAmount = 2050000m,
                ApprovedLaborAmount = 600000m,
                ApprovedPartAmount = 1450000m,
                ApprovedTotalAmount = 2050000m,
                ReimbursedAmount = 2050000m,
                ReimburseDate = DateTime.Now.AddDays(-3),
                AccountingRefNo = "UNC-WR-202603-HN01-001",
                OldPartsInspectionStatus = "ReturnedToFactory",
                Status = "Settled",
                Remark = "Hồ sơ bảo hành tiêu chuẩn chính hãng đã được duyệt chi trả bù trừ công nợ đại lý",
                CreatedBy = "cvdv.tuan",
                CreatedAt = DateTime.Now.AddDays(-10),
                ConfirmedBy = "TechSupport.NguyenVanDuc",
                ConfirmedAt = DateTime.Now.AddDays(-8),
                ApprovedBy = "WarrantyManager.TranThanhSon",
                ApprovedAt = DateTime.Now.AddDays(-5),
                SettledBy = "Accountant.HoangThiMai",
                SettledAt = DateTime.Now.AddDays(-3)
            };
            db.WarrantyReports.Add(wr1);
            await db.SaveChangesAsync();

            db.WarrantyReportLaborLines.AddRange(
                new WarrantyReportLaborLine
                {
                    OrgId = org,
                    WarrantyReportId = wr1.Id,
                    ROWNo = wr1.ROWNo,
                    SerCode = "BH-MDPS-REPAIR",
                    SerName = "Tháo hạ cụm cột lái điện MDPS, thay khớp cao su giảm chấn và căn chỉnh cảm biến góc lái",
                    StdManHour = 1.5m,
                    LaborPrice = 400000m,
                    LaborAmount = 600000m,
                    ApprovedManHour = 1.5m,
                    ApprovedLaborAmount = 600000m,
                    Technician = "KTV-Trưởng Phạm Văn Hưng",
                    Status = "Settled",
                    Remark = "Đã thực hiện đúng quy trình kỹ thuật TSB"
                }
            );

            db.WarrantyReportPartLines.AddRange(
                new WarrantyReportPartLine
                {
                    OrgId = org,
                    WarrantyReportId = wr1.Id,
                    ROWNo = wr1.ROWNo,
                    PartCode = "56315-2K000-FFF",
                    PartName = "Khớp cao su giảm chấn trục lái điện MDPS (Coupling Flexible Steering Column)",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 250000m,
                    TotalAmount = 250000m,
                    ApprovedQty = 1,
                    ApprovedAmount = 250000m,
                    IsMainPart = true,
                    OldPartSerialNo = "MDPS-OLD-88912",
                    OldPartReturnStatus = "Returned",
                    Status = "Settled",
                    Remark = "Phụ tùng lỗi vỡ cao su đã gửi về kho bảo hành OEM Ninh Bình"
                },
                new WarrantyReportPartLine
                {
                    OrgId = org,
                    WarrantyReportId = wr1.Id,
                    ROWNo = wr1.ROWNo,
                    PartCode = "93480-3X000",
                    PartName = "Cụm cảm biến góc lái điện tử EPS (Clock Spring / Steering Angle Sensor)",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 1200000m,
                    TotalAmount = 1200000m,
                    ApprovedQty = 1,
                    ApprovedAmount = 1200000m,
                    IsMainPart = false,
                    OldPartSerialNo = "SAS-OLD-33120",
                    OldPartReturnStatus = "Returned",
                    Status = "Settled",
                    Remark = "Đã thu hồi xác linh kiện"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastWarrantyReportNo = wr1.ROWNo;
                v1.LastWarrantyReportDate = wr1.CreatedAt;
                v1.WarrantyClaimCount = 1;
            }
        }
        if (!await db.ServiceQuotations.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var quote1 = new ServiceQuotation
            {
                OrgId = org,
                QuoteNo = "QT-DLR-HN01-202603-0001",
                QuoteNoUser = "BG-2026/03/HN01-008",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                PlateNo = "30K-888.88",
                OdoKm = 20500,
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerAddress = "Số 12 phố Trần Duy Hưng, Cầu Giấy, Hà Nội",
                CustomerType = "Individual",
                QuotationType = "PeriodicMaintenance",
                ServiceAdvisor = "Trần Đình Khang",
                QuoteDate = DateTime.Now.AddDays(-2),
                ValidUntilDate = DateTime.Now.AddDays(28),
                PaymentMethod = "Cash",
                TotalLaborAmount = 600000m,
                TotalPartAmount = 1450000m,
                DiscountAmount = 100000m,
                VatRate = 10,
                TotalVatAmount = 195000m,
                TotalAmount = 2145000m,
                Status = "CustomerApproved",
                ApprovedByCustomer = true,
                CustomerApprovedAt = DateTime.Now.AddDays(-1),
                CustomerSignature = "Nguyễn Văn An - Ký duyệt qua App KH",
                Remark = "Báo giá gói bảo dưỡng cấp 2 vạn (20.000 km) tiêu chuẩn chính hãng Hyundai Mobis",
                CreatedBy = "advisor.khang",
                CreatedAt = DateTime.Now.AddDays(-2),
                SentBy = "advisor.khang",
                SentAt = DateTime.Now.AddDays(-2)
            };
            db.ServiceQuotations.Add(quote1);
            await db.SaveChangesAsync();

            db.ServiceQuotationLaborLines.AddRange(
                new ServiceQuotationLaborLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    SerCode = "BD-20K",
                    SerName = "Bảo dưỡng định kỳ cấp 20.000 km (Kiểm tra 24 hạng mục an toàn)",
                    ServiceType = "Maintenance",
                    StandardHours = 1.5m,
                    LaborPrice = 300000m,
                    Discount = 50000m,
                    LaborAmount = 400000m,
                    Technician = "Nguyễn Văn Tuấn",
                    Status = "Approved",
                    Remark = "Bao gồm vệ sinh phanh 4 bánh và kiểm tra hệ thống treo"
                },
                new ServiceQuotationLaborLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    SerCode = "CBD-BANH-XE",
                    SerName = "Cân bằng động bánh xe & Đảo lốp định kỳ 4 bánh",
                    ServiceType = "Maintenance",
                    StandardHours = 0.8m,
                    LaborPrice = 250000m,
                    Discount = 0,
                    LaborAmount = 200000m,
                    Technician = "Lê Minh Hùng",
                    Status = "Approved",
                    Remark = "Cân chì lazang đúc hợp kim nhôm"
                }
            );

            db.ServiceQuotationPartLines.AddRange(
                new ServiceQuotationPartLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    PartCode = "26300-35505",
                    PartName = "Lọc dầu động cơ chính hãng Mobis (Oil Filter Element)",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 120000m,
                    Discount = 0,
                    TotalAmount = 120000m,
                    PaymentType = "Customer",
                    Status = "Approved",
                    Remark = "Phụ tùng tiêu hao định kỳ"
                },
                new ServiceQuotationPartLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    PartCode = "05100-00441",
                    PartName = "Dầu động cơ tổng hợp toàn phần Hyundai Premium Gasoline 5W-30 (Can 4L)",
                    Unit = "Can",
                    Quantity = 1,
                    UnitPrice = 680000m,
                    Discount = 50000m,
                    TotalAmount = 630000m,
                    PaymentType = "Customer",
                    Status = "Approved",
                    Remark = "Chuẩn API SP / ILSAC GF-6"
                },
                new ServiceQuotationPartLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    PartCode = "28113-H8100",
                    PartName = "Lọc gió động cơ chính hãng (Air Cleaner Filter)",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 280000m,
                    Discount = 0,
                    TotalAmount = 280000m,
                    PaymentType = "Customer",
                    Status = "Approved",
                    Remark = "Thay mới định kỳ 20.000 km"
                },
                new ServiceQuotationPartLine
                {
                    OrgId = org,
                    ServiceQuotationId = quote1.Id,
                    QuoteNo = quote1.QuoteNo,
                    PartCode = "97133-D1000",
                    PartName = "Lọc gió điều hòa cabin than hoạt tính PM2.5 (Cabin Air Filter)",
                    Unit = "Cái",
                    Quantity = 1,
                    UnitPrice = 420000m,
                    Discount = 0,
                    TotalAmount = 420000m,
                    PaymentType = "Customer",
                    Status = "Approved",
                    Remark = "Khử mùi diệt khuẩn dàn lạnh"
                }
            );

            var v1Quote = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Quote != null)
            {
                v1Quote.LastQuoteNo = quote1.QuoteNo;
                v1Quote.LastQuoteDate = quote1.QuoteDate;
                v1Quote.QuotationCount = 1;
            }
        }

        if (!await db.CustomerCares.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var care1 = new CustomerCare
            {
                OrgId = org,
                CareNo = "CC-HN01-202603-0001",
                CareNoUser = "CSKH-2026/03/HN01-001",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                Vin = "DEMOVIN00000001",
                PlateNo = "30K-888.88",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerEmail = "an.nguyen@gmail.com",
                CustomerAddress = "Số 12 phố Trần Duy Hưng, Cầu Giấy, Hà Nội",
                CareType = "FollowUp72h",
                ContactMethod = "PhoneCall",
                RoNo = "RO-HN01-2026-0001",
                OdoKm = 20500,
                ServiceDate = DateTime.Now.AddDays(-3),
                ContactDate = DateTime.Now.AddDays(-1),
                NextCareDate = DateTime.Now.AddDays(89),
                CallAttempts = 1,
                CareStaff = "CSKH Lê Thùy Linh",
                ServiceAdvisor = "Trần Đình Khang",
                ScoreOverall = 5.0m,
                ScoreQuality = 5.0m,
                ScoreAdvisor = 5.0m,
                ScoreFacility = 4.8m,
                IsProblemSolved = true,
                NpsScore = 10,
                CustomerFeedback = "Khách hàng rất hài lòng về dịch vụ bảo dưỡng, xe chạy êm, cố vấn nhiệt tình giải thích chi tiết các hạng mục thay dầu lọc và vệ sinh phanh.",
                IsResolved = true,
                Status = "Completed",
                CompletedBy = "CSKH Lê Thùy Linh",
                CompletedAt = DateTime.Now.AddDays(-1),
                Remark = "Khảo sát CSI 72h sau bảo dưỡng cấp 20.000 km hoàn tất xuất sắc",
                CreatedBy = "system.trigger",
                CreatedAt = DateTime.Now.AddDays(-3)
            };
            db.CustomerCares.Add(care1);

            var care2 = new CustomerCare
            {
                OrgId = org,
                CareNo = "CC-HN01-202603-0002",
                CareNoUser = "CSKH-2026/03/HN01-002",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                Vin = "DEMOVIN00000002",
                PlateNo = "30K-999.99",
                Model = "Creta 1.5 Cao cấp",
                EngineNo = "G4FL0002",
                CustomerName = "Trần Thị Bích",
                CustomerPhone = "0912345678",
                CustomerEmail = "bich.tran@gmail.com",
                CustomerAddress = "Số 88 đường Láng, Đống Đa, Hà Nội",
                CareType = "MaintenanceReminder",
                ContactMethod = "ZaloZNS",
                OdoKm = 10000,
                ServiceDate = DateTime.Now.AddDays(-90),
                ContactDate = DateTime.Now.AddDays(-2),
                NextCareDate = DateTime.Now.AddDays(7),
                CallAttempts = 1,
                CareStaff = "CSKH Hoàng Mai Trang",
                ServiceAdvisor = "Trần Đình Khang",
                ScoreOverall = 4.8m,
                ScoreQuality = 4.8m,
                ScoreAdvisor = 5.0m,
                ScoreFacility = 4.5m,
                IsProblemSolved = true,
                NpsScore = 9,
                CustomerFeedback = "Khách đã nhận tin nhắn ZNS nhắc bảo dưỡng 10.000 km và đồng ý đặt lịch hẹn thứ 7 tuần tới.",
                IsResolved = true,
                Status = "Completed",
                CompletedBy = "CSKH Hoàng Mai Trang",
                CompletedAt = DateTime.Now.AddDays(-2),
                Remark = "Nhắc bảo dưỡng định kỳ 10.000 km qua Zalo ZNS thành công",
                CreatedBy = "system.auto_reminder",
                CreatedAt = DateTime.Now.AddDays(-5)
            };
            db.CustomerCares.Add(care2);
            await db.SaveChangesAsync();

            var v1Care = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Care != null)
            {
                v1Care.LastCareNo = care1.CareNo;
                v1Care.LastCareDate = care1.ContactDate;
                v1Care.LastCareType = care1.CareType;
                v1Care.LastCsiScore = care1.ScoreOverall;
                v1Care.CareCount = 1;
            }

            var v2Care = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Care != null)
            {
                v2Care.LastCareNo = care2.CareNo;
                v2Care.LastCareDate = care2.ContactDate;
                v2Care.LastCareType = care2.CareType;
                v2Care.LastCsiScore = care2.ScoreOverall;
                v2Care.CareCount = 1;
            }
        }

        if (!await db.ProductionOrders.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var po1 = new ProductionOrder
            {
                OrgId = org,
                OrderNo = "PO-202603-0001",
                OrderNoUser = "SX/2026/03/HTMV1-01",
                OrdMonth = "2026-03",
                OrdType = "MTO",
                OrdCategoryType = "MakeToOrder",
                PlantCode = "HTMV_NINHBINH_1",
                PlantName = "Nhà máy Sản xuất Ô tô Hyundai Ninh Bình số 1 (HTMV 1)",
                TotalPlanQty = 5,
                TotalProducedQty = 2,
                EstimatedCompletionDate = DateTime.Now.AddDays(20),
                Status = "InProduction",
                Remark = "Kế hoạch sản xuất dòng xe Accent 1.4 và Creta 1.5 tháng 03/2026",
                CreatedBy = "plan.oem",
                CreatedAt = DateTime.Now.AddDays(-10),
                ScheduledBy = "HTMV.Planner.NguyenVanTuan",
                ScheduledAt = DateTime.Now.AddDays(-9),
                StartedBy = "QuanDocXuong.LeVanHung",
                StartedAt = DateTime.Now.AddDays(-7)
            };
            db.ProductionOrders.Add(po1);
            await db.SaveChangesAsync();

            db.ProductionOrderLines.AddRange(
                new ProductionOrderLine
                {
                    OrgId = org,
                    ProductionOrderId = po1.Id,
                    OrderNo = po1.OrderNo,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    SpecDescription = "Động cơ Kappa 1.4L MPI, Hộp số tự động 6 cấp, Cửa sổ trời, Màn hình AVN 8 inch",
                    ColorCode = "NWAC/Black",
                    ColorName = "Trắng Ngọc Trai / Nội thất Đen",
                    PlanQty = 3,
                    QtyMonthN1 = 4,
                    QtyMonthN2 = 5,
                    QtyMonthN3 = 5,
                    ProducedQty = 1,
                    ETADate = DateTime.Now.AddDays(10),
                    Stage = "FinalQC",
                    Status = "InProduction",
                    Remark = "Đã xuất xưởng 1 xe DEMOVIN00000001"
                },
                new ProductionOrderLine
                {
                    OrgId = org,
                    ProductionOrderId = po1.Id,
                    OrderNo = po1.OrderNo,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    SpecDescription = "Động cơ Smartstream G1.5, Hộp số iVT, Gói an toàn Hyundai SmartSense, Loa Bose",
                    ColorCode = "SAW/Black",
                    ColorName = "Đen Phantom / Nội thất Đen",
                    PlanQty = 2,
                    QtyMonthN1 = 3,
                    QtyMonthN2 = 4,
                    QtyMonthN3 = 4,
                    ProducedQty = 1,
                    ETADate = DateTime.Now.AddDays(15),
                    Stage = "FinalQC",
                    Status = "InProduction",
                    Remark = "Đã xuất xưởng 1 xe DEMOVIN00000002"
                }
            );

            var po2 = new ProductionOrder
            {
                OrgId = org,
                OrderNo = "PO-202603-0002",
                OrderNoUser = "SX/2026/03/HTMV2-02",
                OrdMonth = "2026-03",
                OrdType = "MTS",
                OrdCategoryType = "Regular",
                PlantCode = "HTMV_NINHBINH_2",
                PlantName = "Nhà máy Sản xuất Ô tô Hyundai Ninh Bình số 2 (HTMV 2)",
                TotalPlanQty = 4,
                TotalProducedQty = 0,
                EstimatedCompletionDate = DateTime.Now.AddDays(25),
                Status = "Scheduled",
                Remark = "Kế hoạch sản xuất xe SUV SantaFe & Tucson thế hệ mới",
                CreatedBy = "plan.oem",
                CreatedAt = DateTime.Now.AddDays(-5),
                ScheduledBy = "HTMV.Planner.TranVanHung",
                ScheduledAt = DateTime.Now.AddDays(-4)
            };
            db.ProductionOrders.Add(po2);
            await db.SaveChangesAsync();

            db.ProductionOrderLines.AddRange(
                new ProductionOrderLine
                {
                    OrgId = org,
                    ProductionOrderId = po2.Id,
                    OrderNo = po2.OrderNo,
                    LineIndex = 1,
                    Model = "SantaFe 2.5T AWD",
                    SpecCode = "2.5T Calligraphy 6 chỗ",
                    SpecDescription = "Động cơ Smartstream 2.5 T-GDI 281 mã lực, Hộp số 8DCT, Dẫn động HTRAC",
                    ColorCode = "R2P/Brown",
                    ColorName = "Đỏ Đô / Da Nappa Nâu Cognac",
                    PlanQty = 2,
                    QtyMonthN1 = 3,
                    QtyMonthN2 = 3,
                    QtyMonthN3 = 4,
                    ProducedQty = 0,
                    ETADate = DateTime.Now.AddDays(22),
                    Stage = "Body",
                    Status = "Scheduled",
                    Remark = "Chuẩn bị dập thân vỏ và hàn khung"
                },
                new ProductionOrderLine
                {
                    OrgId = org,
                    ProductionOrderId = po2.Id,
                    OrderNo = po2.OrderNo,
                    LineIndex = 2,
                    Model = "Tucson 1.6T HTRAC",
                    SpecCode = "1.6 Turbo AWD",
                    SpecDescription = "Động cơ Smartstream 1.6 T-GDI, Cụm màn hình cong Panoramic 12.3 inch",
                    ColorCode = "T2X/Black",
                    ColorName = "Xám Kim Loại / Nội thất Đen",
                    PlanQty = 2,
                    QtyMonthN1 = 3,
                    QtyMonthN2 = 3,
                    QtyMonthN3 = 3,
                    ProducedQty = 0,
                    ETADate = DateTime.Now.AddDays(25),
                    Stage = "Stamping",
                    Status = "Scheduled",
                    Remark = "Chuẩn bị dập linh kiện thân vỏ"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastWorkOrderNo = po1.OrderNo;
                v1.ManufacturedDate = DateTime.Now.AddDays(-7);
                v1.PlantCode = po1.PlantCode;
            }

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastWorkOrderNo = po1.OrderNo;
                v2.ManufacturedDate = DateTime.Now.AddDays(-6);
                v2.PlantCode = po1.PlantCode;
            }
        }

        if (!await db.ProformaInvoices.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var pi1 = new ProformaInvoice
            {
                OrgId = org,
                RefNo = "PI202603-001",
                RefNoUser = "PI/2026/03/HN01-01",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                OrderMonth = "2026-03",
                ProductionMonth = "2026-03",
                ExpectedDeliveryMonth = "2026-04",
                Currency = "USD",
                ExchangeRate = 25450m,
                TotalQuantity = 2,
                TotalAmountForeign = 49115.91m,
                TotalAmount = 1250000000m,
                DepositRate = 10m,
                DepositAmount = 125000000m,
                PaymentTerm = "LC",
                DeparturePort = "BUSAN",
                ArrivalPort = "CANG_HAI_PHONG",
                LCTemp = "LC-VCB-2026-001",
                LCNo = "LC-VCB-2026-001",
                ContractNo = "CTO-HMC-2026-001",
                Status = "Approved",
                Remark = "Hóa đơn chiếu lệ PI đặt xe đợt 1 tháng 03/2026 đại lý Hyundai Hà Nội 01",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "SaleManager.TranQuocTuan",
                ApprovedAt = DateTime.Now.AddDays(-8),
                ExecutedBy = "XNK.NguyenVanNam",
                ExecutedAt = DateTime.Now.AddDays(-5)
            };
            db.ProformaInvoices.Add(pi1);
            await db.SaveChangesAsync();

            db.ProformaInvoiceLines.AddRange(
                new ProformaInvoiceLine
                {
                    OrgId = org,
                    ProformaInvoiceId = pi1.Id,
                    RefNo = pi1.RefNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    SpecDescription = "Động cơ Kappa 1.4L MPI, Hộp số 6AT, Cửa sổ trời, Ghế da",
                    ColorCode = "NWAC",
                    ColorName = "Trắng Ngọc Trai",
                    WorkOrderNo = "PO-202603-0001",
                    PlantCode = "HTMV_NINHBINH_1",
                    PortCode = "CANG_HAI_PHONG",
                    LCTemp = "LC-VCB-2026-001",
                    ContractNo = "CTO-HMC-2026-001",
                    OrderQty = 1,
                    AllocatedQty = 1,
                    UnitPriceForeign = 21610.99m,
                    TotalAmountForeign = 21610.99m,
                    UnitPrice = 550000000m,
                    TotalAmount = 550000000m,
                    Status = "Approved",
                    Remark = "Xe giao đợt đầu tháng 4"
                },
                new ProformaInvoiceLine
                {
                    OrgId = org,
                    ProformaInvoiceId = pi1.Id,
                    RefNo = pi1.RefNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    SpecDescription = "Động cơ Smartstream 1.5L, Gói công nghệ an toàn SmartSense",
                    ColorCode = "SAW",
                    ColorName = "Đen Phantom",
                    WorkOrderNo = "PO-202603-0001",
                    PlantCode = "HTMV_NINHBINH_1",
                    PortCode = "CANG_HAI_PHONG",
                    LCTemp = "LC-VCB-2026-001",
                    ContractNo = "CTO-HMC-2026-001",
                    OrderQty = 1,
                    AllocatedQty = 1,
                    UnitPriceForeign = 27504.92m,
                    TotalAmountForeign = 27504.92m,
                    UnitPrice = 700000000m,
                    TotalAmount = 700000000m,
                    Status = "Approved",
                    Remark = "Xe giao đợt đầu tháng 4"
                }
            );

            var pi2 = new ProformaInvoice
            {
                OrgId = org,
                RefNo = "PI202603-002",
                RefNoUser = "PI/2026/03/SG01-02",
                DealerCode = "DLR-SG01",
                DealerName = "Hyundai Sài Gòn 01",
                OrderMonth = "2026-03",
                ProductionMonth = "2026-04",
                ExpectedDeliveryMonth = "2026-05",
                Currency = "USD",
                ExchangeRate = 25450m,
                TotalQuantity = 2,
                TotalAmountForeign = 85265.23m,
                TotalAmount = 2170000000m,
                DepositRate = 10m,
                DepositAmount = 217000000m,
                PaymentTerm = "LC",
                DeparturePort = "ULSAN",
                ArrivalPort = "CANG_CAT_LAI",
                LCTemp = "LC-TCB-2026-002",
                Status = "Submitted",
                Remark = "Đơn đặt xe SUV SantaFe & Tucson cho thị trường miền Nam",
                CreatedBy = "dealer.sg01",
                CreatedAt = DateTime.Now.AddDays(-3)
            };
            db.ProformaInvoices.Add(pi2);
            await db.SaveChangesAsync();

            db.ProformaInvoiceLines.AddRange(
                new ProformaInvoiceLine
                {
                    OrgId = org,
                    ProformaInvoiceId = pi2.Id,
                    RefNo = pi2.RefNo,
                    LineIndex = 1,
                    Model = "SantaFe 2.5T AWD",
                    SpecCode = "2.5T Calligraphy 6 chỗ",
                    SpecDescription = "Động cơ Smartstream 2.5 T-GDI 281 mã lực, Hộp số 8DCT",
                    ColorCode = "R2P",
                    ColorName = "Đỏ Đô",
                    PlantCode = "HTMV_NINHBINH_2",
                    PortCode = "CANG_CAT_LAI",
                    OrderQty = 1,
                    AllocatedQty = 0,
                    UnitPriceForeign = 50294.70m,
                    TotalAmountForeign = 50294.70m,
                    UnitPrice = 1280000000m,
                    TotalAmount = 1280000000m,
                    Status = "Pending",
                    Remark = "Kế hoạch xuất xưởng tháng 4"
                },
                new ProformaInvoiceLine
                {
                    OrgId = org,
                    ProformaInvoiceId = pi2.Id,
                    RefNo = pi2.RefNo,
                    LineIndex = 2,
                    Model = "Tucson 1.6T HTRAC",
                    SpecCode = "1.6 Turbo AWD",
                    SpecDescription = "Động cơ 1.6 T-GDI Turbo, Màn hình cong 12.3 inch",
                    ColorCode = "T2X",
                    ColorName = "Xám Kim Loại",
                    PlantCode = "HTMV_NINHBINH_2",
                    PortCode = "CANG_CAT_LAI",
                    OrderQty = 1,
                    AllocatedQty = 0,
                    UnitPriceForeign = 34970.53m,
                    TotalAmountForeign = 34970.53m,
                    UnitPrice = 890000000m,
                    TotalAmount = 890000000m,
                    Status = "Pending",
                    Remark = "Kế hoạch xuất xưởng tháng 4"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastPiNo = pi1.RefNo;
                v1.LastPiDate = DateTime.Now.AddDays(-8);
                v1.PiCount = 1;
            }

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastPiNo = pi1.RefNo;
                v2.LastPiDate = DateTime.Now.AddDays(-8);
                v2.PiCount = 1;
            }
        }

        if (!await db.PdiPayments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var pdiPay1 = new PdiPayment
            {
                OrgId = org,
                PmtPdiNo = "PDI-PAY-202603-0001",
                PmtPdiNoUser = "QT-PDI/2026/03/HN01-01",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                StorageCode = "YARD-A1",
                PeriodMonth = "2026-03",
                PaymentDate = DateTime.Now.AddDays(-5),
                TotalVehicleCount = 2,
                TotalCostIn = 500000m,
                TotalCostOut = 400000m,
                TotalAmount = 900000m,
                VatRate = 10m,
                VatAmount = 90000m,
                TotalAmountAfterVAT = 990000m,
                BankRefNo = "UNC-PDI-PAY-20260318-001",
                SettledDate = DateTime.Now.AddDays(-2),
                FileSigned = "https://doc.hyundai.thanhcong.vn/pdi-payments/PDI-PAY-202603-0001.pdf",
                Status = "Settled",
                Remark = "Quyết toán định mức chi phí kiểm tra PDI xe nhập bãi và xuất bãi đợt 1 tháng 03/2026 đại lý Hyundai Hà Nội 01",
                CreatedBy = "pdi.hn01",
                CreatedAt = DateTime.Now.AddDays(-7),
                Approved1By = "ServiceDept.PhamVanHung",
                Approved1At = DateTime.Now.AddDays(-5),
                Approved2By = "FinanceDept.NguyenThuHuong",
                Approved2At = DateTime.Now.AddDays(-4),
                TCMSSignedBy = "TCMSDirector.TranQuocTuan",
                TCMSSignedAt = DateTime.Now.AddDays(-3),
                HTVSignedBy = "HTVDirector.LeNgocDuc",
                HTVSignedAt = DateTime.Now.AddDays(-2),
                SettledBy = "ChiefAccountant.HoangThiMai",
                SettledAt = DateTime.Now.AddDays(-2)
            };
            db.PdiPayments.Add(pdiPay1);
            await db.SaveChangesAsync();

            db.PdiPaymentLines.AddRange(
                new PdiPaymentLine
                {
                    OrgId = org,
                    PdiPaymentId = pdiPay1.Id,
                    PmtPdiNo = pdiPay1.PmtPdiNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    StorageCode = "YARD-A1",
                    DealerCode = "DLR-HN01",
                    PdiReqNo = "PDI-202603-001",
                    DlvMnNo = "DMN202603-001",
                    CostInCheck = 250000m,
                    CostOutCheck = 200000m,
                    TotalCostCheck = 450000m,
                    PdiCompletedDate = DateTime.Now.AddDays(-6),
                    PdiResult = "Passed",
                    Status = "Settled",
                    Remark = "Kiểm tra toàn diện 20 hạng mục đạt 100% tiêu chuẩn xuất xưởng"
                },
                new PdiPaymentLine
                {
                    OrgId = org,
                    PdiPaymentId = pdiPay1.Id,
                    PmtPdiNo = pdiPay1.PmtPdiNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    StorageCode = "YARD-A1",
                    DealerCode = "DLR-HN01",
                    PdiReqNo = "PDI-202603-002",
                    DlvMnNo = "DMN202603-002",
                    CostInCheck = 250000m,
                    CostOutCheck = 200000m,
                    TotalCostCheck = 450000m,
                    PdiCompletedDate = DateTime.Now.AddDays(-6),
                    PdiResult = "Passed",
                    Status = "Settled",
                    Remark = "Kiểm tra toàn diện đạt chuẩn bàn giao xe mới"
                }
            );

            var pdiPay2 = new PdiPayment
            {
                OrgId = org,
                PmtPdiNo = "PDI-PAY-202603-0002",
                PmtPdiNoUser = "QT-PDI/2026/03/SG01-02",
                DealerCode = "DLR-SG01",
                DealerName = "Hyundai Sài Gòn 01",
                StorageCode = "YARD-SG01",
                PeriodMonth = "2026-03",
                PaymentDate = DateTime.Now.AddDays(-2),
                TotalVehicleCount = 2,
                TotalCostIn = 500000m,
                TotalCostOut = 400000m,
                TotalAmount = 900000m,
                VatRate = 10m,
                VatAmount = 90000m,
                TotalAmountAfterVAT = 990000m,
                Status = "Submitted",
                Remark = "Đề nghị quyết toán chi phí kiểm tra PDI xe SUV SantaFe & Tucson khu vực miền Nam",
                CreatedBy = "pdi.sg01",
                CreatedAt = DateTime.Now.AddDays(-3)
            };
            db.PdiPayments.Add(pdiPay2);
            await db.SaveChangesAsync();

            db.PdiPaymentLines.AddRange(
                new PdiPaymentLine
                {
                    OrgId = org,
                    PdiPaymentId = pdiPay2.Id,
                    PmtPdiNo = pdiPay2.PmtPdiNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    StorageCode = "YARD-SG01",
                    DealerCode = "DLR-SG01",
                    CostInCheck = 250000m,
                    CostOutCheck = 200000m,
                    TotalCostCheck = 450000m,
                    PdiCompletedDate = DateTime.Now.AddDays(-3),
                    PdiResult = "Passed",
                    Status = "Submitted",
                    Remark = "PDI hoàn tất đạt chuẩn"
                },
                new PdiPaymentLine
                {
                    OrgId = org,
                    PdiPaymentId = pdiPay2.Id,
                    PmtPdiNo = pdiPay2.PmtPdiNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    StorageCode = "YARD-SG01",
                    DealerCode = "DLR-SG01",
                    CostInCheck = 250000m,
                    CostOutCheck = 200000m,
                    TotalCostCheck = 450000m,
                    PdiCompletedDate = DateTime.Now.AddDays(-3),
                    PdiResult = "Passed",
                    Status = "Submitted",
                    Remark = "PDI hoàn tất đạt chuẩn"
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.IsPdiPaid = true;
                v1.PdiPaidAmount = 450000m;
                v1.LastPdiPaymentNo = pdiPay1.PmtPdiNo;
                v1.LastPdiPaymentDate = DateTime.Now.AddDays(-2);
                v1.PdiPaymentCount = 1;
            }

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.IsPdiPaid = true;
                v2.PdiPaidAmount = 450000m;
                v2.LastPdiPaymentNo = pdiPay1.PmtPdiNo;
                v2.LastPdiPaymentDate = DateTime.Now.AddDays(-2);
                v2.PdiPaymentCount = 1;
            }
        }

        if (!await db.SalesPolicies.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var sp1 = new SalesPolicy
            {
                OrgId = org,
                SPSRCode = "SPL-2026-03-01",
                SPNo = "CV-2026/03/HTV-SALES-01",
                SPSRType = "RetailSupport",
                SPSRRoot = null,
                FormBusinessSupportCode = "DirectCash",
                StartDate = DateTime.Now.AddDays(-15),
                EndDate = DateTime.Now.AddDays(45),
                TotalModelsCount = 2,
                TotalSupportBudget = 500000000m,
                TotalVinApplied = 2,
                TotalActualPaidAmount = 15000000m,
                FilePath = "https://doc.hyundai.thanhcong.vn/policies/SPL-2026-03-01.pdf",
                Status = "Active",
                Remark = "Chính sách hỗ trợ giá kích cầu bán lẻ xe Hyundai Accent và Creta quý 1 năm 2026 toàn quốc",
                CreatedBy = "sales.planner",
                CreatedAt = DateTime.Now.AddDays(-16),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddDays(-15)
            };
            db.SalesPolicies.Add(sp1);
            await db.SaveChangesAsync();

            db.SalesPolicyLines.AddRange(
                new SalesPolicyLine
                {
                    OrgId = org,
                    SalesPolicyId = sp1.Id,
                    SPSRCode = sp1.SPSRCode,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    SpecDescription = "Hỗ trợ kích cầu dòng sedan Accent 1.4 bản Đặc biệt",
                    DealerCode = null,
                    ModelYear = 2026,
                    AmountSupport = 15000000m,
                    Status = "Active",
                    Remark = "Hỗ trợ 15 triệu VNĐ trực tiếp cho mỗi xe bán lẻ có kích hoạt BH"
                },
                new SalesPolicyLine
                {
                    OrgId = org,
                    SalesPolicyId = sp1.Id,
                    SPSRCode = sp1.SPSRCode,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    SpecDescription = "Hỗ trợ dòng B-SUV Creta 1.5 bản Cao cấp",
                    DealerCode = null,
                    ModelYear = 2026,
                    AmountSupport = 20000000m,
                    Status = "Active",
                    Remark = "Hỗ trợ 20 triệu VNĐ trực tiếp cho mỗi xe bán lẻ có kích hoạt BH"
                }
            );

            var sp2 = new SalesPolicy
            {
                OrgId = org,
                SPSRCode = "SPL-2026-03-02",
                SPNo = "CV-2026/03/HTV-SALES-02",
                SPSRType = "RegistrationSupport",
                FormBusinessSupportCode = "InvoiceDeduction",
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(55),
                TotalModelsCount = 2,
                TotalSupportBudget = 800000000m,
                TotalVinApplied = 0,
                TotalActualPaidAmount = 0m,
                FilePath = "https://doc.hyundai.thanhcong.vn/policies/SPL-2026-03-02.pdf",
                Status = "Active",
                Remark = "Chương trình hỗ trợ 50% lệ phí trước bạ cho xe SUV SantaFe & Tucson sản xuất 2026",
                CreatedBy = "sales.planner",
                CreatedAt = DateTime.Now.AddDays(-6),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddDays(-5)
            };
            db.SalesPolicies.Add(sp2);
            await db.SaveChangesAsync();

            db.SalesPolicyLines.AddRange(
                new SalesPolicyLine
                {
                    OrgId = org,
                    SalesPolicyId = sp2.Id,
                    SPSRCode = sp2.SPSRCode,
                    LineIndex = 1,
                    Model = "SantaFe 2.5T AWD",
                    SpecCode = "2.5T Calligraphy 6 chỗ",
                    SpecDescription = "Hỗ trợ 50% trước bạ SantaFe Calligraphy",
                    DealerCode = null,
                    ModelYear = 2026,
                    AmountSupport = 64000000m,
                    Status = "Active",
                    Remark = "Trừ trực tiếp trên hóa đơn bán lẻ"
                },
                new SalesPolicyLine
                {
                    OrgId = org,
                    SalesPolicyId = sp2.Id,
                    SPSRCode = sp2.SPSRCode,
                    LineIndex = 2,
                    Model = "Tucson 1.6T HTRAC",
                    SpecCode = "1.6 Turbo AWD",
                    SpecDescription = "Hỗ trợ 50% trước bạ Tucson 1.6 Turbo HTRAC",
                    DealerCode = null,
                    ModelYear = 2026,
                    AmountSupport = 44500000m,
                    Status = "Active",
                    Remark = "Trừ trực tiếp trên hóa đơn bán lẻ"
                }
            );

            var spsr1 = new SalesPolicySupport
            {
                OrgId = org,
                SupportNo = "SPSR-202603-0001",
                SPSRCode = sp1.SPSRCode,
                SPNo = sp1.SPNo,
                Vin = "DEMOVIN00000001",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                Model = "Accent 1.4 AT",
                SpecCode = "1.4 AT Đặc biệt",
                EngineNo = "G4LC0001",
                Color = "Trắng",
                DateSupport = DateTime.Now.AddDays(-6),
                DateFullStatus = DateTime.Now.AddDays(-4),
                AmountSupport = 15000000m,
                HTCInvoiceNo = "HD26-0001001",
                HTCInvoiceDate = DateTime.Now.AddDays(-4),
                HTCDatePayment = DateTime.Now.AddDays(-2),
                BankRefNo = "UNC-SPL-20260318-001",
                Status = "Settled",
                Remark = "Đã quyết toán chi trả 15 triệu tiền hỗ trợ bán lẻ cho xe Accent",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-6),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddDays(-3),
                SettledBy = "ChiefAccountant.HoangThiMai",
                SettledAt = DateTime.Now.AddDays(-2)
            };
            db.SalesPolicySupports.Add(spsr1);

            var spsr2 = new SalesPolicySupport
            {
                OrgId = org,
                SupportNo = "SPSR-202603-0002",
                SPSRCode = sp1.SPSRCode,
                SPNo = sp1.SPNo,
                Vin = "DEMOVIN00000002",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                Model = "Creta 1.5 Cao cấp",
                SpecCode = "1.5 Cao cấp 2 tông màu",
                EngineNo = "G4FL0002",
                Color = "Đen",
                DateSupport = DateTime.Now.AddDays(-4),
                DateFullStatus = DateTime.Now.AddDays(-3),
                AmountSupport = 20000000m,
                HTCInvoiceNo = "HD26-0001002",
                HTCInvoiceDate = DateTime.Now.AddDays(-4),
                Status = "Approved",
                Remark = "Đã duyệt hỗ trợ 20 triệu tiền khuyến mại bán lẻ cho xe Creta, chờ chuyển khoản",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-4),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddDays(-2)
            };
            db.SalesPolicySupports.Add(spsr2);
            await db.SaveChangesAsync();

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.IsPolicySupported = true;
                v1.PolicySupportAmount = 15000000m;
                v1.LastPolicyCode = sp1.SPSRCode;
                v1.LastPolicyDate = DateTime.Now.AddDays(-3);
                v1.PolicySupportCount = 1;
            }

            var v2Policy = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Policy != null)
            {
                v2Policy.IsPolicySupported = true;
                v2Policy.PolicySupportAmount = 20000000m;
                v2Policy.LastPolicyCode = sp1.SPSRCode;
                v2Policy.LastPolicyDate = DateTime.Now.AddDays(-2);
                v2Policy.PolicySupportCount = 1;
            }
        }

        if (!await db.GpsDevices.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var dev1 = new GpsDevice
            {
                OrgId = org,
                GpsCode = "GPS-2026-0001",
                GpsBoxNo = "BOX-2026-01",
                SerialNo = "VT-4G-9901001",
                ImeiNo = "868901020304001",
                SimNo = "0987654321",
                Provider = "Viettel",
                ModelName = "OBD-4G",
                StorageCodeGps = "KHO_GPS_NINHBINH",
                DeviceStatus = "Installed",
                BatteryVolt = 12.8m,
                BatteryPercent = 98,
                CurrentVin = "DEMOVIN00000001",
                CurrentModel = "Accent 1.4 AT",
                CurrentLocation = "Bãi lưu kho YARD-A1 Nhà máy Ninh Bình",
                Latitude = 20.2520m,
                Longitude = 105.9750m,
                SpeedKmH = 0,
                IsInGeofence = true,
                LastSignalAt = DateTime.Now.AddMinutes(-5),
                LastGpsInNo = "GPSIN-202603-0001",
                Remark = "Thiết bị định vị 4G lắp trên xe Accent tồn bãi",
                CreatedAt = DateTime.Now.AddDays(-10)
            };
            var dev2 = new GpsDevice
            {
                OrgId = org,
                GpsCode = "GPS-2026-0002",
                GpsBoxNo = "BOX-2026-01",
                SerialNo = "VLC-4G-9901002",
                ImeiNo = "868901020304002",
                SimNo = "0987654322",
                Provider = "Veloca",
                ModelName = "VT-03D",
                StorageCodeGps = "KHO_GPS_NINHBINH",
                DeviceStatus = "Installed",
                BatteryVolt = 12.6m,
                BatteryPercent = 95,
                CurrentVin = "DEMOVIN00000002",
                CurrentModel = "Creta 1.5 Cao cấp",
                CurrentLocation = "Bãi lưu kho YARD-B2 Nhà máy Ninh Bình",
                Latitude = 20.2535m,
                Longitude = 105.9762m,
                SpeedKmH = 0,
                IsInGeofence = true,
                LastSignalAt = DateTime.Now.AddMinutes(-12),
                LastGpsInNo = "GPSIN-202603-0001",
                Remark = "Thiết bị định vị Veloca gắn trên xe Creta",
                CreatedAt = DateTime.Now.AddDays(-10)
            };
            var dev3 = new GpsDevice
            {
                OrgId = org,
                GpsCode = "GPS-2026-0003",
                GpsBoxNo = "BOX-2026-02",
                SerialNo = "VT-4G-9901003",
                ImeiNo = "868901020304003",
                SimNo = "0987654323",
                Provider = "Viettel",
                ModelName = "OBD-4G",
                StorageCodeGps = "KHO_GPS_NINHBINH",
                DeviceStatus = "InStock",
                BatteryVolt = 12.9m,
                BatteryPercent = 100,
                Remark = "Thiết bị trong kho sẵn sàng lắp đặt cho xe mới xuất xưởng",
                CreatedAt = DateTime.Now.AddDays(-5)
            };
            var dev4 = new GpsDevice
            {
                OrgId = org,
                GpsCode = "GPS-2026-0004",
                GpsBoxNo = "BOX-2026-02",
                SerialNo = "VLC-4G-9901004",
                ImeiNo = "868901020304004",
                SimNo = "0987654324",
                Provider = "Veloca",
                ModelName = "VT-03D",
                StorageCodeGps = "KHO_GPS_NINHBINH",
                DeviceStatus = "InStock",
                BatteryVolt = 12.8m,
                BatteryPercent = 100,
                Remark = "Thiết bị trong kho dự phòng",
                CreatedAt = DateTime.Now.AddDays(-5)
            };
            db.GpsDevices.AddRange(dev1, dev2, dev3, dev4);

            var install = new GpsInstallation
            {
                OrgId = org,
                GpsInNo = "GPSIN-202603-0001",
                GpsInNoUser = "SF-GPSIN-2026/03-01",
                GpsInType = "First_In",
                StorageCodeGps = "KHO_GPS_NINHBINH",
                InstallationDate = DateTime.Now.AddDays(-8),
                TotalVehicleCount = 2,
                Status = "Approved",
                Remark = "Lắp đặt thiết bị định vị GPS cho lô xe Accent và Creta hoàn tất KCS xuất xưởng",
                CreatedBy = "planner.oem",
                CreatedAt = DateTime.Now.AddDays(-8),
                ApprovedBy = "StorageLead.NguyenVanDinh",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };
            db.GpsInstallations.Add(install);
            await db.SaveChangesAsync();

            db.GpsInstallationLines.AddRange(
                new GpsInstallationLine
                {
                    OrgId = org,
                    GpsInstallationId = install.Id,
                    GpsInNo = install.GpsInNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    StorageCode = "YARD-A1",
                    GpsCode = "GPS-2026-0001",
                    ImeiNo = "868901020304001",
                    SimNo = "0987654321",
                    BatteryVolt = 12.8m,
                    Technician = "KTV Lắp Đặt Bùi Văn Hải",
                    InstalledAt = DateTime.Now.AddDays(-8),
                    InitialSignalStatus = "SignalOK",
                    Status = "Installed",
                    Remark = "Tín hiệu GPS & 4G đạt chuẩn 100%"
                },
                new GpsInstallationLine
                {
                    OrgId = org,
                    GpsInstallationId = install.Id,
                    GpsInNo = install.GpsInNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    StorageCode = "YARD-B2",
                    GpsCode = "GPS-2026-0002",
                    ImeiNo = "868901020304002",
                    SimNo = "0987654322",
                    BatteryVolt = 12.6m,
                    Technician = "KTV Lắp Đặt Bùi Văn Hải",
                    InstalledAt = DateTime.Now.AddDays(-8),
                    InitialSignalStatus = "SignalOK",
                    Status = "Installed",
                    Remark = "Tín hiệu GPS & 4G đạt chuẩn 100%"
                }
            );

            db.GpsLocationLogs.AddRange(
                new GpsLocationLog
                {
                    OrgId = org,
                    GpsCode = "GPS-2026-0001",
                    Vin = "DEMOVIN00000001",
                    Latitude = 20.2520m,
                    Longitude = 105.9750m,
                    SpeedKmH = 0,
                    BatteryVolt = 12.8m,
                    EngineStatus = "Off",
                    Address = "Bãi lưu kho YARD-A1 Nhà máy Ninh Bình",
                    IsInGeofence = true,
                    RecordedAt = DateTime.Now.AddMinutes(-5)
                },
                new GpsLocationLog
                {
                    OrgId = org,
                    GpsCode = "GPS-2026-0002",
                    Vin = "DEMOVIN00000002",
                    Latitude = 20.2535m,
                    Longitude = 105.9762m,
                    SpeedKmH = 0,
                    BatteryVolt = 12.6m,
                    EngineStatus = "Off",
                    Address = "Bãi lưu kho YARD-B2 Nhà máy Ninh Bình",
                    IsInGeofence = true,
                    RecordedAt = DateTime.Now.AddMinutes(-12)
                }
            );

            var v1Gps = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Gps != null)
            {
                v1Gps.IsGpsInstalled = true;
                v1Gps.GpsCode = "GPS-2026-0001";
                v1Gps.GpsInstallDate = DateTime.Now.AddDays(-8);
                v1Gps.LastGpsLatitude = 20.2520m;
                v1Gps.LastGpsLongitude = 105.9750m;
                v1Gps.LastGpsAddress = "Bãi lưu kho YARD-A1 Nhà máy Ninh Bình";
                v1Gps.LastGpsSpeed = 0;
                v1Gps.LastGpsBatteryVolt = 12.8m;
                v1Gps.LastGpsSignalTime = DateTime.Now.AddMinutes(-5);
                v1Gps.GpsDeviceCount = 1;
            }

            var v2Gps = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Gps != null)
            {
                v2Gps.IsGpsInstalled = true;
                v2Gps.GpsCode = "GPS-2026-0002";
                v2Gps.GpsInstallDate = DateTime.Now.AddDays(-8);
                v2Gps.LastGpsLatitude = 20.2535m;
                v2Gps.LastGpsLongitude = 105.9762m;
                v2Gps.LastGpsAddress = "Bãi lưu kho YARD-B2 Nhà máy Ninh Bình";
                v2Gps.LastGpsSpeed = 0;
                v2Gps.LastGpsBatteryVolt = 12.6m;
                v2Gps.LastGpsSignalTime = DateTime.Now.AddMinutes(-12);
                v2Gps.GpsDeviceCount = 1;
            }
        }
        if (!await db.ServiceCavities.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var c1 = new ServiceCavity
            {
                OrgId = org,
                CavityNo = "BAY-01",
                CavityNoUser = "KHOANG-SCC-01",
                CavityName = "Khoang sửa chữa chung 01 (Cầu 2 trụ)",
                DealerCode = "DLR-HN01",
                CavityType = "GeneralRepair",
                Status = "Occupied",
                LiftType = "2PostLift",
                MaxPayloadKg = 4000,
                CurrentVin = "DEMOVIN00000001",
                CurrentModel = "Accent 1.4 AT",
                CurrentPlateNo = "30A-999.88",
                CurrentRoNo = "RO202603-001",
                CurrentAppNo = "APP-202603-001",
                CurrentTechnician = "Nguyễn Văn Hùng (KTV Bậc 4)",
                CurrentAdvisor = "Trần Đình Long (CVDV)",
                CurrentWorkItem = "Bảo dưỡng định kỳ 20.000 km & Cân bằng động lốp",
                OccupiedAt = DateTime.Now.AddHours(-1),
                EstimatedReleaseAt = DateTime.Now.AddHours(1),
                StartUseDate = DateTime.Now.AddMonths(-12),
                IsActive = true,
                Remark = "Khoang cầu 2 trụ Bisonic 4 tấn tiêu chuẩn Hyundai OEM",
                CreatedBy = "admin"
            };

            var c2 = new ServiceCavity
            {
                OrgId = org,
                CavityNo = "EM-01",
                CavityNoUser = "KHOANG-EM-01",
                CavityName = "Khoang bảo dưỡng nhanh EM (Express Maintenance 60p)",
                DealerCode = "DLR-HN01",
                CavityType = "QuickService",
                Status = "Available",
                LiftType = "ScissorLift",
                MaxPayloadKg = 3500,
                StartUseDate = DateTime.Now.AddMonths(-6),
                IsActive = true,
                Remark = "Khoang phục vụ dịch vụ bảo dưỡng nhanh 2 KTV phối hợp",
                CreatedBy = "admin"
            };

            var c3 = new ServiceCavity
            {
                OrgId = org,
                CavityNo = "BP-01",
                CavityNoUser = "BUONG-SON-01",
                CavityName = "Buồng sơn sấy nhiệt công nghệ cao BP 01",
                DealerCode = "DLR-HN01",
                CavityType = "BodyPaint",
                Status = "Available",
                LiftType = "PaintBooth",
                MaxPayloadKg = 5000,
                StartUseDate = DateTime.Now.AddMonths(-18),
                IsActive = true,
                Remark = "Buồng sơn sấy đối lưu nhiệt Blowtherm chuẩn HMC",
                CreatedBy = "admin"
            };

            var c4 = new ServiceCavity
            {
                OrgId = org,
                CavityNo = "PDI-01",
                CavityNoUser = "KHOANG-PDI-01",
                CavityName = "Khoang kiểm tra chất lượng tiền bàn giao PDI",
                DealerCode = "DLR-HN01",
                CavityType = "PDIInspection",
                Status = "Available",
                LiftType = "GroundBay",
                MaxPayloadKg = 3500,
                StartUseDate = DateTime.Now.AddMonths(-10),
                IsActive = true,
                Remark = "Khoang kiểm tra chức năng hệ thống điện tử & quét lỗi OBD",
                CreatedBy = "admin"
            };

            var c5 = new ServiceCavity
            {
                OrgId = org,
                CavityNo = "WASH-01",
                CavityNoUser = "KHOANG-RUA-01",
                CavityName = "Khoang rửa xe & Vệ sinh nội thất Car Care",
                DealerCode = "DLR-HN01",
                CavityType = "Washing",
                Status = "Available",
                LiftType = "WashBay",
                MaxPayloadKg = 4000,
                StartUseDate = DateTime.Now.AddMonths(-24),
                IsActive = true,
                Remark = "Hệ thống rửa xe bọt tuyết và cầu xịt gầm áp lực cao",
                CreatedBy = "admin"
            };

            db.ServiceCavities.AddRange(c1, c2, c3, c4, c5);
            await db.SaveChangesAsync();

            db.CavityDispatchLogs.Add(new CavityDispatchLog
            {
                OrgId = org,
                CavityId = c1.Id,
                CavityNo = c1.CavityNo,
                DealerCode = c1.DealerCode,
                DispatchNo = "DSP-202603-0001",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                PlateNo = "30A-999.88",
                RoNo = "RO202603-001",
                AppNo = "APP-202603-001",
                DispatchType = "CheckIn",
                Technician = "Nguyễn Văn Hùng",
                ServiceAdvisor = "Trần Đình Long",
                WorkDescription = "Bảo dưỡng định kỳ 20.000 km & Cân bằng động lốp",
                CheckInTime = DateTime.Now.AddHours(-1),
                Status = "InCavity",
                CreatedBy = "admin",
                CreatedAt = DateTime.Now.AddHours(-1)
            });

            var v1Cavity = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Cavity != null)
            {
                v1Cavity.LastCavityNo = "BAY-01";
                v1Cavity.LastCavityName = "Khoang sửa chữa chung 01 (Cầu 2 trụ)";
                v1Cavity.LastCavityDate = DateTime.Now.AddHours(-1);
                v1Cavity.CavityVisitCount = 1;
            }
        }

        if (!await db.StoragePayments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var sp1 = new StoragePayment
            {
                OrgId = org,
                PaymentStorageNo = "STP202603-001",
                PaymentStorageNoUser = "BK-LK/2026/03/TCV-01",
                PmtMonth = "2026-03",
                StorageCode = "TCV_YARD",
                StorageProvider = "TCMS - Thanh Cong Motor Services",
                TotalVehicleCount = 2,
                TotalStorageDays = 30,
                TotalBeforeVAT = 1275000m,
                VatRate = 10,
                TotalVatAmount = 127500m,
                TotalAmount = 1402500m,
                Status = "Settled",
                TCMSSignStatus = "Signed",
                TCMSSignDate = DateTime.Now.AddDays(-2),
                TCMSSignBy = "TCMS.Manager.PhamVanCuong",
                HTVSignStatus = "Signed",
                HTVSignDate = DateTime.Now.AddDays(-2),
                HTVSignBy = "HTV.StorageDirector.LeNgocDuc",
                BankRefNo = "UNC-VCB-20260315-7788",
                PaymentDate = DateTime.Now.AddDays(-1),
                FilePath = "https://doc.hyundai.thanhcong.vn/storage-payments/STP202603-001.pdf",
                Remark = "Bảng kê quyết toán chi phí lưu bãi ô tô tồn kho OEM bãi TCV Ninh Bình kỳ tháng 03/2026",
                CreatedBy = "storage.acc.oem",
                CreatedAt = DateTime.Now.AddDays(-5),
                Approved1By = "CostAccountant.NguyenThanhHa",
                Approved1At = DateTime.Now.AddDays(-4),
                Approved2By = "FinanceDirector.TranMinhDuc",
                Approved2At = DateTime.Now.AddDays(-3),
                SettledBy = "ChiefAccountant.VuThiLan",
                SettledAt = DateTime.Now.AddDays(-1)
            };
            db.StoragePayments.Add(sp1);
            await db.SaveChangesAsync();

            db.StoragePaymentLines.AddRange(
                new StoragePaymentLine
                {
                    OrgId = org,
                    StoragePaymentId = sp1.Id,
                    PaymentStorageNo = sp1.PaymentStorageNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    StorageCodeInit = "YARD-A1",
                    StoreDate = DateTime.Now.AddDays(-25),
                    DealerCode = "DLR-HN01",
                    InCostStorageDate = DateTime.Now.AddDays(-15),
                    OutCostStorageDate = DateTime.Now.AddDays(-1),
                    StorageDays = 15,
                    DailyRate = 35000,
                    CoverDailyRate = 5000,
                    StorageCost = 525000m,
                    CoverCost = 75000m,
                    TotalAmount = 600000m,
                    StorageLevel = "Standard",
                    Status = "Settled",
                    Remark = "Lưu bãi ô A1, có bạt phủ chống nắng mưa ngoài trời"
                },
                new StoragePaymentLine
                {
                    OrgId = org,
                    StoragePaymentId = sp1.Id,
                    PaymentStorageNo = sp1.PaymentStorageNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    StorageCodeInit = "YARD-B2",
                    StoreDate = DateTime.Now.AddDays(-25),
                    DealerCode = "DLR-HN01",
                    InCostStorageDate = DateTime.Now.AddDays(-15),
                    OutCostStorageDate = DateTime.Now.AddDays(-1),
                    StorageDays = 15,
                    DailyRate = 45000,
                    CoverDailyRate = 0,
                    StorageCost = 675000m,
                    CoverCost = 0,
                    TotalAmount = 675000m,
                    StorageLevel = "Standard",
                    Status = "Settled",
                    Remark = "Lưu bãi ô B2 tiêu chuẩn"
                }
            );

            var v1Storage = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Storage != null)
            {
                v1Storage.IsStoragePaid = true;
                v1Storage.StoragePaidAmount = 600000m;
                v1Storage.LastStoragePaymentNo = sp1.PaymentStorageNo;
                v1Storage.LastStoragePaymentDate = sp1.PaymentDate;
                v1Storage.StoragePaymentCount = 1;
            }

            var v2Storage = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Storage != null)
            {
                v2Storage.IsStoragePaid = true;
                v2Storage.StoragePaidAmount = 675000m;
                v2Storage.LastStoragePaymentNo = sp1.PaymentStorageNo;
                v2Storage.LastStoragePaymentDate = sp1.PaymentDate;
                v2Storage.StoragePaymentCount = 1;
            }
        }

        if (!await db.AvnPayments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var avn1 = new AvnPayment
            {
                OrgId = org,
                PaymentAVNNo = "AVN-202603-001",
                PaymentAVNNoUser = "BK-AVN/2026/03/MOBIS-01",
                PmtMonth = "2026-03",
                SupplierCode = "MOBIS",
                SupplierName = "Mobis Auto Parts Vietnam",
                TotalVehicleCount = 2,
                TotalBeforeVAT = 18400000m,
                VatRate = 10,
                TotalVatAmount = 1840000m,
                TotalAmount = 20240000m,
                Status = "Settled",
                SupplierSignStatus = "Signed",
                SupplierSignDate = DateTime.Now.AddDays(-2),
                SupplierSignBy = "Mobis.Director.KimMinSoo",
                HTVSignStatus = "Signed",
                HTVSignDate = DateTime.Now.AddDays(-2),
                HTVSignBy = "HTV.TechnicalDirector.NguyenVanNam",
                BankRefNo = "UNC-VCB-20260318-9900",
                PaymentDate = DateTime.Now.AddDays(-1),
                FilePath = "https://doc.hyundai.thanhcong.vn/avn-payments/AVN-202603-001.pdf",
                Remark = "Bảng kê quyết toán chi phí lắp đặt Màn hình AVN & Thẻ bản đồ dẫn đường GPS hãng Mobis kỳ tháng 03/2026",
                CreatedBy = "avn.specialist.oem",
                CreatedAt = DateTime.Now.AddDays(-6),
                Approved1By = "CostAccountant.NguyenThanhHa",
                Approved1At = DateTime.Now.AddDays(-5),
                Approved2By = "PartsDirector.TranVanPhuc",
                Approved2At = DateTime.Now.AddDays(-4),
                SettledBy = "ChiefAccountant.VuThiLan",
                SettledAt = DateTime.Now.AddDays(-1)
            };
            db.AvnPayments.Add(avn1);
            await db.SaveChangesAsync();

            db.AvnPaymentLines.AddRange(
                new AvnPaymentLine
                {
                    OrgId = org,
                    AvnPaymentId = avn1.Id,
                    PaymentAVNNo = avn1.PaymentAVNNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    AvnDeviceCode = "AVN-GEN5-8INCH",
                    AvnSerialNo = "AVN-202603-0001",
                    MapCardSerialNo = "MAP-202603-0001",
                    MapVersion = "VN-MAP-2026.Q1",
                    DevicePrice = 7000000m,
                    MapPrice = 1200000m,
                    InstallationFee = 300000m,
                    AccessoryCost = 200000m,
                    TotalAmount = 8700000m,
                    InStorageDate = DateTime.Now.AddDays(-25),
                    AvnInstallDate = DateTime.Now.AddDays(-20),
                    Status = "Settled",
                    Remark = "Màn hình AVN 8 inch cảm ứng tích hợp Apple CarPlay / Android Auto & Bản đồ Vietmap bản quyền"
                },
                new AvnPaymentLine
                {
                    OrgId = org,
                    AvnPaymentId = avn1.Id,
                    PaymentAVNNo = avn1.PaymentAVNNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    AvnDeviceCode = "AVN-GEN5W-10INCH",
                    AvnSerialNo = "AVN-202603-0002",
                    MapCardSerialNo = "MAP-202603-0002",
                    MapVersion = "VN-MAP-2026.Q1",
                    DevicePrice = 8000000m,
                    MapPrice = 1200000m,
                    InstallationFee = 300000m,
                    AccessoryCost = 200000m,
                    TotalAmount = 9700000m,
                    InStorageDate = DateTime.Now.AddDays(-25),
                    AvnInstallDate = DateTime.Now.AddDays(-20),
                    Status = "Settled",
                    Remark = "Màn hình AVN 10.25 inch độ phân giải cao kết nối Bluelink & Thẻ bản đồ GPS"
                }
            );

            var v1Avn = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Avn != null)
            {
                v1Avn.IsAvnInstalled = true;
                v1Avn.AvnDeviceCode = "AVN-GEN5-8INCH";
                v1Avn.AvnSerialNo = "AVN-202603-0001";
                v1Avn.MapCardSerialNo = "MAP-202603-0001";
                v1Avn.IsAvnPaid = true;
                v1Avn.AvnPaidAmount = 8700000m;
                v1Avn.LastAvnPaymentNo = avn1.PaymentAVNNo;
                v1Avn.LastAvnPaymentDate = avn1.PaymentDate;
                v1Avn.AvnPaymentCount = 1;
            }

            var v2Avn = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Avn != null)
            {
                v2Avn.IsAvnInstalled = true;
                v2Avn.AvnDeviceCode = "AVN-GEN5W-10INCH";
                v2Avn.AvnSerialNo = "AVN-202603-0002";
                v2Avn.MapCardSerialNo = "MAP-202603-0002";
                v2Avn.IsAvnPaid = true;
                v2Avn.AvnPaidAmount = 9700000m;
                v2Avn.LastAvnPaymentNo = avn1.PaymentAVNNo;
                v2Avn.LastAvnPaymentDate = avn1.PaymentDate;
                v2Avn.AvnPaymentCount = 1;
            }
        }

        if (!await db.CustomerTestDrives.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var td1 = new CustomerTestDrive
            {
                OrgId = org,
                DriveTestCode = "DT202603-0001",
                DriveTestCodeUser = "LT/2026/03/HN01-001",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Phạm Văn Đồng",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                SpecCode = "1.4 AT Đặc biệt",
                DrvTestPlateNo = "30E-999.88",
                FullName = "Hoàng Minh Đức",
                PhoneNo = "0987654321",
                Email = "minhduc.hoang@gmail.com",
                CusAddress = "Khu đô thị Ngoại Giao Đoàn, Bắc Từ Liêm, Hà Nội",
                Gender = "Nam",
                BirthYear = 1990,
                RangeAgeCode = "26-35",
                DriverLicenseNo = "GPLX-010190088999",
                LicenseClass = "B2",
                DriveTestType = "Showroom",
                EventName = "Trải nghiệm Hyundai Accent Mới",
                RoutePath = "Showroom Phạm Văn Đồng - Cầu Nhật Tân - Đường Võ Nguyên Giáp - Quay đầu",
                DriveDTime = DateTime.Now.AddDays(-3),
                DurationMinutes = 35,
                OdoStart = 1520,
                OdoEnd = 1545,
                DistanceKm = 25,
                SalesManCode = "TVBH01",
                SalesManName = "Lê Hoàng Long",
                Instructor = "KTV-Chuyên gia Đặng Văn Nam",
                ScoreEngine = 4.8m,
                ScoreHandling = 4.9m,
                ScoreNVH = 4.7m,
                ScoreDesign = 5.0m,
                ScoreFeatures = 5.0m,
                ScoreOverall = 4.9m,
                CustomerFeedback = "Xe vận hành đầm chắc, hộp số mượt mà, màn hình giải trí và phím bấm rất nhạy. Khách rất hài lòng với gói an toàn Hyundai SmartSense.",
                PurchaseIntent = "VeryHigh",
                CompetitorModel = "Toyota Vios, Honda City",
                ExpectedDealDate = DateTime.Now.AddDays(4),
                Status = "Completed",
                Remark = "Khách lái thử cùng vợ, dự định đặt cọc phiên bản 1.4 AT Đặc biệt màu trắng trong tuần",
                CreatedBy = "tvbh.long",
                CreatedAt = DateTime.Now.AddDays(-4),
                ApprovedBy = "Trưởng phòng Bán hàng Trần Văn Thắng",
                ApprovedAt = DateTime.Now.AddDays(-3),
                StartedBy = "Lê Hoàng Long",
                StartedAt = DateTime.Now.AddDays(-3),
                CompletedBy = "Lê Hoàng Long",
                CompletedAt = DateTime.Now.AddDays(-3)
            };

            var td2 = new CustomerTestDrive
            {
                OrgId = org,
                DriveTestCode = "DT202603-0002",
                DriveTestCodeUser = "LT/2026/03/HN01-002",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Phạm Văn Đồng",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                SpecCode = "1.5 Cao cấp 2 tông màu",
                DrvTestPlateNo = "30E-888.66",
                FullName = "Nguyễn Thùy Linh",
                PhoneNo = "0934567890",
                Email = "thuylinh.nguyen@outlook.com",
                CusAddress = "Toà nhà Dolphin Plaza, Mỹ Đình, Nam Từ Liêm, Hà Nội",
                Gender = "Nữ",
                BirthYear = 1993,
                RangeAgeCode = "26-35",
                DriverLicenseNo = "GPLX-010193077666",
                LicenseClass = "B1",
                DriveTestType = "HomeDrive",
                EventName = "Lái thử tại nhà cuối tuần",
                RoutePath = "Khu đô thị Mỹ Đình - Đường vành đai 3 trên cao - Đại lộ Thăng Long",
                DriveDTime = DateTime.Now.AddDays(-1),
                DurationMinutes = 40,
                OdoStart = 2850,
                OdoEnd = 2882,
                DistanceKm = 32,
                SalesManCode = "TVBH02",
                SalesManName = "Trần Thị Mai Anh",
                Instructor = "KTV Đỗ Văn Sơn",
                ScoreEngine = 4.7m,
                ScoreHandling = 4.8m,
                ScoreNVH = 4.6m,
                ScoreDesign = 5.0m,
                ScoreFeatures = 4.9m,
                ScoreOverall = 4.8m,
                CustomerFeedback = "Gầm cao thoáng, quan sát tốt, loa Bose nghe rất hay, cảnh báo điểm mù hiển thị rõ ràng trên gương.",
                PurchaseIntent = "High",
                CompetitorModel = "Kia Seltos, Honda HR-V",
                ExpectedDealDate = DateTime.Now.AddDays(10),
                Status = "Completed",
                Remark = "Khách chọn mua xe phục vụ gia đình đưa đón con đi học, đang chờ ngày tốt để ký hợp đồng",
                CreatedBy = "tvbh.maianh",
                CreatedAt = DateTime.Now.AddDays(-2),
                ApprovedBy = "Trưởng phòng Bán hàng Trần Văn Thắng",
                ApprovedAt = DateTime.Now.AddDays(-1),
                StartedBy = "Trần Thị Mai Anh",
                StartedAt = DateTime.Now.AddDays(-1),
                CompletedBy = "Trần Thị Mai Anh",
                CompletedAt = DateTime.Now.AddDays(-1)
            };

            var td3 = new CustomerTestDrive
            {
                OrgId = org,
                DriveTestCode = "DT202603-0003",
                DriveTestCodeUser = "LT/2026/03/SG01-001",
                DealerCode = "DLR-SG01",
                DealerName = "Hyundai Sài Gòn 01 - Trường Chinh",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                SpecCode = "1.4 AT Tiêu chuẩn",
                DrvTestPlateNo = "51K-999.11",
                FullName = "Phạm Quang Hải",
                PhoneNo = "0978112233",
                Email = "quanghai.pham@gmail.com",
                CusAddress = "Đường Cách Mạng Tháng 8, Quận 10, TP. Hồ Chí Minh",
                Gender = "Nam",
                BirthYear = 1988,
                RangeAgeCode = "36-45",
                DriverLicenseNo = "GPLX-790188055444",
                LicenseClass = "B2",
                DriveTestType = "RoadshowEvent",
                EventName = "Roadshow Hyundai Trải Nghiệm Lái Thử Toàn Quốc 2026",
                RoutePath = "Khu đô thị Sala - Cầu Ba Son - Mai Chí Thọ - Hầm Thủ Thiêm",
                DriveDTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30,
                OdoStart = 1545,
                OdoEnd = null,
                DistanceKm = 0,
                SalesManCode = "TVBH05",
                SalesManName = "Vũ Đình Trọng",
                Instructor = "Chuyên gia Hyundai Lái Xe An Toàn",
                ScoreEngine = null,
                ScoreHandling = null,
                ScoreNVH = null,
                ScoreDesign = null,
                ScoreFeatures = null,
                ScoreOverall = null,
                CustomerFeedback = null,
                PurchaseIntent = "High",
                CompetitorModel = "Mazda 2, Toyota Vios",
                ExpectedDealDate = null,
                Status = "Scheduled",
                Remark = "Đã xác nhận lịch hẹn qua điện thoại với khách hàng, chuẩn bị xe và thẻ đeo sự kiện",
                CreatedBy = "tvbh.trong",
                CreatedAt = DateTime.Now.AddDays(-1),
                ApprovedBy = "Giám đốc Bán hàng Nguyễn Tuấn Kiệt",
                ApprovedAt = DateTime.Now
            };

            db.CustomerTestDrives.AddRange(td1, td2, td3);

            var v1Td = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Td != null)
            {
                v1Td.IsTestCar = true;
                v1Td.LastTestDriveNo = td1.DriveTestCode;
                v1Td.LastTestDriveDate = td1.DriveDTime;
                v1Td.TestDriveCount = 1;
            }

            var v2Td = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Td != null)
            {
                v2Td.IsTestCar = true;
                v2Td.LastTestDriveNo = td2.DriveTestCode;
                v2Td.LastTestDriveDate = td2.DriveDTime;
                v2Td.TestDriveCount = 1;
            }
        }

        if (!await db.TransportPlans.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var tp1 = new TransportPlan
            {
                OrgId = org,
                PlanNo = "TP202603-0001",
                PlanNoUser = "KHVT-2026/03-01",
                PlanMonth = "2026-03",
                PlanDate = DateTime.Now.AddDays(-3),
                StorageCode = "PLANT-HTMV1",
                StorageName = "Kho Tổng Nhà máy HTMV Ninh Bình 1",
                TPType = "Road",
                TotalVehicleCount = 2,
                TotalRealVinCount = 2,
                Status = "Approved",
                Remark = "Kế hoạch điều độ vận tải phân bổ xe đợt 1 tháng 03/2026 từ Nhà máy HTMV Ninh Bình về Hà Nội",
                CreatedBy = "plan.manager",
                CreatedAt = DateTime.Now.AddDays(-3),
                ApprovedBy = "LogisticsDirector.TranVanHai",
                ApprovedAt = DateTime.Now.AddDays(-2),
                ExecutedBy = "dispatcher.quang",
                ExecutedAt = DateTime.Now.AddDays(-2)
            };
            db.TransportPlans.Add(tp1);
            await db.SaveChangesAsync();

            db.TransportPlanLines.AddRange(
                new TransportPlanLine
                {
                    OrgId = org,
                    TransportPlanId = tp1.Id,
                    PlanNo = tp1.PlanNo,
                    LineIndex = 1,
                    VINPlan = "PLN-ACC-202603-001",
                    Vin = "DEMOVIN00000001",
                    FlagRealVin = true,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    SpecDescription = "Accent 1.4L Số tự động bản Đặc biệt",
                    ColorCode = "NWAC",
                    ColorName = "Trắng ngọc trai",
                    StorageCode = "PLANT-HTMV1",
                    DealerCode = "DLR-HN01",
                    DealerName = "Hyundai Hà Nội 01",
                    FProvinceCode = "NB",
                    FProvinceName = "Ninh Bình",
                    FDistrictCode = "GV",
                    FDistrictName = "Gia Viễn",
                    TProvinceCode = "HN",
                    TProvinceName = "Hà Nội",
                    TDistrictCode = "CG",
                    TDistrictName = "Cầu Giấy",
                    TransporterCode = "NYK",
                    TransporterName = "Công ty TNHH Vận tải Hàng hải NYK Việt Nam",
                    TruckPlateNo = "29C-888.99",
                    DriverName = "Lê Hồng Sơn",
                    DriverPhone = "0912345678",
                    CQStartDate = DateTime.Now.AddDays(-4),
                    ExpectedDate = DateTime.Now.AddDays(1),
                    ActualDepartureDate = DateTime.Now.AddDays(-1),
                    TPStatus = "Finished",
                    TransporterStatus = "Confirmed",
                    TransporterAppDate = DateTime.Now.AddDays(-2),
                    TransporterAppBy = "NYK.Dispatcher.NguyenVanManh",
                    Status = "Approved",
                    Remark = "Đã gán VIN thật RVIN và nhà xe NYK đã điều phối xe lồng"
                },
                new TransportPlanLine
                {
                    OrgId = org,
                    TransportPlanId = tp1.Id,
                    PlanNo = tp1.PlanNo,
                    LineIndex = 2,
                    VINPlan = "PLN-CRE-202603-002",
                    Vin = "DEMOVIN00000002",
                    FlagRealVin = true,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    SpecDescription = "Creta 1.5L Cao cấp phối 2 màu thể thao",
                    ColorCode = "SAW",
                    ColorName = "Đen ánh kim",
                    StorageCode = "PLANT-HTMV1",
                    DealerCode = "DLR-HN01",
                    DealerName = "Hyundai Hà Nội 01",
                    FProvinceCode = "NB",
                    FProvinceName = "Ninh Bình",
                    FDistrictCode = "GV",
                    FDistrictName = "Gia Viễn",
                    TProvinceCode = "HN",
                    TProvinceName = "Hà Nội",
                    TDistrictCode = "CG",
                    TDistrictName = "Cầu Giấy",
                    TransporterCode = "NYK",
                    TransporterName = "Công ty TNHH Vận tải Hàng hải NYK Việt Nam",
                    TruckPlateNo = "29C-888.99",
                    DriverName = "Lê Hồng Sơn",
                    DriverPhone = "0912345678",
                    CQStartDate = DateTime.Now.AddDays(-4),
                    ExpectedDate = DateTime.Now.AddDays(1),
                    ActualDepartureDate = DateTime.Now.AddDays(-1),
                    TPStatus = "Finished",
                    TransporterStatus = "Confirmed",
                    TransporterAppDate = DateTime.Now.AddDays(-2),
                    TransporterAppBy = "NYK.Dispatcher.NguyenVanManh",
                    Status = "Approved",
                    Remark = "Đã gán VIN thật RVIN và nhà xe NYK đã điều phối xe lồng"
                }
            );

            var tp2 = new TransportPlan
            {
                OrgId = org,
                PlanNo = "TP202603-0002",
                PlanNoUser = "KHVT-2026/03-02",
                PlanMonth = "2026-03",
                PlanDate = DateTime.Now.AddDays(-1),
                StorageCode = "PLANT-HTMV2",
                StorageName = "Kho Tổng Nhà máy HTMV Ninh Bình 2",
                TPType = "Road",
                TotalVehicleCount = 2,
                TotalRealVinCount = 1,
                Status = "Draft",
                Remark = "Kế hoạch điều độ vận tải phân bổ xe đợt 2 tháng 03/2026 khu vực miền Trung & Nam",
                CreatedBy = "plan.specialist",
                CreatedAt = DateTime.Now.AddDays(-1)
            };
            db.TransportPlans.Add(tp2);
            await db.SaveChangesAsync();

            db.TransportPlanLines.AddRange(
                new TransportPlanLine
                {
                    OrgId = org,
                    TransportPlanId = tp2.Id,
                    PlanNo = tp2.PlanNo,
                    LineIndex = 1,
                    VINPlan = "PLN-POR-202603-003",
                    Vin = "DEMOVIN00000003",
                    FlagRealVin = true,
                    Model = "Hyundai New Porter H150",
                    SpecCode = "H150 Thùng Bạt",
                    SpecDescription = "Xe tải nhẹ H150 1.5 tấn thùng mui bạt",
                    ColorCode = "NWAC",
                    ColorName = "Trắng",
                    StorageCode = "BODY-SHOP-01",
                    DealerCode = "DLR-DN01",
                    DealerName = "Hyundai Đà Nẵng",
                    FProvinceCode = "NB",
                    FProvinceName = "Ninh Bình",
                    FDistrictCode = "GV",
                    FDistrictName = "Gia Viễn",
                    TProvinceCode = "DN",
                    TProvinceName = "Đà Nẵng",
                    TDistrictCode = "HC",
                    TDistrictName = "Hải Châu",
                    TransporterCode = "TRACO",
                    TransporterName = "Công ty CP Vận tải Traco Logistics",
                    CQStartDate = DateTime.Now.AddDays(-2),
                    ExpectedDate = DateTime.Now.AddDays(3),
                    TPStatus = "ApprovedByPlan",
                    TransporterStatus = "Pending",
                    Status = "Pending",
                    Remark = "Đã map VIN xe thương mại chassis đóng thùng"
                },
                new TransportPlanLine
                {
                    OrgId = org,
                    TransportPlanId = tp2.Id,
                    PlanNo = tp2.PlanNo,
                    LineIndex = 2,
                    VINPlan = "PLN-EX8-202603-004",
                    Vin = null,
                    FlagRealVin = false,
                    Model = "Hyundai Mighty EX8 GTL",
                    SpecCode = "EX8 GTL Thùng Lạnh",
                    SpecDescription = "Xe tải trung Mighty EX8 GTL thùng đông lạnh",
                    ColorCode = "BU01",
                    ColorName = "Xanh",
                    StorageCode = "BODY-SHOP-01",
                    DealerCode = "DLR-SG01",
                    DealerName = "Hyundai Sài Gòn 01",
                    FProvinceCode = "NB",
                    FProvinceName = "Ninh Bình",
                    FDistrictCode = "GV",
                    FDistrictName = "Gia Viễn",
                    TProvinceCode = "HCM",
                    TProvinceName = "Hồ Chí Minh",
                    TDistrictCode = "TB",
                    TDistrictName = "Tân Bình",
                    TransporterCode = "VINAFCO",
                    TransporterName = "Công ty CP Vinafco Logistics",
                    CQStartDate = DateTime.Now.AddDays(-1),
                    ExpectedDate = DateTime.Now.AddDays(4),
                    TPStatus = "Pending",
                    TransporterStatus = "Pending",
                    Status = "Pending",
                    Remark = "Đang chờ xưởng KCS hoàn tất nghiệm thu thùng đông lạnh để map VIN thật"
                }
            );

            var v1Tp = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Tp != null)
            {
                v1Tp.LastTranspPlanNo = tp1.PlanNo;
                v1Tp.LastTranspPlanDate = tp1.PlanDate;
                v1Tp.TranspPlanCount = 1;
            }

            var v2Tp = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Tp != null)
            {
                v2Tp.LastTranspPlanNo = tp1.PlanNo;
                v2Tp.LastTranspPlanDate = tp1.PlanDate;
                v2Tp.TranspPlanCount = 1;
            }

            var v3Tp = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000003");
            if (v3Tp != null)
            {
                v3Tp.LastTranspPlanNo = tp2.PlanNo;
                v3Tp.LastTranspPlanDate = tp2.PlanDate;
                v3Tp.TranspPlanCount = 1;
            }
        }

        if (!await db.GpsPayments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var gps1 = new GpsPayment
            {
                OrgId = org,
                PaymentGPSNo = "GPS-202604-001",
                PaymentGPSNoUser = "TTGPS-2026-04-VLC",
                PmtMonth = "2026-04",
                SupplierCode = "VELOCA",
                SupplierName = "Công ty Cổ phần Công nghệ Veloca",
                TotalVehicleCount = 2,
                TotalBeforeVAT = 1000000m,
                VatRate = 10m,
                TotalVatAmount = 100000m,
                TotalAmount = 1100000m,
                Status = "Settled",
                TCMSSignStatus = "Signed",
                TCMSSignDate = DateTime.Now.AddDays(-2),
                TCMSSignBy = "Đại diện Veloca - Giám đốc kỹ thuật",
                HTVSignStatus = "Signed",
                HTVSignDate = DateTime.Now.AddDays(-2),
                HTVSignBy = "Đại diện HTV - Trưởng phòng Phụ tùng",
                BankRefNo = "UNC-VCB-20260430-8812",
                PaymentDate = DateTime.Now.AddDays(-1),
                FilePath = "/documents/gps-payments/GPS-202604-001-signed.pdf",
                Remark = "Quyết toán chi phí thiết bị định vị GPS và cước SIM 4G tháng 04/2026 cho bãi xe nhà máy HTMV",
                CreatedBy = "Kế toán vật tư",
                CreatedAt = DateTime.Now.AddDays(-5),
                Approved1By = "Kế toán chi phí",
                Approved1At = DateTime.Now.AddDays(-4),
                Approved2By = "Giám đốc Logistics",
                Approved2At = DateTime.Now.AddDays(-3),
                SettledBy = "Kế toán thanh toán",
                SettledAt = DateTime.Now.AddDays(-1)
            };
            db.GpsPayments.Add(gps1);
            await db.SaveChangesAsync();

            db.GpsPaymentLines.AddRange(
                new GpsPaymentLine
                {
                    OrgId = org,
                    GpsPaymentId = gps1.Id,
                    PaymentGPSNo = gps1.PaymentGPSNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "SantaFe",
                    SpecCode = "2.5 HTRAC Cao Cấp",
                    EngineNo = "G4KP123456",
                    Color = "Trắng Ngọc Trai",
                    GpsCode = "GPS-VELOCA-001",
                    SimCardNo = "09820260001",
                    ImeiNo = "8620260400000001",
                    CostGPSStartDate = new DateTime(2026, 4, 1),
                    CostGPSEndDate = new DateTime(2026, 4, 30),
                    PlanCostGPSDate = 30,
                    DeductDate = 0,
                    ActualCostGPSDate = 30,
                    DailyRate = 15000m,
                    SimDataFee = 50000m,
                    AmountGPS = 500000m,
                    ContractGPS = "HD-GPS-VELOCA-2026",
                    InStorageDate = DateTime.Now.AddDays(-45),
                    Status = "Settled",
                    Remark = "Định vị bãi xe nhà máy HTMV 1"
                },
                new GpsPaymentLine
                {
                    OrgId = org,
                    GpsPaymentId = gps1.Id,
                    PaymentGPSNo = gps1.PaymentGPSNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Tucson",
                    SpecCode = "1.6T Turbo HTRAC",
                    EngineNo = "G4FP654321",
                    Color = "Đen Sang Trọng",
                    GpsCode = "GPS-VELOCA-002",
                    SimCardNo = "09820260002",
                    ImeiNo = "8620260400000002",
                    CostGPSStartDate = new DateTime(2026, 4, 1),
                    CostGPSEndDate = new DateTime(2026, 4, 30),
                    PlanCostGPSDate = 30,
                    DeductDate = 0,
                    ActualCostGPSDate = 30,
                    DailyRate = 15000m,
                    SimDataFee = 50000m,
                    AmountGPS = 500000m,
                    ContractGPS = "HD-GPS-VELOCA-2026",
                    InStorageDate = DateTime.Now.AddDays(-40),
                    Status = "Settled",
                    Remark = "Định vị xe vận chuyển xe lồng"
                }
            );

            var gps2 = new GpsPayment
            {
                OrgId = org,
                PaymentGPSNo = "GPS-202605-001",
                PaymentGPSNoUser = "TTGPS-2026-05-VTT",
                PmtMonth = "2026-05",
                SupplierCode = "VIETTEL",
                SupplierName = "Viettel Telecom - Chi nhánh Doanh nghiệp",
                TotalVehicleCount = 1,
                TotalBeforeVAT = 500000m,
                VatRate = 10m,
                TotalVatAmount = 50000m,
                TotalAmount = 550000m,
                Status = "Approved2",
                TCMSSignStatus = "Unsigned",
                HTVSignStatus = "Unsigned",
                Remark = "Đợt thanh toán cước thiết bị định vị GPS & data 4G tháng 05/2026",
                CreatedBy = "Kế toán vật tư",
                CreatedAt = DateTime.Now.AddDays(-2),
                Approved1By = "Kế toán chi phí",
                Approved1At = DateTime.Now.AddDays(-1),
                Approved2By = "Giám đốc Logistics",
                Approved2At = DateTime.Now
            };
            db.GpsPayments.Add(gps2);

            db.GpsPaymentLines.Add(
                new GpsPaymentLine
                {
                    OrgId = org,
                    GpsPaymentId = gps2.Id,
                    PaymentGPSNo = gps2.PaymentGPSNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000003",
                    Model = "Accent",
                    SpecCode = "1.5 AT Đặc Biệt",
                    EngineNo = "G4LC789012",
                    Color = "Đỏ Mê Hoặc",
                    GpsCode = "GPS-VTT-001",
                    SimCardNo = "09820260003",
                    ImeiNo = "8620260500000003",
                    CostGPSStartDate = new DateTime(2026, 5, 1),
                    CostGPSEndDate = new DateTime(2026, 5, 30),
                    PlanCostGPSDate = 30,
                    DeductDate = 0,
                    ActualCostGPSDate = 30,
                    DailyRate = 15000m,
                    SimDataFee = 50000m,
                    AmountGPS = 500000m,
                    ContractGPS = "HD-GPS-VIETTEL-2026",
                    InStorageDate = DateTime.Now.AddDays(-20),
                    Status = "Approved2",
                    Remark = "Định vị xe bãi trung tâm"
                }
            );

            var v1Gps = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Gps != null)
            {
                v1Gps.IsGpsPaid = true;
                v1Gps.GpsPaidAmount = 500000m;
                v1Gps.LastGpsPaymentNo = gps1.PaymentGPSNo;
                v1Gps.LastGpsPaymentDate = gps1.PaymentDate;
                v1Gps.GpsPaymentCount = 1;
            }

            var v2Gps = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Gps != null)
            {
                v2Gps.IsGpsPaid = true;
                v2Gps.GpsPaidAmount = 500000m;
                v2Gps.LastGpsPaymentNo = gps1.PaymentGPSNo;
                v2Gps.LastGpsPaymentDate = gps1.PaymentDate;
                v2Gps.GpsPaymentCount = 1;
            }
        }

        if (!await db.TransportInsurancePayments.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            // Bảng kê 1: Quyết toán tháng 2026-05 với nhà xe NYK & Bảo Việt (Đã Settled)
            var tip1 = new TransportInsurancePayment
            {
                OrgId = org,
                TransportInsNo = "TIP202605-0001",
                TransportInsNoUser = "BK-VT-BH/2026/05/NYK-01",
                PmtMonth = "2026-05",
                TransporterCode = "NYK",
                TransporterName = "Công ty TNHH Vận tải Hàng hải NYK Việt Nam",
                InsuranceCompanyCode = "BAOVIET",
                InsuranceCompanyName = "Tổng Công ty Bảo hiểm Bảo Việt",
                TotalVehicleCount = 2,
                TotalFreightAmount = 5200000m,
                TotalDelayPenalty = 100000m,
                TotalInsuranceFee = 600000m,
                TotalBeforeVAT = 5700000m,
                VatRate = 10m,
                TotalVatAmount = 570000m,
                TotalAmount = 6270000m,
                Status = "Settled",
                TransporterSignStatus = "Signed",
                TransporterSignDate = DateTime.Now.AddDays(-2),
                TransporterSignBy = "nyk.director",
                HTVSignStatus = "Signed",
                HTVSignDate = DateTime.Now.AddDays(-2),
                HTVSignBy = "htv.logistics.lead",
                BankRefNo = "UNC-VCB-202605-00889",
                PaymentDate = DateTime.Now.AddDays(-1),
                Remark = "Quyết toán cước vận tải đường bộ xe lồng và phí bảo hiểm hàng hóa tháng 05/2026 - Tuyến Ninh Bình đi Hà Nội & Hải Phòng",
                CreatedBy = "planner.logistics",
                CreatedAt = DateTime.Now.AddDays(-5),
                Approved1By = "accountant.cost",
                Approved1At = DateTime.Now.AddDays(-4),
                Approved2By = "director.logistics",
                Approved2At = DateTime.Now.AddDays(-3),
                SettledBy = "chief.accountant",
                SettledAt = DateTime.Now.AddDays(-1)
            };
            db.TransportInsurancePayments.Add(tip1);
            await db.SaveChangesAsync();

            db.TransportInsurancePaymentLines.AddRange(
                new TransportInsurancePaymentLine
                {
                    OrgId = org,
                    TransportInsurancePaymentId = tip1.Id,
                    TransportInsNo = tip1.TransportInsNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "SantaFe",
                    SpecCode = "2.5 H-Trac Cao Cấp",
                    EngineNo = "G4KP123456",
                    Color = "Trắng Ngọc Trai",
                    FStorageCode = "PLANT-HTMV1",
                    FProvinceName = "Ninh Bình",
                    TStorageCode = "DLR-HN01",
                    TProvinceName = "Hà Nội",
                    DealerCode = "DLR-HN01",
                    DlvStartDate = DateTime.Now.AddDays(-7),
                    ExpectedDays = 2,
                    ExpectedDlvEndDate = DateTime.Now.AddDays(-5),
                    DlvEndDate = DateTime.Now.AddDays(-5),
                    DelayDate = 0,
                    FreightAmount = 2600000m,
                    PenaltyPerDay = 100000m,
                    DelayPenalty = 0m,
                    CarValue = 650000000m,
                    InsuranceRate = 0.05m,
                    InsuranceFee = 325000m,
                    TotalAmount = 2925000m,
                    DlvMnNo = "DMN260501001",
                    TranspReqType = "OEMToDealer",
                    Status = "Settled",
                    StandardRemark = "Giao đúng hạn SLA",
                    Remark = "Xe kiểm tra ngoại quan hoàn hảo"
                },
                new TransportInsurancePaymentLine
                {
                    OrgId = org,
                    TransportInsurancePaymentId = tip1.Id,
                    TransportInsNo = tip1.TransportInsNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Tucson",
                    SpecCode = "2.0 AT Đặc Biệt",
                    EngineNo = "G4NL654321",
                    Color = "Đen Sang Trọng",
                    FStorageCode = "PLANT-HTMV1",
                    FProvinceName = "Ninh Bình",
                    TStorageCode = "DLR-HP01",
                    TProvinceName = "Hải Phòng",
                    DealerCode = "DLR-HP01",
                    DlvStartDate = DateTime.Now.AddDays(-7),
                    ExpectedDays = 2,
                    ExpectedDlvEndDate = DateTime.Now.AddDays(-5),
                    DlvEndDate = DateTime.Now.AddDays(-4),
                    DelayDate = 1,
                    FreightAmount = 2600000m,
                    PenaltyPerDay = 100000m,
                    DelayPenalty = 100000m,
                    CarValue = 550000000m,
                    InsuranceRate = 0.05m,
                    InsuranceFee = 275000m,
                    TotalAmount = 2775000m,
                    DlvMnNo = "DMN260501002",
                    TranspReqType = "OEMToDealer",
                    Status = "Settled",
                    StandardRemark = "Trễ 1 ngày do thời tiết bão",
                    Remark = "Khấu trừ phạt trễ 1 ngày theo hợp đồng"
                }
            );

            // Bảng kê 2: Quyết toán tháng 2026-05 với nhà xe TRACO & Bảo hiểm PVI (Approved2 / Chờ ký số)
            var tip2 = new TransportInsurancePayment
            {
                OrgId = org,
                TransportInsNo = "TIP202605-0002",
                TransportInsNoUser = "BK-VT-BH/2026/05/TRACO-01",
                PmtMonth = "2026-05",
                TransporterCode = "TRACO",
                TransporterName = "Công ty Cổ phần Vận tải Traco",
                InsuranceCompanyCode = "PVI",
                InsuranceCompanyName = "Tổng Công ty Cổ phần Bảo hiểm PVI",
                TotalVehicleCount = 1,
                TotalFreightAmount = 3500000m,
                TotalDelayPenalty = 0m,
                TotalInsuranceFee = 250000m,
                TotalBeforeVAT = 3750000m,
                VatRate = 10m,
                TotalVatAmount = 375000m,
                TotalAmount = 4125000m,
                Status = "Approved2",
                TransporterSignStatus = "Unsigned",
                HTVSignStatus = "Unsigned",
                Remark = "Đợt vận chuyển tuyến Ninh Bình đi Đà Nẵng",
                CreatedBy = "planner.logistics",
                CreatedAt = DateTime.Now.AddDays(-2),
                Approved1By = "accountant.cost",
                Approved1At = DateTime.Now.AddDays(-1),
                Approved2By = "director.logistics",
                Approved2At = DateTime.Now
            };
            db.TransportInsurancePayments.Add(tip2);
            await db.SaveChangesAsync();

            db.TransportInsurancePaymentLines.Add(
                new TransportInsurancePaymentLine
                {
                    OrgId = org,
                    TransportInsurancePaymentId = tip2.Id,
                    TransportInsNo = tip2.TransportInsNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000003",
                    Model = "Accent",
                    SpecCode = "1.5 AT Đặc Biệt",
                    EngineNo = "G4LC789012",
                    Color = "Đỏ Mê Hoặc",
                    FStorageCode = "PLANT-HTMV1",
                    FProvinceName = "Ninh Bình",
                    TStorageCode = "DLR-DN01",
                    TProvinceName = "Đà Nẵng",
                    DealerCode = "DLR-DN01",
                    DlvStartDate = DateTime.Now.AddDays(-4),
                    ExpectedDays = 3,
                    ExpectedDlvEndDate = DateTime.Now.AddDays(-1),
                    DlvEndDate = DateTime.Now.AddDays(-1),
                    DelayDate = 0,
                    FreightAmount = 3500000m,
                    PenaltyPerDay = 100000m,
                    DelayPenalty = 0m,
                    CarValue = 500000000m,
                    InsuranceRate = 0.05m,
                    InsuranceFee = 250000m,
                    TotalAmount = 3750000m,
                    DlvMnNo = "DMN260502001",
                    TranspReqType = "OEMToDealer",
                    Status = "Approved2",
                    StandardRemark = "Vận chuyển đường dài an toàn",
                    Remark = "Đã hoàn thành bàn giao đại lý"
                }
            );

            var v1Tip = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Tip != null)
            {
                v1Tip.IsTranspInsPaid = true;
                v1Tip.TranspInsPaidAmount = 2925000m;
                v1Tip.LastTranspInsPaymentNo = tip1.TransportInsNo;
                v1Tip.LastTranspInsPaymentDate = tip1.PaymentDate;
                v1Tip.TranspInsPaymentCount = 1;
            }

            var v2Tip = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Tip != null)
            {
                v2Tip.IsTranspInsPaid = true;
                v2Tip.TranspInsPaidAmount = 2775000m;
                v2Tip.LastTranspInsPaymentNo = tip1.TransportInsNo;
                v2Tip.LastTranspInsPaymentDate = tip1.PaymentDate;
                v2Tip.TranspInsPaymentCount = 1;
            }
        }

        // ===== Quản lý Định mức Tồn kho An toàn & Cân đối Kho Đại lý (DealerInventoryThreshold & InventoryAuditRecord) =====
        if (!await db.DealerInventoryThresholds.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var th1 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0001",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-HN01-ACC",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                RegionCode = "MienBac",
                Model = "Accent",
                SpecCode = "1.5 AT Tiêu Chuẩn",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 8,
                TargetInvQty = 18,
                MaxInvQty = 35,
                WarningThresholdPercent = 20m,
                DailySalesRate = 0.8m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho an toàn tháng 05/2026 cho dòng sedan phân khúc B Hyundai Accent tại đại lý Đông Đô",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th2 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0002",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-HN01-STA",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                RegionCode = "MienBac",
                Model = "SantaFe",
                SpecCode = "2.5 H-Trac Cao Cấp",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 5,
                TargetInvQty = 12,
                MaxInvQty = 25,
                WarningThresholdPercent = 20m,
                DailySalesRate = 0.4m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho an toàn SUV hạng D Hyundai SantaFe tại đại lý Đông Đô",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th3 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0003",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-HN02-TUC",
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Phạm Văn Đồng",
                RegionCode = "MienBac",
                Model = "Tucson",
                SpecCode = "1.6 Turbo AWD",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 6,
                TargetInvQty = 14,
                MaxInvQty = 28,
                WarningThresholdPercent = 25m,
                DailySalesRate = 0.5m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho an toàn C-SUV Hyundai Tucson tại đại lý Phạm Văn Đồng",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th4 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0004",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-HCM01-CRE",
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                RegionCode = "MienNam",
                Model = "Creta",
                SpecCode = "1.5 Cao Cấp",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 10,
                TargetInvQty = 22,
                MaxInvQty = 45,
                WarningThresholdPercent = 20m,
                DailySalesRate = 1.0m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho an toàn B-SUV Hyundai Creta khu vực TP.HCM",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th5 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0005",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-DN01-I10",
                DealerCode = "DLR-DN01",
                DealerName = "Hyundai Đà Nẵng",
                RegionCode = "MienTrung",
                Model = "Grand i10",
                SpecCode = "1.2 AT Hatchback",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 6,
                TargetInvQty = 15,
                MaxInvQty = 30,
                WarningThresholdPercent = 20m,
                DailySalesRate = 0.6m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho an toàn xe đô thị cỡ nhỏ A-Hatchback tại miền Trung",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th6 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0006",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-CT01-CUS",
                DealerCode = "DLR-CT01",
                DealerName = "Hyundai Cần Thơ",
                RegionCode = "MienNam",
                Model = "Custin",
                SpecCode = "1.5T Đặc Biệt",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 4,
                TargetInvQty = 8,
                MaxInvQty = 18,
                WarningThresholdPercent = 20m,
                DailySalesRate = 0.3m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Active",
                Remark = "Định mức tồn kho MPV cỡ trung cao cấp Hyundai Custin tại ĐBSCL",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "sales.director",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };

            var th7 = new DealerInventoryThreshold
            {
                OrgId = org,
                ThresholdNo = "TH202605-0007",
                ThresholdNoUser = "QĐ-ĐMTK/2026/05-HN01-ION5",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                RegionCode = "MienBac",
                Model = "Ioniq 5",
                SpecCode = "EV Prestige 72.6 kWh",
                PeriodMonth = 5,
                PeriodYear = 2026,
                MinInvQty = 2,
                TargetInvQty = 5,
                MaxInvQty = 10,
                WarningThresholdPercent = 15m,
                DailySalesRate = 0.15m,
                EffectiveFrom = new DateTime(2026, 5, 1),
                EffectiveTo = new DateTime(2026, 5, 31),
                Status = "Draft",
                Remark = "Dự thảo định mức xe điện thông minh thuần điện Hyundai Ioniq 5",
                CreatedBy = "planner.inventory",
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            db.DealerInventoryThresholds.AddRange(th1, th2, th3, th4, th5, th6, th7);
            await db.SaveChangesAsync();

            // Seed các đợt kiểm kê & đối soát sức khỏe tồn kho thực tế
            var aud1 = new InventoryAuditRecord
            {
                OrgId = org,
                AuditNo = "AUD202605-0001",
                ThresholdId = th1.Id,
                ThresholdNo = th1.ThresholdNo,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                RegionCode = "MienBac",
                Model = "Accent",
                SpecCode = "1.5 AT Tiêu Chuẩn",
                MinInvQty = 8,
                TargetInvQty = 18,
                MaxInvQty = 35,
                InStockCount = 12,
                AllocatedCount = 2,
                InTransitCount = 2,
                TotalOnHand = 16,
                VarianceQty = 8,
                StockFulfillmentRate = 200m,
                DaysOfSupply = 20m,
                HealthStatus = "Optimal",
                RebalanceAction = "NoAction",
                AuditDate = DateTime.Now.AddDays(-1),
                AuditedBy = "system.autoaudit",
                Remark = "Tồn kho thực tế dòng Accent đạt 200% định mức tối thiểu (16/8 xe), sức khỏe tồn kho Đạt chuẩn (Optimal), dự trữ 20 ngày bán hàng."
            };

            var aud2 = new InventoryAuditRecord
            {
                OrgId = org,
                AuditNo = "AUD202605-0002",
                ThresholdId = th2.Id,
                ThresholdNo = th2.ThresholdNo,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                RegionCode = "MienBac",
                Model = "SantaFe",
                SpecCode = "2.5 H-Trac Cao Cấp",
                MinInvQty = 5,
                TargetInvQty = 12,
                MaxInvQty = 25,
                InStockCount = 2,
                AllocatedCount = 1,
                InTransitCount = 0,
                TotalOnHand = 3,
                VarianceQty = -2,
                StockFulfillmentRate = 60m,
                DaysOfSupply = 7.5m,
                HealthStatus = "Shortage",
                RebalanceAction = "TransferIn",
                RecommendedTransferDealer = "DLR-HCM01",
                RecommendedTransferQty = 2,
                AuditDate = DateTime.Now.AddDays(-1),
                AuditedBy = "system.autoaudit",
                Remark = "Cảnh báo thiếu xe SantaFe (Total On-Hand 3/5 xe), tỷ lệ đáp ứng 60% < 100%, đề xuất điều chuyển 2 xe từ Hyundai Sài Gòn sang."
            };

            var aud3 = new InventoryAuditRecord
            {
                OrgId = org,
                AuditNo = "AUD202605-0003",
                ThresholdId = th3.Id,
                ThresholdNo = th3.ThresholdNo,
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Phạm Văn Đồng",
                RegionCode = "MienBac",
                Model = "Tucson",
                SpecCode = "1.6 Turbo AWD",
                MinInvQty = 6,
                TargetInvQty = 14,
                MaxInvQty = 28,
                InStockCount = 7,
                AllocatedCount = 3,
                InTransitCount = 1,
                TotalOnHand = 11,
                VarianceQty = 5,
                StockFulfillmentRate = 183.3m,
                DaysOfSupply = 22m,
                HealthStatus = "Optimal",
                RebalanceAction = "NoAction",
                AuditDate = DateTime.Now.AddDays(-1),
                AuditedBy = "system.autoaudit",
                Remark = "Tồn kho dòng Tucson tại Phạm Văn Đồng ở mức an toàn (11 xe), đáp ứng tốt nhu cầu giao xe."
            };

            var aud4 = new InventoryAuditRecord
            {
                OrgId = org,
                AuditNo = "AUD202605-0004",
                ThresholdId = th4.Id,
                ThresholdNo = th4.ThresholdNo,
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                RegionCode = "MienNam",
                Model = "Creta",
                SpecCode = "1.5 Cao Cấp",
                MinInvQty = 10,
                TargetInvQty = 22,
                MaxInvQty = 45,
                InStockCount = 28,
                AllocatedCount = 10,
                InTransitCount = 12,
                TotalOnHand = 50,
                VarianceQty = 5,
                StockFulfillmentRate = 500m,
                DaysOfSupply = 50m,
                HealthStatus = "Surplus",
                RebalanceAction = "TransferOut",
                RecommendedTransferDealer = "DLR-DN01",
                RecommendedTransferQty = 5,
                AuditDate = DateTime.Now.AddDays(-1),
                AuditedBy = "system.autoaudit",
                Remark = "Cảnh báo vượt trần tồn kho Creta (Total 50/45 xe trần), đề xuất điều chuyển 5 xe hỗ trợ cho đại lý Đà Nẵng và miền Trung."
            };

            db.InventoryAuditRecords.AddRange(aud1, aud2, aud3, aud4);

            // Cập nhật thông tin kiểm kê định mức trên hồ sơ xe VIN
            var v1Th = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Th != null)
            {
                v1Th.LastInventoryAuditDate = DateTime.Now.AddDays(-1);
                v1Th.InventoryAlertStatus = "Shortage";
                v1Th.LastThresholdNo = th2.ThresholdNo;
                v1Th.ThresholdAuditCount = 1;
            }

            var v2Th = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Th != null)
            {
                v2Th.LastInventoryAuditDate = DateTime.Now.AddDays(-1);
                v2Th.InventoryAlertStatus = "Optimal";
                v2Th.LastThresholdNo = th1.ThresholdNo;
                v2Th.ThresholdAuditCount = 1;
            }

            var v3Th = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000003");
            if (v3Th != null)
            {
                v3Th.LastInventoryAuditDate = DateTime.Now.AddDays(-1);
                v3Th.InventoryAlertStatus = "Optimal";
                v3Th.LastThresholdNo = th3.ThresholdNo;
                v3Th.ThresholdAuditCount = 1;
            }
        }

        // ===== Quản lý Khóa Đào tạo, Sát hạch & Cấp Chứng chỉ Chuẩn hóa Nhân sự Đại lý (TrainingCourse & StaffCertificate) =====
        if (!await db.TrainingCourses.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            // Khóa 1: Đào tạo sản phẩm xe mới & Hybrid 2026 (Completed)
            var trn1 = new TrainingCourse
            {
                OrgId = org,
                TrainingCode = "TRN-2026-001",
                TrainingCodeUser = "KĐT/2026/03-SP-SANTAFE",
                CourseName = "Khóa Đào tạo Sản phẩm & Công nghệ Hyundai SantaFe All-New & Tucson Turbo Hybrid 2026",
                TrainingType = "NewProduct",
                Level = "Intermediate",
                Format = "PracticalWorkshop",
                TrainerName = "Nguyễn Văn Hùng - Giảng viên Cao cấp HTV Training Center",
                Location = "Trung tâm Đào tạo Kỹ thuật Hyundai Thành Công Ninh Bình",
                StartDate = DateTime.Now.AddDays(-20),
                EndDate = DateTime.Now.AddDays(-17),
                MaxCapacity = 30,
                TotalEnrolled = 3,
                TotalPassed = 3,
                TotalFailed = 0,
                PassingScore = 70.0m,
                BudgetAmount = 85000000m,
                ActualCost = 78000000m,
                Status = "Completed",
                Remark = "Khóa đào tạo chuyên sâu về hệ truyền động Hybrid SmartStream, gói an toàn chủ động Hyundai SmartSense thế hệ mới và tính năng kết nối Bluelink",
                CreatedBy = "training.coordinator",
                CreatedAt = DateTime.Now.AddDays(-25),
                ApprovedBy = "TrainingDirector.LeVanLong",
                ApprovedAt = DateTime.Now.AddDays(-22),
                CompletedBy = "Nguyễn Văn Hùng",
                CompletedAt = DateTime.Now.AddDays(-17)
            };
            db.TrainingCourses.Add(trn1);
            await db.SaveChangesAsync();

            var enr1_1 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn1.Id,
                TrainingCode = trn1.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-001-001",
                LineIndex = 1,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                StaffCode = "TVBH-01",
                StaffName = "Nguyễn Văn Tuấn",
                StaffEmail = "tuan.nv@hyundaidongdo.com.vn",
                StaffPhone = "0901234567",
                Position = "SalesConsultant",
                AttendancePercent = 100,
                TheoryScore = 92,
                PracticeScore = 96,
                FinalScore = 94.4m,
                EvaluationGrade = "Excellent",
                ResultStatus = "Passed",
                IsCertificateIssued = true,
                CertificateNo = "CERT-2026-SALE-0001",
                CertificateIssueDate = DateTime.Now.AddDays(-17),
                Status = "Certified",
                Remark = "Nắm rất vững thông số kỹ thuật động cơ SmartStream và kỹ năng tư vấn tính năng SmartSense cho khách hàng"
            };

            var enr1_2 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn1.Id,
                TrainingCode = trn1.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-001-002",
                LineIndex = 2,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                StaffCode = "TVBH-02",
                StaffName = "Trần Thị Mai",
                StaffEmail = "mai.tt@hyundaidongdo.com.vn",
                StaffPhone = "0912345678",
                Position = "SalesConsultant",
                AttendancePercent = 100,
                TheoryScore = 85,
                PracticeScore = 88,
                FinalScore = 86.8m,
                EvaluationGrade = "Good",
                ResultStatus = "Passed",
                IsCertificateIssued = true,
                CertificateNo = "CERT-2026-SALE-0002",
                CertificateIssueDate = DateTime.Now.AddDays(-17),
                Status = "Certified",
                Remark = "Kỹ năng thuyết trình sản phẩm và xử lý tình huống so sánh đối thủ rất tốt"
            };

            var enr1_3 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn1.Id,
                TrainingCode = trn1.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-001-003",
                LineIndex = 3,
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                StaffCode = "TVBH-03",
                StaffName = "Lê Hoàng Nam",
                StaffEmail = "nam.lh@hyundaisaigon.com.vn",
                StaffPhone = "0987654321",
                Position = "SalesConsultant",
                AttendancePercent = 100,
                TheoryScore = 78,
                PracticeScore = 82,
                FinalScore = 80.4m,
                EvaluationGrade = "Good",
                ResultStatus = "Passed",
                IsCertificateIssued = true,
                CertificateNo = "CERT-2026-SALE-0003",
                CertificateIssueDate = DateTime.Now.AddDays(-17),
                Status = "Certified",
                Remark = "Đạt chuẩn sát hạch tư vấn bán hàng chuyên nghiệp dòng SUV cao cấp"
            };

            db.TrainingEnrollments.AddRange(enr1_1, enr1_2, enr1_3);

            // Khóa 2: Chuẩn hóa kỹ năng Cố vấn dịch vụ CVDV (Completed)
            var trn2 = new TrainingCourse
            {
                OrgId = org,
                TrainingCode = "TRN-2026-002",
                TrainingCodeUser = "KĐT/2026/03-CVDV-ADV",
                CourseName = "Khóa Sát hạch & Chuẩn hóa Kỹ năng Cố vấn Dịch vụ Chuyên nghiệp Hyundai Service Advisor",
                TrainingType = "ServiceAdvisor",
                Level = "Advanced",
                Format = "OfflineInClass",
                TrainerName = "Trần Đình Trọng - Giám khảo Dịch vụ Quốc tế Hyundai Motor",
                Location = "Showroom & Xưởng Dịch vụ Đào tạo Chuẩn 3S Hà Nội",
                StartDate = DateTime.Now.AddDays(-14),
                EndDate = DateTime.Now.AddDays(-12),
                MaxCapacity = 25,
                TotalEnrolled = 2,
                TotalPassed = 2,
                TotalFailed = 0,
                PassingScore = 75.0m,
                BudgetAmount = 65000000m,
                ActualCost = 61000000m,
                Status = "Completed",
                Remark = "Quy trình tiếp nhận xe 6 bước chuẩn Hyundai, kỹ năng giải thích báo giá dịch vụ, nâng cao chỉ số hài lòng khách hàng CSI",
                CreatedBy = "service.trainer",
                CreatedAt = DateTime.Now.AddDays(-18),
                ApprovedBy = "AfterSalesDirector.PhamQuocBao",
                ApprovedAt = DateTime.Now.AddDays(-16),
                CompletedBy = "Trần Đình Trọng",
                CompletedAt = DateTime.Now.AddDays(-12)
            };
            db.TrainingCourses.Add(trn2);
            await db.SaveChangesAsync();

            var enr2_1 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn2.Id,
                TrainingCode = trn2.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-002-001",
                LineIndex = 1,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                StaffCode = "CVDV-01",
                StaffName = "Phạm Quốc Tuấn",
                StaffEmail = "tuan.pq@hyundaidongdo.com.vn",
                StaffPhone = "0934567890",
                Position = "ServiceAdvisor",
                AttendancePercent = 100,
                TheoryScore = 88,
                PracticeScore = 90,
                FinalScore = 89.2m,
                EvaluationGrade = "Good",
                ResultStatus = "Passed",
                IsCertificateIssued = true,
                CertificateNo = "CERT-2026-SERV-0001",
                CertificateIssueDate = DateTime.Now.AddDays(-12),
                Status = "Certified",
                Remark = "Thao tác phần mềm DMS và giao tiếp tư vấn khách hàng giải thích hạng mục sửa chữa rất chuyên nghiệp"
            };

            var enr2_2 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn2.Id,
                TrainingCode = trn2.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-002-002",
                LineIndex = 2,
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Phạm Văn Đồng",
                StaffCode = "CVDV-02",
                StaffName = "Vũ Hồng Sơn",
                StaffEmail = "son.vh@hyundaiphamvandong.com.vn",
                StaffPhone = "0945678901",
                Position = "ServiceAdvisor",
                AttendancePercent = 100,
                TheoryScore = 90,
                PracticeScore = 92,
                FinalScore = 91.2m,
                EvaluationGrade = "Excellent",
                ResultStatus = "Passed",
                IsCertificateIssued = true,
                CertificateNo = "CERT-2026-SERV-0002",
                CertificateIssueDate = DateTime.Now.AddDays(-12),
                Status = "Certified",
                Remark = "Tư vấn báo giá sửa chữa rõ ràng, xử lý phàn nàn khách hàng xuất sắc"
            };

            db.TrainingEnrollments.AddRange(enr2_1, enr2_2);

            // Khóa 3: Chẩn đoán & Kỹ thuật Xe điện IONIQ EV Master (InProgress)
            var trn3 = new TrainingCourse
            {
                OrgId = org,
                TrainingCode = "TRN-2026-003",
                TrainingCodeUser = "KĐT/2026/03-EV-MASTER",
                CourseName = "Khóa Đào tạo Kỹ thuật Chuyên sâu & Chẩn đoán Pin Cao áp Xe điện Hyundai IONIQ 5 & IONIQ 6 EV Master",
                TrainingType = "EVTechnician",
                Level = "Master",
                Format = "PracticalWorkshop",
                TrainerName = "Park Sung-Hoon - Chuyên gia Đào tạo Kỹ thuật Điện áp Cao Hyundai Motor Company",
                Location = "Trung tâm Đào tạo Kỹ thuật Hyundai Thành Công Ninh Bình",
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(2),
                MaxCapacity = 20,
                TotalEnrolled = 2,
                TotalPassed = 0,
                TotalFailed = 0,
                PassingScore = 80.0m,
                BudgetAmount = 120000000m,
                ActualCost = 45000000m,
                Status = "InProgress",
                Remark = "An toàn làm việc với hệ thống điện 800V E-GMP, tháo lắp mô-đun pin cao áp, chẩn đoán hệ thống biến tần Inverter và động cơ điện đồng bộ nam châm vĩnh cửu",
                CreatedBy = "technical.director",
                CreatedAt = DateTime.Now.AddDays(-10),
                ApprovedBy = "AfterSalesDirector.PhamQuocBao",
                ApprovedAt = DateTime.Now.AddDays(-8)
            };
            db.TrainingCourses.Add(trn3);
            await db.SaveChangesAsync();

            var enr3_1 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn3.Id,
                TrainingCode = trn3.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-003-001",
                LineIndex = 1,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                StaffCode = "KTV-01",
                StaffName = "Hoàng Văn Hưng",
                StaffEmail = "hung.hv@hyundaidongdo.com.vn",
                StaffPhone = "0956789012",
                Position = "Technician",
                AttendancePercent = 100,
                TheoryScore = 86,
                PracticeScore = 0,
                FinalScore = 34.4m,
                EvaluationGrade = "Pending",
                ResultStatus = "Attended",
                IsCertificateIssued = false,
                Status = "InTraining",
                Remark = "Đã hoàn thành xuất sắc phần lý thuyết an toàn điện 800V, đang tiến hành bài thi thực hành xưởng"
            };

            var enr3_2 = new TrainingEnrollment
            {
                OrgId = org,
                TrainingCourseId = trn3.Id,
                TrainingCode = trn3.TrainingCode,
                EnrollmentNo = "ENR-TRN-2026-003-002",
                LineIndex = 2,
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                StaffCode = "KTV-02",
                StaffName = "Đỗ Minh Đức",
                StaffEmail = "duc.dm@hyundaisaigon.com.vn",
                StaffPhone = "0967890123",
                Position = "Technician",
                AttendancePercent = 100,
                TheoryScore = 90,
                PracticeScore = 0,
                FinalScore = 36.0m,
                EvaluationGrade = "Pending",
                ResultStatus = "Attended",
                IsCertificateIssued = false,
                Status = "InTraining",
                Remark = "Đã hoàn thành lý thuyết mạch điều khiển BMS, đang thực hành chẩn đoán GDS-Mobile trên xe IONIQ 5"
            };

            db.TrainingEnrollments.AddRange(enr3_1, enr3_2);

            // Seed danh mục chứng chỉ nhân sự đã cấp (StaffCertificate)
            var cert1 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-SALE-0001",
                CertificateNoUser = "CC-2026/TVBH/HN01-001",
                StaffCode = "TVBH-01",
                StaffName = "Nguyễn Văn Tuấn",
                StaffEmail = "tuan.nv@hyundaidongdo.com.vn",
                StaffPhone = "0901234567",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                Position = "SalesConsultant",
                CertificateType = "SalesConsultant",
                Level = "Gold",
                IssueDate = DateTime.Now.AddDays(-17),
                ExpiryDate = DateTime.Now.AddDays(-17).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2026-001",
                LinkedEnrollmentNo = "ENR-TRN-2026-001-001",
                ScoreAchieved = 94.4m,
                Grade = "Excellent",
                Remark = "Chứng chỉ Tư vấn Bán hàng Chuyên nghiệp Hạng Vàng (Hyundai Certified Sales Master)",
                CreatedAt = DateTime.Now.AddDays(-17)
            };

            var cert2 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-SALE-0002",
                CertificateNoUser = "CC-2026/TVBH/HN01-002",
                StaffCode = "TVBH-02",
                StaffName = "Trần Thị Mai",
                StaffEmail = "mai.tt@hyundaidongdo.com.vn",
                StaffPhone = "0912345678",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                Position = "SalesConsultant",
                CertificateType = "SalesConsultant",
                Level = "Silver",
                IssueDate = DateTime.Now.AddDays(-17),
                ExpiryDate = DateTime.Now.AddDays(-17).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2026-001",
                LinkedEnrollmentNo = "ENR-TRN-2026-001-002",
                ScoreAchieved = 86.8m,
                Grade = "Good",
                Remark = "Chứng chỉ Tư vấn Bán hàng Chuyên nghiệp Hạng Bạc (Hyundai Certified Sales Consultant)",
                CreatedAt = DateTime.Now.AddDays(-17)
            };

            var cert3 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-SALE-0003",
                CertificateNoUser = "CC-2026/TVBH/HCM01-003",
                StaffCode = "TVBH-03",
                StaffName = "Lê Hoàng Nam",
                StaffEmail = "nam.lh@hyundaisaigon.com.vn",
                StaffPhone = "0987654321",
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                Position = "SalesConsultant",
                CertificateType = "SalesConsultant",
                Level = "Silver",
                IssueDate = DateTime.Now.AddDays(-17),
                ExpiryDate = DateTime.Now.AddDays(-17).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2026-001",
                LinkedEnrollmentNo = "ENR-TRN-2026-001-003",
                ScoreAchieved = 80.4m,
                Grade = "Good",
                Remark = "Chứng chỉ Tư vấn Bán hàng Chuyên nghiệp Hạng Bạc (Hyundai Certified Sales Consultant)",
                CreatedAt = DateTime.Now.AddDays(-17)
            };

            var cert4 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-SERV-0001",
                CertificateNoUser = "CC-2026/CVDV/HN01-001",
                StaffCode = "CVDV-01",
                StaffName = "Phạm Quốc Tuấn",
                StaffEmail = "tuan.pq@hyundaidongdo.com.vn",
                StaffPhone = "0934567890",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                Position = "ServiceAdvisor",
                CertificateType = "ServiceAdvisor",
                Level = "Gold",
                IssueDate = DateTime.Now.AddDays(-12),
                ExpiryDate = DateTime.Now.AddDays(-12).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2026-002",
                LinkedEnrollmentNo = "ENR-TRN-2026-002-001",
                ScoreAchieved = 89.2m,
                Grade = "Good",
                Remark = "Chứng chỉ Cố vấn Dịch vụ Chuyên nghiệp Hạng Vàng (Hyundai Certified Service Advisor Gold)",
                CreatedAt = DateTime.Now.AddDays(-12)
            };

            var cert5 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-SERV-0002",
                CertificateNoUser = "CC-2026/CVDV/HN02-002",
                StaffCode = "CVDV-02",
                StaffName = "Vũ Hồng Sơn",
                StaffEmail = "son.vh@hyundaiphamvandong.com.vn",
                StaffPhone = "0945678901",
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Phạm Văn Đồng",
                Position = "ServiceAdvisor",
                CertificateType = "ServiceAdvisor",
                Level = "Gold",
                IssueDate = DateTime.Now.AddDays(-12),
                ExpiryDate = DateTime.Now.AddDays(-12).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2026-002",
                LinkedEnrollmentNo = "ENR-TRN-2026-002-002",
                ScoreAchieved = 91.2m,
                Grade = "Excellent",
                Remark = "Chứng chỉ Cố vấn Dịch vụ Xuất sắc Hạng Vàng (Hyundai Master Service Advisor)",
                CreatedAt = DateTime.Now.AddDays(-12)
            };

            var cert6 = new StaffCertificate
            {
                OrgId = org,
                CertificateNo = "CERT-2026-TECH-0001",
                CertificateNoUser = "CC-2026/KTV/HN01-003",
                StaffCode = "KTV-03",
                StaffName = "Bùi Văn Thắng",
                StaffEmail = "thang.bv@hyundaidongdo.com.vn",
                StaffPhone = "0978901234",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                Position = "Technician",
                CertificateType = "MasterTechnician",
                Level = "Master",
                IssueDate = DateTime.Now.AddMonths(-3),
                ExpiryDate = DateTime.Now.AddMonths(-3).AddYears(2),
                IssuedBy = "Trung tâm Đào tạo Hyundai Thành Công Việt Nam (HTV Training Center)",
                Status = "Active",
                LinkedTrainingCode = "TRN-2025-099",
                LinkedEnrollmentNo = "ENR-TRN-2025-099-001",
                ScoreAchieved = 96.0m,
                Grade = "Excellent",
                Remark = "Chứng chỉ Kỹ thuật viên Trưởng Bậc Thầy (Hyundai Master Diagnostic Technician)",
                CreatedAt = DateTime.Now.AddMonths(-3)
            };

            db.StaffCertificates.AddRange(cert1, cert2, cert3, cert4, cert5, cert6);
        }

        if (!await db.ServicePackages.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            // 1. Gói bảo dưỡng cấp 1 (5.000 km)
            var pkg5k = new ServicePackage
            {
                OrgId = org,
                PackageNo = "PKG-5K-CARE",
                PackageNoUser = "GÓI-BD01-5K",
                PackageName = "Gói Bảo Dưỡng Cấp 1 Tiêu Chuẩn (5.000 KM)",
                DealerCode = "ALL",
                DealerName = "Hyundai Toàn Quốc OEM",
                PackageType = "PeriodicMaintenance",
                ApplicableModel = "ALL",
                MilestoneKm = 5000,
                StandardTakingTimeMinutes = 60,
                ValidityMonths = 12,
                MaxUsageCount = 1,
                IsPublic = true,
                IsUseBasePrice = true,
                TotalLaborAmount = 250000m,
                TotalPartAmount = 750000m,
                OriginalPrice = 1000000m,
                DiscountPercent = 15m,
                PackagePrice = 850000m,
                TotalSubscribedCount = 2,
                TotalUsedCount = 1,
                Status = "Active",
                Description = "Gói bảo dưỡng định kỳ cấp nhỏ 5.000 km: Thay dầu động cơ nhớt tổng hợp, thay lọc dầu, kiểm tra vệ sinh phanh 4 bánh, kiểm tra ắc quy & quét lỗi ECU.",
                Remark = "Áp dụng cho tất cả dòng xe du lịch và SUV Hyundai",
                CreatedBy = "htv.aftersales",
                CreatedAt = DateTime.Now.AddMonths(-2),
                ApprovedBy = "aftersales.director",
                ApprovedAt = DateTime.Now.AddMonths(-2)
            };

            // 2. Gói bảo dưỡng cấp 2 & 3 (20.000 km)
            var pkg20k = new ServicePackage
            {
                OrgId = org,
                PackageNo = "PKG-20K-CARE",
                PackageNoUser = "GÓI-BD03-20K",
                PackageName = "Gói Bảo Dưỡng Cấp Trung 20.000 KM / 1 Năm",
                DealerCode = "ALL",
                DealerName = "Hyundai Toàn Quốc OEM",
                PackageType = "PeriodicMaintenance",
                ApplicableModel = "ALL",
                MilestoneKm = 20000,
                StandardTakingTimeMinutes = 90,
                ValidityMonths = 12,
                MaxUsageCount = 1,
                IsPublic = true,
                IsUseBasePrice = true,
                TotalLaborAmount = 550000m,
                TotalPartAmount = 1450000m,
                OriginalPrice = 2000000m,
                DiscountPercent = 15m,
                PackagePrice = 1700000m,
                TotalSubscribedCount = 1,
                TotalUsedCount = 0,
                Status = "Active",
                Description = "Gói bảo dưỡng cấp trung 20.000 km: Thay dầu, thay lọc dầu, thay lọc gió động cơ & điều hòa than hoạt tính, bảo dưỡng phanh, cân bằng động bánh xe.",
                Remark = "Chuẩn bảo dưỡng định kỳ cấp trung",
                CreatedBy = "htv.aftersales",
                CreatedAt = DateTime.Now.AddMonths(-2),
                ApprovedBy = "aftersales.director",
                ApprovedAt = DateTime.Now.AddMonths(-2)
            };

            // 3. Gói bảo dưỡng lớn toàn diện cấp 4 (40.000 km)
            var pkg40k = new ServicePackage
            {
                OrgId = org,
                PackageNo = "PKG-40K-MAJOR",
                PackageNoUser = "GÓI-BD04-40K",
                PackageName = "Gói Đại Tu Bảo Dưỡng Lớn Cấp 4 (40.000 KM)",
                DealerCode = "ALL",
                DealerName = "Hyundai Toàn Quốc OEM",
                PackageType = "PeriodicMaintenance",
                ApplicableModel = "ALL",
                MilestoneKm = 40000,
                StandardTakingTimeMinutes = 180,
                ValidityMonths = 24,
                MaxUsageCount = 1,
                IsPublic = true,
                IsUseBasePrice = true,
                TotalLaborAmount = 1200000m,
                TotalPartAmount = 3800000m,
                OriginalPrice = 5000000m,
                DiscountPercent = 20m,
                PackagePrice = 4000000m,
                TotalSubscribedCount = 0,
                TotalUsedCount = 0,
                Status = "Active",
                Description = "Gói đại tu bảo dưỡng 40.000 km: Thay dầu động cơ, dầu hộp số tự động ATF, dầu phanh DOT4, nước làm mát Coolant, thay toàn bộ lọc, vệ sinh kim phun buồng đốt.",
                Remark = "Đại tu định kỳ chuyên sâu bảo toàn giá trị xe",
                CreatedBy = "htv.aftersales",
                CreatedAt = DateTime.Now.AddMonths(-2),
                ApprovedBy = "aftersales.director",
                ApprovedAt = DateTime.Now.AddMonths(-2)
            };

            // 4. Gói thay dầu trọn gói 3 năm (6 lượt)
            var pkgOil3y = new ServicePackage
            {
                OrgId = org,
                PackageNo = "PKG-OIL-3Y",
                PackageNoUser = "THẺ-DẦU-3N",
                PackageName = "Thẻ Thay Dầu & Lọc Dầu Trọn Gói 3 Năm (6 Lượt)",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                PackageType = "OilService",
                ApplicableModel = "Accent 1.4 AT",
                MilestoneKm = null,
                StandardTakingTimeMinutes = 45,
                ValidityMonths = 36,
                MaxUsageCount = 6,
                IsPublic = true,
                IsUseBasePrice = true,
                TotalLaborAmount = 900000m,
                TotalPartAmount = 3600000m,
                OriginalPrice = 4500000m,
                DiscountPercent = 25m,
                PackagePrice = 3375000m,
                TotalSubscribedCount = 1,
                TotalUsedCount = 1,
                Status = "Active",
                Description = "Thẻ trả trước 6 lượt thay dầu động cơ chính hãng Fully Synthetic 5W-30 và 6 lọc dầu Mobis trong vòng 36 tháng cho xe Accent.",
                Remark = "Gói bán kèm xe mới lúc bàn giao Deal bán lẻ",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddMonths(-1),
                ApprovedBy = "dlr.servicemanager",
                ApprovedAt = DateTime.Now.AddMonths(-1)
            };

            db.ServicePackages.AddRange(pkg5k, pkg20k, pkg40k, pkgOil3y);
            await db.SaveChangesAsync();

            // Dòng công việc & phụ tùng cho PKG-5K-CARE
            db.ServicePackageLaborLines.AddRange(
                new ServicePackageLaborLine
                {
                    OrgId = org,
                    ServicePackageId = pkg5k.Id,
                    PackageNo = pkg5k.PackageNo,
                    LineIndex = 1,
                    ServiceItemCode = "PM_OIL_CHANGE",
                    ServiceItemName = "Thay dầu động cơ & lọc dầu nhớt",
                    StandardHours = 0.3m,
                    LaborPrice = 350000m,
                    DiscountPercent = 15m,
                    LaborAmount = 89250m,
                    IsMandatory = true,
                    Remark = "Nổ máy xả dầu đáy các-te"
                },
                new ServicePackageLaborLine
                {
                    OrgId = org,
                    ServicePackageId = pkg5k.Id,
                    PackageNo = pkg5k.PackageNo,
                    LineIndex = 2,
                    ServiceItemCode = "PM_BRAKE_CLEAN",
                    ServiceItemName = "Kiểm tra và vệ sinh phanh 4 bánh",
                    StandardHours = 0.3m,
                    LaborPrice = 350000m,
                    DiscountPercent = 15m,
                    LaborAmount = 89250m,
                    IsMandatory = true,
                    Remark = "Vệ sinh má phanh & đĩa phanh"
                },
                new ServicePackageLaborLine
                {
                    OrgId = org,
                    ServicePackageId = pkg5k.Id,
                    PackageNo = pkg5k.PackageNo,
                    LineIndex = 3,
                    ServiceItemCode = "PM_DIAGNOSTIC_OBD",
                    ServiceItemName = "Quét chẩn đoán lỗi chuyên sâu ECU/GDS Mobile",
                    StandardHours = 0.2m,
                    LaborPrice = 350000m,
                    DiscountPercent = 15m,
                    LaborAmount = 59500m,
                    IsMandatory = true,
                    Remark = "Kiểm tra hệ thống điện tử"
                }
            );

            db.ServicePackagePartLines.AddRange(
                new ServicePackagePartLine
                {
                    OrgId = org,
                    ServicePackageId = pkg5k.Id,
                    PackageNo = pkg5k.PackageNo,
                    LineIndex = 1,
                    PartCode = "05100-00441",
                    PartName = "Dầu nhớt tổng hợp Hyundai Fully Synthetic 5W-30",
                    Unit = "Can 4L",
                    Quantity = 1.0m,
                    UnitPrice = 650000m,
                    DiscountPercent = 15m,
                    PartAmount = 552500m,
                    IsMandatory = true,
                    Remark = "Dầu động cơ xăng cao cấp tiêu chuẩn API SP / ILSAC GF-6"
                },
                new ServicePackagePartLine
                {
                    OrgId = org,
                    ServicePackageId = pkg5k.Id,
                    PackageNo = pkg5k.PackageNo,
                    LineIndex = 2,
                    PartCode = "26300-35505",
                    PartName = "Lọc dầu động cơ Mobis chính hãng",
                    Unit = "Cái",
                    Quantity = 1.0m,
                    UnitPrice = 100000m,
                    DiscountPercent = 15m,
                    PartAmount = 85000m,
                    IsMandatory = true,
                    Remark = "Lọc nhớt cao cấp chống cặn bẩn"
                }
            );

            // Thẻ đăng ký gói cho xe Accent DEMOVIN00000001
            var sub1 = new ServicePackageSubscription
            {
                OrgId = org,
                SubscriptionNo = "SUB-2026-0001",
                SubscriptionNoUser = "HĐTG-2026/03/HN01-001",
                PackageCardNo = "CRD-HYUNDAI-2026-001",
                ServicePackageId = pkgOil3y.Id,
                PackageNo = pkgOil3y.PackageNo,
                PackageName = pkgOil3y.PackageName,
                PackageType = pkgOil3y.PackageType,
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                EngineNo = "G4LC0001",
                PlateNo = "30K-999.88",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerEmail = "an.nguyen@gmail.com",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                SalesAdvisor = "TVBH-01",
                PurchaseDate = DateTime.Now.AddDays(-20),
                StartDate = DateTime.Now.AddDays(-20),
                ExpiryDate = DateTime.Now.AddDays(-20).AddYears(3),
                TotalPackagePrice = 3375000m,
                PaidAmount = 3375000m,
                IsPaid = true,
                PaymentMethod = "Cash",
                MaxUsageCount = 6,
                UsedCount = 1,
                RemainingCount = 5,
                TotalSavedAmount = 750000m,
                Status = "Active",
                Remark = "Thẻ bảo dưỡng trọn gói 3 năm kích hoạt kèm bàn giao xe",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-20)
            };

            var sub2 = new ServicePackageSubscription
            {
                OrgId = org,
                SubscriptionNo = "SUB-2026-0002",
                SubscriptionNoUser = "HĐTG-2026/03/HN01-002",
                PackageCardNo = "CRD-HYUNDAI-2026-002",
                ServicePackageId = pkg5k.Id,
                PackageNo = pkg5k.PackageNo,
                PackageName = pkg5k.PackageName,
                PackageType = pkg5k.PackageType,
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                EngineNo = "G4FL0002",
                PlateNo = "30K-888.66",
                CustomerName = "Trần Thị Mai",
                CustomerPhone = "0912345678",
                CustomerEmail = "mai.tran@gmail.com",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Đông Đô",
                SalesAdvisor = "TVBH-02",
                PurchaseDate = DateTime.Now.AddDays(-5),
                StartDate = DateTime.Now.AddDays(-5),
                ExpiryDate = DateTime.Now.AddDays(-5).AddYears(1),
                TotalPackagePrice = 850000m,
                PaidAmount = 850000m,
                IsPaid = true,
                PaymentMethod = "BankTransfer",
                MaxUsageCount = 1,
                UsedCount = 0,
                RemainingCount = 1,
                TotalSavedAmount = 0m,
                Status = "Active",
                Remark = "Khách hàng mua voucher bảo dưỡng cấp 1",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-5)
            };

            db.ServicePackageSubscriptions.AddRange(sub1, sub2);
            await db.SaveChangesAsync();

            // Nhật ký sử dụng gói cho sub1
            var usage1 = new ServicePackageUsage
            {
                OrgId = org,
                UsageNo = "USG-2026-0001",
                SubscriptionId = sub1.Id,
                SubscriptionNo = sub1.SubscriptionNo,
                PackageCardNo = sub1.PackageCardNo,
                PackageNo = sub1.PackageNo,
                PackageName = sub1.PackageName,
                Vin = sub1.Vin,
                Model = sub1.Model,
                PlateNo = sub1.PlateNo,
                DealerCode = sub1.DealerCode,
                DealerName = sub1.DealerName,
                UsageDate = DateTime.Now.AddDays(-5),
                OdoKm = 5120,
                MilestoneUsed = 5000,
                RoNo = "RO-2026-0001",
                CavityNo = "BAY-01",
                Technician = "KTV-03 (Bùi Văn Thắng)",
                ServiceAdvisor = "CVDV-01 (Trần Văn Bình)",
                LaborSavedAmount = 150000m,
                PartSavedAmount = 600000m,
                TotalSavedAmount = 750000m,
                Status = "Confirmed",
                CustomerRating = 5.0m,
                CustomerFeedback = "Dịch vụ nhanh chóng, trừ lượt trên thẻ điện tử thuận tiện, xe chạy êm ái.",
                Remark = "Lượt thay dầu số 1/6 hoàn tất",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-5),
                ConfirmedBy = "dlr.serviceadvisor",
                ConfirmedAt = DateTime.Now.AddDays(-5)
            };

            db.ServicePackageUsages.Add(usage1);

            // Cập nhật Vehicle DEMOVIN00000001
            var veh1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (veh1 != null)
            {
                veh1.LastServicePackageNo = sub1.PackageNo;
                veh1.LastPackageCardNo = sub1.PackageCardNo;
                veh1.ActiveServicePackageCount = 1;
                veh1.PackageUsageCount = 1;
            }

            // Cập nhật Vehicle DEMOVIN00000002
            var veh2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (veh2 != null)
            {
                veh2.LastServicePackageNo = sub2.PackageNo;
                veh2.LastPackageCardNo = sub2.PackageCardNo;
                veh2.ActiveServicePackageCount = 1;
                veh2.PackageUsageCount = 0;
            }
        }

        // ===== Cấu hình Điều kiện & Tự động Phân bổ Sinh Lệnh Giao Xe DO Tự Động (BizHTC.Car / Car_ConditionForDOAuto, Mst_DOATCondition, Car_DeliveryOrderAuto) =====
        if (!await db.DOAutoConditions.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            // Cấu hình 1: Phân bổ giao xe tự động FIFO theo ngày tồn kho nhà máy OEM
            var cond1 = new DOAutoCondition
            {
                OrgId = org,
                ConditionCode = "COND-DO-2026-01",
                ConditionNoUser = "QĐ-ATDO/2026/03-FIFO",
                ConditionName = "Cấu hình phân bổ giao xe tự động FIFO xe tồn bãi nhà máy OEM",
                Description = "Tự động phân bổ và sinh Lệnh giao xe DO cho các đại lý theo thứ tự FIFO ngày xe nhập bãi, áp dụng cho xe đã hoàn tất KCS, thông quan và nghiệm thu PDI.",
                PriorityRule = "FIFO_StoreDate",
                EffectiveFrom = DateTime.Now.AddMonths(-1),
                EffectiveTo = DateTime.Now.AddMonths(2),
                MinDepositPercent = 10m,
                MinPaymentPercent = 80m,
                RequireGuaranteeOrPaid = true,
                RequireQC = true,
                RequireCustomsClearance = false,
                RequireTaxPaid = false,
                RequirePdiPassed = true,
                RequireRedeemed = true,
                RequireGpsInstalled = false,
                MaxBatchVehicleQuota = 50,
                TotalExecutedBatches = 1,
                TotalAllocatedVehicles = 2,
                Status = "Active",
                Remark = "Áp dụng toàn quốc cho các dòng xe Accent, Creta, Tucson, SantaFe",
                CreatedBy = "sales.planner.htv",
                CreatedAt = DateTime.Now.AddMonths(-1),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddMonths(-1)
            };

            // Cấu hình 2: Phân bổ giao xe ưu tiên đại lý thanh toán và bảo lãnh ngân hàng
            var cond2 = new DOAutoCondition
            {
                OrgId = org,
                ConditionCode = "COND-DO-2026-02",
                ConditionNoUser = "QĐ-ATDO/2026/03-PAYMENT",
                ConditionName = "Cấu hình phân bổ xe ưu tiên tỷ lệ thanh toán và bảo lãnh ngân hàng",
                Description = "Ưu tiên cấp xe cho các đại lý đã thanh toán 100% tiền xe hoặc có bảo lãnh ngân hàng còn hạn mức cao, tối ưu hóa dòng tiền và giải phóng tồn kho.",
                PriorityRule = "PaymentRatio_Desc",
                EffectiveFrom = DateTime.Now.AddDays(-15),
                EffectiveTo = DateTime.Now.AddMonths(3),
                MinDepositPercent = 20m,
                MinPaymentPercent = 100m,
                RequireGuaranteeOrPaid = true,
                RequireQC = true,
                RequireCustomsClearance = true,
                RequireTaxPaid = true,
                RequirePdiPassed = true,
                RequireRedeemed = true,
                RequireGpsInstalled = true,
                MaxBatchVehicleQuota = 30,
                TotalExecutedBatches = 0,
                TotalAllocatedVehicles = 0,
                Status = "Active",
                Remark = "Ưu tiên phân bổ dòng xe SUV và xe điện",
                CreatedBy = "sales.planner.htv",
                CreatedAt = DateTime.Now.AddDays(-15),
                ApprovedBy = "SalesDirector.NguyenVanTuan",
                ApprovedAt = DateTime.Now.AddDays(-14)
            };

            db.DOAutoConditions.AddRange(cond1, cond2);
            await db.SaveChangesAsync();

            // Dòng model áp dụng cho cond1
            db.DOAutoConditionLines.AddRange(
                new DOAutoConditionLine
                {
                    OrgId = org,
                    DOAutoConditionId = cond1.Id,
                    ConditionCode = cond1.ConditionCode,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    ColorCode = null,
                    MaxQuotaQty = 30,
                    PriorityRank = 1,
                    Status = "Active",
                    Remark = "Dòng sedan bán chạy nhất"
                },
                new DOAutoConditionLine
                {
                    OrgId = org,
                    DOAutoConditionId = cond1.Id,
                    ConditionCode = cond1.ConditionCode,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    ColorCode = null,
                    MaxQuotaQty = 20,
                    PriorityRank = 2,
                    Status = "Active",
                    Remark = "Dòng B-SUV chủ lực"
                }
            );

            // Dòng đại lý áp dụng cho cond1
            db.DOAutoConditionDealerLines.AddRange(
                new DOAutoConditionDealerLine
                {
                    OrgId = org,
                    DOAutoConditionId = cond1.Id,
                    ConditionCode = cond1.ConditionCode,
                    LineIndex = 1,
                    DealerCode = "DLR-HN01",
                    DealerName = "Hyundai Đông Đô",
                    RegionCode = "MienBac",
                    MaxDealerQuota = 20,
                    TierLevel = "PriorityVIP",
                    Status = "Active",
                    Remark = "Đại lý 3S xuất sắc miền Bắc"
                },
                new DOAutoConditionDealerLine
                {
                    OrgId = org,
                    DOAutoConditionId = cond1.Id,
                    ConditionCode = cond1.ConditionCode,
                    LineIndex = 2,
                    DealerCode = "DLR-HCM01",
                    DealerName = "Hyundai Sài Gòn",
                    RegionCode = "MienNam",
                    MaxDealerQuota = 20,
                    TierLevel = "Tier1",
                    Status = "Active",
                    Remark = "Đại lý trọng điểm miền Nam"
                },
                new DOAutoConditionDealerLine
                {
                    OrgId = org,
                    DOAutoConditionId = cond1.Id,
                    ConditionCode = cond1.ConditionCode,
                    LineIndex = 3,
                    DealerCode = "DLR-DN01",
                    DealerName = "Hyundai Đà Nẵng",
                    RegionCode = "MienTrung",
                    MaxDealerQuota = 10,
                    TierLevel = "Tier2",
                    Status = "Active",
                    Remark = "Đại lý khu vực miền Trung"
                }
            );

            // Đợt chạy phân bổ tự động mẫu ADOB-2026-03-0001
            var batch1 = new AutoDeliveryOrderBatch
            {
                OrgId = org,
                BatchNo = "ADOB-2026-03-0001",
                BatchNoUser = "ĐCPB/2026/03-01",
                BatchDate = DateTime.Now.AddDays(-10),
                ConditionId = cond1.Id,
                ConditionCode = cond1.ConditionCode,
                ConditionName = cond1.ConditionName,
                StorageCode = "ALL",
                TotalScannedVehicles = 4,
                TotalEligibleVehicles = 2,
                TotalAllocatedVehicles = 2,
                TotalSkippedVehicles = 2,
                TotalGeneratedDOs = 1,
                Status = "Confirmed",
                ExecutionMode = "LiveExecution",
                Remark = "Đợt chạy phân bổ tự động đầu tháng 03/2026 sinh lệnh giao xe cho đại lý Hyundai Đông Đô",
                CreatedBy = "system.autodo",
                CreatedAt = DateTime.Now.AddDays(-10),
                ExecutedBy = "dispatcher.quang",
                ExecutedAt = DateTime.Now.AddDays(-10),
                ConfirmedBy = "SalesDirector.NguyenVanTuan",
                ConfirmedAt = DateTime.Now.AddDays(-9)
            };
            db.AutoDeliveryOrderBatches.Add(batch1);
            await db.SaveChangesAsync();

            db.AutoDeliveryOrderBatchLines.AddRange(
                new AutoDeliveryOrderBatchLine
                {
                    OrgId = org,
                    AutoDeliveryOrderBatchId = batch1.Id,
                    BatchNo = batch1.BatchNo,
                    LineIndex = 1,
                    Vin = "DEMOVIN00000001",
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    EngineNo = "G4LC0001",
                    Color = "Trắng",
                    StorageCode = "YARD-A1",
                    DealerCode = "DLR-HN01",
                    DealerName = "Hyundai Đông Đô",
                    SOCode = "SO202603-001",
                    AllocatedDoNo = "DO-AUTO-20260318-DLR-HN01-001",
                    AllocationStatus = "Allocated",
                    EligibilityReason = "Thỏa mãn toàn bộ điều kiện kỹ thuật KCS, PDI và bảo lãnh ngân hàng",
                    IsQCPassed = true,
                    IsCustomsCleared = true,
                    IsTaxPaid = true,
                    IsPdiPaid = true,
                    IsRedeemed = true,
                    IsGpsInstalled = true,
                    IsGuaranteedOrPaid = true,
                    Status = "Delivered",
                    Remark = "Đã giao xe thành công"
                },
                new AutoDeliveryOrderBatchLine
                {
                    OrgId = org,
                    AutoDeliveryOrderBatchId = batch1.Id,
                    BatchNo = batch1.BatchNo,
                    LineIndex = 2,
                    Vin = "DEMOVIN00000002",
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    EngineNo = "G4FL0002",
                    Color = "Đen",
                    StorageCode = "YARD-B2",
                    DealerCode = "DLR-HN01",
                    DealerName = "Hyundai Đông Đô",
                    SOCode = "SO202603-001",
                    AllocatedDoNo = "DO-AUTO-20260318-DLR-HN01-001",
                    AllocationStatus = "Allocated",
                    EligibilityReason = "Thỏa mãn toàn bộ điều kiện kỹ thuật KCS, PDI và bảo lãnh ngân hàng",
                    IsQCPassed = true,
                    IsCustomsCleared = true,
                    IsTaxPaid = true,
                    IsPdiPaid = true,
                    IsRedeemed = true,
                    IsGpsInstalled = true,
                    IsGuaranteedOrPaid = true,
                    Status = "Delivered",
                    Remark = "Đã giao xe thành công"
                },
                new AutoDeliveryOrderBatchLine
                {
                    OrgId = org,
                    AutoDeliveryOrderBatchId = batch1.Id,
                    BatchNo = batch1.BatchNo,
                    LineIndex = 3,
                    Vin = "DEMOVIN00000003",
                    Model = "Hyundai New Porter H150",
                    SpecCode = "H150 Thùng Bạt",
                    Color = "Trắng",
                    StorageCode = "BODY-SHOP-01",
                    AllocationStatus = "Skipped",
                    EligibilityReason = "Dòng xe thương mại Porter H150 không nằm trong danh mục model áp dụng của cấu hình COND-DO-2026-01",
                    IsQCPassed = true,
                    IsCustomsCleared = false,
                    IsTaxPaid = false,
                    IsPdiPaid = false,
                    IsRedeemed = true,
                    IsGpsInstalled = false,
                    IsGuaranteedOrPaid = false,
                    Status = "Skipped",
                    Remark = "Bỏ qua trong đợt chạy xe du lịch"
                },
                new AutoDeliveryOrderBatchLine
                {
                    OrgId = org,
                    AutoDeliveryOrderBatchId = batch1.Id,
                    BatchNo = batch1.BatchNo,
                    LineIndex = 4,
                    Vin = "DEMOVIN00000004",
                    Model = "Hyundai Mighty EX8 GTL",
                    SpecCode = "EX8 GTL Thùng Lạnh",
                    Color = "Xanh",
                    StorageCode = "BODY-SHOP-01",
                    AllocationStatus = "Skipped",
                    EligibilityReason = "Dòng xe tải Mighty EX8 không nằm trong danh mục model áp dụng",
                    IsQCPassed = true,
                    IsCustomsCleared = false,
                    IsTaxPaid = false,
                    IsPdiPaid = false,
                    IsRedeemed = true,
                    IsGpsInstalled = false,
                    IsGuaranteedOrPaid = false,
                    Status = "Skipped",
                    Remark = "Bỏ qua trong đợt chạy xe du lịch"
                }
            );

            // Cập nhật thông tin auto-DO trên hồ sơ xe VIN
            var v1Auto = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Auto != null)
            {
                v1Auto.LastAutoDoNo = "DO-AUTO-20260318-DLR-HN01-001";
                v1Auto.LastAutoDoDate = DateTime.Now.AddDays(-10);
                v1Auto.AutoDoCount = 1;
            }

            var v2Auto = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Auto != null)
            {
                v2Auto.LastAutoDoNo = "DO-AUTO-20260318-DLR-HN01-001";
                v2Auto.LastAutoDoDate = DateTime.Now.AddDays(-10);
                v2Auto.AutoDoCount = 1;
            }
        }

        // ===== Khảo sát Chỉ số Hài lòng Bán hàng & Bàn giao xe mới SSI (BizHTC.DealerSales / DLS_VINSurvey, RptSSI_ICIC) =====
        if (!await db.SalesSatisfactionSurveys.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            var ssi1 = new SalesSatisfactionSurvey
            {
                OrgId = org,
                SurveyNo = "SSI-20260320-0001",
                SurveyNoUser = "KS-SSI-2026/03/HN01-01",
                DealNo = "DEAL202603-001",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                SpecCode = "1.4 AT Đặc biệt",
                EngineNo = "G4LC0001",
                Color = "Trắng",
                PlateNo = "30K-999.88",
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerEmail = "an.nguyen@gmail.com",
                CustomerType = "Individual",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                SalesConsultantCode = "TVBH-HN01-08",
                SalesConsultantName = "Trần Thanh Tâm",
                DeliveryDate = DateTime.Now.AddDays(-7),
                SurveyDate = DateTime.Now.AddDays(-4),
                ContactChannel = "PhoneCall",
                CallAttempts = 1,
                SurveyorStaff = "Nguyễn Minh Phương (ICIC Center)",
                ScoreSalesConsultant = 5.0m,
                ScoreDealershipFacility = 4.8m,
                ScoreDeliveryProcess = 5.0m,
                ScorePaperworkFinance = 4.9m,
                ScoreTimeliness = 5.0m,
                ScoreOverall = 5.0m,
                CalculatedSsiScore = 4.95m,
                SsiIndex1000 = 990,
                IsCleanCarDelivered = true,
                IsFeatureExplained = true,
                IsAdasExplained = true,
                IsAvnBluelinkSetup = true,
                IsOriginalDocsHandedOver = true,
                IsFollowUpCallPromised = true,
                NpsScore = 10,
                NpsCategory = "Promoter",
                CustomerFeedback = "Rất hài lòng với dịch vụ của bạn Tâm TVBH và lễ bàn giao xe trang trọng tại showroom. Đã giới thiệu cho bạn bè đồng nghiệp mua xe.",
                HasComplaint = false,
                Status = "Completed",
                Remark = "Khảo sát SSI thành công xuất sắc, khách hàng đánh giá điểm tối đa.",
                CreatedBy = "icic.agent01",
                CreatedAt = DateTime.Now.AddDays(-4),
                CompletedBy = "Nguyễn Minh Phương (ICIC)",
                CompletedAt = DateTime.Now.AddDays(-4)
            };

            var ssi2 = new SalesSatisfactionSurvey
            {
                OrgId = org,
                SurveyNo = "SSI-20260322-0002",
                SurveyNoUser = "KS-SSI-2026/03/HN01-02",
                DealNo = "DEAL202603-002",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                SpecCode = "1.5 Cao cấp 2 tông màu",
                EngineNo = "G4FL0002",
                Color = "Đen",
                PlateNo = "30K-888.66",
                CustomerName = "Trần Thị Mai",
                CustomerPhone = "0912345678",
                CustomerEmail = "mai.tran@gmail.com",
                CustomerType = "Individual",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01",
                SalesConsultantCode = "TVBH-HN01-12",
                SalesConsultantName = "Lê Văn Hùng",
                DeliveryDate = DateTime.Now.AddDays(-5),
                SurveyDate = DateTime.Now.AddDays(-2),
                ContactChannel = "PhoneCall",
                CallAttempts = 1,
                SurveyorStaff = "Nguyễn Minh Phương (ICIC Center)",
                ScoreSalesConsultant = 4.5m,
                ScoreDealershipFacility = 4.5m,
                ScoreDeliveryProcess = 4.0m,
                ScorePaperworkFinance = 3.5m,
                ScoreTimeliness = 4.0m,
                ScoreOverall = 4.2m,
                CalculatedSsiScore = 4.15m,
                SsiIndex1000 = 830,
                IsCleanCarDelivered = true,
                IsFeatureExplained = true,
                IsAdasExplained = true,
                IsAvnBluelinkSetup = true,
                IsOriginalDocsHandedOver = false,
                IsFollowUpCallPromised = true,
                NpsScore = 8,
                NpsCategory = "Passive",
                CustomerFeedback = "Xe chạy rất đầm và êm. Tuy nhiên thủ tục trả góp ngân hàng và hẹn trả giấy đăng kiểm gốc hơi chậm 2 ngày so với dự kiến.",
                HasComplaint = true,
                ComplaintCategory = "PaperworkDelay",
                ComplaintDetails = "Giấy chứng nhận đăng ký xe và hồ sơ vay gốc ngân hàng chậm 2 ngày làm khách hàng lo lắng khi lưu thông.",
                RemedyAction = "Đại lý đã cử TVBH mang hồ sơ công chứng và giấy hẹn đăng ký đến tận nhà khách hàng, tặng voucher 1.000.000 VNĐ bảo dưỡng kỳ đầu.",
                IsComplaintResolved = true,
                ResolvedBy = "Phạm Hoàng Long (GĐ Bán Hàng DLR-HN01)",
                ResolvedAt = DateTime.Now.AddDays(-1),
                Status = "Completed",
                Remark = "Khiếu nại chậm hồ sơ giấy tờ đã được Ban Giám Đốc đại lý xử lý thỏa đáng và khách hàng hài lòng.",
                CreatedBy = "icic.agent01",
                CreatedAt = DateTime.Now.AddDays(-2),
                CompletedBy = "Nguyễn Minh Phương (ICIC)",
                CompletedAt = DateTime.Now.AddDays(-1)
            };

            db.SalesSatisfactionSurveys.AddRange(ssi1, ssi2);
            await db.SaveChangesAsync();

            // Seed các câu hỏi khảo sát chi tiết
            db.SalesSatisfactionSurveyQuestionLines.AddRange(
                new SalesSatisfactionSurveyQuestionLine
                {
                    OrgId = org,
                    SalesSatisfactionSurveyId = ssi1.Id,
                    SurveyNo = ssi1.SurveyNo,
                    LineIndex = 1,
                    QuestionCode = "Q01_GREETING",
                    QuestionCategory = "SalesConsultant",
                    QuestionText = "Thái độ đón tiếp và tác phong chuyên nghiệp của Tư vấn bán hàng (TVBH)?",
                    Score = 5.0m,
                    AnswerText = "Rất niềm nở, lịch sự và chu đáo",
                    Weight = 1.0m
                },
                new SalesSatisfactionSurveyQuestionLine
                {
                    OrgId = org,
                    SalesSatisfactionSurveyId = ssi1.Id,
                    SurveyNo = ssi1.SurveyNo,
                    LineIndex = 2,
                    QuestionCode = "Q02_NEEDS_ANALYSIS",
                    QuestionCategory = "SalesConsultant",
                    QuestionText = "TVBH có tìm hiểu kỹ nhu cầu sử dụng và tư vấn phiên bản xe phù hợp không?",
                    Score = 5.0m,
                    AnswerText = "Tư vấn rất chuẩn xác nhu cầu gia đình",
                    Weight = 1.0m
                },
                new SalesSatisfactionSurveyQuestionLine
                {
                    OrgId = org,
                    SalesSatisfactionSurveyId = ssi1.Id,
                    SurveyNo = ssi1.SurveyNo,
                    LineIndex = 3,
                    QuestionCode = "Q08_CLEAN_CONDITION",
                    QuestionCategory = "DeliveryProcess",
                    QuestionText = "Xe được vệ sinh sạch sẽ, bóng đẹp và hoàn hảo khi bàn giao?",
                    Score = 5.0m,
                    AnswerText = "Xe sạch tinh tươm, không một vết xước",
                    Weight = 1.0m
                },
                new SalesSatisfactionSurveyQuestionLine
                {
                    OrgId = org,
                    SalesSatisfactionSurveyId = ssi1.Id,
                    SurveyNo = ssi1.SurveyNo,
                    LineIndex = 4,
                    QuestionCode = "Q11_ADAS_DEMO",
                    QuestionCategory = "TechnologyADAS",
                    QuestionText = "TVBH có giải thích chi tiết các tính năng an toàn Hyundai SmartSense / ADAS và AVN Bluelink không?",
                    Score = 5.0m,
                    AnswerText = "Đã hướng dẫn và cài đặt Bluelink trực tiếp trên điện thoại",
                    Weight = 1.0m
                },
                new SalesSatisfactionSurveyQuestionLine
                {
                    OrgId = org,
                    SalesSatisfactionSurveyId = ssi1.Id,
                    SurveyNo = ssi1.SurveyNo,
                    LineIndex = 5,
                    QuestionCode = "Q18_NPS_RECOMMEND",
                    QuestionCategory = "OverallNPS",
                    QuestionText = "Mức độ sẵn sàng giới thiệu Đại lý và dòng xe Hyundai cho bạn bè/người thân?",
                    Score = 5.0m,
                    AnswerText = "10/10 điểm - Chắc chắn sẽ giới thiệu",
                    Weight = 1.0m
                }
            );

            // Cập nhật thông tin khảo sát SSI trên hồ sơ số khung xe VIN
            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastSsiNo = ssi1.SurveyNo;
                v1.LastSsiDate = ssi1.SurveyDate;
                v1.LastSsiScore = ssi1.CalculatedSsiScore;
                v1.LastSsiIndex1000 = ssi1.SsiIndex1000;
                v1.SsiSurveyCount = 1;
            }

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastSsiNo = ssi2.SurveyNo;
                v2.LastSsiDate = ssi2.SurveyDate;
                v2.LastSsiScore = ssi2.CalculatedSsiScore;
                v2.LastSsiIndex1000 = ssi2.SsiIndex1000;
                v2.SsiSurveyCount = 1;
            }
        }

        // ===== Quản lý Khách hàng Tham quan Showroom & Phễu Bán hàng (BizHTC.RetailContract / DLR_CtmVisit, FrmCusVisit / CustomerVisit) =====
        if (!await db.CustomerVisits.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var visit1 = new CustomerVisit
            {
                OrgId = org,
                VisitCode = "VIS202603-0001",
                VisitCodeUser = "CV-2026/03/HN01-01",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                VisitDate = DateTime.Now.AddDays(-3),
                CustomerName = "Nguyễn Văn An",
                CustomerPhone = "0901234567",
                CustomerEmail = "an.nguyen@gmail.com",
                CustomerAddress = "12 Trần Duy Hưng, Cầu Giấy, Hà Nội",
                Gender = "Nam",
                RangeAgeCode = "26-35",
                CustomerType = "Individual",
                InterestedModel = "Accent 1.4 AT",
                SpecCode = "1.4 AT Đặc biệt",
                SpecDescription = "Bản full cao cấp cửa sổ trời, 6 túi khí",
                ColorCode = "NWAC",
                ColorName = "Trắng ngọc trai",
                Vin = "DEMOVIN00000001",
                VisitPurpose = "XemXeMoi",
                LeadSource = "ShowroomWalkIn",
                SalesConsultantCode = "TVBH-HN01-01",
                SalesConsultantName = "Trần Thị Tư Vấn",
                HasTradeIn = true,
                TradeInModel = "Grand i10 1.2 AT 2018",
                TradeInYear = 2018,
                TradeInEstimatedPrice = 280000000m,
                PaymentMethodExpected = "BankLoan",
                LoanPercentExpected = 70m,
                EstimatedPurchaseTime = "TrongTuan",
                PurchaseProbability = "VeryHigh",
                BudgetAmount = 560000000m,
                CompetitorModel = "Toyota Vios 1.5G, Honda City RS",
                IsTestDriveTaken = true,
                LinkedDriveTestCode = "DT202603-0001",
                LinkedDealNo = "DEAL202603-001",
                NextFollowUpDate = DateTime.Now.AddDays(2),
                FollowUpAction = "NegotiateContract",
                CustomerFeedback = "Khách hàng rất thích kiểu dáng mới, màn hình AVN tiếng Việt và gói an toàn SmartSense.",
                Status = "ConvertedToDeal",
                Remark = "Đã ký hợp đồng bán lẻ DEAL202603-001 và nộp cọc 20 triệu",
                CreatedBy = "reception.hn01",
                CreatedAt = DateTime.Now.AddDays(-3),
                CompletedBy = "Trần Thị Tư Vấn",
                CompletedAt = DateTime.Now.AddDays(-1)
            };

            var visit2 = new CustomerVisit
            {
                OrgId = org,
                VisitCode = "VIS202603-0002",
                VisitCodeUser = "CV-2026/03/HN01-02",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                VisitDate = DateTime.Now.AddDays(-2),
                CustomerName = "Trần Thị Bình",
                CustomerPhone = "0912345678",
                CustomerEmail = "binh.tran@yahoo.com",
                CustomerAddress = "88 Nguyễn Trãi, Thanh Xuân, Hà Nội",
                Gender = "Nữ",
                RangeAgeCode = "26-35",
                CustomerType = "Individual",
                InterestedModel = "Creta 1.5 Cao cấp",
                SpecCode = "1.5 Cao cấp 2 tông màu",
                SpecDescription = "Bản 2 tông màu mui đen, hệ thống âm thanh 8 loa Bose",
                ColorCode = "SAW",
                ColorName = "Đen Phantom",
                Vin = "DEMOVIN00000002",
                VisitPurpose = "LaiThu",
                LeadSource = "DigitalAds",
                SalesConsultantCode = "TVBH-HN01-02",
                SalesConsultantName = "Lê Văn Bán Hàng",
                HasTradeIn = false,
                PaymentMethodExpected = "Cash",
                LoanPercentExpected = 0m,
                EstimatedPurchaseTime = "TrongThang",
                PurchaseProbability = "High",
                BudgetAmount = 720000000m,
                CompetitorModel = "Kia Seltos, Honda HR-V",
                IsTestDriveTaken = true,
                LinkedDriveTestCode = "DT202603-0002",
                NextFollowUpDate = DateTime.Now.AddDays(3),
                FollowUpAction = "SendQuotation",
                CustomerFeedback = "Lái thử xe rất êm ái, cách âm tốt, đang so sánh giá lăn bánh với đại lý khác.",
                Status = "Quoted",
                Remark = "Đã gửi báo giá lăn bánh Hà Nội kèm gói quà tặng dán phim cách nhiệt & thảm lót sàn",
                CreatedBy = "reception.hn01",
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            var visit3 = new CustomerVisit
            {
                OrgId = org,
                VisitCode = "VIS202603-0003",
                VisitCodeUser = "CV-2026/03/HN02-01",
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Hà Nội 02 - Long Biên",
                VisitDate = DateTime.Now.AddDays(-1),
                CustomerName = "Phạm Văn Cường",
                CustomerPhone = "0987654321",
                CustomerEmail = "cuong.pham@fpt.com",
                CustomerAddress = "55 Nguyễn Văn Cừ, Long Biên, Hà Nội",
                Gender = "Nam",
                RangeAgeCode = "36-45",
                CustomerType = "Individual",
                InterestedModel = "SantaFe 2.5T Calligraphy",
                SpecCode = "2.5T AWD Calligraphy",
                SpecDescription = "Bản máy xăng tăng áp dẫn động 4 bánh toàn thời gian HTRAC cao cấp nhất",
                ColorCode = "R2P",
                ColorName = "Đỏ đô ánh kim",
                VisitPurpose = "BaoGia",
                LeadSource = "WebsiteHyundai",
                SalesConsultantCode = "TVBH-HN02-01",
                SalesConsultantName = "Hoàng Minh TVBH",
                HasTradeIn = true,
                TradeInModel = "Tucson 2.0 AT 2021",
                TradeInYear = 2021,
                TradeInEstimatedPrice = 750000000m,
                PaymentMethodExpected = "Cash",
                EstimatedPurchaseTime = "1Den3Thang",
                PurchaseProbability = "Medium",
                BudgetAmount = 1450000000m,
                CompetitorModel = "Ford Everest Titanium, Kia Sorento Signature",
                IsTestDriveTaken = false,
                NextFollowUpDate = DateTime.Now.AddDays(5),
                FollowUpAction = "ScheduleTestDrive",
                CustomerFeedback = "Thích thiết kế vuông vức phong cách SUV việt dã thế hệ mới, hẹn cuối tuần mang xe qua nhà lái thử.",
                Status = "InConsultation",
                Remark = "Hẹn lái thử tại nhà ngày thứ Bảy",
                CreatedBy = "reception.hn02",
                CreatedAt = DateTime.Now.AddDays(-1)
            };

            db.CustomerVisits.AddRange(visit1, visit2, visit3);
            await db.SaveChangesAsync();

            db.CustomerVisitActionLogs.AddRange(
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit1.Id,
                    VisitCode = visit1.VisitCode,
                    ActionNo = "ACT-20260301-0001",
                    LineIndex = 1,
                    ActionType = "ShowroomGreeting",
                    ActionDate = DateTime.Now.AddDays(-3),
                    SalesConsultant = "Trần Thị Tư Vấn",
                    DiscussionSummary = "Tiếp đón khách hàng Nguyễn Văn An đến showroom xem xe Accent 1.4 AT. Nguồn: Khách vãng lai.",
                    CustomerResponse = "Quan tâm bản Đặc biệt màu trắng, có nhu cầu đổi xe Grand i10 cũ",
                    NextActionPlan = "Định giá xe cũ và hướng dẫn lái thử xe",
                    NextActionDate = DateTime.Now.AddDays(-3),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit1.Id,
                    VisitCode = visit1.VisitCode,
                    ActionNo = "ACT-20260301-0002",
                    LineIndex = 2,
                    ActionType = "TestDriveDone",
                    ActionDate = DateTime.Now.AddDays(-3),
                    SalesConsultant = "Trần Thị Tư Vấn",
                    DiscussionSummary = "Thực hiện lái thử xe Accent cung đường nội đô Cầu Giấy",
                    CustomerResponse = "Khách hàng hài lòng với cảm giác lái và độ êm",
                    NextActionPlan = "Lập phương án vay ngân hàng 70% và chốt hợp đồng",
                    NextActionDate = DateTime.Now.AddDays(-2),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit1.Id,
                    VisitCode = visit1.VisitCode,
                    ActionNo = "ACT-20260302-0003",
                    LineIndex = 3,
                    ActionType = "DealClosed",
                    ActionDate = DateTime.Now.AddDays(-1),
                    SalesConsultant = "Trần Thị Tư Vấn",
                    DiscussionSummary = "Ký hợp đồng bán lẻ DEAL202603-001, nhận đặt cọc 20.000.000 VNĐ.",
                    CustomerResponse = "Đã hoàn tất thanh toán cọc và hẹn ngày nhận bàn giao xe",
                    NextActionPlan = "Chuẩn bị lễ bàn giao xe và hoàn thiện đăng ký",
                    NextActionDate = DateTime.Now.AddDays(2),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit2.Id,
                    VisitCode = visit2.VisitCode,
                    ActionNo = "ACT-20260302-0001",
                    LineIndex = 1,
                    ActionType = "ShowroomGreeting",
                    ActionDate = DateTime.Now.AddDays(-2),
                    SalesConsultant = "Lê Văn Bán Hàng",
                    DiscussionSummary = "Tiếp đón chị Trần Thị Bình đến xem xe Creta 1.5 Cao cấp qua quảng cáo Facebook.",
                    CustomerResponse = "Quan tâm bản 2 tông màu cao cấp, thích hệ thống loa Bose",
                    NextActionPlan = "Cho khách trải nghiệm lái thử xe thực tế",
                    NextActionDate = DateTime.Now.AddDays(-2),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit2.Id,
                    VisitCode = visit2.VisitCode,
                    ActionNo = "ACT-20260302-0002",
                    LineIndex = 2,
                    ActionType = "QuotationSent",
                    ActionDate = DateTime.Now.AddDays(-2),
                    SalesConsultant = "Lê Văn Bán Hàng",
                    DiscussionSummary = "Gửi bảng báo giá chi tiết lăn bánh xe Creta 1.5 Cao cấp 720 triệu VNĐ kèm quà tặng.",
                    CustomerResponse = "Đang cân nhắc phương án trả thẳng tiền mặt",
                    NextActionPlan = "Gọi điện chăm sóc lại sau 2 ngày",
                    NextActionDate = DateTime.Now.AddDays(1),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new CustomerVisitActionLog
                {
                    OrgId = org,
                    CustomerVisitId = visit3.Id,
                    VisitCode = visit3.VisitCode,
                    ActionNo = "ACT-20260303-0001",
                    LineIndex = 1,
                    ActionType = "ShowroomGreeting",
                    ActionDate = DateTime.Now.AddDays(-1),
                    SalesConsultant = "Hoàng Minh TVBH",
                    DiscussionSummary = "Tiếp đón anh Phạm Văn Cường tìm hiểu xe SantaFe 2.5T Calligraphy.",
                    CustomerResponse = "Đang chạy Tucson 2021, muốn nâng cấp lên SUV 7 chỗ rộng rãi",
                    NextActionPlan = "Lên lịch hẹn mang xe SantaFe lái thử tại nhà vào cuối tuần",
                    NextActionDate = DateTime.Now.AddDays(4),
                    Status = "Completed",
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            );

            var v1 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1 != null)
            {
                v1.LastCustomerVisitNo = visit1.VisitCode;
                v1.LastCustomerVisitDate = visit1.VisitDate;
                v1.CustomerVisitCount = 1;
            }

            var v2 = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2 != null)
            {
                v2.LastCustomerVisitNo = visit2.VisitCode;
                v2.LastCustomerVisitDate = visit2.VisitDate;
                v2.CustomerVisitCount = 1;
            }
        }

        // ===== Quản lý Đề nghị & Quyết toán Chi phí Hỗ trợ Marketing Đại lý Phân phối OEM (BizHTC.Marketing / MKT_MarketingFee) =====
        if (!await db.MarketingActivityTypes.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            db.MarketingActivityTypes.AddRange(
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "OOH", MKTActivityTypeName = "Biển bảng Quảng cáo Ngoài trời (OOH & Billboard)", FlagActive = true, Remark = "Pano, billboard tấm lớn, màn hình LED quảng cáo ngoài trời" },
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "DIGITAL", MKTActivityTypeName = "Quảng cáo Trực tuyến (Digital & Social Marketing)", FlagActive = true, Remark = "Facebook Ads, Google Search, TikTok Ads, YouTube TrueView" },
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "EVENT", MKTActivityTypeName = "Sự kiện & Trải nghiệm Lái thử (Roadshow & Test Drive)", FlagActive = true, Remark = "Sự kiện lái thử cuối tuần, Roadshow giới thiệu mẫu xe mới" },
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "POSM", MKTActivityTypeName = "Vật phẩm Quảng cáo Showroom (POSM & Decor)", FlagActive = true, Remark = "Standee, backdrop, brochure, tờ rơi, trang trí nhận diện CI/VI" },
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "PR_MEDIA", MKTActivityTypeName = "Báo chí & Truyền thông Chuyên ngành (PR & Media)", FlagActive = true, Remark = "Bài PR báo mạng Dân Trí, VnExpress, AutoPro, Otofun" },
                new MarketingActivityType { OrgId = org, MKTActivityTypeCode = "RADIO_VOV", MKTActivityTypeName = "Phát thanh Radio & Truyền hình (VOV Traffic)", FlagActive = true, Remark = "Spot quảng cáo kênh VOV Giao thông giờ cao điểm" }
            );
        }

        if (!await db.MarketingActivities.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            db.MarketingActivities.AddRange(
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "OOH_BILLBOARD",
                    MKTActivityName = "Thuê biển Pano Billboard quảng cáo ngoài trời tấm lớn",
                    MKTActivityTypeCode = "OOH",
                    DefaultHTCLimitPrice = 25000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Định mức hỗ trợ tối đa 25 triệu/vị trí/tháng"
                },
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "DIGI_FACEBOOK_ADS",
                    MKTActivityName = "Chạy chiến dịch quảng cáo Lead Generation Facebook Ads",
                    MKTActivityTypeCode = "DIGITAL",
                    DefaultHTCLimitPrice = 15000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Hỗ trợ 50% ngân sách chạy ads thu thập khách tiềm năng"
                },
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "DIGI_GOOGLE_SEARCH",
                    MKTActivityName = "Quảng cáo từ khóa tìm kiếm Google Search & GDN",
                    MKTActivityTypeCode = "DIGITAL",
                    DefaultHTCLimitPrice = 12000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Tối ưu tìm kiếm từ khóa thương hiệu xe Hyundai"
                },
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "EVENT_TEST_DRIVE",
                    MKTActivityName = "Tổ chức Sự kiện Lái thử & Trải nghiệm xe cuối tuần (Weekend Test Drive)",
                    MKTActivityTypeCode = "EVENT",
                    DefaultHTCLimitPrice = 30000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Hỗ trợ chi phí thuê địa điểm, tiệc trà teabreak và quà tặng khách lái thử"
                },
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "POSM_SHOWROOM_STANDEE",
                    MKTActivityName = "In ấn Standee, Banner, Backdrop và Brochure giới thiệu sản phẩm",
                    MKTActivityTypeCode = "POSM",
                    DefaultHTCLimitPrice = 8000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Sản xuất POSM theo đúng guideline chuẩn CI nhận diện Hyundai toàn cầu"
                },
                new MarketingActivity
                {
                    OrgId = org,
                    MKTActivityCode = "PR_PRESS_ARTICLE",
                    MKTActivityName = "Đăng bài PR trải nghiệm đánh giá xe trên báo điện tử & Diễn đàn ô tô",
                    MKTActivityTypeCode = "PR_MEDIA",
                    DefaultHTCLimitPrice = 10000000m,
                    FlagDesignImage = true,
                    FlagActualImage = true,
                    FlagContract = true,
                    FlagInvoice = true,
                    FlagActive = true,
                    Remark = "Bài viết đánh giá tính năng an toàn Hyundai SmartSense"
                }
            );
        }

        if (!await db.MarketingFeeSettlements.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            var fee1 = new MarketingFeeSettlement
            {
                OrgId = org,
                MKTFeeCode = "MKT-202603-HN01-0001",
                MKTFeeCodeUser = "ĐNQT-MKT/2026/03-HN01",
                MKTFeeName = "Chiến dịch Truyền thông & Lái thử Ra mắt Hyundai SantaFe & Creta Tháng 03/2026",
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                CampaignMonth = "2026-03",
                DateStart = DateTime.Now.AddDays(-20),
                DateEnd = DateTime.Now.AddDays(-5),
                TotalActivityCount = 3,
                TotalAmountDealer = 62000000m,
                TotalAmountApproved = 58000000m,
                VatRate = 10m,
                TotalVatAmount = 5800000m,
                TotalAmountAfterVAT = 63800000m,
                Status = "Finished",
                BankRefNo = "UNC-MKT-20260315-0012",
                SettledDate = DateTime.Now.AddDays(-3),
                SettledBy = "Kế toán Chi phí OEM - Lê Thu Hằng",
                ApprovedBy = "Giám đốc Marketing OEM - Trần Tuấn Anh",
                ApprovedAt = DateTime.Now.AddDays(-5),
                Remark = "Hồ sơ quyết toán marketing tháng 03/2026 đầy đủ 4 chứng từ hợp lệ, đã chuyển khoản giải ngân",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-20)
            };

            var fee2 = new MarketingFeeSettlement
            {
                OrgId = org,
                MKTFeeCode = "MKT-202603-HN02-0001",
                MKTFeeCodeUser = "ĐNQT-MKT/2026/03-HN02",
                MKTFeeName = "Chiến dịch Digital Marketing & Trưng bày Showroom Hyundai Tucson & Accent Tháng 03/2026",
                DealerCode = "DLR-HN02",
                DealerName = "Hyundai Hà Nội 02 - Long Biên",
                CampaignMonth = "2026-03",
                DateStart = DateTime.Now.AddDays(-15),
                DateEnd = DateTime.Now.AddDays(-2),
                TotalActivityCount = 2,
                TotalAmountDealer = 35000000m,
                TotalAmountApproved = 32000000m,
                VatRate = 10m,
                TotalVatAmount = 3200000m,
                TotalAmountAfterVAT = 35200000m,
                Status = "Approved",
                ApprovedBy = "Giám đốc Marketing OEM - Trần Tuấn Anh",
                ApprovedAt = DateTime.Now.AddDays(-1),
                Remark = "Đã phê duyệt kinh phí hỗ trợ, chờ kế toán thanh toán giải ngân UNC",
                CreatedBy = "dealer.hn02",
                CreatedAt = DateTime.Now.AddDays(-15)
            };

            var fee3 = new MarketingFeeSettlement
            {
                OrgId = org,
                MKTFeeCode = "MKT-202604-HCM01-0001",
                MKTFeeCodeUser = "ĐNQT-MKT/2026/04-HCM01",
                MKTFeeName = "Chương trình Sự kiện Lái thử Trải nghiệm Xe Điện Ioniq 5 & SantaFe All-New",
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                CampaignMonth = "2026-04",
                DateStart = DateTime.Now.AddDays(5),
                DateEnd = DateTime.Now.AddDays(25),
                TotalActivityCount = 2,
                TotalAmountDealer = 45000000m,
                TotalAmountApproved = 0m,
                VatRate = 10m,
                TotalVatAmount = 0m,
                TotalAmountAfterVAT = 0m,
                Status = "Submitted",
                Remark = "Đại lý đã nộp đề xuất kế hoạch marketing tháng 04/2026 kèm báo giá agency, đang chờ duyệt",
                CreatedBy = "dealer.hcm01",
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            db.MarketingFeeSettlements.AddRange(fee1, fee2, fee3);
            await db.SaveChangesAsync();

            // Details cho fee1
            var d1 = new MarketingFeeDetail
            {
                OrgId = org,
                MarketingFeeSettlementId = fee1.Id,
                MKTFeeCode = fee1.MKTFeeCode,
                LineIndex = 1,
                MKTActivityCode = "EVENT_TEST_DRIVE",
                MKTActivityName = "Tổ chức Sự kiện Lái thử & Trải nghiệm xe cuối tuần (Weekend Test Drive)",
                MKTActivityTypeCode = "EVENT",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                Qty = 1,
                Price = 28000000m,
                TotalAmountDealer = 28000000m,
                HTCLimitPrice = 30000000m,
                ApprovedQty = 1,
                ApprovedAmount = 28000000m,
                FlagDesignImage = true,
                FlagActualImage = true,
                FlagContract = true,
                FlagInvoice = true,
                HasDesignImage = true,
                HasActualImage = true,
                HasContract = true,
                HasInvoice = true,
                Status = "Finished",
                Remark = "Sự kiện thu hút 85 lượt khách lái thử và ký 12 hợp đồng"
            };

            var d2 = new MarketingFeeDetail
            {
                OrgId = org,
                MarketingFeeSettlementId = fee1.Id,
                MKTFeeCode = fee1.MKTFeeCode,
                LineIndex = 2,
                MKTActivityCode = "OOH_BILLBOARD",
                MKTActivityName = "Thuê biển Pano Billboard quảng cáo ngoài trời tấm lớn",
                MKTActivityTypeCode = "OOH",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                Qty = 1,
                Price = 20000000m,
                TotalAmountDealer = 20000000m,
                HTCLimitPrice = 25000000m,
                ApprovedQty = 1,
                ApprovedAmount = 20000000m,
                FlagDesignImage = true,
                FlagActualImage = true,
                FlagContract = true,
                FlagInvoice = true,
                HasDesignImage = true,
                HasActualImage = true,
                HasContract = true,
                HasInvoice = true,
                Status = "Finished",
                Remark = "Vị trí ngã tư Cầu Giấy - Xuân Thủy, lưu lượng 120.000 lượt xe/ngày"
            };

            var d3 = new MarketingFeeDetail
            {
                OrgId = org,
                MarketingFeeSettlementId = fee1.Id,
                MKTFeeCode = fee1.MKTFeeCode,
                LineIndex = 3,
                MKTActivityCode = "DIGI_FACEBOOK_ADS",
                MKTActivityName = "Chạy chiến dịch quảng cáo Lead Generation Facebook Ads",
                MKTActivityTypeCode = "DIGITAL",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                Qty = 1,
                Price = 14000000m,
                TotalAmountDealer = 14000000m,
                HTCLimitPrice = 10000000m,
                ApprovedQty = 1,
                ApprovedAmount = 10000000m,
                FlagDesignImage = true,
                FlagActualImage = true,
                FlagContract = true,
                FlagInvoice = true,
                HasDesignImage = true,
                HasActualImage = true,
                HasContract = true,
                HasInvoice = true,
                Status = "Finished",
                Remark = "OEM duyệt định mức trần 10.000.000đ cho chiến dịch Facebook Ads"
            };

            // Details cho fee2
            var d4 = new MarketingFeeDetail
            {
                OrgId = org,
                MarketingFeeSettlementId = fee2.Id,
                MKTFeeCode = fee2.MKTFeeCode,
                LineIndex = 1,
                MKTActivityCode = "DIGI_GOOGLE_SEARCH",
                MKTActivityName = "Quảng cáo từ khóa tìm kiếm Google Search & GDN",
                MKTActivityTypeCode = "DIGITAL",
                Vin = "DEMOVIN00000002",
                Model = "Creta 1.5 Cao cấp",
                Qty = 1,
                Price = 12000000m,
                TotalAmountDealer = 12000000m,
                HTCLimitPrice = 12000000m,
                ApprovedQty = 1,
                ApprovedAmount = 12000000m,
                FlagDesignImage = true,
                FlagActualImage = true,
                FlagContract = true,
                FlagInvoice = true,
                HasDesignImage = true,
                HasActualImage = true,
                HasContract = true,
                HasInvoice = true,
                Status = "Approved",
                Remark = "Đã nghiệm thu báo cáo KPI từ Google Ads"
            };

            var d5 = new MarketingFeeDetail
            {
                OrgId = org,
                MarketingFeeSettlementId = fee2.Id,
                MKTFeeCode = fee2.MKTFeeCode,
                LineIndex = 2,
                MKTActivityCode = "POSM_SHOWROOM_STANDEE",
                MKTActivityName = "In ấn Standee, Banner, Backdrop và Brochure giới thiệu sản phẩm",
                MKTActivityTypeCode = "POSM",
                Vin = "DEMOVIN00000001",
                Model = "Accent 1.4 AT",
                Qty = 1,
                Price = 23000000m,
                TotalAmountDealer = 23000000m,
                HTCLimitPrice = 20000000m,
                ApprovedQty = 1,
                ApprovedAmount = 20000000m,
                FlagDesignImage = true,
                FlagActualImage = true,
                FlagContract = true,
                FlagInvoice = true,
                HasDesignImage = true,
                HasActualImage = true,
                HasContract = true,
                HasInvoice = true,
                Status = "Approved",
                Remark = "Trang trí khu vực trưng bày showroom chuẩn nhận diện mới GDSI"
            };

            db.MarketingFeeDetails.AddRange(d1, d2, d3, d4, d5);
            await db.SaveChangesAsync();

            // Attachments cho fee1
            db.MarketingFeeDetailAttaches.AddRange(
                new MarketingFeeDetailAttach
                {
                    OrgId = org,
                    MarketingFeeDetailId = d1.Id,
                    MKTFeeCode = fee1.MKTFeeCode,
                    LineIndex = 1,
                    AttachCode = "ATT-MKT-20260301-01-001",
                    FileType = "DesignImage",
                    FileName = "Maket_Backdrop_LaiThu_HN01.pdf",
                    FilePath = "/uploads/mkt/MKT-202603-HN01-0001/Maket_Backdrop_LaiThu_HN01.pdf",
                    FileSizeKb = 4200,
                    Status = "Approved",
                    ApprovedBy = "Chuyên viên Marketing - Phạm Minh Đức",
                    ApprovedAt = DateTime.Now.AddDays(-18),
                    Remark = "Maket đúng chuẩn nhận diện Hyundai CI",
                    UploadedAt = DateTime.Now.AddDays(-19)
                },
                new MarketingFeeDetailAttach
                {
                    OrgId = org,
                    MarketingFeeDetailId = d1.Id,
                    MKTFeeCode = fee1.MKTFeeCode,
                    LineIndex = 1,
                    AttachCode = "ATT-MKT-20260301-01-002",
                    FileType = "ActualImage",
                    FileName = "AnhNghiemThu_SuKienLaiThu_HN01.jpg",
                    FilePath = "/uploads/mkt/MKT-202603-HN01-0001/AnhNghiemThu_SuKienLaiThu_HN01.jpg",
                    FileSizeKb = 8500,
                    Status = "Approved",
                    ApprovedBy = "Chuyên viên Marketing - Phạm Minh Đức",
                    ApprovedAt = DateTime.Now.AddDays(-6),
                    Remark = "Ảnh hiện trường sự kiện có đông đủ khách hàng tham gia",
                    UploadedAt = DateTime.Now.AddDays(-7)
                },
                new MarketingFeeDetailAttach
                {
                    OrgId = org,
                    MarketingFeeDetailId = d1.Id,
                    MKTFeeCode = fee1.MKTFeeCode,
                    LineIndex = 1,
                    AttachCode = "ATT-MKT-20260301-01-003",
                    FileType = "Contract",
                    FileName = "HD_ToChucSuKien_Agency_MediaStar.pdf",
                    FilePath = "/uploads/mkt/MKT-202603-HN01-0001/HD_ToChucSuKien_Agency_MediaStar.pdf",
                    FileSizeKb = 2100,
                    Status = "Approved",
                    ApprovedBy = "Chuyên viên Marketing - Phạm Minh Đức",
                    ApprovedAt = DateTime.Now.AddDays(-6),
                    Remark = "Hợp đồng dịch vụ đầy đủ pháp lý",
                    UploadedAt = DateTime.Now.AddDays(-7)
                },
                new MarketingFeeDetailAttach
                {
                    OrgId = org,
                    MarketingFeeDetailId = d1.Id,
                    MKTFeeCode = fee1.MKTFeeCode,
                    LineIndex = 1,
                    AttachCode = "ATT-MKT-20260301-01-004",
                    FileType = "Invoice",
                    FileName = "HoaDonGTGT_MediaStar_HD001928.pdf",
                    FilePath = "/uploads/mkt/MKT-202603-HN01-0001/HoaDonGTGT_MediaStar_HD001928.pdf",
                    FileSizeKb = 1500,
                    Status = "Approved",
                    ApprovedBy = "Chuyên viên Marketing - Phạm Minh Đức",
                    ApprovedAt = DateTime.Now.AddDays(-6),
                    Remark = "Hóa đơn điện tử hợp lệ",
                    UploadedAt = DateTime.Now.AddDays(-7)
                }
            );

            // Cập nhật thông tin hỗ trợ marketing trên xe VIN
            var v1Mkt = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Mkt != null)
            {
                v1Mkt.IsMktFeeSupported = true;
                v1Mkt.MktFeeSupportedAmount = 38000000m; // 28tr (Line 1) + 10tr (Line 3)
                v1Mkt.LastMktFeeNo = fee1.MKTFeeCode;
                v1Mkt.LastMktFeeDate = DateTime.Now.AddDays(-5);
                v1Mkt.MktFeeCount = 2;
            }

            var v2Mkt = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Mkt != null)
            {
                v2Mkt.IsMktFeeSupported = true;
                v2Mkt.MktFeeSupportedAmount = 20000000m; // 20tr (Line 2)
                v2Mkt.LastMktFeeNo = fee1.MKTFeeCode;
                v2Mkt.LastMktFeeDate = DateTime.Now.AddDays(-5);
                v2Mkt.MktFeeCount = 1;
            }
        }

        // ===== Quản lý Chỉ tiêu Bán hàng & KPI Doanh số Xe Ô tô (BizHTC.MasterData & DMS.NP.Biz / SP_KPIMonth, Mst_SMKPI / SalesTargetKpi) =====
        if (!await db.SalesKpiIndicators.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;
            db.SalesKpiIndicators.AddRange(
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_CALL_LEADS",
                    KPIName = "Số cuộc gọi tiếp cận khách hàng tiềm năng (Outbound Leads Call)",
                    KPICategory = "Activity",
                    Unit = "Cuộc",
                    Weight = 10.0m,
                    TargetDefault = 100,
                    FlagActive = true,
                    Remark = "Gọi điện chào giá, tư vấn khuyến mãi xe mới theo danh sách Leads",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_SHOWROOM_VISIT",
                    KPIName = "Số lượt tiếp đón khách hàng tham quan Showroom (Showroom Traffic)",
                    KPICategory = "Activity",
                    Unit = "Lượt",
                    Weight = 15.0m,
                    TargetDefault = 25,
                    FlagActive = true,
                    Remark = "Tiếp đón, ghi nhận thông tin và tư vấn trực tiếp tại Showroom",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_TEST_DRIVE",
                    KPIName = "Số lượt hướng dẫn khách hàng lái thử xe thực tế (Test Drive)",
                    KPICategory = "Activity",
                    Unit = "Lượt",
                    Weight = 15.0m,
                    TargetDefault = 15,
                    FlagActive = true,
                    Remark = "Tổ chức trải nghiệm lái thử xe Showroom / Roadshow / Home Drive",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_QUOTATION",
                    KPIName = "Số bảng báo giá & dự toán chi phí lăn bánh phát hành (Quotation Sent)",
                    KPICategory = "Activity",
                    Unit = "Báo giá",
                    Weight = 10.0m,
                    TargetDefault = 20,
                    FlagActive = true,
                    Remark = "Lập bảng giá chi tiết lăn bánh kèm phương án vay ngân hàng cho khách",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_CONTRACT_SIGN",
                    KPIName = "Số hợp đồng mua bán xe ký mới & nhận cọc (Contract Signed)",
                    KPICategory = "SalesVolume",
                    Unit = "Hợp đồng",
                    Weight = 15.0m,
                    TargetDefault = 8,
                    FlagActive = true,
                    Remark = "Ký kết hợp đồng bán lẻ và thu tiền đặt cọc theo quy định",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_RETAIL_DELIVERY",
                    KPIName = "Số lượng xe thực tế bàn giao cho khách hàng (Retail Delivery)",
                    KPICategory = "SalesVolume",
                    Unit = "Xe",
                    Weight = 20.0m,
                    TargetDefault = 6,
                    FlagActive = true,
                    Remark = "Hoàn tất thủ tục bàn giao xe mới, kích hoạt sổ bảo hành online SBH",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_INSURANCE_SOLD",
                    KPIName = "Số hợp đồng bảo hiểm vật chất thân vỏ bán kèm xe (Insurance Sold)",
                    KPICategory = "Revenue",
                    Unit = "Hợp đồng",
                    Weight = 5.0m,
                    TargetDefault = 5,
                    FlagActive = true,
                    Remark = "Bán bảo hiểm vật chất chính hãng liên kết Bảo Việt / PJICO / PTI",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_ACCESSORIES_VAL",
                    KPIName = "Tổng doanh số bán phụ kiện & gói làm đẹp xe (Accessories Revenue)",
                    KPICategory = "Revenue",
                    Unit = "VNĐ",
                    Weight = 5.0m,
                    TargetDefault = 35000000,
                    FlagActive = true,
                    Remark = "Doanh thu phụ kiện chính hãng: Phim cách nhiệt, Phủ ceramic, Camera hành trình, Thảm lót sàn",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_FINANCE_LOAN",
                    KPIName = "Số lượng hồ sơ vay vốn ngân hàng giải ngân thành công (Bank Loan)",
                    KPICategory = "Revenue",
                    Unit = "Hồ sơ",
                    Weight = 5.0m,
                    TargetDefault = 4,
                    FlagActive = true,
                    Remark = "Hỗ trợ thủ tục vay trả góp qua VCB, Techcombank, VPBank, Shinhan",
                    CreatedBy = "admin.htv"
                },
                new SalesKpiIndicator
                {
                    OrgId = org,
                    KPICode = "KPI_CSI_SCORE",
                    KPIName = "Điểm đánh giá chỉ số hài lòng khách hàng SSI/CSI (Customer Satisfaction)",
                    KPICategory = "Quality",
                    Unit = "Điểm",
                    Weight = 10.0m,
                    TargetDefault = 4.8m,
                    FlagActive = true,
                    Remark = "Khảo sát sau giao xe ICIC đạt tối thiểu 4.8 / 5.0 sao",
                    CreatedBy = "admin.htv"
                }
            );
        }

        if (!await db.SalesTargetKpis.AnyAsync())
        {
            var org = TenantContext.DefaultOrgId;

            // Kế hoạch 1: Toàn đại lý Hà Nội 01 (DLR-HN01) Tháng 03/2026
            var kpi1 = new SalesTargetKpi
            {
                OrgId = org,
                TargetCode = "KPI-202603-HN01-0001",
                TargetCodeUser = "CT-KPI/2026/03-HN01-ALL",
                PeriodMonth = "2026-03",
                PeriodQuarter = "Q1",
                PeriodYear = 2026,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                UserCode = "ALL",
                UserName = "Toàn Đại Lý Hà Nội 01",
                Position = "DealerOverall",
                TargetCarCount = 20,
                ActualCarCount = 18,
                CarCompletionRate = 90.0m,
                TargetRevenue = 13500000000m,
                ActualRevenue = 12200000000m,
                RevenueCompletionRate = 90.37m,
                TargetTestDriveCount = 45,
                ActualTestDriveCount = 42,
                TargetContractCount = 22,
                ActualContractCount = 20,
                TargetInsuranceCount = 16,
                ActualInsuranceCount = 15,
                TargetAccessoriesRevenue = 150000000m,
                ActualAccessoriesRevenue = 135000000m,
                OverallScore = 91.5m,
                KpiGrade = "Good",
                BonusRate = 10.0m,
                BonusAmount = 54000000m,
                Status = "Approved",
                Remark = "Kế hoạch chỉ tiêu doanh số bán hàng đại lý Hà Nội 01 tháng 03/2026, tập trung thúc đẩy SantaFe & Accent",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-25),
                ApprovedBy = "Giám Đốc Bán Hàng OEM - Phạm Hoàng Long",
                ApprovedAt = DateTime.Now.AddDays(-23)
            };

            // Kế hoạch 2: TVBH Trần Thị Tư Vấn (DLR-HN01) Tháng 03/2026 - Đã nghiệm thu Đạt Xuất sắc
            var kpi2 = new SalesTargetKpi
            {
                OrgId = org,
                TargetCode = "KPI-202603-HN01-0002",
                TargetCodeUser = "CT-KPI/2026/03-HN01-TVBH01",
                PeriodMonth = "2026-03",
                PeriodQuarter = "Q1",
                PeriodYear = 2026,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                UserCode = "TVBH-HN01-01",
                UserName = "Trần Thị Tư Vấn",
                Position = "SalesConsultant",
                TargetCarCount = 6,
                ActualCarCount = 7,
                CarCompletionRate = 116.67m,
                TargetRevenue = 4200000000m,
                ActualRevenue = 4850000000m,
                RevenueCompletionRate = 115.48m,
                TargetTestDriveCount = 15,
                ActualTestDriveCount = 18,
                TargetContractCount = 7,
                ActualContractCount = 8,
                TargetInsuranceCount = 5,
                ActualInsuranceCount = 6,
                TargetAccessoriesRevenue = 40000000m,
                ActualAccessoriesRevenue = 45000000m,
                OverallScore = 115.0m,
                KpiGrade = "Excellent",
                BonusRate = 20.0m,
                BonusAmount = 25200000m,
                Status = "Evaluated",
                Remark = "Hoàn thành vượt 116% chỉ tiêu bán xe và 115% doanh thu, xếp loại Xuất sắc",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-25),
                ApprovedBy = "Trưởng phòng KD - Nguyễn Hữu Thắng",
                ApprovedAt = DateTime.Now.AddDays(-23),
                EvaluatedBy = "Giám Đốc Đại Lý - Trần Văn Bình",
                EvaluatedAt = DateTime.Now.AddDays(-2)
            };

            // Kế hoạch 3: TVBH Lê Văn Bán Hàng (DLR-HN01) Tháng 03/2026 - Đang thực hiện
            var kpi3 = new SalesTargetKpi
            {
                OrgId = org,
                TargetCode = "KPI-202603-HN01-0003",
                TargetCodeUser = "CT-KPI/2026/03-HN01-TVBH02",
                PeriodMonth = "2026-03",
                PeriodQuarter = "Q1",
                PeriodYear = 2026,
                DealerCode = "DLR-HN01",
                DealerName = "Hyundai Hà Nội 01 - Cầu Giấy",
                UserCode = "TVBH-HN01-02",
                UserName = "Lê Văn Bán Hàng",
                Position = "SalesConsultant",
                TargetCarCount = 5,
                ActualCarCount = 4,
                CarCompletionRate = 80.0m,
                TargetRevenue = 3500000000m,
                ActualRevenue = 2800000000m,
                RevenueCompletionRate = 80.0m,
                TargetTestDriveCount = 12,
                ActualTestDriveCount = 10,
                TargetContractCount = 5,
                ActualContractCount = 4,
                TargetInsuranceCount = 4,
                ActualInsuranceCount = 3,
                TargetAccessoriesRevenue = 30000000m,
                ActualAccessoriesRevenue = 22000000m,
                OverallScore = 80.5m,
                KpiGrade = "Pass",
                BonusRate = 0m,
                BonusAmount = 12000000m,
                Status = "Approved",
                Remark = "Đang tích cực bám sát khách hàng tiềm năng để chốt hợp đồng trước cuối tháng",
                CreatedBy = "dealer.hn01",
                CreatedAt = DateTime.Now.AddDays(-25),
                ApprovedBy = "Trưởng phòng KD - Nguyễn Hữu Thắng",
                ApprovedAt = DateTime.Now.AddDays(-23)
            };

            // Kế hoạch 4: TVBH Lê Hoàng Nam (DLR-HCM01) Tháng 04/2026 - Đang trình duyệt
            var kpi4 = new SalesTargetKpi
            {
                OrgId = org,
                TargetCode = "KPI-202604-HCM01-0001",
                TargetCodeUser = "CT-KPI/2026/04-HCM01-TVBH01",
                PeriodMonth = "2026-04",
                PeriodQuarter = "Q2",
                PeriodYear = 2026,
                DealerCode = "DLR-HCM01",
                DealerName = "Hyundai Sài Gòn",
                UserCode = "TVBH-HCM01-01",
                UserName = "Lê Hoàng Nam",
                Position = "SalesConsultant",
                TargetCarCount = 8,
                ActualCarCount = 0,
                CarCompletionRate = 0m,
                TargetRevenue = 6000000000m,
                ActualRevenue = 0m,
                RevenueCompletionRate = 0m,
                TargetTestDriveCount = 20,
                ActualTestDriveCount = 0,
                TargetContractCount = 9,
                ActualContractCount = 0,
                TargetInsuranceCount = 6,
                ActualInsuranceCount = 0,
                TargetAccessoriesRevenue = 50000000m,
                ActualAccessoriesRevenue = 0m,
                OverallScore = 0m,
                KpiGrade = "Pending",
                BonusRate = 15.0m,
                BonusAmount = 0m,
                Status = "Submitted",
                Remark = "Kế hoạch chỉ tiêu tháng 04/2026 chào đón xe điện IONIQ 5 và SUV SantaFe mới",
                CreatedBy = "dealer.hcm01",
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            db.SalesTargetKpis.AddRange(kpi1, kpi2, kpi3, kpi4);
            await db.SaveChangesAsync();

            // Lines cho kpi1 (Toàn đại lý)
            db.SalesTargetKpiLines.AddRange(
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi1.Id,
                    TargetCode = kpi1.TargetCode,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    TargetQty = 8,
                    ActualQty = 7,
                    CompletionRate = 87.5m,
                    TargetRevenue = 4400000000m,
                    ActualRevenue = 3850000000m,
                    CommissionPerCar = 3000000m,
                    BonusAmount = 21000000m,
                    Status = "Approved",
                    Remark = "Dòng xe chủ lực phân khúc sedan B"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi1.Id,
                    TargetCode = kpi1.TargetCode,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    TargetQty = 6,
                    ActualQty = 6,
                    CompletionRate = 100.0m,
                    TargetRevenue = 4200000000m,
                    ActualRevenue = 4200000000m,
                    CommissionPerCar = 3500000m,
                    BonusAmount = 21000000m,
                    Status = "Approved",
                    Remark = "SUV đô thị bán chạy"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi1.Id,
                    TargetCode = kpi1.TargetCode,
                    LineIndex = 3,
                    Model = "SantaFe 2.5T AWD",
                    SpecCode = "2.5T AWD Calligraphy",
                    TargetQty = 4,
                    ActualQty = 3,
                    CompletionRate = 75.0m,
                    TargetRevenue = 5400000000m,
                    ActualRevenue = 4050000000m,
                    CommissionPerCar = 5000000m,
                    BonusAmount = 15000000m,
                    Status = "Approved",
                    Remark = "SUV cao cấp 7 chỗ"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi1.Id,
                    TargetCode = kpi1.TargetCode,
                    LineIndex = 4,
                    Model = "Tucson 1.6T HTRAC",
                    SpecCode = "1.6 Turbo AWD",
                    TargetQty = 2,
                    ActualQty = 2,
                    CompletionRate = 100.0m,
                    TargetRevenue = 1900000000m,
                    ActualRevenue = 1900000000m,
                    CommissionPerCar = 4000000m,
                    BonusAmount = 8000000m,
                    Status = "Approved",
                    Remark = "Crossover phân khúc C"
                }
            );

            // Lines cho kpi2 (TVBH Trần Thị Tư Vấn)
            db.SalesTargetKpiLines.AddRange(
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    TargetQty = 3,
                    ActualQty = 4,
                    CompletionRate = 133.33m,
                    TargetRevenue = 1650000000m,
                    ActualRevenue = 2200000000m,
                    CommissionPerCar = 3000000m,
                    BonusAmount = 14400000m,
                    Status = "Evaluated",
                    Remark = "Bán vượt chỉ tiêu 4 xe Accent (kèm DEMOVIN00000001)"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    TargetQty = 2,
                    ActualQty = 2,
                    CompletionRate = 100.0m,
                    TargetRevenue = 1400000000m,
                    ActualRevenue = 1400000000m,
                    CommissionPerCar = 3500000m,
                    BonusAmount = 8400000m,
                    Status = "Evaluated",
                    Remark = "Bán đúng chỉ tiêu 2 xe Creta (kèm DEMOVIN00000002)"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 3,
                    Model = "SantaFe 2.5T AWD",
                    SpecCode = "2.5T AWD Calligraphy",
                    TargetQty = 1,
                    ActualQty = 1,
                    CompletionRate = 100.0m,
                    TargetRevenue = 1350000000m,
                    ActualRevenue = 1350000000m,
                    CommissionPerCar = 5000000m,
                    BonusAmount = 6000000m,
                    Status = "Evaluated",
                    Remark = "Chốt thành công 1 hợp đồng SantaFe Calligraphy bản Full"
                }
            );

            // Lines cho kpi3 (TVBH Lê Văn Bán Hàng)
            db.SalesTargetKpiLines.AddRange(
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi3.Id,
                    TargetCode = kpi3.TargetCode,
                    LineIndex = 1,
                    Model = "Accent 1.4 AT",
                    SpecCode = "1.4 AT Đặc biệt",
                    TargetQty = 3,
                    ActualQty = 2,
                    CompletionRate = 66.67m,
                    TargetRevenue = 1650000000m,
                    ActualRevenue = 1100000000m,
                    CommissionPerCar = 3000000m,
                    BonusAmount = 6000000m,
                    Status = "Approved",
                    Remark = "Đã giao 2 xe, đang chăm sóc 2 khách hẹn chốt"
                },
                new SalesTargetKpiLine
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi3.Id,
                    TargetCode = kpi3.TargetCode,
                    LineIndex = 2,
                    Model = "Creta 1.5 Cao cấp",
                    SpecCode = "1.5 Cao cấp 2 tông màu",
                    TargetQty = 2,
                    ActualQty = 2,
                    CompletionRate = 100.0m,
                    TargetRevenue = 1400000000m,
                    ActualRevenue = 1400000000m,
                    CommissionPerCar = 3500000m,
                    BonusAmount = 7000000m,
                    Status = "Approved",
                    Remark = "Đã giao 2 xe Creta cho khách hàng gia đình"
                }
            );

            // Daily Logs cho kpi2
            db.SalesKpiDailyLogs.AddRange(
                new SalesKpiDailyLog
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 1,
                    LogDate = DateTime.Now.AddDays(-18),
                    KPICode = "KPI_TEST_DRIVE",
                    KPIName = "Số lượt hướng dẫn khách hàng lái thử xe thực tế",
                    TargetDailyQty = 1,
                    ActualDailyQty = 2,
                    LinkedVin = "DEMOVIN00000001",
                    LinkedRefNo = "DT202603-0001",
                    Notes = "Hướng dẫn khách hàng Hoàng Minh Đức lái thử xe Accent 1.4 AT tại showroom Phạm Văn Đồng",
                    CreatedBy = "dealer.hn01",
                    CreatedAt = DateTime.Now.AddDays(-18)
                },
                new SalesKpiDailyLog
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 2,
                    LogDate = DateTime.Now.AddDays(-15),
                    KPICode = "KPI_CONTRACT_SIGN",
                    KPIName = "Số hợp đồng mua bán xe ký mới & nhận cọc",
                    TargetDailyQty = 1,
                    ActualDailyQty = 1,
                    LinkedVin = "DEMOVIN00000001",
                    LinkedRefNo = "DEAL202603-001",
                    Notes = "Ký hợp đồng bán lẻ DEAL202603-001 xe Accent 1.4 AT cho anh Nguyễn Văn An, nhận cọc 50 triệu",
                    CreatedBy = "dealer.hn01",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new SalesKpiDailyLog
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 3,
                    LogDate = DateTime.Now.AddDays(-10),
                    KPICode = "KPI_RETAIL_DELIVERY",
                    KPIName = "Số lượng xe thực tế bàn giao cho khách hàng",
                    TargetDailyQty = 1,
                    ActualDailyQty = 1,
                    LinkedVin = "DEMOVIN00000001",
                    LinkedRefNo = "DEAL202603-001",
                    Notes = "Bàn giao xe Accent DEMOVIN00000001 cho khách hàng, kích hoạt Sổ bảo hành Online SBH",
                    CreatedBy = "dealer.hn01",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new SalesKpiDailyLog
                {
                    OrgId = org,
                    SalesTargetKpiId = kpi2.Id,
                    TargetCode = kpi2.TargetCode,
                    LineIndex = 4,
                    LogDate = DateTime.Now.AddDays(-5),
                    KPICode = "KPI_RETAIL_DELIVERY",
                    KPIName = "Số lượng xe thực tế bàn giao cho khách hàng",
                    TargetDailyQty = 1,
                    ActualDailyQty = 1,
                    LinkedVin = "DEMOVIN00000002",
                    LinkedRefNo = "DEAL202603-002",
                    Notes = "Bàn giao xe Creta DEMOVIN00000002 cho khách hàng chị Trần Thị Mai",
                    CreatedBy = "dealer.hn01",
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            );

            // Cập nhật thông tin KPI trên hồ sơ xe VIN
            var v1Kpi = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000001");
            if (v1Kpi != null)
            {
                v1Kpi.LastSalesKpiNo = kpi2.TargetCode;
                v1Kpi.LastSalesKpiDate = DateTime.Now.AddDays(-10);
                v1Kpi.SalesKpiCount = 2; // Ghi nhận cả kpi1 (Đại lý) và kpi2 (TVBH)
            }

            var v2Kpi = await db.Vehicles.FirstOrDefaultAsync(v => v.OrgId == org && v.Vin == "DEMOVIN00000002");
            if (v2Kpi != null)
            {
                v2Kpi.LastSalesKpiNo = kpi2.TargetCode;
                v2Kpi.LastSalesKpiDate = DateTime.Now.AddDays(-5);
                v2Kpi.SalesKpiCount = 2;
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Tạo bảng thêm-sau cho Postgres (EnsureCreated chỉ tạo khi DB chưa tồn tại). SQLite ephemeral đã có sẵn qua EnsureCreated.</summary>
    private static async Task MigrateAsync(AppDbContext db)
    {
        if (!db.Database.IsNpgsql()) return;
        var stmts = new[]
        {
            "CREATE TABLE IF NOT EXISTS public.\"Recalls\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"Code\" text NOT NULL DEFAULT '', \"Title\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"Reason\" text NULL, \"Remedy\" text NULL, \"Status\" text NOT NULL DEFAULT 'Open', \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"VehicleRecalls\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CampaignId\" bigint NOT NULL, \"Vin\" text NOT NULL DEFAULT '', \"Status\" text NOT NULL DEFAULT 'Open', \"DoneAt\" timestamp NULL, \"DoneBy\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"Claims\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ClaimNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"Issue\" text NOT NULL DEFAULT '', \"PartsCost\" numeric NOT NULL DEFAULT 0, \"LaborCost\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Submitted', \"DecisionNote\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"DecidedAt\" timestamp NULL, \"SettledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DocRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"Code\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DocType\" text NOT NULL DEFAULT 'Registration', \"Status\" text NOT NULL DEFAULT 'Requested', \"Note\" text NULL, \"TrackingNo\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"ShippedAt\" timestamp NULL, \"ReceivedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"Transfers\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"Code\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"FromDealer\" text NULL, \"ToDealer\" text NOT NULL DEFAULT '', \"Status\" text NOT NULL DEFAULT 'Requested', \"Note\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"ReceivedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DeliveryMinutes\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DlvMnNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DoNo\" text NULL, \"TransporterCode\" text NULL, \"DriverName\" text NULL, \"DriverPhone\" text NULL, \"TruckPlateNo\" text NULL, \"FromStorage\" text NULL, \"ToStorage\" text NULL, \"OdoKm\" integer NOT NULL DEFAULT 0, \"ExteriorCondition\" text NULL, \"InteriorCondition\" text NULL, \"HasSpareWheel\" boolean NOT NULL DEFAULT true, \"HasToolKit\" boolean NOT NULL DEFAULT true, \"KeyCount\" integer NOT NULL DEFAULT 2, \"HasGuarantyBooklet\" boolean NOT NULL DEFAULT true, \"HasUserManual\" boolean NOT NULL DEFAULT true, \"HasOriginalCertificate\" boolean NOT NULL DEFAULT true, \"DeliveredBy\" text NULL, \"ReceivedBy\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"HandoverDate\" timestamp NULL, \"ConfirmedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarRetrieves\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RetrieveNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"ToStorage\" text NULL, \"Reason\" text NULL, \"Status\" text NOT NULL DEFAULT 'Requested', \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"ReceivedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarRetrieveLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CarRetrieveId\" bigint NOT NULL, \"RetrieveNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"StorageCode\" text NULL, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportReqNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"TransporterCode\" text NULL, \"TransportContractNo\" text NULL, \"TruckPlateNo\" text NULL, \"DriverName\" text NULL, \"DriverPhone\" text NULL, \"FromStorage\" text NULL, \"ToStorage\" text NULL, \"EstimatedDeparture\" timestamp NULL, \"EstimatedArrival\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"DispatchedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportRequestId\" bigint NOT NULL, \"TransportReqNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"DeliveryOrderNo\" text NULL, \"StorageCode\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StorageRearranges\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"StorageRearrangeNo\" text NOT NULL DEFAULT '', \"Reason\" text NULL, \"Remark\" text NULL, \"Status\" text NOT NULL DEFAULT 'Requested', \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StorageRearrangeLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"StorageRearrangeId\" bigint NOT NULL, \"StorageRearrangeNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"StorageCodeFrom\" text NULL, \"StorageCodeTo\" text NOT NULL DEFAULT '', \"RearrangeStartDate\" timestamp NULL, \"RearrangeEndDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TestCars\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TestCarCode\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"EventName\" text NULL, \"Purpose\" text NULL, \"StartDate\" timestamp NULL, \"EndDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"FinishedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TestCarLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TestCarRequestId\" bigint NOT NULL, \"TestCarCode\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"OdoStart\" integer NOT NULL DEFAULT 0, \"OdoEnd\" integer NULL, \"ConditionStart\" text NULL, \"ConditionEnd\" text NULL, \"HandoverDate\" timestamp NULL, \"ReturnDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PdiRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PdiReqNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"InspectorName\" text NULL, \"ApprovedBy\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PdiRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PdiRequestId\" bigint NOT NULL, \"PdiReqNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"DlrContractNo\" text NULL, \"RoNo\" text NULL, \"RoStatus\" text NOT NULL DEFAULT 'NORE', \"BatteryVoltage\" double precision NULL DEFAULT 12.6, \"TirePressureOk\" boolean NOT NULL DEFAULT true, \"FluidsOk\" boolean NOT NULL DEFAULT true, \"ElectronicsOk\" boolean NOT NULL DEFAULT true, \"ExteriorOk\" boolean NOT NULL DEFAULT true, \"InteriorCleanOk\" boolean NOT NULL DEFAULT true, \"DiagnosticScanOk\" boolean NOT NULL DEFAULT true, \"PdiResult\" text NOT NULL DEFAULT 'Pending', \"Status\" text NOT NULL DEFAULT 'Pending', \"InspectedAt\" timestamp NULL, \"InspectedBy\" text NULL, \"DefectNotes\" text NULL, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsMortgaged\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"MortgageBankCode\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"MortgageDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"RedeemDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"MortgageRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ReqMortgageNo\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT '', \"MortgageDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"MortgageRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MortgageRequestId\" bigint NOT NULL, \"ReqMortgageNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"MortgageAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"RedeemRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RedeemReqNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT '', \"ReqMortgageNo\" text NULL, \"Reason\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"RedeemRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RedeemRequestId\" bigint NOT NULL, \"RedeemReqNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"ReleaseDocType\" text NOT NULL DEFAULT 'All', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"SOCode\" text NULL",
            "CREATE TABLE IF NOT EXISTS public.\"SalesOrders\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SOCode\" text NOT NULL DEFAULT '', \"SOType\" text NOT NULL DEFAULT 'Normal', \"DealerCode\" text NOT NULL DEFAULT '', \"SPCode\" text NULL, \"OrderMonth\" text NULL, \"ProductionMonth\" text NULL, \"ExpectedMonth\" text NULL, \"TotalOrderQty\" integer NOT NULL DEFAULT 0, \"TotalApprovedQty\" integer NOT NULL DEFAULT 0, \"TotalAllocatedQty\" integer NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"CreatedBy\" text NULL, \"ApprovedBy1\" text NULL, \"ApprovedAt1\" timestamp NULL, \"ApprovedBy2\" text NULL, \"ApprovedAt2\" timestamp NULL, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"SalesOrderLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SalesOrderId\" bigint NOT NULL, \"SOCode\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"Color\" text NULL, \"OrderQty\" integer NOT NULL DEFAULT 1, \"ApprovedQty\" integer NOT NULL DEFAULT 0, \"AllocatedQty\" integer NOT NULL DEFAULT 0, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DealerDeals\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DealNo\" text NOT NULL DEFAULT '', \"DealNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"CustomerCode\" text NULL, \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"CustomerType\" text NOT NULL DEFAULT 'Individual', \"IdNo\" text NULL, \"Address\" text NULL, \"SalesManCode\" text NULL, \"SalesManName\" text NULL, \"SalesType\" text NOT NULL DEFAULT 'Retail', \"PaymentType\" text NOT NULL DEFAULT 'Cash', \"BankCode\" text NULL, \"LoanAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"DiscountAmount\" numeric NOT NULL DEFAULT 0, \"FinalAmount\" numeric NOT NULL DEFAULT 0, \"DepositAmount\" numeric NOT NULL DEFAULT 0, \"DealDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"CreatedBy\" text NULL, \"ApprovedBy\" text NULL, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"DeliveredAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DealerDealLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DealerDealId\" bigint NOT NULL, \"DealNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"Color\" text NULL, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Discount\" numeric NOT NULL DEFAULT 0, \"Price\" numeric NOT NULL DEFAULT 0, \"PlateNo\" text NULL, \"SBHOnlineNo\" text NULL, \"DeliveryOdoKm\" integer NOT NULL DEFAULT 10, \"WarrantyStartDate\" timestamp NULL, \"WarrantyMonths\" integer NOT NULL DEFAULT 36, \"DeliveryDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"Guarantees\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GuaranteeNo\" text NOT NULL DEFAULT '', \"BankGuaranteeNo\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT '', \"BankName\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DateOpen\" timestamp NOT NULL DEFAULT now(), \"DateExpired\" timestamp NOT NULL DEFAULT now(), \"Term\" integer NOT NULL DEFAULT 30, \"TermActual\" integer NOT NULL DEFAULT 30, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"ApprovedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"SettledAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GuaranteeLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentGuaranteeId\" bigint NOT NULL, \"GuaranteeNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"GuaranteePercent\" numeric NOT NULL DEFAULT 100, \"GuaranteeValue\" numeric NOT NULL DEFAULT 0, \"DateStart\" timestamp NULL, \"DateWarning\" timestamp NULL, \"DateExpired\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DealerContracts\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ContractNo\" text NOT NULL DEFAULT '', \"ContractNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"SOCode\" text NULL, \"ContractType\" text NOT NULL DEFAULT 'Wholesale', \"ContractDate\" timestamp NOT NULL DEFAULT now(), \"DeliveryDeadline\" timestamp NULL, \"PaymentTermDays\" integer NOT NULL DEFAULT 30, \"TotalQuantity\" integer NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"DiscountAmount\" numeric NOT NULL DEFAULT 0, \"FinalAmount\" numeric NOT NULL DEFAULT 0, \"DepositAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"CreatedBy\" text NULL, \"ApprovedBy\" text NULL, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DealerContractLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DealerContractId\" bigint NOT NULL, \"ContractNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"Color\" text NULL, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Discount\" numeric NOT NULL DEFAULT 0, \"ActualPrice\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PaymentDiscounts\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentDiscountNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DateEndFrom\" timestamp NULL, \"DateEndTo\" timestamp NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalPaymentAmount\" numeric NOT NULL DEFAULT 0, \"TotalDiscountAmount\" numeric NOT NULL DEFAULT 0, \"DiscountPercent\" numeric NOT NULL DEFAULT 0, \"PenaltyPercent\" numeric NOT NULL DEFAULT 0, \"FilePath\" text NULL, \"PmtDctStatus\" text NOT NULL DEFAULT 'Draft', \"DlrSignStatus\" text NOT NULL DEFAULT 'Pending', \"HTCSignStatus\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"HTCApprBy\" text NULL, \"HTCApprAt\" timestamp NULL, \"DlrSignBy\" text NULL, \"DlrSignAt\" timestamp NULL, \"HTCSignBy\" text NULL, \"HTCSignAt\" timestamp NULL, \"RejectBy\" text NULL, \"RejectAt\" timestamp NULL, \"CancelBy\" text NULL, \"CancelAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PaymentDiscountLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentDiscountId\" bigint NOT NULL, \"PaymentDiscountNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"GuaranteeNo\" text NULL, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"PaymentEndDatePhase1\" timestamp NULL, \"AmountPhase1\" numeric NOT NULL DEFAULT 0, \"DiscountDateNumberPhase1\" integer NOT NULL DEFAULT 0, \"DiscountPercentPhase1\" numeric NOT NULL DEFAULT 0, \"DiscountPricePhase1\" numeric NOT NULL DEFAULT 0, \"PaymentEndDatePhase2\" timestamp NULL, \"AmountPhase2\" numeric NOT NULL DEFAULT 0, \"DiscountDateNumberPhase2\" integer NOT NULL DEFAULT 0, \"DiscountPercentPhase2\" numeric NOT NULL DEFAULT 0, \"DiscountPricePhase2\" numeric NOT NULL DEFAULT 0, \"PaymentEndDatePhase3\" timestamp NULL, \"AmountPhase3\" numeric NOT NULL DEFAULT 0, \"DiscountDateNumberPhase3\" integer NOT NULL DEFAULT 0, \"DiscountPercentPhase3\" numeric NOT NULL DEFAULT 0, \"DiscountPricePhase3\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"TotalDiscountPrice\" numeric NOT NULL DEFAULT 0, \"PG_DateEnd\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"InsuranceRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"InsReqNo\" text NOT NULL DEFAULT '', \"InsCompanyCode\" text NOT NULL DEFAULT '', \"InsCompanyName\" text NULL, \"InsTypeCode\" text NOT NULL DEFAULT 'CARGO', \"PolicyNo\" text NULL, \"EffectiveDate\" timestamp NOT NULL DEFAULT now(), \"ExpireDate\" timestamp NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalInsuredValue\" numeric NOT NULL DEFAULT 0, \"PremiumRate\" numeric NOT NULL DEFAULT 0.15, \"TotalPremiumAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"InsuranceRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"InsuranceRequestId\" bigint NOT NULL, \"InsReqNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"InsuredValue\" numeric NOT NULL DEFAULT 0, \"PremiumRate\" numeric NOT NULL DEFAULT 0.15, \"PremiumAmount\" numeric NOT NULL DEFAULT 0, \"InsuranceDays\" integer NOT NULL DEFAULT 30, \"FromStorage\" text NULL, \"ToStorage\" text NULL, \"CertificateNo\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportMinutes\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportMinutesNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"TransporterCode\" text NOT NULL DEFAULT '', \"TransporterName\" text NULL, \"TruckPlateNo\" text NULL, \"DriverName\" text NULL, \"DriverPhone\" text NULL, \"TransportReqNo\" text NULL, \"DeliveryOrderNo\" text NULL, \"TransportMinutesDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalFreightAmount\" numeric NOT NULL DEFAULT 0, \"TotalSurchargeAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"FilePath\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"DLApprBy\" text NULL, \"DLApprAt\" timestamp NULL, \"DLApprNote\" text NULL, \"HTCAppr1By\" text NULL, \"HTCAppr1At\" timestamp NULL, \"HTCAppr2By\" text NULL, \"HTCAppr2At\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportMinutesLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportMinutesId\" bigint NOT NULL, \"TransportMinutesNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"DeliveryOrderNo\" text NULL, \"TransportReqNo\" text NULL, \"FromStorage\" text NULL, \"ToStorage\" text NULL, \"OdoDeparture\" integer NOT NULL DEFAULT 0, \"OdoArrival\" integer NOT NULL DEFAULT 0, \"FreightAmount\" numeric NOT NULL DEFAULT 0, \"Surcharge\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"CargoCondition\" text NOT NULL DEFAULT 'Good', \"IsInspectionPassed\" boolean NOT NULL DEFAULT true, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsPaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PaidAt\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastStorageMtnDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"NextStorageMtnDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"StorageMtnTimes\" integer NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"DeclarationNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TaxPaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsCustomsCleared\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"CustomsClearanceDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PackingListNo\" text NULL",
            "CREATE TABLE IF NOT EXISTS public.\"PackingLists\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PackingListNo\" text NOT NULL DEFAULT '', \"ContractNo\" text NULL, \"LCNo\" text NULL, \"PortCode\" text NOT NULL DEFAULT 'NHA_MAY_NINH_BINH', \"VesselName\" text NULL, \"VoyageNo\" text NULL, \"ShippingDateStart\" timestamp NULL, \"ShippingDateEndExpected\" timestamp NULL, \"ShippingDateEnd\" timestamp NULL, \"TotalQuantity\" integer NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PackingListLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PackingListId\" bigint NOT NULL, \"PackingListNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"ModelYear\" integer NULL DEFAULT 2026, \"KeyNo\" text NULL, \"ProductionDate\" timestamp NULL, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CustomsDeclarations\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DeclarationNo\" text NOT NULL DEFAULT '', \"PortCode\" text NOT NULL DEFAULT 'HQ_HAI_PHONG', \"PortName\" text NULL, \"ContractNo\" text NULL, \"LCNo\" text NULL, \"BillOfLadingNo\" text NULL, \"DeclarationType\" text NOT NULL DEFAULT 'CBU', \"OpenDate\" timestamp NOT NULL DEFAULT now(), \"TaxPaymentDate\" timestamp NULL, \"ClearanceDate\" timestamp NULL, \"CustomsOfficer\" text NULL, \"DeclarantName\" text NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalTaxValue\" numeric NOT NULL DEFAULT 0, \"ImportTaxAmount\" numeric NOT NULL DEFAULT 0, \"ExciseTaxAmount\" numeric NOT NULL DEFAULT 0, \"VatAmount\" numeric NOT NULL DEFAULT 0, \"TotalTaxAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"ClearedBy\" text NULL, \"ClearedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CustomsDeclarationLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CustomsDeclarationId\" bigint NOT NULL, \"DeclarationNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"ModelYear\" integer NULL DEFAULT 2026, \"PackingListNo\" text NULL, \"TaxValue\" numeric NOT NULL DEFAULT 0, \"ImportTaxRate\" numeric NOT NULL DEFAULT 50, \"ImportTax\" numeric NOT NULL DEFAULT 0, \"ExciseTaxRate\" numeric NOT NULL DEFAULT 35, \"ExciseTax\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"VatTax\" numeric NOT NULL DEFAULT 0, \"TotalTax\" numeric NOT NULL DEFAULT 0, \"TaxPaymentDate\" timestamp NULL, \"ClearanceDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"Payments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"PaymentType\" text NOT NULL DEFAULT 'Payment', \"BankNameSend\" text NULL, \"BankNameReceive\" text NULL, \"BankPaymentNo\" text NULL, \"AccountingRecordNo\" text NULL, \"PaymentEndDate\" timestamp NULL, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"ConfirmBy\" text NULL, \"ConfirmedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DealerPaymentId\" bigint NOT NULL, \"PaymentNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"GuaranteeNo\" text NULL, \"Amount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StorageMaintenances\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MtnNo\" text NOT NULL DEFAULT '', \"StorageCode\" text NOT NULL DEFAULT '', \"MtnType\" text NOT NULL DEFAULT 'Periodic', \"PlanDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"PassedVehicleCount\" integer NOT NULL DEFAULT 0, \"FailedVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"TechnicianCode\" text NULL, \"TechnicianName\" text NULL, \"SupervisorCode\" text NULL, \"SupervisorName\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StorageMaintenanceLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"StorageMaintenanceId\" bigint NOT NULL, \"MtnNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"StorageCode\" text NULL, \"MtnTimes\" integer NOT NULL DEFAULT 0, \"BatteryVoltage\" double precision NULL DEFAULT 12.6, \"ChargeBatteryOk\" boolean NOT NULL DEFAULT true, \"EngineStartCheckOk\" boolean NOT NULL DEFAULT true, \"TirePressureCheckOk\" boolean NOT NULL DEFAULT true, \"TireRotationOk\" boolean NOT NULL DEFAULT true, \"FluidLevelsCheckOk\" boolean NOT NULL DEFAULT true, \"ElectricalSystemsOk\" boolean NOT NULL DEFAULT true, \"BodyCleanOk\" boolean NOT NULL DEFAULT true, \"InspectionResult\" text NOT NULL DEFAULT 'Pending', \"MtnDate\" timestamp NULL, \"NextMtnDate\" timestamp NULL, \"Technician\" text NULL, \"DefectNotes\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TypeCB\" text NOT NULL DEFAULT '0'",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LoaiThung\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"CBReqNo\" text NULL",
            "CREATE TABLE IF NOT EXISTS public.\"CarBoxRequests\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CBReqNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NULL, \"BodyBuilder\" text NULL, \"RequestDate\" timestamp NOT NULL DEFAULT now(), \"ExpectedStartDate\" timestamp NULL, \"ExpectedEndDate\" timestamp NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarBoxRequestLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CarBoxRequestId\" bigint NOT NULL, \"CBReqNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"StorageCodeFrom\" text NULL, \"StorageCodeTo\" text NOT NULL DEFAULT '', \"LoaiThung\" text NOT NULL DEFAULT 'ThungBat', \"TenLoaiThung\" text NULL, \"BoxLengthMm\" double precision NULL, \"BoxWidthMm\" double precision NULL, \"BoxHeightMm\" double precision NULL, \"PayloadKg\" double precision NULL, \"BodyPrice\" numeric NOT NULL DEFAULT 0, \"BodyBuilder\" text NULL, \"InspectionNo\" text NULL, \"InspectionResult\" text NOT NULL DEFAULT 'Pending', \"InspectionDate\" timestamp NULL, \"InspectorName\" text NULL, \"DefectNotes\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"CompletedDate\" timestamp NULL, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsInvoiced\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"InvoiceNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"InvoiceDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"InvoiceListCode\" text NULL",
            "CREATE TABLE IF NOT EXISTS public.\"CarInvoices\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"InvoiceListCode\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"InvoiceType\" text NOT NULL DEFAULT 'VAT', \"InvoiceDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalTaxValue\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"IssuedBy\" text NULL, \"IssuedAt\" timestamp NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarInvoiceLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CarInvoiceId\" bigint NOT NULL, \"InvoiceListCode\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"InvoiceDealerCode\" text NOT NULL DEFAULT '', \"InvoiceNo\" text NOT NULL DEFAULT '', \"InvoiceDate\" timestamp NULL, \"TaxValue\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"VatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"GuaranteeLines\" ADD COLUMN IF NOT EXISTS \"LastGrtExtNo\" text NULL",
            "ALTER TABLE public.\"GuaranteeLines\" ADD COLUMN IF NOT EXISTS \"ExtensionTimes\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"GuaranteeExtensions\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GrtClaimExtNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"BankCode\" text NULL, \"GuaranteeNo\" text NULL, \"ExtensionDays\" integer NOT NULL DEFAULT 30, \"FeeRate\" numeric NOT NULL DEFAULT 0, \"TotalFeeAmount\" numeric NOT NULL DEFAULT 0, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalGuaranteeAmount\" numeric NOT NULL DEFAULT 0, \"FileSigned\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SignedBy\" text NULL, \"SignedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GuaranteeExtensionLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GuaranteeExtensionId\" bigint NOT NULL, \"GrtClaimExtNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"GuaranteeNo\" text NULL, \"CurrentDateExpired\" timestamp NULL, \"NewDateExpired\" timestamp NULL, \"ExtensionDays\" integer NOT NULL DEFAULT 30, \"GuaranteeValue\" numeric NOT NULL DEFAULT 0, \"FeeRate\" numeric NOT NULL DEFAULT 0, \"ExtensionFee\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ContractCancels\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ContractCNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DlrContractNo\" text NULL, \"CancelType\" text NOT NULL DEFAULT 'Partial', \"CancelReason\" text NULL, \"TotalCancelQty\" integer NOT NULL DEFAULT 0, \"TotalCancelAmount\" numeric NOT NULL DEFAULT 0, \"DepositRefundAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ContractCancelLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ContractCancelId\" bigint NOT NULL, \"ContractCNo\" text NOT NULL DEFAULT '', \"DlrContractNo\" text NOT NULL DEFAULT '', \"Vin\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"Color\" text NULL, \"ContractUpdateType\" text NOT NULL DEFAULT 'CANCEL_VIN', \"CancelQty\" integer NOT NULL DEFAULT 1, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"RefundAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarColorChanges\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ChangeNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"ChangeType\" text NOT NULL DEFAULT 'DealerRequest', \"Reason\" text NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CarColorChangeLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CarColorChangeId\" bigint NOT NULL, \"ChangeNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"OldColor\" text NOT NULL DEFAULT '', \"NewColor\" text NOT NULL DEFAULT '', \"OldColorCode\" text NULL, \"NewColorCode\" text NULL, \"OldColorName\" text NULL, \"NewColorName\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsBankBillHandedOver\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"BankBillMnNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"BankBillHandoverDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"BankBillMinutes\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"BankBillMnNo\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT '', \"BankName\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"GuaranteeNo\" text NULL, \"BankBillDate\" timestamp NOT NULL DEFAULT now(), \"BankBillReceiveDate\" timestamp NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"BankOfficer\" text NULL, \"HTCOfficer\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"BankBillMinutesLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"BankBillMinutesId\" bigint NOT NULL, \"BankBillMnNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"InvoiceDealerCode\" text NULL, \"InvoiceNo\" text NULL, \"InvoiceDate\" timestamp NULL, \"GuaranteeNo\" text NULL, \"CarPrice\" numeric NOT NULL DEFAULT 0, \"GuaranteeValue\" numeric NOT NULL DEFAULT 0, \"HasOriginalInvoice\" boolean NOT NULL DEFAULT true, \"HasQualityCert\" boolean NOT NULL DEFAULT true, \"HasInspectionCert\" boolean NOT NULL DEFAULT true, \"HasWarrantyBooklet\" boolean NOT NULL DEFAULT true, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GuaranteeClaims\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ClaimNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT '', \"BankName\" text NULL, \"GuaranteeNo\" text NULL, \"ClaimDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalClaimAmount\" numeric NOT NULL DEFAULT 0, \"ClaimReason\" text NOT NULL DEFAULT 'OverduePayment', \"FileSigned\" text NULL, \"BankRefNo\" text NULL, \"DisbursementDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GuaranteeClaimLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GuaranteeClaimId\" bigint NOT NULL, \"ClaimNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"GuaranteeNo\" text NULL, \"GuaranteeValue\" numeric NOT NULL DEFAULT 0, \"ClaimAmount\" numeric NOT NULL DEFAULT 0, \"DueDate\" timestamp NULL, \"OverdueDays\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"ContractNoOversea\" text NULL",
            "CREATE TABLE IF NOT EXISTS public.\"ContractOverseas\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ContractNo\" text NOT NULL DEFAULT '', \"ContractNoUser\" text NULL, \"SupplierCode\" text NOT NULL DEFAULT '', \"SupplierName\" text NULL, \"IncotermsCode\" text NOT NULL DEFAULT 'CIF_HAI_PHONG', \"Currency\" text NOT NULL DEFAULT 'USD', \"ExchangeRate\" numeric NOT NULL DEFAULT 25450, \"PaymentTerm\" text NOT NULL DEFAULT 'LC', \"DeparturePort\" text NOT NULL DEFAULT 'BUSAN', \"ArrivalPort\" text NOT NULL DEFAULT 'CANG_HAI_PHONG', \"OrderMonth\" text NULL, \"ProductionMonth\" text NULL, \"ExpectedDeliveryMonth\" text NULL, \"ContractDate\" timestamp NOT NULL DEFAULT now(), \"DeliveryDeadline\" timestamp NULL, \"TotalQuantity\" integer NOT NULL DEFAULT 0, \"TotalAmountForeign\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"FileSigned\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ContractOverseaLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ContractOverseaId\" bigint NOT NULL, \"ContractNo\" text NOT NULL DEFAULT '', \"Vin\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"Color\" text NULL, \"ColorCode\" text NULL, \"ModelYear\" integer NULL DEFAULT 2026, \"PlantCode\" text NULL, \"PortCode\" text NULL, \"WorkOrderNo\" text NULL, \"LCTemp\" text NULL, \"OrderQty\" integer NOT NULL DEFAULT 1, \"UnitPriceForeign\" numeric NOT NULL DEFAULT 0, \"TotalAmountForeign\" numeric NOT NULL DEFAULT 0, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LCNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastRoNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastRoDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastOdoKm\" integer NULL",
            "CREATE TABLE IF NOT EXISTS public.\"LettersOfCredit\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"LCNo\" text NOT NULL DEFAULT '', \"LCNoUser\" text NULL, \"ContractNo\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT 'VCB', \"BankName\" text NULL, \"BeneficiaryName\" text NULL, \"ApplicantName\" text NULL, \"Currency\" text NOT NULL DEFAULT 'USD', \"ExchangeRate\" numeric NOT NULL DEFAULT 25450, \"LCAmountForeign\" numeric NOT NULL DEFAULT 0, \"LCAmount\" numeric NOT NULL DEFAULT 0, \"MarginRate\" numeric NOT NULL DEFAULT 10, \"MarginAmount\" numeric NOT NULL DEFAULT 0, \"IssueDate\" timestamp NOT NULL DEFAULT now(), \"ExpiryDate\" timestamp NOT NULL DEFAULT now(), \"LatestShipmentDate\" timestamp NULL, \"PaymentTerm\" text NOT NULL DEFAULT 'AtSight', \"DeparturePort\" text NOT NULL DEFAULT 'BUSAN', \"ArrivalPort\" text NOT NULL DEFAULT 'CANG_HAI_PHONG', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"UtilizedAmountForeign\" numeric NOT NULL DEFAULT 0, \"UtilizedAmount\" numeric NOT NULL DEFAULT 0, \"RemainingAmountForeign\" numeric NOT NULL DEFAULT 0, \"RemainingAmount\" numeric NOT NULL DEFAULT 0, \"SwiftCode\" text NULL, \"FileSigned\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"UtilizedBy\" text NULL, \"UtilizedAt\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"LetterOfCreditLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"LetterOfCreditId\" bigint NOT NULL, \"LCNo\" text NOT NULL DEFAULT '', \"ContractNo\" text NOT NULL DEFAULT '', \"Vin\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"OrderQty\" integer NOT NULL DEFAULT 1, \"UnitPriceForeign\" numeric NOT NULL DEFAULT 0, \"TotalAmountForeign\" numeric NOT NULL DEFAULT 0, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"PackingListNo\" text NULL, \"DeclarationNo\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"RepairOrders\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RoNo\" text NOT NULL DEFAULT '', \"RoNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"CustomerName\" text NULL, \"CustomerPhone\" text NULL, \"RoType\" text NOT NULL DEFAULT 'PeriodicMaintenance', \"ServiceAdvisor\" text NULL, \"Technician\" text NULL, \"OdoKm\" integer NOT NULL DEFAULT 0, \"FuelLevel\" text NULL DEFAULT '1/2', \"CarStatus\" text NULL, \"CustomerRequest\" text NULL, \"DiagnosisNotes\" text NULL, \"CheckInDate\" timestamp NOT NULL DEFAULT now(), \"ExpectedDeliveryDate\" timestamp NULL, \"ActualDeliveryDate\" timestamp NULL, \"TotalLaborAmount\" numeric NOT NULL DEFAULT 0, \"TotalPartAmount\" numeric NOT NULL DEFAULT 0, \"DiscountAmount\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"PaymentStatus\" text NOT NULL DEFAULT 'Unpaid', \"PaymentMethod\" text NOT NULL DEFAULT 'Cash', \"PaymentNotes\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"RepairedBy\" text NULL, \"RepairedAt\" timestamp NULL, \"DeliveredBy\" text NULL, \"DeliveredAt\" timestamp NULL, \"PaidBy\" text NULL, \"PaidAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"RepairOrderServiceLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RepairOrderId\" bigint NOT NULL, \"RoNo\" text NOT NULL DEFAULT '', \"SerCode\" text NOT NULL DEFAULT '', \"SerName\" text NOT NULL DEFAULT '', \"ServiceType\" text NOT NULL DEFAULT 'Maintenance', \"StandardHours\" numeric NOT NULL DEFAULT 1.0, \"LaborPrice\" numeric NOT NULL DEFAULT 300000, \"Discount\" numeric NOT NULL DEFAULT 0, \"LaborAmount\" numeric NOT NULL DEFAULT 300000, \"Technician\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"RepairOrderPartLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RepairOrderId\" bigint NOT NULL, \"RoNo\" text NOT NULL DEFAULT '', \"PartCode\" text NOT NULL DEFAULT '', \"PartName\" text NOT NULL DEFAULT '', \"Unit\" text NOT NULL DEFAULT 'Cái', \"Quantity\" numeric NOT NULL DEFAULT 1, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Discount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"PaymentType\" text NOT NULL DEFAULT 'Customer', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAppointmentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAppointmentDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceAppointments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"AppNo\" text NOT NULL DEFAULT '', \"AppNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"ServiceType\" text NOT NULL DEFAULT 'PeriodicMaintenance', \"AppointmentDate\" timestamp NOT NULL DEFAULT now(), \"AppointmentTime\" text NOT NULL DEFAULT '08:30', \"EstimatedDurationMinutes\" integer NOT NULL DEFAULT 60, \"ServiceAdvisor\" text NULL, \"Technician\" text NULL, \"InsNo\" text NULL, \"CustomerRequest\" text NULL, \"TotalEstimatedLabor\" numeric NOT NULL DEFAULT 0, \"TotalEstimatedParts\" numeric NOT NULL DEFAULT 0, \"TotalEstimatedAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Booked', \"RoNo\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ConfirmedBy\" text NULL, \"ConfirmedAt\" timestamp NULL, \"CheckedInBy\" text NULL, \"CheckedInAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL, \"NoShowAt\" timestamp NULL, \"NoShowReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceAppointmentServiceLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServiceAppointmentId\" bigint NOT NULL, \"AppNo\" text NOT NULL DEFAULT '', \"SerCode\" text NOT NULL DEFAULT '', \"SerName\" text NOT NULL DEFAULT '', \"ServiceType\" text NOT NULL DEFAULT 'Maintenance', \"StandardHours\" numeric NOT NULL DEFAULT 1.0, \"LaborPrice\" numeric NOT NULL DEFAULT 300000, \"Discount\" numeric NOT NULL DEFAULT 0, \"LaborAmount\" numeric NOT NULL DEFAULT 300000, \"Technician\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceAppointmentPartLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServiceAppointmentId\" bigint NOT NULL, \"AppNo\" text NOT NULL DEFAULT '', \"PartCode\" text NOT NULL DEFAULT '', \"PartName\" text NOT NULL DEFAULT '', \"Unit\" text NOT NULL DEFAULT 'Cái', \"Quantity\" numeric NOT NULL DEFAULT 1, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Discount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"PaymentType\" text NOT NULL DEFAULT 'Customer', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastBulletinNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastBulletinDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"TechnicalBulletins\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"BulletinNo\" text NOT NULL DEFAULT '', \"BulletinNoUser\" text NULL, \"Title\" text NOT NULL DEFAULT '', \"Category\" text NOT NULL DEFAULT 'SoftwareUpdate', \"Model\" text NULL, \"Severity\" text NOT NULL DEFAULT 'Medium', \"ReleaseDate\" timestamp NOT NULL DEFAULT now(), \"ExpiryDate\" timestamp NULL, \"Description\" text NULL, \"Remedy\" text NULL, \"AttachmentFileName\" text NULL, \"AttachmentUrl\" text NULL, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"CompletedVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"PublishedBy\" text NULL, \"PublishedAt\" timestamp NULL, \"ArchivedBy\" text NULL, \"ArchivedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TechnicalBulletinLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TechnicalBulletinId\" bigint NOT NULL, \"BulletinNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"DealerCode\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"InspectedAt\" timestamp NULL, \"CompletedAt\" timestamp NULL, \"Technician\" text NULL, \"OdoKm\" integer NULL, \"RoNo\" text NULL, \"ResultNotes\" text NULL, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastDisbursementNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastDisbursementDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"BankDisbursements\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RQ_BankingTransNo\" text NOT NULL DEFAULT '', \"RQ_BankingTransNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"BankCode\" text NOT NULL DEFAULT 'VIETINBANK', \"BankName\" text NULL, \"BizResNumber\" text NULL, \"BeneficiaryAccountNo\" text NULL, \"BeneficiaryAccountName\" text NULL, \"BeneficiaryBankCode\" text NULL, \"DisbursementType\" text NOT NULL DEFAULT 'AutoLoan', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalCollateralValue\" numeric NOT NULL DEFAULT 0, \"DisbursementRate\" numeric NOT NULL DEFAULT 80, \"TotalDisbursementAmount\" numeric NOT NULL DEFAULT 0, \"DisbursedAmount\" numeric NOT NULL DEFAULT 0, \"BkTransStatus\" text NOT NULL DEFAULT 'Draft', \"BkTransBankStatus\" text NOT NULL DEFAULT 'Pending', \"RefBankCode\" text NULL, \"DisbursementDate\" timestamp NULL, \"BankRemark\" text NULL, \"FilePath\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"PushedBy\" text NULL, \"PushedAt\" timestamp NULL, \"DisbursedBy\" text NULL, \"DisbursedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"BankDisbursementLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"BankDisbursementId\" bigint NOT NULL, \"RQ_BankingTransNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"InvoiceNo\" text NULL, \"InvoiceDate\" timestamp NULL, \"GuaranteeNo\" text NULL, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"CollateralValue\" numeric NOT NULL DEFAULT 0, \"DisbursementPercent\" numeric NOT NULL DEFAULT 80, \"DisbursementAmount\" numeric NOT NULL DEFAULT 0, \"DisbursedAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCampaignNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCampaignDate\" timestamp NULL",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceCampaigns\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CamMarketingNo\" text NOT NULL DEFAULT '', \"CamMarketingNoUser\" text NULL, \"CampaignName\" text NOT NULL DEFAULT '', \"CampaignType\" text NOT NULL DEFAULT 'SeasonalService', \"Model\" text NULL, \"DateStart\" timestamp NOT NULL DEFAULT now(), \"DateEnd\" timestamp NOT NULL DEFAULT now(), \"DiscountLaborPercent\" numeric NOT NULL DEFAULT 0, \"DiscountPartPercent\" numeric NOT NULL DEFAULT 0, \"FreeInspectionItems\" text NULL, \"GiftDescription\" text NULL, \"BudgetAmount\" numeric NOT NULL DEFAULT 0, \"ActualAmount\" numeric NOT NULL DEFAULT 0, \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"AttendedVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceCampaignLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServiceCampaignId\" bigint NOT NULL, \"CamMarketingNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"CustomerName\" text NULL, \"CustomerPhone\" text NULL, \"ServiceDate\" timestamp NULL, \"RoNo\" text NULL, \"DiscountLaborAmount\" numeric NOT NULL DEFAULT 0, \"DiscountPartAmount\" numeric NOT NULL DEFAULT 0, \"TotalDiscountAmount\" numeric NOT NULL DEFAULT 0, \"IsGiftDelivered\" boolean NOT NULL DEFAULT false, \"GiftName\" text NULL, \"Technician\" text NULL, \"ServiceAdvisor\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastWarrantyReportNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastWarrantyReportDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"WarrantyClaimCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"WarrantyReports\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ROWNo\" text NOT NULL DEFAULT '', \"ROWNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"RoNo\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"PlateNo\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"OdoKm\" integer NOT NULL DEFAULT 0, \"CheckInDate\" timestamp NOT NULL DEFAULT now(), \"StartDate\" timestamp NULL, \"FinishedDate\" timestamp NULL, \"WarrantyStartDate\" timestamp NULL, \"WarrantyEndDate\" timestamp NULL, \"WarrantyMonths\" integer NOT NULL DEFAULT 36, \"CusName\" text NULL, \"CusTel\" text NULL, \"CusAddress\" text NULL, \"CusRequest\" text NULL, \"DiagnosticResult\" text NULL, \"NaturalCode\" text NOT NULL DEFAULT 'C01', \"CauseCode\" text NOT NULL DEFAULT 'M01', \"MainPartCode\" text NULL, \"MainPartName\" text NULL, \"WarrantyType\" text NOT NULL DEFAULT 'Standard', \"TotalLaborAmount\" numeric NOT NULL DEFAULT 0, \"TotalPartAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"ApprovedLaborAmount\" numeric NOT NULL DEFAULT 0, \"ApprovedPartAmount\" numeric NOT NULL DEFAULT 0, \"ApprovedTotalAmount\" numeric NOT NULL DEFAULT 0, \"ReimbursedAmount\" numeric NOT NULL DEFAULT 0, \"ReimburseDate\" timestamp NULL, \"AccountingRefNo\" text NULL, \"OldPartsInspectionStatus\" text NOT NULL DEFAULT 'PendingReturn', \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ConfirmedBy\" text NULL, \"ConfirmedAt\" timestamp NULL, \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"WarrantyReportLaborLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"WarrantyReportId\" bigint NOT NULL, \"ROWNo\" text NOT NULL DEFAULT '', \"SerCode\" text NOT NULL DEFAULT '', \"SerName\" text NOT NULL DEFAULT '', \"StdManHour\" numeric NOT NULL DEFAULT 1.0, \"LaborPrice\" numeric NOT NULL DEFAULT 300000, \"LaborAmount\" numeric NOT NULL DEFAULT 300000, \"ApprovedManHour\" numeric NOT NULL DEFAULT 1.0, \"ApprovedLaborAmount\" numeric NOT NULL DEFAULT 300000, \"Technician\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"RejectReason\" text NULL, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"WarrantyReportPartLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"WarrantyReportId\" bigint NOT NULL, \"ROWNo\" text NOT NULL DEFAULT '', \"PartCode\" text NOT NULL DEFAULT '', \"PartName\" text NOT NULL DEFAULT '', \"Unit\" text NOT NULL DEFAULT 'Cái', \"Quantity\" numeric NOT NULL DEFAULT 1, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"ApprovedQty\" numeric NOT NULL DEFAULT 1, \"ApprovedAmount\" numeric NOT NULL DEFAULT 0, \"IsMainPart\" boolean NOT NULL DEFAULT false, \"OldPartSerialNo\" text NULL, \"OldPartReturnStatus\" text NOT NULL DEFAULT 'PendingReturn', \"Status\" text NOT NULL DEFAULT 'Pending', \"RejectReason\" text NULL, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastQuoteNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastQuoteDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"QuotationCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceQuotations\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"QuoteNo\" text NOT NULL DEFAULT '', \"QuoteNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"OdoKm\" integer NOT NULL DEFAULT 0, \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"CustomerAddress\" text NULL, \"CustomerType\" text NOT NULL DEFAULT 'Individual', \"QuotationType\" text NOT NULL DEFAULT 'PeriodicMaintenance', \"ServiceAdvisor\" text NULL, \"QuoteDate\" timestamp NOT NULL DEFAULT now(), \"ValidUntilDate\" timestamp NULL, \"PaymentMethod\" text NOT NULL DEFAULT 'Cash', \"TotalLaborAmount\" numeric NOT NULL DEFAULT 0, \"TotalPartAmount\" numeric NOT NULL DEFAULT 0, \"DiscountAmount\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"ApprovedByCustomer\" boolean NOT NULL DEFAULT false, \"CustomerApprovedAt\" timestamp NULL, \"CustomerSignature\" text NULL, \"ConvertedRoNo\" text NULL, \"ConvertedAt\" timestamp NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"SentBy\" text NULL, \"SentAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceQuotationLaborLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServiceQuotationId\" bigint NOT NULL, \"QuoteNo\" text NOT NULL DEFAULT '', \"SerCode\" text NOT NULL DEFAULT '', \"SerName\" text NOT NULL DEFAULT '', \"ServiceType\" text NOT NULL DEFAULT 'Maintenance', \"StandardHours\" numeric NOT NULL DEFAULT 1.0, \"LaborPrice\" numeric NOT NULL DEFAULT 300000, \"Discount\" numeric NOT NULL DEFAULT 0, \"LaborAmount\" numeric NOT NULL DEFAULT 300000, \"Technician\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceQuotationPartLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServiceQuotationId\" bigint NOT NULL, \"QuoteNo\" text NOT NULL DEFAULT '', \"PartCode\" text NOT NULL DEFAULT '', \"PartName\" text NOT NULL DEFAULT '', \"Unit\" text NOT NULL DEFAULT 'Cái', \"Quantity\" numeric NOT NULL DEFAULT 1, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"Discount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"PaymentType\" text NOT NULL DEFAULT 'Customer', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCareNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCareDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCareType\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCsiScore\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"CareCount\" integer NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastWorkOrderNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"ManufacturedDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PlantCode\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPiNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPiDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PiCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"CustomerCares\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CareNo\" text NOT NULL DEFAULT '', \"CareNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"PlateNo\" text NULL, \"Model\" text NULL, \"EngineNo\" text NULL, \"CustomerName\" text NULL, \"CustomerPhone\" text NULL, \"CustomerEmail\" text NULL, \"CustomerAddress\" text NULL, \"CareType\" text NOT NULL DEFAULT 'FollowUp72h', \"ContactMethod\" text NOT NULL DEFAULT 'PhoneCall', \"RoNo\" text NULL, \"DoNo\" text NULL, \"OdoKm\" integer NULL, \"ServiceDate\" timestamp NULL, \"ContactDate\" timestamp NULL, \"NextCareDate\" timestamp NULL, \"CallAttempts\" integer NOT NULL DEFAULT 1, \"CareStaff\" text NULL, \"ServiceAdvisor\" text NULL, \"ScoreOverall\" numeric NOT NULL DEFAULT 5.0, \"ScoreQuality\" numeric NOT NULL DEFAULT 5.0, \"ScoreAdvisor\" numeric NOT NULL DEFAULT 5.0, \"ScoreFacility\" numeric NOT NULL DEFAULT 5.0, \"IsProblemSolved\" boolean NOT NULL DEFAULT true, \"NpsScore\" integer NOT NULL DEFAULT 10, \"CustomerFeedback\" text NULL, \"RemedyAction\" text NULL, \"IsResolved\" boolean NOT NULL DEFAULT true, \"Status\" text NOT NULL DEFAULT 'Pending', \"EscalatedTo\" text NULL, \"EscalatedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"ProductionOrders\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"OrderNo\" text NOT NULL DEFAULT '', \"OrderNoUser\" text NULL, \"OrdMonth\" text NOT NULL DEFAULT '', \"OrdType\" text NOT NULL DEFAULT 'MTO', \"OrdCategoryType\" text NOT NULL DEFAULT 'MakeToOrder', \"PlantCode\" text NOT NULL DEFAULT 'HTMV_NINHBINH_1', \"PlantName\" text NULL, \"TotalPlanQty\" integer NOT NULL DEFAULT 0, \"TotalProducedQty\" integer NOT NULL DEFAULT 0, \"EstimatedCompletionDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ScheduledBy\" text NULL, \"ScheduledAt\" timestamp NULL, \"StartedBy\" text NULL, \"StartedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ProductionOrderLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ProductionOrderId\" bigint NOT NULL, \"OrderNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NOT NULL DEFAULT '', \"SpecDescription\" text NULL, \"ColorCode\" text NOT NULL DEFAULT '', \"ColorName\" text NULL, \"PlanQty\" integer NOT NULL DEFAULT 1, \"QtyMonthN1\" integer NOT NULL DEFAULT 0, \"QtyMonthN2\" integer NOT NULL DEFAULT 0, \"QtyMonthN3\" integer NOT NULL DEFAULT 0, \"ProducedQty\" integer NOT NULL DEFAULT 0, \"ETADate\" timestamp NULL, \"Stage\" text NOT NULL DEFAULT 'Stamping', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ProformaInvoices\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"RefNo\" text NOT NULL DEFAULT '', \"RefNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"OrderMonth\" text NOT NULL DEFAULT '', \"ProductionMonth\" text NULL, \"ExpectedDeliveryMonth\" text NULL, \"Currency\" text NOT NULL DEFAULT 'USD', \"ExchangeRate\" numeric NOT NULL DEFAULT 25450, \"TotalQuantity\" integer NOT NULL DEFAULT 0, \"TotalAmountForeign\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"DepositRate\" numeric NOT NULL DEFAULT 10, \"DepositAmount\" numeric NOT NULL DEFAULT 0, \"PaymentTerm\" text NOT NULL DEFAULT 'LC', \"DeparturePort\" text NULL, \"ArrivalPort\" text NULL, \"LCTemp\" text NULL, \"LCNo\" text NULL, \"ContractNo\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"ExecutedBy\" text NULL, \"ExecutedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ProformaInvoiceLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ProformaInvoiceId\" bigint NOT NULL, \"RefNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NOT NULL DEFAULT '', \"SpecDescription\" text NULL, \"ColorCode\" text NOT NULL DEFAULT 'NWAC', \"ColorName\" text NULL, \"WorkOrderNo\" text NULL, \"PlantCode\" text NULL, \"PortCode\" text NULL, \"LCTemp\" text NULL, \"ContractNo\" text NULL, \"OrderQty\" integer NOT NULL DEFAULT 1, \"AllocatedQty\" integer NOT NULL DEFAULT 0, \"UnitPriceForeign\" numeric NOT NULL DEFAULT 0, \"TotalAmountForeign\" numeric NOT NULL DEFAULT 0, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsPdiPaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PdiPaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPdiPaymentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPdiPaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PdiPaymentCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"PdiPayments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PmtPdiNo\" text NOT NULL DEFAULT '', \"PmtPdiNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"StorageCode\" text NULL, \"PeriodMonth\" text NOT NULL DEFAULT '', \"PaymentDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalCostIn\" numeric NOT NULL DEFAULT 0, \"TotalCostOut\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"VatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmountAfterVAT\" numeric NOT NULL DEFAULT 0, \"FileSigned\" text NULL, \"BankRefNo\" text NULL, \"SettledDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"Approved1By\" text NULL, \"Approved1At\" timestamp NULL, \"Approved2By\" text NULL, \"Approved2At\" timestamp NULL, \"TCMSSignedBy\" text NULL, \"TCMSSignedAt\" timestamp NULL, \"HTVSignedBy\" text NULL, \"HTVSignedAt\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"PdiPaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PdiPaymentId\" bigint NOT NULL, \"PmtPdiNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"StorageCode\" text NULL, \"DealerCode\" text NULL, \"PdiReqNo\" text NULL, \"DlvMnNo\" text NULL, \"CostInCheck\" numeric NOT NULL DEFAULT 0, \"CostOutCheck\" numeric NOT NULL DEFAULT 0, \"TotalCostCheck\" numeric NOT NULL DEFAULT 0, \"PdiCompletedDate\" timestamp NULL, \"PdiResult\" text NOT NULL DEFAULT 'Passed', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsPolicySupported\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PolicySupportAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPolicyCode\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPolicyDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PolicySupportCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"SalesPolicies\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SPSRCode\" text NOT NULL DEFAULT '', \"SPNo\" text NOT NULL DEFAULT '', \"SPSRType\" text NULL DEFAULT 'RetailSupport', \"SPSRRoot\" text NULL, \"FormBusinessSupportCode\" text NULL DEFAULT 'DirectCash', \"StartDate\" timestamp NOT NULL DEFAULT now(), \"EndDate\" timestamp NOT NULL DEFAULT now(), \"TotalModelsCount\" integer NOT NULL DEFAULT 0, \"TotalSupportBudget\" numeric NOT NULL DEFAULT 0, \"TotalVinApplied\" integer NOT NULL DEFAULT 0, \"TotalActualPaidAmount\" numeric NOT NULL DEFAULT 0, \"FilePath\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SuspendedBy\" text NULL, \"SuspendedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"SalesPolicyLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SalesPolicyId\" bigint NOT NULL, \"SPSRCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NOT NULL DEFAULT '', \"SpecDescription\" text NULL, \"DealerCode\" text NULL, \"ModelYear\" integer NULL DEFAULT 2026, \"AmountSupport\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Active', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"SalesPolicySupports\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SupportNo\" text NOT NULL DEFAULT '', \"SPSRCode\" text NOT NULL DEFAULT '', \"SPNo\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"DateSupport\" timestamp NOT NULL DEFAULT now(), \"DateFullStatus\" timestamp NULL, \"AmountSupport\" numeric NOT NULL DEFAULT 0, \"HTCInvoiceNo\" text NULL, \"HTCInvoiceDate\" timestamp NULL, \"HTCDatePayment\" timestamp NULL, \"BankRefNo\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsGpsInstalled\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsCode\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsInstallDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsUninstallDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsLatitude\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsLongitude\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsAddress\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsSpeed\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsBatteryVolt\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsSignalTime\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsDeviceCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"GpsDevices\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsCode\" text NOT NULL DEFAULT '', \"GpsBoxNo\" text NULL, \"SerialNo\" text NULL, \"ImeiNo\" text NULL, \"SimNo\" text NULL, \"Provider\" text NOT NULL DEFAULT 'Viettel', \"ModelName\" text NOT NULL DEFAULT 'OBD-4G', \"StorageCodeGps\" text NOT NULL DEFAULT 'KHO_GPS_NINHBINH', \"DeviceStatus\" text NOT NULL DEFAULT 'InStock', \"BatteryVolt\" numeric NOT NULL DEFAULT 12.6, \"BatteryPercent\" integer NOT NULL DEFAULT 100, \"CurrentVin\" text NULL, \"CurrentModel\" text NULL, \"CurrentLocation\" text NULL, \"Latitude\" numeric NULL, \"Longitude\" numeric NULL, \"SpeedKmH\" numeric NOT NULL DEFAULT 0, \"IsInGeofence\" boolean NOT NULL DEFAULT true, \"LastSignalAt\" timestamp NULL, \"LastGpsInNo\" text NULL, \"LastGpsOutNo\" text NULL, \"LastClaimNo\" text NULL, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"UpdatedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsInstallations\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsInNo\" text NOT NULL DEFAULT '', \"GpsInNoUser\" text NULL, \"GpsInType\" text NOT NULL DEFAULT 'First_In', \"StorageCodeGps\" text NOT NULL DEFAULT 'KHO_GPS_NINHBINH', \"InstallationDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsInstallationLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsInstallationId\" bigint NOT NULL, \"GpsInNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"Color\" text NULL, \"StorageCode\" text NULL, \"GpsCode\" text NOT NULL DEFAULT '', \"ImeiNo\" text NULL, \"SimNo\" text NULL, \"BatteryVolt\" numeric NOT NULL DEFAULT 12.6, \"Technician\" text NULL, \"InstalledAt\" timestamp NULL, \"InitialSignalStatus\" text NOT NULL DEFAULT 'SignalOK', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsUninstallations\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsOutNo\" text NOT NULL DEFAULT '', \"GpsOutNoUser\" text NULL, \"Reason\" text NOT NULL DEFAULT 'DeliveryToDealer', \"StorageCodeGps\" text NOT NULL DEFAULT 'KHO_GPS_NINHBINH', \"ReceiverName\" text NULL, \"UninstallDate\" timestamp NOT NULL DEFAULT now(), \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsUninstallationLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsUninstallationId\" bigint NOT NULL, \"GpsOutNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"GpsCode\" text NULL, \"OdoKm\" integer NULL, \"DeviceCondition\" text NOT NULL DEFAULT 'Good', \"Technician\" text NULL, \"UninstalledAt\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsClaims\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsClaimNo\" text NOT NULL DEFAULT '', \"GpsClaimNoUser\" text NULL, \"GpsCode\" text NOT NULL DEFAULT '', \"ImeiNo\" text NULL, \"SimNo\" text NULL, \"VendorCode\" text NOT NULL DEFAULT 'VELOCA', \"VendorName\" text NULL, \"FaultType\" text NOT NULL DEFAULT 'PowerLoss', \"FaultDescription\" text NULL, \"Vin\" text NULL, \"RepairCost\" numeric NOT NULL DEFAULT 0, \"ReplacementGpsCode\" text NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"SubmittedBy\" text NULL, \"SubmittedAt\" timestamp NULL, \"SentBy\" text NULL, \"SentAt\" timestamp NULL, \"RepairedBy\" text NULL, \"RepairedAt\" timestamp NULL, \"ReceivedBy\" text NULL, \"ReceivedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsLocationLogs\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsCode\" text NOT NULL DEFAULT '', \"Vin\" text NULL, \"Latitude\" numeric NOT NULL DEFAULT 0, \"Longitude\" numeric NOT NULL DEFAULT 0, \"SpeedKmH\" numeric NOT NULL DEFAULT 0, \"BatteryVolt\" numeric NOT NULL DEFAULT 12.6, \"EngineStatus\" text NULL DEFAULT 'Off', \"Address\" text NULL, \"IsInGeofence\" boolean NOT NULL DEFAULT true, \"RecordedAt\" timestamp NOT NULL DEFAULT now())",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCavityNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCavityName\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCavityDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"CavityVisitCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"ServiceCavities\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CavityNo\" text NOT NULL DEFAULT '', \"CavityNoUser\" text NULL, \"CavityName\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"CavityType\" text NOT NULL DEFAULT 'GeneralRepair', \"Status\" text NOT NULL DEFAULT 'Available', \"LiftType\" text NULL DEFAULT '2PostLift', \"MaxPayloadKg\" numeric NOT NULL DEFAULT 4000, \"CurrentVin\" text NULL, \"CurrentModel\" text NULL, \"CurrentPlateNo\" text NULL, \"CurrentRoNo\" text NULL, \"CurrentAppNo\" text NULL, \"CurrentTechnician\" text NULL, \"CurrentAdvisor\" text NULL, \"CurrentWorkItem\" text NULL, \"OccupiedAt\" timestamp NULL, \"EstimatedReleaseAt\" timestamp NULL, \"StartUseDate\" timestamp NULL, \"FinishUseDate\" timestamp NULL, \"IsActive\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"UpdatedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CavityDispatchLogs\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CavityId\" bigint NOT NULL, \"CavityNo\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DispatchNo\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NULL, \"PlateNo\" text NULL, \"RoNo\" text NULL, \"AppNo\" text NULL, \"DispatchType\" text NOT NULL DEFAULT 'CheckIn', \"FromCavityNo\" text NULL, \"ToCavityNo\" text NULL, \"Technician\" text NULL, \"ServiceAdvisor\" text NULL, \"WorkDescription\" text NULL, \"CheckInTime\" timestamp NOT NULL DEFAULT now(), \"CheckOutTime\" timestamp NULL, \"DurationMinutes\" integer NULL, \"Status\" text NOT NULL DEFAULT 'InCavity', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsStoragePaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"StoragePaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastStoragePaymentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastStoragePaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"StoragePaymentCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"StoragePayments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentStorageNo\" text NOT NULL DEFAULT '', \"PaymentStorageNoUser\" text NULL, \"PmtMonth\" text NOT NULL DEFAULT '', \"StorageCode\" text NOT NULL DEFAULT 'TCV_YARD', \"StorageProvider\" text NULL DEFAULT 'TCMS - Thanh Cong Motor Services', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalStorageDays\" integer NOT NULL DEFAULT 0, \"TotalBeforeVAT\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"TCMSSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"TCMSSignDate\" timestamp NULL, \"TCMSSignBy\" text NULL, \"HTVSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"HTVSignDate\" timestamp NULL, \"HTVSignBy\" text NULL, \"BankRefNo\" text NULL, \"PaymentDate\" timestamp NULL, \"FilePath\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"Approved1By\" text NULL, \"Approved1At\" timestamp NULL, \"Approved2By\" text NULL, \"Approved2At\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StoragePaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"StoragePaymentId\" bigint NOT NULL, \"PaymentStorageNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"StorageCodeInit\" text NULL, \"StoreDate\" timestamp NULL, \"DeliveryOutDate\" timestamp NULL, \"DealerCode\" text NULL, \"InCostStorageDate\" timestamp NOT NULL DEFAULT now(), \"OutCostStorageDate\" timestamp NOT NULL DEFAULT now(), \"StorageDays\" integer NOT NULL DEFAULT 1, \"DailyRate\" numeric NOT NULL DEFAULT 35000, \"CoverDailyRate\" numeric NOT NULL DEFAULT 0, \"StorageCost\" numeric NOT NULL DEFAULT 35000, \"CoverCost\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 35000, \"StorageLevel\" text NOT NULL DEFAULT 'Standard', \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsAvnInstalled\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"AvnDeviceCode\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"AvnSerialNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"MapCardSerialNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsAvnPaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"AvnPaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAvnPaymentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAvnPaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"AvnPaymentCount\" integer NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTestDriveNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTestDriveDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TestDriveCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"AvnPayments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentAVNNo\" text NOT NULL DEFAULT '', \"PaymentAVNNoUser\" text NULL, \"PmtMonth\" text NOT NULL DEFAULT '', \"SupplierCode\" text NOT NULL DEFAULT 'MOBIS', \"SupplierName\" text NULL DEFAULT 'Mobis Auto Parts Vietnam', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalBeforeVAT\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"SupplierSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"SupplierSignDate\" timestamp NULL, \"SupplierSignBy\" text NULL, \"HTVSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"HTVSignDate\" timestamp NULL, \"HTVSignBy\" text NULL, \"BankRefNo\" text NULL, \"PaymentDate\" timestamp NULL, \"FilePath\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"Approved1By\" text NULL, \"Approved1At\" timestamp NULL, \"Approved2By\" text NULL, \"Approved2At\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"AvnPaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"AvnPaymentId\" bigint NOT NULL, \"PaymentAVNNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"AvnDeviceCode\" text NOT NULL DEFAULT 'AVN-GEN5W-10INCH', \"AvnSerialNo\" text NOT NULL DEFAULT '', \"MapCardSerialNo\" text NULL, \"MapVersion\" text NULL DEFAULT 'VN-MAP-2026.Q1', \"DevicePrice\" numeric NOT NULL DEFAULT 7500000, \"MapPrice\" numeric NOT NULL DEFAULT 1200000, \"InstallationFee\" numeric NOT NULL DEFAULT 300000, \"AccessoryCost\" numeric NOT NULL DEFAULT 200000, \"TotalAmount\" numeric NOT NULL DEFAULT 9200000, \"InStorageDate\" timestamp NULL, \"AvnInstallDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CustomerTestDrives\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DriveTestCode\" text NOT NULL DEFAULT '', \"DriveTestCodeUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"DrvTestPlateNo\" text NULL, \"FullName\" text NOT NULL DEFAULT '', \"PhoneNo\" text NOT NULL DEFAULT '', \"Email\" text NULL, \"CusAddress\" text NULL, \"Gender\" text NOT NULL DEFAULT 'Nam', \"BirthYear\" integer NULL, \"RangeAgeCode\" text NULL DEFAULT '26-35', \"DriverLicenseNo\" text NOT NULL DEFAULT '', \"LicenseClass\" text NULL DEFAULT 'B2', \"DriveTestType\" text NOT NULL DEFAULT 'Showroom', \"EventName\" text NULL, \"RoutePath\" text NULL, \"DriveDTime\" timestamp NOT NULL DEFAULT now(), \"DurationMinutes\" integer NOT NULL DEFAULT 30, \"OdoStart\" integer NOT NULL DEFAULT 0, \"OdoEnd\" integer NULL, \"DistanceKm\" integer NOT NULL DEFAULT 0, \"SalesManCode\" text NULL, \"SalesManName\" text NULL, \"Instructor\" text NULL, \"ScoreEngine\" numeric NULL DEFAULT 5.0, \"ScoreHandling\" numeric NULL DEFAULT 5.0, \"ScoreNVH\" numeric NULL DEFAULT 5.0, \"ScoreDesign\" numeric NULL DEFAULT 5.0, \"ScoreFeatures\" numeric NULL DEFAULT 5.0, \"ScoreOverall\" numeric NULL DEFAULT 5.0, \"CustomerFeedback\" text NULL, \"PurchaseIntent\" text NOT NULL DEFAULT 'High', \"CompetitorModel\" text NULL, \"ExpectedDealDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"StartedBy\" text NULL, \"StartedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTranspPlanNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTranspPlanDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TranspPlanCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"TransportPlans\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PlanNo\" text NOT NULL DEFAULT '', \"PlanNoUser\" text NULL, \"PlanMonth\" text NOT NULL DEFAULT '', \"PlanDate\" timestamp NOT NULL DEFAULT now(), \"StorageCode\" text NOT NULL DEFAULT 'PLANT-HTMV1', \"StorageName\" text NULL DEFAULT 'Kho Tổng Nhà máy HTMV Ninh Bình 1', \"TPType\" text NOT NULL DEFAULT 'Road', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalRealVinCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"ExecutedBy\" text NULL, \"ExecutedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportPlanLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportPlanId\" bigint NOT NULL, \"PlanNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"VINPlan\" text NOT NULL DEFAULT '', \"Vin\" text NULL, \"FlagRealVin\" boolean NOT NULL DEFAULT false, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"SpecDescription\" text NULL, \"ColorCode\" text NOT NULL DEFAULT 'NWAC', \"ColorName\" text NULL DEFAULT 'Trắng ngọc trai', \"StorageCode\" text NOT NULL DEFAULT 'PLANT-HTMV1', \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"FProvinceCode\" text NOT NULL DEFAULT 'NB', \"FProvinceName\" text NULL DEFAULT 'Ninh Bình', \"FDistrictCode\" text NOT NULL DEFAULT 'GV', \"FDistrictName\" text NULL DEFAULT 'Gia Viễn', \"TProvinceCode\" text NOT NULL DEFAULT 'HN', \"TProvinceName\" text NULL DEFAULT 'Hà Nội', \"TDistrictCode\" text NOT NULL DEFAULT 'CG', \"TDistrictName\" text NULL DEFAULT 'Cầu Giấy', \"TransporterCode\" text NOT NULL DEFAULT 'NYK', \"TransporterName\" text NULL DEFAULT 'Công ty TNHH Vận tải Hàng hải NYK Việt Nam', \"TruckPlateNo\" text NULL, \"DriverName\" text NULL, \"DriverPhone\" text NULL, \"CQStartDate\" timestamp NULL, \"ExpectedDate\" timestamp NOT NULL DEFAULT now(), \"ActualDepartureDate\" timestamp NULL, \"ActualArrivalDate\" timestamp NULL, \"TPStatus\" text NOT NULL DEFAULT 'Pending', \"TransporterStatus\" text NOT NULL DEFAULT 'Pending', \"TransporterAppDate\" timestamp NULL, \"TransporterAppBy\" text NULL, \"TransporterRejectReason\" text NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsGpsPaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsPaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsPaymentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastGpsPaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"GpsPaymentCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"GpsPayments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PaymentGPSNo\" text NOT NULL DEFAULT '', \"PaymentGPSNoUser\" text NULL, \"PmtMonth\" text NOT NULL DEFAULT '', \"SupplierCode\" text NOT NULL DEFAULT 'VELOCA', \"SupplierName\" text NULL DEFAULT 'Công ty Cổ phần Công nghệ Veloca', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalBeforeVAT\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"TCMSSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"TCMSSignDate\" timestamp NULL, \"TCMSSignBy\" text NULL, \"HTVSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"HTVSignDate\" timestamp NULL, \"HTVSignBy\" text NULL, \"BankRefNo\" text NULL, \"PaymentDate\" timestamp NULL, \"FilePath\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"Approved1By\" text NULL, \"Approved1At\" timestamp NULL, \"Approved2By\" text NULL, \"Approved2At\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"GpsPaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"GpsPaymentId\" bigint NOT NULL, \"PaymentGPSNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"GpsCode\" text NOT NULL DEFAULT '', \"SimCardNo\" text NULL, \"ImeiNo\" text NULL, \"CostGPSStartDate\" timestamp NOT NULL DEFAULT now(), \"CostGPSEndDate\" timestamp NOT NULL DEFAULT now(), \"PlanCostGPSDate\" integer NOT NULL DEFAULT 30, \"DeductDate\" integer NOT NULL DEFAULT 0, \"ActualCostGPSDate\" integer NOT NULL DEFAULT 30, \"DailyRate\" numeric NOT NULL DEFAULT 15000, \"SimDataFee\" numeric NOT NULL DEFAULT 50000, \"AmountGPS\" numeric NOT NULL DEFAULT 500000, \"ContractGPS\" text NULL, \"InStorageDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsTranspInsPaid\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TranspInsPaidAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTranspInsPaymentNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastTranspInsPaymentDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"TranspInsPaymentCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"TransportInsurancePayments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportInsNo\" text NOT NULL DEFAULT '', \"TransportInsNoUser\" text NULL, \"PmtMonth\" text NOT NULL DEFAULT '', \"TransporterCode\" text NOT NULL DEFAULT 'NYK', \"TransporterName\" text NULL DEFAULT 'Công ty TNHH Vận tải Hàng hải NYK Việt Nam', \"InsuranceCompanyCode\" text NOT NULL DEFAULT 'BAOVIET', \"InsuranceCompanyName\" text NULL DEFAULT 'Tổng Công ty Bảo hiểm Bảo Việt', \"TotalVehicleCount\" integer NOT NULL DEFAULT 0, \"TotalFreightAmount\" numeric NOT NULL DEFAULT 0, \"TotalDelayPenalty\" numeric NOT NULL DEFAULT 0, \"TotalInsuranceFee\" numeric NOT NULL DEFAULT 0, \"TotalBeforeVAT\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"TransporterSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"TransporterSignDate\" timestamp NULL, \"TransporterSignBy\" text NULL, \"HTVSignStatus\" text NOT NULL DEFAULT 'Unsigned', \"HTVSignDate\" timestamp NULL, \"HTVSignBy\" text NULL, \"BankRefNo\" text NULL, \"PaymentDate\" timestamp NULL, \"FilePath\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"Approved1By\" text NULL, \"Approved1At\" timestamp NULL, \"Approved2By\" text NULL, \"Approved2At\" timestamp NULL, \"SettledBy\" text NULL, \"SettledAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TransportInsurancePaymentLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TransportInsurancePaymentId\" bigint NOT NULL, \"TransportInsNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"FStorageCode\" text NULL DEFAULT 'PLANT-HTMV1', \"FProvinceName\" text NULL DEFAULT 'Ninh Bình', \"TStorageCode\" text NULL, \"TProvinceName\" text NULL DEFAULT 'Hà Nội', \"DealerCode\" text NULL, \"DlvStartDate\" timestamp NOT NULL DEFAULT now(), \"ExpectedDays\" integer NOT NULL DEFAULT 2, \"ExpectedDlvEndDate\" timestamp NOT NULL DEFAULT now(), \"DlvEndDate\" timestamp NOT NULL DEFAULT now(), \"DelayDate\" integer NOT NULL DEFAULT 0, \"FreightAmount\" numeric NOT NULL DEFAULT 2500000, \"PenaltyPerDay\" numeric NOT NULL DEFAULT 100000, \"DelayPenalty\" numeric NOT NULL DEFAULT 0, \"CarValue\" numeric NOT NULL DEFAULT 550000000, \"InsuranceRate\" numeric NOT NULL DEFAULT 0.05, \"InsuranceFee\" numeric NOT NULL DEFAULT 275000, \"TotalAmount\" numeric NOT NULL DEFAULT 2775000, \"DlvMnNo\" text NULL, \"TranspReqType\" text NOT NULL DEFAULT 'OEMToDealer', \"Status\" text NOT NULL DEFAULT 'Pending', \"StandardRemark\" text NULL, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastInventoryAuditDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"InventoryAlertStatus\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastThresholdNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"ThresholdAuditCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"DealerInventoryThresholds\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ThresholdNo\" text NOT NULL DEFAULT '', \"ThresholdNoUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"RegionCode\" text NULL DEFAULT 'MienBac', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"PeriodMonth\" integer NOT NULL DEFAULT 5, \"PeriodYear\" integer NOT NULL DEFAULT 2026, \"MinInvQty\" integer NOT NULL DEFAULT 5, \"TargetInvQty\" integer NOT NULL DEFAULT 10, \"MaxInvQty\" integer NOT NULL DEFAULT 25, \"WarningThresholdPercent\" numeric NOT NULL DEFAULT 20, \"DailySalesRate\" numeric NOT NULL DEFAULT 0.5, \"EffectiveFrom\" timestamp NULL, \"EffectiveTo\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SuspendedBy\" text NULL, \"SuspendedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"InventoryAuditRecords\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"AuditNo\" text NOT NULL DEFAULT '', \"ThresholdId\" bigint NULL, \"ThresholdNo\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"RegionCode\" text NULL, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"MinInvQty\" integer NOT NULL DEFAULT 5, \"TargetInvQty\" integer NOT NULL DEFAULT 10, \"MaxInvQty\" integer NOT NULL DEFAULT 25, \"InStockCount\" integer NOT NULL DEFAULT 0, \"AllocatedCount\" integer NOT NULL DEFAULT 0, \"InTransitCount\" integer NOT NULL DEFAULT 0, \"TotalOnHand\" integer NOT NULL DEFAULT 0, \"VarianceQty\" integer NOT NULL DEFAULT 0, \"StockFulfillmentRate\" numeric NOT NULL DEFAULT 0, \"DaysOfSupply\" numeric NOT NULL DEFAULT 0, \"HealthStatus\" text NOT NULL DEFAULT 'Optimal', \"RebalanceAction\" text NULL, \"RecommendedTransferDealer\" text NULL, \"RecommendedTransferQty\" integer NOT NULL DEFAULT 0, \"AuditDate\" timestamp NOT NULL DEFAULT now(), \"AuditedBy\" text NULL, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TrainingCourses\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TrainingCode\" text NOT NULL DEFAULT '', \"TrainingCodeUser\" text NULL, \"CourseName\" text NOT NULL DEFAULT '', \"TrainingType\" text NOT NULL DEFAULT 'SalesConsultant', \"Level\" text NOT NULL DEFAULT 'Intermediate', \"Format\" text NOT NULL DEFAULT 'OfflineInClass', \"TrainerName\" text NULL, \"Location\" text NULL, \"StartDate\" timestamp NOT NULL DEFAULT now(), \"EndDate\" timestamp NOT NULL DEFAULT now(), \"MaxCapacity\" integer NOT NULL DEFAULT 30, \"TotalEnrolled\" integer NOT NULL DEFAULT 0, \"TotalPassed\" integer NOT NULL DEFAULT 0, \"TotalFailed\" integer NOT NULL DEFAULT 0, \"PassingScore\" numeric NOT NULL DEFAULT 70.0, \"BudgetAmount\" numeric NOT NULL DEFAULT 0, \"ActualCost\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"TrainingEnrollments\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TrainingCourseId\" bigint NOT NULL, \"TrainingCode\" text NOT NULL DEFAULT '', \"EnrollmentNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"StaffCode\" text NOT NULL DEFAULT '', \"StaffName\" text NOT NULL DEFAULT '', \"StaffEmail\" text NULL, \"StaffPhone\" text NULL, \"Position\" text NOT NULL DEFAULT 'SalesConsultant', \"AttendancePercent\" numeric NOT NULL DEFAULT 100, \"TheoryScore\" numeric NOT NULL DEFAULT 0, \"PracticeScore\" numeric NOT NULL DEFAULT 0, \"FinalScore\" numeric NOT NULL DEFAULT 0, \"EvaluationGrade\" text NOT NULL DEFAULT 'Pending', \"ResultStatus\" text NOT NULL DEFAULT 'Registered', \"IsCertificateIssued\" boolean NOT NULL DEFAULT false, \"CertificateNo\" text NULL, \"CertificateIssueDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"StaffCertificates\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CertificateNo\" text NOT NULL DEFAULT '', \"CertificateNoUser\" text NULL, \"StaffCode\" text NOT NULL DEFAULT '', \"StaffName\" text NOT NULL DEFAULT '', \"StaffEmail\" text NULL, \"StaffPhone\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"Position\" text NOT NULL DEFAULT 'SalesConsultant', \"CertificateType\" text NOT NULL DEFAULT 'SalesConsultant', \"Level\" text NOT NULL DEFAULT 'Certified', \"IssueDate\" timestamp NOT NULL DEFAULT now(), \"ExpiryDate\" timestamp NOT NULL DEFAULT now(), \"IssuedBy\" text NOT NULL DEFAULT 'HTV Training Center', \"Status\" text NOT NULL DEFAULT 'Active', \"LinkedTrainingCode\" text NULL, \"LinkedEnrollmentNo\" text NULL, \"ScoreAchieved\" numeric NOT NULL DEFAULT 0, \"Grade\" text NULL, \"RevokeReason\" text NULL, \"RevokedBy\" text NULL, \"RevokedAt\" timestamp NULL, \"FileUrl\" text NULL, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"UpdatedAt\" timestamp NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastServicePackageNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastPackageCardNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"ActiveServicePackageCount\" integer NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"PackageUsageCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"ServicePackages\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"PackageNo\" text NOT NULL DEFAULT '', \"PackageNoUser\" text NULL, \"PackageName\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT 'ALL', \"DealerName\" text NULL DEFAULT 'Hyundai Toàn Quốc OEM', \"PackageType\" text NOT NULL DEFAULT 'PeriodicMaintenance', \"ApplicableModel\" text NOT NULL DEFAULT 'ALL', \"MilestoneKm\" integer NULL DEFAULT 5000, \"StandardTakingTimeMinutes\" integer NOT NULL DEFAULT 60, \"ValidityMonths\" integer NOT NULL DEFAULT 12, \"MaxUsageCount\" integer NOT NULL DEFAULT 1, \"IsPublic\" boolean NOT NULL DEFAULT true, \"IsUseBasePrice\" boolean NOT NULL DEFAULT true, \"TotalLaborAmount\" numeric NOT NULL DEFAULT 0, \"TotalPartAmount\" numeric NOT NULL DEFAULT 0, \"OriginalPrice\" numeric NOT NULL DEFAULT 0, \"DiscountPercent\" numeric NOT NULL DEFAULT 15, \"PackagePrice\" numeric NOT NULL DEFAULT 0, \"TotalSubscribedCount\" integer NOT NULL DEFAULT 0, \"TotalUsedCount\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Description\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"ArchivedBy\" text NULL, \"ArchivedAt\" timestamp NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServicePackageLaborLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServicePackageId\" bigint NOT NULL, \"PackageNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"ServiceItemCode\" text NOT NULL DEFAULT '', \"ServiceItemName\" text NOT NULL DEFAULT '', \"StandardHours\" numeric NOT NULL DEFAULT 0.5, \"LaborPrice\" numeric NOT NULL DEFAULT 350000, \"DiscountPercent\" numeric NOT NULL DEFAULT 0, \"LaborAmount\" numeric NOT NULL DEFAULT 175000, \"IsMandatory\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServicePackagePartLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ServicePackageId\" bigint NOT NULL, \"PackageNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"PartCode\" text NOT NULL DEFAULT '', \"PartName\" text NOT NULL DEFAULT '', \"Unit\" text NOT NULL DEFAULT 'Cái', \"Quantity\" numeric NOT NULL DEFAULT 1.0, \"UnitPrice\" numeric NOT NULL DEFAULT 0, \"DiscountPercent\" numeric NOT NULL DEFAULT 0, \"PartAmount\" numeric NOT NULL DEFAULT 0, \"IsMandatory\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServicePackageSubscriptions\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SubscriptionNo\" text NOT NULL DEFAULT '', \"SubscriptionNoUser\" text NULL, \"PackageCardNo\" text NOT NULL DEFAULT '', \"ServicePackageId\" bigint NOT NULL, \"PackageNo\" text NOT NULL DEFAULT '', \"PackageName\" text NOT NULL DEFAULT '', \"PackageType\" text NOT NULL DEFAULT 'PeriodicMaintenance', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"EngineNo\" text NULL, \"PlateNo\" text NULL, \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"CustomerEmail\" text NULL, \"DealerCode\" text NOT NULL DEFAULT 'DLR-HN01', \"DealerName\" text NULL DEFAULT 'Hyundai Hà Nội 01', \"SalesAdvisor\" text NULL, \"PurchaseDate\" timestamp NOT NULL DEFAULT now(), \"StartDate\" timestamp NOT NULL DEFAULT now(), \"ExpiryDate\" timestamp NOT NULL DEFAULT now(), \"TotalPackagePrice\" numeric NOT NULL DEFAULT 0, \"PaidAmount\" numeric NOT NULL DEFAULT 0, \"IsPaid\" boolean NOT NULL DEFAULT true, \"PaymentMethod\" text NOT NULL DEFAULT 'Cash', \"MaxUsageCount\" integer NOT NULL DEFAULT 1, \"UsedCount\" integer NOT NULL DEFAULT 0, \"RemainingCount\" integer NOT NULL DEFAULT 1, \"TotalSavedAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Active', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"ServicePackageUsages\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"UsageNo\" text NOT NULL DEFAULT '', \"SubscriptionId\" bigint NOT NULL, \"SubscriptionNo\" text NOT NULL DEFAULT '', \"PackageCardNo\" text NOT NULL DEFAULT '', \"PackageNo\" text NOT NULL DEFAULT '', \"PackageName\" text NOT NULL DEFAULT '', \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"PlateNo\" text NULL, \"DealerCode\" text NOT NULL DEFAULT 'DLR-HN01', \"DealerName\" text NULL DEFAULT 'Hyundai Hà Nội 01', \"UsageDate\" timestamp NOT NULL DEFAULT now(), \"OdoKm\" integer NOT NULL DEFAULT 5000, \"MilestoneUsed\" integer NULL DEFAULT 5000, \"RoNo\" text NULL, \"CavityNo\" text NULL, \"Technician\" text NULL, \"ServiceAdvisor\" text NULL, \"LaborSavedAmount\" numeric NOT NULL DEFAULT 0, \"PartSavedAmount\" numeric NOT NULL DEFAULT 0, \"TotalSavedAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Confirmed', \"CustomerRating\" numeric NULL DEFAULT 5.0, \"CustomerFeedback\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ConfirmedBy\" text NULL, \"ConfirmedAt\" timestamp NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAutoDoNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastAutoDoDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"AutoDoCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"DOAutoConditions\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"ConditionCode\" text NOT NULL DEFAULT '', \"ConditionNoUser\" text NULL, \"ConditionName\" text NOT NULL DEFAULT '', \"Description\" text NULL, \"PriorityRule\" text NOT NULL DEFAULT 'FIFO_StoreDate', \"EffectiveFrom\" timestamp NOT NULL DEFAULT now(), \"EffectiveTo\" timestamp NOT NULL DEFAULT now(), \"MinDepositPercent\" numeric NOT NULL DEFAULT 10, \"MinPaymentPercent\" numeric NOT NULL DEFAULT 80, \"RequireGuaranteeOrPaid\" boolean NOT NULL DEFAULT true, \"RequireQC\" boolean NOT NULL DEFAULT true, \"RequireCustomsClearance\" boolean NOT NULL DEFAULT false, \"RequireTaxPaid\" boolean NOT NULL DEFAULT false, \"RequirePdiPassed\" boolean NOT NULL DEFAULT true, \"RequireRedeemed\" boolean NOT NULL DEFAULT true, \"RequireGpsInstalled\" boolean NOT NULL DEFAULT false, \"MaxBatchVehicleQuota\" integer NOT NULL DEFAULT 100, \"TotalExecutedBatches\" integer NOT NULL DEFAULT 0, \"TotalAllocatedVehicles\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"SuspendedBy\" text NULL, \"SuspendedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DOAutoConditionLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DOAutoConditionId\" bigint NOT NULL, \"ConditionCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"ColorCode\" text NULL, \"MaxQuotaQty\" integer NOT NULL DEFAULT 50, \"PriorityRank\" integer NOT NULL DEFAULT 1, \"Status\" text NOT NULL DEFAULT 'Active', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"DOAutoConditionDealerLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"DOAutoConditionId\" bigint NOT NULL, \"ConditionCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"RegionCode\" text NULL DEFAULT 'MienBac', \"MaxDealerQuota\" integer NOT NULL DEFAULT 20, \"TierLevel\" text NOT NULL DEFAULT 'Tier1', \"Status\" text NOT NULL DEFAULT 'Active', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"AutoDeliveryOrderBatches\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"BatchNo\" text NOT NULL DEFAULT '', \"BatchNoUser\" text NULL, \"BatchDate\" timestamp NOT NULL DEFAULT now(), \"ConditionId\" bigint NULL, \"ConditionCode\" text NOT NULL DEFAULT '', \"ConditionName\" text NULL, \"StorageCode\" text NULL DEFAULT 'ALL', \"TotalScannedVehicles\" integer NOT NULL DEFAULT 0, \"TotalEligibleVehicles\" integer NOT NULL DEFAULT 0, \"TotalAllocatedVehicles\" integer NOT NULL DEFAULT 0, \"TotalSkippedVehicles\" integer NOT NULL DEFAULT 0, \"TotalGeneratedDOs\" integer NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"ExecutionMode\" text NOT NULL DEFAULT 'LiveExecution', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ExecutedBy\" text NULL, \"ExecutedAt\" timestamp NULL, \"ConfirmedBy\" text NULL, \"ConfirmedAt\" timestamp NULL, \"RollbackedBy\" text NULL, \"RollbackedAt\" timestamp NULL, \"RollbackReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"AutoDeliveryOrderBatchLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"AutoDeliveryOrderBatchId\" bigint NOT NULL, \"BatchNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"StorageCode\" text NULL, \"DealerCode\" text NULL, \"DealerName\" text NULL, \"SOCode\" text NULL, \"ContractNo\" text NULL, \"AllocatedDoNo\" text NULL, \"AllocationStatus\" text NOT NULL DEFAULT 'Allocated', \"EligibilityReason\" text NULL, \"IsQCPassed\" boolean NOT NULL DEFAULT true, \"IsCustomsCleared\" boolean NOT NULL DEFAULT true, \"IsTaxPaid\" boolean NOT NULL DEFAULT true, \"IsPdiPaid\" boolean NOT NULL DEFAULT true, \"IsRedeemed\" boolean NOT NULL DEFAULT true, \"IsGpsInstalled\" boolean NOT NULL DEFAULT true, \"IsGuaranteedOrPaid\" boolean NOT NULL DEFAULT true, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSsiNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSsiDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSsiScore\" numeric NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSsiIndex1000\" integer NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"SsiSurveyCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"SalesSatisfactionSurveys\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SurveyNo\" text NOT NULL DEFAULT '', \"SurveyNoUser\" text NULL, \"DealNo\" text NULL, \"Vin\" text NOT NULL DEFAULT '', \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"EngineNo\" text NULL, \"Color\" text NULL, \"PlateNo\" text NULL, \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"CustomerEmail\" text NULL, \"CustomerType\" text NOT NULL DEFAULT 'Individual', \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"SalesConsultantCode\" text NULL, \"SalesConsultantName\" text NULL, \"DeliveryDate\" timestamp NOT NULL DEFAULT now(), \"SurveyDate\" timestamp NOT NULL DEFAULT now(), \"ContactChannel\" text NOT NULL DEFAULT 'PhoneCall', \"CallAttempts\" integer NOT NULL DEFAULT 1, \"SurveyorStaff\" text NULL, \"ScoreSalesConsultant\" numeric NOT NULL DEFAULT 5.0, \"ScoreDealershipFacility\" numeric NOT NULL DEFAULT 5.0, \"ScoreDeliveryProcess\" numeric NOT NULL DEFAULT 5.0, \"ScorePaperworkFinance\" numeric NOT NULL DEFAULT 5.0, \"ScoreTimeliness\" numeric NOT NULL DEFAULT 5.0, \"ScoreOverall\" numeric NOT NULL DEFAULT 5.0, \"CalculatedSsiScore\" numeric NOT NULL DEFAULT 5.0, \"SsiIndex1000\" integer NOT NULL DEFAULT 1000, \"IsCleanCarDelivered\" boolean NOT NULL DEFAULT true, \"IsFeatureExplained\" boolean NOT NULL DEFAULT true, \"IsAdasExplained\" boolean NOT NULL DEFAULT true, \"IsAvnBluelinkSetup\" boolean NOT NULL DEFAULT true, \"IsOriginalDocsHandedOver\" boolean NOT NULL DEFAULT true, \"IsFollowUpCallPromised\" boolean NOT NULL DEFAULT true, \"NpsScore\" integer NOT NULL DEFAULT 10, \"NpsCategory\" text NOT NULL DEFAULT 'Promoter', \"CustomerFeedback\" text NULL, \"HasComplaint\" boolean NOT NULL DEFAULT false, \"ComplaintCategory\" text NULL, \"ComplaintDetails\" text NULL, \"RemedyAction\" text NULL, \"IsComplaintResolved\" boolean NOT NULL DEFAULT false, \"ResolvedBy\" text NULL, \"ResolvedAt\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"EscalatedBy\" text NULL, \"EscalatedAt\" timestamp NULL, \"EscalateReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"SalesSatisfactionSurveyQuestionLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SalesSatisfactionSurveyId\" bigint NOT NULL, \"SurveyNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"QuestionCode\" text NOT NULL DEFAULT '', \"QuestionCategory\" text NOT NULL DEFAULT 'SalesConsultant', \"QuestionText\" text NOT NULL DEFAULT '', \"Score\" numeric NOT NULL DEFAULT 5.0, \"AnswerText\" text NULL, \"Weight\" numeric NOT NULL DEFAULT 1.0, \"Remark\" text NULL)",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCustomerVisitNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastCustomerVisitDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"CustomerVisitCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"CustomerVisits\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"VisitCode\" text NOT NULL DEFAULT '', \"VisitCodeUser\" text NULL, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"VisitDate\" timestamp NOT NULL DEFAULT now(), \"CustomerName\" text NOT NULL DEFAULT '', \"CustomerPhone\" text NOT NULL DEFAULT '', \"CustomerEmail\" text NULL, \"CustomerAddress\" text NULL, \"Gender\" text NOT NULL DEFAULT 'Nam', \"RangeAgeCode\" text NOT NULL DEFAULT '26-35', \"CustomerType\" text NOT NULL DEFAULT 'Individual', \"InterestedModel\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"SpecDescription\" text NULL, \"ColorCode\" text NOT NULL DEFAULT 'NWAC', \"ColorName\" text NOT NULL DEFAULT 'Trắng ngọc trai', \"Vin\" text NULL, \"VisitPurpose\" text NOT NULL DEFAULT 'XemXeMoi', \"LeadSource\" text NOT NULL DEFAULT 'ShowroomWalkIn', \"SalesConsultantCode\" text NULL, \"SalesConsultantName\" text NULL, \"HasTradeIn\" boolean NOT NULL DEFAULT false, \"TradeInModel\" text NULL, \"TradeInYear\" integer NULL, \"TradeInEstimatedPrice\" numeric NOT NULL DEFAULT 0, \"PaymentMethodExpected\" text NOT NULL DEFAULT 'Cash', \"LoanPercentExpected\" numeric NOT NULL DEFAULT 0, \"EstimatedPurchaseTime\" text NOT NULL DEFAULT 'TrongThang', \"PurchaseProbability\" text NOT NULL DEFAULT 'High', \"BudgetAmount\" numeric NOT NULL DEFAULT 0, \"CompetitorModel\" text NULL, \"IsTestDriveTaken\" boolean NOT NULL DEFAULT false, \"LinkedDriveTestCode\" text NULL, \"LinkedDealNo\" text NULL, \"NextFollowUpDate\" timestamp NULL, \"FollowUpAction\" text NULL DEFAULT 'CallBack', \"CustomerFeedback\" text NULL, \"Status\" text NOT NULL DEFAULT 'CheckedIn', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"CompletedBy\" text NULL, \"CompletedAt\" timestamp NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"CustomerVisitActionLogs\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"CustomerVisitId\" bigint NOT NULL, \"VisitCode\" text NOT NULL DEFAULT '', \"ActionNo\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"ActionType\" text NOT NULL DEFAULT 'ShowroomGreeting', \"ActionDate\" timestamp NOT NULL DEFAULT now(), \"SalesConsultant\" text NULL, \"DiscussionSummary\" text NOT NULL DEFAULT '', \"CustomerResponse\" text NULL, \"NextActionPlan\" text NULL, \"NextActionDate\" timestamp NULL, \"Status\" text NOT NULL DEFAULT 'Completed', \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"IsMktFeeSupported\" boolean NOT NULL DEFAULT false",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"MktFeeSupportedAmount\" numeric NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastMktFeeNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastMktFeeDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"MktFeeCount\" integer NOT NULL DEFAULT 0",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSalesKpiNo\" text NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"LastSalesKpiDate\" timestamp NULL",
            "ALTER TABLE public.\"Vehicles\" ADD COLUMN IF NOT EXISTS \"SalesKpiCount\" integer NOT NULL DEFAULT 0",
            "CREATE TABLE IF NOT EXISTS public.\"MarketingActivityTypes\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MKTActivityTypeCode\" text NOT NULL DEFAULT '', \"MKTActivityTypeName\" text NOT NULL DEFAULT '', \"FlagActive\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"MarketingActivities\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MKTActivityCode\" text NOT NULL DEFAULT '', \"MKTActivityName\" text NOT NULL DEFAULT '', \"MKTActivityTypeCode\" text NOT NULL DEFAULT 'DIGITAL', \"DefaultHTCLimitPrice\" numeric NOT NULL DEFAULT 0, \"FlagDesignImage\" boolean NOT NULL DEFAULT true, \"FlagActualImage\" boolean NOT NULL DEFAULT true, \"FlagContract\" boolean NOT NULL DEFAULT true, \"FlagInvoice\" boolean NOT NULL DEFAULT true, \"FlagActive\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"MarketingFeeSettlements\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MKTFeeCode\" text NOT NULL DEFAULT '', \"MKTFeeCodeUser\" text NULL, \"MKTFeeName\" text NOT NULL DEFAULT '', \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"CampaignMonth\" text NOT NULL DEFAULT '', \"DateStart\" timestamp NOT NULL DEFAULT now(), \"DateEnd\" timestamp NOT NULL DEFAULT now(), \"TotalActivityCount\" integer NOT NULL DEFAULT 0, \"TotalAmountDealer\" numeric NOT NULL DEFAULT 0, \"TotalAmountApproved\" numeric NOT NULL DEFAULT 0, \"VatRate\" numeric NOT NULL DEFAULT 10, \"TotalVatAmount\" numeric NOT NULL DEFAULT 0, \"TotalAmountAfterVAT\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"BankRefNo\" text NULL, \"SettledDate\" timestamp NULL, \"SettledBy\" text NULL, \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"MarketingFeeDetails\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MarketingFeeSettlementId\" bigint NOT NULL, \"MKTFeeCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"MKTActivityCode\" text NOT NULL DEFAULT '', \"MKTActivityName\" text NOT NULL DEFAULT '', \"MKTActivityTypeCode\" text NOT NULL DEFAULT 'DIGITAL', \"Vin\" text NULL, \"Model\" text NULL, \"SpecCode\" text NULL, \"Qty\" numeric NOT NULL DEFAULT 1, \"Price\" numeric NOT NULL DEFAULT 0, \"TotalAmountDealer\" numeric NOT NULL DEFAULT 0, \"HTCLimitPrice\" numeric NOT NULL DEFAULT 0, \"ApprovedQty\" numeric NOT NULL DEFAULT 0, \"ApprovedAmount\" numeric NOT NULL DEFAULT 0, \"FlagDesignImage\" boolean NOT NULL DEFAULT true, \"FlagActualImage\" boolean NOT NULL DEFAULT true, \"FlagContract\" boolean NOT NULL DEFAULT true, \"FlagInvoice\" boolean NOT NULL DEFAULT true, \"HasDesignImage\" boolean NOT NULL DEFAULT false, \"HasActualImage\" boolean NOT NULL DEFAULT false, \"HasContract\" boolean NOT NULL DEFAULT false, \"HasInvoice\" boolean NOT NULL DEFAULT false, \"Status\" text NOT NULL DEFAULT 'Pending', \"RejectReason\" text NULL, \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"MarketingFeeDetailAttaches\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"MarketingFeeDetailId\" bigint NOT NULL, \"MKTFeeCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"AttachCode\" text NOT NULL DEFAULT '', \"FileType\" text NOT NULL DEFAULT 'ActualImage', \"FileName\" text NOT NULL DEFAULT '', \"FilePath\" text NULL, \"FileSizeKb\" bigint NOT NULL DEFAULT 1024, \"Status\" text NOT NULL DEFAULT 'Approved', \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"Remark\" text NULL, \"UploadedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"SalesKpiIndicators\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"KPICode\" text NOT NULL DEFAULT '', \"KPIName\" text NOT NULL DEFAULT '', \"KPICategory\" text NOT NULL DEFAULT 'SalesVolume', \"Unit\" text NOT NULL DEFAULT 'Xe', \"Weight\" numeric NOT NULL DEFAULT 10.0, \"TargetDefault\" numeric NOT NULL DEFAULT 10.0, \"FlagActive\" boolean NOT NULL DEFAULT true, \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())",
            "CREATE TABLE IF NOT EXISTS public.\"SalesTargetKpis\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"TargetCode\" text NOT NULL DEFAULT '', \"TargetCodeUser\" text NULL, \"PeriodMonth\" text NOT NULL DEFAULT '', \"PeriodQuarter\" text NOT NULL DEFAULT 'Q1', \"PeriodYear\" integer NOT NULL DEFAULT 2026, \"DealerCode\" text NOT NULL DEFAULT '', \"DealerName\" text NULL, \"UserCode\" text NOT NULL DEFAULT 'ALL', \"UserName\" text NULL DEFAULT 'Toàn Đại Lý', \"Position\" text NOT NULL DEFAULT 'SalesConsultant', \"TargetCarCount\" integer NOT NULL DEFAULT 0, \"ActualCarCount\" integer NOT NULL DEFAULT 0, \"CarCompletionRate\" numeric NOT NULL DEFAULT 0, \"TargetRevenue\" numeric NOT NULL DEFAULT 0, \"ActualRevenue\" numeric NOT NULL DEFAULT 0, \"RevenueCompletionRate\" numeric NOT NULL DEFAULT 0, \"TargetTestDriveCount\" integer NOT NULL DEFAULT 0, \"ActualTestDriveCount\" integer NOT NULL DEFAULT 0, \"TargetContractCount\" integer NOT NULL DEFAULT 0, \"ActualContractCount\" integer NOT NULL DEFAULT 0, \"TargetInsuranceCount\" integer NOT NULL DEFAULT 0, \"ActualInsuranceCount\" integer NOT NULL DEFAULT 0, \"TargetAccessoriesRevenue\" numeric NOT NULL DEFAULT 0, \"ActualAccessoriesRevenue\" numeric NOT NULL DEFAULT 0, \"OverallScore\" numeric NOT NULL DEFAULT 0, \"KpiGrade\" text NOT NULL DEFAULT 'Pending', \"BonusRate\" numeric NOT NULL DEFAULT 0, \"BonusAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Draft', \"Remark\" text NULL, \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now(), \"ApprovedBy\" text NULL, \"ApprovedAt\" timestamp NULL, \"EvaluatedBy\" text NULL, \"EvaluatedAt\" timestamp NULL, \"RejectedBy\" text NULL, \"RejectedAt\" timestamp NULL, \"RejectReason\" text NULL, \"CancelledBy\" text NULL, \"CancelledAt\" timestamp NULL, \"CancelReason\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"SalesTargetKpiLines\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SalesTargetKpiId\" bigint NOT NULL, \"TargetCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"Model\" text NOT NULL DEFAULT '', \"SpecCode\" text NULL, \"TargetQty\" integer NOT NULL DEFAULT 1, \"ActualQty\" integer NOT NULL DEFAULT 0, \"CompletionRate\" numeric NOT NULL DEFAULT 0, \"TargetRevenue\" numeric NOT NULL DEFAULT 0, \"ActualRevenue\" numeric NOT NULL DEFAULT 0, \"CommissionPerCar\" numeric NOT NULL DEFAULT 3000000, \"BonusAmount\" numeric NOT NULL DEFAULT 0, \"Status\" text NOT NULL DEFAULT 'Pending', \"Remark\" text NULL)",
            "CREATE TABLE IF NOT EXISTS public.\"SalesKpiDailyLogs\" (\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, \"OrgId\" uuid NOT NULL, \"SalesTargetKpiId\" bigint NOT NULL, \"TargetCode\" text NOT NULL DEFAULT '', \"LineIndex\" integer NOT NULL DEFAULT 1, \"LogDate\" timestamp NOT NULL DEFAULT now(), \"KPICode\" text NOT NULL DEFAULT 'KPI_RETAIL_DELIVERY', \"KPIName\" text NOT NULL DEFAULT 'Bàn giao xe bán lẻ cho khách', \"TargetDailyQty\" numeric NOT NULL DEFAULT 1, \"ActualDailyQty\" numeric NOT NULL DEFAULT 1, \"LinkedVin\" text NULL, \"LinkedRefNo\" text NULL, \"Notes\" text NOT NULL DEFAULT '', \"CreatedBy\" text NULL, \"CreatedAt\" timestamp NOT NULL DEFAULT now())"
        };
        foreach (var s in stmts) try { await db.Database.ExecuteSqlRawAsync(s); } catch { }
    }
}
