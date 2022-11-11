using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface ICouponRepository : IRepository {
    Task<IReadOnlyList<Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds, CancellationToken cancellationToken = default);

    Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default);
    Task AddAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels,
        CancellationToken cancellationToken = default);

}