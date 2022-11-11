using CaaS.Core.Exceptions;
using CaaS.Core.Shop;
using CaaS.Core.Shop.Entities;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Shop.DataModel;

namespace CaaS.Infrastructure.Shop;

public class ShopRepository : CrudRepository<ShopDataModel, Core.Shop.Entities.Shop>, IShopRepository {
    public ShopRepository(IDao<ShopDataModel> shopDao, IDao<ShopAdminDataModel> shopAdminDao) : 
            base(shopDao, new ShopDomainModelConverter(shopAdminDao)) { }

    public async Task<Core.Shop.Entities.Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(ShopDataModel.Name), name), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
}

internal class ShopDomainModelConverter : IDomainModelConverter<ShopDataModel, Core.Shop.Entities.Shop> {
    public IEnumerable<OrderParameter> DefaultOrderParameters { get; } = OrderParameter.From(nameof(ShopDataModel.Name));

    private readonly ShopAdminRepository _shopAdminRepository;

    public ShopDomainModelConverter(IDao<ShopAdminDataModel> shopAdminDao) {
        _shopAdminRepository = new ShopAdminRepository(shopAdminDao);
    }

    public ValueTask<Core.Shop.Entities.Shop> ConvertToDomain(ShopDataModel dataModel) => ConvertToDomain(dataModel, (CancellationToken)default);

    public ShopDataModel ApplyDomainModel(ShopDataModel dataModel, Core.Shop.Entities.Shop domainModel) {
        return dataModel with {
            Name = domainModel.Name
        };
    }
    
    public ShopDataModel ConvertFromDomain(Core.Shop.Entities.Shop domainModel) {
        return new ShopDataModel() {
            Id = domainModel.Id,
            Name = domainModel.Name,
            CartLifetimeMinutes = domainModel.CartLifetimeMinutes,
            AdminId = domainModel.ShopAdmin.Id,
            RowVersion = domainModel.GetRowVersion()
        };
    }

    public async ValueTask<Core.Shop.Entities.Shop> ConvertToDomain(ShopDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<ShopDataModel>() { dataModel }, cancellationToken)).First();
    }

    public async Task<List<Core.Shop.Entities.Shop>> ConvertToDomain(IReadOnlyCollection<ShopDataModel> dataModels,
            CancellationToken cancellationToken = default) {
        var shopAdminIds = dataModels.Select(p => p.AdminId).ToHashSet();
        var shopAdminDomainModels = (await _shopAdminRepository.FindByIdsAsync(shopAdminIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
        return dataModels.Select(dataModel => ConvertToDomain(dataModel, shopAdminDomainModels)).ToList();
    }

    public async Task<IReadOnlyList<Core.Shop.Entities.Shop>> ConvertToDomain(IAsyncEnumerable<ShopDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
        return await dataModels
                .SelectAwait(ConvertToDomain)
                .ToListAsync(cancellationToken);
    }

    private Core.Shop.Entities.Shop ConvertToDomain(ShopDataModel dataModel, IReadOnlyDictionary<Guid, ShopAdmin> shopAdminDomainModels) {
        if (!shopAdminDomainModels.TryGetValue(dataModel.AdminId, out var shopAdmin)) {
            throw new CaasDomainMappingException($"Failed to find shop-admin {dataModel.AdminId} for shop {dataModel.Id}");
        }
        return new Core.Shop.Entities.Shop() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            CartLifetimeMinutes = dataModel.CartLifetimeMinutes,
            ShopAdmin = shopAdmin,
            ConcurrencyToken = dataModel.RowVersion.ToString()
        };
    }
}