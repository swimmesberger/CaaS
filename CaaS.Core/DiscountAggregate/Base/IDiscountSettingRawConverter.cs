using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountSettingRawConverter {
    DiscountSetting DeserializeSetting(DiscountSettingRaw rawSetting);
    
    DiscountSettingRaw SerializeSetting(DiscountSetting setting);

    DiscountMetadataSettingRaw SerializeSetting(DiscountSettingMetadata metadata);

    DiscountSettingMetadata DeserializeSetting(DiscountMetadataSettingRaw rawSetting);
}