using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.ShopData; 

internal class ShopAdminRepository : IShopAdminRepository {
    private IDao<ShopAdminDataModel> Dao { get; }
    private ShopAdminDomainModelConverter Converter { get; }

    public ShopAdminRepository(IDao<ShopAdminDataModel> dao) {
        Dao = dao;
        Converter = new ShopAdminDomainModelConverter();
    }
    
    public async Task<IReadOnlyList<ShopAdmin>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    public async Task<ShopAdmin?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    private class ShopAdminDomainModelConverter : IDomainReadModelConverter<ShopAdminDataModel, ShopAdmin> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(ShopAdminDataModel.Name));

        public ValueTask<ShopAdmin> ConvertToDomain(ShopAdminDataModel dataModel, CancellationToken cancellationToken) {
            return new ValueTask<ShopAdmin>(new ShopAdmin() {
                Id = dataModel.Id,
                EMail = dataModel.EMail,
                Name = dataModel.Name,
                ShopId = dataModel.ShopId,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }

        public async Task<IReadOnlyList<ShopAdmin>> ConvertToDomain(IAsyncEnumerable<ShopAdminDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            return await dataModels
                .SelectAwaitWithCancellation(ConvertToDomain)
                .ToListAsync(cancellationToken);
        }
    }
}