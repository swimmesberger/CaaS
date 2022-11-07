using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base;

// marker interface for all repositories
public interface IRepository { }

public interface ICrudReadRepository<T> : IRepository where T : IEntityBase {
    Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<T>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}