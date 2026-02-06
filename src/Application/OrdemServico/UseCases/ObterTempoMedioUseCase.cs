using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class ObterTempoMedioUseCase
{
    public async Task ExecutarAsync(Ator ator, int quantidadeDias, IOrdemServicoGateway gateway, IObterTempoMedioPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem obter tempo médio de execução.", ErrorType.NotAllowed, "Acesso negado para obter tempo médio pelo usuário ator {Ator_UsuarioId} com {QuantidadeDias} dias", ator.UsuarioId, quantidadeDias);

            if (quantidadeDias < 1 || quantidadeDias > 365)
                throw new DomainException("A quantidade de dias deve estar entre 1 e 365.", ErrorType.InvalidInput, "Quantidade de dias inválida: {QuantidadeDias}. Deve estar entre 1 e 365", quantidadeDias);

            var ordensEntregues = await gateway.ObterEntreguesUltimosDiasAsync(quantidadeDias);
            if (!ordensEntregues.Any())
                throw new DomainException("Nenhuma ordem de serviço entregue encontrada no período especificado.", ErrorType.DomainRuleBroken, "Nenhuma ordem entregue nos últimos {QuantidadeDias} dias", quantidadeDias);

            // Calcular tempo médio completo (criação até entrega)
            var duracaoCompleta = ordensEntregues
                .Select(ordem => ordem.Historico.DataEntrega!.Value - ordem.Historico.DataCriacao)
                .ToList();

            var mediaCompletaTicks = duracaoCompleta.Average(d => d.Ticks);
            var duracaoMediaCompleta = new TimeSpan((long)mediaCompletaTicks);
            var tempoMedioCompletoHoras = Math.Round(duracaoMediaCompleta.TotalHours, 2);

            // Calcular tempo médio de execução (início execução até finalização)
            var duracaoExecucao = ordensEntregues
                .Select(ordem => ordem.Historico.DataFinalizacao!.Value - ordem.Historico.DataInicioExecucao!.Value)
                .ToList();

            var mediaExecucaoTicks = duracaoExecucao.Average(d => d.Ticks);
            var duracaoMediaExecucao = new TimeSpan((long)mediaExecucaoTicks);
            var tempoMedioExecucaoHoras = Math.Round(duracaoMediaExecucao.TotalHours, 2);

            presenter.ApresentarSucesso(
                quantidadeDias,
                DateTime.UtcNow.AddDays(-quantidadeDias),
                DateTime.UtcNow,
                ordensEntregues.Count(),
                tempoMedioCompletoHoras,
                tempoMedioExecucaoHoras);
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
}