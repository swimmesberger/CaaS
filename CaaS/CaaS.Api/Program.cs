using System.Data.Common;
using CaaS.Api;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Core.Request;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Repositories;
using CaaS.Infrastructure.Repositories.Base;
using Microsoft.Extensions.Options;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//         .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// register custom services
builder.Services.AddScoped<IRequestDataAccessor, HttpRequestDataAccessor>();
builder.Services.AddScoped<ITenantService, ShopTenantService>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.Configure<RelationalOptions>(builder.Configuration.GetSection(RelationalOptions.Key));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RelationalOptions>>().Value);
builder.Services.AddSingleton(sp => {
    var connectionConfig = sp.GetRequiredService<RelationalOptions>();
    // register all supported database factories
    // postgres
    DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
    // sql-server
    //DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    return DbProviderFactories.GetFactory(connectionConfig.ProviderName);
});
builder.Services.AddSingleton<IRelationalConnectionFactory, RelationalConnectionFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();