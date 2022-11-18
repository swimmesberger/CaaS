using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Core.ProductAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ProductEndpoints; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;

    public ProductController(IProductService productService) {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<IEnumerable<Product>> GetByTextSearch(string searchQuery, CancellationToken cancellationToken = default) {
        var result = await _productService.GetByTextSearch(searchQuery, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return result;
    }
}