using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Application.Contracts;

namespace OrderFlow.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderDbContext _context;

    public CreateOrderHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            CreatedAt = DateTime.UtcNow,
            Items = request.Items.Select(i => new OrderItem
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
