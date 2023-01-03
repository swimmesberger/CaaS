namespace CaaS.Core.CouponAggregate; 

public static class CouponRepositoryExtensions {
    public static Task<IReadOnlyList<Coupon>> FindByOrderIdAsync(this ICouponRepository repository, Guid orderId, CancellationToken cancellationToken = default) {
        return repository.FindByAsync(new CouponQuery(){ OrderId = orderId }, cancellationToken: cancellationToken);
    }
    
    public static Task<IReadOnlyList<Coupon>> FindByCustomerIdAsync(this ICouponRepository repository, Guid customerId, CancellationToken cancellationToken = default) {
        return repository.FindByAsync(new CouponQuery(){ CustomerId = customerId }, cancellationToken: cancellationToken);
    }
    
    public static Task<IReadOnlyList<Coupon>> FindByCartIdAsync(this ICouponRepository repository, Guid cartId, CancellationToken cancellationToken = default) {
        return repository.FindByAsync(new CouponQuery(){ CartId = cartId }, cancellationToken: cancellationToken);
    }
    
    public static async Task<Coupon?> FindByCodeAsync(this ICouponRepository repository, string code, CancellationToken cancellationToken = default) {
        var result = await repository.FindByAsync(new CouponQuery(){ Code = code }, cancellationToken: cancellationToken);
        return result.FirstOrDefault();
    }
}