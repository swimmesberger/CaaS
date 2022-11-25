using CaaS.Core.Base.Exceptions;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
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
}

internal class ShopDomainModelConverter : IDomainModelConverter<ShopDataModel, Shop> {
    public IEnumerable<OrderParameter> DefaultOrderParameters { get; } = OrderParameter.From(nameof(ShopDataModel.Name));

    private readonly ShopAdminRepository _shopAdminRepository;

    public ShopDomainModelConverter(IDao<ShopAdminDataModel> shopAdminDao) {
        _shopAdminRepository = new ShopAdminRepository(shopAdminDao);
    }
    
    public ShopDataModel ApplyDomainModel(ShopDataModel dataModel, Shop domainModel) {
        return dataModel with {
            Name = domainModel.Name
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
        return await dataModels
                .SelectAwaitWithCancellation(ConvertToDomain)
                .ToListAsync(cancellationToken);
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
            ConcurrencyToken = dataModel.RowVersion.ToString()
        };
    }
}