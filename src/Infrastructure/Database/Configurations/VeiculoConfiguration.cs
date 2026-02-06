using Domain.Cadastros.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.ToTable("veiculos");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                   .HasColumnName("id");

            builder.Property(v => v.ClienteId)
                   .HasColumnName("cliente_id")
                   .IsRequired();

            builder.HasOne<Cliente>()
                   .WithMany()
                   .HasForeignKey(v => v.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(v => v.Placa, placa =>
            {
                placa.Property(p => p.Valor)
                   .HasColumnName("placa")
                   .IsRequired()
                   .HasMaxLength(7);
            });

            builder.OwnsOne(v => v.Modelo, modelo =>
            {
                modelo.Property(p => p.Valor)
                    .HasColumnName("modelo")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(v => v.Marca, marca =>
            {
                marca.Property(p => p.Valor)
                    .HasColumnName("marca")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(v => v.Cor, cor =>
            {
                cor.Property(p => p.Valor)
                    .HasColumnName("cor")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.OwnsOne(v => v.Ano, ano =>
            {
                ano.Property(p => p.Valor)
                    .HasColumnName("ano")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.TipoVeiculo, tipo =>
            {
                tipo.Property(p => p.Valor)
                    .HasColumnName("tipo_veiculo")
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasConversion(
                        v => v.ToString().ToLower(),
                        v => Enum.Parse<Domain.Cadastros.Enums.TipoVeiculoEnum>(v, true)
                    );
            });
        }
    }
}
