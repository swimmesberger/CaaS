using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping.Base;

namespace CaaS.Infrastructure.Ado.Base; 

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