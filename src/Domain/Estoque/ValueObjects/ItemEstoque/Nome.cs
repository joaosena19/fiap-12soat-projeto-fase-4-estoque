using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Estoque.ValueObjects.ItemEstoque
{
    [ValueObject]
    public record Nome
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private Nome() { }

        public Nome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome não pode ser vazio", ErrorType.InvalidInput);

            if (nome.Length > 200)
                throw new DomainException("Nome não pode ter mais de 200 caracteres", ErrorType.InvalidInput);

            _valor = nome;
        }

        public string Valor => _valor;
    }
}
