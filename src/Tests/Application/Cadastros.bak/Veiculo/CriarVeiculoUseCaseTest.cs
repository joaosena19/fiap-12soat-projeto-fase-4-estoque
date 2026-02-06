using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Cadastros.Enums;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;
using Shared.Exceptions;

namespace Tests.Application.Cadastros.Veiculo
{
    public class CriarVeiculoUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;
        private readonly MockLogger _mockLogger;

        public CriarVeiculoUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
            _mockLogger = MockLogger.Criar();
        }

        [Fact(DisplayName = "Deve criar veículo com sucesso")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveCriarVeiculoComSucesso()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculo);

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, "dd", veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>(veiculo);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteId = Guid.NewGuid();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteId).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, clienteId, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            //Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Cliente não encontrado para realizar associação com o veículo.", ErrorType.ReferenceNotFound);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe veículo com placa")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoJaExiste()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).Retorna(veiculo);

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Já existe um veículo cadastrado com esta placa.", ErrorType.Conflict);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var placaInvalida = "";

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(placaInvalida).NaoRetornaNada();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, placaInvalida, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Placa não pode ser vazia", ErrorType.InvalidInput);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve criar veículo com sucesso quando cliente é dono")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveCriarVeiculoComSucesso_QuandoClienteEhDono()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoCliente(cliente.Id).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculo);

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>(veiculo);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro NotAllowed quando cliente tenta criar veículo para outro cliente")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroNotAllowed_QuandoClienteTentaCriarVeiculoParaOutroCliente()
        {
            // Arrange
            var clienteDono = new ClienteBuilder().Build();
            var clienteOutro = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoCliente(clienteDono.Id).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteOutro.Id).Build();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, clienteOutro.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarVeiculoPresenterMock.DeveTerApresentadoErro<ICriarVeiculoPresenter, VeiculoAggregate>("Acesso negado.", ErrorType.NotAllowed);
            _fixture.CriarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var clienteDono = new ClienteBuilder().Build();
            var clienteOutro = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoCliente(clienteDono.Id).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(clienteOutro.Id).Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, clienteOutro.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
            mockLogger.NaoDeveTerLogadoNenhumError();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "CriarVeiculo")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var veiculo = new VeiculoBuilder().ComClienteId(cliente.Id).Build();
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(veiculo.Placa.Valor).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarVeiculoUseCase.ExecutarAsync(
                ator, cliente.Id, veiculo.Placa.Valor, veiculo.Modelo.Valor, veiculo.Marca.Valor, veiculo.Cor.Valor, veiculo.Ano.Valor, veiculo.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.CriarVeiculoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}