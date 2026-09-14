using MediatR;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using OrderFlow.Application.Contracts;

namespace OrderFlow.Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderDbContext _context;
    private readonly HybridCache _cache;

    public GetOrderByIdHandler(IOrderDbContext context, HybridCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"order:{request.Id}",
            async ct =>
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

                return order?.Adapt<OrderDto>();
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            },
            cancellationToken: cancellationToken);
    }
}
