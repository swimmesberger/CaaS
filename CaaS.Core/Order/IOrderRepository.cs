using CaaS.Core.Base;

namespace CaaS.Core.Order; 

public interface IOrderRepository : IRepository {
    Task<IReadOnlyList<Entities.Order>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Entities.Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Order>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<Entities.Order> AddAsync(Entities.Order entity, CancellationToken cancellationToken = default);
    Task<Entities.Order> UpdateAsync(Entities.Order oldEntity, Entities.Order newEntity, CancellationToken cancellationToken = default);
}