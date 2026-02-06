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
    public class BuscarVeiculosPorClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid clienteId, IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, IBuscarVeiculosPorClientePresenter presenter, IAppLogger logger)
        {
            try
            {
                var cliente = await clienteGateway.ObterPorIdAsync(clienteId);
                if (cliente == null)
                    throw new DomainException("Cliente não encontrado.", ErrorType.ReferenceNotFound, "Cliente não encontrado para ClienteId {ClienteId}", clienteId);

                if (!ator.PodeListarVeiculosDoCliente(clienteId))
                    throw new DomainException("Acesso negado. Somente administradores ou o próprio cliente podem visualizar seus veículos.", ErrorType.NotAllowed, "Acesso negado para listar veículos do cliente {ClienteId} para usuário ator {Ator_UsuarioId}", clienteId, ator.UsuarioId);

                var veiculos = await veiculoGateway.ObterPorClienteIdAsync(clienteId);
                presenter.ApresentarSucesso(veiculos);
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