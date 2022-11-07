using System.Data.Common;
using System.Text;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping.Base;
using CaaS.Infrastructure.DataModel.Base;
using CaaS.Infrastructure.Di;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.Repositories;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;

namespace CaaS.Test.Integration; 

public class BaseDaoTest : IAsyncLifetime {
    private const string PostgresProviderName = "Npgsql";
    private const string PostgreSqlImage = "postgres:15";
    
    private readonly TestcontainerDatabase _postgresqlContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration(PostgreSqlImage) {
                Database = "db",
                Username = "postgres",
                Password = "postgres",
            })
            .Build();
    
    public async Task InitializeAsync() {
        await _postgresqlContainer.StartAsync();
        await ExecuteSqlScript("Sql.V1__Database.sql");
        await ExecuteSqlScript("Sql.V2__Initial_version.sql");
        await ExecuteSqlScript("TestData.sql");
    }

    public Task DisposeAsync() => _postgresqlContainer.DisposeAsync().AsTask();

    private async Task ExecuteSqlScript(string fileName) {
        var connectionProvider = GetAdoUnitOfWorkManager().ConnectionProvider;
        var sqlStatements = GetSqlStatements(fileName);
        await foreach (var (lineNumber, sqlStatement) in sqlStatements) {
            try {
                await ExecuteSqlStatement(connectionProvider, sqlStatement);
            } catch (PostgresException ex) {
                throw new NpgsqlException($"Failed to execute statement on line {lineNumber} in file {fileName} with statement {sqlStatement}", ex);
            }
        }
    }

    private async Task ExecuteSqlStatement(IConnectionProvider connectionProvider, string statement) {
        await using var command = new NpgsqlCommand();
        command.Connection = (NpgsqlConnection)await connectionProvider.GetDbConnectionAsync();
        command.CommandText = statement;
        await command.ExecuteNonQueryAsync();
    }

    private async IAsyncEnumerable<(int LineNumber, string Line)> GetSqlStatements(string fileName) {
        using var sqlFile = new StreamReader(typeof(ShopDaoTest).Assembly
                                                     .GetManifestResourceStream($"CaaS.Test.Integration.{fileName}") 
                                             ?? throw new FileNotFoundException($"Can't load sql file {fileName}"));
        var lineNumber = 0;
        var buffer = new StringBuilder();
        while (!sqlFile.EndOfStream) {
            var line = await sqlFile.ReadLineAsync();
            if (line == null) break;
            var commentStartIdx = line.IndexOf("--", StringComparison.Ordinal);
            if (commentStartIdx != -1) {
                line = line[..commentStartIdx];
            }
            line = line.Trim();
            lineNumber += 1;
            if (string.IsNullOrEmpty(line)) continue;
            var endOfStatementIdx = line.IndexOf(';');
            if (endOfStatementIdx != -1) {
                endOfStatementIdx += 1;
                buffer.Append(' ').Append(line[..endOfStatementIdx]);
            } else {
                buffer.Append(line);
            }
            if (endOfStatementIdx == -1) continue;
            yield return (lineNumber, buffer.ToString());
            buffer.Clear();
            buffer.Append(line[endOfStatementIdx..]);
        }
    }
    
    protected GenericDao<T> GetDao<T>(IDataRecordMapper<T> dataRecordMapper, string? tenantId = null) where T : DataModel, new() {
        var unitOfWorkManager = GetAdoUnitOfWorkManager();
        var statementExecutor = new AdoStatementExecutor(unitOfWorkManager);
        var statementGenerator = new AdoStatementGenerator<T>(dataRecordMapper);
        IServiceProvider<ITenantService> spTenantService = IServiceProvider<ITenantService>.Empty;
        if (tenantId != null) {
            spTenantService = new ShopTenantService(
                    new ShopRepository(GetDao(new ShopDataRecordMapper())),
                    new StaticTenantIdAccessor(tenantId)
            ).AsTypedService();
        }
        return new GenericDao<T>(statementExecutor, statementGenerator, spTenantService);
    }

    private AdoUnitOfWorkManager GetAdoUnitOfWorkManager() {
        var dbOptions = new RelationalOptions() {
                ConnectionString = _postgresqlContainer.ConnectionString,
                ProviderName = PostgresProviderName
        };
        var dbFactory = GetDbProviderFactory(dbOptions);
        var connectionFactory = new AdoConnectionFactory(dbFactory, dbOptions);
        return new AdoUnitOfWorkManager(connectionFactory);
    }
    
    private static DbProviderFactory GetDbProviderFactory(RelationalOptions relationalOptions) {
        DbProviderFactories.RegisterFactory(PostgresProviderName, NpgsqlFactory.Instance);
        return DbProviderFactories.GetFactory(relationalOptions.ProviderName);
    }
}