using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable ClassNeverInstantiated.Global
public class PercentageDiscountAction : IDiscountAction {
    public static readonly Guid Id = new Guid("29AD1EEA-1CFB-4473-8556-65F86FCA0471");
    
    private readonly PercentageDiscountSettings _settings;

    public PercentageDiscountAction(IDiscountOptions<PercentageDiscountSettings> settings) {
        _settings = settings.Value;
    }
    
    public Task<Cart> ApplyDiscountAsync(Cart cart, RuleResult triggeredRule, CancellationToken cancellationToken = default) {
        return Task.FromResult(cart.ApplyDiscounts(triggeredRule, data => {
            var discountValue = Math.Round(data.TotalPrice * _settings.Percentage!.Value, 2, MidpointRounding.ToEven);
            return new Discount() {
                DiscountName = _settings.Name,
                DiscountValue = discountValue,
                ParentId = cart.Id,
                ShopId = cart.ShopId
            };
        }));
    }
}