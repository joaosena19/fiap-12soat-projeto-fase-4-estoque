using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;
using Application.Identidade.Dtos;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Identidade.Aggregates;
using Domain.Identidade.ValueObjects;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using UsuarioAggregate = Domain.Identidade.Aggregates.Usuario;
using Application.Contracts.Monitoramento;

namespace Application.Identidade.UseCases.Usuario
{
    public class CriarUsuarioUseCase
    {
        public async Task ExecutarAsync(Ator ator, CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter, IPasswordHasher passwordHasher, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeGerenciarUsuarios())
                    throw new DomainException("Acesso negado. Apenas administradores podem criar usuários.", ErrorType.NotAllowed, "Acesso negado para criar usuário para usuário {Ator_UsuarioId}", ator.UsuarioId);

                var usuarioExistente = await gateway.ObterPorDocumentoAsync(dto.DocumentoIdentificador);
                if (usuarioExistente != null)
                    throw new DomainException("Já existe um usuário cadastrado com este documento.", ErrorType.Conflict, "Já existe usuário com documento {Documento} para usuário {Ator_UsuarioId}", dto.DocumentoIdentificador, ator.UsuarioId);

                // Busca as roles existentes no banco ao invés de criar novas instâncias
                var roles = await gateway.ObterRolesAsync(dto.Roles);
                
                if (roles.Count != dto.Roles.Count)
                    throw new DomainException("Uma ou mais roles informadas são inválidas.", ErrorType.InvalidInput, "Roles inválidas {Roles} para usuário {Ator_UsuarioId}", string.Join(",", dto.Roles), ator.UsuarioId);

                var senhaHasheada = passwordHasher.Hash(dto.SenhaNaoHasheada);
                var senhaHash = new SenhaHash(senhaHasheada);

                var novoUsuario = UsuarioAggregate.Criar(dto.DocumentoIdentificador, senhaHash.Valor, roles);
                var usuarioSalvo = await gateway.SalvarAsync(novoUsuario);

                presenter.ApresentarSucesso(usuarioSalvo);
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