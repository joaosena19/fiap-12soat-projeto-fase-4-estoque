using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.DTOs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Helpers;
using Tests.Infrastructure.Messaging.Consumers.Helpers;
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
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<ConsumeContext<ReducaoEstoqueSolicitacao>> _contextMock;
    private readonly ReducaoEstoqueSolicitacaoConsumer _consumer;

    public ReducaoEstoqueSolicitacaoConsumerTests()
    {
        // Setup InMemory database usando builder
        _context = AppDbContextInMemoryBuilder.Novo().Build();
        
        // Setup mocked logger factory (hermetic - sem console output)
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        var mockLogger = new Mock<ILogger>();
        _loggerFactoryMock
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(mockLogger.Object);
        
        _contextMock = new Mock<ConsumeContext<ReducaoEstoqueSolicitacao>>();
        _consumer = new ReducaoEstoqueSolicitacaoConsumer(_context, _loggerFactoryMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact(DisplayName = "Consumir redução de estoque deve reduzir quantidade e publicar sucesso quando estoque suficiente")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Sucesso")]
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
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoSucesso(correlationId, ordemServicoId);

        // Verifica que a quantidade foi reduzida no banco
        var itemAtualizado = await _context.ItensEstoque.FindAsync(itemEstoque.Id);
        itemAtualizado.Should().NotBeNull();
        itemAtualizado!.Quantidade.Valor.Should().Be(5);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve publicar falha com motivo quando estoque insuficiente")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Falha")]
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
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoFalha(correlationId, ordemServicoId, "estoque_insuficiente");

        // Verifica que a quantidade não foi alterada
        var itemAtualizado = await _context.ItensEstoque.FindAsync(itemEstoque.Id);
        itemAtualizado.Should().NotBeNull();
        itemAtualizado!.Quantidade.Valor.Should().Be(quantidadeOriginal);
    }

    [Fact(DisplayName = "Consumir redução de estoque deve publicar falha com motivo quando item não encontrado")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Falha")]
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
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoFalha(correlationId, ordemServicoId, "estoque_insuficiente");
    }

    [Fact(DisplayName = "Consumir redução de estoque deve processar todos itens com sucesso quando múltiplos itens")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Sucesso")]
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
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoSucesso(correlationId, ordemServicoId);

        // Verifica que ambas as quantidades foram reduzidas
        var item1Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque1.Id);
        var item2Atualizado = await _context.ItensEstoque.FindAsync(itemEstoque2.Id);
        item1Atualizado.Should().NotBeNull();
        item2Atualizado.Should().NotBeNull();
        item1Atualizado!.Quantidade.Valor.Should().Be(16);
        item2Atualizado!.Quantidade.Valor.Should().Be(13);
    }

    [Fact(DisplayName = "Consumir redução de estoque não deve reduzir quantidade quando múltiplos itens e um insuficiente")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Falha")]
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
        item1Atualizado.Should().NotBeNull();
        item2Atualizado.Should().NotBeNull();
        item1Atualizado!.Quantidade.Valor.Should().Be(quantidadeOriginal1);
        item2Atualizado!.Quantidade.Valor.Should().Be(quantidadeOriginal2);
        
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoFalha(correlationId, ordemServicoId, "estoque_insuficiente");
    }

    [Fact(DisplayName = "Consumir redução de estoque deve propagar CorrelationId no resultado")]
    [Trait("Categoria", "Mensageria")]
    [Trait("Cenario", "Sucesso")]
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
        _contextMock.DeveTerPublicadoReducaoEstoqueResultadoSucesso(correlationId, ordemServicoId);
    }
}
