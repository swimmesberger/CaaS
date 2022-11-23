using System.Collections.Immutable;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public record AndDiscountActionSettings() : DiscountParameters(version: 1) {
    public IReadOnlyList<Guid> DiscountSettingIds { get; init; } = ImmutableArray<Guid>.Empty;
}