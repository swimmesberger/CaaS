namespace CaaS.Core.CouponAggregate; 

public interface ICouponService {
    Task<Coupon> GetCouponById(Guid couponId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Coupon>> GetCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Coupon>> GetCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Coupon>> GetCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<Coupon> SetValueOfCoupon(Guid couponId, decimal value, CancellationToken cancellationToken = default);
    
    Task<Coupon> UpdateCoupon(Guid couponId, Coupon updatedCoupon, CancellationToken cancellationToken = default);
    
    Task<Coupon> AddCoupon(Coupon coupon, CancellationToken cancellationToken = default);
    
    Task DeleteCoupon(Guid couponId, CancellationToken cancellationToken = default);
}