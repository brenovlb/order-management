using MediatR;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order == null)
        {
            return null;
        }

        List<OrderItemResponse> items = new List<OrderItemResponse>();

        foreach (OrderItem item in order.Items)
        {
            OrderItemResponse itemResponse = new OrderItemResponse(
                item.Id,
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.TotalAmount());

            items.Add(itemResponse);
        }

        OrderResponse response = new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.CreatedAt,
            order.TotalAmount(),
            items);

        return response;
    }
}