using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarClientesPresenter : IBasePresenter<IEnumerable<Cliente>> { }
}