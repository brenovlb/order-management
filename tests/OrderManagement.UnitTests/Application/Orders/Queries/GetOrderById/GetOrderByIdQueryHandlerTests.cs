using Moq;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Entities;

namespace OrderManagement.UnitTests.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        OrderItem item =
            new OrderItem(
                "Notebook",
                2,
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

        GetOrderByIdQueryHandler handler =
            new GetOrderByIdQueryHandler(
                repositoryMock.Object);

        GetOrderByIdQuery query =
            new GetOrderByIdQuery(order.Id);

        OrderResponse? result =
            await handler.Handle(
                query,
                CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(7000, result.TotalAmount);

        Assert.Single(result.Items);

        repositoryMock.Verify(
            repository => repository.GetByIdAsync(
                order.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        GetOrderByIdQueryHandler handler =
            new GetOrderByIdQueryHandler(
                repositoryMock.Object);

        GetOrderByIdQuery query =
            new GetOrderByIdQuery(Guid.NewGuid());

        OrderResponse? result =
            await handler.Handle(
                query,
                CancellationToken.None);

        Assert.Null(result);
    }
}