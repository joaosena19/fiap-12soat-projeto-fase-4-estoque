using Application.OrdemServico.Dtos.External;

namespace Application.OrdemServico.Interfaces.External
{
    /// <summary>
    /// Interface anti-corruption para acessar dados do bounded context de Cadastros (Clientes)
    /// </summary>
    public interface IClienteExternalService
    {
        Task<ClienteExternalDto?> ObterClientePorVeiculoIdAsync(Guid veiculoId);
    }
}
