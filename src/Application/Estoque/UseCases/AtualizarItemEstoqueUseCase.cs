using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Estoque.Enums;
using Shared.Exceptions;
using Shared.Enums;
using Application.Identidade.Services.Extensions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Estoque.UseCases;

public class AtualizarItemEstoqueUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid id, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, IAtualizarItemEstoquePresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
                throw new DomainException("Acesso negado. Apenas administradores podem atualizar estoque.", ErrorType.NotAllowed, "Acesso negado para atualizar item de estoque para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var itemExistente = await gateway.ObterPorIdAsync(id);
            if (itemExistente == null)
                throw new DomainException($"Item de estoque com ID {id} não foi encontrado", ErrorType.ResourceNotFound, "Item de estoque não encontrado para Id {ItemId}", id);

            itemExistente.Atualizar(nome, quantidade, tipoItemEstoque, preco);
            var itemAtualizado = await gateway.AtualizarAsync(itemExistente);

            presenter.ApresentarSucesso(itemAtualizado);
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