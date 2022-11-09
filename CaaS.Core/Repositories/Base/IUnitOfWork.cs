namespace CaaS.Core.Repositories.Base; 

public interface IUnitOfWork : IAsyncDisposable {
    Task CompleteAsync(CancellationToken cancellationToken = default);
}