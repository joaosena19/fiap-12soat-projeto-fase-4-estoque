using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.Contracts.Presenters;

public interface IAdicionarServicosPresenter : IBasePresenter<OrdemServicoAggregate> { }