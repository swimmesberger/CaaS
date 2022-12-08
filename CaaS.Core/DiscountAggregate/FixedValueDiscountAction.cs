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
            var discountValue = data.TotalPrice - _settings.Value!.Value;
            if (discountValue < 0) {
                discountValue -= Math.Abs(discountValue);
            }
            return new Discount() {
                DiscountName = _settings.Name,
                DiscountValue = discountValue,
                ParentId = cart.Id,
                ShopId = cart.ShopId
            };
        }));
    }
}