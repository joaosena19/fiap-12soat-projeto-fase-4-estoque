using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record Ano
    {
        private readonly int _valor = 0;

        // Construtor sem parâmetro para EF Core
        private Ano() { }

        public Ano(int ano)
        {
            var anoAtual = DateTime.Now.Year;
            var anoMinimo = 1885; //Ano de criação do primeiro automóvel, o Benz Patent-Motorwagen

            if (ano < anoMinimo || ano > anoAtual + 1) // Permite um ano a mais para modelos do próximo ano
                throw new DomainException($"Ano deve estar entre {anoMinimo} e {anoAtual + 1}", ErrorType.InvalidInput);

            _valor = ano;
        }

        public int Valor => _valor;
    }
}
