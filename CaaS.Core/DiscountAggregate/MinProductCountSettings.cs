using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public record MinProductCountSettings : DiscountParameters {
    public Guid ProductId { get; init; }
    public int NumberOfItems { get; init; }
}