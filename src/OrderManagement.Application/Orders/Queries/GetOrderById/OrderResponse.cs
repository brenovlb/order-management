namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResponse> Items
);