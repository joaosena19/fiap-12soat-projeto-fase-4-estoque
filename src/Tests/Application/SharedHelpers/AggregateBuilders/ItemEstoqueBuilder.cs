using Bogus;
using Domain.Estoque.Enums;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.SharedHelpers.AggregateBuilders
{
    public class ItemEstoqueBuilder
    {
        private string _nome;
        private int _quantidade;
        private TipoItemEstoqueEnum _tipoItemEstoque;
        private decimal _preco;
        private readonly Faker _faker = new Faker("pt_BR");

        public ItemEstoqueBuilder()
        {
            _nome = _faker.Commerce.ProductName();
            _quantidade = _faker.Random.Int(1, 100);
            _tipoItemEstoque = _faker.PickRandom<TipoItemEstoqueEnum>();
            _preco = _faker.Random.Decimal(10, 1000);
        }

        public ItemEstoqueBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ItemEstoqueBuilder ComQuantidade(int quantidade)
        {
            _quantidade = quantidade;
            return this;
        }

        public ItemEstoqueBuilder ComTipoItemEstoque(TipoItemEstoqueEnum tipoItemEstoque)
        {
            _tipoItemEstoque = tipoItemEstoque;
            return this;
        }

        public ItemEstoqueBuilder ComPreco(decimal preco)
        {
            _preco = preco;
            return this;
        }

        public ItemEstoqueAggregate Build()
        {
            return ItemEstoqueAggregate.Criar(_nome, _quantidade, _tipoItemEstoque, _preco);
        }
    }
}