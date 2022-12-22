using System.ComponentModel.DataAnnotations;

namespace CaaS.Api.ShopApi.Models; 

public record ShopForUpdateDto(string Name, [Required] int? CartLifetimeMinutes, Guid ShopAdminId,  string AppKey, string ConcurrencyToken);