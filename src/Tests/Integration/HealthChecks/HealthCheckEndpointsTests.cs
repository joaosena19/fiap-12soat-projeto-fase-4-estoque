using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Tests.Integration.HealthChecks;

public class HealthCheckEndpointsTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public HealthCheckEndpointsTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Endpoint /health/live deve retornar 200 OK com status Healthy")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthLive_DeveRetornar200ComStatusHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "o endpoint de liveness deve retornar 200 OK");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var json = JsonDocument.Parse(content);
        json.RootElement.GetProperty("status").GetString().Should().Be("Healthy", "o status geral deve ser Healthy");
    }

    [Fact(DisplayName = "Endpoint /health/live deve retornar resposta JSON com propriedade checks")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthLive_DeveConterPropriedadeChecksNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
    }

    [Fact(DisplayName = "Endpoint /health/ready deve responder com JSON estruturado")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthReady_DeveResponderComJsonEstruturado()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var json = JsonDocument.Parse(content);
        json.RootElement.TryGetProperty("status", out _).Should().BeTrue("a resposta deve conter a propriedade 'status'");
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
    }

    [Fact(DisplayName = "Endpoint /health/ready deve retornar resposta JSON com propriedade checks")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthReady_DeveConterPropriedadeChecksNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
        json.RootElement.TryGetProperty("status", out _).Should().BeTrue("a resposta deve conter a propriedade 'status'");
    }

    [Fact(DisplayName = "Endpoint /health/startup deve responder com JSON estruturado")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthStartup_DeveResponderComJsonEstruturado()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/startup");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        var json = JsonDocument.Parse(content);
        json.RootElement.TryGetProperty("status", out _).Should().BeTrue("a resposta deve conter a propriedade 'status'");
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
    }

    [Fact(DisplayName = "Endpoint /health/startup deve retornar resposta JSON com propriedade checks")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthStartup_DeveConterPropriedadeChecksNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/startup");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
        json.RootElement.TryGetProperty("status", out _).Should().BeTrue("a resposta deve conter a propriedade 'status'");
    }

    [Fact(DisplayName = "Resposta JSON dos endpoints deve conter totalDuration")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_DeveConterTotalDurationNaResposta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("totalDuration", out var totalDuration).Should().BeTrue("a resposta deve conter a propriedade 'totalDuration'");
        totalDuration.ValueKind.Should().Be(JsonValueKind.String, "totalDuration deve ser uma string representando TimeSpan");
    }

    [Fact(DisplayName = "Resposta JSON dos checks deve conter estrutura com propriedades obrigatórias quando presentes")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_ChecksDevemConterEstruturaCorreta()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        
        json.RootElement.TryGetProperty("status", out _).Should().BeTrue("a resposta deve conter a propriedade 'status'");
        json.RootElement.TryGetProperty("checks", out _).Should().BeTrue("a resposta deve conter a propriedade 'checks'");
        json.RootElement.TryGetProperty("totalDuration", out _).Should().BeTrue("a resposta deve conter a propriedade 'totalDuration'");
    }

    [Fact(DisplayName = "Resposta deve ter Content-Type application/json")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_DeveRetornarContentTypeJson()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json", "o response writer deve definir Content-Type como application/json");
    }

    [Fact(DisplayName = "Resposta JSON deve estar formatada (indented)")]
    [Trait("Integration", "HealthCheck")]
    public async Task HealthEndpoints_RespostaDeveEstarFormatada()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\n", "a resposta JSON deve estar indentada (WriteIndented = true)");
    }
}
