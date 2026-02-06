using Domain.Cadastros.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    [ValueObject]
    public record TipoVeiculo
    {
        private readonly TipoVeiculoEnum _valor;

        // Construtor sem parâmetro para o EF Core
        private TipoVeiculo() { }

        public TipoVeiculo(TipoVeiculoEnum tipoVeiculoEnum)
        {
            if (!Enum.IsDefined(typeof(TipoVeiculoEnum), tipoVeiculoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoVeiculoEnum)));
                throw new DomainException($"Tipo de veículo '{tipoVeiculoEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = tipoVeiculoEnum;
        }

        public TipoVeiculoEnum Valor => _valor;
    }
}
