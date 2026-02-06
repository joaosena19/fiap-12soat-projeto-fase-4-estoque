using API.Presenters;
using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using Shared.Enums;

namespace API.Presenters.OrdemServico
{
    public class ObterTempoMedioPresenter : BasePresenter, IObterTempoMedioPresenter
    {
        public void ApresentarSucesso(int quantidadeDias, DateTime dataInicio, DateTime dataFim, int quantidadeOrdensAnalisadas, double tempoMedioCompletoHoras, double tempoMedioExecucaoHoras)
        {
            var retorno = new RetornoTempoMedioDto
            {
                QuantidadeDias = quantidadeDias,
                DataInicio = dataInicio,
                DataFim = dataFim,
                QuantidadeOrdensAnalisadas = quantidadeOrdensAnalisadas,
                TempoMedioCompletoHoras = tempoMedioCompletoHoras,
                TempoMedioExecucaoHoras = tempoMedioExecucaoHoras
            };

            DefinirSucesso(retorno);
        }
    }
}