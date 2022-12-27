using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.BlobData; 

[GenerateMapper]
[SqlTable("files")]
public record BlobDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public ReadOnlyMemory<byte> Blob { get; init; } = ReadOnlyMemory<byte>.Empty;
}