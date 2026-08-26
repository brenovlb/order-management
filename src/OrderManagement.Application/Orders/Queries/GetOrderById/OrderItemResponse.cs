namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public record OrderItemResponse(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount
);