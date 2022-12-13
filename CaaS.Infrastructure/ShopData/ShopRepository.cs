using CaaS.Core.Base.Exceptions;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.ShopData;

public class ShopRepository : CrudRepository<ShopDataModel, Shop>, IShopRepository {
    
    public ShopRepository(IDao<ShopDataModel> shopDao, IDao<ShopAdminDataModel> shopAdminDao) : 
            base(shopDao, new ShopDomainModelConverter(shopAdminDao)) { }

    public async Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(ShopDataModel.Name), name), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<int?> FindCartLifetimeByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var parameters = new StatementParameters {
            SelectParameters = new SelectParameters(nameof(ShopDataModel.CartLifetimeMinutes)),
            WhereParameters = new WhereParameters(nameof(ShopDataModel.Id), id)
        };
        return await Dao.FindScalarBy<int>(parameters, cancellationToken).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> VerifyAppKeyAsync(Guid id, string appKey, CancellationToken cancellationToken = default) {
        var parameters = new StatementParameters {
            SelectParameters = SelectParameters.Empty,
            WhereParameters = new WhereParameters(new QueryParameter[] {
                new(nameof(ShopDataModel.Id), id),
                new(nameof(ShopDataModel.AppKey), appKey)
            })
        };
        return Dao.FindScalarBy<int>(parameters, cancellationToken).AnyAsync(cancellationToken).AsTask();
    }
}

internal class ShopDomainModelConverter : IDomainModelConverter<ShopDataModel, Shop> {
    public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(ShopDataModel.Name));

    private readonly ShopAdminRepository _shopAdminRepository;

    public ShopDomainModelConverter(IDao<ShopAdminDataModel> shopAdminDao) {
        _shopAdminRepository = new ShopAdminRepository(shopAdminDao);
    }
    
    public ShopDataModel ApplyDomainModel(ShopDataModel dataModel, Shop domainModel) {
        return dataModel with {
            Name = domainModel.Name,
            CartLifetimeMinutes = domainModel.CartLifetimeMinutes,
            AdminId = domainModel.ShopAdmin.Id,
            AppKey = domainModel.AppKey
        };
    }

    public Shop ApplyDataModel(Shop domainModel, ShopDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public ShopDataModel ConvertFromDomain(Shop domainModel) {
        return new ShopDataModel() {
            Id = domainModel.Id,
            Name = domainModel.Name,
            CartLifetimeMinutes = domainModel.CartLifetimeMinutes,
            AdminId = domainModel.ShopAdmin.Id,
            AppKey = domainModel.AppKey,
            RowVersion = domainModel.GetRowVersion()
        };
    }

    public async ValueTask<Shop> ConvertToDomain(ShopDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<ShopDataModel>() { dataModel }, cancellationToken)).First();
    }

    public async Task<List<Shop>> ConvertToDomain(IReadOnlyCollection<ShopDataModel> dataModels,
            CancellationToken cancellationToken = default) {
        var shopAdminIds = dataModels.Select(p => p.AdminId).ToHashSet();
        var shopAdminDomainModels = (await _shopAdminRepository.FindByIdsAsync(shopAdminIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
        return dataModels.Select(dataModel => ConvertToDomain(dataModel, shopAdminDomainModels)).ToList();
    }

    public async Task<IReadOnlyList<Shop>> ConvertToDomain(IAsyncEnumerable<ShopDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(items, cancellationToken);
    }

    private Shop ConvertToDomain(ShopDataModel dataModel, IReadOnlyDictionary<Guid, ShopAdmin> shopAdminDomainModels) {
        if (!shopAdminDomainModels.TryGetValue(dataModel.AdminId, out var shopAdmin)) {
            throw new CaasDomainMappingException($"Failed to find shop-admin {dataModel.AdminId} for shop {dataModel.Id}");
        }
        return new Shop() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            CartLifetimeMinutes = dataModel.CartLifetimeMinutes,
            ShopAdmin = shopAdmin,
            AppKey = dataModel.AppKey,
            ConcurrencyToken = dataModel.RowVersion.ToString()
        };
    }
}