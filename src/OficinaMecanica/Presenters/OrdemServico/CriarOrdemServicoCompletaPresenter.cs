using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace API.Presenters.OrdemServico;

public class CriarOrdemServicoCompletaPresenter : BasePresenter, ICriarOrdemServicoCompletaPresenter
{
    public void ApresentarSucesso(OrdemServicoAggregate ordemServico)
    {
        var retorno = new RetornoOrdemServicoDto
        {
            Id = ordemServico.Id,
            VeiculoId = ordemServico.VeiculoId,
            Codigo = ordemServico.Codigo.Valor,
            Status = ordemServico.Status.Valor.ToString(),
            DataCriacao = ordemServico.Historico.DataCriacao
        };

        DefinirSucessoComLocalizacao("GetById", "OrdemServico", new { id = ordemServico.Id }, retorno);
    }
}