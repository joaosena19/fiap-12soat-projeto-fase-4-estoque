using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Bogus;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;
using Shared.Exceptions;

namespace Tests.Application.Cadastros.Cliente
{
    public class AtualizarClienteUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;
        private readonly MockLogger _mockLogger;

        public AtualizarClienteUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
            _mockLogger = MockLogger.Criar();
        }

        [Fact(DisplayName = "Deve atualizar cliente com sucesso quando cliente existir e usuário tem permissão")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveAtualizarClienteComSucesso_QuandoClienteExistirEUsuarioTemPermissao()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var nomeOriginal = clienteExistente.Nome.Valor;
            var novoNome = new Faker("pt_BR").Person.FullName;

            ClienteAggregate? clienteAtualizado = null;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().ComCallback(cliente => clienteAtualizado = cliente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be(novoNome);

            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarClientePresenter, ClienteAggregate>();
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente não existir")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteId = Guid.NewGuid();
            var nomeAtualizado = new Faker("pt_BR").Person.FullName;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteId, nomeAtualizado, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Cliente não encontrado.", ErrorType.ResourceNotFound);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var nomeInvalido = ""; // Nome inválido para provocar DomainException

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, nomeInvalido, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var novoNome = new Faker("pt_BR").Person.FullName;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve atualizar cliente quando cliente edita seus próprios dados")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveAtualizarCliente_QuandoClienteEditaPropriosDados()
        {
            // Arrange
            var clienteExistente = new ClienteBuilder().Build();
            var clienteId = clienteExistente.Id;
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var novoNome = new Faker("pt_BR").Person.FullName;

            ClienteAggregate? clienteAtualizado = null;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().ComCallback(cliente => clienteAtualizado = cliente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be(novoNome);

            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarClientePresenter, ClienteAggregate>();
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta editar dados de outro cliente")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaEditarDadosDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var clienteExistente = new ClienteBuilder().Build(); // Outro cliente diferente do ator
            var novoNome = new Faker("pt_BR").Person.FullName;

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.AtualizarClientePresenterMock.DeveTerApresentadoErro<IAtualizarClientePresenter, ClienteAggregate>("Acesso negado. Somente administradores ou o próprio cliente podem editar os dados.", ErrorType.NotAllowed);
            _fixture.AtualizarClientePresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var clienteExistente = new ClienteBuilder().Build(); // Outro cliente diferente do ator
            var novoNome = new Faker("pt_BR").Person.FullName;
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
            mockLogger.NaoDeveTerLogadoNenhumError();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "AtualizarCliente")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var novoNome = new Faker("pt_BR").Person.FullName;
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorId(clienteExistente.Id).Retorna(clienteExistente);
            _fixture.ClienteGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AtualizarClienteUseCase.ExecutarAsync(ator, clienteExistente.Id, novoNome, _fixture.ClienteGatewayMock.Object, _fixture.AtualizarClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}