using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base;

public interface IDiscountService {

    Task<DiscountSettingRaw> AddDiscountSettingAsync(DiscountSettingRaw discountSetting, CancellationToken cancellationToken = default);

    Task<DiscountSettingRaw> UpdateDiscountSettingAsync(DiscountSettingRaw discountSetting, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<DiscountSettingRaw>> GetAllDiscountSettingsAsync(CancellationToken cancellationToken = default);

    Task<DiscountSettingRaw> GetDiscountSettingByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteDiscountSettingAsync(Guid discountSettingId, CancellationToken cancellationToken = default);
    
    Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default);
    
    IEnumerable<DiscountComponentMetadata> GetDiscountMetadata();
}