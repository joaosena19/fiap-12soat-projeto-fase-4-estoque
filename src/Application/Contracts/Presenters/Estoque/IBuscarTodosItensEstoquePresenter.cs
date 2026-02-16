using Domain.Estoque.Aggregates;

namespace Application.Contracts.Presenters;

public interface IBuscarTodosItensEstoquePresenter : IBasePresenter<IEnumerable<ItemEstoque>> { }