using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Cart.DataModel;

[GenerateMapper]
public record ProductCartDataModel : Base.DataModel.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid ProductId { get; init; }
    public Guid CartId { get; init; }
    public int Amount { get; init; }
}