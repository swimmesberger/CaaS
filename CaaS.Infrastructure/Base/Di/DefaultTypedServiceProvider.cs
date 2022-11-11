using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Infrastructure.Base.Di; 

public class DefaultTypedServiceProvider<T> : IServiceProvider<T> where T : notnull {
    private readonly IServiceProvider _serviceProvider;

    public DefaultTypedServiceProvider(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public T? GetService() => _serviceProvider.GetService<T>();
    
    public T GetRequiredService() => _serviceProvider.GetRequiredService<T>();
}