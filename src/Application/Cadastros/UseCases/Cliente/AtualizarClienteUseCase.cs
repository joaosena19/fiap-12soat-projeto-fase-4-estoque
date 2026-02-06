using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Exceptions;
using Shared.Enums;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Cadastros.UseCases
{
    public class AtualizarClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, string nome, IClienteGateway gateway, IAtualizarClientePresenter presenter, IAppLogger logger)
        {
            try
            {
                var cliente = await gateway.ObterPorIdAsync(id);
                if (cliente == null)
                    throw new DomainException("Cliente não encontrado.", ErrorType.ResourceNotFound, "Cliente não encontrado para Id {ClienteId}", id);

                if (!ator.PodeEditarCliente(cliente.Id))
                    throw new DomainException("Acesso negado. Somente administradores ou o próprio cliente podem editar os dados.", ErrorType.NotAllowed, "Acesso negado para edição do cliente {ClienteId} por usuário ator {Ator_UsuarioId}", cliente.Id, ator.UsuarioId);

                cliente.Atualizar(nome);
                var clienteAtualizado = await gateway.AtualizarAsync(cliente);

                presenter.ApresentarSucesso(clienteAtualizado);
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