using Domain.Identidade.Aggregates;
using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .HasColumnName("id");

            builder.OwnsOne(u => u.DocumentoIdentificadorUsuario, doc =>
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
                       v => Enum.Parse<TipoDocumentoIdentificadorUsuarioEnum>(v, true)
                   );
            });

            builder.OwnsOne(u => u.SenhaHash, senha =>
            {
                senha.Property(p => p.Valor)
                     .HasColumnName("senha_hash")
                     .IsRequired()
                     .HasMaxLength(500);
            });

            builder.OwnsOne(u => u.Status, status =>
            {
                status.Property(p => p.Valor)
                      .HasColumnName("status")
                      .IsRequired()
                      .HasConversion(
                          v => v.ToString().ToLower(),
                          v => Enum.Parse<StatusUsuarioEnum>(v, true)
                      );
            });

            // Configuração da lista de Roles com tabela de junção
            builder.HasMany(u => u.Roles)
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                       "usuarios_roles",
                       j => j.HasOne<Role>()
                            .WithMany()
                            .HasForeignKey("role_id")
                            .HasPrincipalKey(nameof(Role.Id)),
                       j => j.HasOne<Usuario>()
                            .WithMany()
                            .HasForeignKey("usuario_id"),
                       j =>
                       {
                           j.ToTable("usuarios_roles");
                           j.HasKey("usuario_id", "role_id");
                           
                           j.Property("usuario_id")
                            .HasColumnName("usuario_id");
                            
                           j.Property("role_id")
                            .HasColumnName("role_id");
                       });
        }
    }
}