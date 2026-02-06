using UUIDNext;
using Domain.OrdemServico.ValueObjects.Orcamento;
using Shared.Attributes;

namespace Domain.OrdemServico.Aggregates.OrdemServico
{
    [AggregateMember]
    public class Orcamento
    {
        public Guid Id { get; private init; }
        public DataCriacao DataCriacao { get; private set; } = null!;
        public PrecoOrcamento Preco { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private Orcamento() { }

        private Orcamento(Guid id, DataCriacao dataCriacao, PrecoOrcamento preco)
        {
            Id = id;
            DataCriacao = dataCriacao;
            Preco = preco;
        }

        public static Orcamento GerarOrcamento(IEnumerable<ServicoIncluido> servicos, IEnumerable<ItemIncluido> itens)
        {
            var totalServicos = servicos.Sum(s => s.Preco.Valor);
            var totalItens = itens.Sum(i => i.Preco.Valor * i.Quantidade.Valor);
            var total = totalServicos + totalItens;

            return new Orcamento(Uuid.NewSequential(), new DataCriacao(DateTime.UtcNow), new PrecoOrcamento(total));
        }
    }
}