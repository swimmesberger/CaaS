using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementGenerator<T> : IDataRecordProvider<T> {
    Statement<long> CreateCount(StatementParameters statementParameters);

    Statement<T> CreateFind(StatementParameters statementParameters);
    
    Statement<TResult> CreateFind<TResult>(StatementParameters statementParameters, RowMapper<TResult> mapper);

    Statement<T> CreateInsert(T entity);
    
    Statement<T> CreateInsert(IEnumerable<T> entities);

    Statement<T> CreateUpdate(T entity, int origRowVersion);
    
    Statement<T> CreateUpdate(IEnumerable<VersionedEntity<T>> entities);

    Statement<T> CreateDelete(T entity);
    
    Statement<T> CreateDelete(IEnumerable<T> entities);
}