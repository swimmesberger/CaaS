using CaaS.Core.CartAggregate;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountApplier {
    Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default);

}