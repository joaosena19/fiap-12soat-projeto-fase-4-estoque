using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Identidade.Enums;
using Shared.Enums;
using Tests.Application.Cadastros.Cliente.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;
using UsuarioAggregate = Domain.Identidade.Aggregates.Usuario;
using Shared.Exceptions;

namespace Tests.Application.Cadastros.Cliente
{
    public class CriarClienteUseCaseTest
    {
        private readonly ClienteTestFixture _fixture;
        private readonly MockLogger _mockLogger;

        public CriarClienteUseCaseTest()
        {
            _fixture = new ClienteTestFixture();
            _mockLogger = MockLogger.Criar();
        }

        [Fact(DisplayName = "Deve criar cliente com sucesso quando usuário é admin")]
        public async Task ExecutarAsync_DeveCriarClienteComSucesso_QuandoUsuarioEhAdmin()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<ICriarClientePresenter, ClienteAggregate>();
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve criar cliente com sucesso quando usuário cria com próprio documento")]
        public async Task ExecutarAsync_DeveCriarClienteComSucesso_QuandoUsuarioCriaComProprioDocumento()
        {
            // Arrange
            var documentoComum = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var usuario = new UsuarioBuilder().ComDocumento(documentoComum).Build();
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).ComUsuario(usuario.Id).Build(); // Usuário com cliente ID qualquer
            var novoCliente = new ClienteBuilder().ComDocumento(documentoComum).Build(); // Cliente com mesmo documento do usuário
            
            _fixture.UsuarioGatewayMock.AoObterPorId(usuario.Id).Retorna(usuario);
            _fixture.ClienteGatewayMock.AoObterPorDocumento(novoCliente.DocumentoIdentificador.Valor).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(novoCliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, novoCliente.Nome.Valor, novoCliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<ICriarClientePresenter, ClienteAggregate>();
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando usuário tenta criar cliente com documento diferente")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoUsuarioTentaCriarClienteComDocumentoDiferente()
        {
            // Arrange
            var documentoUsuario = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var documentoDiferente = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var usuario = new UsuarioBuilder().ComDocumento(documentoUsuario).Build();
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).ComUsuario(usuario.Id).Build();
            
            _fixture.UsuarioGatewayMock.AoObterPorId(usuario.Id).Retorna(usuario);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, "Nome Qualquer", documentoDiferente, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Acesso negado. Administradores podem cadastrar qualquer cliente, usuários podem criar cliente apenas com o mesmo documento.", ErrorType.NotAllowed);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe cliente com documento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteJaExiste()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).Retorna(cliente);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var nomeInvalido = "";
            var documentoValido = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var ator = new AtorBuilder().ComoAdministrador().Build();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(documentoValido).NaoRetornaNada();

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, nomeInvalido, documentoValido, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, _mockLogger.Object);

            // Assert
            _fixture.CriarClientePresenterMock.DeveTerApresentadoErro<ICriarClientePresenter, ClienteAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarClientePresenterMock.NaoDeveTerApresentadoSucesso<ICriarClientePresenter, ClienteAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando ocorrer DomainException")]
        [Trait("UseCase", "CriarCliente")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOcorrerDomainException()
        {
            // Arrange
            var documentoUsuario = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var documentoDiferente = new ClienteBuilder().ComCpfValido().Build().DocumentoIdentificador.Valor;
            var usuario = new UsuarioBuilder().ComDocumento(documentoUsuario).Build();
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).ComUsuario(usuario.Id).Build();
            var mockLogger = MockLogger.Criar();
            
            _fixture.UsuarioGatewayMock.AoObterPorId(usuario.Id).Retorna(usuario);

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, "Nome Qualquer", documentoDiferente, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
            mockLogger.NaoDeveTerLogadoNenhumError();
        }

        [Fact(DisplayName = "Deve logar error quando ocorrer Exception")]
        [Trait("UseCase", "CriarCliente")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerException()
        {
            // Arrange
            var cliente = new ClienteBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var mockLogger = MockLogger.Criar();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(cliente.DocumentoIdentificador.Valor).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarClienteUseCase.ExecutarAsync(ator, cliente.Nome.Valor, cliente.DocumentoIdentificador.Valor, _fixture.ClienteGatewayMock.Object, _fixture.UsuarioGatewayMock.Object, _fixture.CriarClientePresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
