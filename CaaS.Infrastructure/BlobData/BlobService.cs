using CaaS.Core.BlobAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;

namespace CaaS.Infrastructure.BlobData; 

public sealed class BlobService : IBlobService {
    private readonly IDao<BlobDataModel> _dao;

    public BlobService(IDao<BlobDataModel> dao) {
        _dao = dao;
    }
    public async Task<IBlobItem?> GetAsync(string path, CancellationToken cancellationToken = default) {
        return ItemFromDataModel(await GetModelAsync(path, cancellationToken));
    }
    
    public async Task AddOrUpdateAsync(IBlobItem blobItem, CancellationToken cancellationToken = default) {
        var existingItem = await GetModelAsync(blobItem.Path, cancellationToken);
        if (existingItem == null) {
            await _dao.AddAsync(DataModelFromItem(blobItem), cancellationToken);
        } else {
            await _dao.UpdateAsync(DataModelFromItem(existingItem, blobItem), cancellationToken);
        }
    }

    private async Task<BlobDataModel?> GetModelAsync(string path, CancellationToken cancellationToken = default) {
        return await _dao.FindBy(new StatementParameters() {
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

    private static BlobDataModel DataModelFromItem(IBlobItem blobItem) {
        return new BlobDataModel() {
            RowVersion = blobItem.Version,
            CreationTime = blobItem.CreationTime,
            LastModificationTime = blobItem.LastModificationTime,
            Name = blobItem.Name,
            Path = blobItem.Path,
            MimeType = blobItem.MimeType,
            Blob = blobItem.Blob
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