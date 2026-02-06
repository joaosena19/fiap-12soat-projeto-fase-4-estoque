using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record Modelo
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Modelo() { }

        public Modelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("Modelo não pode ser vazio", ErrorType.InvalidInput);

            if (modelo.Length > 200)
                throw new DomainException("Modelo não pode ter mais de 200 caracteres", ErrorType.InvalidInput);

            _valor = modelo.Trim();
        }

        public string Valor => _valor;
    }
}
