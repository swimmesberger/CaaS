using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.CartAggregate; 

public record Cart : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public Customer? Customer { get; init; }
    public IReadOnlyList<CartItem> Items { get; init; } = ImmutableArray<CartItem>.Empty;
    public IReadOnlyList<Coupon> Coupons { get; init; } = ImmutableArray<Coupon>.Empty;
    public IReadOnlyList<Discount> CartDiscounts { get; init; } = ImmutableArray<Discount>.Empty;
    public DateTimeOffset LastAccess { get; init; } = SystemClock.GetNow();
    public string ConcurrencyToken { get; init; } = string.Empty;

    public decimal TotalPrice {
        get {
            var price = Items.Select(i => i.TotalPrice).Sum();
            foreach (var cartCoupon in Coupons) {
                price -= cartCoupon.Value;
            }
            foreach (var cartDiscount in CartDiscounts) {
                price -= cartDiscount.DiscountValue;
            }
            return price;
        }
    }
}