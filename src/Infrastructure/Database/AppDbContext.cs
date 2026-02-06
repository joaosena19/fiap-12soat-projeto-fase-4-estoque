using Domain.Cadastros.Aggregates;
using Domain.Estoque.Aggregates;
using Domain.OrdemServico.Aggregates.OrdemServico;
using Domain.Identidade.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<ItemEstoque> ItensEstoque { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<ServicoIncluido> ServicosIncluidos { get; set; }
        public DbSet<ItemIncluido> ItensIncluidos { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
