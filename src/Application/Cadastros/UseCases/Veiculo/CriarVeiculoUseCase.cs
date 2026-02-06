using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Shared.Exceptions;
using Shared.Enums;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Cadastros.UseCases
{
    public class CriarVeiculoUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo, 
            IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, ICriarVeiculoPresenter presenter, IAppLogger logger)
        {
            try
            {
                if (!ator.PodeCriarVeiculoParaCliente(clienteId))
                    throw new DomainException("Acesso negado.", ErrorType.NotAllowed, "Acesso negado para criação de veículo para cliente {ClienteId} por usuário ator {Ator_UsuarioId}", clienteId, ator.UsuarioId);

                var veiculoExistente = await veiculoGateway.ObterPorPlacaAsync(placa);
                if (veiculoExistente != null)
                    throw new DomainException("Já existe um veículo cadastrado com esta placa.", ErrorType.Conflict, "Tentativa de criação de veículo com placa duplicada {Placa} por usuário ator {Ator_UsuarioId}", placa, ator.UsuarioId);

                var cliente = await clienteGateway.ObterPorIdAsync(clienteId);
                if (cliente == null)
                    throw new DomainException("Cliente não encontrado para realizar associação com o veículo.", ErrorType.ReferenceNotFound, "Cliente não encontrado para ClienteId {ClienteId} ao tentar criar veículo por usuário ator {Ator_UsuarioId}", clienteId, ator.UsuarioId);

                var novoVeiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);
                var veiculoSalvo = await veiculoGateway.SalvarAsync(novoVeiculo);

                presenter.ApresentarSucesso(veiculoSalvo);
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