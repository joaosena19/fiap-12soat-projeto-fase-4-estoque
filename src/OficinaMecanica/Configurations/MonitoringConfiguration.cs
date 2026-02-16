using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;

namespace API.Configurations;

/// <summary>
/// Configuração de monitoramento e observabilidade.
/// </summary>
public static class MonitoringConfiguration
{
    /// <summary>
    /// Registra serviços de monitoramento (correlation ID, etc).
    /// </summary>
    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        // Registrar HttpContextAccessor (necessário para middleware)
        services.AddHttpContextAccessor();

        // Registrar o Accessor de Correlation ID
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        return services;
    }
}
