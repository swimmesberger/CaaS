﻿using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate; 

public interface IShopRepository : IRepository {
    Task<Shop?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shop>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shop>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    
    Task<Shop> UpdateAsync(Shop entity, CancellationToken cancellationToken = default);
}