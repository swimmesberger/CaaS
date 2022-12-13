namespace CaaS.Api.CouponApi.Models; 

public record CouponForUpdateDto(decimal Value, Guid? OrderId, Guid? CartId, Guid? CustomerId, string ConcurrencyToken);