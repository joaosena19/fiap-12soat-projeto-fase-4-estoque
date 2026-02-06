using Domain.Identidade.Aggregates;
using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using FluentAssertions;
using Shared.Exceptions;
using Shared.Enums;

namespace Tests.Domain.Identidade
{
    public class RoleUnitTest
    {
        #region Testes Construtor com RoleEnum

        [Theory(DisplayName = "Deve criar Role com RoleEnum válido")]
        [InlineData(RoleEnum.Administrador, "Administrador")]
        [InlineData(RoleEnum.Cliente, "Cliente")]
        [Trait("Construtor", "RoleEnum")]
        public void RoleConstrutor_Deve_CriarRole_Quando_RoleEnumValido(RoleEnum roleEnum, string expectedNome)
        {
            // Act
            var role = new Role(roleEnum);

            // Assert
            role.Should().NotBeNull();
            role.Id.Should().Be(roleEnum);
            role.Nome.Valor.Should().Be(expectedNome);
        }

        #endregion

        #region Testes Construtor com String

        [Theory(DisplayName = "Deve criar Role com strings válidas")]
        [InlineData("Administrador", RoleEnum.Administrador)]
        [InlineData("Cliente", RoleEnum.Cliente)]
        [InlineData("administrador", RoleEnum.Administrador)] // Case insensitive
        [InlineData("cliente", RoleEnum.Cliente)] // Case insensitive
        [InlineData("ADMINISTRADOR", RoleEnum.Administrador)] // Case insensitive
        [InlineData("CLIENTE", RoleEnum.Cliente)] // Case insensitive
        [InlineData("1", RoleEnum.Administrador)] // Valor númerico do enum
        [InlineData("2", RoleEnum.Cliente)] // Valor númerico do enum
        [Trait("Construtor", "String")]
        public void RoleConstrutor_Deve_CriarRole_Quando_StringValida(string roleString, RoleEnum expectedEnum)
        {
            // Act
            var role = new Role(roleString);

            // Assert
            role.Should().NotBeNull();
            role.Id.Should().Be(expectedEnum);
            role.Nome.Valor.Should().Be(expectedEnum.ToString());
        }

        [Theory(DisplayName = "Não deve criar Role com strings inválidas")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("RoleInvalida")]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("123")]
        [InlineData("4")] // Enum não possui valor 4
        [InlineData(null)]
        [Trait("Construtor", "String")]
        public void RoleConstrutor_Deve_ThrowException_Quando_StringInvalida(string? roleStringInvalida)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Role(roleStringInvalida!))
                .Should().Throw<DomainException>()
                .Which.ErrorType.Should().Be(ErrorType.InvalidInput);
        }

        #endregion

        #region Testes Métodos Factory


        [Theory(DisplayName = "Deve criar Role via From com RoleEnum")]
        [InlineData(RoleEnum.Administrador)]
        [InlineData(RoleEnum.Cliente)]
        [Trait("Factory", "FromEnum")]
        public void RoleFromEnum_Deve_CriarRole_Quando_RoleEnumValido(RoleEnum roleEnum)
        {
            // Act
            var role = Role.From(roleEnum);

            // Assert
            role.Should().NotBeNull();
            role.Id.Should().Be(roleEnum);
            role.Nome.Valor.Should().Be(roleEnum.ToString());
        }

        [Theory(DisplayName = "Deve criar Role via From com string")]
        [InlineData("Administrador", RoleEnum.Administrador)]
        [InlineData("Cliente", RoleEnum.Cliente)]
        [InlineData("administrador", RoleEnum.Administrador)]
        [InlineData("cliente", RoleEnum.Cliente)]
        [Trait("Factory", "FromString")]
        public void RoleFromString_Deve_CriarRole_Quando_StringValida(string roleString, RoleEnum expectedEnum)
        {
            // Act
            var role = Role.From(roleString);

            // Assert
            role.Should().NotBeNull();
            role.Id.Should().Be(expectedEnum);
            role.Nome.Valor.Should().Be(expectedEnum.ToString());
        }

        #endregion

        #region Testes ValueObject NomeRole

        [Theory(DisplayName = "Deve criar NomeRole com enum válido")]
        [InlineData(RoleEnum.Administrador, "Administrador")]
        [InlineData(RoleEnum.Cliente, "Cliente")]
        [Trait("ValueObject", "NomeRole")]
        public void NomeRole_Deve_CriarNome_Quando_EnumValido(RoleEnum roleEnum, string expectedNome)
        {
            // Act
            var nomeRole = new NomeRole(roleEnum);

            // Assert
            nomeRole.Should().NotBeNull();
            nomeRole.Valor.Should().Be(expectedNome);
        }

        #endregion

        #region Testes Igualdade

        [Fact(DisplayName = "Roles com mesmo enum devem ser consideradas iguais pelo Id")]
        [Trait("Igualdade", "Id")]
        public void Role_Deve_SerIgual_Quando_MesmoEnum()
        {
            // Arrange
            var role1 = Role.Administrador();
            var role2 = new Role(RoleEnum.Administrador);

            // Assert
            role1.Id.Should().Be(role2.Id);
            role1.Nome.Valor.Should().Be(role2.Nome.Valor);
        }

        [Fact(DisplayName = "Roles com enums diferentes devem ser diferentes")]
        [Trait("Igualdade", "Id")]
        public void Role_Deve_SerDiferente_Quando_EnumDiferente()
        {
            // Arrange
            var roleAdmin = Role.Administrador();
            var roleCliente = Role.Cliente();

            // Assert
            roleAdmin.Id.Should().NotBe(roleCliente.Id);
            roleAdmin.Nome.Valor.Should().NotBe(roleCliente.Nome.Valor);
        }

        #endregion
    }
}