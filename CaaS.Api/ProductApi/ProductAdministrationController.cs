using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ProductApi.Models;
using CaaS.Core.Base;
using CaaS.Core.ProductAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.ProductApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class ProductAdministrationController : ControllerBase {
    private readonly IProductService _productService;
    private readonly IStatisticsService _statisticsService;
    private readonly IMapper _mapper;

    public ProductAdministrationController(IProductService productService, IStatisticsService statisticsService, IMapper mapper) {
        _productService = productService;
        _mapper = mapper;
        _statisticsService = statisticsService;
    }
    
    [HttpPost]
    public async Task<ActionResult> AddProduct(ProductForCreationDto productDto, CancellationToken cancellationToken = default) {
        var product = _mapper.Map<Product>(productDto);
        await _productService.AddProduct(product, cancellationToken);
        
        return CreatedAtAction(
            actionName: nameof(AddProduct),
            routeValues: new { productId = product.Id },
            value: null);
    }
    
    [HttpPut("{productId:guid}")]
    public async Task<ProductDto> SetPriceOfProduct(Guid productId, [FromQuery][Required] decimal price,
        CancellationToken cancellationToken = default) {
        var result = await _productService.SetPriceOfProduct(productId, price, cancellationToken);
        return _mapper.Map<ProductDto>(result);
    }
    
    [HttpDelete("{productId:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken = default) {
        await _productService.DeleteProduct(productId, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("/MostSoldProducts")]
    public async Task<MostSoldProductResult> GetMostSoldProduct([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset until, CancellationToken cancellationToken = default) {
        return await _statisticsService.GetMostSoldProduct(from, until, cancellationToken: cancellationToken);
    }
}