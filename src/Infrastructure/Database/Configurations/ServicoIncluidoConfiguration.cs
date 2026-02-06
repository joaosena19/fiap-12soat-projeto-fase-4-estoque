using Domain.OrdemServico.Aggregates.OrdemServico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ServicoIncluidoConfiguration : IEntityTypeConfiguration<ServicoIncluido>
    {
        public void Configure(EntityTypeBuilder<ServicoIncluido> builder)
        {
            builder.ToTable("servicos_incluidos");
            builder.HasKey(si => si.Id);

            builder.Property(si => si.Id)
                   .HasColumnName("id");

            builder.Property(si => si.ServicoOriginalId)
                   .HasColumnName("servico_original_id")
                   .IsRequired();

            builder.Property<Guid>("OrdemServicoId") //Shadow property
                   .HasColumnName("ordem_servico_id")
                   .IsRequired();

            builder.OwnsOne(si => si.Nome, nome =>
            {
                nome.Property(n => n.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(500);
            });

            builder.OwnsOne(si => si.Preco, preco =>
            {
                preco.Property(p => p.Valor)
                     .HasColumnName("preco")
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
            });
        }
    }
}
