using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Bogus;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente
{
    public class BuscarClientePorDocumentoUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;

        public BuscarClientePorDocumentoUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
        }

        [Fact(DisplayName = "Deve buscar cliente com sucesso quando cliente existir")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveBuscarClienteComSucesso_QuandoClienteExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var documento = clienteExistente.DocumentoIdentificador.Valor;
            var logger = MockLogger.CriarSimples();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).Retorna(clienteExistente);

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>(clienteExistente);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var logger = MockLogger.CriarSimples();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var logger = MockLogger.CriarSimples();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve buscar cliente quando cliente busca seus próprios dados")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveBuscarCliente_QuandoClienteBuscaPropriosDados()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var clienteId = clienteExistente.Id;
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var documento = clienteExistente.DocumentoIdentificador.Valor;
            var logger = MockLogger.CriarSimples();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).Retorna(clienteExistente);

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>(clienteExistente);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar dados de outro cliente")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarDadosDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var clienteExistente = new ClienteBuilder().Build(); // Outro cliente diferente do ator
            var documento = clienteExistente.DocumentoIdentificador.Valor;
            var logger = MockLogger.CriarSimples();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).Retorna(clienteExistente);

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarClientePorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarClientePorDocumentoPresenter, ClienteAggregate>("Acesso negado. Somente administradores ou o próprio cliente podem acessar os dados.", ErrorType.NotAllowed);
            _fixture.BuscarClientePorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarClientePorDocumentoPresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarClientePorDocumento")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documento).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarClientePorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.ClienteGatewayMock.Object, _fixture.BuscarClientePorDocumentoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}