namespace CaaS.Api.CustomerApi.Models; 

public record CustomerForCreationDto(Guid Id, string Name, string EMail, string CreditCardNumber);