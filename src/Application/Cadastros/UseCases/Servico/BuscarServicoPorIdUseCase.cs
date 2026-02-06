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
    public class BuscarServicoPorIdUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, IServicoGateway gateway, IBuscarServicoPorIdPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                    throw new DomainException("Acesso negado. Apenas administradores podem buscar serviços.", ErrorType.NotAllowed, "Acesso negado para buscar serviços para usuário ator {Ator_UsuarioId}", ator.UsuarioId);

                var servico = await gateway.ObterPorIdAsync(id);
                if (servico == null)
                    throw new DomainException("Serviço não encontrado.", ErrorType.ResourceNotFound, "Serviço não encontrado para Id {ServicoId}", id);

                presenter.ApresentarSucesso(servico);
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