using CaaS.Core.Base;

namespace CaaS.Test; 

public class MockUnitOfWorkManager : IUnitOfWorkManager {
    public IUnitOfWork? Current { get; private set; }
    public IUnitOfWork Begin() {
        Current ??= new MockUnitOfWork();
        return Current;
    }

    private class MockUnitOfWork : IUnitOfWork {
        public ValueTask DisposeAsync() {
            return ValueTask.CompletedTask;
        }
        public Task CompleteAsync(CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
        public Task RollbackAsync(CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }
    }
}