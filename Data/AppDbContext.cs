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

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Org>().HasIndex(x => x.ApiKey).IsUnique();
        b.Entity<Vehicle>().HasIndex(x => new { x.OrgId, x.Vin }).IsUnique();
        b.Entity<Vehicle>().Property(x => x.Status).HasConversion<int>();
        b.Entity<DeliveryOrder>().HasIndex(x => new { x.OrgId, x.DoNo }).IsUnique();
    }
}
