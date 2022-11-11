using System.Data.Common;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping.Base;
using CaaS.Infrastructure.DataModel.Base;
using CaaS.Infrastructure.Di;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.Repositories;
using CaaS.Test.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class BaseDaoTest : IAsyncLifetime {
    private const string PostgresProviderName = "Npgsql";
    private const string PostgreSqlImage = "postgres:15";

    private readonly TestcontainerDatabase _postgresqlContainer;
    private IConnectionFactory? _connectionFactory;
    
    protected ITestOutputHelper Output { get; }
    
    public BaseDaoTest(ITestOutputHelper output) {
        Output = output;
        _postgresqlContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration(PostgreSqlImage) {
                Database = "caas",
                Username = "postgres",
                Password = "postgres",
            })
            .Build();
    }

    public async Task InitializeAsync() {
        await _postgresqlContainer.StartAsync();
        _connectionFactory = GetConnectionFactory();
        await ExecuteSqlScript("InitDatabase.sql");
        await ExecuteSqlScript("Sql.V2__Initial_version.sql");
        await ExecuteSqlScript("TestData.sql");
    }

    public Task DisposeAsync() => _postgresqlContainer.DisposeAsync().AsTask();

    private async Task ExecuteSqlScript(string fileName) {
        string script;
        using(var sqlFile = new StreamReader(typeof(ShopDaoTest).Assembly
                                                 .GetManifestResourceStream($"CaaS.Test.Integration.{fileName}") 
                                             ?? throw new FileNotFoundException($"Can't load sql file {fileName}"))) {
            script = await sqlFile.ReadToEndAsync();
        }
        var result = await _postgresqlContainer.ExecScriptAsync(script);
        Assert.Equal(0, result.ExitCode);
    }
    
    protected GenericDao<T> GetDao<T>(IDataRecordMapper<T> dataRecordMapper, string? tenantId = null) where T : DataModel, new() {
        var statementExecutor = new AdoStatementExecutor(GetAdoUnitOfWorkManager());
        var statementGenerator = new AdoStatementGenerator<T>(dataRecordMapper);
        IServiceProvider<ITenantService> spTenantService = IServiceProvider<ITenantService>.Empty;
        if (tenantId != null) {
            spTenantService = new ShopTenantService(
                    new ShopRepository(GetDao(new ShopDataRecordMapper()), GetDao(new ShopAdminDataRecordMapper())),
                    new StaticTenantIdAccessor(tenantId)
            ).AsTypedService();
        }
        return new GenericDao<T>(statementExecutor, statementGenerator, spTenantService);
    }

    private AdoUnitOfWorkManager GetAdoUnitOfWorkManager() {
        return new AdoUnitOfWorkManager(_connectionFactory!);
    }

    private IConnectionFactory GetConnectionFactory() {
        var dbOptions = new RelationalOptions() {
            ConnectionString = _postgresqlContainer.ConnectionString,
            ProviderName = PostgresProviderName
        };
        Output.WriteLine($"Using connection {dbOptions.ConnectionString}");
        var dbFactory = GetDbProviderFactory(dbOptions);
        return new AdoConnectionFactory(dbFactory, dbOptions);
    }
    
    private static DbProviderFactory GetDbProviderFactory(RelationalOptions relationalOptions) {
        DbProviderFactories.RegisterFactory(PostgresProviderName, NpgsqlFactory.Instance);
        return DbProviderFactories.GetFactory(relationalOptions.ProviderName);
    }
}