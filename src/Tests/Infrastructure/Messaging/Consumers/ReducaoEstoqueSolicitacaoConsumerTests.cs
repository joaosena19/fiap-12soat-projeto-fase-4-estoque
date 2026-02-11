using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.DTOs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
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
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
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

    [Fact(DisplayName = "Deve reduzir estoque e publicar sucesso quando estoque suficiente")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenEstoqueSuficiente_ReduzQuantidadeEPublicaSucesso()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoque.Criar("Pneu", 10, TipoItemEstoqueEnum.Peca, 250.00m);
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemEstoque.Id, Quantidade = 5 }
            }
        };

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

    [Fact(DisplayName = "Deve publicar falha quando estoque insuficiente")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenEstoqueInsuficiente_PublicaFalhaComMotivo()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoque.Criar("Pneu", 3, TipoItemEstoqueEnum.Peca, 250.00m);
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var quantidadeOriginal = itemEstoque.Quantidade.Valor;
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemEstoque.Id, Quantidade = 10 }
            }
        };

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

    [Fact(DisplayName = "Deve publicar falha quando item não encontrado")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenItemNaoEncontrado_PublicaFalhaComMotivo()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        var itemIdInexistente = Guid.NewGuid();
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemIdInexistente, Quantidade = 5 }
            }
        };

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

    [Fact(DisplayName = "Deve processar múltiplos itens com sucesso")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenMultiplosItens_ProcessaTodosComSucesso()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque1 = ItemEstoque.Criar("Pneu", 20, TipoItemEstoqueEnum.Peca, 250.00m);
        var itemEstoque2 = ItemEstoque.Criar("Óleo", 15, TipoItemEstoqueEnum.Insumo, 45.00m);
        _context.ItensEstoque.Add(itemEstoque1);
        _context.ItensEstoque.Add(itemEstoque2);
        await _context.SaveChangesAsync();
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemEstoque1.Id, Quantidade = 4 },
                new ItemReducao { ItemEstoqueId = itemEstoque2.Id, Quantidade = 2 }
            }
        };

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

    [Fact(DisplayName = "Deve falhar se um dos múltiplos itens não tiver estoque suficiente")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenMultiplosItensEUmInsuficiente_NaoReducNenhum()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque1 = ItemEstoque.Criar("Pneu", 20, TipoItemEstoqueEnum.Peca, 250.00m);
        var itemEstoque2 = ItemEstoque.Criar("Óleo", 1, TipoItemEstoqueEnum.Insumo, 45.00m);
        _context.ItensEstoque.Add(itemEstoque1);
        _context.ItensEstoque.Add(itemEstoque2);
        await _context.SaveChangesAsync();
        
        var quantidadeOriginal1 = itemEstoque1.Quantidade.Valor;
        var quantidadeOriginal2 = itemEstoque2.Quantidade.Valor;
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemEstoque1.Id, Quantidade = 4 },
                new ItemReducao { ItemEstoqueId = itemEstoque2.Id, Quantidade = 5 } // Mais do que disponível
            }
        };

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

    [Fact(DisplayName = "Deve propagar CorrelationId no processamento")]
    [Trait("Category", "Messaging")]
    public async Task Consume_WhenProcessing_PropagaCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var ordemServicoId = Guid.NewGuid();
        
        var itemEstoque = ItemEstoque.Criar("Pneu", 10, TipoItemEstoqueEnum.Peca, 250.00m);
        _context.ItensEstoque.Add(itemEstoque);
        await _context.SaveChangesAsync();
        
        var solicitacao = new ReducaoEstoqueSolicitacao
        {
            CorrelationId = correlationId,
            OrdemServicoId = ordemServicoId,
            Itens = new List<ItemReducao>
            {
                new ItemReducao { ItemEstoqueId = itemEstoque.Id, Quantidade = 5 }
            }
        };

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
