using Domain.OrdemServico.Aggregates.OrdemServico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
    {
        public void Configure(EntityTypeBuilder<OrdemServico> builder)
        {
            builder.ToTable("ordens_servico");
            builder.HasKey(os => os.Id);

            builder.Property(os => os.Id)
                   .HasColumnName("id");

            builder.Property(os => os.VeiculoId)
                   .HasColumnName("veiculo_id")
                   .IsRequired();

            builder.OwnsOne(os => os.Codigo, codigo =>
            {
                codigo.Property(c => c.Valor)
                      .HasColumnName("codigo")
                      .IsRequired()
                      .HasMaxLength(50);
            });

            builder.OwnsOne(os => os.Status, status =>
            {
                status.Property(s => s.Valor)
                      .HasColumnName("status")
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasConversion(
                          v => v.ToString().ToLower(),
                          v => Enum.Parse<Domain.OrdemServico.Enums.StatusOrdemServicoEnum>(v, true)
                      );
            });

            builder.OwnsOne(os => os.Historico, historico =>
            {
                historico.Property(h => h.DataCriacao)
                         .HasColumnName("data_criacao")
                         .IsRequired();

                historico.Property(h => h.DataInicioExecucao)
                         .HasColumnName("data_inicio_execucao");

                historico.Property(h => h.DataFinalizacao)
                         .HasColumnName("data_finalizacao");

                historico.Property(h => h.DataEntrega)
                         .HasColumnName("data_entrega");
            });

            builder.HasMany(os => os.ServicosIncluidos)
                   .WithOne()
                   .HasForeignKey("OrdemServicoId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(OrdemServico.ServicosIncluidos))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(os => os.ItensIncluidos)
                   .WithOne()
                   .HasForeignKey("OrdemServicoId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(OrdemServico.ItensIncluidos))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne(os => os.Orcamento)
                   .WithOne()
                   .HasForeignKey<Orcamento>("OrdemServicoId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
