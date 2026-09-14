using Mapster;
using OrderFlow.Domain.Entities;
using OrderFlow.Application.Features.Orders.GetOrderById;
using OrderFlow.Application.Features.Orders.GetDashboardOrders;

namespace OrderFlow.Application.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.Total, src => src.Total)
            .Map(dest => dest.Items, src => src.Items);

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice);

        config.NewConfig<OrderDashboardView, DashboardOrderDto>()
            .Map(dest => dest.OrderId, src => src.OrderId);
    }
}
