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
    public class BuscarVeiculosPorClienteUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculosPorClienteUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar veículos do cliente quando usuário tem permissão")]
        public async Task ExecutarAsync_DeveRetornarVeiculosDoClienteQuandoUsuarioTemPermissao()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var veiculos = new List<VeiculoAggregate>
            {
                new VeiculoBuilder().ComClienteId(cliente.Id).Build(),
                new VeiculoBuilder().ComClienteId(cliente.Id).Build()
            };

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).Retorna(veiculos);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>(veiculos);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteId = Guid.NewGuid();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, clienteId, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>("Cliente não encontrado.", ErrorType.ReferenceNotFound);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando cliente não tem veículos")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoClienteNaoTemVeiculos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var listaVazia = new List<VeiculoAggregate>();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).Retorna(listaVazia);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>(listaVazia);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar veículos quando cliente consulta seus próprios veículos")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveRetornarVeiculos_QuandoClienteConsultaSeusPropriosVeiculos()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var clienteId = cliente.Id;
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var veiculos = new List<VeiculoAggregate>
            {
                new VeiculoBuilder().ComClienteId(clienteId).Build()
            };

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(clienteId).Retorna(veiculos);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, clienteId, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>(veiculos);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta consultar veículos de outro cliente")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaConsultarVeiculosDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroCliente = new ClienteBuilder().Build();
            var outroClienteId = outroCliente.Id;
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();

            _fixture.ClienteGatewayMock.AoObterPorId(outroClienteId).Retorna(outroCliente);

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, outroClienteId, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.BuscarVeiculosPorClientePresenterMock.DeveTerApresentadoErro<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>("Acesso negado. Somente administradores ou o próprio cliente podem visualizar seus veículos.", ErrorType.NotAllowed);
            _fixture.BuscarVeiculosPorClientePresenterMock.NaoDeveTerApresentadoSucesso<IBuscarVeiculosPorClientePresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve logar information quando DomainException for lançada")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoDomainExceptionForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, clienteId, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando Exception for lançada")]
        [Trait("UseCase", "BuscarVeiculosPorCliente")]
        public async Task ExecutarAsync_DeveLogarError_QuandoExceptionForLancada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);
            _fixture.VeiculoGatewayMock.AoObterPorClienteId(cliente.Id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarVeiculosPorClienteUseCase.ExecutarAsync(ator, cliente.Id, _fixture.VeiculoGatewayMock.Object, _fixture.ClienteGatewayMock.Object, _fixture.BuscarVeiculosPorClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}