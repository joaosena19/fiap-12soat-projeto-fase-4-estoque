using Application.Contracts.Gateways;
using Domain.Cadastros.Aggregates;

namespace Application.Identidade.Services.Extensions;

public static class AtorVeiculoExtensions
{
    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static bool PodeAcessarVeiculo(this Ator ator, Veiculo veiculo)
    {
        return ator.PodeGerenciarSistema() || veiculo.ClienteId == ator.ClienteId;
    }

    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeAcessarVeiculoAsync(this Ator ator, Guid veiculoId, IVeiculoGateway gateway)
    {
        if (ator.PodeGerenciarSistema()) return true;

        var veiculo = await gateway.ObterPorIdAsync(veiculoId);
        return veiculo != null && ator.PodeAcessarVeiculo(veiculo);
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeCriarVeiculoParaCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || ator.ClienteId == clienteId;
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeListarVeiculosDoCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || ator.ClienteId == clienteId;
    }

    /// <summary>
    /// Somente administrador pode listar todos os veículos
    /// </summary>
    public static bool PodeListarTodosVeiculos(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }
}