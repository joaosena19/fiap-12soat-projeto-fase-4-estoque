using Domain.Identidade.Aggregates;

namespace Application.Contracts.Gateways
{
    public interface IUsuarioGateway
    {
        Task<Usuario> SalvarAsync(Usuario usuario);
        Task<Usuario?> ObterPorDocumentoAsync(string documento);
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<List<Role>> ObterRolesAsync(IEnumerable<string> roleStrings);
    }
}