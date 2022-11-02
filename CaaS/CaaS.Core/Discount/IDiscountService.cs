using CaaS.Core.Entities;

namespace CaaS.Core.Discount; 

public interface IDiscountService {
    Cart ApplyDiscount(Cart cart);
}