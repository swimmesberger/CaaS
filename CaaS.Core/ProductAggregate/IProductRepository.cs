using CaaS.Core.Base;

namespace CaaS.Core.ProductAggregate; 

public interface IProductRepository : IRepository {
    Task<Product?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> FindAllAsync(CancellationToken cancellationToken = default);
}