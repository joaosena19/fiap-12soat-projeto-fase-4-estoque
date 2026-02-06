using Bogus;
using Domain.Cadastros.Aggregates;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.SharedHelpers.AggregateBuilders
{
    public class ServicoBuilder
    {
        private string _nome;
        private decimal _preco;
        private readonly Faker _faker = new Faker("pt_BR");

        public ServicoBuilder()
        {
            _nome = _faker.Commerce.ProductName();
            _preco = _faker.Random.Decimal(10, 1000);
        }

        public ServicoBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ServicoBuilder ComPreco(decimal preco)
        {
            _preco = preco;
            return this;
        }

        public ServicoAggregate Build()
        {
            return ServicoAggregate.Criar(_nome, _preco);
        }
    }
}