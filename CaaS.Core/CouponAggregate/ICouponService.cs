using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;

namespace CaaS.Core.CouponAggregate; 

public interface ICouponService {
    Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default);

    Task<CountedResult<Coupon>> GetByAsync(CouponQuery couponQuery, CancellationToken cancellationToken = default);
    Task<PagedResult<Coupon>> GetByPagedAsync(CouponQuery couponQuery, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default);
    
    Task<Coupon> UpdateAsync(Coupon updatedCoupon, CancellationToken cancellationToken = default);
    
    Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid couponId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Coupon>> RedeemCouponsAsync(IEnumerable<Coupon> oldDomainModels, IReadOnlyCollection<Coupon> newDomainModels, Guid cartId, Guid? customerId = null,
        CancellationToken cancellationToken = default);
}