using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountSettingRepository {
    
    Task<IReadOnlyList<DiscountSetting>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<DiscountSetting>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    
    Task<DiscountSetting?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DiscountSetting> AddAsync(DiscountSetting entity, CancellationToken cancellationToken = default);

    Task<DiscountSetting> UpdateAsync(DiscountSetting oldEntity, DiscountSetting newEntity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(DiscountSetting entity, CancellationToken cancellationToken = default);
}