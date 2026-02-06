using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class DesaprovarOrcamentoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IOperacaoOrdemServicoPresenter presenter, IAppLogger logger)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            if (!await ator.PodeAprovarDesaprovarOrcamento(ordemServico, veiculoGateway))
                throw new DomainException("Acesso negado. Apenas administradores ou donos da ordem de serviço podem desaprovar orçamentos.", ErrorType.NotAllowed, "Acesso negado para desaprovar orçamento da ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            ordemServico.DesaprovarOrcamento();
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