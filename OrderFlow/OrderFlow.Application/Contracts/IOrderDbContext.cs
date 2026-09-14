using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Contracts;

public interface IOrderDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderDashboardView> OrderDashboardViews { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
