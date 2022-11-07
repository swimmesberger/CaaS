using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface IShopRepository : ICrudRepository<Shop> {
    Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}