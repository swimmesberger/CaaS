using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

public sealed class RuleBasedDiscountApplier : IDiscountApplier {
    private readonly IDiscountRule _rule;
    private readonly IDiscountAction _action;

    public RuleBasedDiscountApplier(IDiscountRule rule, IDiscountAction action) {
        _rule = rule;
        _action = action;
    }

    public async Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default) {
        var evaluation = await _rule.Evaluate(cart, cancellationToken);
        if (evaluation.Type == RuleResultType.None) return cart;
        return await _action.ApplyDiscountAsync(cart, evaluation, cancellationToken);
    }
}