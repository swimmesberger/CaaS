namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountComponentProvider {
    IEnumerable<DiscountComponentMetadata> GetDiscountMetadata();
}