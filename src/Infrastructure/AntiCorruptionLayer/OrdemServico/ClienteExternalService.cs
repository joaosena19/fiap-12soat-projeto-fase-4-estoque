using Application.Contracts.Gateways;
using Application.OrdemServico.Dtos.External;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar clientes do bounded context de Cadastros
    /// </summary>
    public class ClienteExternalService : IClienteExternalService
    {
        private readonly IVeiculoGateway _veiculoGateway;
        private readonly IClienteGateway _clienteGateway;

        public ClienteExternalService(IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway)
        {
            _veiculoGateway = veiculoGateway;
            _clienteGateway = clienteGateway;
        }

        public async Task<ClienteExternalDto?> ObterClientePorVeiculoIdAsync(Guid veiculoId)
        {
            var veiculo = await _veiculoGateway.ObterPorIdAsync(veiculoId);
            if (veiculo == null)
                return null;

            var cliente = await _clienteGateway.ObterPorIdAsync(veiculo.ClienteId);
            if (cliente == null)
                return null;

            return new ClienteExternalDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor
            };
        }
    }
}
