using Application.Contracts.Monitoramento;
using Application.Extensions;
using Application.Extensions.Enums;
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
    /// <param name="logger">Logger para registrar a publicação</param>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <returns>Task representando a operação assíncrona</returns>
    public async Task PublicarSucessoAsync(IAppLogger logger, ConsumeContext context, string correlationId, Guid ordemServicoId)
    {
        logger
            .ComMensageria(NomeMensagemEnum.ReducaoEstoqueResultado, TipoMensagemEnum.Publicacao)
            .LogInformation("Publicando resultado de sucesso da redução de estoque para OS {OrdemServicoId}", ordemServicoId);

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
    /// <param name="logger">Logger para registrar a publicação</param>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <param name="motivoFalha">Motivo da falha na redução</param>
    /// <returns>Task representando a operação assíncrona</returns>
    public async Task PublicarFalhaAsync(IAppLogger logger, ConsumeContext context, string correlationId, Guid ordemServicoId, string motivoFalha)
    {
        logger
            .ComMensageria(NomeMensagemEnum.ReducaoEstoqueResultado, TipoMensagemEnum.Publicacao)
            .LogWarning("Publicando resultado de falha da redução de estoque para OS {OrdemServicoId}. Motivo: {MotivoFalha}", ordemServicoId, motivoFalha);

        await context.Publish(new ReducaoEstoqueResultado
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Sucesso = false,
            MotivoFalha = motivoFalha
        });
    }
}