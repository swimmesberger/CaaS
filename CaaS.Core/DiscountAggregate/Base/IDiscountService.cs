using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base;

public interface IDiscountService {

    Task<IEnumerable<DiscountSettingRaw>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DiscountSettingRaw?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DiscountSettingRaw> AddAsync(DiscountSettingRaw discountSetting, CancellationToken cancellationToken = default);
    Task<DiscountSettingRaw> UpdateAsync(Guid id, DiscountSettingRaw discountSetting, CancellationToken cancellationToken = default);
    Task DeleteDiscountSettingAsync(Guid discountSettingId, CancellationToken cancellationToken = default);
    Task<Cart> ApplyDiscountAsync(Cart cart, CancellationToken cancellationToken = default);
    IEnumerable<DiscountComponentMetadata> GetDiscountMetadata();
}