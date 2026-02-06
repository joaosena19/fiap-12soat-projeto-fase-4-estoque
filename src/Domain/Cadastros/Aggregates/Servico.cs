using Domain.Cadastros.ValueObjects.Servico;
using Shared.Attributes;
using UUIDNext;

namespace Domain.Cadastros.Aggregates
{
    [AggregateRoot]
    public class Servico
    {
        public Guid Id { get; private set; }
        public NomeServico Nome { get; private set; } = null!;
        public PrecoServico Preco { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private Servico() { }

        private Servico(Guid id, NomeServico nome, PrecoServico preco)
        {
            Id = id;
            Nome = nome;
            Preco = preco;
        }

        public static Servico Criar(string nome, decimal preco)
        {
            return new Servico(Uuid.NewSequential(), new NomeServico(nome), new PrecoServico(preco));
        }

        public void Atualizar(string nome, decimal preco)
        {
            Nome = new NomeServico(nome);
            Preco = new PrecoServico(preco);
        }
    }

}
