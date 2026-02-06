using API.Presenters;
using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace API.Presenters.OrdemServico
{
    public class BuscarOrdensServicoPresenter : BasePresenter, IBuscarOrdensServicoPresenter
    {
        public void ApresentarSucesso(IEnumerable<OrdemServicoAggregate> ordensServico)
        {
            var retorno = ordensServico.Select(os => new RetornoOrdemServicoCompletaDto
            {
                Id = os.Id,
                Codigo = os.Codigo.Valor,
                VeiculoId = os.VeiculoId,
                Status = os.Status.Valor.ToString(),
                DataCriacao = os.Historico.DataCriacao,
                DataEntrega = os.Historico.DataEntrega,
                ServicosIncluidos = os.ServicosIncluidos?.Select(s => new RetornoServicoIncluidoDto
                {
                    Id = s.Id,
                    ServicoOriginalId = s.ServicoOriginalId,
                    Nome = s.Nome.Valor,
                    Preco = s.Preco.Valor
                }).ToList() ?? new List<RetornoServicoIncluidoDto>(),
                ItensIncluidos = os.ItensIncluidos?.Select(i => new RetornoItemIncluidoDto
                {
                    Id = i.Id,
                    ItemEstoqueOriginalId = i.ItemEstoqueOriginalId,
                    Nome = i.Nome.Valor,
                    Preco = i.Preco.Valor,
                    Quantidade = i.Quantidade.Valor,
                    TipoItemIncluido = i.TipoItemIncluido.Valor.ToString()
                }).ToList() ?? new List<RetornoItemIncluidoDto>(),
                Orcamento = os.Orcamento != null ? new RetornoOrcamentoDto
                {
                    Id = os.Orcamento.Id,
                    Preco = os.Orcamento.Preco.Valor,
                    DataCriacao = os.Orcamento.DataCriacao.Valor
                } : null
            });

            DefinirSucesso(retorno);
        }
    }
}