using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class CompositeDiscountRule : IDiscountRule {
    public static readonly Guid Id = new Guid("B5791E0F-D839-45CE-92FE-94F3F5F19DEF");
    
    private readonly CompositeDiscountRuleSettings _settings;
    private readonly IDiscountSettingRepository _discountSettingRepository;
    private readonly IDiscountComponentFactory _discountComponentFactory;

    private readonly IEnumerable<DiscountComponent>? _discountComponents;
    private readonly DiscountCombinationType _combinationType;

    public CompositeDiscountRule(IDiscountOptions<CompositeDiscountRuleSettings> settings, IDiscountSettingRepository discountSettingRepository, 
        IDiscountComponentFactory discountComponentFactory) {
        _discountComponentFactory = discountComponentFactory;
        _discountSettingRepository = discountSettingRepository;
        _settings = settings.Value;
        _combinationType = _settings.CombinationType;
    }
    
    internal CompositeDiscountRule(IEnumerable<DiscountComponent> discountComponents, DiscountCombinationType combinationType = DiscountCombinationType.And) {
        _settings = null!;
        _discountSettingRepository = null!;
        _discountComponentFactory = null!;
        
        _discountComponents = discountComponents;
        _combinationType = combinationType;
    }
    
    public Task<RuleResult> Evaluate(Cart cart, CancellationToken cancellationToken = default) {
        return _combinationType == DiscountCombinationType.And
            ? AndIsApplicable(cart, cancellationToken)
            : OrIsApplicable(cart, cancellationToken);
    }
    
    private async Task<RuleResult> AndIsApplicable(Cart cart, CancellationToken cancellationToken = default) {
        var result = RuleResult.Applicable;
        foreach (var discountComponent in await GetDiscountComponents(cancellationToken)) {
            var cResult = await discountComponent.Rule.Evaluate(cart, cancellationToken);
            if (cResult.Type == RuleResultType.None) {
                return RuleResult.NotApplicable;
            }
            result = result.Add(cResult);
        }
        return result;
    }
    
    private async Task<RuleResult> OrIsApplicable(Cart cart, CancellationToken cancellationToken = default) {
        foreach (var discountComponent in await GetDiscountComponents(cancellationToken)) {
            var cResult = await discountComponent.Rule.Evaluate(cart, cancellationToken);
            if (cResult.Type == RuleResultType.Applicable) {
                return cResult;
            }
        }
        return RuleResult.NotApplicable;
    }

    private async Task<IEnumerable<DiscountComponent>> GetDiscountComponents(CancellationToken cancellationToken = default) {
        IEnumerable<DiscountComponent> discountComponents;
        if (_discountComponents != null) {
            discountComponents = _discountComponents;
        } else {
            var settings = await _discountSettingRepository.FindByIdsAsync(_settings.DiscountSettingIds, cancellationToken);
            discountComponents = settings.Select(_discountComponentFactory.CreateComponent);
        }
        return discountComponents;
    }
}