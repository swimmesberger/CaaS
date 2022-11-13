using System.ComponentModel.DataAnnotations;
using CaaS.Api.Base.Attributes;
using CaaS.Core.CartAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.CartEndpoints; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
public class CartController : ControllerBase {
    private readonly ICartService _cartService;

    public CartController(ICartService cartService) {
        _cartService = cartService;
    }

    [HttpGet("{cartId:guid}")]
    [ReadApi]
    public async Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default) {
        return await _cartService.GetCartById(cartId, cancellationToken);
    }

    [HttpPost("{cartId:guid}/product/{productId:guid}")]
    [WriteApi]
    public async Task<Cart> AddProductToCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity, CancellationToken cancellationToken = default) {
        return await _cartService.AddProductToCart(cartId, productId, productQuantity, cancellationToken);
    }

    [HttpDelete("{cartId:guid}/product/{productId:guid}")]
    [WriteApi]
    public async Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default) {
        return await _cartService.RemoveProductFromCart(cartId, productId, cancellationToken);
    }

    [HttpPut("{cartId:guid}/product/{productId:guid}")]
    [WriteApi]
    public async Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, [Required][FromQuery] int productQuantity,
            CancellationToken cancellationToken = default) {
        return await _cartService.SetProductQuantityInCart(cartId, productId, productQuantity, cancellationToken);
    }
}