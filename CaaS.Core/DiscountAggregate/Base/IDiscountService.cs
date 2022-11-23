using CaaS.Core.CartAggregate;

namespace CaaS.Core.DiscountAggregate.Base;

public interface IDiscountService {
    Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default);

    Task<IEnumerable<DiscountComponentMetadata>> GetDiscountRules();
    
    Task<IEnumerable<DiscountComponentMetadata>> GetDiscountActions();
}