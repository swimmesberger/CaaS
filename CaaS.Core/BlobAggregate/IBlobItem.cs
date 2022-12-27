namespace CaaS.Core.BlobAggregate; 

public interface IBlobItem {
    string Name { get; }
    string Path { get; }
    string MimeType { get; }
    ReadOnlyMemory<byte> Blob { get; }
    DateTimeOffset CreationTime { get; }
    DateTimeOffset LastModificationTime { get; }
    int Version { get; init; }
}