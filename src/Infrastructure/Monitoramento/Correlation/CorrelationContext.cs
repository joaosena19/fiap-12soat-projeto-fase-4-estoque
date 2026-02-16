namespace Infrastructure.Monitoramento.Correlation;

/// <summary>
/// Armazena o correlation ID atual de forma thread-safe usando AsyncLocal.
/// </summary>
public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> _currentCorrelationId = new();

    /// <summary>
    /// Obtém o correlation ID atual do contexto assíncrono.
    /// </summary>
    public static string? Current => _currentCorrelationId.Value;

    /// <summary>
    /// Define o correlation ID no contexto assíncrono.
    /// Retorna um IDisposable que restaura o valor anterior quando descartado.
    /// </summary>
    /// <param name="correlationId">O correlation ID a ser definido.</param>
    /// <returns>Um IDisposable que restaura o valor anterior quando descartado.</returns>
    public static IDisposable Push(string correlationId)
    {
        return new CorrelationScope(correlationId);
    }

    private sealed class CorrelationScope : IDisposable
    {
        private readonly string? _previousValue;
        private bool _disposed;

        public CorrelationScope(string correlationId)
        {
            _previousValue = _currentCorrelationId.Value;
            _currentCorrelationId.Value = correlationId;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _currentCorrelationId.Value = _previousValue;
                _disposed = true;
            }
        }
    }
}
