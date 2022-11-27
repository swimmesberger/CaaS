using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class CompositeDiscountRule : IDiscountRule {
    public static readonly Guid Id = new Guid("B5791E0F-D839-45CE-92FE-94F3F5F19DEF");
    
    private readonly CompositeDiscountRuleSettings _settings;
    private readonly IDiscountComponentFactory _discountComponentFactory;

    private readonly IEnumerable<IDiscountRule>? _discountRules;
    private readonly DiscountCombinationType _combinationType;

    public CompositeDiscountRule(IDiscountOptions<CompositeDiscountRuleSettings> settings, IDiscountComponentFactory discountComponentFactory) {
        _discountComponentFactory = discountComponentFactory;
        _settings = settings.Value;
        _combinationType = _settings.CombinationType;
    }
    
    internal CompositeDiscountRule(IEnumerable<IDiscountRule> discountRules, DiscountCombinationType combinationType = DiscountCombinationType.And) {
        _settings = null!;
        _discountComponentFactory = null!;
        
        _discountRules = discountRules;
        _combinationType = combinationType;
    }
    
    public Task<RuleResult> Evaluate(Cart cart, CancellationToken cancellationToken = default) {
        return _combinationType == DiscountCombinationType.And
            ? AndIsApplicable(cart, cancellationToken)
            : OrIsApplicable(cart, cancellationToken);
    }
    
    private async Task<RuleResult> AndIsApplicable(Cart cart, CancellationToken cancellationToken = default) {
        var result = RuleResult.Applicable;
        foreach (var discountComponent in GetDiscountRules()) {
            var cResult = await discountComponent.Evaluate(cart, cancellationToken);
            if (cResult.Type == RuleResultType.None) {
                return RuleResult.NotApplicable;
            }
            result = result.Add(cResult);
        }
        return result;
    }
    
    private async Task<RuleResult> OrIsApplicable(Cart cart, CancellationToken cancellationToken = default) {
        foreach (var discountComponent in GetDiscountRules()) {
            var cResult = await discountComponent.Evaluate(cart, cancellationToken);
            if (cResult.Type == RuleResultType.Applicable) {
                return cResult;
            }
        }
        return RuleResult.NotApplicable;
    }

    private IEnumerable<IDiscountRule> GetDiscountRules() {
        return _discountRules ?? _settings.DiscountSettings.Select(s => _discountComponentFactory.CreateRule(s));
    }
}