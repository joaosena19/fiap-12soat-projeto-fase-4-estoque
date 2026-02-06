using Domain.Identidade.Enums;
using Shared.Attributes;
using Shared.Exceptions;
using Shared.Enums;

namespace Domain.Identidade.ValueObjects
{
    [ValueObject]
    public record StatusUsuario
    {
        private readonly StatusUsuarioEnum _valor;

        // Construtor sem parâmetro para EF Core
        private StatusUsuario() { }

        public StatusUsuario(StatusUsuarioEnum statusUsuarioEnum)
        {
            if (!Enum.IsDefined(typeof(StatusUsuarioEnum), statusUsuarioEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(StatusUsuarioEnum)));
                throw new DomainException($"Status de usuário '{statusUsuarioEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = statusUsuarioEnum;
        }

        public StatusUsuarioEnum Valor => _valor;

        public static StatusUsuario Ativo() => new(StatusUsuarioEnum.Ativo);
        public static StatusUsuario Inativo() => new(StatusUsuarioEnum.Inativo);

        public bool EstaAtivo() => _valor == StatusUsuarioEnum.Ativo;
        public bool EstaInativo() => _valor == StatusUsuarioEnum.Inativo;
    }
}