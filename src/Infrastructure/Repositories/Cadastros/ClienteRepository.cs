using Application.Contracts.Gateways;
using Domain.Cadastros.Aggregates;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Infrastructure.Repositories.Cadastros
{
    public class ClienteRepository : IClienteGateway
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente> SalvarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
        {
            var documentoLimpo = Regex.Replace(documento, @"[^\d]", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

            return await _context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == documentoLimpo);
        }

        public async Task<Cliente?> ObterPorIdAsync(Guid id)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> AtualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }
    }
}
