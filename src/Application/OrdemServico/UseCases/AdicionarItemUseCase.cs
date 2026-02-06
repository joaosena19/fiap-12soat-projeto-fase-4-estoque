using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class AdicionarItemUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, Guid itemEstoqueOriginalId, int quantidade, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IAdicionarItemPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem adicionar itens.", ErrorType.NotAllowed, "Acesso negado para adicionar item na ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            var itemEstoque = await estoqueExternalService.ObterItemEstoquePorIdAsync(itemEstoqueOriginalId);
            if (itemEstoque == null)
                throw new DomainException($"Item de estoque com ID {itemEstoqueOriginalId} não encontrado.", ErrorType.ReferenceNotFound, "Item de estoque não encontrado para Id {ItemEstoqueId}", itemEstoqueOriginalId);

            ordemServico.AdicionarItem(
                itemEstoque.Id,
                itemEstoque.Nome,
                itemEstoque.Preco,
                quantidade,
                itemEstoque.TipoItemIncluido);

            var result = await gateway.AtualizarAsync(ordemServico);
            presenter.ApresentarSucesso(result);
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