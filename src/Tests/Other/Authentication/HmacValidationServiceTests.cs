using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Other.Authentication;

public class HmacValidationServiceTests
{
    private static string ComputeSignature(string secret, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static HmacValidationService CreateServiceWithSecret(string secret)
    {
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Webhook:HmacSecret"])!.Returns(secret);
        return new HmacValidationService(configurationMock.Object);
    }

    [Fact(DisplayName = "Deve retornar true quando assinatura HMAC corresponde (sem prefixo)")]
    [Trait("Método", "ValidateSignature")]
    public void ValidateSignature_Deve_RetornarTrue_Quando_AssinaturaCorresponde_SemPrefixo()
    {
        // Arrange
        const string secret = "super-secret";
        const string payload = "{\"id\":123,\"status\":\"ok\"}";
        var service = CreateServiceWithSecret(secret);
        var signature = ComputeSignature(secret, payload);

        // Act
        var isValid = service.ValidateSignature(payload, signature);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar true quando assinatura HMAC corresponde (com prefixo sha256=)")]
    [Trait("Método", "ValidateSignature")]
    public void ValidateSignature_Deve_RetornarTrue_Quando_AssinaturaCorresponde_ComPrefixo()
    {
        // Arrange
        const string secret = "super-secret";
        const string payload = "body";
        var service = CreateServiceWithSecret(secret);
        var signature = "sha256=" + ComputeSignature(secret, payload);

        // Act
        var isValid = service.ValidateSignature(payload, signature);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar false quando assinatura HMAC não corresponde")]
    [Trait("Método", "ValidateSignature")]
    public void ValidateSignature_Deve_RetornarFalse_Quando_AssinaturaNaoCorresponde()
    {
        // Arrange
        const string secret = "super-secret";
        const string payload = "some-body";
        var service = CreateServiceWithSecret(secret);
        var wrongSignature = ComputeSignature("outro-segredo", payload);

        // Act
        var isValid = service.ValidateSignature(payload, wrongSignature);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory(DisplayName = "Deve retornar false quando payload ou assinatura são nulos/vazios")]
    [Trait("Método", "ValidateSignature")]
    [InlineData(null, null)]
    [InlineData("", "")] 
    [InlineData(null, "abc")] 
    [InlineData("{ }", null)] 
    [InlineData("{ }", "")] 
    [InlineData("", "abc")] 
    public void ValidateSignature_Deve_RetornarFalse_Quando_PayloadOuAssinaturaInvalidos(string? payload, string? signature)
    {
        // Arrange
        var service = CreateServiceWithSecret("secret");

        // Act
        var isValid = service.ValidateSignature(payload!, signature!);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve lançar InvalidOperationException quando segredo não está configurado")]
    public void Construtor_Deve_Lancar_Quando_SegredoNaoConfigurado()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Webhook:HmacSecret"])!.Returns((string?)null);

        // Act
        var act = () => new HmacValidationService(configurationMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Webhook HMAC Secret não configurado");
    }
}
