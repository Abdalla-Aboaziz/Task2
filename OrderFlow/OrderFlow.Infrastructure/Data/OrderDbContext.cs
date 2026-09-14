using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Contracts;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Data;

public class OrderDbContext : DbContext, IOrderDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderDashboardView> OrderDashboardViews => Set<OrderDashboardView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            b.Property(o => o.Status).HasConversion<string>();
            b.Ignore(o => o.Total);
            b.HasMany(o => o.Items)
             .WithOne(i => i.Order)
             .HasForeignKey(i => i.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            b.Ignore(i => i.TotalPrice);
        });

        modelBuilder.Entity<OrderDashboardView>(b =>
        {
            b.HasKey(v => v.Id);
            b.ToTable("OrderDashboardViews");
            b.Property(v => v.Status).HasConversion<string>();
            b.Property(v => v.Total).HasColumnType("decimal(18,2)");
        });
    }
}
