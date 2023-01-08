using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ShopApi.Models;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ShopAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ShopApi;

// Authorize for shop administration is not really a good idea because it is not shop specific
// This should have OAuth authentication when implemented
// [Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
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
        return _mapper.Map<IEnumerable<ShopDto>>(result.Items);
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
    
    [HttpGet("adminMail/{adminMail}")]
    // verify app-key
    [Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
    [RequireTenant]
    public async Task<ActionResult<ShopDto?>> GetByAdminMail(string adminMail, [FromServices] ITenantIdAccessor tenantIdAccessor, 
        CancellationToken cancellationToken = default) {
        var tenantShop = await _shopService.GetByIdAsync(tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (!adminMail.Equals(tenantShop?.ShopAdmin.EMail)) {
            return NotFound();
        }
        return _mapper.Map<ShopDto>(tenantShop);
    }

    [HttpGet("{shopId:guid}")]
    public async Task<ShopDto?> GetById(Guid shopId, CancellationToken cancellationToken = default) {
        Shop? shop = await _shopService.GetByIdAsync(shopId, cancellationToken);
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
    public async Task<ShopDto> Update(Guid shopId, [FromBody] ShopForUpdateDto shopDto, CancellationToken cancellationToken = default) {
        var updatedShop = _mapper.Map<Shop>(shopDto);
        updatedShop = updatedShop with { Id = shopId };
        var result = await _shopService.UpdateAsync(updatedShop, cancellationToken);
        return _mapper.Map<ShopDto>(result);
    }
    
    [HttpDelete("{shopId:guid}")]
    public async Task<ActionResult> Delete(Guid shopId, CancellationToken cancellationToken = default) {
        await _shopService.DeleteAsync(shopId, cancellationToken);
        return NoContent();
    }
}