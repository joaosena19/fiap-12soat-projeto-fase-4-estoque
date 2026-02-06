using Application.Contracts.Presenters;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscaPublicaOrdemServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscaPublicaOrdemServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando código e documento do cliente conferirem")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoCodigoEDocumentoConferirem()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var cliente = new ClienteExternalDtoBuilder()
                .ComDocumentoIdentificador("12345678901")
                .Build();
            var codigo = "OS-12345";
            var documentoCliente = "12345678901";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).Retorna(ordemServico);
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(ordemServico.VeiculoId).Retorna(cliente);

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoSucesso<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscaPublicaOrdemServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar não encontrado quando ordem de serviço não existir")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarNaoEncontrado_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var codigo = "OS-INEXISTENTE";
            var documentoCliente = "12345678901";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).NaoRetornaNada();

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoNaoEncontrado();
            _fixture.BuscaPublicaOrdemServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar não encontrado quando cliente não existir")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarNaoEncontrado_QuandoClienteNaoExistir()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var codigo = "OS-12345";
            var documentoCliente = "12345678901";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).Retorna(ordemServico);
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(ordemServico.VeiculoId).NaoRetornaNada();

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoNaoEncontrado();
            _fixture.BuscaPublicaOrdemServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar não encontrado quando documento do cliente não conferir")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarNaoEncontrado_QuandoDocumentoClienteNaoConferir()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            var cliente = new ClienteExternalDtoBuilder()
                .ComDocumentoIdentificador("98765432100")
                .Build();
            var codigo = "OS-12345";
            var documentoCliente = "12345678901"; // Documento diferente

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).Retorna(ordemServico);
            _fixture.ClienteExternalServiceMock.AoObterClientePorVeiculoId(ordemServico.VeiculoId).Retorna(cliente);

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoNaoEncontrado();
            _fixture.BuscaPublicaOrdemServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar não encontrado quando ocorrer exceção")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveApresentarNaoEncontrado_QuandoOcorrerExcecao()
        {
            // Arrange
            var codigo = "OS-12345";
            var documentoCliente = "12345678901";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).LancaExcecao(new InvalidOperationException("Erro inesperado"));

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoNaoEncontrado();
            _fixture.BuscaPublicaOrdemServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscaPublicaOrdemServicoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscaPublicaOrdemServico")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var codigo = "OS-123456";
            var documentoCliente = "12345678901";
            var mockLogger = MockLogger.Criar();
            
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscaPublicaOrdemServicoUseCase.ExecutarAsync(
                codigo,
                documentoCliente,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteExternalServiceMock.Object,
                _fixture.BuscaPublicaOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
            _fixture.BuscaPublicaOrdemServicoPresenterMock.DeveTerApresentadoNaoEncontrado();
        }
    }
}
