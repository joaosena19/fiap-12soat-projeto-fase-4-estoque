using API.Configurations;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Other.Authentication
{
    public class AuthenticationConfigurationTests
    {
        private readonly ServiceCollection _services;

        public AuthenticationConfigurationTests()
        {
            _services = new ServiceCollection();
        }

        #region Testes Método AddJwtAuthentication

        [Fact(DisplayName = "Deve lançar exceção quando JWT Key está ausente")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtKeyEstaAusente()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns((string?)null);
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando JWT Key está vazia")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtKeyEstaVazia()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns(string.Empty);
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando JWT Issuer está ausente")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtIssuerEstaAusente()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-256-bits-for-hmacsha256-algorithm");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns((string?)null);
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando JWT Issuer está vazio")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtIssuerEstaVazio()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-256-bits-for-hmacsha256-algorithm");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(string.Empty);
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando JWT Audience está ausente")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtAudienceEstaAusente()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-256-bits-for-hmacsha256-algorithm");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns((string?)null);

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando JWT Audience está vazio")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoJwtAudienceEstaVazio()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-256-bits-for-hmacsha256-algorithm");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns(string.Empty);

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando todos os parâmetros JWT estão ausentes")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoTodosParametrosJwtEstaoAusentes()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns((string?)null);
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns((string?)null);
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns((string?)null);

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve lançar exceção quando todos os parâmetros JWT estão vazios")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveLancarExcecao_QuandoTodosParametrosJwtEstaoVazios()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns(string.Empty);
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns(string.Empty);
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns(string.Empty);

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT*ausente*");
        }

        [Fact(DisplayName = "Deve configurar autenticação JWT com sucesso quando todos os parâmetros são válidos")]
        [Trait("Metodo", "AddJwtAuthentication")]
        public void AddJwtAuthentication_DeveConfigurarComSucesso_QuandoTodosParametrosSaoValidos()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key-with-minimum-256-bits-for-hmacsha256-algorithm-security");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var act = () => _services.AddJwtAuthentication(configurationMock.Object);

            // Assert
            act.Should().NotThrow();
            _services.Should().NotBeEmpty();
        }

        #endregion
    }
}
