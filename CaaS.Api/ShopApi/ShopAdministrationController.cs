using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ShopApi.Models;
using CaaS.Core.ShopAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ShopApi;

//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
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
        var result = await _shopService.GetAllAsync(cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<ShopDto>>(result);
    }
    
    [HttpGet("name/{name}")]
    public async Task<ShopDto?> GetByName(string name, CancellationToken cancellationToken = default) {
        Shop? shop = await _shopService.GetByNameAsync(name, cancellationToken);
        return _mapper.Map<ShopDto>(shop);
    }
    
    [HttpGet("adminId/{adminId:guid}")]
    public async Task<ShopDto?> GetByAdminId(Guid adminId, CancellationToken cancellationToken = default) {
        Shop? shop = await _shopService.GetByAdminIdAsync(adminId, cancellationToken);
        return _mapper.Map<ShopDto>(shop);
    }
    
    [HttpGet("{shopId:guid}")]
    public async Task<ShopDto?> GetById(Guid id, CancellationToken cancellationToken = default) {
        Shop? shop = await _shopService.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<ShopDto>(shop);
    }
    
    [HttpPost("{id:guid}/name/{name}")]
    public async Task<ActionResult> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        await _shopService.SetNameAsync(id, name, cancellationToken);
        return NoContent();
    }
    
    [HttpPost]
    public async Task<ActionResult> Add([FromBody] ShopForCreationDto shopDto, CancellationToken cancellationToken = default) {
        var shop = _mapper.Map<Shop>(shopDto);
        var result = await _shopService.AddAsync(shop, cancellationToken);

        return CreatedAtAction(
            controllerName: "ShopAdministration",
            actionName: nameof(GetById),
            routeValues: new { shopId = result.Id },
            value: _mapper.Map<ShopDto>(result));
    }
        
    [HttpPut("{shopId:guid}")]
    public async Task<ShopDto> UpdateCoupon(Guid shopId, [FromBody] ShopForUpdateDto couponDto, CancellationToken cancellationToken = default) {
        var updatedCoupon = _mapper.Map<Shop>(couponDto);
        var result = await _shopService.UpdateAsync(shopId, updatedCoupon, cancellationToken);
        return _mapper.Map<ShopDto>(result);
    }
    
    [HttpDelete("{shopId:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid shopId, CancellationToken cancellationToken = default) {
        await _shopService.DeleteAsync(shopId, cancellationToken);
        return NoContent();
    }
}