using Domain.Identidade.Aggregates;
using Tests.Helpers;

namespace Tests.Application.SharedHelpers.AggregateBuilders
{
    public class UsuarioBuilder
    {
        private string _documento = DocumentoHelper.GerarCpfValido();
        private string _senhaHash = "$argon2id$v=19$m=65536,t=4,p=1$abcdefghijklmnop$1234567890abcdef1234567890abcdef";
        private List<Role> _roles = new() { Role.Cliente() };

        public UsuarioBuilder ComDocumento(string documento)
        {
            _documento = documento;
            return this;
        }

        public UsuarioBuilder ComSenhaHash(string senhaHash)
        {
            _senhaHash = senhaHash;
            return this;
        }

        public UsuarioBuilder ComRoles(params Role[] roles)
        {
            _roles = roles.ToList();
            return this;
        }

        public UsuarioBuilder ComRoleAdministrador()
        {
            _roles = new List<Role> { Role.Administrador() };
            return this;
        }

        public UsuarioBuilder ComRoleCliente()
        {
            _roles = new List<Role> { Role.Cliente() };
            return this;
        }

        public Usuario Build()
        {
            return Usuario.Criar(_documento, _senhaHash, _roles);
        }
    }
}