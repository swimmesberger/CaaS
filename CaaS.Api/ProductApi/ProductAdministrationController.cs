using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ProductApi.Models;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.ProductAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.ProductApi; 

[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
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
    public async Task<ActionResult> AddProduct([FromBody] ProductForCreationDto productDto, CancellationToken cancellationToken = default) {
        var product = _mapper.Map<Product>(productDto);
        var result = await _productService.AddAsync(product, cancellationToken);
        
        return CreatedAtAction(
            controllerName: "Product",
            actionName: nameof(ProductController.GetById),
            routeValues: new { productId = product.Id },
            value: _mapper.Map<ProductDto>(result));
    }
    
    [HttpPut("setPrice/{productId:guid}")]
    public async Task<ProductDto> SetPriceOfProduct(Guid productId, [FromQuery][Required] decimal price,
        CancellationToken cancellationToken = default) {
        var result = await _productService.SetPriceAsync(productId, price, cancellationToken);
        return _mapper.Map<ProductDto>(result);
    }
    
    [HttpPut("{productId:guid}")]
    public async Task<ProductDto> UpdateCoupon(Guid productId, [FromBody] ProductForUpdateDto productDto, CancellationToken cancellationToken = default) {
        var updatedCoupon = _mapper.Map<Product>(productDto);
        var result = await _productService.UpdateAsync(productId, updatedCoupon, cancellationToken);
        return _mapper.Map<ProductDto>(result);
    }
    
    [HttpDelete("{productId:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken = default) {
        await _productService.DeleteAsync(productId, cancellationToken);
        return NoContent();
    }
    
    [HttpGet("/MostSoldProducts")]
    public async Task<MostSoldProductResult> MostSoldProducts([FromQuery][Required] DateTimeOffset from, 
                    [FromQuery][Required] DateTimeOffset until, CancellationToken cancellationToken = default) {
        if (from.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(from)} must be specified in UTC");
        }
        if (until.Offset != TimeSpan.Zero) {
            throw new CaasValidationException($"{nameof(until)} must be specified in UTC");
        }
        return await _statisticsService.MostSoldProductOverall(from, until, cancellationToken: cancellationToken);
    }
}