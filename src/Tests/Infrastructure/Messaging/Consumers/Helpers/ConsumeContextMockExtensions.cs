using Infrastructure.Messaging.DTOs;
using MassTransit;
using Moq;

namespace Tests.Infrastructure.Messaging.Consumers.Helpers;

/// <summary>
/// Builder para configurar o setup de publicação de mensagens no ConsumeContext mock.
/// Permite capturar mensagens publicadas para validação em testes.
/// </summary>
/// <typeparam name="TMessage">Tipo da mensagem a ser publicada</typeparam>
public class ConsumeContextPublishSetupBuilder<TMessage> where TMessage : class
{
    private readonly Mock<ConsumeContext> _mock;

    public ConsumeContextPublishSetupBuilder(Mock<ConsumeContext> mock)
    {
        _mock = mock;
    }

    /// <summary>
    /// Configura o mock para capturar a mensagem publicada através de um callback
    /// </summary>
    /// <param name="captura">Action que será executada com a mensagem capturada</param>
    public void CapturaMensagem(Action<TMessage?> captura)
    {
        _mock
            .Setup(x => x.Publish(
                It.IsAny<TMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((msg, _) =>
            {
                captura(msg as TMessage);
            })
            .Returns(Task.CompletedTask);
    }
}

/// <summary>
/// Extensões para configuração (Setup) de Mock de ConsumeContext em testes de consumidores MassTransit.
/// </summary>
public static class ConsumeContextMockSetupExtensions
{
    /// <summary>
    /// Retorna um builder para configurar o setup de publicação de mensagens
    /// </summary>
    /// <typeparam name="TMessage">Tipo da mensagem</typeparam>
    /// <param name="mock">Mock do ConsumeContext</param>
    /// <returns>Builder para configuração do setup</returns>
    public static ConsumeContextPublishSetupBuilder<TMessage> AoPublicar<TMessage>(this Mock<ConsumeContext> mock) where TMessage : class
    {
        return new ConsumeContextPublishSetupBuilder<TMessage>(mock);
    }
}

/// <summary>
/// Extensões para verificação de Mock de ConsumeContext em testes de consumidores MassTransit.
/// Fornece métodos auxiliares para validar publicação de mensagens ReducaoEstoqueResultado.
/// </summary>
public static class ConsumeContextMockExtensions
{
    /// <summary>
    /// Verifica se foi publicado um ReducaoEstoqueResultado indicando sucesso na operação.
    /// </summary>
    /// <param name="mock">Mock do ConsumeContext</param>
    /// <param name="correlationId">ID de correlação esperado</param>
    /// <param name="ordemServicoId">ID da ordem de serviço esperado</param>
    public static void DeveTerPublicadoReducaoEstoqueResultadoSucesso(this Mock<ConsumeContext<ReducaoEstoqueSolicitacao>> mock, string correlationId, Guid ordemServicoId)
    {
        mock.Verify(
            x => x.Publish(
                It.Is<ReducaoEstoqueResultado>(r =>
                    r.CorrelationId == correlationId &&
                    r.OrdemServicoId == ordemServicoId &&
                    r.Sucesso == true &&
                    r.MotivoFalha == null
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once,
            $"Era esperado que fosse publicado um ReducaoEstoqueResultado com sucesso exatamente uma vez " +
            $"(CorrelationId: {correlationId}, OrdemServicoId: {ordemServicoId}, Sucesso: true, MotivoFalha: null)."
        );
    }

    /// <summary>
    /// Verifica se foi publicado um ReducaoEstoqueResultado indicando falha na operação.
    /// </summary>
    /// <param name="mock">Mock do ConsumeContext</param>
    /// <param name="correlationId">ID de correlação esperado</param>
    /// <param name="ordemServicoId">ID da ordem de serviço esperado</param>
    /// <param name="motivoFalha">Motivo da falha esperado</param>
    public static void DeveTerPublicadoReducaoEstoqueResultadoFalha(this Mock<ConsumeContext<ReducaoEstoqueSolicitacao>> mock, string correlationId, Guid ordemServicoId, string motivoFalha)
    {
        mock.Verify(
            x => x.Publish(
                It.Is<ReducaoEstoqueResultado>(r =>
                    r.CorrelationId == correlationId &&
                    r.OrdemServicoId == ordemServicoId &&
                    r.Sucesso == false &&
                    r.MotivoFalha == motivoFalha
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once,
            $"Era esperado que fosse publicado um ReducaoEstoqueResultado com falha exatamente uma vez " +
            $"(CorrelationId: {correlationId}, OrdemServicoId: {ordemServicoId}, Sucesso: false, MotivoFalha: '{motivoFalha}')."
        );
    }

    // Sobrecarga para ConsumeContext sem tipo genérico (usado em testes do Publisher)
    
    /// <summary>
    /// Verifica se foi publicado um ReducaoEstoqueResultado indicando sucesso na operação.
    /// </summary>
    /// <param name="mock">Mock do ConsumeContext</param>
    /// <param name="correlationId">ID de correlação esperado</param>
    /// <param name="ordemServicoId">ID da ordem de serviço esperado</param>
    public static void DeveTerPublicadoReducaoEstoqueResultadoSucesso(this Mock<ConsumeContext> mock, string correlationId, Guid ordemServicoId)
    {
        mock.Verify(
            x => x.Publish(
                It.Is<ReducaoEstoqueResultado>(r =>
                    r.CorrelationId == correlationId &&
                    r.OrdemServicoId == ordemServicoId &&
                    r.Sucesso == true &&
                    r.MotivoFalha == null
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once,
            $"Era esperado que fosse publicado um ReducaoEstoqueResultado com sucesso exatamente uma vez " +
            $"(CorrelationId: {correlationId}, OrdemServicoId: {ordemServicoId}, Sucesso: true, MotivoFalha: null)."
        );
    }

    /// <summary>
    /// Verifica se foi publicado um ReducaoEstoqueResultado indicando falha na operação.
    /// </summary>
    /// <param name="mock">Mock do ConsumeContext</param>
    /// <param name="correlationId">ID de correlação esperado</param>
    /// <param name="ordemServicoId">ID da ordem de serviço esperado</param>
    /// <param name="motivoFalha">Motivo da falha esperado</param>
    public static void DeveTerPublicadoReducaoEstoqueResultadoFalha(this Mock<ConsumeContext> mock, string correlationId, Guid ordemServicoId, string motivoFalha)
    {
        mock.Verify(
            x => x.Publish(
                It.Is<ReducaoEstoqueResultado>(r =>
                    r.CorrelationId == correlationId &&
                    r.OrdemServicoId == ordemServicoId &&
                    r.Sucesso == false &&
                    r.MotivoFalha == motivoFalha
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once,
            $"Era esperado que fosse publicado um ReducaoEstoqueResultado com falha exatamente uma vez " +
            $"(CorrelationId: {correlationId}, OrdemServicoId: {ordemServicoId}, Sucesso: false, MotivoFalha: '{motivoFalha}')."
        );
    }
}
