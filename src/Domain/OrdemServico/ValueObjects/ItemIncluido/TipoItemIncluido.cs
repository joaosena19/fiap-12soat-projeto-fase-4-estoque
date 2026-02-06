using Domain.OrdemServico.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.ItemIncluido
{
    [ValueObject]
    public record TipoItemIncluido
    {
        private readonly TipoItemIncluidoEnum _valor;

        // Construtor sem parâmetro para o EF Core
        private TipoItemIncluido() { }

        public TipoItemIncluido(TipoItemIncluidoEnum tipoItemIncluidoEnum)
        {
            if (!Enum.IsDefined(typeof(TipoItemIncluidoEnum), tipoItemIncluidoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoItemIncluidoEnum)));
                throw new DomainException($"Tipo de item incluí­do na Ordem de Serviço '{tipoItemIncluidoEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = tipoItemIncluidoEnum;
        }

        public TipoItemIncluidoEnum Valor => _valor;
    }
}
