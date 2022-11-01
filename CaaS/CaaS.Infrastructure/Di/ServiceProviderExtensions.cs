namespace CaaS.Infrastructure.Di; 

public static class ServiceProviderExtensions {
    public static IServiceProvider<T> AsTypedService<T>(this T? obj) where T: class {
        return new StaticTypedServiceProvider<T>(obj);
    }
}