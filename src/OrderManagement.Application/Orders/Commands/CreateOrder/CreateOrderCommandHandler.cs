using MediatR;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        List<OrderItem> items = new List<OrderItem>();

        foreach (CreateOrderItemCommand item in request.Items)
        {
            OrderItem orderItem = new OrderItem(
                item.ProductName,
                item.Quantity,
                item.UnitPrice);

            items.Add(orderItem);
        }

        Order order = new Order(
            request.CustomerId,
            items);

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        return order.Id;
    }
}