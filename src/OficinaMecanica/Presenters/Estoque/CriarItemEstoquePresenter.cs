using Application.Contracts.Presenters;
using Application.Estoque.Dtos;
using Domain.Estoque.Aggregates;

namespace API.Presenters.Estoque
{
    public class CriarItemEstoquePresenter : BasePresenter, ICriarItemEstoquePresenter
    {
        public void ApresentarSucesso(ItemEstoque itemEstoque)
        {
            var retorno = new RetornoItemEstoqueDto
            {
                Id = itemEstoque.Id,
                Nome = itemEstoque.Nome.Valor,
                Quantidade = itemEstoque.Quantidade.Valor,
                TipoItemEstoque = itemEstoque.TipoItemEstoque.Valor.ToString(),
                Preco = itemEstoque.Preco.Valor
            };

            DefinirSucessoComLocalizacao("GetById", "EstoqueItem", new { id = itemEstoque.Id }, retorno);
        }
    }
}