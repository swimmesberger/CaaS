using CaaS.Api.Base;
using CaaS.Api.Base.Swagger;
using CaaS.Core.Base;
using CaaS.Core.Exceptions;
using CaaS.Core.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.Shop;

//[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ShopController : ControllerBase {
    private readonly ILogger<ShopController> _logger;
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ShopController(ILogger<ShopController> logger, IShopRepository shopRepository, IUnitOfWorkManager unitOfWorkManager) {
        _logger = logger;
        _shopRepository = shopRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    [HttpGet]
    [ProducesTotalCountHeader]
    public async Task<IReadOnlyList<Core.Shop.Entities.Shop>> Get(CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Shop/ -> Get()");
        var shopCount = await _shopRepository.CountAsync(cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(shopCount.ToString());
        return await _shopRepository.FindAllAsync(cancellationToken);
    }
    
    [HttpGet("name/{name}")]
    public async Task<ActionResult<Core.Shop.Entities.Shop>> GetByName(string name, CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Shop/ -> GetByName({Name})", name);
        var shop = await _shopRepository.FindByNameAsync(name, cancellationToken);
        if (shop == null) {
            return NotFound();
        }
        return shop;
    }
    
    [HttpPost("{id:guid}/name/{name}")]
    public async Task<ActionResult<Core.Shop.Entities.Shop>> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        _logger.LogInformation("[POST] /Shop/ -> SetName({Name})", name);
        await using var uow = _unitOfWorkManager.Begin();
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            return NotFound();
        }
        shop = shop with { Name = name };
        try {
            shop = await _shopRepository.UpdateAsync(shop, cancellationToken);
        } catch (CaasUpdateConcurrencyDbException) {
            return Conflict();
        }
        await uow.CompleteAsync(cancellationToken);
        
        return shop;
    }
}