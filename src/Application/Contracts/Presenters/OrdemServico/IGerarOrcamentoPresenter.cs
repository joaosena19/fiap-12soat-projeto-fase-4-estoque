using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IGerarOrcamentoPresenter
{
    void ApresentarSucesso(OrdemServicoAggregate ordemServico);
    void ApresentarErro(string mensagem, ErrorType errorType);
}