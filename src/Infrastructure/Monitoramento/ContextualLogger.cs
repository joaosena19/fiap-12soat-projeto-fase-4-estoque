using Application.Contracts.Monitoramento;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Infrastructure.Monitoramento;

public class ContextualLogger : IAppLogger
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, object?> _context;

    public ContextualLogger(ILogger logger, Dictionary<string, object?> context)
    {
        _logger = logger;
        _context = context;
    }

    private void LogWithContext(string messageTemplate, Action logAction)
    {
        var disposables = new List<IDisposable>();
        
        // Adiciona o template da mensagem como propriedade
        disposables.Add(LogContext.PushProperty("message_template", messageTemplate));

        foreach (var kvp in _context)
        {
            if (kvp.Value != null)
            {
                disposables.Add(LogContext.PushProperty(kvp.Key, kvp.Value));
            }
        }

        try
        {
            logAction();
        }
        finally
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }

    public void LogInformation(string messageTemplate, params object[] args)
    {
        LogWithContext(messageTemplate, () => _logger.LogInformation(messageTemplate, args));
    }

    public void LogWarning(string messageTemplate, params object[] args)
    {
        LogWithContext(messageTemplate, () => _logger.LogWarning(messageTemplate, args));
    }

    public void LogError(string messageTemplate, params object[] args)
    {
        LogWithContext(messageTemplate, () => _logger.LogError(messageTemplate, args));
    }

    public void LogError(Exception ex, string messageTemplate, params object[] args)
    {
        LogWithContext(messageTemplate, () => _logger.LogError(ex, messageTemplate, args));
    }

    public IAppLogger ComPropriedade(string key, object? value)
    {
        var newContext = new Dictionary<string, object?>(_context)
        {
            [key] = value
        };
        return new ContextualLogger(_logger, newContext);
    }
}