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
    public class BuscarClientesUseCase
    {
        public async Task ExecutarAsync(Ator ator, IClienteGateway gateway, IBuscarClientesPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeListarClientes())
                    throw new DomainException("Acesso negado. Somente administradores podem listar clientes.", ErrorType.NotAllowed, "Acesso negado para listar clientes para usuário ator {Ator_UsuarioId}", ator.UsuarioId);

                var clientes = await gateway.ObterTodosAsync();
                presenter.ApresentarSucesso(clientes);
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