using API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.API.Middleware.Helpers;

/// <summary>
/// Fixture para testes do ExceptionHandlingMiddleware
/// Centraliza criação de mocks e helpers para setup de testes
/// </summary>
public class ExceptionHandlingMiddlewareTestFixture
{
    public Mock<ILogger<ExceptionHandlingMiddleware>> LoggerMock { get; }

    public ExceptionHandlingMiddlewareTestFixture() {
        LoggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
    }

    /// <summary>
    /// Cria um DefaultHttpContext configurado para testes de middleware
    /// </summary>
    /// <returns>HttpContext com Response.Body configurado como MemoryStream</returns>
    public DefaultHttpContext CriarHttpContext() {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    /// <summary>
    /// Invoca o middleware com um RequestDelegate que lança uma exceção específica
    /// </summary>
    /// <param name="context">HttpContext para o middleware</param>
    /// <param name="exception">Exceção a ser lançada pelo RequestDelegate</param>
    public async Task InvocarMiddlewareComExcecao(HttpContext context, Exception exception) {
        var middleware = new ExceptionHandlingMiddleware(
            next: _ => throw exception,
            logger: LoggerMock.Object
        );

        await middleware.InvokeAsync(context);
    }

    /// <summary>
    /// Lê o corpo da resposta HTTP como string
    /// </summary>
    /// <param name="context">HttpContext com resposta populada</param>
    /// <returns>Conteúdo do Response.Body como string</returns>
    public static string LerResponseBody(HttpContext context) {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return reader.ReadToEnd();
    }
}
