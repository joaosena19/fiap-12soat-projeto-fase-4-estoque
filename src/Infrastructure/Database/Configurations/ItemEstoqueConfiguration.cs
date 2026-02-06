using Domain.Estoque.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ItemEstoqueConfiguration : IEntityTypeConfiguration<ItemEstoque>
    {
        public void Configure(EntityTypeBuilder<ItemEstoque> builder)
        {
            builder.ToTable("itens_estoque");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                   .HasColumnName("id");

            builder.OwnsOne(i => i.Nome, nome =>
            {
                nome.Property(p => p.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(i => i.Quantidade, quantidade =>
            {
                quantidade.Property(p => p.Valor)
                          .HasColumnName("quantidade")
                          .IsRequired();
            });

            builder.OwnsOne(i => i.TipoItemEstoque, tipo =>
            {
                tipo.Property(p => p.Valor)
                    .HasColumnName("tipo_item_estoque")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion(
                        v => v.ToString().ToLower(),
                        v => Enum.Parse<Domain.Estoque.Enums.TipoItemEstoqueEnum>(v, true)
                    );
            });

            builder.OwnsOne(i => i.Preco, preco =>
            {
                preco.Property(p => p.Valor)
                     .HasColumnName("preco")
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
            });
        }
    }
}
