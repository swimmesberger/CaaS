using System.ComponentModel.DataAnnotations;
using CaaS.Api.Base.Middleware;
using CaaS.Core.Cart;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Cart; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
public class CartController {
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) {
        _cartService = cartService;
    }

    [HttpGet("{cartId:guid}")]
    public async Task<Core.Cart.Entities.Cart?> FindCartById(Guid cartId, CancellationToken cancellationToken = default) {
        return await _cartService.FindCartById(cartId, cancellationToken);
    }

    [HttpPost]
    public async Task<Core.Cart.Entities.Cart> CreateCart(CancellationToken cancellationToken = default) {
        return await _cartService.CreateCart(cancellationToken);
    }

    [HttpPost("{cartId:guid}/product/{productId:guid}")]
    public async Task<Core.Cart.Entities.Cart> AddProductToCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity, CancellationToken cancellationToken = default) {
        return await _cartService.AddProductToCart(cartId, productId, productQuantity, cancellationToken);
    }

    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    public async Task<Core.Cart.Entities.Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        return await _cartService.RemoveProductFromCart(cartId, productId, cancellationToken);
    }

    [HttpPut("{cartId:guid}/product/{productId:guid}")]
    public async Task<Core.Cart.Entities.Cart> SetProductQuantityInCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity,
            CancellationToken cancellationToken = default) {
        return await _cartService.SetProductQuantityInCart(cartId, productId, productQuantity, cancellationToken);
    }
}