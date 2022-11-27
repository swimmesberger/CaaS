using System.Collections.Immutable;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public static class CartExtensions {

    public static Cart ApplyDiscounts(this Cart cart, RuleResult triggeredRule, Func<CartDiscountData, Discount> discount) {
        if (triggeredRule.HasAffectedItems) {
            return cart with {
                Items = cart.Items.Select(item => {
                    if (!triggeredRule.AffectedItemIds.Contains(item.Id)) return item;
                    return item with {
                        CartItemDiscounts = item.CartItemDiscounts.Add(discount.Invoke(new CartDiscountData(item.TotalPrice)))
                    };
                }).ToImmutableArray()
            };
        }
        return cart with {
            CartDiscounts = cart.CartDiscounts.Add(discount.Invoke(new CartDiscountData(cart.TotalPrice)))
        };
    }
}