using Domain.Estoque.Aggregates;

namespace Application.Contracts.Gateways;

public interface IItemEstoqueGateway
{
    Task<ItemEstoque> SalvarAsync(ItemEstoque itemEstoque);
    Task<ItemEstoque?> ObterPorIdAsync(Guid id);
    Task<ItemEstoque?> ObterPorNomeAsync(string nome);
    Task<ItemEstoque> AtualizarAsync(ItemEstoque itemEstoque);
    Task<IEnumerable<ItemEstoque>> ObterTodosAsync();
}