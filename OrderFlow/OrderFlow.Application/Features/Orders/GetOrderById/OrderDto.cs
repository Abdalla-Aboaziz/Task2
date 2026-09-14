using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Features.Orders.GetOrderById;

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);

public record OrderDto(
    int Id,
    string CustomerName,
    DateTime CreatedAt,
    OrderStatus Status,
    decimal Total,
    List<OrderItemDto> Items);
