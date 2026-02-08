using Infrastructure.Messaging.Consumers;
using MassTransit;

namespace API.Configurations;

/// <summary>
/// Configuração de mensageria usando MassTransit + Amazon SQS.
///Implementa o padrão Saga coreografado conforme especificado no Plano de Execução E-03.
/// </summary>
public static class MessagingConfiguration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Registrar consumer de solicitação de redução de estoque
            x.AddConsumer<ReducaoEstoqueSolicitacaoConsumer>();

            // Configurar Amazon SQS como transport
            x.UsingAmazonSqs((context, cfg) =>
            {
                // Configurar região AWS
                cfg.Host("us-east-1", h =>
                {
                    // Credenciais via IAM role do pod - sem necessidade de access key
                    // A role anexada ao node group do EKS já tem permissão
                });

                // Configurar endpoint para receber mensagens de solicitação
                cfg.ReceiveEndpoint("fase4-estoque-reducao-estoque-solicitacao", e =>
                {
                    e.ConfigureConsumer<ReducaoEstoqueSolicitacaoConsumer>(context);
                    
                    // Configurar retry policy: 3 tentativas com 5 segundos de intervalo
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                // Configurar formato de mensagens
                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    return options;
                });
            });
        });

        return services;
    }
}
