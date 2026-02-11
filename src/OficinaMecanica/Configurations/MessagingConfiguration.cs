using Infrastructure.Messaging.Consumers;
using MassTransit;

namespace API.Configurations;

/// <summary>
/// Configura MassTransit com Amazon SQS para mensageria Saga coreografado (E-03).
/// </summary>
public static class MessagingConfiguration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReducaoEstoqueSolicitacaoConsumer>();

            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    // Credenciais via IAM role do pod - sem necessidade de access key
                    // A role anexada ao node group do EKS já tem permissão
                });

                cfg.ReceiveEndpoint("fase4-estoque-reducao-estoque-solicitacao", e =>
                {
                    e.ConfigureConsumer<ReducaoEstoqueSolicitacaoConsumer>(context);
                    
                    // Retry policy: 3 tentativas com 5 segundos de intervalo
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

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
