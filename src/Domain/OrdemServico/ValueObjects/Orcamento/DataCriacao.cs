using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.Orcamento
{
    [ValueObject]
    public record DataCriacao
    {
        private readonly DateTime _valor;

        // Construtor sem parâmetro para EF Core
        private DataCriacao() { }

        public DataCriacao(DateTime dataCriacao)
        {
            if (dataCriacao == default)
                throw new DomainException("Data de criação não pode ser vazia", ErrorType.InvalidInput);

            _valor = dataCriacao;
        }

        public DateTime Valor => _valor;
    }
}
