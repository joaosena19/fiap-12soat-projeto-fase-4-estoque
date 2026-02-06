using Application.Contracts.Gateways;
using Domain.Identidade.Aggregates;
using Domain.Identidade.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Infrastructure.Repositories.Identidade
{
    public class UsuarioRepository : IUsuarioGateway
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> SalvarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> ObterPorDocumentoAsync(string documento)
        {
            var documentoLimpo = Regex.Replace(documento, @"[^\d]", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));

            return await _context.Usuarios
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.DocumentoIdentificadorUsuario.Valor == documentoLimpo);
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Role>> ObterRolesAsync(IEnumerable<string> roleStrings)
        {
            var roleEnums = roleStrings
                .Select(roleString => Enum.TryParse<RoleEnum>(roleString, true, out var roleEnum) ? roleEnum : (RoleEnum?)null)
                .Where(role => role.HasValue)
                .Select(role => role!.Value)
                .ToList();

            return await _context.Roles
                .Where(r => roleEnums.Contains(r.Id))
                .ToListAsync();
        }
    }
}