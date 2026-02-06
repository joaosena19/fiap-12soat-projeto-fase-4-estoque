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

public class IniciarDiagnosticoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IAppLogger logger, IMetricsService metricsService)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem iniciar diagnósticos.", ErrorType.NotAllowed, "Acesso negado para iniciar diagnóstico da ordem de serviço {OrdemServicoId} pelo usuário ator {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            var statusAnterior = ordemServico.Status.Valor;
            ordemServico.IniciarDiagnostico();
            await gateway.AtualizarAsync(ordemServico);
            var statusNovo = ordemServico.Status.Valor;
            
            RegistrarMetricaTempoDiagnostico(ordemServico, statusAnterior, statusNovo, metricsService, ator, logger);
            
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

    private void RegistrarMetricaTempoDiagnostico(OS ordemServico, StatusOrdemServicoEnum statusAnterior, StatusOrdemServicoEnum statusNovo, IMetricsService metricsService, Ator ator, IAppLogger logger)
    {
        try
        {
            // Mede o tempo entre a criação da ordem e o momento atual (início do diagnóstico)
            var duracaoMs = (DateTime.UtcNow - ordemServico.Historico.DataCriacao).TotalMilliseconds;
            metricsService.RegistrarMudancaOrdemServicoStatus(ordemServico.Id, statusAnterior.ToString(), statusNovo.ToString(), duracaoMs);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro ao registrar métrica de tempo de diagnóstico. OrdemServicoId: {OrdemServicoId}", ordemServico.Id);
        }
    }
}