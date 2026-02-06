using Domain.Identidade.Enums;

namespace Application.Identidade.Services;

public class Ator
{
    public Guid UsuarioId { get; private set; }
    public Guid? ClienteId { get; private set; }
    public IReadOnlyList<RoleEnum> Roles { get; private set; }

    private Ator(Guid usuarioId, Guid? clienteId, List<RoleEnum> roles)
    {
        UsuarioId = usuarioId;
        ClienteId = clienteId;
        Roles = roles.AsReadOnly();
    }

    public static Ator Administrador(Guid usuarioId) => new(usuarioId, null, [RoleEnum.Administrador]);

    public static Ator Cliente(Guid usuarioId, Guid clienteId) => new(usuarioId, clienteId, [RoleEnum.Cliente]);

    public static Ator Sistema() => new(Guid.Empty, null, [RoleEnum.Sistema]);

    public static Ator ComRoles(Guid usuarioId, Guid? clienteId, List<RoleEnum> roles) => new(usuarioId, clienteId, roles);

    // Métodos de validação básicos
    public bool PodeGerenciarSistema() => Roles.Contains(RoleEnum.Administrador);
    public bool EhCliente() => Roles.Contains(RoleEnum.Cliente);
    public bool PodeAcionarWebhooks() => Roles.Contains(RoleEnum.Sistema);
}