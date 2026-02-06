using Domain.OrdemServico.Enums;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Application.OrdemServico.Dtos;
using Application.OrdemServico.Dtos.External;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class CriarOrdemServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public CriarOrdemServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar NotAllowed quando cliente tenta criar ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveRetornarNotAllowed_QuandoClienteTentaCriarOrdemServico()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var veiculoId = Guid.NewGuid();

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Acesso negado. Apenas administradores podem criar ordens de serviço.", ErrorType.NotAllowed), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
            _fixture.VeiculoExternalServiceMock.Verify(v => v.VerificarExistenciaVeiculo(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar ordem de serviço com status inicial Recebida")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoComStatusRecebida()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Status.Valor.Should().Be(StatusOrdemServicoEnum.Recebida);
            ordemServicoSalva.Historico.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(false);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve gerar novo código quando código já existir")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveGerarNovoCodigo_QuandoCodigoJaExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            var ordemServicoExistente = new OrdemServicoBuilder().ComVeiculoId(Guid.NewGuid()).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);

            // Simula que na primeira tentativa o código já existe, na segunda não
            _fixture.OrdemServicoGatewayMock.SetupSequence(g => g.ObterPorCodigoAsync(It.IsAny<string>()))
                .ReturnsAsync(ordemServicoExistente)
                .ReturnsAsync((OrdemServicoAggregate?)null);
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Codigo.Valor.Should().NotBe(ordemServicoExistente.Codigo.Valor);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).LancaExcecao(new DomainException("Erro de domínio personalizado", ErrorType.DomainRuleBroken));

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Erro de domínio personalizado", ErrorType.DomainRuleBroken), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError), Times.Once);
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        // Teste removido: responsabilidade já coberta em outros testes

        [Fact(DisplayName = "Deve verificar se código já existe e salvar ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveVerificarCodigoExistenteESalvar()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { });

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.OrdemServicoGatewayMock.DeveTerVerificadoCodigoExistente();
            _fixture.OrdemServicoGatewayMock.DeveTerSalvoOrdemServico();
        }

        [Fact(DisplayName = "Deve gerar código no formato correto")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveGerarCodigoFormatoCorreto()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Codigo.Valor.Should().StartWith("OS-");
            ordemServicoSalva.Codigo.Valor.Should().HaveLength(18);
        }

        [Fact(DisplayName = "Deve logar Information quando ator não for administrador")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoAtorNaoForAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var veiculoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }

        [Fact(DisplayName = "Deve registrar métrica de ordem de serviço criada")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveRegistrarMetricaOrdemServicoCriada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var cliente = new ClienteExternalDtoBuilder().ComId(clienteId).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(veiculoId).Retorna(cliente);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            _fixture.MetricsServiceMock.DeveTerRegistradoOrdemServicoCriada(ordemServicoSalva!.Id, clienteId, ator.UsuarioId);
        }

        [Fact(DisplayName = "Deve logar erro quando registro de métrica falhar")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarErro_QuandoRegistroMetricaFalhar()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var cliente = new ClienteExternalDtoBuilder().ComId(clienteId).Build();
            var mockLogger = MockLogger.Criar();
            var excecaoMetrica = new Exception("Erro ao registrar métrica");

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { });
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(veiculoId).Retorna(cliente);
            _fixture.MetricsServiceMock.AoRegistrarOrdemServicoCriada().LancaExcecao(excecaoMetrica);

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
            _fixture.CriarOrdemServicoPresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Deve logar warning quando cliente for null ao registrar métrica")]
        [Trait("UseCase", "CriarOrdemServico")]
        public async Task ExecutarAsync_DeveLogarWarning_QuandoClienteForNullAoRegistrarMetrica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            _fixture.VeiculoExternalServiceMock.AoVerificarExistenciaVeiculo(veiculoId).Retorna(true);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { });
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(veiculoId).NaoRetornaNada();

            // Act
            await _fixture.CriarOrdemServicoUseCase.ExecutarAsync(
                ator,
                veiculoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoExternalServiceMock.Object,
                _fixture.CriarOrdemServicoPresenterMock.Object,
                mockLogger.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoWarning();
            _fixture.MetricsServiceMock.NaoDeveTerRegistradoOrdemServicoCriada();
        }
    }
}
