using MediatR;
using OrderManagement.Application.Abstractions.Persistence;

namespace OrderManagement.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler
    : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        order.Cancel();

        await _orderRepository.UpdateAsync(
            order,
            cancellationToken);
    }
}