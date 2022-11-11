namespace CaaS.Core.Base; 

public interface IUnitOfWork : IAsyncDisposable {
    Task CompleteAsync(CancellationToken cancellationToken = default);
}