using System.ComponentModel.DataAnnotations;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public record FixedValueDiscountSettings() : DiscountParameters(1, "fixedValueDiscountAction") {
    [Required]
    public decimal? Value { get; init; }
}