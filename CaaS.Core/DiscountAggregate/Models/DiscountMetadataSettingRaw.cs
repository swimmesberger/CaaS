using System.Text.Json;
using CaaS.Core.Base;

namespace CaaS.Core.DiscountAggregate.Models;

public record DiscountMetadataSettingRaw {
    public static readonly DiscountMetadataSettingRaw Empty = new DiscountMetadataSettingRaw();
    
    public Guid Id { get; init; }
    public JsonElement Parameters { get; init; }

    public DiscountMetadataSettingRaw() { }
    
    public DiscountMetadataSettingRaw(Guid id, JsonElement parameters) {
        Id = id;
        Parameters = parameters;
    }
}