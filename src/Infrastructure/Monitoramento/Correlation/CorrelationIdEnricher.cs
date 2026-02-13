using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Monitoramento.Correlation;

/// <summary>
/// Enriquecedor Serilog que adiciona o correlation ID aos eventos de log.
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    /// <summary>
    /// Enriquece o evento de log com o correlation ID do contexto atual.
    /// </summary>
    /// <param name="logEvent">O evento de log a ser enriquecido.</param>
    /// <param name="propertyFactory">Factory para criar propriedades de log.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Se o evento já possui a propriedade CorrelationId, não faz nada
        if (logEvent.Properties.ContainsKey(CorrelationConstants.LogPropertyName))
        {
            return;
        }

        // Obtém o correlation ID do contexto atual
        var correlationId = CorrelationContext.Current;

        // Se houver um correlation ID no contexto, adiciona ao evento de log
        if (!string.IsNullOrEmpty(correlationId))
        {
            var property = new LogEventProperty(
                CorrelationConstants.LogPropertyName,
                new ScalarValue(correlationId));

            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
