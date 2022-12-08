using System.Text.Json.Nodes;

namespace CaaS.Api.DiscountApi.Models; 

public record DiscountSettingForCreationDto(Guid Id, string Name, DiscountMetadataSettingDto Rule, DiscountMetadataSettingDto Action);

public record DiscountMetadataSettingDto(Guid Id, JsonObject Parameters);