using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate; 

public interface IShopService {
    Task<CountedResult<Shop>> GetAll(CancellationToken cancellationToken = default);
    
    Task<Shop?> GetByName(string name, CancellationToken cancellationToken = default);

    Task<Shop> SetName(Guid id, string name, CancellationToken cancellationToken = default);

    Task<Shop> Add(Shop shop, CancellationToken cancellationToken = default);
}