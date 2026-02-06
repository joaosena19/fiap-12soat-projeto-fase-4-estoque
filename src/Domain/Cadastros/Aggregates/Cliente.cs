using Domain.Cadastros.ValueObjects.Cliente;
using Shared.Attributes;
using UUIDNext;

namespace Domain.Cadastros.Aggregates
{
    [AggregateRoot]
    public class Cliente
    {
        public Guid Id { get; private set; }
        public NomeCliente Nome { get; private set; } = null!;
        public DocumentoIdentificador DocumentoIdentificador { get; private set; } = null!;

        // Contrutor sem parâmetro para EF Core
        private Cliente() { }

        private Cliente(Guid id, NomeCliente nome, DocumentoIdentificador documentoIdentificador)
        {
            Id = id;
            Nome = nome;
            DocumentoIdentificador = documentoIdentificador;
        }

        public static Cliente Criar(string nome, string documento)
        {
            return new Cliente(Uuid.NewSequential(), new NomeCliente(nome), new DocumentoIdentificador(documento));
        }

        public void Atualizar(string nome)
        {
            Nome = new NomeCliente(nome);
        }
    }
}
