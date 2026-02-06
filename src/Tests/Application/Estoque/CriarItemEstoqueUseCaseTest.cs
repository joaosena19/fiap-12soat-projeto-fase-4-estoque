using Application.Contracts.Presenters;
using Domain.Estoque.Enums;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class CriarItemEstoqueUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public CriarItemEstoqueUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve criar item de estoque com sucesso quando não existir item com mesmo nome")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveCriarItemEstoqueComSucesso_QuandoNaoExistirItemComMesmoNome()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemEsperado = new ItemEstoqueBuilder().Build();
            var logger = MockLogger.CriarSimples();

            ItemEstoqueAggregate? itemCriado = null;

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemEsperado.Nome.Valor).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoSalvar().ComCallback(item => itemCriado = item);

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemEsperado.Nome.Valor,
                itemEsperado.Quantidade.Valor,
                itemEsperado.TipoItemEstoque.Valor,
                itemEsperado.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            itemCriado.Should().NotBeNull();
            itemCriado!.Nome.Valor.Should().Be(itemEsperado.Nome.Valor);
            itemCriado.Quantidade.Valor.Should().Be(itemEsperado.Quantidade.Valor);
            itemCriado.TipoItemEstoque.Valor.Should().Be(itemEsperado.TipoItemEstoque.Valor);
            itemCriado.Preco.Valor.Should().Be(itemEsperado.Preco.Valor);

            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>(itemCriado);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de conflito quando já existir item com mesmo nome")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroConflito_QuandoJaExistirItemComMesmoNome()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder()
                .ComNome("Filtro de Óleo")
                .Build();

            var itemParaTentar = new ItemEstoqueBuilder()
                .ComNome("Filtro de Óleo")
                .Build();

            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemExistente.Nome.Valor).Retorna(itemExistente);

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemParaTentar.Nome.Valor,
                itemParaTentar.Quantidade.Valor,
                itemParaTentar.TipoItemEstoque.Valor,
                itemParaTentar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Já existe um item de estoque cadastrado com este nome.", ErrorType.Conflict);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var nomeInvalido = "";
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;
            var precoValido = 100m;
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(nomeInvalido).NaoRetornaNada();

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                nomeInvalido,
                quantidadeValida,
                tipoValido,
                precoValido,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemParaCriar = new ItemEstoqueBuilder()
                .ComNome("Filtro de Ar")
                .ComQuantidade(20)
                .ComTipoItemEstoque(TipoItemEstoqueEnum.Peca)
                .ComPreco(35.50m)
                .Build();

            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemParaCriar.Nome.Valor,
                itemParaCriar.Quantidade.Valor,
                itemParaCriar.TipoItemEstoque.Valor,
                itemParaCriar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta criar item de estoque")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaCriarItemEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemParaCriar = new ItemEstoqueBuilder().Build();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemParaCriar.Nome.Valor,
                itemParaCriar.Quantidade.Valor,
                itemParaCriar.TipoItemEstoque.Valor,
                itemParaCriar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.CriarItemEstoquePresenterMock.DeveTerApresentadoErro<ICriarItemEstoquePresenter, ItemEstoqueAggregate>("Acesso negado. Apenas administradores podem criar estoque.", ErrorType.NotAllowed);
            _fixture.CriarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<ICriarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemParaCriar = new ItemEstoqueBuilder().Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemParaCriar.Nome.Valor,
                itemParaCriar.Quantidade.Valor,
                itemParaCriar.TipoItemEstoque.Valor,
                itemParaCriar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "CriarItemEstoque")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemParaCriar = new ItemEstoqueBuilder().ComNome("Filtro de Ar").Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorNome(itemParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemParaCriar.Nome.Valor,
                itemParaCriar.Quantidade.Valor,
                itemParaCriar.TipoItemEstoque.Valor,
                itemParaCriar.Preco.Valor,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.CriarItemEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}