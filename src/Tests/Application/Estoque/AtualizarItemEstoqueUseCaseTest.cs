using Application.Contracts.Presenters;
using Domain.Estoque.Enums;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque
{
    public class AtualizarItemEstoqueUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public AtualizarItemEstoqueUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar item de estoque com sucesso quando item existir")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveAtualizarItemEstoqueComSucesso_QuandoItemExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();

            var novoNome = "Filtro Atualizado";
            var novaQuantidade = 25;
            var novoTipo = TipoItemEstoqueEnum.Insumo;
            var novoPreco = 35.50m;
            var logger = MockLogger.CriarSimples();

            ItemEstoqueAggregate? itemAtualizado = null;

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);
            _fixture.ItemEstoqueGatewayMock.AoAtualizar().ComCallback(item => itemAtualizado = item);

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                novoNome,
                novaQuantidade,
                novoTipo,
                novoPreco,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            itemAtualizado.Should().NotBeNull();
            itemAtualizado.Nome.Valor.Should().Be(novoNome);
            itemAtualizado.Quantidade.Valor.Should().Be(novaQuantidade);
            itemAtualizado.TipoItemEstoque.Valor.Should().Be(novoTipo);
            itemAtualizado.Preco.Valor.Should().Be(novoPreco);

            _fixture.AtualizarItemEstoquePresenterMock.DeveTerApresentadoSucesso<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>(itemAtualizado);
            _fixture.AtualizarItemEstoquePresenterMock.NaoDeveTerApresentadoErro<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não existir")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemId,
                "Nome",
                10,
                TipoItemEstoqueEnum.Peca,
                100m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.AtualizarItemEstoquePresenterMock.DeveTerApresentadoErro<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>($"Item de estoque com ID {itemId} não foi encontrado", ErrorType.ResourceNotFound);
            _fixture.AtualizarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var nomeInvalido = ""; // Nome inválido para provocar DomainException
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                nomeInvalido,
                25,
                TipoItemEstoqueEnum.Insumo,
                35.50m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.AtualizarItemEstoquePresenterMock.DeveTerApresentadoErro<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.AtualizarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);
            _fixture.ItemEstoqueGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                "Nome",
                10,
                TipoItemEstoqueEnum.Peca,
                100m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.AtualizarItemEstoquePresenterMock.DeveTerApresentadoErro<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de acesso negado quando cliente tentar atualizar item do estoque")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveApresentarErroAcessoNegado_QuandoClienteTentarAtualizarItemEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemId = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemId,
                "Nome",
                25,
                TipoItemEstoqueEnum.Insumo,
                35.50m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                logger);

            // Assert
            _fixture.AtualizarItemEstoquePresenterMock.DeveTerApresentadoErro<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>("Acesso negado. Apenas administradores podem atualizar estoque.", ErrorType.NotAllowed);
            _fixture.AtualizarItemEstoquePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarItemEstoquePresenter, ItemEstoqueAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemId,
                "Nome",
                10,
                TipoItemEstoqueEnum.Peca,
                100m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "AtualizarItemEstoque")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);
            _fixture.ItemEstoqueGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarItemEstoqueUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                "Nome",
                25,
                TipoItemEstoqueEnum.Insumo,
                35.50m,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.AtualizarItemEstoquePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}