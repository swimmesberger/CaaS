namespace CaaS.Infrastructure.DataModel.Base; 

public abstract record DataModel : IDataModelBase {
    public Guid Id { get; init; }
    // concurrencyToken
    public int RowVersion { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModificationTime { get; init; }

    public DataModel() {
        Id = Guid.NewGuid();
        RowVersion = 0;
        CreationTime = DateTimeOffset.UtcNow;
        LastModificationTime = CreationTime;
    }
}