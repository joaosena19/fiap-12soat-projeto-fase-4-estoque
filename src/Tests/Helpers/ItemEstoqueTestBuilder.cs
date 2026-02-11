using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;

namespace Tests.Helpers;

/// <summary>
/// Builder para criação de entidades ItemEstoque em testes.
/// Facilita a construção de itens com valores padrão e customizáveis.
/// </summary>
public class ItemEstoqueTestBuilder
{
    private string _nome = "Item Teste";
    private int _quantidade = 10;
    private TipoItemEstoqueEnum _tipo = TipoItemEstoqueEnum.Peca;
    private decimal _preco = 100.00m;

    public ItemEstoqueTestBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public ItemEstoqueTestBuilder ComQuantidade(int quantidade)
    {
        _quantidade = quantidade;
        return this;
    }

    public ItemEstoqueTestBuilder ComTipo(TipoItemEstoqueEnum tipo)
    {
        _tipo = tipo;
        return this;
    }

    public ItemEstoqueTestBuilder ComPreco(decimal preco)
    {
        _preco = preco;
        return this;
    }

    public ItemEstoque Build()
    {
        return ItemEstoque.Criar(_nome, _quantidade, _tipo, _preco);
    }

    /// <summary>
    /// Cria uma nova instância do builder.
    /// </summary>
    public static ItemEstoqueTestBuilder Novo() => new();
}
