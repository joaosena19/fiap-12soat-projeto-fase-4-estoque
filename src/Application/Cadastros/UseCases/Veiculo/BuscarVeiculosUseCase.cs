using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculosUseCase
    {
        public async Task ExecutarAsync(Ator ator, IVeiculoGateway gateway, IBuscarVeiculosPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeListarTodosVeiculos())
                    throw new DomainException("Acesso negado.", ErrorType.NotAllowed, "Acesso negado para listagem de todos os veículos para usuário ator {Ator_UsuarioId}", ator.UsuarioId);

                var veiculos = await gateway.ObterTodosAsync();
                presenter.ApresentarSucesso(veiculos);
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
}