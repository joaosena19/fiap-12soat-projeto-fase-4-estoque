using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdensServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem listar ordens de serviço.", ErrorType.NotAllowed, "Acesso negado para listar ordens de serviço para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var ordensServico = await gateway.ObterTodosAsync();
            
            // Filtrar
            var ordensAtivas = ordensServico.Where(os => 
                os.Status.Valor != StatusOrdemServicoEnum.Finalizada && 
                os.Status.Valor != StatusOrdemServicoEnum.Entregue &&
                os.Status.Valor != StatusOrdemServicoEnum.Cancelada);

            // Ordenar
            var ordensOrdenadas = ordensAtivas
                .OrderBy(os => ObterPrioridadeStatus(os.Status.Valor))
                .ThenBy(os => os.Historico.DataCriacao);
            
            presenter.ApresentarSucesso(ordensOrdenadas);
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
    
    private static int ObterPrioridadeStatus(StatusOrdemServicoEnum status)
    {
        return status switch
        {
            StatusOrdemServicoEnum.EmExecucao => 1,
            StatusOrdemServicoEnum.AguardandoAprovacao => 2,
            StatusOrdemServicoEnum.EmDiagnostico => 3,
            StatusOrdemServicoEnum.Recebida => 4,
            _ => 5
        };
    }
}