namespace CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;

public interface IWhereStatement {
    IEnumerable<QueryParameter> Parameters { get; }

    IWhereStatement MapParameterNames(Func<string, string> selector);

    IWhereStatement Add(IWhereStatement where);
}