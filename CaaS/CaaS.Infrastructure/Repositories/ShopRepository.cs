using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories;

public class ShopRepository : AbstractRepository<ShopDataModel, Shop>, IShopRepository {
    public ShopRepository(IDao<ShopDataModel> shopDao) : base(shopDao) { }

    public async Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(Shop.Name), name), cancellationToken)
                .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await ConvertToDomain(dataModel, cancellationToken);
    }

    protected override StatementParameters PreProcessFindManyParameters(StatementParameters parameters) {
        return parameters.WithOrderBy(nameof(Shop.Name));
    }

    protected override ShopDataModel ApplyDomainModel(ShopDataModel dataModel, Shop domainModel) {
        return dataModel with {
            Name = domainModel.Name
        };
    }
    
    protected override ShopDataModel ConvertFromDomain(Shop domainModel) {
        return new ShopDataModel() {
            Id = domainModel.Id,
            Name = domainModel.Name,
            RowVersion = domainModel.GetRowVersion()
        };
    }

    protected override ValueTask<Shop> ConvertToDomain(ShopDataModel dataModel, CancellationToken cancellationToken) {
        return ValueTask.FromResult(new Shop() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            ConcurrencyToken = dataModel.RowVersion.ToString()
        });
    }

    protected override async Task<List<Shop>> ConvertToDomain(IAsyncEnumerable<ShopDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
        return await dataModels
                .SelectAwait(ConvertToDomain)
                .ToListAsync(cancellationToken);
    }
}