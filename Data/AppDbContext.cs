using Microsoft.EntityFrameworkCore;
using MiniVehicle.Models;

namespace MiniVehicle.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<Org> Orgs => Set<Org>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();
    public DbSet<DeliveryOrderLine> DeliveryOrderLines => Set<DeliveryOrderLine>();
    public DbSet<VehicleEvent> Events => Set<VehicleEvent>();
    public DbSet<RecallCampaign> Recalls => Set<RecallCampaign>();
    public DbSet<VehicleRecall> VehicleRecalls => Set<VehicleRecall>();
    public DbSet<WarrantyClaim> Claims => Set<WarrantyClaim>();
    public DbSet<DocRequest> DocRequests => Set<DocRequest>();
    public DbSet<StockTransfer> Transfers => Set<StockTransfer>();
    public DbSet<DeliveryMinutes> DeliveryMinutes => Set<DeliveryMinutes>();
    public DbSet<CarRetrieve> CarRetrieves => Set<CarRetrieve>();
    public DbSet<CarRetrieveLine> CarRetrieveLines => Set<CarRetrieveLine>();
    public DbSet<TransportRequest> TransportRequests => Set<TransportRequest>();
    public DbSet<TransportRequestLine> TransportRequestLines => Set<TransportRequestLine>();
    public DbSet<StorageRearrange> StorageRearranges => Set<StorageRearrange>();
    public DbSet<StorageRearrangeLine> StorageRearrangeLines => Set<StorageRearrangeLine>();
    public DbSet<TestCarRequest> TestCars => Set<TestCarRequest>();
    public DbSet<TestCarLine> TestCarLines => Set<TestCarLine>();
    public DbSet<PdiRequest> PdiRequests => Set<PdiRequest>();
    public DbSet<PdiRequestLine> PdiRequestLines => Set<PdiRequestLine>();
    public DbSet<MortgageRequest> MortgageRequests => Set<MortgageRequest>();
    public DbSet<MortgageRequestLine> MortgageRequestLines => Set<MortgageRequestLine>();
    public DbSet<RedeemRequest> RedeemRequests => Set<RedeemRequest>();
    public DbSet<RedeemRequestLine> RedeemRequestLines => Set<RedeemRequestLine>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<DealerDeal> DealerDeals => Set<DealerDeal>();
    public DbSet<DealerDealLine> DealerDealLines => Set<DealerDealLine>();
    public DbSet<PaymentGuarantee> Guarantees => Set<PaymentGuarantee>();
    public DbSet<PaymentGuaranteeLine> GuaranteeLines => Set<PaymentGuaranteeLine>();
    public DbSet<DealerContract> DealerContracts => Set<DealerContract>();
    public DbSet<DealerContractLine> DealerContractLines => Set<DealerContractLine>();
    public DbSet<PaymentDiscount> PaymentDiscounts => Set<PaymentDiscount>();
    public DbSet<PaymentDiscountLine> PaymentDiscountLines => Set<PaymentDiscountLine>();
    public DbSet<InsuranceRequest> InsuranceRequests => Set<InsuranceRequest>();
    public DbSet<InsuranceRequestLine> InsuranceRequestLines => Set<InsuranceRequestLine>();
    public DbSet<TransportMinutes> TransportMinutes => Set<TransportMinutes>();
    public DbSet<TransportMinutesLine> TransportMinutesLines => Set<TransportMinutesLine>();
    public DbSet<DealerPayment> Payments => Set<DealerPayment>();
    public DbSet<DealerPaymentLine> PaymentLines => Set<DealerPaymentLine>();
    public DbSet<StorageMaintenance> StorageMaintenances => Set<StorageMaintenance>();
    public DbSet<StorageMaintenanceLine> StorageMaintenanceLines => Set<StorageMaintenanceLine>();
    public DbSet<PackingList> PackingLists => Set<PackingList>();
    public DbSet<PackingListLine> PackingListLines => Set<PackingListLine>();
    public DbSet<CustomsDeclaration> CustomsDeclarations => Set<CustomsDeclaration>();
    public DbSet<CustomsDeclarationLine> CustomsDeclarationLines => Set<CustomsDeclarationLine>();
    public DbSet<CarBoxRequest> CarBoxRequests => Set<CarBoxRequest>();
    public DbSet<CarBoxRequestLine> CarBoxRequestLines => Set<CarBoxRequestLine>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Org>().HasIndex(x => x.ApiKey).IsUnique();
        b.Entity<Vehicle>().HasIndex(x => new { x.OrgId, x.Vin }).IsUnique();
        b.Entity<Vehicle>().Property(x => x.Status).HasConversion<int>();
        b.Entity<DeliveryOrder>().HasIndex(x => new { x.OrgId, x.DoNo }).IsUnique();
        b.Entity<DeliveryMinutes>().HasIndex(x => new { x.OrgId, x.DlvMnNo }).IsUnique();
        b.Entity<CarRetrieve>().HasIndex(x => new { x.OrgId, x.RetrieveNo }).IsUnique();
        b.Entity<TransportRequest>().HasIndex(x => new { x.OrgId, x.TransportReqNo }).IsUnique();
        b.Entity<StorageRearrange>().HasIndex(x => new { x.OrgId, x.StorageRearrangeNo }).IsUnique();
        b.Entity<TestCarRequest>().HasIndex(x => new { x.OrgId, x.TestCarCode }).IsUnique();
        b.Entity<PdiRequest>().HasIndex(x => new { x.OrgId, x.PdiReqNo }).IsUnique();
        b.Entity<MortgageRequest>().HasIndex(x => new { x.OrgId, x.ReqMortgageNo }).IsUnique();
        b.Entity<RedeemRequest>().HasIndex(x => new { x.OrgId, x.RedeemReqNo }).IsUnique();
        b.Entity<SalesOrder>().HasIndex(x => new { x.OrgId, x.SOCode }).IsUnique();
        b.Entity<DealerDeal>().HasIndex(x => new { x.OrgId, x.DealNo }).IsUnique();
        b.Entity<PaymentGuarantee>().HasIndex(x => new { x.OrgId, x.GuaranteeNo }).IsUnique();
        b.Entity<DealerContract>().HasIndex(x => new { x.OrgId, x.ContractNo }).IsUnique();
        b.Entity<PaymentDiscount>().HasIndex(x => new { x.OrgId, x.PaymentDiscountNo }).IsUnique();
        b.Entity<InsuranceRequest>().HasIndex(x => new { x.OrgId, x.InsReqNo }).IsUnique();
        b.Entity<TransportMinutes>().HasIndex(x => new { x.OrgId, x.TransportMinutesNo }).IsUnique();
        b.Entity<DealerPayment>().HasIndex(x => new { x.OrgId, x.PaymentNo }).IsUnique();
        b.Entity<StorageMaintenance>().HasIndex(x => new { x.OrgId, x.MtnNo }).IsUnique();
        b.Entity<PackingList>().HasIndex(x => new { x.OrgId, x.PackingListNo }).IsUnique();
        b.Entity<CustomsDeclaration>().HasIndex(x => new { x.OrgId, x.DeclarationNo }).IsUnique();
        b.Entity<CarBoxRequest>().HasIndex(x => new { x.OrgId, x.CBReqNo }).IsUnique();
    }
}
