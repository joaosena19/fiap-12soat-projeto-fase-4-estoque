using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Estoque.UseCases;

public class VerificarDisponibilidadeUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid id, int quantidadeRequisitada, IItemEstoqueGateway gateway, IVerificarDisponibilidadePresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
                throw new DomainException("Acesso negado. Apenas administradores podem verificar estoque.", ErrorType.NotAllowed, "Acesso negado para verificar disponibilidade de estoque para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var item = await gateway.ObterPorIdAsync(id);
            if (item == null)
                throw new DomainException($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound, "Item de estoque não encontrado para Id {ItemId}", id);

            var disponivel = item.VerificarDisponibilidade(quantidadeRequisitada);
            presenter.ApresentarSucesso(item, quantidadeRequisitada, disponivel);
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