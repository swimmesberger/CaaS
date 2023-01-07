using CaaS.Core.DiscountAggregate.Models;


namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
public record TimeWindowDiscountSettings() : DiscountParameters(1, "timeWindowDiscountRule") {
    public DateTimeOffset FromTime { get; init; }
    public DateTimeOffset ToTime { get; init; }
}