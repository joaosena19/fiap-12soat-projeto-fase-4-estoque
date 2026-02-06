using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Bogus;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculoPorPlacaUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculoPorPlacaUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve buscar veículo com sucesso quando veículo existir e usuário tem permissão")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveBuscarVeiculoComSucesso_QuandoVeiculoExistirEUsuarioTemPermissao()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var veiculoExistente = new VeiculoBuilder().Build();
            var placa = veiculoExistente.Placa.Valor;

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).Retorna(veiculoExistente);

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>(veiculoExistente);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var placa = new Faker("pt_BR").Random.Replace("???-####");

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var placa = new Faker("pt_BR").Random.Replace("???-####");

            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve retornar veículo quando cliente é o proprietário")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveRetornarVeiculo_QuandoClienteEhProprietario()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();
            var placa = veiculo.Placa.Valor;
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>(veiculo);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta acessar veículo de outro cliente")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAcessarVeiculoDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(outroClienteId).Build();
            var placa = veiculo.Placa.Valor;
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).Retorna(veiculo);

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculoPorPlacaPresenterMock.DeveTerApresentadoErro<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>("Acesso negado. Somente administradores ou o proprietário do veículo podem visualizá-lo.", ErrorType.NotAllowed);
            _fixture.BuscarVeiculoPorPlacaPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculoPorPlacaPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando DomainException for lançada")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoDomainExceptionForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var placa = new Faker("pt_BR").Random.Replace("???-####");
            var erroDominio = new Shared.Exceptions.DomainException("Erro de domínio", ErrorType.DomainRuleBroken);
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).LancaExcecao(erroDominio);

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando Exception genérica for lançada")]
        [Trait("UseCase", "BuscarVeiculoPorPlaca")]
        public async Task ExecutarAsync_DeveLogarError_QuandoExceptionGenericaForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var placa = new Faker("pt_BR").Random.Replace("???-####");
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placa).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculoPorPlacaUseCase.ExecutarAsync(ator, placa, _fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculoPorPlacaPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}