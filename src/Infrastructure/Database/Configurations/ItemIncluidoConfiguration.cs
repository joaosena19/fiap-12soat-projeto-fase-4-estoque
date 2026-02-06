using Domain.OrdemServico.Aggregates.OrdemServico;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ItemIncluidoConfiguration : IEntityTypeConfiguration<ItemIncluido>
    {
        public void Configure(EntityTypeBuilder<ItemIncluido> builder)
        {
            builder.ToTable("itens_incluidos");
            builder.HasKey(ii => ii.Id);

            builder.Property(ii => ii.Id)
                   .HasColumnName("id");

            builder.Property(ii => ii.ItemEstoqueOriginalId)
                   .HasColumnName("item_estoque_original_id")
                   .IsRequired();

            builder.Property<Guid>("OrdemServicoId") //Shadow property
                   .HasColumnName("ordem_servico_id")
                   .IsRequired();

            builder.OwnsOne(ii => ii.Nome, nome =>
            {
                nome.Property(n => n.Valor)
                    .HasColumnName("nome")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(ii => ii.Quantidade, quantidade =>
            {
                quantidade.Property(q => q.Valor)
                          .HasColumnName("quantidade")
                          .IsRequired();
            });

            builder.OwnsOne(ii => ii.TipoItemIncluido, tipo =>
            {
                tipo.Property(t => t.Valor)
                    .HasColumnName("tipo_item_incluido")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion(
                        v => v.ToString().ToLower(),
                        v => Enum.Parse<Domain.OrdemServico.Enums.TipoItemIncluidoEnum>(v, true)
                    );
            });

            builder.OwnsOne(ii => ii.Preco, preco =>
            {
                preco.Property(p => p.Valor)
                     .HasColumnName("preco")
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
            });
        }
    }
}
