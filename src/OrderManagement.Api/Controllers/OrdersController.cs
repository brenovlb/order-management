using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Orders.Queries.GetOrders;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        Guid orderId =
            await _sender.Send(
                command,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderId },
            new { id = orderId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        GetOrdersQuery query =
            new GetOrdersQuery(
                page,
                pageSize);

        PagedResult<OrderListItemResponse> result =
            await _sender.Send(
                query,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        GetOrderByIdQuery query =
            new GetOrderByIdQuery(id);

        OrderResponse? result =
            await _sender.Send(
                query,
                cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        CancelOrderCommand command =
            new CancelOrderCommand(id);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }
}