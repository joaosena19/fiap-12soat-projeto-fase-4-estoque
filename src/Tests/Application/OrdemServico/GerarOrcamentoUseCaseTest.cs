using Application.Identidade.Services;
using Domain.OrdemServico.Enums;
using Moq;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class GerarOrcamentoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public GerarOrcamentoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando administrador gerar orçamento")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoAdministradorGerarOrcamento()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ComItens().ComServicos().ComStatus(StatusOrdemServicoEnum.EmDiagnostico).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Once);
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound), Times.Once);
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ComStatus(StatusOrdemServicoEnum.EmDiagnostico).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), ErrorType.DomainRuleBroken), Times.Once);
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ComItens().ComServicos().ComStatus(StatusOrdemServicoEnum.EmDiagnostico).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError), Times.Once);
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tentar gerar orçamento")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentarGerarOrcamento()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarErro("Acesso negado. Apenas administradores podem gerar orçamentos.", ErrorType.NotAllowed), Times.Once);
            _fixture.GerarOrcamentoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve logar Information quando ator não for administrador")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoAtorNaoForAdministrador()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "GerarOrcamento")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ComItens().ComServicos().ComStatus(StatusOrdemServicoEnum.EmDiagnostico).Build();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.GerarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.GerarOrcamentoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
