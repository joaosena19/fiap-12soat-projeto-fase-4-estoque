using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Filtro MassTransit que propaga correlation ID automaticamente em todas as mensagens enviadas (Send).
/// Lê do CorrelationContext e adiciona ao header e envelope MassTransit.
/// </summary>
/// <typeparam name="T">Tipo da mensagem sendo enviada</typeparam>
public class SendCorrelationIdFilter<T> : IFilter<SendContext<T>> where T : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("correlationId");
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        // Obter correlation ID do contexto atual
        var correlationId = CorrelationContext.Current ?? Guid.NewGuid().ToString();

        // Adicionar ao header da mensagem (interoperabilidade)
        context.Headers.Set(CorrelationConstants.HeaderName, correlationId);

        // Se for GUID válido, também setar no envelope MassTransit (best-effort)
        if (Guid.TryParse(correlationId, out var guid))
        {
            context.CorrelationId = guid;
        }

        await next.Send(context);
    }
}
