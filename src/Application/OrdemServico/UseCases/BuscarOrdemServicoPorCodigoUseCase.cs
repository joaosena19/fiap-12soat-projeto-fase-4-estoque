using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdemServicoPorCodigoUseCase
{
    public async Task ExecutarAsync(Ator ator, string codigo, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorCodigoPresenter presenter, IAppLogger logger)
    {
        try
        {
            var ordemServico = await gateway.ObterPorCodigoAsync(codigo);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para código {CodigoOrdemServico}", codigo);

            if (!await ator.PodeAcessarOrdemServicoAsync(ordemServico, veiculoGateway))
                throw new DomainException("Acesso negado. Apenas administradores ou donos da ordem de serviço podem visualizá-la.", ErrorType.NotAllowed, "Acesso negado para visualizar ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServico.Id, ator.UsuarioId);

            presenter.ApresentarSucesso(ordemServico);
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