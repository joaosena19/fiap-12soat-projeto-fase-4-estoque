using API.Presenters;
using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace API.Presenters.OrdemServico
{
    public class BuscarOrdemServicoPorIdPresenter : BasePresenter, IBuscarOrdemServicoPorIdPresenter
    {
        public void ApresentarSucesso(OrdemServicoAggregate ordemServico)
        {
            var retorno = new RetornoOrdemServicoCompletaDto
            {
                Id = ordemServico.Id,
                Codigo = ordemServico.Codigo.Valor,
                VeiculoId = ordemServico.VeiculoId,
                Status = ordemServico.Status.Valor.ToString(),
                DataCriacao = ordemServico.Historico.DataCriacao,
                DataEntrega = ordemServico.Historico.DataEntrega,
                ServicosIncluidos = ordemServico.ServicosIncluidos?.Select(s => new RetornoServicoIncluidoDto
                {
                    Id = s.Id,
                    ServicoOriginalId = s.ServicoOriginalId,
                    Nome = s.Nome.Valor,
                    Preco = s.Preco.Valor
                }).ToList() ?? new List<RetornoServicoIncluidoDto>(),
                ItensIncluidos = ordemServico.ItensIncluidos?.Select(i => new RetornoItemIncluidoDto
                {
                    Id = i.Id,
                    ItemEstoqueOriginalId = i.ItemEstoqueOriginalId,
                    Nome = i.Nome.Valor,
                    Preco = i.Preco.Valor,
                    Quantidade = i.Quantidade.Valor,
                    TipoItemIncluido = i.TipoItemIncluido.Valor.ToString()
                }).ToList() ?? new List<RetornoItemIncluidoDto>(),
                Orcamento = ordemServico.Orcamento != null ? new RetornoOrcamentoDto
                {
                    Id = ordemServico.Orcamento.Id,
                    Preco = ordemServico.Orcamento.Preco.Valor,
                    DataCriacao = ordemServico.Orcamento.DataCriacao.Valor
                } : null
            };

            DefinirSucesso(retorno);
        }
    }
}