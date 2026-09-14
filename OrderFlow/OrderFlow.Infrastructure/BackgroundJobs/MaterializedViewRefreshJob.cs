using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.BackgroundJobs;

public class MaterializedViewRefreshJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MaterializedViewRefreshJob> _logger;

    public MaterializedViewRefreshJob(IServiceScopeFactory scopeFactory, ILogger<MaterializedViewRefreshJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task RefreshAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        var orders = await db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync();

        var views = orders.Select(o => new OrderDashboardView
        {
            OrderId = o.Id,
            CustomerName = o.CustomerName,
            ItemCount = o.Items.Count,
            Total = o.Items.Sum(i => i.Quantity * i.UnitPrice),
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            RefreshedAt = DateTime.UtcNow
        }).ToList();

        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE OrderDashboardViews");

        db.OrderDashboardViews.AddRange(views);
        await db.SaveChangesAsync();

        _logger.LogInformation("Materialized view refreshed with {Count} records.", views.Count);
    }
}
