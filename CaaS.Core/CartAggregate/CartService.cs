using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using FluentValidation.Results;

namespace CaaS.Core.CartAggregate; 

public class CartService : ICartService {
    private readonly ICartRepository _cartRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly IShopRepository _shopRepository;

    public CartService(ICartRepository cartRepository, ITenantIdAccessor tenantIdAccessor, IShopRepository shopRepository) {
        _cartRepository = cartRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _shopRepository = shopRepository;
    }

    public async Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) return null;
        var updatedCart = cart with { LastAccess = DateTimeOffsetProvider.GetNow() };
        return await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
    }

    public async Task<int> DeleteExpiredCarts(CancellationToken cancellationToken = default) {
        var shops = await _shopRepository.FindAllAsync(cancellationToken);
        int count = 0;
        foreach (var shop in shops) {
            var expiredCarts = await _cartRepository.FindExpiredCarts(shop.CartLifetimeMinutes, cancellationToken);
            await _cartRepository.DeleteAsync(expiredCarts, cancellationToken);
            count += expiredCarts.Count;
        }
        return count;
    }

    public async Task<Cart> AddProductToCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default) {
        if (productQuantity <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(productQuantity), "Invalid product quantity"));
        }
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) {
            // create cart if it does not exist yet
            cart = new Cart() {
                Id = cartId,
                ShopId = _tenantIdAccessor.GetTenantGuid()
            };
            cart = await _cartRepository.AddAsync(cart, cancellationToken);
        }
        var productItemIdx = cart.Items.FindIndex(p => p.Product.Id == productId);
        Cart updatedCart;
        if (productItemIdx != -1) {
            var productItem = cart.Items[productItemIdx];
            productItem = productItem with {
                Amount = productItem.Amount + productQuantity
            };
            updatedCart = cart with {
                Items = cart.Items.SetItem(productItemIdx, productItem),
                LastAccess = DateTimeOffsetProvider.GetNow()
            };
        } else {
            var productItem = new CartItem() {
                Amount = productQuantity,
                CartId = cartId,
                Product = new Product() { Id = productId }
            };
            updatedCart = cart with {
                Items = cart.Items.Add(productItem),
                LastAccess = DateTimeOffsetProvider.GetNow()
            };
        }
        return await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
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
            LastAccess = DateTimeOffsetProvider.GetNow()
        };
        return await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
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
            LastAccess = DateTimeOffsetProvider.GetNow()
        };
        return await _cartRepository.UpdateAsync(cart, updatedCart, cancellationToken);
    }
}