using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;
using Shared.Exceptions;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculosUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;
        private readonly MockLogger _mockLogger;

        public BuscarVeiculosUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
            _mockLogger = MockLogger.Criar();
        }

        [Fact(DisplayName = "Deve retornar lista de veículos")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveRetornarListaDeVeiculos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculos = new List<VeiculoAggregate> { new VeiculoBuilder().Build(), new VeiculoBuilder().Build() };
            _fixture.VeiculoGatewayMock.AoObterTodos().Retorna(veiculos);

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>(veiculos);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver veículos")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverVeiculos()
        {
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var listaVazia = new List<VeiculoAggregate>();
            _fixture.VeiculoGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>(listaVazia);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.VeiculoGatewayMock.AoObterTodos().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando usuário não é admin")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoUsuarioNaoEhAdmin()
        {
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoCliente(cliente.Id).Build();

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>("Acesso negado.", ErrorType.NotAllowed);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoCliente(cliente.Id).Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
            mockLogger.NaoDeveTerLogadoNenhumError();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var mockLogger = MockLogger.Criar();
            _fixture.VeiculoGatewayMock.AoObterTodos().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(ator, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}