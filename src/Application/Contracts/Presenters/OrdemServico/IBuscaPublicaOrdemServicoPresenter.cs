using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.Contracts.Presenters;

public interface IBuscaPublicaOrdemServicoPresenter : IBasePresenter<OrdemServicoAggregate>
{
    void ApresentarNaoEncontrado();
}