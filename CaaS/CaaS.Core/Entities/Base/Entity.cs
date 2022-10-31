namespace CaaS.Core.Entities.Base; 

public abstract record Entity : IEntityBase {
    public Guid Id { get; init; }
    // concurrencyToken
    public int RowVersion { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModificationTime { get; init; }

    public Entity() {
        Id = Guid.NewGuid();
        RowVersion = 0;
        CreationTime = DateTimeOffset.UtcNow;
        LastModificationTime = CreationTime;
    }
}