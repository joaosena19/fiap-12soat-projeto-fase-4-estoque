using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace API.Presenters.OrdemServico
{
    public class CriarOrdemServicoPresenter : BasePresenter, ICriarOrdemServicoPresenter
    {
        public void ApresentarSucesso(OrdemServicoAggregate ordemServico)
        {
            var retorno = new RetornoOrdemServicoDto
            {
                Id = ordemServico.Id,
                Codigo = ordemServico.Codigo.Valor,
                VeiculoId = ordemServico.VeiculoId,
                Status = ordemServico.Status.Valor.ToString(),
                DataCriacao = ordemServico.Historico.DataCriacao
            };

            DefinirSucessoComLocalizacao("GetById", "OrdemServico", new { id = ordemServico.Id }, retorno);
        }
    }
}