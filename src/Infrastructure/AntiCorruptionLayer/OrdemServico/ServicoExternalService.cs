using Application.Contracts.Gateways;
using Application.OrdemServico.Dtos.External;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar serviços do bounded context de Cadastros
    /// </summary>
    public class ServicoExternalService : IServicoExternalService
    {
        private readonly IServicoGateway _servicoGateway;

        public ServicoExternalService(IServicoGateway servicoGateway)
        {
            _servicoGateway = servicoGateway;
        }

        public async Task<ServicoExternalDto?> ObterServicoPorIdAsync(Guid servicoId)
        {
            var servico = await _servicoGateway.ObterPorIdAsync(servicoId);
            
            if (servico == null)
                return null;

            return new ServicoExternalDto
            {
                Id = servico.Id,
                Nome = servico.Nome.Valor,
                Preco = servico.Preco.Valor
            };
        }
    }
}
