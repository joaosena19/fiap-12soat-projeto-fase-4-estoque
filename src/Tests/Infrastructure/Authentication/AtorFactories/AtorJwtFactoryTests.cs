using Application.Identidade.Services;
using FluentAssertions;
using Infrastructure.Authentication.AtorFactories;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Helpers;

namespace Tests.Infrastructure.Authentication.AtorFactories;

[Trait("Infrastructure", "AtorJwtFactory")]
public class AtorJwtFactoryTests
{
    #region Testes de Erro

    [Fact(DisplayName = "Deve lançar Unauthorized quando token inválido")]
    public void CriarAtor_DeveLancarUnauthorized_QuandoTokenInvalido()
    {
        // Arrange
        var tokenInvalido = new JwtTokenBuilder().BuildTokenInvalido();

        // Act & Assert
        FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenInvalido))
            .Should().Throw<DomainException>()
            .WithMessage("Token JWT inválido")
            .Where(ex => ex.ErrorType == ErrorType.Unauthorized);
    }

    [Fact(DisplayName = "Deve lançar Unauthorized quando userId ausente")]
    public void CriarAtor_DeveLancarUnauthorized_QuandoUserIdAusente()
    {
        // Arrange
        var tokenSemUserId = new JwtTokenBuilder()
            .ComRole(RoleEnum.Administrador)
            .Build();

        // Act & Assert
        FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenSemUserId))
            .Should().Throw<DomainException>()
            .WithMessage("Token deve conter userId válido")
            .Where(ex => ex.ErrorType == ErrorType.Unauthorized);
    }

    [Fact(DisplayName = "Deve lançar Unauthorized quando userId inválido")]
    public void CriarAtor_DeveLancarUnauthorized_QuandoUserIdInvalido()
    {
        // Arrange
        var tokenComUserIdInvalido = new JwtTokenBuilder()
            .ComClaimCustomizada("userId", "nao-eh-um-guid")
            .ComRole(RoleEnum.Administrador)
            .Build();

        // Act & Assert
        FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenComUserIdInvalido))
            .Should().Throw<DomainException>()
            .WithMessage("Token deve conter userId válido")
            .Where(ex => ex.ErrorType == ErrorType.Unauthorized);
    }

    [Fact(DisplayName = "Deve lançar Unauthorized quando roles ausentes")]
    public void CriarAtor_DeveLancarUnauthorized_QuandoRolesAusentes()
    {
        // Arrange
        var tokenSemRoles = new JwtTokenBuilder()
            .ComUsuarioId(Guid.NewGuid())
            .Build();

        // Act & Assert
        FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenSemRoles))
            .Should().Throw<DomainException>()
            .WithMessage("Token deve conter pelo menos uma role")
            .Where(ex => ex.ErrorType == ErrorType.Unauthorized);
    }

    [Fact(DisplayName = "Deve lançar Unauthorized quando role inválida")]
    public void CriarAtor_DeveLancarUnauthorized_QuandoRoleInvalida()
    {
        // Arrange
        var tokenComRoleInvalida = new JwtTokenBuilder()
            .ComUsuarioId(Guid.NewGuid())
            .ComClaimCustomizada("role", "RoleInexistente")
            .Build();

        // Act & Assert
        FluentActions.Invoking(() => AtorJwtFactory.CriarPorTokenJwt(tokenComRoleInvalida))
            .Should().Throw<DomainException>()
            .WithMessage("Role 'RoleInexistente' não é válida*")
            .Where(ex => ex.ErrorType == ErrorType.Unauthorized);
    }

    #endregion

    #region Testes de Sucesso

    [Fact(DisplayName = "Deve criar ator quando token válido")]
    public void CriarAtor_DeveCriarAtor_QuandoTokenValido()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var tokenValido = new JwtTokenBuilder()
            .ComUsuarioId(usuarioId)
            .ComClienteId(clienteId)
            .ComRoles(RoleEnum.Administrador, RoleEnum.Cliente)
            .Build();

        // Act
        var ator = AtorJwtFactory.CriarPorTokenJwt(tokenValido);

        // Assert
        ator.Should().NotBeNull();
        ator.UsuarioId.Should().Be(usuarioId);
        ator.ClienteId.Should().Be(clienteId);
        ator.Roles.Should().HaveCount(2);
        ator.Roles.Should().Contain(RoleEnum.Administrador);
        ator.Roles.Should().Contain(RoleEnum.Cliente);
    }

    #endregion
}
