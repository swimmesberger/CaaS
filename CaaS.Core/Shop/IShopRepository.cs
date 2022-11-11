using CaaS.Core.Base;

namespace CaaS.Core.Shop; 

public interface IShopRepository : IRepository {
    Task<Entities.Shop?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Shop>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Entities.Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Shop>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<Entities.Shop> UpdateAsync(Entities.Shop entity, CancellationToken cancellationToken = default);
}