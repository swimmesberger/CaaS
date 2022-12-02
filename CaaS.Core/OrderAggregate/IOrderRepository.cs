﻿using CaaS.Core.Base;

namespace CaaS.Core.OrderAggregate; 

public interface IOrderRepository : IRepository {
    Task<IReadOnlyList<Order>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> FindByDateRange(DateTimeOffset from, DateTimeOffset until, CancellationToken cancellationToken = default);
    
    Task<Order> AddAsync(Order entity, Address address, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order oldEntity, Order newEntity, CancellationToken cancellationToken = default);
}