using Shared.Attributes;

namespace Domain.Identidade.ValueObjects
{
    [ValueObject]
    public record SenhaHash
    {
        public string Valor { get; private init; } = string.Empty;

        // Construtor sem parâmetro para EF Core
        private SenhaHash() { }

        public SenhaHash(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Senha hash não pode ser vazia", nameof(valor));

            Valor = valor;
        }
    }
}