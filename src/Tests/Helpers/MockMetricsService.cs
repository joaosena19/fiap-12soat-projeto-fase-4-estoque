using Application.Contracts.Monitoramento;

namespace Tests.Helpers;

/// <summary>
/// Mock do IMetricsService para testes de integração.
/// Este mock não registra métricas reais, apenas retorna sem fazer nada.
/// </summary>
public class MockMetricsService : IMetricsService
{
    public void RegistrarOrdemServicoCriada(Guid ordemServicoId, Guid clienteId, Guid usuarioId)
    {
        // Mock não faz nada - apenas para testes não quebrarem
    }

    public void RegistrarMudancaOrdemServicoStatus(Guid ordemServicoId, string statusAnterior, string statusNovo, double duracaoMs)
    {
        // Mock não faz nada - apenas para testes não quebrarem
    }
}