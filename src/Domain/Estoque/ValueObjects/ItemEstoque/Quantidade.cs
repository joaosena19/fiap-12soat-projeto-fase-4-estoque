using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Estoque.ValueObjects.ItemEstoque
{
    [ValueObject]
    public record Quantidade
    {
        private readonly int _valor;

        // Construtor sem parâmetro para o EF Core
        private Quantidade() { }

        public Quantidade(int quantidade)
        {
            if (quantidade < 0)
                throw new DomainException("Quantidade não pode ser negativa", ErrorType.InvalidInput);

            _valor = quantidade;
        }

        public int Valor => _valor;
    }
}
