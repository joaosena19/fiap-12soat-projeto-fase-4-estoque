using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Microsoft.Extensions.Options;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static void SeedItensEstoque(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.ItensEstoque.Any())
                return;

            // 2. Cria dados de teste para itens de estoque
            var itensEstoqueDeTeste = new List<ItemEstoque>
            {
                // Peças
                ItemEstoque.Criar("Óleo Motor 5W30", 50, TipoItemEstoqueEnum.Peca, 45.90m),
                ItemEstoque.Criar("Filtro de Óleo", 30, TipoItemEstoqueEnum.Peca, 25.50m),
                ItemEstoque.Criar("Pastilha de Freio Dianteira", 20, TipoItemEstoqueEnum.Peca, 89.90m),
                ItemEstoque.Criar("Pastilha de Freio Traseira", 25, TipoItemEstoqueEnum.Peca, 65.90m),
                ItemEstoque.Criar("Filtro de Ar", 40, TipoItemEstoqueEnum.Peca, 32.90m),
                ItemEstoque.Criar("Correia Dentada", 15, TipoItemEstoqueEnum.Peca, 125.90m),
                ItemEstoque.Criar("Vela de Ignição", 60, TipoItemEstoqueEnum.Peca, 18.90m),
                ItemEstoque.Criar("Disco de Freio", 10, TipoItemEstoqueEnum.Peca, 189.90m),
                
                // Insumos
                ItemEstoque.Criar("Fluido de Freio", 100, TipoItemEstoqueEnum.Insumo, 15.90m),
                ItemEstoque.Criar("Aditivo para Radiador", 80, TipoItemEstoqueEnum.Insumo, 22.50m),
                ItemEstoque.Criar("Graxa Multiuso", 200, TipoItemEstoqueEnum.Insumo, 8.90m),
                ItemEstoque.Criar("Desengraxante", 150, TipoItemEstoqueEnum.Insumo, 12.90m),
                ItemEstoque.Criar("Spray Lubrificante", 120, TipoItemEstoqueEnum.Insumo, 16.50m)
            };

            // 3. Salva os dados no banco
            context.ItensEstoque.AddRange(itensEstoqueDeTeste);
            context.SaveChanges();
        }

        public static void SeedAll(AppDbContext context)
        {
            SeedItensEstoque(context);
        }
    }
}
