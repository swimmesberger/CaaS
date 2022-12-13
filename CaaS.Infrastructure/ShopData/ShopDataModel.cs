using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.ShopData;

[GenerateMapper]
public record ShopDataModel : DataModel {
    public string Name { get; init; } = string.Empty;
    public int CartLifetimeMinutes { get; init; } = Core.ShopAggregate.Shop.DefaultCartLifetimeMinutes;
    public Guid AdminId { get; init; }
    public string AppKey { get; init; } = string.Empty;
}