using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable ClassNeverInstantiated.Global
public class FixedValueDiscountAction : IDiscountAction {
    public static readonly Guid Id = new Guid("68A4020D-A8AC-4A74-8A04-24E449786898");
    
    private readonly FixedValueDiscountSettings _settings;

    public FixedValueDiscountAction(IDiscountOptions<FixedValueDiscountSettings> settings) {
        _settings = settings.Value;
    }
    
    public Task<Cart> ApplyDiscountAsync(Cart cart, RuleResult triggeredRule, CancellationToken cancellationToken = default) {
        return Task.FromResult(cart.ApplyDiscounts(triggeredRule, data => {
            var cartValue = data.TotalPrice - _settings.Value!.Value;
            
            if (cart.TotalPrice == 0) {     //if cart is already 0 do not apply discounts
                return null;
            }
            
            if (cartValue < 0) {            //if discount value is greater cart value: only apply remaining cart-value
                return new Discount() {
                    DiscountName = _settings.Name,
                    DiscountValue = cart.TotalPrice,
                    ParentId = cart.Id,
                    ShopId = cart.ShopId
                };
            }
            
            return new Discount() {        //standard case: apply discount fully
                DiscountName = _settings.Name,
                DiscountValue = _settings.Value!.Value,
                ParentId = cart.Id,
                ShopId = cart.ShopId
            };
        }));
    }
}