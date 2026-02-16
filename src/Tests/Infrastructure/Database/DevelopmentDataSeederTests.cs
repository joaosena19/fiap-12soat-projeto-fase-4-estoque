using API.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Tests.Infrastructure.Database;

/// <summary>
/// Testes unitários para DevelopmentDataSeeder.
/// Valida comportamento dos guards (ambiente e detecção de testes).
/// </summary>
public class DevelopmentDataSeederTests
{
    [Fact(DisplayName = "SeedIfDevelopment não deve lançar exceção quando ambiente não for Development")]
    [Trait("Componente", "Seed")]
    [Trait("Infrastructure", "Bootstrap")]
    public void SeedIfDevelopment_NaoDeveLancarExcecao_QuandoAmbienteNaoForDevelopment()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });
        var app = builder.Build();

        // Act & Assert
        FluentActions.Invoking(() => DevelopmentDataSeeder.SeedIfDevelopment(app))
            .Should().NotThrow("porque o método deve retornar imediatamente se ambiente não for Development");
    }

    [Fact(DisplayName = "Seed não deve lançar exceção quando estiver em ambiente de testes")]
    [Trait("Componente", "Seed")]
    [Trait("Infrastructure", "Bootstrap")]
    public void Seed_NaoDeveLancarExcecao_QuandoEstiverEmAmbienteDeTestes()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Development });
        var app = builder.Build();

        // Act & Assert
        FluentActions.Invoking(() => DevelopmentDataSeeder.Seed(app))
            .Should().NotThrow("porque o método deve retornar imediatamente ao detectar assembly de testes");
    }
}
