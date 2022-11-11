using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementGenerator<T> {
    IDataRecordMapper<T> DataRecordMapper { get; }
    
    Statement CreateCount();

    Statement CreateFind(StatementParameters statementParameters);

    Statement CreateInsert(T entity);
    
    Statement CreateInsert(IEnumerable<T> entities);

    Statement CreateUpdate(T entity, int origRowVersion);
    
    Statement CreateUpdate(IEnumerable<VersionedEntity<T>> entities);

    Statement CreateDelete(T entity);
    
    Statement CreateDelete(IEnumerable<T> entities);
}