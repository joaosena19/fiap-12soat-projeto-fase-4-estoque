using Domain.Cadastros.Aggregates;

namespace Application.Contracts.Presenters
{
    public interface IBuscarServicosPresenter : IBasePresenter<IEnumerable<Servico>> { }
}