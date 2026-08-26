using Moq;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCancelOrder_WhenOrderIsPending()
    {
        OrderItem item =
            new OrderItem(
                "Notebook",
                1,
                3500);

        Order order =
            new Order(
                Guid.NewGuid(),
                new List<OrderItem>
                {
                    item
                });

        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        CancelOrderCommandHandler handler =
            new CancelOrderCommandHandler(
                repositoryMock.Object);

        CancelOrderCommand command =
            new CancelOrderCommand(order.Id);

        await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal(
            OrderStatus.Cancelled,
            order.Status);

        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                order,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
    {
        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        CancelOrderCommandHandler handler =
            new CancelOrderCommandHandler(
                repositoryMock.Object);

        CancelOrderCommand command =
            new CancelOrderCommand(Guid.NewGuid());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await handler.Handle(
                    command,
                    CancellationToken.None);
            });

        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}