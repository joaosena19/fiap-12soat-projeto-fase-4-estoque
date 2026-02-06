using Shared.Enums;
using Tests.Application.Estoque.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.Estoque
{
    public class VerificarDisponibilidadeUseCaseTest
    {
        private readonly ItemEstoqueTestFixture _fixture;

        public VerificarDisponibilidadeUseCaseTest()
        {
            _fixture = new ItemEstoqueTestFixture();
        }

        [Fact(DisplayName = "Deve verificar disponibilidade com sucesso quando item existir e quantidade for suficiente")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveVerificarDisponibilidadeComSucesso_QuandoItemExistirEQuantidadeForSuficiente()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder()
                .ComQuantidade(50)
                .Build();
            var quantidadeRequisitada = 20;
            var disponibilidadeEsperada = true;
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoSucessoVerificacaoDisponibilidade(itemExistente, quantidadeRequisitada, disponibilidadeEsperada);
        }

        [Fact(DisplayName = "Deve verificar disponibilidade com sucesso quando item existir e quantidade for insuficiente")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveVerificarDisponibilidadeComSucesso_QuandoItemExistirEQuantidadeForInsuficiente()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder()
                .ComQuantidade(10)
                .Build();
            var quantidadeRequisitada = 20;
            var disponibilidadeEsperada = false;
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoSucessoVerificacaoDisponibilidade(itemExistente, quantidadeRequisitada, disponibilidadeEsperada);
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não existir")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 10;
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoErroVerificacaoDisponibilidade($"Item de estoque com ID {itemId} não foi encontrado", ErrorType.ResourceNotFound);
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemExistente = new ItemEstoqueBuilder().Build();
            var quantidadeInvalida = -5; // Quantidade inválida para provocar DomainException
            var logger = MockLogger.CriarSimples();

            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemExistente.Id).Retorna(itemExistente);

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemExistente.Id,
                quantidadeInvalida,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoErroVerificacaoDisponibilidade("Quantidade requisitada deve ser maior que 0", ErrorType.InvalidInput);
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 10;
            var logger = MockLogger.CriarSimples();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoErroVerificacaoDisponibilidade("Erro interno do servidor.", ErrorType.UnexpectedError);
        }

        [Fact(DisplayName = "Deve apresentar erro de acesso negado quando cliente tentar verificar disponibilidade do estoque")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveApresentarErroAcessoNegado_QuandoClienteTentarVerificarDisponibilidadeEstoque()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 10;
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                logger);

            // Assert
            _fixture.VerificarDisponibilidadePresenterMock.DeveTerApresentadoErroVerificacaoDisponibilidade("Acesso negado. Apenas administradores podem verificar estoque.", ErrorType.NotAllowed);
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 10;
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "VerificarDisponibilidade")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 10;
            var mockLogger = MockLogger.Criar();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.VerificarDisponibilidadeUseCase.ExecutarAsync(
                ator,
                itemId,
                quantidadeRequisitada,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.VerificarDisponibilidadePresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}