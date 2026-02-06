using Application.Contracts.Gateways;
using Domain.Cadastros.Aggregates;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Cadastros
{
    public class VeiculoRepository : IVeiculoGateway
    {
        private readonly AppDbContext _context;

        public VeiculoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Veiculo> SalvarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<Veiculo?> ObterPorPlacaAsync(string placa)
        {
            return await _context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor.ToUpper() == placa.ToUpper());
        }

        public async Task<Veiculo?> ObterPorIdAsync(Guid id)
        {
            return await _context.Veiculos
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Veiculo> AtualizarAsync(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            await _context.SaveChangesAsync();
            return veiculo;
        }

        public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
        {
            return await _context.Veiculos.ToListAsync();
        }

        public async Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(Guid clienteId)
        {
            return await _context.Veiculos
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}
