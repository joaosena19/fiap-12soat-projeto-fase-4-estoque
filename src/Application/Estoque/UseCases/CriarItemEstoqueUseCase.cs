using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Shared.Exceptions;
using Shared.Enums;
using Application.Identidade.Services.Extensions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Estoque.UseCases;

public class CriarItemEstoqueUseCase
{
    public async Task ExecutarAsync(Ator ator, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
                throw new DomainException("Acesso negado. Apenas administradores podem criar estoque.", ErrorType.NotAllowed, "Acesso negado para criar item de estoque para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var itemExistente = await gateway.ObterPorNomeAsync(nome);
            if (itemExistente != null)
                throw new DomainException("Já existe um item de estoque cadastrado com este nome.", ErrorType.Conflict, "Já existe item de estoque com nome {Nome} para usuário {Ator_UsuarioId}", nome, ator.UsuarioId);

            var novoItemEstoque = ItemEstoque.Criar(nome, quantidade, tipoItemEstoque, preco);
            var itemSalvo = await gateway.SalvarAsync(novoItemEstoque);

            presenter.ApresentarSucesso(itemSalvo);
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