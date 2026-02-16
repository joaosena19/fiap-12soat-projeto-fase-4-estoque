using API.Configurations;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Tests.API.Configurations;

public class HealthCheckConfigurationTests
{
    [Fact(DisplayName = "AddHealthChecks deve registrar health check 'self' com tag 'live'")]
    [Trait("Configuration", "HealthCheck")]
    public void AddHealthChecks_DeveRegistrarHealthCheckSelfComTagLive()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

        // Act
        services.AddHealthChecks(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckOptions = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        
        var selfCheck = healthCheckOptions.Value.Registrations.FirstOrDefault(r => r.Name == "self");
        selfCheck.Should().NotBeNull("deve existir um health check chamado 'self'");
        selfCheck!.Tags.Should().Contain("live", "o health check 'self' deve ter a tag 'live'");
    }

    [Fact(DisplayName = "AddHealthChecks deve registrar health check 'database' com tag 'ready'")]
    [Trait("Configuration", "HealthCheck")]
    public void AddHealthChecks_DeveRegistrarHealthCheckDatabaseComTagReady()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

        // Act
        services.AddHealthChecks(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckOptions = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        
        var databaseCheck = healthCheckOptions.Value.Registrations.FirstOrDefault(r => r.Name == "database");
        databaseCheck.Should().NotBeNull("deve existir um health check chamado 'database'");
        databaseCheck!.Tags.Should().Contain("ready", "o health check 'database' deve ter a tag 'ready'");
    }

    [Fact(DisplayName = "AddHealthChecks deve registrar exatamente 2 health checks")]
    [Trait("Configuration", "HealthCheck")]
    public void AddHealthChecks_DeveRegistrarExatamenteDoisHealthChecks()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

        // Act
        services.AddHealthChecks(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckOptions = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        
        healthCheckOptions.Value.Registrations.Should().HaveCount(2, "devem existir exatamente 2 health checks registrados: 'self' e 'database'");
    }

    [Fact(DisplayName = "AddHealthChecks deve retornar IServiceCollection para permitir chaining")]
    [Trait("Configuration", "HealthCheck")]
    public void AddHealthChecks_DeveRetornarIServiceCollectionParaChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

        // Act
        var resultado = services.AddHealthChecks(configuration);

        // Assert
        resultado.Should().BeSameAs(services, "o método deve retornar a mesma instância de IServiceCollection para permitir chaining");
    }

    [Fact(DisplayName = "AddHealthChecks deve configurar database check com HealthStatus.Unhealthy como failureStatus")]
    [Trait("Configuration", "HealthCheck")]
    public void AddHealthChecks_DeveConfigurarDatabaseCheckComUnhealthyFailureStatus()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

        // Act
        services.AddHealthChecks(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckOptions = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        
        var databaseCheck = healthCheckOptions.Value.Registrations.FirstOrDefault(r => r.Name == "database");
        databaseCheck.Should().NotBeNull();
        databaseCheck!.FailureStatus.Should().Be(HealthStatus.Unhealthy, "o health check 'database' deve ter HealthStatus.Unhealthy como failure status");
    }
}
