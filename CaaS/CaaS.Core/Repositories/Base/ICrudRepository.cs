using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base;

public interface ICrudRepository<T> : ICrudReadRepository<T> where T : IEntityBase {
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}