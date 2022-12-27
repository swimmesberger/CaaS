namespace CaaS.Core.BlobAggregate; 

public static class BlobServiceExtensions {
    public static async Task AddEmptyAsync(this IBlobService blobService, string path, CancellationToken cancellationToken = default) {
        await blobService.AddOrUpdateAsync(new BlobItem() {
            Path = path
        }, cancellationToken: cancellationToken);
    }
}