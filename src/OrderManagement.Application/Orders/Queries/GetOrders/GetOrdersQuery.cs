using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(
    int Page,
    int PageSize
) : IRequest<PagedResult<OrderListItemResponse>>;