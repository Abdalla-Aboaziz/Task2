using MediatR;

namespace OrderFlow.Application.Features.Orders.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
