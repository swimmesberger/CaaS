using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ShopController : ControllerBase {
    private readonly ILogger<ShopController> _logger;
    private readonly IShopRepository _shopRepository;

    public ShopController(ILogger<ShopController> logger, IShopRepository shopRepository) {
        _logger = logger;
        _shopRepository = shopRepository;
    }
    
    [HttpGet]
    public async Task<IReadOnlyList<Shop>> Get(CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Shop/ -> Get()");
        return await _shopRepository.GetAllAsync(cancellationToken);
    }
}