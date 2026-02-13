using Application.Contracts.Monitoramento;
using Application.Extensions;
using Application.Extensions.Enums;
using Infrastructure.Database;
using Infrastructure.Messaging.DTOs;
using Infrastructure.Messaging.Publishers;
using Infrastructure.Repositories.Estoque;
using MassTransit;
using Microsoft.Extensions.Logging;
using SerilogContext = Serilog.Context.LogContext;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer que processa solicitações de redução de estoque enviadas pelo serviço de Ordem de Serviço.
/// Implementa o padrão Saga coreografado conforme especificado no Plano de Execução E-03.
/// Segue o padrão de Pure DI do sistema, criando dependências manualmente.
/// </summary>
public class ReducaoEstoqueSolicitacaoConsumer : BaseConsumer, IConsumer<ReducaoEstoqueSolicitacao>
{
    public ReducaoEstoqueSolicitacaoConsumer(AppDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
    {
    }

    public async Task Consume(ConsumeContext<ReducaoEstoqueSolicitacao> context)
    {
        // Pure DI: criar dependências manualmente
        var gateway = new ItemEstoqueRepository(_context);
        var publisher = new ReducaoEstoqueResultadoPublisher();
        var logger = CriarLoggerPara<ReducaoEstoqueSolicitacaoConsumer>();

        var msg = context.Message;

        // O filter já estabeleceu o scope de CorrelationId automaticamente
        await ProcessarMensagemAsync(gateway, publisher, logger, context, msg);
    }

    private async Task ProcessarMensagemAsync(ItemEstoqueRepository gateway, IReducaoEstoqueResultadoPublisher publisher, IAppLogger logger, ConsumeContext<ReducaoEstoqueSolicitacao> context, ReducaoEstoqueSolicitacao msg)
    {
        logger
            .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
            .LogInformation("Recebida solicitação de redução de estoque para OS {OrdemServicoId}", msg.OrdemServicoId);

        try
        {
            // Validar disponibilidade de todos os itens antes de reduzir qualquer um
            foreach (var item in msg.Itens)
            {
                var estoque = await gateway.ObterPorIdAsync(item.ItemEstoqueId);

                if (estoque == null)
                {
                    logger
                        .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
                        .LogWarning(
                            "Item de estoque {ItemId} não encontrado para OS {OrdemServicoId}",
                            item.ItemEstoqueId,
                            msg.OrdemServicoId);

                    await publisher.PublicarFalhaAsync(logger, context, msg.CorrelationId, msg.OrdemServicoId, "estoque_insuficiente");
                    return;
                }

                if (!estoque.VerificarDisponibilidade(item.Quantidade))
                {
                    logger
                        .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
                        .LogWarning(
                            "Estoque insuficiente para item {ItemId} (necessário: {QuantidadeNecessaria}, disponível: {QuantidadeDisponivel}) " +
                            "para OS {OrdemServicoId}",
                            item.ItemEstoqueId,
                            item.Quantidade,
                            estoque.Quantidade.Valor,
                            msg.OrdemServicoId);

                    await publisher.PublicarFalhaAsync(logger, context, msg.CorrelationId, msg.OrdemServicoId, "estoque_insuficiente");
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

                logger
                    .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
                    .LogInformation(
                        "Estoque do item {ItemId} reduzido em {Quantidade} unidades (novo total: {NovaQuantidade}) para OS {OrdemServicoId}",
                        item.ItemEstoqueId,
                        item.Quantidade,
                        novaQuantidade,
                        msg.OrdemServicoId);
            }

            await publisher.PublicarSucessoAsync(logger, context, msg.CorrelationId, msg.OrdemServicoId);

            logger
                .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
                .LogInformation(
                    "Redução de estoque concluída com sucesso para OS {OrdemServicoId}",
                    msg.OrdemServicoId);
        }
        catch (Exception ex)
        {
            logger
                .ComMensageria(NomeMensagemEnum.ReducaoEstoqueSolicitacao, TipoMensagemEnum.Consumo)
                .LogError(
                    ex,
                    "Erro ao processar redução de estoque para OS {OrdemServicoId}",
                    msg.OrdemServicoId);

            await publisher.PublicarFalhaAsync(logger, context, msg.CorrelationId, msg.OrdemServicoId, "erro_interno");
        }
    }
}
