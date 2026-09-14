using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Application.Features.Orders.GetOrderById;
using OrderFlow.Application.Features.Orders.GetOrders;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateOrderCommand> _validator;

    public OrdersController(IMediator mediator, IValidator<CreateOrderCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
            return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));

        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id }, new { id });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(int id, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrdersQuery(), cancellationToken);
        return Ok(orders);
    }
}
