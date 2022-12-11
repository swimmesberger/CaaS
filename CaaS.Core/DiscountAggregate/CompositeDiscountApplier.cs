using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;

namespace CaaS.Core.DiscountAggregate; 

public sealed class CompositeDiscountApplier : IDiscountApplier {
    private readonly IEnumerable<IDiscountApplier> _actions;

    public CompositeDiscountApplier(IEnumerable<DiscountComponent> components) : this(components
        .Select(c => new RuleBasedDiscountApplier(c.Rule, c.Action))) { }
    
    public CompositeDiscountApplier(IEnumerable<IDiscountApplier> actions) {
        _actions = actions;
    }

    public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
        foreach (var action in _actions) {
            cart = await action.ApplyDiscountAsync(cart, cancellationToken);
        }
        return cart;
    }
}