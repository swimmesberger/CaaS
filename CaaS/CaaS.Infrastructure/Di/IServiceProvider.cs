namespace CaaS.Infrastructure.Di; 

public interface IServiceProvider<out T> {
    T? GetService();
    
    T GetRequiredService();
}