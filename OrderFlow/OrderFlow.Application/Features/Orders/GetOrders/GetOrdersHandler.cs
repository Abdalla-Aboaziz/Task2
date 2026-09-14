using MediatR;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Contracts;
using OrderFlow.Application.Features.Orders.GetOrderById;

namespace OrderFlow.Application.Features.Orders.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IOrderDbContext _context;

    public GetOrdersHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders.Adapt<List<OrderDto>>();
    }
}
