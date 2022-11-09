using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base; 

public interface ICrudBulkWriteRepository<in T> : IRepository where T : IEntityBase {
    Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}