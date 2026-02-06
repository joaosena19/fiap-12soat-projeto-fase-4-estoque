using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using Microsoft.AspNetCore.Mvc;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace API.Presenters.OrdemServico
{
    public class GerarOrcamentoPresenter : BasePresenter, IGerarOrcamentoPresenter
    {
        public void ApresentarSucesso(OrdemServicoAggregate ordemServico)
        {
            var retorno = new RetornoOrcamentoDto
            {
                Id = ordemServico.Orcamento!.Id,
                Preco = ordemServico.Orcamento!.Preco.Valor,
                DataCriacao = ordemServico.Orcamento!.DataCriacao.Valor
            };

            var result = new CreatedResult($"/api/ordens-servico/{ordemServico.Id}", retorno);
            DefinirSucesso(result);
        }
    }
}