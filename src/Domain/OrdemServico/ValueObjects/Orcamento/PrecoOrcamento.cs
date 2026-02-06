using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.Orcamento
{
    [ValueObject]
    public record PrecoOrcamento
    {
        private readonly decimal _valor;

        // Construtor sem parâmetro para EF Core
        private PrecoOrcamento() { }

        public PrecoOrcamento(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("Preço do orçamento não pode ser negativo", ErrorType.InvalidInput);

            _valor = preco;
        }

        public decimal Valor => _valor;
    }
}
