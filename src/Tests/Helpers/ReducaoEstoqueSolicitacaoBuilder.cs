using Infrastructure.Messaging.DTOs;

namespace Tests.Helpers;

/// <summary>
/// Builder para criação de objetos ReducaoEstoqueSolicitacao em testes.
/// Facilita a construção de mensagens com valores padrão e customizáveis.
/// </summary>
public class ReducaoEstoqueSolicitacaoBuilder
{
    private Guid _correlationId = Guid.NewGuid();
    private Guid _ordemServicoId = Guid.NewGuid();
    private List<ItemReducao> _itens = new();

    public ReducaoEstoqueSolicitacaoBuilder ComCorrelationId(Guid correlationId)
    {
        _correlationId = correlationId;
        return this;
    }

    public ReducaoEstoqueSolicitacaoBuilder ComOrdemServicoId(Guid ordemServicoId)
    {
        _ordemServicoId = ordemServicoId;
        return this;
    }

    public ReducaoEstoqueSolicitacaoBuilder ComItem(Guid itemEstoqueId, int quantidade)
    {
        _itens.Add(new ItemReducao 
        { 
            ItemEstoqueId = itemEstoqueId, 
            Quantidade = quantidade 
        });
        return this;
    }

    public ReducaoEstoqueSolicitacaoBuilder ComItens(List<ItemReducao> itens)
    {
        _itens = itens;
        return this;
    }

    public ReducaoEstoqueSolicitacao Build()
    {
        return new ReducaoEstoqueSolicitacao
        {
            CorrelationId = _correlationId,
            OrdemServicoId = _ordemServicoId,
            Itens = _itens
        };
    }

    /// <summary>
    /// Cria uma nova instância do builder com novos GUIDs.
    /// </summary>
    public static ReducaoEstoqueSolicitacaoBuilder Novo() => new();
}
