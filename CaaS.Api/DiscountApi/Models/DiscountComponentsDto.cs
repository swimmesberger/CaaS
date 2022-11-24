namespace CaaS.Api.DiscountApi.Models; 

public record DiscountComponentsDto(IEnumerable<DiscountComponentDto> Rules, IEnumerable<DiscountComponentDto> Actions);