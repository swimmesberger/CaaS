using System.Collections.Immutable;
using CaaS.Core.Entities;

namespace CaaS.Core.Repositories.Base; 

public interface ICouponRepository : ICrudRepository<Coupon> {
    Task<IImmutableList<Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IImmutableList<Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<IImmutableList<Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default);
}