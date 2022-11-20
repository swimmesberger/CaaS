using CaaS.Core.Base;

namespace CaaS.Core.ProductAggregate; 

public interface IProductRepository : IRepository {
    Task<Product?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product entity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> FindByTextSearchAsync(string text, CancellationToken cancellationToken = default);
}