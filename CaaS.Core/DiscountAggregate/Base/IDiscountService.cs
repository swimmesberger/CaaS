using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base;

public interface IDiscountService {

    Task<DiscountSetting> AddDiscountSettingAsync(DiscountSetting discountSetting, CancellationToken cancellationToken = default);

    Task<DiscountSetting> UpdateDiscountSettingAsync(Guid discountSettingId, DiscountSetting updatedDiscountSetting,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<DiscountSetting>> GetAllDiscountSettingsAsync(CancellationToken cancellationToken = default);

    Task DeleteDiscountSettingAsync(Guid discountSettingId, CancellationToken cancellationToken = default);
    
    Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default);
    
    IEnumerable<DiscountComponentMetadata> GetDiscountMetadata();
}