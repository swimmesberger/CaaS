using CaaS.Core.Base;

namespace CaaS.Core.Coupon; 

public interface ICouponRepository : IRepository {
    Task<IReadOnlyList<Entities.Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Entities.Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default);

    Task<Entities.Coupon> AddAsync(Entities.Coupon entity, CancellationToken cancellationToken = default);
    Task AddAsync(IEnumerable<Entities.Coupon> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<Entities.Coupon> oldDomainModels, IEnumerable<Entities.Coupon> newDomainModels,
        CancellationToken cancellationToken = default);

}