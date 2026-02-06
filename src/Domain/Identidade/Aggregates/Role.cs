using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Shared.Attributes;
using Shared.Exceptions;
using Shared.Enums;

namespace Domain.Identidade.Aggregates
{
    [AggregateMember]
    public class Role
    {
        public RoleEnum Id { get; private set; }
        public NomeRole Nome { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private Role() { }

        public Role(RoleEnum roleEnum)
        {
            Id = roleEnum;
            Nome = new NomeRole(roleEnum);
        }

        public Role(string roleString)
        {
            if (string.IsNullOrWhiteSpace(roleString))
                throw new DomainException("Role não pode ser vazio.", ErrorType.InvalidInput);

            // Tenta fazer parse do enum (por nome ou por valor numérico)
            if (!Enum.TryParse<RoleEnum>(roleString, true, out var roleEnum) || !Enum.IsDefined(typeof(RoleEnum), roleEnum))
                throw new DomainException($"Role inválido: {roleString}", ErrorType.InvalidInput);

            Id = roleEnum;
            Nome = new NomeRole(roleEnum);
        }

        public static Role Administrador() => new(RoleEnum.Administrador);
        public static Role Cliente() => new(RoleEnum.Cliente);

        public static Role From(RoleEnum roleEnum) => new(roleEnum);
        public static Role From(string roleString) => new(roleString);
    }
}