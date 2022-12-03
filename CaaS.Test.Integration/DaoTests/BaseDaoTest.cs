using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;
using CaaS.Infrastructure.Base.Tenant;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
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
        var statementGenerator = new AdoStatementGenerator<T>(dataRecordMapper);
        var sqlGenerator = new AdoStatementMaterializer();
        var statementExecutor = new AdoStatementExecutor(GetAdoUnitOfWorkManager());
        var spTenantService = IServiceProvider<ITenantIdAccessor>.Empty;
        if (tenantId != null) {
            spTenantService = new StaticTenantIdAccessor(tenantId).AsTypedService();
        }
        return new GenericDao<T>(statementExecutor, sqlGenerator, statementGenerator, DateTimeOffsetProvider.Instance, spTenantService);
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
        var dbFactory = DbProviderFactoryUtil.GetDbProviderFactory(dbOptions);
        return new AdoConnectionFactory(dbFactory, dbOptions);
    }
}