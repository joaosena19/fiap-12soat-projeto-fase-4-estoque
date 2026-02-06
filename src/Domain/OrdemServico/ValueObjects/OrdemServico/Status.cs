using Domain.OrdemServico.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.OrdemServico
{
    [ValueObject]
    public record Status
    {
        private readonly StatusOrdemServicoEnum _valor;

        // Construtor sem parâmetro para o EF Core
        private Status() { }

        public Status(StatusOrdemServicoEnum statusOrdemServicoEnum)
        {
            if (!Enum.IsDefined(typeof(StatusOrdemServicoEnum), statusOrdemServicoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(StatusOrdemServicoEnum)));
                throw new DomainException($"Status da Ordem de Serviço '{statusOrdemServicoEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = statusOrdemServicoEnum;
        }

        public StatusOrdemServicoEnum Valor => _valor;
    }
}
