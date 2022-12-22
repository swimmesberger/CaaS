using CaaS.Core.Base;

namespace CaaS.Core.CouponAggregate; 

public interface ICouponRepository : IRepository {
    Task<Coupon?> FindByIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIdsAsync(IReadOnlyCollection<Guid> orderIds, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByCartIdsAsync(IReadOnlyCollection<Guid> cartIds, CancellationToken cancellationToken = default);

    Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels,
        CancellationToken cancellationToken = default);
    Task<Coupon> UpdateAsync(Coupon oldEntity, Coupon newEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Coupon coupon, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
}