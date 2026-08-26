using Moq;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;

namespace OrderManagement.UnitTests.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
    {
        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        CreateOrderCommandHandler handler =
            new CreateOrderCommandHandler(repositoryMock.Object);

        CreateOrderCommand command =
            new CreateOrderCommand(
                Guid.NewGuid(),
                new List<CreateOrderItemCommand>
                {
                    new CreateOrderItemCommand(
                        "Notebook",
                        2,
                        3500)
                });

        Guid orderId =
            await handler.Handle(
                command,
                CancellationToken.None);

        Assert.NotEqual(Guid.Empty, orderId);

        repositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Order>(order =>
                    order.Id == orderId &&
                    order.Items.Count == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}