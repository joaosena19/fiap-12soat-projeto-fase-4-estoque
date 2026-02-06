using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Cadastros.UseCases
{
    public class CriarClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, string nome, string documento, IClienteGateway clienteGateway, IUsuarioGateway usuarioGateway, ICriarClientePresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!await ator.PodeCriarClienteAsync(documento, usuarioGateway))
                    throw new DomainException("Acesso negado. Administradores podem cadastrar qualquer cliente, usuários podem criar cliente apenas com o mesmo documento.", ErrorType.NotAllowed, "Acesso negado para criação de cliente com documento {Documento} por usuário ator {Ator_UsuarioId}", documento, ator.UsuarioId);

                var clienteExistente = await clienteGateway.ObterPorDocumentoAsync(documento);
                if (clienteExistente != null)
                    throw new DomainException("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict, "Tentativa de criação de cliente com documento duplicado {Documento} por usuário ator {Ator_UsuarioId}", documento, ator.UsuarioId);

                var novoCliente = Cliente.Criar(nome, documento);
                var clienteSalvo = await clienteGateway.SalvarAsync(novoCliente);

                presenter.ApresentarSucesso(clienteSalvo);
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