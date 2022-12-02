using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IDao<T> {
    IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TValue> FindScalarBy<TValue>(StatementParameters parameters, CancellationToken cancellationToken = default);

    Task<long> CountAsync(StatementParameters? parameters = null, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> AddAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> UpdateAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default );
    Task DeleteAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default);

    IReadOnlyDictionary<string, object?> ReadPropertiesFromModel(T model, IEnumerable<string> properties);
}