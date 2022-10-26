using CaaS.Infrastructure.Ado;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public interface IStatementGenerator<T> {
    IRecordMapper<T> RecordMapper { get; }
    
    Statement CreateCount();

    Statement CreateFind(IEnumerable<QueryParameter>? parameters = null);

    Statement AddFindParameters(Statement statement, IEnumerable<QueryParameter> parameters);
    Statement AddFindParameter(Statement statement, QueryParameter queryParameter);
    Statement AddFindParameterByProperty(Statement statement, string propertyName, object value);
    
    Statement CreateInsert(T entity);

    Statement CreateUpdate(T entity, int origRowVersion);

    Statement CreateDelete(T entity);
}