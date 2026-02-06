using Application.Contracts.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.Identidade.Services.Extensions;

public static class AtorOrdemServicoExtensions
{
    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeAcessarOrdemServicoAsync(this Ator ator, OrdemServicoAggregate ordemServico, IVeiculoGateway veiculoGateway)
    {
        if (ator.PodeGerenciarSistema()) return true;

        var veiculo = await veiculoGateway.ObterPorIdAsync(ordemServico.VeiculoId);
        return veiculo?.ClienteId == ator.ClienteId;
    }

    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeAcessarOrdemServicoAsync(this Ator ator, Guid ordemServicoId, IOrdemServicoGateway ordemServicoGateway, IVeiculoGateway veiculoGateway)
    {
        if (ator.PodeGerenciarSistema()) return true;

        var ordemServico = await ordemServicoGateway.ObterPorIdAsync(ordemServicoId);
        if (ordemServico == null) return false;

        return await ator.PodeAcessarOrdemServicoAsync(ordemServico, veiculoGateway);
    }

    /// <summary>
    /// Administrador ou dono do veículo
    /// </summary>
    public static async Task<bool> PodeCriarOrdemServicoParaVeiculoAsync(this Ator ator, Guid veiculoId, IVeiculoGateway veiculoGateway)
    {
        return await ator.PodeAcessarVeiculoAsync(veiculoId, veiculoGateway);
    }

    /// <summary>
    /// Somente administrador ou sistema (webhooks)
    /// </summary>
    public static bool PodeAtualizarStatusOrdem(this Ator ator)
    {
        return ator.PodeGerenciarSistema() || ator.PodeAcionarWebhooks();
    }

    /// <summary>
    /// Verifica se o ator tem permissão para gerenciar ordens de serviço.
    /// Apenas administradores podem gerenciar ordens de serviço.
    /// </summary>
    /// <param name="ator">O ator que está tentando realizar a operação</param>
    /// <returns>True se o ator pode gerenciar ordens de serviço, False caso contrário</returns>
    public static bool PodeGerenciarOrdemServico(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }

    /// <summary>
    /// Verifica se o ator pode aprovar/desaprovar orçamentos.
    /// Administradores, donos da ordem de serviço ou sistema (webhooks) podem aprovar/desaprovar orçamentos.
    /// </summary>
    /// <param name="ator">O ator que está tentando realizar a operação</param>
    /// <param name="ordemServico">A ordem de serviço relacionada ao orçamento</param>
    /// <param name="veiculoGateway">Gateway para acessar dados dos veículos</param>
    /// <returns>True se o ator pode aprovar/desaprovar orçamentos, False caso contrário</returns>
    public static async Task<bool> PodeAprovarDesaprovarOrcamento(this Ator ator, OrdemServicoAggregate ordemServico, IVeiculoGateway veiculoGateway)
    {
        // Administradores e sistema podem aprovar/desaprovar qualquer orçamento
        if (ator.PodeGerenciarSistema() || ator.PodeAcionarWebhooks()) return true;

        // Cliente pode aprovar/desaprovar orçamento apenas se for dono do veículo
        var veiculo = await veiculoGateway.ObterPorIdAsync(ordemServico.VeiculoId);
        return veiculo?.ClienteId == ator.ClienteId;
    }
}