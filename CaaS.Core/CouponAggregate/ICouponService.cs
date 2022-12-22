using CaaS.Core.Base;

namespace CaaS.Core.CouponAggregate; 

public interface ICouponService {
    Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    
    Task<CountedResult<Coupon>> GetByCartIdAsync(Guid cartId, CancellationToken cancellationToken = default);
    
    Task<CountedResult<Coupon>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    
    Task<CountedResult<Coupon>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<Coupon> SetValueOfCouponAsync(Guid couponId, decimal value, CancellationToken cancellationToken = default);
    
    Task<Coupon> UpdateAsync(Guid couponId, Coupon updatedCoupon, CancellationToken cancellationToken = default);
    
    Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid couponId, CancellationToken cancellationToken = default);
}