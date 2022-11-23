namespace CaaS.Core.DiscountAggregate.Base; 

public record DiscountComponentMetadata(Guid Id, Type ServiceType, Type SettingsType, DiscountComponentType ComponentType);