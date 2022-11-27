using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountComponentFactory : IDiscountComponentProvider {
    DiscountComponent CreateComponent(DiscountSetting discountSetting);
    
    IDiscountRule CreateRule(DiscountSettingMetadata settingMetadata);

    IDiscountAction CreateAction(DiscountSettingMetadata settingMetadata);
}