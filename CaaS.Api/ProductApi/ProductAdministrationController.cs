using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ProductApi.Models;
using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
    
    [HttpPut("{productId:guid}")]
    public async Task<ProductDto> UpdateProduct(Guid productId, [FromBody] ProductForUpdateDto productDto, CancellationToken cancellationToken = default) {
        var updatedProduct = _mapper.Map<Product>(productDto);
        updatedProduct = updatedProduct with {
            Id = productId
        };
        var result = await _productService.UpdateAsync(updatedProduct, cancellationToken);
        return _mapper.Map<ProductDto>(result);
    }
    
    [HttpDelete("{productId:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken = default) {
        await _productService.DeleteAsync(productId, cancellationToken);
        return NoContent();
    }
}