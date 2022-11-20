using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Core.ShopAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ShopApi;

//[Authorize]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiController]
[Route("[controller]")]
[CaasApiConvention]
public class ShopAdministrationController : ControllerBase {
    private readonly IShopService _shopService;

    public ShopAdministrationController(IShopService shopService) {
        _shopService = shopService;
    }

    [HttpGet]
    public async Task<IEnumerable<Shop>> GetAll(CancellationToken cancellationToken = default) {
        var result = await _shopService.GetAll(cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return result;
    }
    
    [HttpGet("name/{name}")]
    public async Task<Shop?> GetByName(string name, CancellationToken cancellationToken = default) {
        return await _shopService.GetByName(name, cancellationToken);
    }
    
    [HttpPost("{id:guid}/name/{name}")]
    public async Task<Shop> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        return await _shopService.SetName(id, name, cancellationToken);
    }
    
    [HttpPost]
    public async Task<Shop> CreateShop(string name, Guid adminId, int cartLifetimeMinutes = 120, CancellationToken cancellationToken = default) {
        return await _shopService.Add(name, adminId, cartLifetimeMinutes, cancellationToken);
    }
}