using CaaS.Api.CustomerApi.Models;
using CaaS.Core.OrderAggregate;

namespace CaaS.Api.OrderApi.Models; 

public record OrderForCreationDto(Guid CustomerId, Address BillingAddress);