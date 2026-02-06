using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdemServicoPorIdUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdemServicoPorIdUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir e ator for administrador")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistirEAtorForAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var id = ordemServico.Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).Retorna(ordemServico);

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir e ator for dono do veículo")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistirEAtorForDonoDoVeiculo()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();
            var id = ordemServico.Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tentar buscar ordem de serviço de outro cliente")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentarBuscarOrdemServicoDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(outroClienteId).Build(); // Veículo de outro cliente
            var id = ordemServico.Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>("Acesso negado. Apenas administradores ou donos da ordem de serviço podem visualizá-la.", ErrorType.NotAllowed);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdemServicoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorIdPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar Information quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdemServicoPorId")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(id).LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.BuscarOrdemServicoPorIdUseCase.ExecutarAsync(
                ator,
                id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorIdPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}