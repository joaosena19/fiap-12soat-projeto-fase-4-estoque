using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class BuscarItemEstoquePorIdUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public BuscarItemEstoquePorIdUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve buscar item de estoque com sucesso quando item existir")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveBuscarItemEstoqueComSucesso_QuandoItemExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>(itemExistente);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não existir")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>($"Item de estoque com ID {itemId} não foi encontrado", ErrorType.ResourceNotFound);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de acesso negado quando cliente tentar buscar item do estoque")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveApresentarErroAcessoNegado_QuandoClienteTentarBuscarItemEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemId = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarItemEstoquePorIdPresenterMock.DeveTerApresentadoErro<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>("Acesso negado. Apenas administradores podem buscar estoque.", ErrorType.NotAllowed);
            _fixture.BuscarItemEstoquePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarItemEstoquePorIdPresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarItemEstoquePorId")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarItemEstoquePorIdUseCase.ExecutarAsync(
                ator,
                itemId,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarItemEstoquePorIdPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}