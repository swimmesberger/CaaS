namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountOptions<out TOptions> where TOptions : class {
    TOptions Value { get; }
}