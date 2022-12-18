using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Validation;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.CartAggregate; 

public class CartService : ICartService {
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IDiscountService _discountService;
    private readonly ICouponRepository _couponRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly ISystemClock _timeProvider;

    public CartService(ICartRepository cartRepository, ICustomerRepository customerRepository, IProductRepository productRepository, 
        IShopRepository shopRepository, IDiscountService discountService, ICouponRepository couponRepository, 
        ITenantIdAccessor tenantIdAccessor,  ISystemClock timeProvider) {
        _cartRepository = cartRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _couponRepository = couponRepository;
        _discountService = discountService;
        _tenantIdAccessor = tenantIdAccessor;
        _timeProvider = timeProvider;
    }

    public async Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) return null;
        var updatedCart = cart with { LastAccess = _timeProvider.UtcNow };
        cart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        cart = await PostProcessCart(cart);
        return cart;
    }

    public async Task<int> DeleteExpiredCarts(CancellationToken cancellationToken = default) {
        var cartLifetime = await _shopRepository.FindCartLifetimeByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (cartLifetime == null) {
            throw new CaasValidationException(new ValidationFailure("tenantId", "Invalid tenant id"));
        }
        var expiredCarts = await _cartRepository.FindExpiredCarts(cartLifetime.Value, cancellationToken);
        await _cartRepository.DeleteAsync(expiredCarts, cancellationToken);
        return expiredCarts.Count;
    }

    public async Task<Cart> AddProductToCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default) {
        if (productQuantity <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(productQuantity), "Invalid product quantity"));
        }

        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException($"product {productId} not found");
        }
        
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            // create cart if it does not exist yet
            cart = new Cart() {
                Id = cartId,
                ShopId = _tenantIdAccessor.GetTenantGuid(),
                LastAccess = _timeProvider.UtcNow
            };
            cart = await _cartRepository.AddAsync(cart, cancellationToken);
        }
        var productItemIdx = cart.Items.FindIndex(p => p.Product.Id == productId);
        Cart updatedCart;
        if (productItemIdx != -1) {     //product already in cart
            var productItem = cart.Items[productItemIdx];
            productItem = productItem with {
                Amount = productItem.Amount + productQuantity
            };
            updatedCart = cart with {
                Items = cart.Items.SetItem(productItemIdx, productItem),
                LastAccess = _timeProvider.UtcNow
            };
        } else {    //product not yet in cart
            var productItem = new CartItem() {
                Product = product,
                ShopId = _tenantIdAccessor.GetTenantGuid(),
                CartId = cartId,
                Amount = productQuantity,
            };
            updatedCart = cart with {
                Items = cart.Items.Add(productItem),
                LastAccess = _timeProvider.UtcNow
            };
        }
        updatedCart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        updatedCart = await PostProcessCart(updatedCart);
        return updatedCart;
    }
    
    public async Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        var changedProducts = cart.Items.RemoveAll(p => p.Product.Id == productId);
        if (cart.Items.Count == changedProducts.Count) {
            return cart;
        }
        var updatedCart = cart with {
            Items = changedProducts,
            LastAccess = _timeProvider.UtcNow
        };
        updatedCart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        updatedCart = await PostProcessCart(updatedCart);
        return updatedCart;
    }
    
    public async Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default) {
        if (productQuantity <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(productQuantity), "Invalid product quantity"));
        }
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        var productItemIdx = cart.Items.FindIndex(p => p.Product.Id == productId);
        if (productItemIdx == -1) {
            throw new CaasItemNotFoundException();
        } 
        var productItem = cart.Items[productItemIdx];
        productItem = productItem with {
            Amount = productQuantity,
        };
        var updatedCart = cart with {
            Items = cart.Items.SetItem(productItemIdx, productItem),
            LastAccess = _timeProvider.UtcNow
        };
        updatedCart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        updatedCart = await PostProcessCart(updatedCart);
        return updatedCart;
    }
    
    public async Task<Cart> AddCouponToCart(Guid cartId, Guid couponId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException($"Cart {cartId} not found");
        }
        
        var coupon = await _couponRepository.FindByIdAsync(couponId, cancellationToken);
        if (coupon == null) {
            throw new CaasItemNotFoundException($"Coupon {couponId} not found");
        }

        if (cart.TotalPrice - coupon.Value < 0) {
            throw new CaasValidationException("Cannot add coupon because cart value would be negative");
        }
        
        if (coupon.OrderId != null) {
            throw new CaasValidationException($"Coupon '{couponId}' was already redeemed by order '{coupon.OrderId}'");
        }

        var updatedCoupon = coupon with {
            CartId = cartId,
            CustomerId = cart.Customer?.Id
        };

        await _couponRepository.UpdateAsync(coupon, updatedCoupon, cancellationToken);
        return (await _cartRepository.FindByIdAsync(cartId, cancellationToken))!;
    }

    private async Task<Cart> PostProcessCart(Cart cart) {
        return await _discountService.ApplyDiscountAsync(cart);
    }
    
    public async Task DeleteCart(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        await _cartRepository.DeleteAsync(cart, cancellationToken);
    }
    public Task<long> Count(CancellationToken cancellationToken = default) {
        return _cartRepository.CountAsync(cancellationToken);
    }
}