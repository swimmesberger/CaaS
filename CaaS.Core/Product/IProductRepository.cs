using CaaS.Core.Base;

namespace CaaS.Core.Product; 

public interface IProductRepository : IRepository {
    Task<Entities.Product?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Product>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Product>> FindAllAsync(CancellationToken cancellationToken = default);
}