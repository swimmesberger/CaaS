using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Test.Common; 

public class MemoryDao<T> : IDao<T> where T: IDataModelBase {
    private readonly List<T> _data;
    private readonly Dictionary<string, PropertyInfo> _properties;

    public MemoryDao(List<T> data) {
        _data = data;
        _properties = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);
    }
    
    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return _data.ToAsyncEnumerable();
    }

    public IAsyncEnumerable<TValue> FindScalarBy<TValue>(StatementParameters parameters, CancellationToken cancellationToken = default) {
        return FindBy(parameters, cancellationToken).Select(model => {
            var propName = parameters.SelectParameters.Properties![0];
            return (TValue)_properties[propName].GetValue(model)!;
        });
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        var enumerable = _data.Where(d => {
            foreach (var param in parameters.WhereParameters.Parameters) {
                var prop = _properties[param.Name];
                if (param.Value is IEnumerable enumerable and not string) {
                    var lookup = enumerable.Cast<object?>().ToHashSet();
                    if (!lookup.Contains(prop.GetValue(d)))
                        return false;
                } else {
                    if (!prop.GetValue(d)!.Equals(param.Value))
                        return false;
                }
            }
            return true;
        });
        
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
}