using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public List<OrderItem> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.TotalPrice);
}
