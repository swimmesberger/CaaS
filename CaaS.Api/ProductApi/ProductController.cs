using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Api.BlobApi;
using CaaS.Api.ProductApi.Models;
using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ProductApi; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
[CaasApiConvention]
public class ProductController : ControllerBase {
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper) {
        _productService = productService;
        _mapper = mapper;
    }
    
    [HttpGet("{productId:guid}")]
    public async Task<ProductDetailDto?> GetById(Guid productId, CancellationToken cancellationToken = default) {
        var result = await _productService.GetByIdAsync(productId, cancellationToken);
        return _mapper.Map<ProductDetailDto?>(result);
    }
    
    [HttpGet]
    public async Task<PagedResult<ProductMinimalDto>> GetByTextSearch([FromQuery(Name = "q")] string? searchQuery, 
        [FromQuery] KeysetPaginationDirection paginationDirection = KeysetPaginationDirection.Forward,
        [FromQuery(Name = "$skiptoken")] string? skipToken = null, [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default) {
        var paginationToken = new PaginationToken(paginationDirection, skipToken, limit);
        var result = await _productService.GetByTextSearchAsync(searchQuery, paginationToken, cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        var viewProducts = _mapper.Map<IReadOnlyCollection<ProductMinimalDto>>(result.Items);
        return result.WithItems(viewProducts);
    }
}