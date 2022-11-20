using System.ComponentModel.DataAnnotations;
using CaaS.Api.Base.Attributes;
using CaaS.Core.ProductAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.ProductApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class ProductAdministrationController : ControllerBase {
    private readonly IProductService _productService;

    public ProductAdministrationController(IProductService productService) {
        _productService = productService;
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct(string name, decimal price, 
        string description = "", string downloadLink = "", CancellationToken cancellationToken = default) {
        var product = await _productService.AddProduct(name, price, description, downloadLink, cancellationToken);
        
        return CreatedAtAction(
            actionName: nameof(AddProduct),
            routeValues: new { productId = product.Id },
            value: product);
    }
    
    [HttpPut("{productId:guid}/price/{price}")]
    public async Task<Product> SetPriceOfProduct(Guid productId, [Required][FromQuery] decimal price,
        CancellationToken cancellationToken = default) {
        return await _productService.SetPriceOfProduct(productId, price, cancellationToken);
    }
    
    [HttpDelete("{productId:guid}")]
    public async Task DeleteProduct(Guid productId, CancellationToken cancellationToken = default) {
        await _productService.DeleteProduct(productId, cancellationToken);
    }
}