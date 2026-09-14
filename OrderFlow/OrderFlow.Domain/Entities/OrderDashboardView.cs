using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class OrderDashboardView
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime RefreshedAt { get; set; }
}
