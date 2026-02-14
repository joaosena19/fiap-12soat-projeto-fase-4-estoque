using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Shared.Seed;
using Tests.Helpers;
using Xunit;

namespace Tests.Infrastructure.Database;

/// <summary>
/// Testes unitários para SeedData.
/// Valida seeding de dados iniciais, idempotência e comportamento de não-duplicação.
/// </summary>
public class SeedDataTests : IDisposable
{
    private readonly AppDbContext _context;

    public SeedDataTests()
    {
        _context = AppDbContextInMemoryBuilder.Novo().Build();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact(DisplayName = "SeedItensEstoque deve popular banco vazio com 13 itens")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_PopularBancoVazioComTrezeItens_Quando_BancoEstaVazio()
    {
        // Arrange - banco já está vazio do construtor

        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itensNoBank = _context.ItensEstoque.ToList();
        itensNoBank.Should().HaveCount(13);
    }

    [Fact(DisplayName = "SeedItensEstoque não deve adicionar itens quando já existem dados")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_NaoDeve_AdicionarItens_Quando_JaExistemDados()
    {
        // Arrange
        var itemExistente = ItemEstoque.Criar("Item Existente", 10, TipoItemEstoqueEnum.Peca, 100.00m);
        _context.ItensEstoque.Add(itemExistente);
        _context.SaveChanges();

        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itensNoBank = _context.ItensEstoque.ToList();
        itensNoBank.Should().HaveCount(1);
        itensNoBank.First().Nome.Valor.Should().Be("Item Existente");
    }

    [Fact(DisplayName = "SeedItensEstoque deve ser idempotente - múltiplas chamadas não alteram resultado")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_SerIdempotente_Quando_ChamadoMultiplasVezes()
    {
        // Act - primeira chamada
        SeedData.SeedItensEstoque(_context);
        var itensAposPrimeiraChamada = _context.ItensEstoque.Count();

        // Act - segunda chamada
        SeedData.SeedItensEstoque(_context);
        var itensAposSegundaChamada = _context.ItensEstoque.Count();

        // Act - terceira chamada
        SeedData.SeedItensEstoque(_context);
        var itensAposTerceiraChamada = _context.ItensEstoque.Count();

        // Assert
        itensAposPrimeiraChamada.Should().Be(13);
        itensAposSegundaChamada.Should().Be(itensAposPrimeiraChamada);
        itensAposTerceiraChamada.Should().Be(itensAposSegundaChamada);
    }

    [Fact(DisplayName = "SeedItensEstoque deve criar itens com IDs específicos do SeedIds")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_CriarItensComIdEspecificos_Quando_DefinidosNoSeedIds()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var oleoMotor = _context.ItensEstoque.Find(SeedIds.ItensEstoque.OleoMotor5w30);
        var filtroOleo = _context.ItensEstoque.Find(SeedIds.ItensEstoque.FiltroDeOleo);
        var pastilhaFreio = _context.ItensEstoque.Find(SeedIds.ItensEstoque.PastilhaDeFreioDianteira);

        oleoMotor.Should().NotBeNull();
        oleoMotor.Nome.Valor.Should().Be("Óleo Motor 5W30");
        oleoMotor.Quantidade.Valor.Should().Be(50);
        oleoMotor.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Peca);
        oleoMotor.Preco.Valor.Should().Be(45.90m);

        filtroOleo.Should().NotBeNull();
        filtroOleo.Nome.Valor.Should().Be("Filtro de Óleo");
        filtroOleo.Quantidade.Valor.Should().Be(30);
        filtroOleo.Preco.Valor.Should().Be(25.50m);

        pastilhaFreio.Should().NotBeNull();
        pastilhaFreio.Nome.Valor.Should().Be("Pastilha de Freio Dianteira");
        pastilhaFreio.Quantidade.Valor.Should().Be(20);
        pastilhaFreio.Preco.Valor.Should().Be(89.90m);
    }

    [Fact(DisplayName = "SeedItensEstoque deve criar peças e insumos nos tipos corretos")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_CriarPecasEInsumosNosTiposCorretos_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        var pecas = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca).ToList();
        var insumos = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo).ToList();

        pecas.Should().HaveCount(8);
        insumos.Should().HaveCount(5);

        pecas.Should().Contain(p => p.Nome.Valor == "Óleo Motor 5W30");
        pecas.Should().Contain(p => p.Nome.Valor == "Correia Dentada");
        insumos.Should().Contain(i => i.Nome.Valor == "Fluido de Freio");
        insumos.Should().Contain(i => i.Nome.Valor == "Graxa Multiuso");
    }

    [Fact(DisplayName = "SeedAll deve chamar SeedItensEstoque")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedAll")]
    public void SeedAll_Deve_ChamarSeedItensEstoque_Quando_Executado()
    {
        // Arrange - banco vazio

        // Act
        SeedData.SeedAll(_context);

        // Assert
        var itensNoBank = _context.ItensEstoque.Count();
        itensNoBank.Should().Be(13);
    }

    [Fact(DisplayName = "SeedItensEstoque deve garantir que todos os itens tenham preços positivos")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_GarantirTodosItensComPrecosPositivos_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        itens.Should().AllSatisfy(item =>
        {
            item.Preco.Valor.Should().BePositive($"Item '{item.Nome.Valor}' deve ter preço positivo");
            item.Quantidade.Valor.Should().BeGreaterThanOrEqualTo(0, $"Item '{item.Nome.Valor}' deve ter quantidade não-negativa");
        });
    }

    [Fact(DisplayName = "SeedItensEstoque deve garantir que nomes dos itens não sejam nulos ou vazios")]
    [Trait("Componente", "Seed")]
    [Trait("Método", "SeedItensEstoque")]
    public void SeedItensEstoque_Deve_GarantirNomesValidosParaTodosItens_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        itens.Should().AllSatisfy(item =>
        {
            item.Nome.Valor.Should().NotBeNullOrWhiteSpace("Nome do item não pode ser nulo ou vazio");
            item.Nome.Valor.Length.Should().BeGreaterThanOrEqualTo(3, $"Nome '{item.Nome.Valor}' deve ter pelo menos 3 caracteres");
        });
    }
}