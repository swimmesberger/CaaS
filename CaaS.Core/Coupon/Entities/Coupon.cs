using CaaS.Core.Base;

namespace CaaS.Core.Coupon.Entities; 

public record Coupon : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; } = default;
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; } = default!;
    public Guid? CartId { get; init; } = default!;
    public Guid? CustomerId { get; init; } = default!;
    public string ConcurrencyToken { get; init; } = string.Empty;
}