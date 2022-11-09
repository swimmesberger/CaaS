namespace CaaS.Infrastructure.Di; 

public interface IServiceProvider<out T> {
    public static readonly IServiceProvider<T> Empty = new EmptyTypedServiceProvider<T>(); 
    
    T? GetService();
    
    T GetRequiredService();
}

public class EmptyTypedServiceProvider<T> : IServiceProvider<T> {
    public T? GetService() => default; 
    
    public T GetRequiredService() => throw new InvalidOperationException();

    internal EmptyTypedServiceProvider() { }
}