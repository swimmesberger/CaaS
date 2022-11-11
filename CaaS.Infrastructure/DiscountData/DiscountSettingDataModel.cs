using System.Collections.Immutable;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.DiscountData;

[GenerateMapper]
public record DiscountSettingDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid Rule { get; init; }
    public Guid Action { get; init; }
    
    [JsonColumn]
    public IImmutableDictionary<string, object> RuleParameters { get; init; } = ImmutableDictionary<string, object>.Empty;
    
    [JsonColumn]
    public IImmutableDictionary<string, object> ActionParameters { get; init; } = ImmutableDictionary<string, object>.Empty;
}