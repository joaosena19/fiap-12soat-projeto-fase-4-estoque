using Domain.Cadastros.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .HasColumnName("id");

            builder.OwnsOne(c => c.DocumentoIdentificador, doc =>
            {
                doc.Property(p => p.Valor)
                   .HasColumnName("documento_identificador")
                   .IsRequired()
                   .HasMaxLength(14);

                doc.Property(p => p.TipoDocumento)
                   .HasColumnName("tipo_documento_identificador")
                   .IsRequired()
                   .HasMaxLength(4)
                   .HasConversion(
                       v => v.ToString().ToLower(),
                       v => Enum.Parse<Domain.Cadastros.Enums.TipoDocumentoEnum>(v, true)
                   );
            });

            builder.OwnsOne(c => c.Nome, nome =>
            {
                nome.Property(p => p.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}
