using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
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
    [Trait("Category", "SeedData")]
    public void SeedItensEstoque_Deve_PopularBancoVazioComTrezeItens_Quando_BancoEstaVazio()
    {
        // Arrange - banco já está vazio do construtor

        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itensNoBank = _context.ItensEstoque.ToList();
        Assert.Equal(13, itensNoBank.Count);
    }

    [Fact(DisplayName = "SeedItensEstoque não deve adicionar itens quando já existem dados")]
    [Trait("Category", "SeedData")]
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
        Assert.Single(itensNoBank); // Apenas o item existente
        Assert.Equal("Item Existente", itensNoBank.First().Nome.Valor);
    }

    [Fact(DisplayName = "SeedItensEstoque deve ser idempotente - múltiplas chamadas não alteram resultado")]
    [Trait("Category", "SeedData")]
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
        Assert.Equal(13, itensAposPrimeiraChamada);
        Assert.Equal(itensAposPrimeiraChamada, itensAposSegundaChamada);
        Assert.Equal(itensAposSegundaChamada, itensAposTerceiraChamada);
    }

    [Fact(DisplayName = "SeedItensEstoque deve criar itens com IDs específicos do SeedIds")]
    [Trait("Category", "SeedData")]
    public void SeedItensEstoque_Deve_CriarItensComIdEspecificos_Quando_DefinidosNoSeedIds()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var oleoMotor = _context.ItensEstoque.Find(SeedIds.ItensEstoque.OleoMotor5w30);
        var filtroOleo = _context.ItensEstoque.Find(SeedIds.ItensEstoque.FiltroDeOleo);
        var pastilhaFreio = _context.ItensEstoque.Find(SeedIds.ItensEstoque.PastilhaDeFreioDianteira);

        Assert.NotNull(oleoMotor);
        Assert.Equal("Óleo Motor 5W30", oleoMotor.Nome.Valor);
        Assert.Equal(50, oleoMotor.Quantidade.Valor);
        Assert.Equal(TipoItemEstoqueEnum.Peca, oleoMotor.TipoItemEstoque.Valor);
        Assert.Equal(45.90m, oleoMotor.Preco.Valor);

        Assert.NotNull(filtroOleo);
        Assert.Equal("Filtro de Óleo", filtroOleo.Nome.Valor);
        Assert.Equal(30, filtroOleo.Quantidade.Valor);
        Assert.Equal(25.50m, filtroOleo.Preco.Valor);

        Assert.NotNull(pastilhaFreio);
        Assert.Equal("Pastilha de Freio Dianteira", pastilhaFreio.Nome.Valor);
        Assert.Equal(20, pastilhaFreio.Quantidade.Valor);
        Assert.Equal(89.90m, pastilhaFreio.Preco.Valor);
    }

    [Fact(DisplayName = "SeedItensEstoque deve criar peças e insumos nos tipos corretos")]
    [Trait("Category", "SeedData")]
    public void SeedItensEstoque_Deve_CriarPecasEInsumosNosTiposCorretos_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        var pecas = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca).ToList();
        var insumos = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo).ToList();

        Assert.Equal(8, pecas.Count); // Peças: Óleo, Filtro, Pastilhas, Correia, Vela, Disco, etc.
        Assert.Equal(5, insumos.Count); // Insumos: Fluido, Aditivo, Graxa, Desengraxante, Spray

        // Verificar alguns nomes específicos
        Assert.Contains(pecas, p => p.Nome.Valor == "Óleo Motor 5W30");
        Assert.Contains(pecas, p => p.Nome.Valor == "Correia Dentada");
        Assert.Contains(insumos, i => i.Nome.Valor == "Fluido de Freio");
        Assert.Contains(insumos, i => i.Nome.Valor == "Graxa Multiuso");
    }

    [Fact(DisplayName = "SeedAll deve chamar SeedItensEstoque")]
    [Trait("Category", "SeedData")]
    public void SeedAll_Deve_ChamarSeedItensEstoque_Quando_Executado()
    {
        // Arrange - banco vazio

        // Act
        SeedData.SeedAll(_context);

        // Assert
        var itensNoBank = _context.ItensEstoque.Count();
        Assert.Equal(13, itensNoBank); // Mesmo resultado de SeedItensEstoque
    }

    [Fact(DisplayName = "SeedItensEstoque deve garantir que todos os itens tenham preços positivos")]
    [Trait("Category", "SeedData")]
    public void SeedItensEstoque_Deve_GarantirTodosItensComPrecosPositivos_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        Assert.All(itens, item =>
        {
            Assert.True(item.Preco.Valor > 0, $"Item '{item.Nome.Valor}' deve ter preço positivo");
            Assert.True(item.Quantidade.Valor >= 0, $"Item '{item.Nome.Valor}' deve ter quantidade não-negativa");
        });
    }

    [Fact(DisplayName = "SeedItensEstoque deve garantir que nomes dos itens não sejam nulos ou vazios")]
    [Trait("Category", "SeedData")]
    public void SeedItensEstoque_Deve_GarantirNomesValidosParaTodosItens_Quando_Executado()
    {
        // Act
        SeedData.SeedItensEstoque(_context);

        // Assert
        var itens = _context.ItensEstoque.ToList();
        Assert.All(itens, item =>
        {
            Assert.False(string.IsNullOrWhiteSpace(item.Nome.Valor), "Nome do item não pode ser nulo ou vazio");
            Assert.True(item.Nome.Valor.Length >= 3, $"Nome '{item.Nome.Valor}' deve ter pelo menos 3 caracteres");
        });
    }
}