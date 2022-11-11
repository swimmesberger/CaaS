using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface IOrderRepository : IRepository {
    Task<IReadOnlyList<Order>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order oldEntity, Order newEntity, CancellationToken cancellationToken = default);
}