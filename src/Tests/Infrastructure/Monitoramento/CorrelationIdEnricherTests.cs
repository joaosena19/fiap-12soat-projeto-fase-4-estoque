using FluentAssertions;
using Infrastructure.Monitoramento.Correlation;
using Moq;
using Serilog.Core;
using Serilog.Events;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationIdEnricherTests
{
    private readonly CorrelationIdEnricher _enriquecedor;
    private readonly Mock<ILogEventPropertyFactory> _factoryPropriedadesMock;

    public CorrelationIdEnricherTests()
    {
        _enriquecedor = new CorrelationIdEnricher();
        _factoryPropriedadesMock = new Mock<ILogEventPropertyFactory>();
    }

    [Fact(DisplayName = "Enrich deve adicionar CorrelationId quando contexto tem valor e propriedade não existe")]
    public void Enrich_DeveAdicionarCorrelationId_QuandoContextoTemValorEPropriedadeNaoExiste()
    {
        // Arrange
        var correlationId = "test-correlation-id";
        var eventoLog = CriarEventoLog();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog.Properties.Should().ContainKey(CorrelationConstants.LogPropertyName);
            var propriedade = eventoLog.Properties[CorrelationConstants.LogPropertyName];
            propriedade.Should().BeOfType<ScalarValue>();
            ((ScalarValue)propriedade).Value.Should().Be(correlationId);
        }
    }

    [Fact(DisplayName = "Enrich não deve adicionar propriedade quando contexto está vazio")]
    public void Enrich_NaoDeveAdicionarPropriedade_QuandoContextoEstaVazio()
    {
        // Arrange
        var eventoLog = CriarEventoLog();

        // Act
        _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

        // Assert
        eventoLog.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
    }

    [Fact(DisplayName = "Enrich não deve adicionar propriedade quando contexto é null")]
    public void Enrich_NaoDeveAdicionarPropriedade_QuandoContextoEhNull()
    {
        // Arrange
        var eventoLog = CriarEventoLog();
        // CorrelationContext.Current é null por padrão

        // Act
        _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

        // Assert
        eventoLog.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
    }

    [Fact(DisplayName = "Enrich não deve duplicar propriedade quando propriedade já existe")]
    public void Enrich_NaoDeveDuplicarPropriedade_QuandoPropriedadeJaExiste()
    {
        // Arrange
        var correlationIdExistente = "existing-id";
        var novoCorrelationId = "new-id";
        
        var eventoLog = CriarEventoLog();
        eventoLog.AddPropertyIfAbsent(new LogEventProperty(
            CorrelationConstants.LogPropertyName,
            new ScalarValue(correlationIdExistente)));

        using (CorrelationContext.Push(novoCorrelationId))
        {
            // Act
            _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog.Properties.Should().ContainKey(CorrelationConstants.LogPropertyName);
            var propriedade = eventoLog.Properties[CorrelationConstants.LogPropertyName];
            ((ScalarValue)propriedade).Value.Should().Be(correlationIdExistente);
        }
    }

    [Fact(DisplayName = "Enrich não deve adicionar propriedade quando CorrelationId é string vazia")]
    public void Enrich_NaoDeveAdicionarPropriedade_QuandoCorrelationIdEhStringVazia()
    {
        // Arrange
        var eventoLog = CriarEventoLog();

        using (CorrelationContext.Push(""))
        {
            // Act
            _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
        }
    }

    [Fact(DisplayName = "Enrich deve adicionar propriedade com nome correto")]
    public void Enrich_DeveAdicionarPropriedadeComNomeCorreto()
    {
        // Arrange
        var correlationId = "test-id";
        var eventoLog = CriarEventoLog();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog.Properties.Should().ContainKey("CorrelationId");
        }
    }

    [Fact(DisplayName = "Enrich deve usar ScalarValue para propriedade CorrelationId")]
    public void Enrich_DeveUsarScalarValue_ParaPropriedadeCorrelationId()
    {
        // Arrange
        var correlationId = "test-id-with-special-chars-!@#$";
        var eventoLog = CriarEventoLog();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enriquecedor.Enrich(eventoLog, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
        }
    }

    [Fact(DisplayName = "Enrich deve funcionar com múltiplos eventos de log")]
    public void Enrich_DeveFuncionarComMultiplosEventosDeLog()
    {
        // Arrange
        var correlationIdCompartilhado = "shared-correlation-id";
        var eventoLog1 = CriarEventoLog();
        var eventoLog2 = CriarEventoLog();

        using (CorrelationContext.Push(correlationIdCompartilhado))
        {
            // Act
            _enriquecedor.Enrich(eventoLog1, _factoryPropriedadesMock.Object);
            _enriquecedor.Enrich(eventoLog2, _factoryPropriedadesMock.Object);

            // Assert
            eventoLog1.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
            ((ScalarValue)eventoLog1.Properties[CorrelationConstants.LogPropertyName]).Value.Should().Be(correlationIdCompartilhado);
            
            eventoLog2.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
            ((ScalarValue)eventoLog2.Properties[CorrelationConstants.LogPropertyName]).Value.Should().Be(correlationIdCompartilhado);
        }
    }

    private static LogEvent CriarEventoLog()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            MessageTemplate.Empty,
            Array.Empty<LogEventProperty>());
    }
}
