using System.Linq.Expressions;
using CaaS.Core.Entities.Base;

namespace CaaS.Core.Repositories.Base; 

public interface IRepository<T> where T : Entity {
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default );
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}