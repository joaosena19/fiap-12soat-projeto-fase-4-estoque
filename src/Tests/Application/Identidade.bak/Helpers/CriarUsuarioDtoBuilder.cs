using Application.Identidade.Dtos;
using Tests.Helpers;

namespace Tests.Application.Identidade.Helpers
{
    public class CriarUsuarioDtoBuilder
    {
        private string _documentoIdentificador = DocumentoHelper.GerarCpfValido();
        private string _senhaNaoHasheada = "senha123";
        private List<string> _roles = new() { "Cliente" };

        public CriarUsuarioDtoBuilder ComDocumento(string documento)
        {
            _documentoIdentificador = documento;
            return this;
        }

        public CriarUsuarioDtoBuilder ComSenhaNaoHasheada(string senha)
        {
            _senhaNaoHasheada = senha;
            return this;
        }

        public CriarUsuarioDtoBuilder ComRoles(params string[] roles)
        {
            _roles = roles.ToList();
            return this;
        }

        public CriarUsuarioDtoBuilder ComRoleAdministrador()
        {
            _roles = new List<string> { "Administrador" };
            return this;
        }

        public CriarUsuarioDtoBuilder ComRoleCliente()
        {
            _roles = new List<string> { "Cliente" };
            return this;
        }

        public CriarUsuarioDtoBuilder ComMultiplasRoles()
        {
            _roles = new List<string> { "Administrador", "Cliente" };
            return this;
        }

        public CriarUsuarioDtoBuilder ComRolesVazia()
        {
            _roles = new List<string>();
            return this;
        }

        public CriarUsuarioDto Build()
        {
            return new CriarUsuarioDto
            {
                DocumentoIdentificador = _documentoIdentificador,
                SenhaNaoHasheada = _senhaNaoHasheada,
                Roles = _roles
            };
        }
    }
}