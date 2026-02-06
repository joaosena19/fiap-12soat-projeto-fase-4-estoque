using Application.Contracts.Monitoramento;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Infrastructure.Monitoramento;

public class LoggerAdapter<T> : IAppLogger
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string messageTemplate, params object[] args)
    {
        using (LogContext.PushProperty("message_template", messageTemplate))
        {
            _logger.LogInformation(messageTemplate, args);
        }
    }

    public void LogWarning(string messageTemplate, params object[] args)
    {
        using (LogContext.PushProperty("message_template", messageTemplate))
        {
            _logger.LogWarning(messageTemplate, args);
        }
    }

    public void LogError(string messageTemplate, params object[] args)
    {
        using (LogContext.PushProperty("message_template", messageTemplate))
        {
            _logger.LogError(messageTemplate, args);
        }
    }

    public void LogError(Exception ex, string messageTemplate, params object[] args)
    {
        using (LogContext.PushProperty("message_template", messageTemplate))
        {
            _logger.LogError(ex, messageTemplate, args);
        }
    }

    public IAppLogger ComPropriedade(string key, object? value)
    {
        var context = new Dictionary<string, object?> { [key] = value };
        return new ContextualLogger(_logger, context);
    }
}