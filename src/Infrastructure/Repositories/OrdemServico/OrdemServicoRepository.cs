using Application.Contracts.Gateways;
using Domain.OrdemServico.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Infrastructure.Repositories.OrdemServico
{
    public class OrdemServicoRepository : IOrdemServicoGateway
    {
        private readonly AppDbContext _context;

        public OrdemServicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrdemServicoAggregate> SalvarAsync(OrdemServicoAggregate ordemServico)
        {
            await _context.OrdensServico.AddAsync(ordemServico);
            await _context.SaveChangesAsync();

            return ordemServico;
        }

        public async Task<OrdemServicoAggregate?> ObterPorIdAsync(Guid id)
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstOrDefaultAsync(os => os.Id == id);
        }

        public async Task<OrdemServicoAggregate?> ObterPorCodigoAsync(string codigo)
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstOrDefaultAsync(os => os.Codigo.Valor.Trim().Replace("-", "").ToUpper() == codigo.Trim().Replace("-", "").ToUpper());
        }

        public async Task<OrdemServicoAggregate> AtualizarAsync(OrdemServicoAggregate ordemServico)
        {
            // Busca os dados atuais para ver se as entidades filhas de OrdemServico devem ser adicionadas
            var existingOrdemServico = await _context.OrdensServico.AsNoTracking()
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .FirstAsync(os => os.Id == ordemServico.Id);

            var existingServicoIds = existingOrdemServico.ServicosIncluidos.Select(s => s.Id).ToHashSet();
            var existingItensIds = existingOrdemServico.ItensIncluidos.Select(s => s.Id).ToHashSet();
            var existingOrcamento = existingOrdemServico.Orcamento?.Id;

            await _context.ServicosIncluidos.AddRangeAsync(ordemServico.ServicosIncluidos.Where(si => !existingServicoIds.Contains(si.Id)));
            await _context.ItensIncluidos.AddRangeAsync(ordemServico.ItensIncluidos.Where(si => !existingItensIds.Contains(si.Id)));

            if(existingOrcamento is null && ordemServico.Orcamento is not null)
                await _context.Orcamentos.AddAsync(ordemServico.Orcamento);

            await _context.SaveChangesAsync();

            return ordemServico;
        }

        public async Task<IEnumerable<OrdemServicoAggregate>> ObterTodosAsync()
        {
            return await _context.OrdensServico
                .Include(os => os.ServicosIncluidos)
                .Include(os => os.ItensIncluidos)
                .Include(os => os.Orcamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdemServicoAggregate>> ObterEntreguesUltimosDiasAsync(int quantidadeDias)
        {
            var dataLimite = DateTime.UtcNow.AddDays(-quantidadeDias);
            
            return await _context.OrdensServico
                .Where(os => os.Status.Valor == StatusOrdemServicoEnum.Entregue && 
                            os.Historico.DataCriacao.Date >= dataLimite.Date)
                .ToListAsync();
        }
    }
}
