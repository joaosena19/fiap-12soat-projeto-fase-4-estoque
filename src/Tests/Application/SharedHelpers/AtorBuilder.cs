using Application.Identidade.Services;
using Domain.Identidade.Enums;

namespace Tests.Application.SharedHelpers
{
    public class AtorBuilder
    {
        private Guid _userId = Guid.NewGuid();
        private RoleEnum _role = RoleEnum.Administrador;
        private Guid? _clienteId = null;

        public AtorBuilder ComUsuario(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public AtorBuilder ComoAdministrador()
        {
            _role = RoleEnum.Administrador;
            _clienteId = null;
            return this;
        }

        public AtorBuilder ComoCliente(Guid clienteId)
        {
            _role = RoleEnum.Cliente;
            _clienteId = clienteId;
            return this;
        }

        public AtorBuilder ComoSistema()
        {
            _role = RoleEnum.Sistema;
            _clienteId = null;
            return this;
        }

        public Ator Build()
        {
            if (_role == RoleEnum.Cliente && !_clienteId.HasValue)
                _clienteId = Guid.NewGuid();

            return _role switch
            {
                RoleEnum.Administrador => Ator.Administrador(_userId),
                RoleEnum.Cliente => Ator.Cliente(_userId, _clienteId!.Value),
                RoleEnum.Sistema => Ator.Sistema(),
                _ => throw new ArgumentException($"Role não suportada: {_role}")
            };
        }


    }
}