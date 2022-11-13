﻿using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ProductAggregate;
using FluentValidation.Results;

namespace CaaS.Core.CartAggregate; 

public class CartService : ICartService {
    private readonly ICartRepository _cartRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public CartService(ICartRepository cartRepository, ITenantIdAccessor tenantIdAccessor) {
        _cartRepository = cartRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }

    public async Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);
        if (cart == null) return null;
        cart = cart with { LastAccess = DateTimeOffset.UtcNow };
        return await _cartRepository.UpdateAsync(cart, cancellationToken);
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
        if (productItemIdx != -1) {
            var productItem = cart.Items[productItemIdx];
            productItem = productItem with {
                Amount = productItem.Amount + productQuantity
            };
            cart = cart with {
                Items = cart.Items.SetItem(productItemIdx, productItem),
                LastAccess = DateTimeOffset.UtcNow
            };
        } else {
            var productItem = new CartItem() {
                Amount = productQuantity,
                CartId = cartId,
                Product = new Product() { Id = productId }
            };
            cart = cart with {
                Items = cart.Items.Add(productItem),
                LastAccess = DateTimeOffset.UtcNow
            };
        }
        return await _cartRepository.UpdateAsync(cart, cancellationToken);
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
        cart = cart with {
            Items = changedProducts,
            LastAccess = DateTimeOffset.UtcNow
        };
        return await _cartRepository.UpdateAsync(cart, cancellationToken);
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
        cart = cart with {
            Items = cart.Items.SetItem(productItemIdx, productItem),
            LastAccess = DateTimeOffset.UtcNow
        };
        return await _cartRepository.UpdateAsync(cart, cancellationToken);
    }
}