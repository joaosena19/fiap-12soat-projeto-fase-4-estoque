using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Infrastructure.Monitoramento;

/// <summary>
/// Implementação do ICorrelationIdAccessor que lê do CorrelationContext.
/// Funciona em qualquer contexto (HTTP, consumer, background service) onde um correlation scope foi estabelecido.
/// </summary>
public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    public string GetCorrelationId()
    {
        // Preferir CorrelationContext (funciona em HTTP, consumers, background services)
        var current = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(current))
        {
            return current;
        }

        // Fallback: gerar novo GUID para contextos sem scope estabelecido
        return Guid.NewGuid().ToString();
    }
}
