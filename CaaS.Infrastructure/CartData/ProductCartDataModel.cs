using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.CartData;

[GenerateMapper]
public record ProductCartDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid ProductId { get; init; }
    public Guid CartId { get; init; }
    public int Amount { get; init; }
}