using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using FluentValidation.Results;

namespace CaaS.Core.CartAggregate; 

public class CartService : ICartService {
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IDiscountService _discountService;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IDateTimeOffsetProvider _timeProvider;

    public CartService(ICartRepository cartRepository, IShopRepository shopRepository, IDiscountService discountService, 
        ITenantIdAccessor tenantIdAccessor,  IDateTimeOffsetProvider timeProvider, ICustomerRepository customerRepository, IProductRepository productRepository) {
        _cartRepository = cartRepository;
        _shopRepository = shopRepository;
        _discountService = discountService;
        _tenantIdAccessor = tenantIdAccessor;
        _timeProvider = timeProvider;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) return null;
        var updatedCart = cart with { LastAccess = _timeProvider.GetNow() };
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
                LastAccess = _timeProvider.GetNow()
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
                LastAccess = _timeProvider.GetNow()
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
                LastAccess = _timeProvider.GetNow()
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
            LastAccess = _timeProvider.GetNow()
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
        var productItemIdx = cart.Items.FindIndex(p => p.Id == productId);
        if (productItemIdx == -1) {
            throw new CaasItemNotFoundException();
        } 
        var productItem = cart.Items[productItemIdx];
        productItem = productItem with {
            Amount = productQuantity,
        };
        var updatedCart = cart with {
            Items = cart.Items.SetItem(productItemIdx, productItem),
            LastAccess = _timeProvider.GetNow()
        };
        updatedCart = await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
        updatedCart = await PostProcessCart(updatedCart);
        return updatedCart;
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
    
}