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
        };
        foreach (var s in stmts) try { await db.Database.ExecuteSqlRawAsync(s); } catch { }
    }
}
