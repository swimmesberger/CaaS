using CaaS.Api.Base;
using CaaS.Api.Base.Middleware;
using CaaS.Api.Base.Swagger;
using CaaS.Core.Base;
using CaaS.Core.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.Product; 

[ApiController]
[Route("[controller]")]
[RequireTenant]
public class ProductController : ControllerBase {
    private readonly ILogger<ProductController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ProductController(ILogger<ProductController> logger, IProductRepository productRepository, 
            IUnitOfWorkManager unitOfWorkManager) {
        _logger = logger;
        _productRepository = productRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }
    
    [HttpGet]
    [ProducesTotalCountHeader]
    public async Task<IReadOnlyList<Core.Product.Entities.Product>> Get(CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Product/ -> Get()");
        Response.Headers[HeaderConstants.TotalCount] = new StringValues("0");
        return await _productRepository.FindAllAsync(cancellationToken);
    }
}