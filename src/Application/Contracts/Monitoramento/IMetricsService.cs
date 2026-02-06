namespace Application.Contracts.Monitoramento
{
    public interface IMetricsService
    {
        /// <summary>
        /// Registra o evento de criação de uma nova OS (Para dashboard de Volume)
        /// </summary>
        void RegistrarOrdemServicoCriada(Guid ordemServicoId, Guid clienteId, Guid usuarioId);

        /// <summary>
        /// Registra a transição de status e quanto tempo a OS ficou no status anterior (Para dashboard de Tempo Médio)
        /// </summary>
        void RegistrarMudancaOrdemServicoStatus(Guid ordemServicoId, string statusAnterior, string statusNovo, double duracaoMs);
    }
}