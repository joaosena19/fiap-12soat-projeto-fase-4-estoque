using Application.Contracts.Presenters;
using Application.Estoque.Dtos;
using Domain.Estoque.Aggregates;

namespace API.Presenters.Estoque
{
    public class VerificarDisponibilidadePresenter : BasePresenter, IVerificarDisponibilidadePresenter
    {
        public void ApresentarSucesso(ItemEstoque itemEstoque, int quantidadeRequisitada, bool disponivel)
        {
            var retorno = new RetornoDisponibilidadeDto
            {
                Disponivel = disponivel,
                QuantidadeEmEstoque = itemEstoque.Quantidade.Valor,
                QuantidadeSolicitada = quantidadeRequisitada
            };

            DefinirSucesso(retorno);
        }
    }
}