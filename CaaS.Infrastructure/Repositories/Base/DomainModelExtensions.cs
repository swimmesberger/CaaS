using CaaS.Core.Entities.Base;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.DataModel.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public static class DomainModelExtensions {
    public static int GetRowVersion(this IEntityBase entity) {
        if (string.IsNullOrEmpty(entity.ConcurrencyToken)) {
            return 0;
        }
        if (!int.TryParse(entity.ConcurrencyToken, out var rowVersion)) {
            throw new CaasDomainMappingException($"Invalid concurrency token {entity.ConcurrencyToken} for entity {entity.Id}");
        }
        return rowVersion;
    }

    public static string GetConcurrencyToken(this IDataModelBase dataModel) {
        return dataModel.RowVersion.ToString();
    }
}