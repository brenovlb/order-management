using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = new List<OrderItem>();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items
    {
        get
        {
            return _items.AsReadOnly();
        }
    }

    private Order()
    {
    }

    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer id is required.");
        }

        if (items == null)
        {
            throw new DomainException("Order items are required.");
        }

        List<OrderItem> orderItems = items.ToList();

        if (orderItems.Count == 0)
        {
            throw new DomainException("Order must contain at least one item.");
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        foreach (OrderItem item in orderItems)
        {
            item.SetOrderId(Id);
            _items.Add(item);
        }
    }

    public decimal TotalAmount()
    {
        decimal totalAmount = 0;

        foreach (OrderItem item in _items)
        {
            totalAmount += item.TotalAmount();
        }

        return totalAmount;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}