using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Filters;
using MassTransit;
using System.Text.Json.Serialization;

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
                var region = configuration["AWS:Region"] ?? "us-east-1";

                cfg.Host(region, h =>
                {
                    var accessKey = configuration["AWS:AccessKeyId"];
                    var secretKey = configuration["AWS:SecretAccessKey"];

                    if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                    {
                        h.AccessKey(accessKey);
                        h.SecretKey(secretKey);
                    }
                });

                // Registrar filtros globais de correlação
                cfg.UseConsumeFilter(typeof(ConsumeCorrelationIdFilter<>), context);
                cfg.UseSendFilter(typeof(SendCorrelationIdFilter<>), context);
                cfg.UsePublishFilter(typeof(PublishCorrelationIdFilter<>), context);

                cfg.ReceiveEndpoint("fase4-estoque-reducao-estoque-solicitacao", e =>
                {
                    e.ConfigureConsumer<ReducaoEstoqueSolicitacaoConsumer>(context);
                    
                    // Retry policy: 3 tentativas com 5 segundos de intervalo
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });

                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.Converters.Add(new JsonStringEnumConverter());
                    return options;
                });
            });
        });

        return services;
    }
}
