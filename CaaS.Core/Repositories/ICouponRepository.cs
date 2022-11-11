using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface ICouponRepository : ICrudRepository<Coupon>, ICrudBulkWriteRepository<Coupon> {
    Task<IImmutableList<Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IImmutableList<Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<IImmutableList<Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default);

    Task UpdateCouponsAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels,
        CancellationToken cancellationToken = default);

}