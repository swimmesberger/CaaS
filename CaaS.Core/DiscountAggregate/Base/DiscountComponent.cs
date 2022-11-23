namespace CaaS.Core.DiscountAggregate.Base; 

public record DiscountComponent(IDiscountRule Rule, IDiscountAction Action);