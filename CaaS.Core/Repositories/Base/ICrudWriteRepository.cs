using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base; 

public interface ICrudWriteRepository<T> : IRepository where T : IEntityBase {
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T oldEntity, T newEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}