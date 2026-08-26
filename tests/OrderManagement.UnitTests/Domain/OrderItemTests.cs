using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.UnitTests.Domain;

public sealed class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldCreateItem_WhenDataIsValid()
    {
        OrderItem item =
            new OrderItem(
                "Notebook",
                2,
                3500);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Notebook", item.ProductName);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(3500, item.UnitPrice);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenProductNameIsEmpty()
    {
        DomainException exception =
            Assert.Throws<DomainException>(
                () =>
                {
                    new OrderItem(
                        "",
                        1,
                        100);
                });

        Assert.Equal(
            "Product name is required.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenQuantityIsZero()
    {
        DomainException exception =
            Assert.Throws<DomainException>(
                () =>
                {
                    new OrderItem(
                        "Notebook",
                        0,
                        3500);
                });

        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUnitPriceIsZero()
    {
        DomainException exception =
            Assert.Throws<DomainException>(
                () =>
                {
                    new OrderItem(
                        "Notebook",
                        1,
                        0);
                });

        Assert.Equal(
            "Unit price must be greater than zero.",
            exception.Message);
    }

    [Fact]
    public void TotalAmount_ShouldMultiplyQuantityByUnitPrice()
    {
        OrderItem item =
            new OrderItem(
                "Notebook",
                2,
                3500);

        decimal totalAmount =
            item.TotalAmount();

        Assert.Equal(7000, totalAmount);
    }
}