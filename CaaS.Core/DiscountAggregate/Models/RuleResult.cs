using System.Collections.Immutable;

namespace CaaS.Core.DiscountAggregate.Models;

public record RuleResult(RuleResultType Type = RuleResultType.None) {
    public static readonly RuleResult NotApplicable = new RuleResult();
    public static readonly RuleResult Applicable = new RuleResult(RuleResultType.Applicable);
    
    public IReadOnlySet<Guid> AffectedItemIds { get; init; } = ImmutableHashSet<Guid>.Empty;
    public bool HasAffectedItems => AffectedItemIds.Any();

    public RuleResult Add(RuleResult result) {
        var type = result.Type == RuleResultType.Applicable || Type == RuleResultType.Applicable ? 
            RuleResultType.Applicable : 
            RuleResultType.None;
        return new RuleResult(type) {
            AffectedItemIds = AffectedItemIds.Concat(result.AffectedItemIds).ToHashSet()
        };
    }
}

public enum RuleResultType {
    None,
    Applicable
}