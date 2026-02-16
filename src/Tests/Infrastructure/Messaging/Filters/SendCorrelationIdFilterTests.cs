using FluentAssertions;
using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;
using Moq;
using Tests.Infrastructure.Messaging.Filters.Helpers;

namespace Tests.Infrastructure.Messaging.Filters;

/// <summary>
/// Testes unitários para SendCorrelationIdFilter.
/// Valida a propagação do correlation ID em mensagens enviadas via MassTransit.
/// </summary>
public class SendCorrelationIdFilterTests
{
    private readonly CorrelationIdFilterTestFixture<MensagemTeste> _fixture;

    public SendCorrelationIdFilterTests()
    {
        _fixture = new CorrelationIdFilterTestFixture<MensagemTeste>();
    }

    #region Probe

    [Fact(DisplayName = "Probe deve criar filter scope com nome correlationId")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public void Probe_DeveCriarFilterScope_ComNomeCorrelationId()
    {
        // Act
        _fixture.SendFilter.Probe(_fixture.ProbeContextMock.Object);

        // Assert
        _fixture.ProbeContextMock.DeveTerCriadoFilterScope();
    }

    #endregion

    #region Propagação com CorrelationContext definido

    [Fact(DisplayName = "Send deve propagar correlation ID do contexto atual para o header")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DevePropagarCorrelationId_DoContextoAtualParaHeader()
    {
        // Arrange
        var correlationId = "send-correlation-id-123";

        // Act
        using (CorrelationContext.Push(correlationId))
            await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        _fixture.SendContextMock.DeveTerSetadoHeader(correlationId);
    }

    [Fact(DisplayName = "Send deve setar CorrelationId no envelope quando valor é GUID válido")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveSetarCorrelationIdNoEnvelope_QuandoValorEhGuidValido()
    {
        // Arrange
        var guidValido = Guid.NewGuid();

        // Act
        using (CorrelationContext.Push(guidValido.ToString()))
            await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        _fixture.SendContextMock.Object.CorrelationId.Should().Be(guidValido);
    }

    [Fact(DisplayName = "Send não deve setar CorrelationId no envelope quando valor não é GUID")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_NaoDeveSetarCorrelationIdNoEnvelope_QuandoValorNaoEhGuid()
    {
        // Arrange
        var correlationIdNaoGuid = "meu-correlation-id-customizado";
        _fixture.SendContextMock.Object.CorrelationId = null;

        // Act
        using (CorrelationContext.Push(correlationIdNaoGuid))
            await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        _fixture.SendContextMock.Object.CorrelationId.Should().BeNull();
    }

    #endregion

    #region Fallback quando CorrelationContext não definido

    [Fact(DisplayName = "Send deve gerar novo GUID quando CorrelationContext é null")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveGerarNovoGuid_QuandoCorrelationContextEhNull()
    {
        // Arrange
        string? headerCapturado = null;
        _fixture.SendContextMock.AoSetarHeader().CapturaValor(valor => headerCapturado = valor);

        // Act
        await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        headerCapturado.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(headerCapturado, out _).Should().BeTrue();
    }

    [Fact(DisplayName = "Send deve setar envelope com GUID gerado quando CorrelationContext é null")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveSetarEnvelopeComGuidGerado_QuandoCorrelationContextEhNull()
    {
        // Act
        await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        _fixture.SendContextMock.Object.CorrelationId.Should().NotBeNull();
        _fixture.SendContextMock.Object.CorrelationId.Should().NotBe(Guid.Empty);
    }

    #endregion

    #region Chamada ao próximo pipeline

    [Fact(DisplayName = "Send deve chamar next.Send exatamente uma vez")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveChamarNextSend_ExatamenteUmaVez()
    {
        // Act
        await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        _fixture.SendNextPipeMock.DeveTerEnviadoContexto(_fixture.SendContextMock);
    }

    [Fact(DisplayName = "Send deve chamar next.Send com o mesmo contexto")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveChamarNextSend_ComMesmoContexto()
    {
        // Arrange
        SendContext<MensagemTeste>? contextoCapturado = null;
        _fixture.SendNextPipeMock.AoEnviar().CapturaCorrelationId(_ => contextoCapturado = _fixture.SendContextMock.Object);

        // Act
        await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        contextoCapturado.Should().BeSameAs(_fixture.SendContextMock.Object);
    }

    #endregion

    #region Consistência header e envelope

    [Fact(DisplayName = "Send deve setar header e envelope com mesmo valor quando GUID válido")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "SendCorrelationIdFilter")]
    public async Task Send_DeveSetarHeaderEEnvelope_ComMesmoValor_QuandoGuidValido()
    {
        // Arrange
        var guidValido = Guid.NewGuid();
        string? headerCapturado = null;
        _fixture.SendContextMock.AoSetarHeader().CapturaValor(valor => headerCapturado = valor);

        // Act
        using (CorrelationContext.Push(guidValido.ToString()))
            await _fixture.SendFilter.Send(_fixture.SendContextMock.Object, _fixture.SendNextPipeMock.Object);

        // Assert
        headerCapturado.Should().Be(guidValido.ToString());
        _fixture.SendContextMock.Object.CorrelationId.Should().Be(guidValido);
    }

    #endregion

    #region Tipos de Teste

    public class MensagemTeste { }

    #endregion
}
