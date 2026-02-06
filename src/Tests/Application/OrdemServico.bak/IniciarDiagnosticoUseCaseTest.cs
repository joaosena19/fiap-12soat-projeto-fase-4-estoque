using Application.Identidade.Services;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;

namespace Tests.Application.OrdemServico
{
    public class IniciarDiagnosticoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public IniciarDiagnosticoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando administrador iniciar diagnóstico")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoAdministradorIniciarDiagnostico()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();
            var domainException = new DomainException("Não é possível iniciar diagnóstico neste status.", ErrorType.DomainRuleBroken);

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(domainException);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro(domainException.Message, domainException.ErrorType);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tentar iniciar diagnóstico")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentarIniciarDiagnostico()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                logger,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Acesso negado. Apenas administradores podem iniciar diagnósticos.", ErrorType.NotAllowed);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = Ator.Cliente(Guid.NewGuid(), Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();
            var mockLogger = MockLogger.Criar();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro inesperado"));

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }

        [Fact(DisplayName = "Deve registrar métrica de mudança de status quando iniciar diagnóstico com sucesso")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveRegistrarMetricaMudancaStatus_QuandoIniciarDiagnosticoComSucesso()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().Build(); // Status Recebida por padrão

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.MetricsServiceMock.DeveTerRegistradoMudancaOrdemServicoStatus();
        }

        [Fact(DisplayName = "Deve logar erro quando falha ao registrar métrica")]
        [Trait("UseCase", "IniciarDiagnostico")]
        public async Task ExecutarAsync_DeveLogarErro_QuandoFalhaAoRegistrarMetrica()
        {
            // Arrange
            var ator = Ator.Administrador(Guid.NewGuid());
            var ordemServico = new OrdemServicoBuilder().Build(); // Status Recebida por padrão
            var mockLogger = MockLogger.Criar();
            var excecaoMetrica = new InvalidOperationException("Erro ao enviar métrica");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);
            _fixture.MetricsServiceMock.AoRegistrarMudancaOrdemServicoStatus().LancaExcecao(excecaoMetrica);

            // Act
            await _fixture.IniciarDiagnosticoUseCase.ExecutarAsync(
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