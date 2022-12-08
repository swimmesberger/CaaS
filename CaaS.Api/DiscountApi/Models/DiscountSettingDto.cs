namespace CaaS.Api.DiscountApi.Models; 

public record DiscountSettingDto(Guid Id, string Name, DiscountMetadataSettingDto Rule, DiscountMetadataSettingDto Action, Guid ShopId, string ConcurrencyToken);