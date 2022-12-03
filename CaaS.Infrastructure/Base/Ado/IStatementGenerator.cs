using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Query;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementGenerator<T> : IDataRecordProvider<T> {
    Statement<long> CreateCount(StatementParameters statementParameters);

    Statement<T> CreateFind(StatementParameters statementParameters);
    
    Statement<TResult> CreateFind<TResult>(StatementParameters statementParameters, RowMapper<TResult> mapper);

    Statement CreateInsert(T entity);
    
    Statement CreateInsert(IEnumerable<T> entities);

    Statement CreateUpdate(T entity, int origRowVersion);
    
    StatementBatch CreateUpdate(IEnumerable<VersionedEntity<T>> entities);

    Statement CreateDelete(T entity);
    
    Statement CreateDelete(IEnumerable<T> entities);
}