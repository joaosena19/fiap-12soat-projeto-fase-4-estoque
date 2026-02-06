using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class CancelarOrdemServicoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem cancelar ordens de serviço.", ErrorType.NotAllowed, "Acesso negado para cancelar ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            ordemServico.Cancelar();
            await gateway.AtualizarAsync(ordemServico);
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
}