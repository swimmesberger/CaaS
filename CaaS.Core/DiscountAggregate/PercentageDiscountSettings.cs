using System.ComponentModel.DataAnnotations;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public record PercentageDiscountSettings() : DiscountParameters(version: 1) {
    
    [Required]
    public decimal? Percentage { get; init; }
}