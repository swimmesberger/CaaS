using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable ClassNeverInstantiated.Global
public class TimeWindowDiscountRule : IDiscountRule {
    public static readonly Guid Id = new Guid("0F4E0B04-E32B-4897-804C-92F858468D8A");
    
    private readonly TimeWindowDiscountSettings _settings;

    public TimeWindowDiscountRule(IDiscountOptions<TimeWindowDiscountSettings> settings) {
        _settings = settings.Value;
    }

    public Task<RuleResult> Evaluate(Cart cart, CancellationToken cancellationToken = default) {
        var time = DateTimeOffsetProvider.GetNow();
        return Task.FromResult(time >= _settings.FromTime && time < _settings.ToTime ? 
            RuleResult.Applicable : 
            RuleResult.NotApplicable);
    }
}