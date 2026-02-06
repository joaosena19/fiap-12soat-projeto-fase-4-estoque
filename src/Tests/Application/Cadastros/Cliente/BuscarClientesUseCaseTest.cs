using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientesUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientesUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve retornar lista de clientes")]
        public async Task ExecutarAsync_DeveRetornarListaDeClientes()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clientes = new List<ClienteAggregate> { new ClienteBuilder().Build(), new ClienteBuilder().Build() };
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterTodos().Retorna(clientes);

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>(clientes);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver clientes")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverClientes()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var listaVazia = new List<ClienteAggregate>();
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>(listaVazia);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterTodos().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>("Erro interno do servidor.", Shared.Enums.ErrorType.UnexpectedError);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente (não administrador) tenta listar clientes")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaListarClientes()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientesPresenterMock.DeveTerApresentadoErro<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>("Acesso negado. Somente administradores podem listar clientes.", Shared.Enums.ErrorType.NotAllowed);
            _fixture.BuscarClientesPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientesPresenter, IEnumerable<ClienteAggregate>>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarClientes")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarClientes")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ClienteGatewayMock.AoObterTodos().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarClientesUseCase.ExecutarAsync(ator, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientesPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
