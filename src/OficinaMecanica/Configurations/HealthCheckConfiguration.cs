using Infrastructure.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace API.Configurations
{
    /// <summary>
    /// Configuração de verificações de saúde (health checks) da aplicação
    /// </summary>
    public static class HealthCheckConfiguration
    {
        /// <summary>
        /// Adiciona e configura os serviços de verificação de saúde da aplicação
        /// </summary>
        /// <param name="services">Coleção de serviços do container de dependências</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>Coleção de serviços configurada com health checks</returns>
        /// <remarks>
        /// Configura dois tipos de verificações de saúde:
        /// - Self: Verifica se a aplicação está respondendo (usado pelo liveness probe)
        /// - Database: Verifica conectividade com o banco de dados PostgreSQL (usado pelos probes de readiness e startup)
        /// </remarks>
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("A aplicação está viva."), tags: new[] { "live" })
                .AddDbContextCheck<AppDbContext>("database", HealthStatus.Unhealthy, tags: new[] { "ready" });

            return services;
        }

        /// <summary>
        /// Configura os endpoints de verificação de saúde da aplicação
        /// </summary>
        /// <param name="app">Instância da aplicação web</param>
        /// <returns>Aplicação configurada com endpoints de health check</returns>
        /// <remarks>
        /// Mapeia três endpoints de health check:
        /// - /health/live: Verifica se o processo da aplicação está ativo
        /// - /health/ready: Verifica se a aplicação está pronta para receber tráfego, incluindo conectividade com banco
        /// - /health/startup: Verifica se a aplicação inicializou corretamente, reutilizando a verificação de banco
        /// </remarks>
        public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live"),
                ResponseWriter = WriteHealthCheckResponse
            });

            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = WriteHealthCheckResponse
            });

            app.MapHealthChecks("/health/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = WriteHealthCheckResponse
            });

            return app;
        }

        /// <summary>
        /// Escreve uma resposta personalizada em formato JSON para os health checks
        /// </summary>
        /// <param name="context">Contexto HTTP da requisição</param>
        /// <param name="report">Relatório contendo os resultados das verificações de saúde</param>
        /// <remarks>
        /// Gera uma resposta JSON estruturada contendo:
        /// - Status geral das verificações
        /// - Detalhes de cada verificação individual (nome, status, descrição, duração)
        /// - Duração total de todas as verificações
        /// </remarks>
        private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var options = new JsonSerializerOptions { WriteIndented = true };

            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration
                }),
                totalDuration = report.TotalDuration
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}