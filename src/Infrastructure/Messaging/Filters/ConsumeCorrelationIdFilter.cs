using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Filtro MassTransit que estabelece correlation scope para todos os consumers.
/// Extrai correlation ID de headers/envelope/mensagem e enriquece logs automaticamente.
/// </summary>
/// <typeparam name="T">Tipo da mensagem sendo consumida</typeparam>
public class ConsumeCorrelationIdFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("correlationId");
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        // Tentar resolver correlation ID em ordem de precedência:
        // 1. Header X-Correlation-ID (string)
        // 2. MassTransit envelope CorrelationId (Guid?)
        // 3. Propriedade CorrelationId na mensagem (via reflexão)
        // 4. Gerar novo GUID

        string correlationId = ExtractCorrelationId(context);

        // Estabelecer scopes para logging e acesso via ICorrelationIdAccessor
        using (CorrelationContext.Push(correlationId))
        using (Serilog.Context.LogContext.PushProperty(CorrelationConstants.LogPropertyName, correlationId))
        {
            await next.Send(context);
        }
    }

    private string ExtractCorrelationId(ConsumeContext<T> context)
    {
        // 1. Tentar header X-Correlation-ID
        if (context.Headers.TryGetHeader(CorrelationConstants.HeaderName, out var headerValue) 
            && headerValue is string headerString 
            && !string.IsNullOrWhiteSpace(headerString))
        {
            return headerString;
        }

        // 2. Tentar envelope CorrelationId do MassTransit
        if (context.CorrelationId.HasValue)
        {
            return context.CorrelationId.Value.ToString();
        }

        // 3. Tentar propriedade CorrelationId da mensagem (reflexão)
        var messageType = context.Message.GetType();
        var correlationIdProperty = messageType.GetProperty("CorrelationId");
        if (correlationIdProperty != null)
        {
            var value = correlationIdProperty.GetValue(context.Message);
            if (value != null)
            {
                // Suporta tanto Guid quanto string
                var valueStr = value.ToString();
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    return valueStr;
                }
            }
        }

        // 4. Fallback: gerar novo GUID
        return Guid.NewGuid().ToString();
    }
}
