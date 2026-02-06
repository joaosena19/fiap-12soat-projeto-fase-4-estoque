using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;

namespace Tests.Application.OrdemServico
{
    public class RemoverItemUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public RemoverItemUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando remover item")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoRemoverItem()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComItens().Build();
            var itemIncluidoId = ordemServico.ItensIncluidos.First().Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var itemIncluidoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComItens().Build();
            var itemIncluidoIdInexistente = Guid.NewGuid(); // ID que não existe na ordem

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemIncluidoIdInexistente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErroComTipo(ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComItens().Build();
            var itemIncluidoId = ordemServico.ItensIncluidos.First().Id;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tentar remover item")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentarRemoverItem()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var itemIncluidoId = Guid.NewGuid();

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Acesso negado. Apenas administradores podem remover itens.", ErrorType.NotAllowed);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var itemIncluidoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "RemoverItem")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComItens().Build();
            var itemIncluidoId = ordemServico.ItensIncluidos.First().Id;
            var mockLogger = MockLogger.Criar();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro inesperado"));

            // Act
            await _fixture.RemoverItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemIncluidoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
