using CaaS.Core.CartAggregate;

namespace CaaS.Core.DiscountAggregate; 

public interface IDiscountService {
    Cart ApplyDiscount(Cart cart);
}