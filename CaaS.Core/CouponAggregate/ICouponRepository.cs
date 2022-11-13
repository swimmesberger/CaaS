using CaaS.Core.Base;

namespace CaaS.Core.CouponAggregate; 

public interface ICouponRepository : IRepository {
    Task<IReadOnlyList<Coupon>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByCartId(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default);

    Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default);
    Task AddAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels,
        CancellationToken cancellationToken = default);
    Task<Coupon> UpdateAsync(Coupon oldEntity, Coupon newEntity, CancellationToken cancellationToken = default);

}