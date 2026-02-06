using Domain.Cadastros.Aggregates;

namespace Application.Contracts.Gateways
{
    public interface IServicoGateway
    {
        Task<Servico> SalvarAsync(Servico servico);
        Task<Servico?> ObterPorIdAsync(Guid id);
        Task<Servico?> ObterPorNomeAsync(string nome);
        Task<Servico> AtualizarAsync(Servico servico);
        Task<IEnumerable<Servico>> ObterTodosAsync();
    }
}