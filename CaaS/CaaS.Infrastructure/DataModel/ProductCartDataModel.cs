using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record ProductCartDataModel() : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid ProductId { get; init; }
    public Guid CartId { get; init; }
    public int Amount { get; init; }
}