using Domain.Identidade.Enums;
using Shared.Attributes;

namespace Domain.Identidade.ValueObjects
{
    [ValueObject]
    public record NomeRole
    {
        public string Valor { get; private init; } = string.Empty;

        // Construtor sem parâmetro para EF Core
        private NomeRole() { }

        public NomeRole(RoleEnum roleEnum)
        {
            Valor = roleEnum.ToString();
        }
    }
}