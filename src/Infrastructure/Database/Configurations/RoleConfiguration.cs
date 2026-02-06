using Domain.Identidade.Enums;
using Domain.Identidade.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                   .HasColumnName("id")
                   .HasConversion(
                       v => (int)v,
                       v => (RoleEnum)v
                   );

            builder.OwnsOne(r => r.Nome, nome =>
            {
                nome.Property(p => p.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Seed data para as roles
            builder.HasData(
                new { Id = RoleEnum.Administrador },
                new { Id = RoleEnum.Cliente }
            );
            
            builder.OwnsOne(r => r.Nome).HasData(
                new { RoleId = RoleEnum.Administrador, Valor = RoleEnum.Administrador.ToString() },
                new { RoleId = RoleEnum.Cliente, Valor = RoleEnum.Cliente.ToString() }
            );
        }
    }
}