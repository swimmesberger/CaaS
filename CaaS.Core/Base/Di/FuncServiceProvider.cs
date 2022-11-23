namespace CaaS.Core.Base.Di; 

public class FuncServiceProvider : IServiceProvider {
    private readonly Func<Type, object?> _func;

    public FuncServiceProvider(Func<Type, object?> func) {
        _func = func;
    }

    public object? GetService(Type serviceType) => _func.Invoke(serviceType);
}