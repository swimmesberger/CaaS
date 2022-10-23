namespace CaaS.Infrastructure.Ado; 

public interface IHasConnectionProvider {
    IConnectionProvider ConnectionProvider { get; }
}