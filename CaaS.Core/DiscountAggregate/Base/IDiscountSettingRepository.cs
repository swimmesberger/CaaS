using CaaS.Core.DiscountAggregate.Models;

namespace CaaS.Core.DiscountAggregate.Base; 

public interface IDiscountSettingRepository {
    Task<IReadOnlyList<DiscountSetting>> FindAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<DiscountSetting>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}