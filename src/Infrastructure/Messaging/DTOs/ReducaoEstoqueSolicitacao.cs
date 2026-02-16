namespace Infrastructure.Messaging.DTOs;

/// <summary>
/// Mensagem de solicitação de redução de estoque enviada pelo serviço de Ordem de Serviço
/// para o serviço de Estoque via Amazon SQS.
/// </summary>
public record ReducaoEstoqueSolicitacao
{
    /// <summary>
    /// ID de correlação para rastreamento distribuído da saga.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// ID da ordem de serviço que solicitou a redução.
    /// </summary>
    public Guid OrdemServicoId { get; init; }

    /// <summary>
    /// Lista de itens de estoque a serem reduzidos.
    /// </summary>
    public List<ItemReducao> Itens { get; init; } = new();
}

/// <summary>
/// Representa um item de estoque a ser reduzido.
/// </summary>
public record ItemReducao
{
    /// <summary>
    /// ID do item de estoque.
    /// </summary>
    public Guid ItemEstoqueId { get; init; }

    /// <summary>
    /// Quantidade a ser reduzida.
    /// </summary>
    public int Quantidade { get; init; }
}
