using Application.Contracts.Presenters;
using Bogus;
using Shared.Enums;
using Tests.Application.Identidade.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using UsuarioAggregate = Domain.Identidade.Aggregates.Usuario;

namespace Tests.Application.Identidade
{
    public class BuscarUsuarioPorDocumentoUseCaseTest
    {
        private readonly IdentidadeTestFixture _fixture;

        public BuscarUsuarioPorDocumentoUseCaseTest()
        {
            _fixture = new IdentidadeTestFixture();
        }

        [Fact(DisplayName = "Deve buscar usuário com sucesso quando usuário existir")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveBuscarUsuarioComSucesso_QuandoUsuarioExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var usuarioExistente = new UsuarioBuilder().Build();
            var documento = usuarioExistente.DocumentoIdentificadorUsuario.Valor;
            var logger = MockLogger.CriarSimples();

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).Retorna(usuarioExistente);

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.DeveTerApresentadoSucesso<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>(usuarioExistente);
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.NaoDeveTerApresentadoErro<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando usuário não existir")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoUsuarioNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var logger = MockLogger.CriarSimples();

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>("Usuário não encontrado.", ErrorType.ResourceNotFound);
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var logger = MockLogger.CriarSimples();

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta buscar usuário por documento")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaBuscarUsuarioPorDocumento()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, logger);

            // Assert
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.DeveTerApresentadoErro<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>("Acesso negado. Apenas administradores podem buscar usuários.", ErrorType.NotAllowed);
            _fixture.BuscarUsuarioPorDocumentoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarUsuarioPorDocumentoPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "BuscarUsuarioPorDocumento")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = new Faker("pt_BR").Random.Replace("###########");
            var mockLogger = MockLogger.Criar();
            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarUsuarioPorDocumentoUseCase.ExecutarAsync(ator, documento, _fixture.UsuarioGatewayMock.Object, _fixture.BuscarUsuarioPorDocumentoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}