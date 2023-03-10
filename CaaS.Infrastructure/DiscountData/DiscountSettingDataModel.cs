using System.Text.Json;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.DiscountData;

// ReSharper disable UnusedAutoPropertyAccessor.Global
[GenerateMapper]
public record DiscountSettingDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;

    public Guid RuleId { get; init; }
    public Guid ActionId { get; init; }
    
    [JsonColumn]
    public JsonElement RuleParameters { get; init; }
    
    [JsonColumn]
    public JsonElement ActionParameters { get; init; }
}