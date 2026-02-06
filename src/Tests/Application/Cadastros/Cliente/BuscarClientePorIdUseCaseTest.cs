using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientePorIdUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientePorIdUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve retornar cliente quando encontrado")]
        public async Task ExecutarAsync_DeveRetornarCliente_QuandoEncontrado()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var cliente = new ClienteBuilder().Build();
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, cliente.Id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>(cliente);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não encontrado")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoNaoEncontrado()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var logger = MockLogger.CriarSimples();
            _fixture.ClienteGatewayMock.AoObterPorId(id).LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve buscar cliente quando cliente busca seus próprios dados")]
        public async Task ExecutarAsync_DeveBuscarCliente_QuandoClienteBuscaPropriosDados()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var clienteId = cliente.Id;
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var logger = MockLogger.CriarSimples();
            
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, cliente.Id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>(cliente);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar dados de outro cliente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarDadosDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var cliente = new ClienteBuilder().Build(); // Outro cliente diferente do ator
            var logger = MockLogger.CriarSimples();
            
            _fixture.ClienteGatewayMock.AoObterPorId(cliente.Id).Retorna(cliente);

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, cliente.Id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorIdPresenterMock.DeveTerApresentadoErro<IBuscarClientePorIdPresenter, ClienteAggregate>("Acesso negado. Somente administradores ou o próprio cliente podem acessar os dados.", ErrorType.NotAllowed);
            _fixture.BuscarClientePorIdPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorIdPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarClientePorId")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            _fixture.ClienteGatewayMock.AoObterPorId(id).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarClientePorId")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var id = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();
            _fixture.ClienteGatewayMock.AoObterPorId(id).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarClientePorIdUseCase.ExecutarAsync(ator, id, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorIdPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
