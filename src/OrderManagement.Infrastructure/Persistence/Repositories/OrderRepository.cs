using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(
            order,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        _dbContext.Orders.Update(order);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        List<Order> orders = await _dbContext.Orders
            .Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return orders;
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .CountAsync(cancellationToken);
    }
}