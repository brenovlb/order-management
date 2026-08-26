namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderItemCommand(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);