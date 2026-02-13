using Application.Contracts.Monitoramento;
using FluentAssertions;
using Infrastructure.Messaging.DTOs;
using Infrastructure.Messaging.Publishers;
using MassTransit;
using Moq;
using Tests.Infrastructure.Messaging.Consumers.Helpers;

namespace Tests.Infrastructure.Messaging.Publishers;

/// <summary>
/// Testes unitários para ReducaoEstoqueResultadoPublisher.
/// Valida a publicação de mensagens de resultado seguindo padrões TDD e abstração.
/// </summary>
[Trait("Categoria", "Mensageria")]
public class ReducaoEstoqueResultadoPublisherTests
{
    [Fact(DisplayName = "PublicarSucessoAsync quando chamado deve publicar mensagem com Sucesso true")]
    [Trait("Cenario", "Sucesso")]
    public async Task PublicarSucessoAsync_QuandoChamado_DevePublicarMensagemComSucessoTrue()
    {
        // Arrange
        var contextMock = new Mock<ConsumeContext>();
        var loggerMock = new Mock<IAppLogger>();
        loggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>())).Returns(loggerMock.Object);

        var publisher = new ReducaoEstoqueResultadoPublisher();
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();

        // Act
        await publisher.PublicarSucessoAsync(loggerMock.Object, contextMock.Object, correlationId, ordemServicoId);

        // Assert
        contextMock.DeveTerPublicadoReducaoEstoqueResultadoSucesso(correlationId, ordemServicoId);
    }

    [Fact(DisplayName = "PublicarSucessoAsync quando chamado deve propagar CorrelationId e OrdemServicoId")]
    [Trait("Cenario", "Sucesso")]
    public async Task PublicarSucessoAsync_QuandoChamado_DevePropagarCorrelationIdEOrdemServicoId()
    {
        // Arrange
        var contextMock = new Mock<ConsumeContext>();
        var loggerMock = new Mock<IAppLogger>();
        loggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>())).Returns(loggerMock.Object);

        var publisher = new ReducaoEstoqueResultadoPublisher();
        var correlationId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var ordemServicoId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        ReducaoEstoqueResultado? capturedMessage = null;
        contextMock.AoPublicar<ReducaoEstoqueResultado>().CapturaMensagem(msg => capturedMessage = msg);

        // Act
        await publisher.PublicarSucessoAsync(loggerMock.Object, contextMock.Object, correlationId, ordemServicoId);

        // Assert
        capturedMessage.Should().NotBeNull("uma mensagem deveria ter sido publicada");
        capturedMessage!.CorrelationId.Should().Be(correlationId, "o CorrelationId deve ser propagado corretamente");
        capturedMessage.OrdemServicoId.Should().Be(ordemServicoId, "o OrdemServicoId deve ser propagado corretamente");
    }

    [Fact(DisplayName = "PublicarFalhaAsync quando chamado deve publicar mensagem com Sucesso false")]
    [Trait("Cenario", "Falha")]
    public async Task PublicarFalhaAsync_QuandoChamado_DevePublicarMensagemComSucessoFalse()
    {
        // Arrange
        var contextMock = new Mock<ConsumeContext>();
        var loggerMock = new Mock<IAppLogger>();
        loggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>())).Returns(loggerMock.Object);

        var publisher = new ReducaoEstoqueResultadoPublisher();
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        var motivoFalha = "estoque_insuficiente";

        // Act
        await publisher.PublicarFalhaAsync(loggerMock.Object, contextMock.Object, correlationId, ordemServicoId, motivoFalha);

        // Assert
        contextMock.DeveTerPublicadoReducaoEstoqueResultadoFalha(correlationId, ordemServicoId, motivoFalha);
    }

    [Fact(DisplayName = "PublicarFalhaAsync quando chamado deve incluir MotivoFalha")]
    [Trait("Cenario", "Falha")]
    public async Task PublicarFalhaAsync_QuandoChamado_DeveIncluirMotivoFalha()
    {
        // Arrange
        var contextMock = new Mock<ConsumeContext>();
        var loggerMock = new Mock<IAppLogger>();
        loggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>())).Returns(loggerMock.Object);

        var publisher = new ReducaoEstoqueResultadoPublisher();
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        var motivoFalha = "erro_interno";

        ReducaoEstoqueResultado? capturedMessage = null;
        contextMock.AoPublicar<ReducaoEstoqueResultado>().CapturaMensagem(msg => capturedMessage = msg);

        // Act
        await publisher.PublicarFalhaAsync(loggerMock.Object, contextMock.Object, correlationId, ordemServicoId, motivoFalha);

        // Assert
        capturedMessage.Should().NotBeNull("uma mensagem deveria ter sido publicada");
        capturedMessage!.MotivoFalha.Should().Be(motivoFalha, "o motivo da falha deve ser incluído na mensagem");
        capturedMessage.Sucesso.Should().BeFalse("a mensagem deve indicar falha");
    }

    [Fact(DisplayName = "PublicarFalhaAsync quando chamado deve propagar CorrelationId e OrdemServicoId")]
    [Trait("Cenario", "Falha")]
    public async Task PublicarFalhaAsync_QuandoChamado_DevePropagarCorrelationIdEOrdemServicoId()
    {
        // Arrange
        var contextMock = new Mock<ConsumeContext>();
        var loggerMock = new Mock<IAppLogger>();
        loggerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>())).Returns(loggerMock.Object);

        var publisher = new ReducaoEstoqueResultadoPublisher();
        var correlationId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var ordemServicoId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var motivoFalha = "servico_indisponivel";

        ReducaoEstoqueResultado? capturedMessage = null;
        contextMock.AoPublicar<ReducaoEstoqueResultado>().CapturaMensagem(msg => capturedMessage = msg);

        // Act
        await publisher.PublicarFalhaAsync(loggerMock.Object, contextMock.Object, correlationId, ordemServicoId, motivoFalha);

        // Assert
        capturedMessage.Should().NotBeNull("uma mensagem deveria ter sido publicada");
        capturedMessage!.CorrelationId.Should().Be(correlationId, "o CorrelationId deve ser propagado corretamente no cenário de falha");
        capturedMessage.OrdemServicoId.Should().Be(ordemServicoId, "o OrdemServicoId deve ser propagado corretamente no cenário de falha");
    }
}
