using Serilog.Context;

namespace API.Middleware;

/// <summary>
/// Middleware que garante a propagação do header X-Correlation-ID em todas as requisições.
/// Se a requisição não contiver o header, gera um novo GUID como correlation ID.
/// O correlation ID é incluído na resposta e enriquecido no contexto de log do Serilog.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(CorrelationIdHeader))
        {
            context.Request.Headers.Append(CorrelationIdHeader, Guid.NewGuid().ToString());
        }

        var correlationId = context.Request.Headers[CorrelationIdHeader].ToString();
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
