using CaaS.Core.Base;

namespace CaaS.Infrastructure.Base.Model; 

public abstract record DataModel : IDataModelBase {
    public Guid Id { get; init; }
    // concurrencyToken
    public int RowVersion { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModificationTime { get; init; }

    protected DataModel() {
        Id = Guid.NewGuid();
        RowVersion = 0;
        CreationTime = DateTimeOffsetProvider.GetNow();
        LastModificationTime = CreationTime;
    }
}