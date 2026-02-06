using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class ObterTempoMedioUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public ObterTempoMedioUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando calcular tempo médio com ordens entregues")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoCalcularTempoMedioComOrdensEntregues()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 30;
            var dataBase = DateTime.UtcNow.AddDays(-15);

            var ordensEntregues = new List<OrdemServicoAggregate>
            {
                CriarOrdemEntregueComHistorico(dataBase.AddDays(-5), dataBase.AddDays(-4), dataBase.AddDays(-3), dataBase.AddDays(-2), dataBase.AddDays(-1)),
                CriarOrdemEntregueComHistorico(dataBase.AddDays(-10), dataBase.AddDays(-9), dataBase.AddDays(-8), dataBase.AddDays(-7), dataBase.AddDays(-6))
            };

            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).Retorna(ordensEntregues);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                quantidadeDias,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                ordensEntregues.Count,
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando quantidade de dias for menor que 1")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoQuantidadeDiasForMenorQue1()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 0;

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                "A quantidade de dias deve estar entre 1 e 365.",
                ErrorType.InvalidInput
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando quantidade de dias for maior que 365")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoQuantidadeDiasForMaiorQue365()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 366;

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                "A quantidade de dias deve estar entre 1 e 365.",
                ErrorType.InvalidInput
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando nenhuma ordem entregue for encontrada")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNenhumaOrdemEntregueForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 30;
            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).RetornaListaVazia();

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                "Nenhuma ordem de serviço entregue encontrada no período especificado.",
                ErrorType.DomainRuleBroken
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando gateway lançar DomainException")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoGatewayLancarDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 30;
            var domainException = new DomainException("Erro de acesso aos dados.", ErrorType.DomainRuleBroken);

            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).LancaExcecao(domainException);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                domainException.Message,
                domainException.ErrorType
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 30;

            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                "Erro interno do servidor.",
                ErrorType.UnexpectedError
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tentar obter tempo médio")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentarObterTempoMedio()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var quantidadeDias = 30;

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarErro(
                "Acesso negado. Apenas administradores podem obter tempo médio de execução.",
                ErrorType.NotAllowed
            ), Times.Once);

            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                It.IsAny<double>()
            ), Times.Never);
        }

        [Theory(DisplayName = "Deve calcular tempo médio corretamente com diferentes cenários")]
        [Trait("UseCase", "ObterTempoMedio")]
        [InlineData(1, 1)] // 1 dia, 1 ordem
        [InlineData(7, 3)] // 1 semana, 3 ordens
        [InlineData(30, 5)] // 1 mês, 5 ordens
        public async Task ExecutarAsync_DeveCalcularTempoMedioCorretamente_ComDiferentesCenarios(int quantidadeDias, int quantidadeOrdens)
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dataBase = DateTime.UtcNow.AddDays(-quantidadeDias / 2);
            var ordensEntregues = new List<OrdemServicoAggregate>();

            for (int i = 0; i < quantidadeOrdens; i++)
            {
                var ordemData = dataBase.AddDays(-i);
                ordensEntregues.Add(CriarOrdemEntregueComHistorico(
                    ordemData.AddHours(-24), // Criação: 24h antes da entrega
                    ordemData.AddHours(-20), // Início diagnóstico: 20h antes
                    ordemData.AddHours(-16), // Início execução: 16h antes
                    ordemData.AddHours(-4),  // Finalização: 4h antes
                    ordemData                // Entrega: agora
                ));
            }

            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).Retorna(ordensEntregues);

            // Act
            var logger = MockLogger.CriarSimples();
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                logger);

            // Assert
            _fixture.ObterTempoMedioPresenterMock.Verify(p => p.ApresentarSucesso(
                quantidadeDias,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                quantidadeOrdens,
                24.0, // Tempo completo: 24 horas
                12.0  // Tempo execução: 12 horas (16h - 4h)
            ), Times.Once);
        }

        private OrdemServicoAggregate CriarOrdemEntregueComHistorico(DateTime dataCriacao, DateTime dataInicioDiagnostico, DateTime dataInicioExecucao, DateTime dataFinalizacao, DateTime dataEntrega)
        {
            var ordem = new OrdemServicoBuilder().Entregue().Build();

            // Usar reflection para definir as datas do histórico, pois em tempo de execução os valores seriam muito baixos
            var historicoProperty = ordem.GetType().GetProperty("Historico");
            var historico = historicoProperty?.GetValue(ordem);

            if (historico != null)
            {
                var historicoType = historico.GetType();

                // Definir DataCriacao
                var dataCriacaoField = historicoType.GetField("_dataCriacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                dataCriacaoField?.SetValue(historico, dataCriacao);

                // Definir DataInicioDiagnostico
                var dataInicioDiagnosticoField = historicoType.GetField("_dataInicioDiagnostico", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                dataInicioDiagnosticoField?.SetValue(historico, dataInicioDiagnostico);

                // Definir DataInicioExecucao
                var dataInicioExecucaoField = historicoType.GetField("_dataInicioExecucao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                dataInicioExecucaoField?.SetValue(historico, dataInicioExecucao);

                // Definir DataFinalizacao
                var dataFinalizacaoField = historicoType.GetField("_dataFinalizacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                dataFinalizacaoField?.SetValue(historico, dataFinalizacao);

                // Definir DataEntrega
                var dataEntregaField = historicoType.GetField("_dataEntrega", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                dataEntregaField?.SetValue(historico, dataEntrega);
            }

            return ordem;
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var quantidadeDias = 30;
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "ObterTempoMedio")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var quantidadeDias = 30;
            var mockLogger = MockLogger.Criar();

            _fixture.OrdemServicoGatewayMock.AoObterEntreguesUltimosDias(quantidadeDias).LancaExcecao(new InvalidOperationException("Erro inesperado"));

            // Act
            await _fixture.ObterTempoMedioUseCase.ExecutarAsync(
                ator,
                quantidadeDias,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ObterTempoMedioPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
