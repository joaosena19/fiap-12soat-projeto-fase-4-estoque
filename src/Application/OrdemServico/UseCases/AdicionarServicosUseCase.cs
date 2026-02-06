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

public class AdicionarServicosUseCase
{
    public async Task ExecutarAsync(Ator ator, Guid ordemServicoId, List<Guid> servicosOriginaisIds, IOrdemServicoGateway gateway, IServicoExternalService servicoExternalService, IAdicionarServicosPresenter presenter, IAppLogger logger)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem adicionar serviços.", ErrorType.NotAllowed, "Acesso negado para adicionar serviços na ordem de serviço {OrdemServicoId} para usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            if (servicosOriginaisIds == null || servicosOriginaisIds.Count == 0)
                throw new DomainException("É necessário informar ao menos um serviço para adicionar na Ordem de Serviço", ErrorType.InvalidInput, "Lista de serviços vazia para ordem de serviço {OrdemServicoId} pelo usuário {Ator_UsuarioId}", ordemServicoId, ator.UsuarioId);

            var ordemServico = await gateway.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound, "Ordem de serviço não encontrada para Id {OrdemServicoId}", ordemServicoId);

            foreach (var servicoId in servicosOriginaisIds)
            {
                var servico = await servicoExternalService.ObterServicoPorIdAsync(servicoId);
                if (servico == null)
                    throw new DomainException($"Serviço com ID {servicoId} não encontrado.", ErrorType.ReferenceNotFound, "Serviço não encontrado para Id {ServicoId}", servicoId);

                ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            }

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