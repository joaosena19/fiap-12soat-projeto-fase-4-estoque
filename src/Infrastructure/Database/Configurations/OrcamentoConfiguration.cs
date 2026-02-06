using Domain.OrdemServico.Aggregates.OrdemServico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
    {
        public void Configure(EntityTypeBuilder<Orcamento> builder)
        {
            builder.ToTable("orcamentos");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                   .HasColumnName("id");

            builder.Property<Guid>("OrdemServicoId") //Shadow property
                   .HasColumnName("ordem_servico_id")
                   .IsRequired();

            builder.OwnsOne(o => o.DataCriacao, dataCriacao =>
            {
                dataCriacao.Property(dc => dc.Valor)
                           .HasColumnName("data_criacao")
                           .IsRequired();
            });

            builder.OwnsOne(o => o.Preco, preco =>
            {
                preco.Property(p => p.Valor)
                     .HasColumnName("preco")
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
            });
        }
    }
}
