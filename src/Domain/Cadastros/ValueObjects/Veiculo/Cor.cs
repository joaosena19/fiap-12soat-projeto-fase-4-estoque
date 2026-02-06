using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record Cor
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Cor() { }

        public Cor(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
                throw new DomainException("Cor não pode ser vazia", ErrorType.InvalidInput);

            if (cor.Length > 100)
                throw new DomainException("Cor não pode ter mais de 100 caracteres", ErrorType.InvalidInput);

            _valor = cor.Trim();
        }

        public string Valor => _valor;
    }
}
