using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountComponentFactory {
    DiscountComponent CreateComponent(DiscountSetting discountSetting);
    
    IEnumerable<DiscountComponentMetadata> GetDiscountMetadata();
}