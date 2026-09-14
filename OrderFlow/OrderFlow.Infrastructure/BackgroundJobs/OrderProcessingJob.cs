using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.BackgroundJobs;

public class OrderProcessingJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HybridCache _cache;
    private readonly ILogger<OrderProcessingJob> _logger;

    public OrderProcessingJob(IServiceScopeFactory scopeFactory, HybridCache cache, ILogger<OrderProcessingJob> logger)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task ProcessPendingOrdersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        var pendingOrders = await db.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync();

        if (!pendingOrders.Any())
        {
            _logger.LogInformation("No pending orders to process.");
            return;
        }

        foreach (var order in pendingOrders)
        {
            order.Status = OrderStatus.Completed;
            await _cache.RemoveAsync($"order:{order.Id}");
            _logger.LogInformation("Order {OrderId} marked as Completed.", order.Id);
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("Processed {Count} pending orders.", pendingOrders.Count);
    }
}
