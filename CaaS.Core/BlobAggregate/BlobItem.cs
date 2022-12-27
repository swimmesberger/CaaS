using CaaS.Core.Base;

namespace CaaS.Core.BlobAggregate; 

public record BlobItem : IBlobItem {
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public ReadOnlyMemory<byte> Blob { get; init; }
    public int Version { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModificationTime { get; init; }
    
    public BlobItem() {
        CreationTime = SystemClock.GetNow();
        LastModificationTime = CreationTime;
    }
}