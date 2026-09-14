using MediatR;
using OrderFlow.Application.Features.Orders.GetOrderById;

namespace OrderFlow.Application.Features.Orders.GetOrders;

public record GetOrdersQuery : IRequest<List<OrderDto>>;
