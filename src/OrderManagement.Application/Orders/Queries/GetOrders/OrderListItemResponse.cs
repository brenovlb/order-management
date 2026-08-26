namespace OrderManagement.Application.Orders.Queries.GetOrders;

public record OrderListItemResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount
);