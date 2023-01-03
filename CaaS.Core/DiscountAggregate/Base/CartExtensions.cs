using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public static class CartExtensions {

    public static Cart ApplyDiscounts(this Cart cart, RuleResult triggeredRule, Func<CartDiscountData, Discount?> discountAction) {
        if (triggeredRule.HasAffectedItems) {
            return cart with {
                Items = cart.Items.Select(item => {
                    if (!triggeredRule.AffectedItemIds.Contains(item.Id)) return item;
                    
                    var itemDiscount = discountAction.Invoke(new CartDiscountData(item.TotalPrice));
                    if (itemDiscount == null) 
                        return item;
                    return item with {
                        CartItemDiscounts = item.CartItemDiscounts.Add(itemDiscount)
                    };
                }).ToImmutableArray()
            };
        }
        var discount = discountAction.Invoke(new CartDiscountData(cart.TotalPrice));
        if (discount == null) 
            return cart;
        return cart with {
            CartDiscounts = cart.CartDiscounts.Add(discount)
        };
    }
}