using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdemServicoPorCodigoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdemServicoPorCodigoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir e ator for administrador")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistirEAtorForAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(ordemServico.Codigo.Valor).Retorna(ordemServico);

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                ordemServico.Codigo.Valor,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando ordem de serviço existir e ator for dono do veículo")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoOrdemServicoExistirEAtorForDonoDoVeiculo()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(ordemServico.Codigo.Valor).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                ordemServico.Codigo.Valor,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>(ordemServico);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var codigo = "OS-INEXISTENTE";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).NaoRetornaNada();

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tentar buscar ordem de serviço de outro cliente")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentarBuscarOrdemServicoDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(outroClienteId).Build(); // Veículo de outro cliente

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(ordemServico.Codigo.Valor).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                ordemServico.Codigo.Valor,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Acesso negado. Apenas administradores ou donos da ordem de serviço podem visualizá-la.", ErrorType.NotAllowed);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var codigo = "OS-12345";

            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var codigo = "OS-123456";
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarOrdemServicoPorCodigo")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var codigo = "OS-123456";
            var mockLogger = MockLogger.Criar();
            
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(codigo).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdemServicoPorCodigoUseCase.ExecutarAsync(
                ator,
                codigo,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.BuscarOrdemServicoPorCodigoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
            _fixture.BuscarOrdemServicoPorCodigoPresenterMock.DeveTerApresentadoErro<IBuscarOrdemServicoPorCodigoPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}
