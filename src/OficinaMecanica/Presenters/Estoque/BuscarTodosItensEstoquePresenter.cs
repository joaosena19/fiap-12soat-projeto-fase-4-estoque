using Application.Contracts.Presenters;
using Application.Estoque.Dtos;
using Domain.Estoque.Aggregates;

namespace API.Presenters.Estoque
{
    public class BuscarTodosItensEstoquePresenter : BasePresenter, IBuscarTodosItensEstoquePresenter
    {
        public void ApresentarSucesso(IEnumerable<ItemEstoque> itens)
        {
            var retorno = itens.Select(i => new RetornoItemEstoqueDto
            {
                Id = i.Id,
                Nome = i.Nome.Valor,
                Quantidade = i.Quantidade.Valor,
                TipoItemEstoque = i.TipoItemEstoque.Valor.ToString(),
                Preco = i.Preco.Valor
            });

            DefinirSucesso(retorno);
        }
    }
}