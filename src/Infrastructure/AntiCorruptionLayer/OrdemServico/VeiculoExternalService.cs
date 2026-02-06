using Application.Contracts.Gateways;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar veículos do bounded context de Cadastros
    /// </summary>
    public class VeiculoExternalService : IVeiculoExternalService
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public VeiculoExternalService(IVeiculoGateway veiculoGateway)
        {
            _veiculoGateway = veiculoGateway;
        }

        public async Task<bool> VerificarExistenciaVeiculo(Guid veiculoId)
        {
            var veiculo = await _veiculoGateway.ObterPorIdAsync(veiculoId);
            return veiculo != null;
        }
    }
}
