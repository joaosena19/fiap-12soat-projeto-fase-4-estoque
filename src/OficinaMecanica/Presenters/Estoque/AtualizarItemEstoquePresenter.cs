using API.Presenters;
using Application.Contracts.Presenters;
using Application.Estoque.Dtos;
using Domain.Estoque.Aggregates;
using Shared.Enums;

namespace API.Presenters.Estoque
{
    public class AtualizarItemEstoquePresenter : BasePresenter, IAtualizarItemEstoquePresenter
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

            DefinirSucesso(retorno);
        }
    }
}