﻿using System.Diagnostics.CodeAnalysis;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Dao;
using CaaS.Infrastructure.DataModel.Base;

namespace CaaS.Test; 

public class MemoryDao<T> : IDao<T> where T: IDataModelBase {
    private readonly List<T> _data;

    public MemoryDao(List<T> data) {
        _data = data;
    }
    
    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return _data.ToAsyncEnumerable();
    }
    
    public Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return Task.FromResult(_data.FirstOrDefault(d => d.Id == id));
    }
    
    public IAsyncEnumerable<T> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var idSet = ids.ToHashSet();
        return _data.Where(d => idSet.Contains(d.Id)).ToAsyncEnumerable();
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        var propertyNames = parameters.ParameterNames.ToHashSet();
        var properties = typeof(T).GetProperties()
                .Where(p => propertyNames.Contains(p.Name)).ToDictionary(p => p.Name, p => p);
        var enumerable = _data.Where(d => {
            foreach (var param in parameters.Where) {
                var prop = properties[param.Name];
                if (!prop.GetValue(d)!.Equals(param.Value))
                    return false;
            }
            return true;
        });
        
        IOrderedEnumerable<T>? orderedEnumerable = null;
        foreach (var orderParameter in parameters.OrderBy) {
            object? KeySelector(T d) => properties[orderParameter.Name].GetValue(d);
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

    public Task<long> CountAsync(CancellationToken cancellationToken = default) => Task.FromResult<long>(_data.Count);
    
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        _data.Add(entity);
        return Task.FromResult(entity);
    }
    
    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var dIdx = _data.FindIndex(d => d.Id == entity.Id);
        _data[dIdx] = entity;
        return Task.FromResult(entity);
    }
    
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var dIdx = _data.FindIndex(d => d.Id == entity.Id);
        _data.RemoveAt(dIdx);
        return Task.CompletedTask;
    }
}