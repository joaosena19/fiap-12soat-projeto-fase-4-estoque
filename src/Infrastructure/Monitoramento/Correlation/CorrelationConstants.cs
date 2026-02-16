namespace Infrastructure.Monitoramento.Correlation;

/// <summary>
/// Constantes relacionadas ao correlation ID.
/// </summary>
public static class CorrelationConstants
{
    /// <summary>
    /// Nome do header HTTP usado para o correlation ID.
    /// </summary>
    public const string HeaderName = "X-Correlation-ID";

    /// <summary>
    /// Nome da propriedade de log usada para o correlation ID.
    /// </summary>
    public const string LogPropertyName = "CorrelationId";
}
