using Infrastructure.Database;
using Infrastructure.Messaging.Contracts;
using Infrastructure.Repositories.Estoque;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer que processa solicitações de redução de estoque enviadas pelo serviço de Ordem de Serviço.
/// Implementa o padrão Saga coreografado conforme especificado no Plano de Execução E-03.
/// Segue o padrão de Pure DI do sistema, criando dependências manualmente.
/// </summary>
public class ReducaoEstoqueSolicitacaoConsumer : BaseConsumer, IConsumer<ReducaoEstoqueSolicitacao>
{
    public ReducaoEstoqueSolicitacaoConsumer(
        AppDbContext context,
        ILoggerFactory loggerFactory)
        : base(context, loggerFactory)
    {
    }

    public async Task Consume(ConsumeContext<ReducaoEstoqueSolicitacao> context)
    {
        // Seguindo o padrão Pure DI: criar dependências manualmente
        var gateway = new ItemEstoqueRepository(_context);
        var logger = CriarLoggerPara<ReducaoEstoqueSolicitacaoConsumer>();
        
        var msg = context.Message;
        logger.LogInformation(
            "Recebida solicitação de redução de estoque para OS {OrdemServicoId}. CorrelationId: {CorrelationId}",
            msg.OrdemServicoId, 
            msg.CorrelationId);

        try
        {
            // Validar disponibilidade de todos os itens antes de reduzir qualquer um
            foreach (var item in msg.Itens)
            {
                var estoque = await gateway.ObterPorIdAsync(item.ItemEstoqueId);
                
                if (estoque == null)
                {
                    logger.LogWarning(
                        "Item de estoque {ItemId} não encontrado para OS {OrdemServicoId}. CorrelationId: {CorrelationId}",
                        item.ItemEstoqueId,
                        msg.OrdemServicoId,
                        msg.CorrelationId);

                    await context.Publish(new ReducaoEstoqueResultado
                    {
                        CorrelationId = msg.CorrelationId,
                        OrdemServicoId = msg.OrdemServicoId,
                        Sucesso = false,
                        MotivoFalha = "estoque_insuficiente"
                    });
                    return;
                }

                if (!estoque.VerificarDisponibilidade(item.Quantidade))
                {
                    logger.LogWarning(
                        "Estoque insuficiente para item {ItemId} (necessário: {QuantidadeNecessaria}, disponível: {QuantidadeDisponivel}) " +
                        "para OS {OrdemServicoId}. CorrelationId: {CorrelationId}",
                        item.ItemEstoqueId,
                        item.Quantidade,
                        estoque.Quantidade.Valor,
                        msg.OrdemServicoId,
                        msg.CorrelationId);

                    await context.Publish(new ReducaoEstoqueResultado
                    {
                        CorrelationId = msg.CorrelationId,
                        OrdemServicoId = msg.OrdemServicoId,
                        Sucesso = false,
                        MotivoFalha = "estoque_insuficiente"
                    });
                    return;
                }
            }

            // Todos os itens têm estoque disponível - proceder com a redução
            foreach (var item in msg.Itens)
            {
                var estoque = await gateway.ObterPorIdAsync(item.ItemEstoqueId);
                if (estoque == null) continue; // Já validado acima, mas protege contra race condition

                var novaQuantidade = estoque.Quantidade.Valor - item.Quantidade;
                estoque.AtualizarQuantidade(novaQuantidade);
                await gateway.AtualizarAsync(estoque);

                logger.LogInformation(
                    "Estoque do item {ItemId} reduzido em {Quantidade} unidades (novo total: {NovaQuantidade}). " +
                    "OS {OrdemServicoId}, CorrelationId: {CorrelationId}",
                    item.ItemEstoqueId,
                    item.Quantidade,
                    novaQuantidade,
                    msg.OrdemServicoId,
                    msg.CorrelationId);
            }

            // Publicar resultado de sucesso
            await context.Publish(new ReducaoEstoqueResultado
            {
                CorrelationId = msg.CorrelationId,
                OrdemServicoId = msg.OrdemServicoId,
                Sucesso = true
            });

            logger.LogInformation(
                "Redução de estoque concluída com sucesso para OS {OrdemServicoId}. CorrelationId: {CorrelationId}",
                msg.OrdemServicoId,
                msg.CorrelationId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Erro ao processar redução de estoque para OS {OrdemServicoId}. CorrelationId: {CorrelationId}",
                msg.OrdemServicoId,
                msg.CorrelationId);

            await context.Publish(new ReducaoEstoqueResultado
            {
                CorrelationId = msg.CorrelationId,
                OrdemServicoId = msg.OrdemServicoId,
                Sucesso = false,
                MotivoFalha = "erro_interno"
            });
        }
    }
}
