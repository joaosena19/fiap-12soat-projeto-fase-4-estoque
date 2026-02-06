using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculoPorIdUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculoPorIdUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar veículo quando encontrado e usuário tem permissão")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveRetornarVeiculo_QuandoEncontradoEUsuarioTemPermissao()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculo = new VeiculoBuilder().Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, veiculo.Id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>(veiculo);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não encontrado")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            _fixture.VeiculoGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            _fixture.VeiculoGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve retornar veículo quando cliente é o proprietário")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveRetornarVeiculo_QuandoClienteEhProprietario()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, veiculo.Id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>(veiculo);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta acessar veículo de outro cliente")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAcessarVeiculoDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(outroClienteId).Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, veiculo.Id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorIdPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>("Acesso negado. Somente administradores ou o proprietário do veículo podem visualizá-lo.", ErrorType.NotAllowed);
            _fixture.BuscarVeiculoPorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorIdPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando DomainException for lançada")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoDomainExceptionForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var erroDominio = new Shared.Exceptions.DomainException("Erro de domínio", ErrorType.DomainRuleBroken);
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorId(id).LancaExcecao(erroDominio);

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando Exception genérica for lançada")]
        [Trait("UseCase", "BuscarVeiculoPorId")]
        public async Task ExecutarAsync_DeveLogarError_QuandoExceptionGenericaForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculoPorIdUseCase.ExecutarAsync(ator, id, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorIdPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}