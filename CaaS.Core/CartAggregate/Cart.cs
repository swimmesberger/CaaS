using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.OrderAggregate;

namespace CaaS.Core.CartAggregate; 

public record Cart : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; } = default;
    public Customer? Customer { get; init; }
    public IImmutableList<CartItem> Items { get; init; } = ImmutableArray<CartItem>.Empty;
    public IImmutableList<Coupon> Coupons { get; init; } = ImmutableArray<Coupon>.Empty;
    public IImmutableList<Discount> CartDiscounts { get; init; } = ImmutableArray<Discount>.Empty;
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffset.UtcNow;
    public string ConcurrencyToken { get; init; } = string.Empty;
}