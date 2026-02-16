using Infrastructure.Monitoramento.Correlation;
using MassTransit;
using Moq;

namespace Tests.Infrastructure.Messaging.Filters.Helpers;

/// <summary>
/// Extensões DSL fluentes para verificação de mocks de contextos MassTransit em testes de filtros de CorrelationId.
/// Segue o padrão DeveTer... para asserções.
/// </summary>
public static class CorrelationIdFilterMockVerifyExtensions
{
    #region ProbeContext

    /// <summary>
    /// Verifica que Probe criou um filter scope com o nome "correlationId".
    /// </summary>
    public static void DeveTerCriadoFilterScope(this Mock<ProbeContext> mock)
    {
        mock.Verify(p => p.CreateScope("filters"), Times.Once,
            "Era esperado que CreateScope(\"filters\") fosse chamado exatamente uma vez (via CreateFilterScope).");
    }

    #endregion

    #region ConsumeContext Pipe

    /// <summary>
    /// Verifica que next.Send foi chamado exatamente uma vez com o ConsumeContext fornecido.
    /// </summary>
    public static void DeveTerEnviadoContexto<TMessage>(this Mock<IPipe<ConsumeContext<TMessage>>> mock, Mock<ConsumeContext<TMessage>> contextoEsperado) where TMessage : class
    {
        mock.Verify(n => n.Send(contextoEsperado.Object), Times.Once,
            "Era esperado que next.Send fosse chamado exatamente uma vez com o contexto fornecido.");
    }

    #endregion

    #region PublishContext Pipe

    /// <summary>
    /// Verifica que next.Send foi chamado exatamente uma vez com o PublishContext fornecido.
    /// </summary>
    public static void DeveTerEnviadoContexto<TMessage>(this Mock<IPipe<PublishContext<TMessage>>> mock, Mock<PublishContext<TMessage>> contextoEsperado) where TMessage : class
    {
        mock.Verify(n => n.Send(contextoEsperado.Object), Times.Once,
            "Era esperado que next.Send fosse chamado exatamente uma vez com o contexto fornecido.");
    }

    #endregion

    #region SendContext Pipe

    /// <summary>
    /// Verifica que next.Send foi chamado exatamente uma vez com o SendContext fornecido.
    /// </summary>
    public static void DeveTerEnviadoContexto<TMessage>(this Mock<IPipe<SendContext<TMessage>>> mock, Mock<SendContext<TMessage>> contextoEsperado) where TMessage : class
    {
        mock.Verify(n => n.Send(contextoEsperado.Object), Times.Once,
            "Era esperado que next.Send fosse chamado exatamente uma vez com o contexto fornecido.");
    }

    #endregion

    #region PublishContext Header

    /// <summary>
    /// Verifica que o header X-Correlation-ID foi setado no PublishContext com o valor esperado.
    /// </summary>
    public static void DeveTerSetadoHeader<TMessage>(this Mock<PublishContext<TMessage>> mock, string valorEsperado) where TMessage : class
    {
        mock.Verify(c => c.Headers.Set(CorrelationConstants.HeaderName, valorEsperado), Times.Once,
            $"Era esperado que o header '{CorrelationConstants.HeaderName}' fosse setado com o valor '{valorEsperado}'.");
    }

    #endregion

    #region SendContext Header

    /// <summary>
    /// Verifica que o header X-Correlation-ID foi setado no SendContext com o valor esperado.
    /// </summary>
    public static void DeveTerSetadoHeader<TMessage>(this Mock<SendContext<TMessage>> mock, string valorEsperado) where TMessage : class
    {
        mock.Verify(c => c.Headers.Set(CorrelationConstants.HeaderName, valorEsperado), Times.Once,
            $"Era esperado que o header '{CorrelationConstants.HeaderName}' fosse setado com o valor '{valorEsperado}'.");
    }

    #endregion
}
