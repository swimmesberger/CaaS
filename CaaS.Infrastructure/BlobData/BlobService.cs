using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.BlobAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Di;

namespace CaaS.Infrastructure.BlobData; 

public sealed class BlobService : IBlobService {
    private readonly IDao<BlobDataModel> _dao;
    private readonly ITenantIdAccessor _tenantService;

    public BlobService(IDao<BlobDataModel> dao, ITenantIdAccessor tenantService) {
        _dao = dao;
        _tenantService = tenantService;
    }
    
    public async Task<IBlobItem?> GetAsync(string path, CancellationToken cancellationToken = default) {
        var dao = _dao;
        if (!_tenantService.HasTenantId() && dao is GenericDao<BlobDataModel> genericDao) {
            // all paths should be unique and to allow access to the files without providing a header value we disable tenantId preProcessing
            // = no WHERE shop_id = tenantId in query
            dao = genericDao.WithOptions(new GenericDaoOptions() { IgnoreTenantId = true });
        }
        return ItemFromDataModel(await GetModelAsync(dao, path, cancellationToken));
    }
    
    public async Task AddOrUpdateAsync(IBlobItem blobItem, CancellationToken cancellationToken = default) {
        var existingItem = await GetModelAsync(_dao, blobItem.Path, cancellationToken);
        if (existingItem == null) {
            await _dao.AddAsync(DataModelFromItem(blobItem), cancellationToken);
        } else {
            await _dao.UpdateAsync(DataModelFromItem(existingItem, blobItem), cancellationToken);
        }
    }

    private async Task<BlobDataModel?> GetModelAsync(IDao<BlobDataModel> dao, string path, CancellationToken cancellationToken = default) {
        return await dao.FindBy(new StatementParameters() {
            Where = new QueryParameter[] { new(nameof(BlobDataModel.Path), path) }
        }, cancellationToken).FirstOrDefaultAsync(cancellationToken);
    }

    private static BlobItem? ItemFromDataModel(BlobDataModel? dataModel) {
        if (dataModel == null) return null;
        return new BlobItem() {
            Version = dataModel.RowVersion,
            CreationTime = dataModel.CreationTime,
            LastModificationTime = dataModel.LastModificationTime,
            Name = dataModel.Name,
            Path = dataModel.Path,
            MimeType = dataModel.MimeType,
            Blob = dataModel.Blob
        };
    }

    private BlobDataModel DataModelFromItem(IBlobItem blobItem) {
        if (_tenantService == null) {
            throw new CaasNoTenantException();
        }
        return new BlobDataModel() {
            RowVersion = blobItem.Version,
            CreationTime = blobItem.CreationTime,
            LastModificationTime = blobItem.LastModificationTime,
            Name = blobItem.Name,
            Path = blobItem.Path,
            MimeType = blobItem.MimeType,
            Blob = blobItem.Blob,
            ShopId = _tenantService.GetTenantGuid()
        };
    }
    
    private static BlobDataModel DataModelFromItem(BlobDataModel existing, IBlobItem blobItem) {
        return existing with {
            Name = blobItem.Name,
            MimeType = blobItem.MimeType,
            LastModificationTime = blobItem.LastModificationTime,
            Blob = blobItem.Blob
        };
    }
}