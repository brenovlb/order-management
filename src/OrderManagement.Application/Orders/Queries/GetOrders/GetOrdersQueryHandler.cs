using MediatR;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderListItemResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderListItemResponse>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Order> orders =
            await _orderRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

        int totalCount =
            await _orderRepository.CountAsync(cancellationToken);

        List<OrderListItemResponse> items =
            new List<OrderListItemResponse>();

        foreach (Order order in orders)
        {
            OrderListItemResponse item =
                new OrderListItemResponse(
                    order.Id,
                    order.CustomerId,
                    order.Status.ToString(),
                    order.CreatedAt,
                    order.TotalAmount());

            items.Add(item);
        }

        return new PagedResult<OrderListItemResponse>(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }
}