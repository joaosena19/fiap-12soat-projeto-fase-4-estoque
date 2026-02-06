using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Application.OrdemServico.Interfaces.External;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

public class AprovarOrcamentoUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IEstoqueExternalService estoqueExternalService, IOperacaoOrdemServicoPresenter presenter, IAppLogger logger)
    {
        try
        {
            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            if (!await ator.PodeAprovarDesaprovarOrcamento(ordemServico, veiculoGateway))
                throw new DomainException("Acesso negado. Apenas administradores ou donos da ordem de serviço podem aprovar orçamentos.", ErrorType.NotAllowed, "Acesso negado para aprovar orçamento da ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            // Verificar disponibilidade dos itens no estoque antes de aprovar o orçamento
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var disponivel = await estoqueExternalService.VerificarDisponibilidadeAsync(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor);

                if (!disponivel)
                    throw new DomainException($"Item '{itemIncluido.Nome.Valor}' não está disponível no estoque na quantidade necessária ({itemIncluido.Quantidade.Valor}).", ErrorType.DomainRuleBroken, "Item {ItemId} não disponível no estoque para quantidade {Quantidade} na ordem {OrdemServicoId}", itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor, ordemServicoId);
            }

            // Se todos os itens estão disponíveis - pode aprovar o orçamento
            ordemServico.AprovarOrcamento();

            // Atualizar as quantidades no estoque após aprovar o orçamento
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var itemEstoque = await estoqueExternalService.ObterItemEstoquePorIdAsync(itemIncluido.ItemEstoqueOriginalId);
                if (itemEstoque != null)
                {
                    var novaQuantidade = itemEstoque.Quantidade - itemIncluido.Quantidade.Valor;
                    await estoqueExternalService.AtualizarQuantidadeEstoqueAsync(itemIncluido.ItemEstoqueOriginalId, novaQuantidade);
                }
            }

            await gateway.AtualizarAsync(ordemServico);
            presenter.ApresentarSucesso();
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