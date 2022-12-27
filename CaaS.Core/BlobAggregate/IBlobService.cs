namespace CaaS.Core.BlobAggregate; 

public interface IBlobService {
    Task<IBlobItem?> GetAsync(string path, CancellationToken cancellationToken = default);

    Task AddOrUpdateAsync(IBlobItem blobItem, CancellationToken cancellationToken = default);
}
