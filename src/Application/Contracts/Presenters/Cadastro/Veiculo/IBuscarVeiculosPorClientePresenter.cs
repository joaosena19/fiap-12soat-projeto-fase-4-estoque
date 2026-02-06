using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarVeiculosPorClientePresenter : IBasePresenter<IEnumerable<Veiculo>> { }
}