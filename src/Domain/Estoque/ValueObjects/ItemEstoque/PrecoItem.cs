using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Estoque.ValueObjects.ItemEstoque
{
    [ValueObject]
    public record PrecoItem
    {
        private readonly decimal _valor = 0M;

        // Construtor sem parâmetro para EF Core
        private PrecoItem() { }

        public PrecoItem(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("Preço não pode ser negativo", ErrorType.InvalidInput);

            _valor = preco;
        }

        public decimal Valor => _valor;
    }
}
