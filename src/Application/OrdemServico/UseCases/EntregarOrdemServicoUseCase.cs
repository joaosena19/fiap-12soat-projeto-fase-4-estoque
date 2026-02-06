using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;
using Domain.OrdemServico.Enums;
using OS = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Application.OrdemServico.UseCases;

public class EntregarOrdemServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IAppLogger logger, IMetricsService metricsService)
    {
        try
        {
            // Validar permissão - apenas administradores podem entregar ordens de serviço
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem entregar ordens de serviço.", ErrorType.NotAllowed, "Acesso negado para entregar ordem de serviço {OrdemServicoId} pelo usuário ator {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            var statusAnterior = ordemServico.Status.Valor;
            ordemServico.Entregar();
            await gateway.AtualizarAsync(ordemServico);
            var statusNovo = ordemServico.Status.Valor;
            
            RegistrarMetricaTempoFinalizacao(ordemServico, statusAnterior, statusNovo, metricsService, ator, logger);
            
            presenter.ApresentarSucesso();
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .ComDomainErrorType(ex)
                  .LogInformation(ex.LogTemplate, ex.LogArgs);

            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro interno do servidor.");

            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }

    private void RegistrarMetricaTempoFinalizacao(OS ordemServico, StatusOrdemServicoEnum statusAnterior, StatusOrdemServicoEnum statusNovo, IMetricsService metricsService, Ator ator, IAppLogger logger)
    {
        try
        {
            // O próprio Value Object HistoricoTemporal garante que as datas não serão nulas.
            var duracaoMs = (ordemServico.Historico.DataEntrega!.Value - ordemServico.Historico.DataFinalizacao!.Value).TotalMilliseconds;
            metricsService.RegistrarMudancaOrdemServicoStatus(ordemServico.Id, statusAnterior.ToString(), statusNovo.ToString(), duracaoMs);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro ao registrar métrica de tempo de finalização. OrdemServicoId: {OrdemServicoId}", ordemServico.Id);
        }
    }
}