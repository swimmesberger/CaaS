using System.Data;
using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories; 

public class ShopRepository : Repository<Shop>, IShopRepository {
    private const string TableName = "shop";
    
    public ShopRepository(IQueryExecutor queryExecutor, 
            IPropertyMappingProvider<Shop> propertyMappingProvider) : base(queryExecutor, propertyMappingProvider) { }

    public async Task<Shop?> FindByNameAsync(string name, CancellationToken cancellationToken = default) {
        return (await QueryAsync(
                $" AND name = @name",
                new[]{new QueryParameter("name", name)}, 
                cancellationToken)).FirstOrDefault();
    }

    protected override string GetTableName() => TableName;

    protected override Shop SetFromRecord(Shop value, RecordValues record) {
        value = base.SetFromRecord(value, record);
        return value with {
            Name = record.GetString(nameof(Shop.Name))
        };
    }

    protected override object GetRecordValue(Shop value, string propertyName) {
        if (propertyName.Equals(nameof(Shop.Name))) {
            return value.Name;
        }
        return base.GetRecordValue(value, propertyName);
    }
}