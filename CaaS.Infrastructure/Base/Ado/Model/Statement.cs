using System.Data.Common;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado.Model;

public record Statement {
    public static readonly Statement Empty = new Statement(StatementType.Find, EmptyStatementSqlGenerator.Instance);
    
    public StatementType Type { get; }
    private readonly IStatementSqlGenerator _sqlGenerator;

    public string From { get; init; } = string.Empty;
    public StatementParameters Parameters { get; init; } = StatementParameters.Empty;
    
    public Statement(StatementType type, IStatementSqlGenerator sqlGenerator) {
        Type = type;
        _sqlGenerator = sqlGenerator;
    }

    public virtual Statement MapToColumnNames() => this;

    public MaterializedStatements Materialize() => _sqlGenerator.MaterializeStatement(this);

    public static Statement<T> CreateEmpty<T>() => Statement<T>.Empty;
}

public record Statement<T> : Statement {
    internal new static readonly Statement<T> Empty = new Statement<T>(StatementType.Find, EmptyStatementSqlGenerator.Instance, EmptyPropertyMapping.EmptyMapper);
    
    private readonly RowMapper<T>? _rowMapper;
    private readonly IPropertyMapper _propertyToColumnMapper;
    
    public Statement(StatementType type, IStatementSqlGenerator sqlGenerator, IPropertyMapper propertyToColumnMapper, 
        RowMapper<T>? rowMapper = null) : base(type, sqlGenerator) {
        _propertyToColumnMapper = propertyToColumnMapper;
        _rowMapper = rowMapper;
    }

    public override Statement<T> MapToColumnNames() {
        return this with {
            From = _propertyToColumnMapper.MappedTypeName,
            Parameters = Parameters.MapParameterNames(_propertyToColumnMapper.MapName)
        };
    }

    public Statement<T> AddWhereParameter(string name, object value) {
        return AddParameters(StatementParameters.CreateWhere(QueryParameter.From(name, value)));
    }

    public ValueTask<T> MapResult(DbDataReader record, CancellationToken cancellationToken = default) {
        if (_rowMapper == null) {
            throw new InvalidOperationException("Can't map result when using modification statement");
        }
        return _rowMapper.Invoke(record, cancellationToken);
    }

    private Statement<T> AddParameters(StatementParameters parameters) {
        return this with {
            Parameters = Parameters.Add(parameters)
        };
    }
}