namespace CaaS.Infrastructure.Ado.Base; 

public interface IHasConnectionProvider {
    IConnectionProvider ConnectionProvider { get; }
}