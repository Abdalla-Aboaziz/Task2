using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Features.Orders.GetDashboardOrders;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("orders")]
    [ProducesResponseType(typeof(List<DashboardOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardOrders(CancellationToken cancellationToken)
    {
        var dashboard = await _mediator.Send(new GetDashboardOrdersQuery(), cancellationToken);
        return Ok(dashboard);
    }
}
