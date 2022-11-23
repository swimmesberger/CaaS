using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountAction {
    Task<Cart> ApplyDiscountAsync(Cart cart, RuleResult triggeredRule, CancellationToken cancellationToken = default);
}