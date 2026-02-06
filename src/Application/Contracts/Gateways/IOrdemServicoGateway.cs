using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.Contracts.Gateways;

public interface IOrdemServicoGateway
{
    Task<OrdemServicoAggregate> SalvarAsync(OrdemServicoAggregate ordemServico);
    Task<OrdemServicoAggregate?> ObterPorIdAsync(Guid id);
    Task<OrdemServicoAggregate?> ObterPorCodigoAsync(string codigo);
    Task<OrdemServicoAggregate> AtualizarAsync(OrdemServicoAggregate ordemServico);
    Task<IEnumerable<OrdemServicoAggregate>> ObterTodosAsync();
    Task<IEnumerable<OrdemServicoAggregate>> ObterEntreguesUltimosDiasAsync(int quantidadeDias);
}