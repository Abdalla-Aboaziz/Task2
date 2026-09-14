using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Features.Orders.GetDashboardOrders;

public record DashboardOrderDto(
    int OrderId,
    string CustomerName,
    int ItemCount,
    decimal Total,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime RefreshedAt);
