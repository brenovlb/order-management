using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Order>> GetPagedAsync(int page,
                                                    int pageSize,
                                                    CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}