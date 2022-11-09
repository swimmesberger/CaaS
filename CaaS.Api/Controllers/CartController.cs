using System.ComponentModel.DataAnnotations;
using CaaS.Api.Middleware;
using CaaS.Core.Entities;
using CaaS.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
public class CartController {
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) {
        _cartService = cartService;
    }

    [HttpGet("{cartId:guid}")]
    public async Task<Cart?> FindCartById(Guid cartId, CancellationToken cancellationToken = default) {
        return await _cartService.FindCartById(cartId, cancellationToken);
    }

    [HttpPost]
    public async Task<Cart> CreateCart(CancellationToken cancellationToken = default) {
        return await _cartService.CreateCart(cancellationToken);
    }

    [HttpPost("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> AddProductToCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity, CancellationToken cancellationToken = default) {
        return await _cartService.AddProductToCart(cartId, productId, productQuantity, cancellationToken);
    }

    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        return await _cartService.RemoveProductFromCart(cartId, productId, cancellationToken);
    }

    [HttpPut("{cartId:guid}/product/{productId:guid}")]
    public async Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity,
            CancellationToken cancellationToken = default) {
        return await _cartService.SetProductQuantityInCart(cartId, productId, productQuantity, cancellationToken);
    }
}