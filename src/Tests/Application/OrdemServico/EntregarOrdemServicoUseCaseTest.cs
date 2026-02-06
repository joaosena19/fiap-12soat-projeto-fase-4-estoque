using Application.Identidade.Services;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;

namespace Tests.Application.OrdemServico
{
    public class EntregarOrdemServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public EntregarOrdemServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando administrador entregar ordem de serviço")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoAdministradorEntregarOrdemServico()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ProntoParaEntrega().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().Build(); // Ordem em status incorreto

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErroComTipo(ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ProntoParaEntrega().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tentar entregar ordem de serviço")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentarEntregarOrdemServico()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Acesso negado. Apenas administradores podem entregar ordens de serviço.", ErrorType.NotAllowed);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve logar Information quando ator não for administrador")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoAtorNaoForAdministrador()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ProntoParaEntrega().Build();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }

        [Fact(DisplayName = "Deve registrar métrica de mudança de status quando entregar ordem de serviço com sucesso")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveRegistrarMetricaMudancaStatus_QuandoEntregarOrdemServicoComSucesso()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ProntoParaEntrega().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.MetricsServiceMock.DeveTerRegistradoMudancaOrdemServicoStatus();
        }

        [Fact(DisplayName = "Deve logar erro quando falha ao registrar métrica")]
        [Trait("UseCase", "EntregarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarErro_QuandoFalhaAoRegistrarMetrica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().ProntoParaEntrega().Build();
            var mockLogger = MockLogger.Criar();
            var excecaoMetrica = new InvalidOperationException("Erro ao enviar métrica");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);
            _fixture.MetricsServiceMock.AoRegistrarMudancaOrdemServicoStatus().LancaExcecao(excecaoMetrica);

            // Act
            await _fixture.EntregarOrdemServicoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoError();
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso(); // Operação deve continuar mesmo com erro de métrica
        }
    }
}
