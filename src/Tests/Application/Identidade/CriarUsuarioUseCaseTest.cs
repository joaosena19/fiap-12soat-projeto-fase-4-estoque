using Application.Contracts.Presenters;
using Domain.Identidade.Aggregates;
using Moq;
using Shared.Enums;
using Tests.Application.Identidade.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using Tests.Helpers;
using UsuarioAggregate = Domain.Identidade.Aggregates.Usuario;

namespace Tests.Application.Identidade
{
    public class CriarUsuarioUseCaseTest
    {
        private readonly IdentidadeTestFixture _fixture;

        public CriarUsuarioUseCaseTest()
        {
            _fixture = new IdentidadeTestFixture();
        }

        [Fact(DisplayName = "Deve criar usuário com sucesso")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveCriarUsuarioComSucesso()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaNaoHasheada = "senha123";
            var senhaHasheada = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$hashedpassword123";
            var logger = MockLogger.CriarSimples();

            var dto = new CriarUsuarioDtoBuilder()
                .ComDocumento(documento)
                .ComSenhaNaoHasheada(senhaNaoHasheada)
                .ComRoleCliente()
                .Build();

            var usuarioEsperado = new UsuarioBuilder()
                .ComDocumento(documento)
                .ComSenhaHash(senhaHasheada)
                .ComRoleCliente()
                .Build();

            var rolesEsperadas = new List<Role> { Role.Cliente() };

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();
            _fixture.UsuarioGatewayMock.AoObterRoles(dto.Roles).Retorna(rolesEsperadas);
            _fixture.PasswordHasherMock.Setup(ph => ph.Hash(senhaNaoHasheada)).Returns(senhaHasheada);
            _fixture.UsuarioGatewayMock.AoSalvar().Retorna(usuarioEsperado);

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, logger);

            // Assert
            _fixture.CriarUsuarioPresenterMock.DeveTerApresentadoSucesso<ICriarUsuarioPresenter, UsuarioAggregate>(usuarioEsperado);
            _fixture.CriarUsuarioPresenterMock.NaoDeveTerApresentadoErro<ICriarUsuarioPresenter, UsuarioAggregate>();
            _fixture.PasswordHasherMock.Verify(ph => ph.Hash(senhaNaoHasheada), Times.Once);
        }

        [Fact(DisplayName = "Deve apresentar erro quando já existe usuário com documento")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoUsuarioJaExiste()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documento = DocumentoHelper.GerarCpfValido();
            var usuarioExistente = new UsuarioBuilder().ComDocumento(documento).Build();
            var logger = MockLogger.CriarSimples();

            var dto = new CriarUsuarioDtoBuilder()
                .ComDocumento(documento)
                .ComRoleCliente()
                .Build();

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documento).Retorna(usuarioExistente);

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, logger);

            // Assert
            _fixture.CriarUsuarioPresenterMock.DeveTerApresentadoErro<ICriarUsuarioPresenter, UsuarioAggregate>("Já existe um usuário cadastrado com este documento.", ErrorType.Conflict);
            _fixture.CriarUsuarioPresenterMock.NaoDeveTerApresentadoSucesso<ICriarUsuarioPresenter, UsuarioAggregate>();
            _fixture.PasswordHasherMock.Verify(ph => ph.Hash(It.IsAny<string>()), Times.Never);
        }


        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer um erro de domínio")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoErroDocumentoInvalido()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var documentoInvalido = "documento_invalido";
            var logger = MockLogger.CriarSimples();
            var dto = new CriarUsuarioDtoBuilder()
                .ComDocumento(documentoInvalido)
                .ComRoleCliente()
                .Build();

            var rolesEsperadas = new List<Role> { Role.Cliente() };

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(documentoInvalido).NaoRetornaNada();
            _fixture.UsuarioGatewayMock.AoObterRoles(dto.Roles).Retorna(rolesEsperadas);
            _fixture.PasswordHasherMock.Setup(ph => ph.Hash("senha123")).Returns("hashedpassword");

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, logger);

            // Assert
            _fixture.CriarUsuarioPresenterMock.DeveTerApresentadoErro<ICriarUsuarioPresenter, UsuarioAggregate>("Documento de identificação de usuário inválido", ErrorType.InvalidInput);
            _fixture.CriarUsuarioPresenterMock.NaoDeveTerApresentadoSucesso<ICriarUsuarioPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var logger = MockLogger.CriarSimples();
            var dto = new CriarUsuarioDtoBuilder()
                .ComRoleCliente()
                .Build();

            var rolesEsperadas = new List<Role> { Role.Cliente() };

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(dto.DocumentoIdentificador).NaoRetornaNada();
            _fixture.UsuarioGatewayMock.AoObterRoles(dto.Roles).Retorna(rolesEsperadas);
            _fixture.PasswordHasherMock.Setup(ph => ph.Hash("senha123")).Returns("hashedpassword");
            _fixture.UsuarioGatewayMock.AoSalvar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, logger);

            // Assert
            _fixture.CriarUsuarioPresenterMock.DeveTerApresentadoErro<ICriarUsuarioPresenter, UsuarioAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarUsuarioPresenterMock.NaoDeveTerApresentadoSucesso<ICriarUsuarioPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta criar usuário")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaCriarUsuario()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var logger = MockLogger.CriarSimples();
            var dto = new CriarUsuarioDtoBuilder()
                .ComRoleCliente()
                .Build();

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, logger);

            // Assert
            _fixture.CriarUsuarioPresenterMock.DeveTerApresentadoErro<ICriarUsuarioPresenter, UsuarioAggregate>("Acesso negado. Apenas administradores podem criar usuários.", ErrorType.NotAllowed);
            _fixture.CriarUsuarioPresenterMock.NaoDeveTerApresentadoSucesso<ICriarUsuarioPresenter, UsuarioAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var dto = new CriarUsuarioDtoBuilder().ComRoleCliente().Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "CriarUsuario")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarUsuarioDtoBuilder().ComRoleCliente().Build();
            var mockLogger = MockLogger.Criar();
            var rolesEsperadas = new List<Role> { Role.Cliente() };

            _fixture.UsuarioGatewayMock.AoObterPorDocumento(dto.DocumentoIdentificador).NaoRetornaNada();
            _fixture.UsuarioGatewayMock.AoObterRoles(dto.Roles).Retorna(rolesEsperadas);
            _fixture.PasswordHasherMock.Setup(ph => ph.Hash("senha123")).Returns("hashedpassword");
            _fixture.UsuarioGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarUsuarioUseCase.ExecutarAsync(ator, dto, _fixture.UsuarioGatewayMock.Object, _fixture.CriarUsuarioPresenterMock.Object, _fixture.PasswordHasherMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}