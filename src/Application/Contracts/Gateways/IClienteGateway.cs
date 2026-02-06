using Domain.Cadastros.Aggregates;

namespace Application.Contracts.Gateways
{
    public interface IClienteGateway
    {
        Task<Cliente> SalvarAsync(Cliente cliente);
        Task<Cliente?> ObterPorDocumentoAsync(string documento);
        Task<Cliente?> ObterPorIdAsync(Guid id);
        Task<Cliente> AtualizarAsync(Cliente cliente);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
    }
}