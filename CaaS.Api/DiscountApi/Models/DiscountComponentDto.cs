using CaaS.Core.DiscountAggregate.Base;

namespace CaaS.Api.DiscountApi.Models; 

public record DiscountComponentDto(Guid Id, DiscountComponentParametersDto Parameters, DiscountComponentType ComponentType);