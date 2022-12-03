namespace CaaS.Infrastructure.Base.Ado.Query.Parameters;

public record OrderParameter(string Name, OrderType OrderType = OrderType.Asc);

public enum OrderType {
    Asc,
    Desc
}