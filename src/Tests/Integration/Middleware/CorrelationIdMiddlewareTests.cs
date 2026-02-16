using FluentAssertions;
using System.Net;

namespace Tests.Integration.Middleware;

public class CorrelationIdMiddlewareTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly TestWebApplicationFactory<Program> _factory;

    public CorrelationIdMiddlewareTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Requisição sem X-Correlation-ID deve gerar um novo GUID e retorná-lo no header de resposta")]
    [Trait("Middleware", "CorrelationId")]
    public async Task InvokeAsync_WhenNoCorrelationId_GeneratesAndReturnsOne()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.Headers.Contains(CorrelationIdHeader).Should().BeTrue(
            "o middleware deve gerar e retornar um X-Correlation-ID quando não fornecido");

        var correlationId = response.Headers.GetValues(CorrelationIdHeader).FirstOrDefault();
        correlationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(correlationId, out _).Should().BeTrue(
            "o correlation ID gerado deve ser um GUID válido");
    }

    [Fact(DisplayName = "Requisição com X-Correlation-ID deve propagar o mesmo valor no header de resposta")]
    [Trait("Middleware", "CorrelationId")]
    public async Task InvokeAsync_WhenCorrelationIdExists_PropagatesIt()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();
        var expectedCorrelationId = "test-correlation-id-12345";
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, expectedCorrelationId);

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.Headers.Contains(CorrelationIdHeader).Should().BeTrue(
            "o middleware deve propagar o X-Correlation-ID fornecido na resposta");

        var correlationId = response.Headers.GetValues(CorrelationIdHeader).FirstOrDefault();
        correlationId.Should().Be(expectedCorrelationId,
            "o correlation ID retornado deve ser exatamente o mesmo que foi enviado");
    }

    [Fact(DisplayName = "Duas requisições sem X-Correlation-ID devem gerar correlation IDs distintos")]
    [Trait("Middleware", "CorrelationId")]
    public async Task InvokeAsync_MultipleRequestsWithoutCorrelationId_GenerateDistinctIds()
    {
        // Arrange
        var client1 = _factory.CreateAuthenticatedClient();
        var client2 = _factory.CreateAuthenticatedClient();

        // Act
        var response1 = await client1.GetAsync("/health/live");
        var response2 = await client2.GetAsync("/health/live");

        // Assert
        var correlationId1 = response1.Headers.GetValues(CorrelationIdHeader).FirstOrDefault();
        var correlationId2 = response2.Headers.GetValues(CorrelationIdHeader).FirstOrDefault();

        correlationId1.Should().NotBeNullOrEmpty();
        correlationId2.Should().NotBeNullOrEmpty();
        correlationId1.Should().NotBe(correlationId2,
            "cada requisição sem correlation ID deve gerar um identificador único");
    }
}
