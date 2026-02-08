using API.Configurations;
using API.Configurations.Swagger;
using API.Middleware;
using Serilog;
using Serilog.Events;
using NewRelic.LogEnrichers.Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Configurar Serilog com New Relic
var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithNewRelicLogsInContext()
    .WriteTo.Console();

var licenseKey = configuration["NEW_RELIC_LICENSE_KEY"];
if (!string.IsNullOrWhiteSpace(licenseKey))
{
    loggerConfig.WriteTo.NewRelicLogs(
        endpointUrl: "https://log-api.newrelic.com/log/v1",
        applicationName: "Fiap.Fase3.Oficina.API",
        licenseKey: licenseKey
    );
}

Log.Logger = loggerConfig.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Usar Serilog como provedor de logs
builder.Host.UseSerilog();

builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddHealthChecks();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseSecurityHeadersConfiguration();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthCheckEndpoints();
app.MapControllers();


// Popula dados mock
DevelopmentDataSeeder.Seed(app);

await app.RunAsync();

//Necessário para testes de integração
public partial class Program { }

