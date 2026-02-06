using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record Placa
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Placa() { }

        public Placa(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new DomainException("Placa não pode ser vazia", ErrorType.InvalidInput);

            placa = placa.Replace("-", "").ToUpper().Trim();

            if (placa.Length != 7)
                throw new DomainException("Placa deve ter exatamente 7 caracteres", ErrorType.InvalidInput);

            if (!Regex.IsMatch(placa, @"^([A-Z]{3}[0-9]{4}|[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2})$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                throw new DomainException("Formato de placa inválido", ErrorType.InvalidInput);

            _valor = placa;
        }

        public string Valor => _valor;
    }
}
