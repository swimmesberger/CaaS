namespace CaaS.Core.Discount; 

public interface IDiscountService {
    Cart.Entities.Cart ApplyDiscount(Cart.Entities.Cart cart);
}