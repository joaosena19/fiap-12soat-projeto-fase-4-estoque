using API.Configurations;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Other.Database;

public class DatabaseConfigurationTests
{
    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values!)
            .Build();
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando configuração do banco está incompleta")]
    public void AddDatabase_Deve_Lancar_Quando_ConfigIncompleta()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["DatabaseConnection:Host"] = "localhost",
            // Port ausente
            ["DatabaseConnection:DatabaseName"] = "db",
            ["DatabaseConnection:User"] = "user",
            ["DatabaseConnection:Password"] = "pass"
        });

        // Act
        var act = () => services.AddDatabase(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Configuração de banco de dados incompleta.*");
    }

    [Fact(DisplayName = "Deve registrar AppDbContext quando configuração é válida")]
    public void AddDatabase_Deve_Registrar_AppDbContext_Quando_ConfigValida()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["DatabaseConnection:Host"] = "localhost",
            ["DatabaseConnection:Port"] = "5432",
            ["DatabaseConnection:DatabaseName"] = "testdb",
            ["DatabaseConnection:User"] = "user",
            ["DatabaseConnection:Password"] = "pass"
        });

        // Act
        services.AddDatabase(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetService<AppDbContext>();
        context.Should().NotBeNull();
        var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<AppDbContext>>();
        options.Should().NotBeNull();
    }
}
