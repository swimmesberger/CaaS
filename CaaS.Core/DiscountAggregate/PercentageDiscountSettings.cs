using System.ComponentModel.DataAnnotations;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public record PercentageDiscountSettings() : DiscountParameters(version: 1) {
    
    [Required]
    [Range(0.0, 1.0)]
    public decimal? Percentage { get; init; }
}