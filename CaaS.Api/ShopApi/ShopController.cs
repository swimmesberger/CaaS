using AutoMapper;
using CaaS.Api.Base;
using CaaS.Api.Base.Attributes;
using CaaS.Api.ShopApi.Models;
using CaaS.Core.ShopAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace CaaS.Api.ShopApi; 

[ApiController]
[Route("[controller]")]
[CaasApiConvention]
public class ShopController : ControllerBase {
    private readonly IShopService _shopService;
    private readonly IMapper _mapper;

    public ShopController(IShopService shopService, IMapper mapper) {
        _shopService = shopService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ShopMinimalDto>> GetAll(CancellationToken cancellationToken = default) {
        var result = await _shopService.GetAllAsync(cancellationToken);
        Response.Headers[HeaderConstants.TotalCount] = new StringValues(result.TotalCount.ToString());
        return _mapper.Map<IEnumerable<ShopMinimalDto>>(result.Items);
    }
}