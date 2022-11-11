using CaaS.Core.Shop.Entities;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Shop.DataModel;

namespace CaaS.Infrastructure.Shop; 

internal class ShopAdminRepository {
    private IDao<ShopAdminDataModel> Dao { get; }
    private ShopAdminDomainModelConvert Converter { get; }

    public ShopAdminRepository(IDao<ShopAdminDataModel> dao) {
        Dao = dao;
        Converter = new ShopAdminDomainModelConvert();
    }

    public async Task<IReadOnlyList<ShopAdmin>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    private class ShopAdminDomainModelConvert : IDomainReadModelConverter<ShopAdminDataModel, ShopAdmin> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(ShopAdminDataModel.Name));
        
        public ValueTask<ShopAdmin> ConvertToDomain(ShopAdminDataModel dataModel) => ConvertToDomain(dataModel, default);

        public ValueTask<ShopAdmin> ConvertToDomain(ShopAdminDataModel dataModel, CancellationToken cancellationToken) {
            return new ValueTask<ShopAdmin>(new ShopAdmin() {
                Id = dataModel.Id,
                EMail = dataModel.EMail,
                Name = dataModel.Name,
                ShopId = dataModel.ShopId,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        
        public async Task<IReadOnlyList<ShopAdmin>> ConvertToDomain(IAsyncEnumerable<ShopAdminDataModel> dataModels, CancellationToken cancellationToken = default) {
            return await dataModels
                    .SelectAwait(ConvertToDomain)
                    .ToListAsync(cancellationToken);
        }
    }
}