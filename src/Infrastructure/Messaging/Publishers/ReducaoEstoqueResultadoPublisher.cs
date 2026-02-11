using Infrastructure.Messaging.DTOs;
using MassTransit;

namespace Infrastructure.Messaging.Publishers;

/// <summary>
/// Publisher responsável por publicar mensagens de resultado de redução de estoque.
/// Implementa o padrão Pure DI, aceitando apenas primitivos do MassTransit no construtor.
/// Extrai a lógica de publicação do consumer para melhor separação de responsabilidades.
/// </summary>
public class ReducaoEstoqueResultadoPublisher : IReducaoEstoqueResultadoPublisher
{
    /// <summary>
    /// Publica uma mensagem de resultado de redução de estoque com sucesso.
    /// </summary>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <returns>Task representando a operação assíncrona</returns>
    public async Task PublicarSucessoAsync(ConsumeContext context, Guid correlationId, Guid ordemServicoId)
    {
        await context.Publish(new ReducaoEstoqueResultado
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Sucesso = true
        });
    }

    /// <summary>
    /// Publica uma mensagem de resultado de redução de estoque com falha.
    /// </summary>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <param name="motivoFalha">Motivo da falha na redução</param>
    /// <returns>Task representando a operação assíncrona</returns>
    public async Task PublicarFalhaAsync(ConsumeContext context, Guid correlationId, Guid ordemServicoId, string motivoFalha)
    {
        await context.Publish(new ReducaoEstoqueResultado
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Sucesso = false,
            MotivoFalha = motivoFalha
        });
    }
}