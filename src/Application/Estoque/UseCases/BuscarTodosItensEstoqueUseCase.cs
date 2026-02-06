using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Estoque.UseCases;

public class BuscarTodosItensEstoqueUseCase
{
    public async Task ExecutarAsync(Ator ator, IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarEstoque())
                throw new DomainException("Acesso negado. Apenas administradores podem listar estoque.", ErrorType.NotAllowed, "Acesso negado para listar itens de estoque para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var itens = await gateway.ObterTodosAsync();
            presenter.ApresentarSucesso(itens);
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