using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Application.OrdemServico.Interfaces.External;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class CriarOrdemServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, ICriarOrdemServicoPresenter presenter, IAppLogger logger, IClienteExternalService clienteExternalService, IMetricsService metricsService)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem criar ordens de serviço.", ErrorType.NotAllowed, "Acesso negado para criar ordem de serviço para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var veiculoExiste = await veiculoExternalService.VerificarExistenciaVeiculo(veiculoId);
            if (!veiculoExiste)
                throw new DomainException("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound, "Veículo não encontrado para Id {VeiculoId}", veiculoId);

            OrdemServicoAggregate novaOrdemServico;
            OrdemServicoAggregate? ordemServicoExistente;

            // Gerar código único
            do
            {
                novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);
                ordemServicoExistente = await gateway.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
            } while (ordemServicoExistente != null);

            var result = await gateway.SalvarAsync(novaOrdemServico);

            await RegistrarMetricaOrdemServicoCriadaAsync(result.Id, veiculoId, ator, clienteExternalService, metricsService, logger);

            presenter.ApresentarSucesso(result);
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

    /// <summary>
    /// Registra a métrica de ordem de serviço criada. Não lança exceções em caso de falha.
    /// </summary>
    private async Task RegistrarMetricaOrdemServicoCriadaAsync(Guid ordemServicoId, Guid veiculoId, Ator ator, IClienteExternalService clienteExternalService, IMetricsService metricsService, IAppLogger logger)
    {
        try
        {
            var cliente = await clienteExternalService.ObterClientePorVeiculoIdAsync(veiculoId);
            if (cliente != null)
            {
                metricsService.RegistrarOrdemServicoCriada(ordemServicoId, cliente.Id, ator.UsuarioId);
            }
            else
            {
                logger.ComUseCase(this)
                      .ComAtor(ator)
                      .LogWarning("Cliente não encontrado para VeiculoId {VeiculoId} ao registrar métrica. OrdemServicoId: {OrdemServicoId}", veiculoId, ordemServicoId);
            }
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro ao registrar métrica de ordem de serviço criada. OrdemServicoId: {OrdemServicoId}", ordemServicoId);
        }
    }
}