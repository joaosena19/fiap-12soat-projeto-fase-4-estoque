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
    public class CriarServicoUseCase
    {
        public async Task ExecutarAsync(Ator ator, string nome, decimal preco, IServicoGateway gateway, ICriarServicoPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                    throw new DomainException("Acesso negado. Apenas administradores podem criar serviços.", ErrorType.NotAllowed, "Acesso negado para criar serviços para usuário ator {Ator_UsuarioId}", ator.UsuarioId);

                var servicoExistente = await gateway.ObterPorNomeAsync(nome);
                if (servicoExistente != null)
                    throw new DomainException("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict, "Serviço já existente com nome {Nome}", nome);

                var novoServico = Servico.Criar(nome, preco);
                var servicoSalvo = await gateway.SalvarAsync(novoServico);

                presenter.ApresentarSucesso(servicoSalvo);
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