using Application.Identidade.Services;
using FluentAssertions;
using Tests.Application.SharedHelpers;
using Xunit;

namespace Tests.Application.Identidade;

public class AtorTests
{
    #region Factory Methods

    [Fact(DisplayName = "Deve criar ator com role Sistema quando chamado")]
    [Trait("Application", "Ator")]
    public void Sistema_DeveCriarAtorComRoleSistema_QuandoChamado()
    {
        // Act
        var ator = Ator.Sistema();

        // Assert
        ator.Should().NotBeNull();
        ator.UsuarioId.Should().Be(Guid.Empty);
        ator.ClienteId.Should().BeNull();
        ator.Roles.Should().ContainSingle()
            .Which.Should().Be(RoleEnum.Sistema);
    }

    #endregion

    #region EhCliente

    [Fact(DisplayName = "Deve retornar true quando role Cliente presente")]
    [Trait("Application", "Ator")]
    public void EhCliente_DeveRetornarTrue_QuandoRoleClientePresente()
    {
        // Arrange
        var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();

        // Act
        var resultado = ator.EhCliente();

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar false quando role Cliente ausente")]
    [Trait("Application", "Ator")]
    public void EhCliente_DeveRetornarFalse_QuandoRoleClienteAusente()
    {
        // Arrange
        var ator = new AtorBuilder().ComoAdministrador().Build();

        // Act
        var resultado = ator.EhCliente();

        // Assert
        resultado.Should().BeFalse();
    }

    #endregion

    #region PodeAcionarWebhooks

    [Fact(DisplayName = "Deve retornar true quando Sistema ou Administrador")]
    [Trait("Application", "Ator")]
    public void PodeAcionarWebhooks_DeveRetornarTrue_QuandoSistemaOuAdministrador()
    {
        // Arrange
        var atorSistema = new AtorBuilder().ComoSistema().Build();
        var atorAdministrador = new AtorBuilder().ComoAdministrador().Build();

        // Act
        var resultadoSistema = atorSistema.PodeAcionarWebhooks();
        var resultadoAdministrador = atorAdministrador.PodeAcionarWebhooks();

        // Assert
        resultadoSistema.Should().BeTrue();
        resultadoAdministrador.Should().BeTrue();
    }

    #endregion
}
