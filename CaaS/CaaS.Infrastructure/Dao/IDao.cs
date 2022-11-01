using CaaS.Infrastructure.Ado;

namespace CaaS.Infrastructure.Dao; 

public interface IDao<T> {
    IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindBy(string propertyName, object value, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindBy(QueryParameter parameter, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindBy(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default );
}