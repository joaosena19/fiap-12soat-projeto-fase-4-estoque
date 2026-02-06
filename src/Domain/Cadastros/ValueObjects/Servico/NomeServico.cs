using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Servico
{
    [ValueObject]
    public record NomeServico
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private NomeServico() { }

        public NomeServico(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome não pode ser vazio", ErrorType.InvalidInput);

            if (nome.Length > 500)
                throw new DomainException("Nome não pode ter mais de 500 caracteres", ErrorType.InvalidInput);

            _valor = nome;
        }

        public string Valor => _valor;


    }
}
