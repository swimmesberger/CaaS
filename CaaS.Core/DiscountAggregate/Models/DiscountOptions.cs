using CaaS.Core.DiscountAggregate.Base;

namespace CaaS.Core.DiscountAggregate.Models; 

public class DiscountOptions<T> : IDiscountOptions<T> where T : class {
    public T Value { get; }
    
    public DiscountOptions(T value) {
        Value = value;
    }
}