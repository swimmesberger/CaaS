using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public class AndDiscountAction : IDiscountAction {
    public static readonly Guid Id = new Guid("5B8D1040-23C6-45A6-A4EB-5B8495504A76");
    
    private readonly AndDiscountActionSettings _settings;
    private readonly IDiscountComponentFactory _discountComponentFactory;
    
    private readonly IEnumerable<IDiscountAction>? _discountActions;

    public AndDiscountAction(AndDiscountActionSettings settings, IDiscountComponentFactory discountComponentFactory) {
        _settings = settings;
        _discountComponentFactory = discountComponentFactory;
    }
    
    internal AndDiscountAction(IEnumerable<IDiscountAction> discountActions) {
        _settings = null!;
        _discountComponentFactory = null!;
        
        _discountActions = discountActions;
    }

    public async Task<Cart> ApplyDiscountAsync(Cart cart, RuleResult triggeredRule, CancellationToken cancellationToken = default) {
        foreach (var discountComponent in GetDiscountComponents()) {
            cart = await discountComponent.ApplyDiscountAsync(cart, triggeredRule, cancellationToken);
        }
        return cart;
    }
    
    private IEnumerable<IDiscountAction> GetDiscountComponents() {
        return _discountActions ?? _settings.DiscountSettings.Select(s => _discountComponentFactory.CreateAction(s));
    }
}