using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ProductApi; 

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
    public async Task<PagedResult<Product>> GetByTextSearch([FromQuery] string searchQuery, 
        [FromQuery] KeysetPaginationDirection paginationDirection = KeysetPaginationDirection.Forward,
        [FromQuery(Name = "$skiptoken")] string? skipToken = null,
        CancellationToken cancellationToken = default) {
        var paginationToken = new PaginationToken(paginationDirection, skipToken);
        var result = await _productService.GetByTextSearch(searchQuery, paginationToken, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return result;
    }
}