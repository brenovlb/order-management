using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.UnitTests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreatePendingOrder()
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

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Single(order.Items);
    }

    [Fact]
    public void TotalAmount_ShouldCalculateSumOfItems()
    {
        OrderItem notebook =
            new OrderItem(
                "Notebook",
                2,
                3500);

        OrderItem mouse =
            new OrderItem(
                "Mouse",
                1,
                150);

        Order order =
            new Order(
                Guid.NewGuid(),
                new List<OrderItem>
                {
                    notebook,
                    mouse
                });

        decimal totalAmount =
            order.TotalAmount();

        Assert.Equal(7150, totalAmount);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled_WhenOrderIsPending()
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

        order.Cancel();

        Assert.Equal(
            OrderStatus.Cancelled,
            order.Status);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenOrderIsNotPending()
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

        order.Cancel();

        DomainException exception =
            Assert.Throws<DomainException>(
                () => order.Cancel());

        Assert.Equal(
            "Only pending orders can be cancelled.",
            exception.Message);
    }
}