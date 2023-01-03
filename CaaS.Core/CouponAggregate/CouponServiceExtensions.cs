using CaaS.Core.Base;

namespace CaaS.Core.CouponAggregate; 

public static class CouponServiceExtensions {
    public static Task<CountedResult<Coupon>> GetByOrderIdAsync(this ICouponService service, Guid orderId, CancellationToken cancellationToken = default) {
        return service.GetByAsync(new CouponQuery(){ OrderId = orderId }, cancellationToken: cancellationToken);
    }
    
    public static Task<CountedResult<Coupon>> GetByCustomerIdAsync(this ICouponService service, Guid customerId, CancellationToken cancellationToken = default) {
        return service.GetByAsync(new CouponQuery(){ CustomerId = customerId }, cancellationToken: cancellationToken);
    }
    
    public static Task<CountedResult<Coupon>> GetByCartIdAsync(this ICouponService service, Guid cartId, CancellationToken cancellationToken = default) {
        return service.GetByAsync(new CouponQuery(){ CartId = cartId }, cancellationToken: cancellationToken);
    }
    
    public static async Task<Coupon?> GetByCodeAsync(this ICouponService service, string code, CancellationToken cancellationToken = default) {
        var result = await service.GetByAsync(new CouponQuery(){ Code = code }, cancellationToken: cancellationToken);
        return result.Items.FirstOrDefault();
    }
}