using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Identidade.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Tests.API.Endpoints;

/// <summary>
/// Testes para BaseController
/// </summary>
public class BaseControllerTests
{
    private readonly TestController _controller;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;

    public BaseControllerTests()
    {
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _controller = new TestController(_loggerFactoryMock.Object);
    }

    #region BuscarAtorAtual

    [Fact(DisplayName = "Deve lançar UnauthorizedAccessException quando Authorization header ausente")]
    [Trait("API", "BaseController")]
    public void BuscarAtorAtual_DeveLancarUnauthorized_QuandoAuthorizationHeaderAusente()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act & Assert
        FluentActions.Invoking(() => _controller.BuscarAtorAtualPublic())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Token de autorização é obrigatório");
    }

    [Fact(DisplayName = "Deve lançar UnauthorizedAccessException quando Authorization header não for Bearer")]
    [Trait("API", "BaseController")]
    public void BuscarAtorAtual_DeveLancarUnauthorized_QuandoAuthorizationHeaderNaoForBearer()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Basic abc123";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act & Assert
        FluentActions.Invoking(() => _controller.BuscarAtorAtualPublic())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Token de autorização é obrigatório");
    }

    [Fact(DisplayName = "Deve retornar Ator quando Authorization header for Bearer válido")]
    [Trait("API", "BaseController")]
    public void BuscarAtorAtual_DeveRetornarAtor_QuandoAuthorizationHeaderForBearer()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var token = CriarTokenJwtValido(usuarioId, clienteId, "Administrador");

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var ator = _controller.BuscarAtorAtualPublic();

        // Assert
        ator.Should().NotBeNull();
        ator.UsuarioId.Should().Be(usuarioId);
        ator.ClienteId.Should().Be(clienteId);
        ator.Roles.Should().Contain(RoleEnum.Administrador);
    }

    #endregion

    #region Helper Methods

    private string CriarTokenJwtValido(Guid usuarioId, Guid? clienteId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim("userId", usuarioId.ToString()),
            new Claim("role", role)
        };

        if (clienteId.HasValue)
            claims.Add(new Claim("clienteId", clienteId.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-jwt-key-for-integration-tests-minimum-256-bits"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TestOficinaMecanicaApi",
            audience: "TestAuthorizedServices",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion

    #region Test Controller

    /// <summary>
    /// Controller concreto para testar BaseController
    /// </summary>
    private class TestController : global::API.Endpoints.BaseController
    {
        public TestController(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public Ator BuscarAtorAtualPublic() => BuscarAtorAtual();
    }

    #endregion
}
