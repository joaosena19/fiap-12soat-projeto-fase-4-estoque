using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class BuscarTodosItensEstoqueUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public BuscarTodosItensEstoqueUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve buscar todos os itens de estoque com sucesso")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveBuscarTodosItensEstoqueComSucesso()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itensEstoque = new List<ItemEstoqueAggregate>
            {
                new ItemEstoqueBuilder().ComNome("Filtro de Óleo").Build(),
                new ItemEstoqueBuilder().ComNome("Filtro de Ar").Build(),
                new ItemEstoqueBuilder().ComNome("Óleo Motor").Build()
            };
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterTodos().Retorna(itensEstoque);

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarTodosItensEstoquePresenterMock.DeveTerApresentadoSucesso<IBuscarTodosItensEstoquePresenter, IEnumerable<ItemEstoqueAggregate>>(itensEstoque);
        }

        [Fact(DisplayName = "Deve apresentar lista vazia quando não houver itens de estoque")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveApresentarListaVazia_QuandoNaoHouverItensEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterTodos().Retorna(new List<ItemEstoqueAggregate>());

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarTodosItensEstoquePresenterMock.DeveTerApresentadoSucesso(new List<ItemEstoqueAggregate>());
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterTodos().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarTodosItensEstoquePresenterMock.DeveTerApresentadoErro<IBuscarTodosItensEstoquePresenter, IEnumerable<ItemEstoqueAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar todos os itens de estoque")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarTodosItensEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.BuscarTodosItensEstoquePresenterMock.DeveTerApresentadoErro<IBuscarTodosItensEstoquePresenter, IEnumerable<ItemEstoqueAggregate>>("Acesso negado. Apenas administradores podem listar estoque.", ErrorType.NotAllowed);
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarTodosItensEstoque")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterTodos().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarTodosItensEstoqueUseCase.ExecutarAsync(
                ator,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.BuscarTodosItensEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}