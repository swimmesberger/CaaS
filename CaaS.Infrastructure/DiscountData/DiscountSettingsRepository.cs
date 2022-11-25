using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.DiscountData; 

public class DiscountSettingsRepository : CrudRepository<DiscountSettingDataModel, DiscountSetting>, IDiscountSettingRepository {
    public DiscountSettingsRepository(IDao<DiscountSettingDataModel> dao, DiscountSettingsJsonConverter jsonConverter) : 
        base(dao, new DiscountSettingsConverter(jsonConverter)) { }
}

internal class DiscountSettingsConverter : IDomainModelConverter<DiscountSettingDataModel, DiscountSetting> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(DiscountSettingDataModel.Name));
    
    private readonly DiscountSettingsJsonConverter _settingsJsonConverter;

    public DiscountSettingsConverter(DiscountSettingsJsonConverter settingsJsonConverter) {
        _settingsJsonConverter = settingsJsonConverter;
    }

    public ValueTask<DiscountSetting> ConvertToDomain(DiscountSettingDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<DiscountSetting>(new DiscountSetting() {
            Id = dataModel.Id,
            Action = _settingsJsonConverter.DeserializeSettings(dataModel.ActionId, dataModel.ActionParameters),
            Rule = _settingsJsonConverter.DeserializeSettings(dataModel.RuleId, dataModel.RuleParameters)
        });
    }
    
    public async Task<IReadOnlyList<DiscountSetting>> ConvertToDomain(IAsyncEnumerable<DiscountSettingDataModel> dataModels, CancellationToken cancellationToken = default) {
        return await dataModels.SelectAwaitWithCancellation(ConvertToDomain).ToListAsync(cancellationToken);
    }

    public DiscountSettingDataModel ApplyDomainModel(DiscountSettingDataModel dataModel, DiscountSetting domainModel) {
        return dataModel with {
            Name = domainModel.Name,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = _settingsJsonConverter.SerializeSettings(domainModel.Rule),
            ActionParameters = _settingsJsonConverter.SerializeSettings(domainModel.Action)
        };
    }

    public DiscountSetting ApplyDataModel(DiscountSetting domainModel, DiscountSettingDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public DiscountSettingDataModel ConvertFromDomain(DiscountSetting domainModel) {
        return new DiscountSettingDataModel() {
            Id = domainModel.Id,
            ShopId = domainModel.Shop.Id,
            Name = domainModel.Name,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = _settingsJsonConverter.SerializeSettings(domainModel.Rule),
            ActionParameters = _settingsJsonConverter.SerializeSettings(domainModel.Action)
        };
    }
}