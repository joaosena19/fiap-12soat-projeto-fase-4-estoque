using Domain.Cadastros.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.ToTable("servicos");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .HasColumnName("id");

            builder.OwnsOne(c => c.Nome, cpf =>
            {
                cpf.Property(p => p.Valor)
                   .HasColumnName("nome")
                   .IsRequired()
                   .HasMaxLength(500);
            });

            builder.OwnsOne(c => c.Preco, nome =>
            {
                nome.Property(p => p.Valor)
                    .HasColumnName("preco")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}
