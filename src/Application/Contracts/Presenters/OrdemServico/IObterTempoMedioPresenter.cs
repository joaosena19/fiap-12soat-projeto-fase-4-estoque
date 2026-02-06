using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IObterTempoMedioPresenter
{
    void ApresentarSucesso(int quantidadeDias, DateTime dataInicio, DateTime dataFim, int quantidadeOrdensAnalisadas, double tempoMedioCompletoHoras, double tempoMedioExecucaoHoras);
    void ApresentarErro(string mensagem, ErrorType errorType);
}