using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountRule {
    Task<RuleResult> Evaluate(Cart cart, CancellationToken cancellationToken = default);
}