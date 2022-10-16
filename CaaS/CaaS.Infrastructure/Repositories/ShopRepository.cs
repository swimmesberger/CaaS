using System.Data;
using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

public class ShopRepository : Repository<Shop>, IShopRepository {
    private const string TableName = "shop";
    private const string ColumnName = "name";
    private static readonly string[] ColumnNames = { ColumnName };

    public ShopRepository(IRelationalConnectionFactory dbProviderFactory) : base(dbProviderFactory) { }

    protected override string GetTableName() => TableName;

    protected override IReadOnlyList<string> GetColumnNames() => ColumnNames;
    
    protected override Shop SetFromRecord(Shop value, IDataRecord record) {
        return value with {
            Name = record.GetString(ColumnName)
        };
    }
}