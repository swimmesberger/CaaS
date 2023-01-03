using System.ComponentModel.DataAnnotations;

namespace CaaS.Api.ProductApi.Models; 

public record ProductForUpdateDto(
    [Required] string? Name, 
    [Required] string? Description, 
    [Required] decimal? Price, 
    string ConcurrencyToken
);