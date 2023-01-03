using CaaS.Core.Base;

namespace CaaS.Core.CouponAggregate; 

public record Coupon : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public string Code { get; init; }
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CartId { get; init; }
    public Guid? CustomerId { get; init; }
    public string ConcurrencyToken { get; init; } = string.Empty;
}