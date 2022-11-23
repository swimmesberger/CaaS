using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public class MinProductCountDiscountRule : IDiscountRule {
    public static readonly Guid Id = new Guid("24EEFF2C-65C6-4482-B1AC-C6CB5F2D6B84");
    
    private readonly MinProductCountSettings _settings;

    public MinProductCountDiscountRule(IDiscountOptions<MinProductCountSettings> settings) {
        _settings = settings.Value;
    }

    public Task<RuleResult> Evaluate(Cart cart, CancellationToken cancellationToken = default) {
        var applicableItemIds = cart.Items
            .Where(i => i.Product.Id == _settings.ProductId && i.Amount >= _settings.NumberOfItems)
            .Select(i => i.Id)
            .ToHashSet();
        if(!applicableItemIds.Any()) 
            return Task.FromResult(RuleResult.NotApplicable);
        return Task.FromResult(RuleResult.Applicable with {
            AffectedItemIds = applicableItemIds
        });
    }
}