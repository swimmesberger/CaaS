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
            var price = BasePrice;
            price -= DiscountValue;
            return price;
        }
    }
    
    public decimal BasePrice {
        get {
            return Items.Select(i => i.TotalPrice).Sum();
        }
    }
    
    public decimal DiscountValue {
        get {
            decimal discount = 0;
            foreach (var cartCoupon in Coupons) {
                discount += cartCoupon.Value;
            }
            foreach (var cartDiscount in CartDiscounts) {
                discount += cartDiscount.DiscountValue;
            }
            return discount;
        }
    }
}