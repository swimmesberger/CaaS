namespace CaaS.Infrastructure.Base.Di; 

public class StaticTypedServiceProvider<T> : IServiceProvider<T> {
    private readonly T? _service;

    public StaticTypedServiceProvider(T? service = default) {
        _service = service;
    }

    public T? GetService() => _service;
    
    public T GetRequiredService() {
        if (_service == null) {
            throw new InvalidOperationException();
        }
        return _service;
    }
}