using CaaS.Api.Base;

var builder = WebApplication.CreateBuilder(args);

// register CaaS services
builder.Services.AddCaas(builder.Configuration);

await using var app = builder.Build();
app.UseCaas();
await app.RunAsync();