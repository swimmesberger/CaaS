// ReSharper disable NotAccessedPositionalProperty.Global
namespace CaaS.Api.CustomerApi.Models; 

public record CustomerForUpdateDto(string FirstName, string LastName, string EMail, string TelephoneNumber, string CreditCardNumber, string ConcurrencyToken);