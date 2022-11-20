using System.Text.Json.Serialization;
using CaaS.Api.BackgroundServices;
using CaaS.Api.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//         .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllers()
    .AddMvcOptions(options => {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddHostedService<ScheduledUpdateService>();


// register CaaS services
builder.Services.AddCaas(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpLogging();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCaas();

app.MapControllers();

await app.RunAsync();
