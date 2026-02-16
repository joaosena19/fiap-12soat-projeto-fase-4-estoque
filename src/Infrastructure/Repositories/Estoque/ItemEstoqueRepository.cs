using Application.Contracts.Gateways;
using Domain.Estoque.Aggregates;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Estoque
{
    public class ItemEstoqueRepository : IItemEstoqueGateway
    {
        private readonly AppDbContext _context;

        public ItemEstoqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ItemEstoque> SalvarAsync(ItemEstoque itemEstoque)
        {
            await _context.ItensEstoque.AddAsync(itemEstoque);
            await _context.SaveChangesAsync();

            return itemEstoque;
        }

        public async Task<ItemEstoque?> ObterPorIdAsync(Guid id)
        {
            return await _context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ItemEstoque?> ObterPorNomeAsync(string nome)
        {
            return await _context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == nome);
        }

        public async Task<ItemEstoque> AtualizarAsync(ItemEstoque itemEstoque)
        {
            _context.ItensEstoque.Update(itemEstoque);
            await _context.SaveChangesAsync();

            return itemEstoque;
        }

        public async Task<IEnumerable<ItemEstoque>> ObterTodosAsync()
        {
            return await _context.ItensEstoque.ToListAsync();
        }
    }
}
