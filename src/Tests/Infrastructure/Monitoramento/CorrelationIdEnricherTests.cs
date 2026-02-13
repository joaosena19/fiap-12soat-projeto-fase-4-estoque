using FluentAssertions;
using Infrastructure.Monitoramento.Correlation;
using Moq;
using Serilog.Core;
using Serilog.Events;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationIdEnricherTests
{
    private readonly CorrelationIdEnricher _enricher;
    private readonly Mock<ILogEventPropertyFactory> _propertyFactoryMock;

    public CorrelationIdEnricherTests()
    {
        _enricher = new CorrelationIdEnricher();
        _propertyFactoryMock = new Mock<ILogEventPropertyFactory>();
    }

    [Fact]
    public void Enrich_ShouldAddCorrelationId_WhenContextHasValueAndPropertyDoesNotExist()
    {
        // Arrange
        var correlationId = "test-correlation-id";
        var logEvent = CreateLogEvent();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

            // Assert
            logEvent.Properties.Should().ContainKey(CorrelationConstants.LogPropertyName);
            var property = logEvent.Properties[CorrelationConstants.LogPropertyName];
            property.Should().BeOfType<ScalarValue>();
            ((ScalarValue)property).Value.Should().Be(correlationId);
        }
    }

    [Fact]
    public void Enrich_ShouldNotAddProperty_WhenContextIsEmpty()
    {
        // Arrange
        var logEvent = CreateLogEvent();

        // Act
        _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

        // Assert
        logEvent.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
    }

    [Fact]
    public void Enrich_ShouldNotAddProperty_WhenContextIsNull()
    {
        // Arrange
        var logEvent = CreateLogEvent();
        // CorrelationContext.Current is null by default

        // Act
        _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

        // Assert
        logEvent.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
    }

    [Fact]
    public void Enrich_ShouldNotDuplicateProperty_WhenPropertyAlreadyExists()
    {
        // Arrange
        var existingCorrelationId = "existing-id";
        var newCorrelationId = "new-id";
        
        var logEvent = CreateLogEvent();
        logEvent.AddPropertyIfAbsent(new LogEventProperty(
            CorrelationConstants.LogPropertyName,
            new ScalarValue(existingCorrelationId)));

        using (CorrelationContext.Push(newCorrelationId))
        {
            // Act
            _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

            // Assert
            logEvent.Properties.Should().ContainKey(CorrelationConstants.LogPropertyName);
            var property = logEvent.Properties[CorrelationConstants.LogPropertyName];
            ((ScalarValue)property).Value.Should().Be(existingCorrelationId);
        }
    }

    [Fact]
    public void Enrich_ShouldNotAddProperty_WhenCorrelationIdIsEmptyString()
    {
        // Arrange
        var logEvent = CreateLogEvent();

        using (CorrelationContext.Push(""))
        {
            // Act
            _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

            // Assert
            logEvent.Properties.Should().NotContainKey(CorrelationConstants.LogPropertyName);
        }
    }

    [Fact]
    public void Enrich_ShouldAddPropertyWithCorrectName()
    {
        // Arrange
        var correlationId = "test-id";
        var logEvent = CreateLogEvent();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

            // Assert
            logEvent.Properties.Should().ContainKey("CorrelationId");
        }
    }

    [Fact]
    public void Enrich_ShouldUseScalarValue_ForCorrelationIdProperty()
    {
        // Arrange
        var correlationId = "test-id-with-special-chars-!@#$";
        var logEvent = CreateLogEvent();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enricher.Enrich(logEvent, _propertyFactoryMock.Object);

            // Assert
            logEvent.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
        }
    }

    [Fact]
    public void Enrich_ShouldWorkWithMultipleLogEvents()
    {
        // Arrange
        var correlationId = "shared-correlation-id";
        var logEvent1 = CreateLogEvent();
        var logEvent2 = CreateLogEvent();

        using (CorrelationContext.Push(correlationId))
        {
            // Act
            _enricher.Enrich(logEvent1, _propertyFactoryMock.Object);
            _enricher.Enrich(logEvent2, _propertyFactoryMock.Object);

            // Assert
            logEvent1.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
            ((ScalarValue)logEvent1.Properties[CorrelationConstants.LogPropertyName]).Value.Should().Be(correlationId);
            
            logEvent2.Properties[CorrelationConstants.LogPropertyName].Should().BeOfType<ScalarValue>();
            ((ScalarValue)logEvent2.Properties[CorrelationConstants.LogPropertyName]).Value.Should().Be(correlationId);
        }
    }

    private static LogEvent CreateLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            MessageTemplate.Empty,
            Array.Empty<LogEventProperty>());
    }
}
