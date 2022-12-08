using System.Text.Json;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;
using Microsoft.Extensions.Options;

namespace CaaS.Infrastructure.DiscountData; 

public class DiscountSettingsRepository : CrudRepository<DiscountSettingDataModel, DiscountSetting>, IDiscountSettingRepository {
    public DiscountSettingsRepository(IDao<DiscountSettingDataModel> dao, IOptions<DiscountJsonOptions> jsonOptions) : 
        base(dao, new DiscountSettingsConverter(jsonOptions)) { }
}

internal class DiscountSettingsConverter : IDomainModelConverter<DiscountSettingDataModel, DiscountSetting> {
    public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(DiscountSettingDataModel.Name));
    
    private readonly JsonSerializerOptions _jsonOptions;

    public DiscountSettingsConverter(IOptions<DiscountJsonOptions> jsonOptions) {
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public ValueTask<DiscountSetting> ConvertToDomain(DiscountSettingDataModel dataModel, CancellationToken cancellationToken) {
        return new ValueTask<DiscountSetting>(new DiscountSetting() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            ShopId = dataModel.ShopId,
            Action = dataModel.DeserializeAction(_jsonOptions),
            Rule = dataModel.DeserializeRule(_jsonOptions),
        });
    }
    
    public async Task<IReadOnlyList<DiscountSetting>> ConvertToDomain(IAsyncEnumerable<DiscountSettingDataModel> dataModels, CancellationToken cancellationToken = default) {
        return await dataModels.SelectAwaitWithCancellation(ConvertToDomain).ToListAsync(cancellationToken);
    }

    public DiscountSettingDataModel ApplyDomainModel(DiscountSettingDataModel dataModel, DiscountSetting domainModel) {
        return dataModel with {
            Id = domainModel.Id,
            Name = domainModel.Name,
            ShopId = domainModel.ShopId,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = domainModel.SerializeRule(_jsonOptions),
            ActionParameters = domainModel.SerializeAction(_jsonOptions)
        };
    }

    public DiscountSetting ApplyDataModel(DiscountSetting domainModel, DiscountSettingDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public DiscountSettingDataModel ConvertFromDomain(DiscountSetting domainModel) {
        return new DiscountSettingDataModel() {
            Id = domainModel.Id,
            ShopId = domainModel.ShopId,
            Name = domainModel.Name,
            RuleId = domainModel.Rule.Id,
            ActionId = domainModel.Action.Id,
            RuleParameters = domainModel.SerializeRule(_jsonOptions),
            ActionParameters = domainModel.SerializeAction(_jsonOptions)
        };
    }
}