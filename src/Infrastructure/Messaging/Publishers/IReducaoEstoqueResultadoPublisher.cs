using MassTransit;

namespace Infrastructure.Messaging.Publishers;

/// <summary>
/// Interface para o publisher de resultados de redução de estoque.
/// Permite testes unitários isolados e inversão de dependência quando necessário.
/// </summary>
public interface IReducaoEstoqueResultadoPublisher
{
    /// <summary>
    /// Publica uma mensagem de resultado de redução de estoque com sucesso.
    /// </summary>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task PublicarSucessoAsync(ConsumeContext context, Guid correlationId, Guid ordemServicoId);

    /// <summary>
    /// Publica uma mensagem de resultado de redução de estoque com falha.
    /// </summary>
    /// <param name="context">Contexto de consumo do MassTransit para publicação</param>
    /// <param name="correlationId">ID de correlação da saga</param>
    /// <param name="ordemServicoId">ID da ordem de serviço</param>
    /// <param name="motivoFalha">Motivo da falha na redução</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task PublicarFalhaAsync(ConsumeContext context, Guid correlationId, Guid ordemServicoId, string motivoFalha);
}