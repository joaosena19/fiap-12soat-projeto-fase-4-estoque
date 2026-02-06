using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Shared.Attributes;
using Shared.Exceptions;
using Shared.Enums;
using UUIDNext;

namespace Domain.Identidade.Aggregates
{
    [AggregateRoot]
    public class Usuario
    {
        public Guid Id { get; private set; }
        public DocumentoIdentificadorUsuario DocumentoIdentificadorUsuario { get; private set; } = null!;
        public SenhaHash SenhaHash { get; private set; } = null!;
        public StatusUsuario Status { get; private set; } = null!;
        private readonly List<Role> _roles = new();
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

        // Construtor sem parâmetro para EF Core
        private Usuario() { }

        private Usuario(Guid id, DocumentoIdentificadorUsuario documentoIdentificadorUsuario, SenhaHash senhaHash, StatusUsuario status, List<Role> roles)
        {
            Id = id;
            DocumentoIdentificadorUsuario = documentoIdentificadorUsuario;
            SenhaHash = senhaHash;
            Status = status;
            _roles.AddRange(roles);
        }

        public static Usuario Criar(string documento, string senhaHash, List<Role> roles)
        {
            if (roles == null || !roles.Any())
                throw new DomainException("Usuário deve ter pelo menos uma role.", ErrorType.InvalidInput);

            return new Usuario(
                Uuid.NewSequential(), 
                new DocumentoIdentificadorUsuario(documento), 
                new SenhaHash(senhaHash),
                StatusUsuario.Ativo(), // Novo usuário sempre criado como ativo
                roles);
        }

        public static Usuario Criar(string documento, string senhaHash, Role role)
        {
            return Criar(documento, senhaHash, [role]);
        }

        public void Ativar()
        {
            Status = StatusUsuario.Ativo();
        }

        public void Inativar()
        {
            Status = StatusUsuario.Inativo();
        }
    }
}