using CaaS.Core.Base;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Base.DataModel;

namespace CaaS.Infrastructure.Base.Repository; 

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