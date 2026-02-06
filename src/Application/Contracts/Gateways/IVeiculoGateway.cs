using Domain.Cadastros.Aggregates;

namespace Application.Contracts.Gateways
{
    public interface IVeiculoGateway
    {
        Task<Veiculo> SalvarAsync(Veiculo veiculo);
        Task<Veiculo?> ObterPorPlacaAsync(string placa);
        Task<Veiculo?> ObterPorIdAsync(Guid id);
        Task<Veiculo> AtualizarAsync(Veiculo veiculo);
        Task<IEnumerable<Veiculo>> ObterTodosAsync();
        Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(Guid clienteId);
    }
}