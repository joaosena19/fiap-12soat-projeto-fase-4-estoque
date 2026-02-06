using Application.OrdemServico.Dtos.External;

namespace Application.OrdemServico.Interfaces.External
{
    /// <summary>
    /// Interface anti-corruption para acessar dados do bounded context de Cadastros (Veículos)
    /// </summary>
    public interface IVeiculoExternalService
    {
        Task<bool> VerificarExistenciaVeiculo(Guid veiculoId);
    }
}
