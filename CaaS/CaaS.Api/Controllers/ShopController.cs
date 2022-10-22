using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers;

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
    public async Task<IReadOnlyList<Shop>> Get(CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Shop/ -> Get()");
        return await _shopRepository.FindAllAsync(cancellationToken);
    }
    
    [HttpGet("name/{name}")]
    public async Task<ActionResult<Shop>> GetByName(string name, CancellationToken cancellationToken = default) {
        _logger.LogInformation("[GET] /Shop/ -> GetByName({Name})", name);
        var shop = await _shopRepository.FindByNameAsync(name, cancellationToken);
        if (shop == null) {
            return NotFound();
        }
        return shop;
    }
    
    [HttpPost("{id:guid}/name/{name}")]
    public async Task<ActionResult<Shop>> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        _logger.LogInformation("[POST] /Shop/ -> SetName({Name})", name);
        await using var uow = _unitOfWorkManager.Begin();
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            return NotFound();
        }
        shop = shop with { Name = name };
        try {
            shop = await _shopRepository.UpdateAsync(shop, cancellationToken);
        } catch (DbUpdateConcurrencyException) {
            return Conflict();
        }
        await uow.CompleteAsync(cancellationToken);
        return shop;
    }
}