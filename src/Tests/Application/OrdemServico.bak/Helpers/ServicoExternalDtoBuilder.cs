using Application.OrdemServico.Dtos.External;
using Bogus;

namespace Tests.Application.OrdemServico.Helpers
{
    public class ServicoExternalDtoBuilder
    {
        private Guid _id;
        private string _nome;
        private decimal _preco;
        private readonly Faker _faker = new Faker("pt_BR");

        public ServicoExternalDtoBuilder()
        {
            _id = Guid.NewGuid();
            _nome = _faker.Commerce.ProductName();
            _preco = _faker.Random.Decimal(10, 500);
        }

        public ServicoExternalDtoBuilder ComId(Guid id)
        {
            _id = id;
            return this;
        }

        public ServicoExternalDtoBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ServicoExternalDtoBuilder ComPreco(decimal preco)
        {
            _preco = preco;
            return this;
        }

        public ServicoExternalDto Build()
        {
            return new ServicoExternalDto
            {
                Id = _id,
                Nome = _nome,
                Preco = _preco
            };
        }
    }
}