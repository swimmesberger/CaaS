using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.ShopData; 

public class ShopAdminRepository : IShopAdminRepository {
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

    public async Task<ShopAdmin?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindBy(StatementParameters.CreateWhere(nameof(ShopAdminDataModel.EMail), email), cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<ShopAdmin> AddAsync(ShopAdmin entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.AddAsync(dataModel, cancellationToken);
        entity = Converter.ApplyDataModel(entity, dataModel);
        return entity;
    }

    public async Task<ShopAdmin> UpdateAsync(ShopAdmin oldEntity, ShopAdmin newEntity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(newEntity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        newEntity = Converter.ApplyDataModel(newEntity, dataModel);
        return newEntity;
    }

    public async Task DeleteAsync(ShopAdmin entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }

    private class ShopAdminDomainModelConverter : IDomainReadModelConverter<ShopAdminDataModel, ShopAdmin> {
        public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(ShopAdminDataModel.Name));

        public ValueTask<ShopAdmin> ConvertToDomain(ShopAdminDataModel dataModel, CancellationToken cancellationToken) {
            return new ValueTask<ShopAdmin>(new ShopAdmin() {
                Id = dataModel.Id,
                EMail = dataModel.EMail,
                Name = dataModel.Name,
                ShopId = dataModel.ShopId,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        
        public ShopAdminDataModel ConvertFromDomain(ShopAdmin domainModel) {
            return new ShopAdminDataModel() {
                Id = domainModel.Id,
                Name = domainModel.Name,
                EMail = domainModel.EMail,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion()
            };
        }
        
        public ShopAdmin ApplyDataModel(ShopAdmin domainModel, ShopAdminDataModel dataModel) {
            return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
        }

        public async Task<IReadOnlyList<ShopAdmin>> ConvertToDomain(IAsyncEnumerable<ShopAdminDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            return await dataModels
                .SelectAwaitWithCancellation(ConvertToDomain)
                .ToListAsync(cancellationToken);
        }
    }
}