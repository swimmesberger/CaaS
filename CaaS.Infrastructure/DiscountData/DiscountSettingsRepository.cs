using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.DiscountData; 

public class DiscountSettingsRepository : CrudRepository<DiscountSettingDataModel, DiscountSetting>, IDiscountSettingRepository {
    public DiscountSettingsRepository(IDao<DiscountSettingDataModel> dao, IDiscountSettingRawConverter rawConverter) : 
        base(dao, new DiscountSettingsConverter(rawConverter)) { }
}

internal class DiscountSettingsConverter : IDomainModelConverter<DiscountSettingDataModel, DiscountSetting> {
    public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(DiscountSettingDataModel.Name));
    
    private readonly IDiscountSettingRawConverter _rawConverter;

    public DiscountSettingsConverter(IDiscountSettingRawConverter rawConverter) {
        _rawConverter = rawConverter;
    }

    public ValueTask<DiscountSetting> ConvertToDomain(DiscountSettingDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<DiscountSetting>(_rawConverter.DeserializeSetting(new DiscountSettingRaw() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            ShopId = dataModel.ShopId,
            Action = new DiscountMetadataSettingRaw(dataModel.ActionId, dataModel.ActionParameters),
            Rule = new DiscountMetadataSettingRaw(dataModel.RuleId, dataModel.RuleParameters),
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        }));
    }
    
    public async Task<IReadOnlyList<DiscountSetting>> ConvertToDomain(IAsyncEnumerable<DiscountSettingDataModel> dataModels, CancellationToken cancellationToken = default) {
        return await dataModels.SelectAwaitWithCancellation(ConvertToDomain).ToListAsync(cancellationToken);
    }

    public DiscountSettingDataModel ApplyDomainModel(DiscountSettingDataModel dataModel, DiscountSetting domainModel) {
        var rule = _rawConverter.SerializeSetting(domainModel.Rule);
        var action = _rawConverter.SerializeSetting(domainModel.Action);
        return dataModel with {
            Id = domainModel.Id,
            Name = domainModel.Name,
            ShopId = domainModel.ShopId,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = rule.Parameters,
            ActionParameters = action.Parameters,
            RowVersion = domainModel.GetRowVersion()
        };
    }

    public DiscountSetting ApplyDataModel(DiscountSetting domainModel, DiscountSettingDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public DiscountSettingDataModel ConvertFromDomain(DiscountSetting domainModel) {
        var rule = _rawConverter.SerializeSetting(domainModel.Rule);
        var action = _rawConverter.SerializeSetting(domainModel.Action);
        return new DiscountSettingDataModel() {
            Id = domainModel.Id,
            ShopId = domainModel.ShopId,
            Name = domainModel.Name,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = rule.Parameters,
            ActionParameters = action.Parameters
        };
    }
}