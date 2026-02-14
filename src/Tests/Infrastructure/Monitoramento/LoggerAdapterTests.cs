using FluentAssertions;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Infrastructure.SharedHelpers;
using Xunit;

namespace Tests.Infrastructure.Monitoramento;

public class LoggerAdapterTests
{
    private readonly Mock<ILogger<LoggerAdapterTests>> _loggerMock;
    private readonly LoggerAdapter<LoggerAdapterTests> _sut;

    public LoggerAdapterTests()
    {
        _loggerMock = new Mock<ILogger<LoggerAdapterTests>>();
        _sut = new LoggerAdapter<LoggerAdapterTests>(_loggerMock.Object);
    }

    [Fact(DisplayName = "Deve repassar chamada para ILogger quando LogInformation é chamado")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogInformation_DeveRepassarChamadaParaILogger_QuandoChamado()
    {
        // Arrange
        var messageTemplate = "Processando item {ItemId}";
        var args = new object[] { 123 };

        // Act
        _sut.LogInformation(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve repassar chamada para ILogger quando LogWarning é chamado")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogWarning_DeveRepassarChamadaParaILogger_QuandoChamado()
    {
        // Arrange
        var messageTemplate = "Atenção: recurso {Resource} está próximo do limite";
        var args = new object[] { "Memória" };

        // Act
        _sut.LogWarning(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve repassar chamada para ILogger quando LogError é chamado com mensagem")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_DeveRepassarChamadaParaILogger_QuandoChamado()
    {
        // Arrange
        var messageTemplate = "Erro ao processar pedido {PedidoId}";
        var args = new object[] { 456 };

        // Act
        _sut.LogError(messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve repassar chamada para ILogger quando LogError é chamado com exceção")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_ComException_DeveRepassarChamadaParaILogger_QuandoChamado()
    {
        // Arrange
        var exception = new InvalidOperationException("Operação inválida");
        var messageTemplate = "Falha ao executar operação {Operation}";
        var args = new object[] { "ProcessarPedido" };

        // Act
        _sut.LogError(exception, messageTemplate, args);

        // Assert
        _loggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve retornar ContextualLogger quando ComPropriedade é chamado")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void ComPropriedade_DeveRetornarContextualLogger_QuandoChamado()
    {
        // Arrange
        var key = "CorrelationId";
        var value = Guid.NewGuid();

        // Act
        var resultado = _sut.ComPropriedade(key, value);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<ContextualLogger>();
    }
}
