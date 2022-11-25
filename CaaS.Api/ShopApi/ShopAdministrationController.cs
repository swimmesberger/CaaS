using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ShopApi.Models;
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
    private readonly IMapper _mapper;

    public ShopAdministrationController(IShopService shopService, IMapper mapper) {
        _shopService = shopService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ShopDto>> GetAll(CancellationToken cancellationToken = default) {
        var result = await _shopService.GetAll(cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<ShopDto>>(result);
    }
    
    [HttpGet("name/{name}")]
    public async Task<ShopDto?> GetByName(string name, CancellationToken cancellationToken = default) {
        Shop? shop = await _shopService.GetByName(name, cancellationToken);
        return _mapper.Map<ShopDto>(shop);
    }
    
    [HttpPost("{id:guid}/name/{name}")]
    public async Task<ActionResult> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        await _shopService.SetName(id, name, cancellationToken);
        return NoContent();
    }
    
    [HttpPost]
    public async Task<ShopDto> CreateShop([FromBody] ShopForCreationDto shopDto, CancellationToken cancellationToken = default) {
        Shop shop = _mapper.Map<Shop>(shopDto);
        
        Shop result = await _shopService.Add(shop, cancellationToken);
        return _mapper.Map<ShopDto>(result);
    }
}