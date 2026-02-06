using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Enums;
using Shared.Exceptions;
using Shared.Enums;
using Application.Extensions;
using Application.Contracts.Monitoramento;

namespace Application.Cadastros.UseCases
{
    public class AtualizarVeiculoUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo, IVeiculoGateway gateway, IAtualizarVeiculoPresenter presenter, IAppLogger logger)
        {
            try
            {
                var veiculo = await gateway.ObterPorIdAsync(id);
                if (veiculo == null)
                    throw new DomainException("Veículo não encontrado.", ErrorType.ResourceNotFound, "Veículo não encontrado para Id {VeiculoId}", id);

                if (!ator.PodeAcessarVeiculo(veiculo))
                    throw new DomainException("Acesso negado ao veículo.", ErrorType.NotAllowed, "Acesso negado ao veículo {VeiculoId} para usuário ator {Ator_UsuarioId}", id, ator.UsuarioId);

                veiculo.Atualizar(modelo, marca, cor, ano, tipoVeiculo);
                var veiculoAtualizado = await gateway.AtualizarAsync(veiculo);

                presenter.ApresentarSucesso(veiculoAtualizado);
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