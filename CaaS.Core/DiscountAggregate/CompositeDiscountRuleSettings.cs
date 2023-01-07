using System.Collections.Immutable;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate; 

// ReSharper disable once ClassNeverInstantiated.Global
public record CompositeDiscountRuleSettings() : DiscountParameters(1, "compositeDiscountRule") {
    public IReadOnlyList<DiscountSettingMetadata> DiscountSettings { get; init; } = ImmutableArray<DiscountSettingMetadata>.Empty;
    public DiscountCombinationType CombinationType { get; init; } = DiscountCombinationType.And;
}