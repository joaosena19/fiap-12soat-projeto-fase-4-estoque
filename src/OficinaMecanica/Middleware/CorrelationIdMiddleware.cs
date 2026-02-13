using Infrastructure.Monitoramento.Correlation;
using Serilog.Context;

namespace API.Middleware;

/// <summary>
/// Middleware que garante a propagação do header X-Correlation-ID em todas as requisições.
/// Se a requisição não contiver o header, gera um novo GUID como correlation ID.
/// Preserva valores não-GUID recebidos (não valida formato).
/// O correlation ID é incluído na resposta e enriquecido no contexto de log do Serilog.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Obter o primeiro valor do header X-Correlation-ID
        var headerValue = context.Request.Headers[CorrelationConstants.HeaderName].FirstOrDefault();
        
        // Se ausente ou vazio/whitespace, gerar novo GUID
        string correlationId;
        if (string.IsNullOrWhiteSpace(headerValue))
        {
            correlationId = Guid.NewGuid().ToString();
        }
        else
        {
            // Preservar o valor recebido sem validação de formato
            correlationId = headerValue;
        }
        
        // Normalizar request header para conter exatamente um valor
        if (context.Request.Headers.ContainsKey(CorrelationConstants.HeaderName))
        {
            context.Request.Headers.Remove(CorrelationConstants.HeaderName);
        }
        context.Request.Headers.Append(CorrelationConstants.HeaderName, correlationId);
        
        // Definir response header usando OnStarting para evitar duplicatas
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Envolver pipeline downstream com dois escopos
        using (CorrelationContext.Push(correlationId))
        using (LogContext.PushProperty(CorrelationConstants.LogPropertyName, correlationId))
        {
            await _next(context);
        }
    }
}
