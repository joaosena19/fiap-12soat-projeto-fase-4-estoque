using FluentAssertions;
using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;
using Moq;
using Tests.Infrastructure.Messaging.Filters.Helpers;

namespace Tests.Infrastructure.Messaging.Filters;

/// <summary>
/// Testes unitários para ConsumeCorrelationIdFilter.
/// Valida a extração e propagação do correlation ID durante o consumo de mensagens MassTransit.
/// </summary>
public class ConsumeCorrelationIdFilterTests
{
    private readonly CorrelationIdFilterTestFixture<MensagemTeste> _fixture;

    public ConsumeCorrelationIdFilterTests()
    {
        _fixture = new CorrelationIdFilterTestFixture<MensagemTeste>();
    }

    #region Probe

    [Fact(DisplayName = "Probe deve criar filter scope com nome correlationId")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public void Probe_DeveCriarFilterScope_ComNomeCorrelationId()
    {
        // Act
        _fixture.ConsumeFilter.Probe(_fixture.ProbeContextMock.Object);

        // Assert
        _fixture.ProbeContextMock.DeveTerCriadoFilterScope();
    }

    #endregion

    #region Extração via Header

    [Fact(DisplayName = "Send deve usar correlation ID do header quando presente")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDoHeader_QuandoPresente()
    {
        // Arrange
        var correlationIdEsperado = "header-correlation-id-123";
        _fixture.ConsumeContextMock.AoObterHeader(correlationIdEsperado);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(correlationIdEsperado);
    }

    [Fact(DisplayName = "Send deve ignorar header quando valor é whitespace")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveIgnorarHeader_QuandoValorEhWhitespace()
    {
        // Arrange
        var envelopeGuid = Guid.NewGuid();
        _fixture.ConsumeContextMock.AoObterHeaderInvalido("   ");
        _fixture.ConsumeContextMock.AoObterCorrelationIdDoEnvelope(envelopeGuid);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(envelopeGuid.ToString());
    }

    [Fact(DisplayName = "Send deve ignorar header quando valor é string vazia")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveIgnorarHeader_QuandoValorEhStringVazia()
    {
        // Arrange
        var envelopeGuid = Guid.NewGuid();
        _fixture.ConsumeContextMock.AoObterHeaderInvalido("");
        _fixture.ConsumeContextMock.AoObterCorrelationIdDoEnvelope(envelopeGuid);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(envelopeGuid.ToString());
    }

    #endregion

    #region Extração via Envelope MassTransit

    [Fact(DisplayName = "Send deve usar CorrelationId do envelope quando header não presente")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDoEnvelope_QuandoHeaderNaoPresente()
    {
        // Arrange
        var envelopeGuid = Guid.NewGuid();
        _fixture.ConsumeContextMock.AoObterCorrelationIdDoEnvelope(envelopeGuid);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(envelopeGuid.ToString());
    }

    #endregion

    #region Extração via Reflexão na Mensagem

    [Fact(DisplayName = "Send deve usar CorrelationId da mensagem via reflexão quando header e envelope ausentes")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDaMensagem_QuandoHeaderEEnvelopeAusentes()
    {
        // Arrange
        var correlationIdMensagem = "mensagem-correlation-id-456";
        var mensagem = new MensagemComCorrelationId { CorrelationId = correlationIdMensagem };
        var contextMock = CorrelationIdFilterMockSetupExtensions.CriarConsumeContextSemHeaderNemEnvelope(mensagem);

        var pipeMock = new Mock<IPipe<ConsumeContext<MensagemComCorrelationId>>>();
        string? correlationIdCapturado = null;
        pipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        var filtro = new ConsumeCorrelationIdFilter<MensagemComCorrelationId>();
        await filtro.Send(contextMock.Object, pipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(correlationIdMensagem);
    }

    [Fact(DisplayName = "Send deve usar CorrelationId Guid da mensagem via reflexão")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdGuidDaMensagem_ViaReflexao()
    {
        // Arrange
        var guidMensagem = Guid.NewGuid();
        var mensagem = new MensagemComCorrelationIdGuid { CorrelationId = guidMensagem };
        var contextMock = CorrelationIdFilterMockSetupExtensions.CriarConsumeContextSemHeaderNemEnvelope(mensagem);

        var pipeMock = new Mock<IPipe<ConsumeContext<MensagemComCorrelationIdGuid>>>();
        string? correlationIdCapturado = null;
        pipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        var filtro = new ConsumeCorrelationIdFilter<MensagemComCorrelationIdGuid>();
        await filtro.Send(contextMock.Object, pipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(guidMensagem.ToString());
    }

    [Fact(DisplayName = "Send deve gerar novo GUID quando propriedade CorrelationId da mensagem é null")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveGerarNovoGuid_QuandoPropriedadeCorrelationIdDaMensagemEhNull()
    {
        // Arrange
        var mensagem = new MensagemComCorrelationId { CorrelationId = null! };
        var contextMock = CorrelationIdFilterMockSetupExtensions.CriarConsumeContextSemHeaderNemEnvelope(mensagem);

        var pipeMock = new Mock<IPipe<ConsumeContext<MensagemComCorrelationId>>>();
        string? correlationIdCapturado = null;
        pipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        var filtro = new ConsumeCorrelationIdFilter<MensagemComCorrelationId>();
        await filtro.Send(contextMock.Object, pipeMock.Object);

        // Assert
        correlationIdCapturado.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdCapturado, out _).Should().BeTrue();
    }

    #endregion

    #region Fallback - Gerar novo GUID

    [Fact(DisplayName = "Send deve gerar novo GUID quando nenhuma fonte de correlation ID disponível")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveGerarNovoGuid_QuandoNenhumaFonteDisponivel()
    {
        // Arrange
        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdCapturado, out _).Should().BeTrue();
    }

    #endregion

    #region Propagação de Contexto

    [Fact(DisplayName = "Send deve definir CorrelationContext durante execução do pipeline")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveDefinirCorrelationContext_DuranteExecucaoDoPipeline()
    {
        // Arrange
        var correlationIdEsperado = "context-propagation-test";
        _fixture.ConsumeContextMock.AoObterHeader(correlationIdEsperado);

        string? correlationContextDentro = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationContextDentro = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationContextDentro.Should().Be(correlationIdEsperado);
        CorrelationContext.Current.Should().BeNull("o contexto deve ser restaurado após o escopo");
    }

    [Fact(DisplayName = "Send deve chamar next.Send exatamente uma vez")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveChamarNextSend_ExatamenteUmaVez()
    {
        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        _fixture.ConsumeNextPipeMock.DeveTerEnviadoContexto(_fixture.ConsumeContextMock);
    }

    [Fact(DisplayName = "Send deve restaurar CorrelationContext após execução")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveRestaurarCorrelationContext_AposExecucao()
    {
        // Arrange
        var correlationIdPreExistente = "pre-existing-id";
        _fixture.ConsumeContextMock.AoObterHeader("novo-id");

        // Act
        using (CorrelationContext.Push(correlationIdPreExistente))
        {
            await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

            // Assert
            CorrelationContext.Current.Should().Be(correlationIdPreExistente);
        }
    }

    #endregion

    #region Precedência

    [Fact(DisplayName = "Send deve priorizar header sobre envelope e mensagem")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DevePriorizarHeader_SobreEnvelopeEMensagem()
    {
        // Arrange
        var headerCorrelationId = "header-id";
        var envelopeGuid = Guid.NewGuid();
        _fixture.ConsumeContextMock.AoObterHeader(headerCorrelationId);
        _fixture.ConsumeContextMock.AoObterCorrelationIdDoEnvelope(envelopeGuid);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(headerCorrelationId);
    }

    [Fact(DisplayName = "Send deve priorizar envelope sobre mensagem")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Filter", "ConsumeCorrelationIdFilter")]
    public async Task Send_DevePriorizarEnvelope_SobreMensagem()
    {
        // Arrange
        var envelopeGuid = Guid.NewGuid();
        _fixture.ConsumeContextMock.AoObterCorrelationIdDoEnvelope(envelopeGuid);

        string? correlationIdCapturado = null;
        _fixture.ConsumeNextPipeMock.AoEnviar().CapturaCorrelationId(valor => correlationIdCapturado = valor);

        // Act
        await _fixture.ConsumeFilter.Send(_fixture.ConsumeContextMock.Object, _fixture.ConsumeNextPipeMock.Object);

        // Assert
        correlationIdCapturado.Should().Be(envelopeGuid.ToString());
    }

    #endregion

    #region Tipos de Teste

    public class MensagemTeste { }

    public class MensagemComCorrelationId
    {
        public string? CorrelationId { get; set; }
    }

    public class MensagemComCorrelationIdGuid
    {
        public Guid CorrelationId { get; set; }
    }

    #endregion
}
