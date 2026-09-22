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
    }
}
