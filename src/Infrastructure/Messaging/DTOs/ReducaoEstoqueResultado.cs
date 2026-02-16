namespace Infrastructure.Messaging.DTOs;

/// <summary>
/// Mensagem de resultado de redução de estoque enviada pelo serviço de Estoque
/// de volta ao serviço de Ordem de Serviço via Amazon SQS.
/// </summary>
public record ReducaoEstoqueResultado
{
    /// <summary>
    /// ID de correlação para rastreamento distribuído da saga.
    /// Deve corresponder ao CorrelationId da solicitação.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// ID da ordem de serviço relacionada.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Indica se a redução de estoque foi bem-sucedida.
    /// </summary>
    public bool Sucesso { get; init; }

    /// <summary>
    /// Motivo da falha, caso Sucesso seja false.
    /// Valores possíveis: "estoque_insuficiente", "erro_interno", "servico_indisponivel"
    /// </summary>
    public string? MotivoFalha { get; init; }
}
