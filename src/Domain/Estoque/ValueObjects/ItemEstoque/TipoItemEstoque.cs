using Domain.Estoque.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Estoque.ValueObjects.ItemEstoque
{
    [ValueObject]
    public record TipoItemEstoque
    {
        private readonly TipoItemEstoqueEnum _valor;

        // Construtor sem parâmetro para o EF Core
        private TipoItemEstoque() { }

        public TipoItemEstoque(TipoItemEstoqueEnum tipoItemEstoqueEnum)
        {
            if (!Enum.IsDefined(typeof(TipoItemEstoqueEnum), tipoItemEstoqueEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoItemEstoqueEnum)));
                throw new DomainException($"Tipo de item de estoque '{tipoItemEstoqueEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = tipoItemEstoqueEnum;
        }

        public TipoItemEstoqueEnum Valor => _valor;
    }
}
