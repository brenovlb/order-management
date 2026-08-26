using Moq;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.UnitTests.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedOrders()
    {
        Order firstOrder =
            new Order(
                Guid.NewGuid(),
                new List<OrderItem>
                {
                    new OrderItem(
                        "Notebook",
                        1,
                        3500)
                });

        Order secondOrder =
            new Order(
                Guid.NewGuid(),
                new List<OrderItem>
                {
                    new OrderItem(
                        "Mouse",
                        2,
                        150)
                });

        IReadOnlyCollection<Order> orders =
            new List<Order>
            {
                firstOrder,
                secondOrder
            };

        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetPagedAsync(
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        repositoryMock
            .Setup(repository =>
                repository.CountAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        GetOrdersQueryHandler handler =
            new GetOrdersQueryHandler(
                repositoryMock.Object);

        GetOrdersQuery query =
            new GetOrdersQuery(
                1,
                10);

        PagedResult<OrderListItemResponse> result =
            await handler.Handle(
                query,
                CancellationToken.None);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);

        repositoryMock.Verify(
            repository => repository.GetPagedAsync(
                1,
                10,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            repository => repository.CountAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenNoOrdersExist()
    {
        Mock<IOrderRepository> repositoryMock =
            new Mock<IOrderRepository>();

        repositoryMock
            .Setup(repository =>
                repository.GetPagedAsync(
                    1,
                    10,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new List<Order>());

        repositoryMock
            .Setup(repository =>
                repository.CountAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        GetOrdersQueryHandler handler =
            new GetOrdersQueryHandler(
                repositoryMock.Object);

        GetOrdersQuery query =
            new GetOrdersQuery(
                1,
                10);

        PagedResult<OrderListItemResponse> result =
            await handler.Handle(
                query,
                CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}