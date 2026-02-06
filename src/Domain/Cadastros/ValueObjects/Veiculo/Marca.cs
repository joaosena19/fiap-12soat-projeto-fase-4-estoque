using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record Marca
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Marca() { }

        public Marca(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("Marca não pode ser vazia", ErrorType.InvalidInput);

            if (marca.Length > 200)
                throw new DomainException("Marca não pode ter mais de 200 caracteres", ErrorType.InvalidInput);

            _valor = marca.Trim();
        }

        public string Valor => _valor;
    }
}
