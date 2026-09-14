using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Features.Orders.CreateOrder;

public record CreateOrderItemDto(string ProductName, int Quantity, decimal UnitPrice);

public record CreateOrderCommand(string CustomerName, List<CreateOrderItemDto> Items)
    : MediatR.IRequest<int>;
