using MediatR;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Contracts;

namespace OrderFlow.Application.Features.Orders.GetDashboardOrders;

public class GetDashboardOrdersHandler : IRequestHandler<GetDashboardOrdersQuery, List<DashboardOrderDto>>
{
    private readonly IOrderDbContext _context;

    public GetDashboardOrdersHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<List<DashboardOrderDto>> Handle(GetDashboardOrdersQuery request, CancellationToken cancellationToken)
    {
        var views = await _context.OrderDashboardViews
            .AsNoTracking()
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);

        return views.Adapt<List<DashboardOrderDto>>();
    }
}
