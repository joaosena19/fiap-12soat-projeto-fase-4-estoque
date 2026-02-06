using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.ItemIncluido;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.OrdemServico.Aggregates.OrdemServico
{
    [AggregateMember]
    public class ItemIncluido
    {
        public Guid Id { get; private set; }
        public Guid ItemEstoqueOriginalId { get; private set; }
        public Nome Nome { get; private set; } = null!;
        public Quantidade Quantidade { get; private set; } = null!;
        public TipoItemIncluido TipoItemIncluido { get; private set; } = null!;
        public PrecoItem Preco { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private ItemIncluido() { }

        private ItemIncluido(Guid id, Guid itemEstoqueOriginalId, PrecoItem preco, Nome nome, Quantidade quantidade, TipoItemIncluido tipoItemIncluido)
        {
            Id = id;
            ItemEstoqueOriginalId = itemEstoqueOriginalId;
            Preco = preco;
            Nome = nome;
            Quantidade = quantidade;
            TipoItemIncluido = tipoItemIncluido;
        }

        public static ItemIncluido Criar(Guid itemEstoqueOriginalId, string nome, decimal precoUnitario, int quantidade, TipoItemIncluidoEnum tipoItemIncluido)
        {
            return new ItemIncluido(
                Uuid.NewSequential(),
                itemEstoqueOriginalId,
                new PrecoItem(precoUnitario),
                new Nome(nome), 
                new Quantidade(quantidade), 
                new TipoItemIncluido(tipoItemIncluido)
            );
        }

        public void AtualizarQuantidade(int quantidade)
        {
            Quantidade = new Quantidade(quantidade);
        }

        public void IncrementarQuantidade(int quantidadeAdicional)
        {
            if (quantidadeAdicional <= 0)
                throw new DomainException("A quantidade a adicionar deve ser maior que zero.", ErrorType.InvalidInput);

            Quantidade = new Quantidade(Quantidade.Valor + quantidadeAdicional);
        }
    }
}
