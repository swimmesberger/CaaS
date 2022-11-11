using System.Collections.Immutable;
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record DiscountSettingDataModel : Base.DataModel {
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