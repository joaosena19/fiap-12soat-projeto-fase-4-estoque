using Infrastructure.Monitoramento.Correlation;
using MassTransit;
using Moq;

namespace Tests.Infrastructure.Messaging.Filters.Helpers;

/// <summary>
/// Extensões DSL fluentes para setup de mocks de contextos MassTransit em testes de filtros de CorrelationId.
/// Segue o padrão Ao... para configuração de comportamento.
/// </summary>
public static class CorrelationIdFilterMockSetupExtensions
{
    #region ConsumeContext Setup

    /// <summary>
    /// Configura o header X-Correlation-ID no ConsumeContext mock com o valor especificado.
    /// </summary>
    public static void AoObterHeader<TMessage>(this Mock<ConsumeContext<TMessage>> mock, string valor) where TMessage : class
    {
        var headersMock = new Mock<Headers>();
        object? headerOut = valor;
        headersMock.Setup(h => h.TryGetHeader(CorrelationConstants.HeaderName, out headerOut)).Returns(true);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);
    }

    /// <summary>
    /// Configura o header X-Correlation-ID no ConsumeContext mock com valor vazio/whitespace (simula header inválido).
    /// </summary>
    public static void AoObterHeaderInvalido<TMessage>(this Mock<ConsumeContext<TMessage>> mock, string valorInvalido) where TMessage : class
    {
        var headersMock = new Mock<Headers>();
        object? headerOut = valorInvalido;
        headersMock.Setup(h => h.TryGetHeader(CorrelationConstants.HeaderName, out headerOut)).Returns(true);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);
    }

    /// <summary>
    /// Configura o CorrelationId do envelope MassTransit no ConsumeContext mock.
    /// </summary>
    public static void AoObterCorrelationIdDoEnvelope<TMessage>(this Mock<ConsumeContext<TMessage>> mock, Guid envelopeGuid) where TMessage : class
    {
        mock.Setup(c => c.CorrelationId).Returns(envelopeGuid);
    }

    /// <summary>
    /// Configura a mensagem no ConsumeContext mock.
    /// </summary>
    public static void AoObterMensagem<TMessage>(this Mock<ConsumeContext<TMessage>> mock, TMessage mensagem) where TMessage : class
    {
        mock.Setup(c => c.Message).Returns(mensagem);
    }

    /// <summary>
    /// Cria um ConsumeContext mock sem header nem envelope, apenas com a mensagem fornecida.
    /// </summary>
    public static Mock<ConsumeContext<TMessage>> CriarConsumeContextSemHeaderNemEnvelope<TMessage>(TMessage mensagem) where TMessage : class
    {
        var mock = new Mock<ConsumeContext<TMessage>>();
        var headersMock = new Mock<Headers>();
        object? headerOut = null;
        headersMock.Setup(h => h.TryGetHeader(It.IsAny<string>(), out headerOut)).Returns(false);
        mock.Setup(c => c.Headers).Returns(headersMock.Object);
        mock.Setup(c => c.CorrelationId).Returns((Guid?)null);
        mock.Setup(c => c.Message).Returns(mensagem);
        return mock;
    }

    #endregion

    #region Pipe Callback Setup

    /// <summary>
    /// Configura um callback no next pipe para capturar o CorrelationContext.Current durante a execução.
    /// </summary>
    public static CorrelationIdCapturaBuilder<TContext> AoEnviar<TContext>(this Mock<IPipe<TContext>> mock) where TContext : class, PipeContext
    {
        return new CorrelationIdCapturaBuilder<TContext>(mock);
    }

    #endregion

    #region PublishContext/SendContext Header Callback Setup

    /// <summary>
    /// Configura captura do header Set no PublishContext mock.
    /// </summary>
    public static HeaderCapturaBuilder<TMessage> AoSetarHeader<TMessage>(this Mock<PublishContext<TMessage>> mock) where TMessage : class
    {
        return new HeaderCapturaBuilder<TMessage>(mock);
    }

    /// <summary>
    /// Configura captura do header Set no SendContext mock.
    /// </summary>
    public static HeaderCapturaBuilder<TMessage> AoSetarHeader<TMessage>(this Mock<SendContext<TMessage>> mock) where TMessage : class
    {
        return new HeaderCapturaBuilder<TMessage>(mock);
    }

    #endregion
}

/// <summary>
/// Builder para captura do CorrelationContext.Current durante execução do pipe.
/// </summary>
public class CorrelationIdCapturaBuilder<TContext> where TContext : class, PipeContext
{
    private readonly Mock<IPipe<TContext>> _mock;

    public CorrelationIdCapturaBuilder(Mock<IPipe<TContext>> mock)
    {
        _mock = mock;
    }

    /// <summary>
    /// Captura o valor de CorrelationContext.Current no momento da execução do pipe.
    /// </summary>
    public void CapturaCorrelationId(Action<string?> captura)
    {
        _mock.Setup(n => n.Send(It.IsAny<TContext>()))
             .Callback<TContext>(_ => captura(CorrelationContext.Current))
             .Returns(Task.CompletedTask);
    }
}

/// <summary>
/// Builder genérico para captura de headers Set em PublishContext ou SendContext.
/// </summary>
public class HeaderCapturaBuilder<TMessage> where TMessage : class
{
    private readonly Mock _mock;

    public HeaderCapturaBuilder(Mock mock)
    {
        _mock = mock;
    }

    /// <summary>
    /// Captura o valor do header X-Correlation-ID quando Set for chamado.
    /// </summary>
    public void CapturaValor(Action<string?> captura)
    {
        if (_mock is Mock<PublishContext<TMessage>> publishMock)
            publishMock.Setup(c => c.Headers.Set(CorrelationConstants.HeaderName, It.IsAny<string>()))
                       .Callback<string, object>((_, valor) => captura(valor.ToString()));
        else if (_mock is Mock<SendContext<TMessage>> sendMock)
            sendMock.Setup(c => c.Headers.Set(CorrelationConstants.HeaderName, It.IsAny<string>()))
                    .Callback<string, object>((_, valor) => captura(valor.ToString()));
    }
}
