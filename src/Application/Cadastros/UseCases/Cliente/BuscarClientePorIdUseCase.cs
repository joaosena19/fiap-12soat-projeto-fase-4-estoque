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
    public class BuscarClientePorIdUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, IClienteGateway gateway, IBuscarClientePorIdPresenter presenter, IAppLogger logger)
        {
            try
            {
                var cliente = await gateway.ObterPorIdAsync(id);
                if (cliente == null)
                    throw new DomainException("Cliente não encontrado.", ErrorType.ResourceNotFound, "Cliente não encontrado para Id {ClienteId}", id);

                if (!ator.PodeAcessarCliente(cliente))
                    throw new DomainException("Acesso negado. Somente administradores ou o próprio cliente podem acessar os dados.", ErrorType.NotAllowed, "Acesso negado ao cliente {ClienteId} para usuário ator {Ator_UsuarioId}", cliente.Id, ator.UsuarioId);

                presenter.ApresentarSucesso(cliente);
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