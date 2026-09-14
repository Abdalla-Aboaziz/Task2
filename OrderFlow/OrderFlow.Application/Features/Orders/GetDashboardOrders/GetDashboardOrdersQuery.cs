using MediatR;

namespace OrderFlow.Application.Features.Orders.GetDashboardOrders;

public record GetDashboardOrdersQuery : IRequest<List<DashboardOrderDto>>;
