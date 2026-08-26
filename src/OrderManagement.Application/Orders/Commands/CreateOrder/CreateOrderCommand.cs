using MediatR;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemCommand> Items
) : IRequest<Guid>;