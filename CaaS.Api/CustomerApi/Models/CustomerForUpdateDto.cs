namespace CaaS.Api.CustomerApi.Models; 

public record CustomerForUpdateDto(string Name, string EMail, string CreditCardNumber, string ConcurrencyToken);