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
    .Enrich.With<Infrastructure.Monitoramento.Correlation.CorrelationIdEnricher>()
    .Enrich.WithNewRelicLogsInContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

var licenseKey = configuration["NEW_RELIC_LICENSE_KEY"];
var appName = configuration["NEW_RELIC_APP_NAME"] ?? "EstoqueService";

if (!string.IsNullOrWhiteSpace(licenseKey))
{
    loggerConfig.WriteTo.NewRelicLogs(
        endpointUrl: "https://log-api.newrelic.com/log/v1",
        applicationName: appName,
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
builder.Services.AddMonitoring();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddHealthChecks();


var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
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

