using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;
using Moq;

namespace Tests.Infrastructure.Messaging.Filters.Helpers;

/// <summary>
/// Fixture para testes dos filtros de CorrelationId do MassTransit.
/// Encapsula criação de mocks e SUT para ConsumeCorrelationIdFilter, PublishCorrelationIdFilter e SendCorrelationIdFilter.
/// </summary>
public class CorrelationIdFilterTestFixture<TMessage> where TMessage : class, new()
{
    public ConsumeCorrelationIdFilter<TMessage> ConsumeFilter { get; }
    public PublishCorrelationIdFilter<TMessage> PublishFilter { get; }
    public SendCorrelationIdFilter<TMessage> SendFilter { get; }

    public Mock<ConsumeContext<TMessage>> ConsumeContextMock { get; }
    public Mock<IPipe<ConsumeContext<TMessage>>> ConsumeNextPipeMock { get; }

    public Mock<PublishContext<TMessage>> PublishContextMock { get; }
    public Mock<IPipe<PublishContext<TMessage>>> PublishNextPipeMock { get; }

    public Mock<SendContext<TMessage>> SendContextMock { get; }
    public Mock<IPipe<SendContext<TMessage>>> SendNextPipeMock { get; }

    public Mock<ProbeContext> ProbeContextMock { get; }

    public CorrelationIdFilterTestFixture()
    {
        ConsumeFilter = new ConsumeCorrelationIdFilter<TMessage>();
        PublishFilter = new PublishCorrelationIdFilter<TMessage>();
        SendFilter = new SendCorrelationIdFilter<TMessage>();

        ConsumeContextMock = new Mock<ConsumeContext<TMessage>>();
        ConsumeNextPipeMock = new Mock<IPipe<ConsumeContext<TMessage>>>();

        PublishContextMock = new Mock<PublishContext<TMessage>>();
        PublishNextPipeMock = new Mock<IPipe<PublishContext<TMessage>>>();

        SendContextMock = new Mock<SendContext<TMessage>>();
        SendNextPipeMock = new Mock<IPipe<SendContext<TMessage>>>();

        ProbeContextMock = new Mock<ProbeContext>();
        ProbeContextMock.Setup(p => p.CreateScope(It.IsAny<string>())).Returns(new Mock<ProbeContext>().Object);

        ConfigurarConsumeContextPadrao();
        ConfigurarPublishContextPadrao();
        ConfigurarSendContextPadrao();
    }

    private void ConfigurarConsumeContextPadrao()
    {
        var headersMock = new Mock<Headers>();
        object? headerOut = null;
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out headerOut)).Returns(false);
        ConsumeContextMock.Setup(c => c.Headers).Returns(headersMock.Object);
        ConsumeContextMock.Setup(c => c.CorrelationId).Returns((Guid?)null);
        ConsumeContextMock.Setup(c => c.Message).Returns(new TMessage());
        ConsumeNextPipeMock.Setup(n => n.Send(It.IsAny<ConsumeContext<TMessage>>())).Returns(Task.CompletedTask);
    }

    private void ConfigurarPublishContextPadrao()
    {
        var headersMock = new Mock<SendHeaders>();
        PublishContextMock.Setup(c => c.Headers).Returns(headersMock.Object);
        PublishContextMock.SetupProperty(c => c.CorrelationId);
        PublishNextPipeMock.Setup(n => n.Send(It.IsAny<PublishContext<TMessage>>())).Returns(Task.CompletedTask);
    }

    private void ConfigurarSendContextPadrao()
    {
        var headersMock = new Mock<SendHeaders>();
        SendContextMock.Setup(c => c.Headers).Returns(headersMock.Object);
        SendContextMock.SetupProperty(c => c.CorrelationId);
        SendNextPipeMock.Setup(n => n.Send(It.IsAny<SendContext<TMessage>>())).Returns(Task.CompletedTask);
    }
}
