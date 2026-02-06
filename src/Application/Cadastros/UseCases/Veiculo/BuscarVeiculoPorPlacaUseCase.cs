using Application.Contracts.Gateways;
using Application.Contracts.Monitoramento;
using Application.Contracts.Presenters;
using Application.Extensions;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Cadastros.UseCases
{
    public class BuscarVeiculoPorPlacaUseCase
    {
        public async Task ExecutarAsync(Ator ator, string placa, IVeiculoGateway gateway, IBuscarVeiculoPorPlacaPresenter presenter, IAppLogger logger)
        {
            try
            {
                var veiculo = await gateway.ObterPorPlacaAsync(placa);
                if (veiculo == null)
                    throw new DomainException("Veículo não encontrado.", ErrorType.ResourceNotFound, "Veículo não encontrado para Placa {Placa}", placa);

                if (!ator.PodeAcessarVeiculo(veiculo))
                    throw new DomainException("Acesso negado. Somente administradores ou o proprietário do veículo podem visualizá-lo.", ErrorType.NotAllowed, "Acesso negado ao veículo {VeiculoId} com placa {Placa} para usuário ator {Ator_UsuarioId}", veiculo.Id, placa, ator.UsuarioId);

                presenter.ApresentarSucesso(veiculo);
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