using System.Net;
using System.Text.Json;
using FluentAssertions;
using Shared.Enums;
using Shared.Exceptions;
using Tests.API.Middleware.Helpers;
using Tests.Infrastructure.SharedHelpers;

namespace Tests.API.Middleware;

/// <summary>
/// Testes unitários para ExceptionHandlingMiddleware
/// </summary>
public class ExceptionHandlingMiddlewareTests : IClassFixture<ExceptionHandlingMiddlewareTestFixture>
{
    private readonly ExceptionHandlingMiddlewareTestFixture _fixture;

    public ExceptionHandlingMiddlewareTests(ExceptionHandlingMiddlewareTestFixture fixture) {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Quando não há exceção, deve chamar próximo delegate")]
    public async Task InvokeAsync_QuandoSemExcecao_DeveChamarProximoDelegate() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var nextCalled = false;

        var middleware = new global::API.Middleware.ExceptionHandlingMiddleware(
            next: _ => { nextCalled = true; return Task.CompletedTask; },
            logger: _fixture.LoggerMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }

    [Fact(DisplayName = "Quando ocorre DomainException com InvalidInput, deve retornar 400 Bad Request")]
    public async Task InvokeAsync_QuandoDomainExceptionComInvalidInput_DeveRetornar400() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Dados de entrada inválidos";
        var domainException = new DomainException(mensagemErro, ErrorType.InvalidInput);
        var expectedStatusCode = (int)HttpStatusCode.BadRequest;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);
        context.Response.ContentType.Should().Be("application/json");

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(mensagemErro);
        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com ResourceNotFound, deve retornar 404 Not Found")]
    public async Task InvokeAsync_QuandoDomainExceptionComResourceNotFound_DeveRetornar404() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Item de estoque não encontrado";
        var domainException = new DomainException(mensagemErro, ErrorType.ResourceNotFound);
        var expectedStatusCode = (int)HttpStatusCode.NotFound;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(mensagemErro);
        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com ReferenceNotFound, deve retornar 422 Unprocessable Entity")]
    public async Task InvokeAsync_QuandoDomainExceptionComReferenceNotFound_DeveRetornar422() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Referência não encontrada";
        var domainException = new DomainException(mensagemErro, ErrorType.ReferenceNotFound);
        var expectedStatusCode = (int)HttpStatusCode.UnprocessableEntity;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com DomainRuleBroken, deve retornar 422 Unprocessable Entity")]
    public async Task InvokeAsync_QuandoDomainExceptionComDomainRuleBroken_DeveRetornar422() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Regra de domínio violada";
        var domainException = new DomainException(mensagemErro, ErrorType.DomainRuleBroken);
        var expectedStatusCode = (int)HttpStatusCode.UnprocessableEntity;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com Conflict, deve retornar 409 Conflict")]
    public async Task InvokeAsync_QuandoDomainExceptionComConflict_DeveRetornar409() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Recurso já existe";
        var domainException = new DomainException(mensagemErro, ErrorType.Conflict);
        var expectedStatusCode = (int)HttpStatusCode.Conflict;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com Unauthorized, deve retornar 401 Unauthorized")]
    public async Task InvokeAsync_QuandoDomainExceptionComUnauthorized_DeveRetornar401() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Acesso não autorizado";
        var domainException = new DomainException(mensagemErro, ErrorType.Unauthorized);
        var expectedStatusCode = (int)HttpStatusCode.Unauthorized;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre DomainException com UnexpectedError, deve retornar 500 Internal Server Error")]
    public async Task InvokeAsync_QuandoDomainExceptionComUnexpectedError_DeveRetornar500() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var mensagemErro = "Erro inesperado";
        var domainException = new DomainException(mensagemErro, ErrorType.UnexpectedError);
        var expectedStatusCode = (int)HttpStatusCode.InternalServerError;

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode);

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Quando ocorre exceção inesperada, deve retornar 500 com mensagem padrão e logar Error")]
    public async Task InvokeAsync_QuandoExcecaoInesperada_DeveRetornar500ELogarError() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var unexpectedException = new InvalidOperationException("Erro inesperado do sistema");
        var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
        var expectedMessage = "Ocorreu um erro interno no servidor.";

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, unexpectedException);

        // Assert
        context.Response.StatusCode.Should().Be(expectedStatusCode, "exceções não-domain devem retornar 500 Internal Server Error");
        context.Response.ContentType.Should().Be("application/json");

        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        using var jsonDoc = JsonDocument.Parse(responseBody);
        var root = jsonDoc.RootElement;

        root.GetProperty("message").GetString().Should().Be(expectedMessage, "a mensagem deve ser a mensagem padrão para erros internos");
        root.GetProperty("statusCode").GetInt32().Should().Be(expectedStatusCode);

        // Não deve expor detalhes da exceção interna
        responseBody.Should().NotContain("Erro inesperado do sistema");

        _fixture.LoggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Resposta JSON deve estar em camelCase")]
    public async Task InvokeAsync_RespostaJSON_DeveEstarEmCamelCase() {
        // Arrange
        var context = _fixture.CriarHttpContext();
        var domainException = new DomainException("Teste", ErrorType.InvalidInput);

        // Act
        await _fixture.InvocarMiddlewareComExcecao(context, domainException);

        // Assert
        var responseBody = ExceptionHandlingMiddlewareTestFixture.LerResponseBody(context);
        
        responseBody.Should().Contain("\"message\":", "a propriedade deve estar em camelCase (message, não Message)");
        responseBody.Should().Contain("\"statusCode\":", "a propriedade deve estar em camelCase (statusCode, não StatusCode)");
    }
}
