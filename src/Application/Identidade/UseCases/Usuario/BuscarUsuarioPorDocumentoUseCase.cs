using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Identidade.UseCases.Usuario
{
    public class BuscarUsuarioPorDocumentoUseCase
    {
        public async Task ExecutarAsync(Ator ator, string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeGerenciarUsuarios())
                    throw new DomainException("Acesso negado. Apenas administradores podem buscar usuários.", ErrorType.NotAllowed, "Acesso negado para buscar usuário por documento para usuário {Ator_UsuarioId}", ator.UsuarioId);

                var usuario = await gateway.ObterPorDocumentoAsync(documento);
                if (usuario == null)
                    throw new DomainException("Usuário não encontrado.", ErrorType.ResourceNotFound, "Usuário não encontrado para documento {Documento}", documento);

                presenter.ApresentarSucesso(usuario);
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