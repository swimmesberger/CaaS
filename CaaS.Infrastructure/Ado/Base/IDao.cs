using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado.Base; 

public interface IDao<T> {
    IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> AddAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> UpdateAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default );
    Task DeleteAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default );
}