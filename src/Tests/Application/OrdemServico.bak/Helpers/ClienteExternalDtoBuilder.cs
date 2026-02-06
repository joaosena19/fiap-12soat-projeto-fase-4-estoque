using Application.OrdemServico.Dtos.External;
using Bogus;

namespace Tests.Application.OrdemServico.Helpers
{
    public class ClienteExternalDtoBuilder
    {
        private Guid _id;
        private string _nome;
        private string _documentoIdentificador;
        private readonly Faker _faker = new Faker("pt_BR");

        public ClienteExternalDtoBuilder()
        {
            _id = Guid.NewGuid();
            _nome = _faker.Person.FullName;
            _documentoIdentificador = _faker.Random.Replace("###.###.###-##");
        }

        public ClienteExternalDtoBuilder ComId(Guid id)
        {
            _id = id;
            return this;
        }

        public ClienteExternalDtoBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ClienteExternalDtoBuilder ComDocumentoIdentificador(string documentoIdentificador)
        {
            _documentoIdentificador = documentoIdentificador;
            return this;
        }

        public ClienteExternalDto Build()
        {
            return new ClienteExternalDto
            {
                Id = _id,
                Nome = _nome,
                DocumentoIdentificador = _documentoIdentificador
            };
        }
    }
}