using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.Dao; 

public interface IStatementGenerator<T> {
    IDataRecordMapper<T> DataRecordMapper { get; }
    
    Statement CreateCount();

    Statement CreateFind(StatementParameters statementParameters);

    Statement CreateInsert(T entity);

    Statement CreateUpdate(T entity, int origRowVersion);

    Statement CreateDelete(T entity);
}