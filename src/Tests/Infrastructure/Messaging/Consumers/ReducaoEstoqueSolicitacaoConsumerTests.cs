using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.DTOs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.Infrastructure.Messaging.Consumers;

/// <summary>
/// Testes unitários para o ReducaoEstoqueSolicitacaoConsumer.
/// Valida o processamento de mensagens de redução de estoque conforme especificado no Plano de Execução E-03.
/// Utiliza InMemory database para testar com o padrão Pure DI.
/// </summary>
public class ReducaoEstoqueSolicitacaoConsumerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Mock<ConsumeContext<ReducaoEstoqueSolicitacao>> _contextMock;
    private readonly ReducaoEstoqueSolicitacaoConsumer _consumer;

    public ReducaoEstoqueSolicitacaoConsumerTests()
    {
        // Setup InMemory database usando builder
        _context = AppDbContextInMemoryBuilder.Novo().Build();
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _contextMock = new Mock<ConsumeContext<ReducaoEstoqueSolicitacao>>();
        _consumer = new ReducaoEstoqueSolicitacaoConsumer(_context, _loggerFactory);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _loggerFactory.Dispose();
    }

    [Fact(DisplayName = "Consumir redução de estoque deve reduzir quantidade e publicar sucesso quando estoque suficiente")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_Deve_ReduzirQuantidadeEPublicarSucesso_Quando_EstoqueSuficiente()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoqueTestBuilder.Novo()
            .ComNome("Pneu")
            .ComQuantidade(10)
            .ComTipo(TipoItemEstoqueEnum.Peca)
            .ComPreco(250.00m)
            .Build();
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemEstoque.Id, 5)
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r =>
                r.CorrelationId == correlationId &&
                r.OrdemServicoId == ordemServicoId &&
                r.Sucesso == true &&
                r.MotivoFalha == null
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verifica que a quantidade foi reduzida no banco
        var itemAtualizado = await _context.ItensEstoque.FindAsync(itemEstoque.Id);
        Assert.NotNull(itemAtualizado);
        Assert.Equal(5, itemAtualizado.Quantidade.Valor);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve publicar falha com motivo quando estoque insuficiente")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_Deve_PublicarFalhaComMotivo_Quando_EstoqueInsuficiente()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoqueTestBuilder.Novo()
            .ComNome("Pneu")
            .ComQuantidade(3)
            .ComTipo(TipoItemEstoqueEnum.Peca)
            .ComPreco(250.00m)
            .Build();
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var quantidadeOriginal = itemEstoque.Quantidade.Valor;
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemEstoque.Id, 10)
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r =>
                r.CorrelationId == correlationId &&
                r.OrdemServicoId == ordemServicoId &&
                r.Sucesso == false &&
                r.MotivoFalha == "estoque_insuficiente"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verifica que a quantidade não foi alterada
        var itemAtualizado = await _context.ItensEstoque.FindAsync(itemEstoque.Id);
        Assert.NotNull(itemAtualizado);
        Assert.Equal(quantidadeOriginal, itemAtualizado.Quantidade.Valor);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve publicar falha com motivo quando item não encontrado")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_Deve_PublicarFalhaComMotivo_Quando_ItemNaoEncontrado()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        var itemIdInexistente = Guid.NewGuid();
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemIdInexistente, 5)
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r =>
                r.CorrelationId == correlationId &&
                r.OrdemServicoId == ordemServicoId &&
                r.Sucesso == false &&
                r.MotivoFalha == "estoque_insuficiente"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve processar todos itens com sucesso quando múltiplos itens")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_Deve_ProcessarTodosItensComSucesso_Quando_MultiplosItens()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque1 = ItemEstoqueTestBuilder.Novo()
            .ComNome("Pneu")
            .ComQuantidade(20)
            .ComTipo(TipoItemEstoqueEnum.Peca)
            .ComPreco(250.00m)
            .Build();
        var itemEstoque2 = ItemEstoqueTestBuilder.Novo()
            .ComNome("Óleo")
            .ComQuantidade(15)
            .ComTipo(TipoItemEstoqueEnum.Insumo)
            .ComPreco(45.00m)
            .Build();
        _context.ItensEstoque.Add(itemEstoque1);
        _context.ItensEstoque.Add(itemEstoque2);
        await _context.SaveChangesAsync();
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemEstoque1.Id, 4)
            .ComItem(itemEstoque2.Id, 2)
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r =>
                r.CorrelationId == correlationId &&
                r.OrdemServicoId == ordemServicoId &&
                r.Sucesso == true
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verifica que ambas as quantidades foram reduzidas
        var item1Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque1.Id);
        var item2Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque2.Id);
        Assert.NotNull(item1Atualizado);
        Assert.NotNull(item2Atualizado);
        Assert.Equal(16, item1Atualizado.Quantidade.Valor);
        Assert.Equal(13, item2Atualizado.Quantidade.Valor);
    }

    [Fact(DisplayName = "Consumir redução de estoque não deve reduzir quantidade quando múltiplos itens e um insuficiente")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_NaoDeve_ReduzirQuantidade_Quando_MultiplosItensEUmInsuficiente()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque1 = ItemEstoqueTestBuilder.Novo()
            .ComNome("Pneu")
            .ComQuantidade(20)
            .ComTipo(TipoItemEstoqueEnum.Peca)
            .ComPreco(250.00m)
            .Build();
        var itemEstoque2 = ItemEstoqueTestBuilder.Novo()
            .ComNome("Óleo")
            .ComQuantidade(1)
            .ComTipo(TipoItemEstoqueEnum.Insumo)
            .ComPreco(45.00m)
            .Build();
        _context.ItensEstoque.Add(itemEstoque1);
        _context.ItensEstoque.Add(itemEstoque2);
        await _context.SaveChangesAsync();
        
        var quantidadeOriginal1 = itemEstoque1.Quantidade.Valor;
        var quantidadeOriginal2 = itemEstoque2.Quantidade.Valor;
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemEstoque1.Id, 4)
            .ComItem(itemEstoque2.Id, 5) // Mais do que disponível
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert - nenhuma atualização deve ser feita
        var item1Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque1.Id);
        var item2Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque2.Id);
        Assert.NotNull(item1Atualizado);
        Assert.NotNull(item2Atualizado);
        Assert.Equal(quantidadeOriginal1, item1Atualizado.Quantidade.Valor);
        Assert.Equal(quantidadeOriginal2, item2Atualizado.Quantidade.Valor);
        
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r =>
                r.Sucesso == false &&
                r.MotivoFalha == "estoque_insuficiente"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve propagar CorrelationId no resultado")]
    [Trait("Category", "Messaging")]
    public async Task ConsumeReducaoEstoque_Deve_PropagarCorrelationIdNoResultado()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoqueTestBuilder.Novo()
            .ComNome("Pneu")
            .ComQuantidade(10)
            .ComTipo(TipoItemEstoqueEnum.Peca)
            .ComPreco(250.00m)
            .Build();
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var solicitacao = ReducaoEstoqueSolicitacaoBuilder.Novo()
            .ComCorrelationId(correlationId)
            .ComOrdemServicoId(ordemServicoId)
            .ComItem(itemEstoque.Id, 5)
            .Build();

        _contextMock.Setup(x => x.Message).Returns(solicitacao);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert - Verifica que o resultado publicado contém o CorrelationId correto
        _contextMock.Verify(x => x.Publish(
            It.Is<ReducaoEstoqueResultado>(r => r.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
