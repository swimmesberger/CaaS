using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Test.Common; 

public class MemoryDao<T> : IDao<T>, IHasMetadataProvider where T: IDataModelBase {
    public IRecordMetadataProvider MetadataProvider { get; }
    
    private readonly List<T> _data;
    private readonly Dictionary<string, PropertyInfo> _properties;
    
    public MemoryDao(List<T> data) {
        _data = data;
        _properties = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);
        MetadataProvider = new MemoryDaoRecordMetadataProvider(_properties);
    }
    
    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return _data.ToAsyncEnumerable();
    }

    public IAsyncEnumerable<TValue> FindScalarBy<TValue>(StatementParameters parameters, CancellationToken cancellationToken = default) {
        return FindBy(parameters, cancellationToken).Select(model => {
            var propName = parameters.SelectParameters.Properties[0];
            return (TValue)_properties[propName].GetValue(model)!;
        });
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        var enumerable = _data.Where(item => Filter(item, parameters.WhereParameters));
        
        IOrderedEnumerable<T>? orderedEnumerable = null;
        foreach (var orderParameter in parameters.OrderBy) {
            object? KeySelector(T d) => _properties[orderParameter.Name].GetValue(d);
            if (orderedEnumerable == null) {
                orderedEnumerable = orderParameter.OrderType == OrderType.Asc ? 
                        enumerable.OrderBy(KeySelector) : 
                        enumerable.OrderByDescending(KeySelector);
            } else {
                orderedEnumerable = orderParameter.OrderType == OrderType.Asc ? 
                        orderedEnumerable.ThenBy(KeySelector) : 
                        orderedEnumerable.ThenByDescending(KeySelector);
            }
            enumerable = orderedEnumerable;
        }
        return enumerable.ToAsyncEnumerable();
    }

    public async Task<long> CountAsync(StatementParameters? parameters = null, CancellationToken cancellationToken = default) {
        if (parameters == null) {
            return _data.Count; 
        }
        return await FindBy(parameters, cancellationToken).CountAsync(cancellationToken);
    }
    
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        _data.Add(entity);
        return Task.FromResult(entity);
    }

    public async Task<IReadOnlyCollection<T>> AddAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        foreach (var entity in entities) {
            await AddAsync(entity, cancellationToken);
        }
        return entities;
    }

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var dIdx = _data.FindIndex(d => d.Id == entity.Id);
        _data[dIdx] = entity;
        return Task.FromResult(entity);
    }

    public async Task<IReadOnlyCollection<T>> UpdateAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        foreach (var entity in entities) {
            await UpdateAsync(entity, cancellationToken);
        }
        return entities;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var dIdx = _data.FindIndex(d => d.Id == entity.Id);
        _data.RemoveAt(dIdx);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        foreach (var entity in entities) {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public IReadOnlyDictionary<string, object?> ReadPropertiesFromModel(T model, IEnumerable<string> properties) {
        var propertyValuePairs = new Dictionary<string, object?>();
        foreach (var property in properties) {
            propertyValuePairs[property] = _properties[property].GetValue(model);
        }
        return propertyValuePairs;
    }

        private bool Filter(T item, WhereParameters where) {
        foreach (var whereStatement in where.Statements) {
            switch (whereStatement) {
                case SimpleWhere simpleWhere:
                    if (!FilterSimple(item, simpleWhere)) 
                        return false;
                    break;
                case RowValueWhere rowValueWhere:
                    if (!FilterSimple(item, rowValueWhere)) 
                        return false;
                    break;
                case SearchWhere searchWhere:
                    if (!FilterSearch(item, searchWhere)) 
                        return false;
                    break;
                default:
                    throw new ArgumentException($"Unsupported where clause '{whereStatement.GetType()}'");
            }
        }
        return true;
    }

    private bool FilterSimple(T item, IWhereStatement where) {
        foreach (var param in where.Parameters) {
            var prop = _properties[param.Name];
            if (param.Value is IEnumerable enumerable and not string) {
                var lookup = enumerable.Cast<object?>().ToHashSet();
                if (!lookup.Contains(prop.GetValue(item)))
                    return false;
            } else {
                if (!prop.GetValue(item)!.Equals(param.Value))
                    return false;
            }
        }
        return true;
    }
    
    private bool FilterSearch(T item, SearchWhere where) {
        foreach (var param in where.Parameters) {
            var prop = _properties[param.Name];
            var propValueString = prop.GetValue(item)?.ToString();
            var paramValueString = param.Value?.ToString();
            if (propValueString == null || paramValueString == null) continue;
            if (!paramValueString.Contains(propValueString, StringComparison.CurrentCultureIgnoreCase))
                return false;
        }
        return true;
    }


    private class MemoryDaoRecordMetadataProvider: IRecordMetadataProvider {

        private Dictionary<string, PropertyInfo> PropertyInfos;

        public MemoryDaoRecordMetadataProvider(Dictionary<string, PropertyInfo> properties) {
            PropertyInfos = properties;
        }
        
        public int? GetObjectType(string key) {
            return null;
        }
        public Type GetPropertyType(string key) {
            var info = PropertyInfos[key];
            return info.PropertyType;
        }
    }
}