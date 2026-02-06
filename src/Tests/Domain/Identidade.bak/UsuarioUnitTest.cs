using Domain.Identidade.Aggregates;
using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using FluentAssertions;
using Shared.Exceptions;
using Tests.Helpers;

namespace Tests.Domain.Identidade
{
    public class UsuarioUnitTest
    {
        #region Testes Método Criar

        [Fact(DisplayName = "Deve criar novo Usuário com dados válidos e lista de roles")]
        [Trait("Método", "Criar")]
        public void UsuarioCriar_Deve_CriarUsuario_Quando_DadosValidosComListaRoles()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Administrador(), Role.Cliente() };

            // Act
            var usuario = Usuario.Criar(documento, senhaHash, roles);

            // Assert
            usuario.Should().NotBeNull();
            usuario.Id.Should().NotBe(Guid.Empty);
            usuario.DocumentoIdentificadorUsuario.Valor.Should().Be(documento);
            usuario.SenhaHash.Valor.Should().Be(senhaHash);
            usuario.Roles.Should().HaveCount(2);
            usuario.Roles.Should().Contain(r => r.Id == RoleEnum.Administrador);
            usuario.Roles.Should().Contain(r => r.Id == RoleEnum.Cliente);
        }

        [Fact(DisplayName = "Deve criar novo Usuário com role única")]
        [Trait("Método", "Criar")]
        public void UsuarioCriar_Deve_CriarUsuario_Quando_DadosValidosComRoleUnica()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var role = Role.Cliente();

            // Act
            var usuario = Usuario.Criar(documento, senhaHash, role);

            // Assert
            usuario.Should().NotBeNull();
            usuario.Id.Should().NotBe(Guid.Empty);
            usuario.DocumentoIdentificadorUsuario.Valor.Should().Be(documento);
            usuario.SenhaHash.Valor.Should().Be(senhaHash);
            usuario.Roles.Should().HaveCount(1);
            usuario.Roles.First().Id.Should().Be(RoleEnum.Cliente);
        }

        #endregion

        #region Testes ValueObject DocumentoIdentificadorUsuario

        [Fact(DisplayName = "Não deve criar novo Usuário com CPF inválido")]
        [Trait("ValueObject", "DocumentoIdentificadorUsuario")]
        public void UsuarioCriar_Deve_ThrowException_Quando_CpfInvalido()
        {
            // Arrange
            var cpfInvalido = DocumentoHelper.GerarCpfInvalido();
            var senhaHashValida = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Cliente() };

            // Act & Assert
            FluentActions.Invoking(() => Usuario.Criar(cpfInvalido, senhaHashValida, roles))
                .Should().Throw<DomainException>()
                .WithMessage("*Documento*identifica*inv*");
        }

        [Fact(DisplayName = "Não deve criar novo Usuário com CNPJ inválido")]
        [Trait("ValueObject", "DocumentoIdentificadorUsuario")]
        public void UsuarioCriar_Deve_ThrowException_Quando_CnpjInvalido()
        {
            // Arrange
            var cnpjInvalido = DocumentoHelper.GerarCnpjInvalido();
            var senhaHashValida = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Cliente() };

            // Act & Assert
            FluentActions.Invoking(() => Usuario.Criar(cnpjInvalido, senhaHashValida, roles))
                .Should().Throw<DomainException>()
                .WithMessage("*Documento*identifica*inv*");
        }

        [Fact(DisplayName = "Deve criar Usuário com CPF válido")]
        [Trait("Dados Válidos", "CPF")]
        public void UsuarioCriar_Deve_CriarUsuario_QuandoCpfValido()
        {
            // Arrange
            var cpfValido = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Cliente() };

            // Act
            var usuario = Usuario.Criar(cpfValido, senhaHash, roles);

            // Assert
            usuario.Should().NotBeNull();
            usuario.DocumentoIdentificadorUsuario.Valor.Should().MatchRegex(@"^\d{11}$"); // CPF deve ter 11 dígitos
        }

        [Fact(DisplayName = "Deve criar Usuário com CNPJ válido")]
        [Trait("Dados Válidos", "CNPJ")]
        public void UsuarioCriar_Deve_CriarUsuario_QuandoCnpjValido()
        {
            // Arrange
            var cnpjValido = DocumentoHelper.GerarCnpjValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Cliente() };

            // Act
            var usuario = Usuario.Criar(cnpjValido, senhaHash, roles);

            // Assert
            usuario.Should().NotBeNull();
            usuario.DocumentoIdentificadorUsuario.Valor.Should().MatchRegex(@"^\d{14}$"); // CNPJ deve ter 14 dígitos
        }

        #endregion

        #region Testes ValueObject SenhaHash

        [Theory(DisplayName = "Não deve criar novo Usuário se a senha hash for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [Trait("ValueObject", "SenhaHash")]
        public void UsuarioCriar_Deve_ThrowException_Quando_SenhaHashInvalida(string? senhaHashInvalida)
        {
            // Arrange
            var documentoValido = DocumentoHelper.GerarCpfValido();
            var roles = new List<Role> { Role.Cliente() };

            // Act & Assert
            FluentActions.Invoking(() => Usuario.Criar(documentoValido, senhaHashInvalida!, roles))
                .Should().Throw<ArgumentException>()
                .WithMessage("*Senha hash*vazia*");
        }

        #endregion

        #region Testes Roles

        [Fact(DisplayName = "Não deve criar usuário com lista de roles vazia")]
        [Trait("Roles", "ListaVazia")]
        public void UsuarioCriar_Deve_ThrowException_Quando_ListaRolesVazia()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var rolesVazias = new List<Role>();

            // Act & Assert
            FluentActions.Invoking(() => Usuario.Criar(documento, senhaHash, rolesVazias))
                .Should().Throw<DomainException>();
        }

        #endregion

        #region Testes ValueObject StatusUsuario

        [Fact(DisplayName = "Deve criar usuário com status Ativo por padrão")]
        [Trait("ValueObject", "StatusUsuario")]
        public void UsuarioCriar_Deve_TerStatusAtivo_Quando_Criado()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var role = Role.Cliente();

            // Act
            var usuario = Usuario.Criar(documento, senhaHash, role);

            // Assert
            usuario.Should().NotBeNull();
            usuario.Status.Should().NotBeNull();
            usuario.Status.Valor.Should().Be(StatusUsuarioEnum.Ativo);
            usuario.Status.EstaAtivo().Should().BeTrue();
            usuario.Status.EstaInativo().Should().BeFalse();
        }

        [Fact(DisplayName = "Deve permitir ativar usuário")]
        [Trait("ValueObject", "StatusUsuario")]
        public void Usuario_Deve_PermitirAtivar_Quando_ChamadoMetodoAtivar()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var role = Role.Cliente();
            var usuario = Usuario.Criar(documento, senhaHash, role);

            // Act
            usuario.Ativar();

            // Assert
            usuario.Status.Valor.Should().Be(StatusUsuarioEnum.Ativo);
            usuario.Status.EstaAtivo().Should().BeTrue();
        }

        [Fact(DisplayName = "Deve permitir inativar usuário")]
        [Trait("ValueObject", "StatusUsuario")]
        public void Usuario_Deve_PermitirInativar_Quando_ChamadoMetodoInativar()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var role = Role.Cliente();
            var usuario = Usuario.Criar(documento, senhaHash, role);

            // Act
            usuario.Inativar();

            // Assert
            usuario.Status.Valor.Should().Be(StatusUsuarioEnum.Inativo);
            usuario.Status.EstaInativo().Should().BeTrue();
            usuario.Status.EstaAtivo().Should().BeFalse();
        }

        [Theory(DisplayName = "Deve criar StatusUsuario com factory methods")]
        [InlineData(StatusUsuarioEnum.Ativo)]
        [InlineData(StatusUsuarioEnum.Inativo)]
        [Trait("ValueObject", "StatusUsuario")]
        public void StatusUsuario_Deve_CriarComFactory_Quando_EnumValido(StatusUsuarioEnum statusEnum)
        {
            // Act
            var status = statusEnum == StatusUsuarioEnum.Ativo 
                ? StatusUsuario.Ativo() 
                : StatusUsuario.Inativo();

            // Assert
            status.Should().NotBeNull();
            status.Valor.Should().Be(statusEnum);
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar usuário")]
        [Trait("Método", "Criar")]
        public void UsuarioCriar_Deve_GerarUuidVersao7_Quando_CriarUsuario()
        {
            // Arrange
            var documento = DocumentoHelper.GerarCpfValido();
            var senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
            var roles = new List<Role> { Role.Cliente() };

            // Act
            var usuario = Usuario.Criar(documento, senhaHash, roles);

            // Assert
            usuario.Id.Should().NotBe(Guid.Empty);
            var guidString = usuario.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}