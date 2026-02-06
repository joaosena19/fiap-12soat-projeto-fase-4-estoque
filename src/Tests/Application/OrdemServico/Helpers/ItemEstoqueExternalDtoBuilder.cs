using Application.OrdemServico.Dtos.External;
using Bogus;
using Domain.OrdemServico.Enums;

namespace Tests.Application.OrdemServico.Helpers
{
    public class ItemEstoqueExternalDtoBuilder
    {
        private Guid _id;
        private string _nome;
        private decimal _preco;
        private int _quantidade;
        private TipoItemIncluidoEnum _tipoItemIncluido;
        private readonly Faker _faker = new Faker("pt_BR");

        public ItemEstoqueExternalDtoBuilder()
        {
            _id = Guid.NewGuid();
            _nome = _faker.Commerce.ProductName();
            _preco = _faker.Random.Decimal(10, 500);
            _quantidade = _faker.Random.Int(1, 100);
            _tipoItemIncluido = _faker.PickRandom<TipoItemIncluidoEnum>();
        }

        public ItemEstoqueExternalDtoBuilder ComId(Guid id)
        {
            _id = id;
            return this;
        }

        public ItemEstoqueExternalDtoBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ItemEstoqueExternalDtoBuilder ComPreco(decimal preco)
        {
            _preco = preco;
            return this;
        }

        public ItemEstoqueExternalDtoBuilder ComQuantidade(int quantidade)
        {
            _quantidade = quantidade;
            return this;
        }

        public ItemEstoqueExternalDtoBuilder ComTipoItemIncluido(TipoItemIncluidoEnum tipoItemIncluido)
        {
            _tipoItemIncluido = tipoItemIncluido;
            return this;
        }

        public ItemEstoqueExternalDto Build()
        {
            return new ItemEstoqueExternalDto
            {
                Id = _id,
                Nome = _nome,
                Preco = _preco,
                Quantidade = _quantidade,
                TipoItemIncluido = _tipoItemIncluido
            };
        }
    }
}