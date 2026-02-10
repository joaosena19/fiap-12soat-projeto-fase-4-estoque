using Application.Identidade.Services;
// Note: RoleEnum removed - using Ator factory methods directly without Domain.Identidade dependency

namespace Tests.Application.SharedHelpers
{
    public class AtorBuilder
    {
        private Guid _userId = Guid.NewGuid();
        private string _tipoAtor = "Administrador"; // "Administrador", "Cliente", "Sistema"
        private Guid? _clienteId = null;

        public AtorBuilder ComUsuario(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public AtorBuilder ComoAdministrador()
        {
            _tipoAtor = "Administrador";
            _clienteId = null;
            return this;
        }

        public AtorBuilder ComoCliente(Guid clienteId)
        {
            _tipoAtor = "Cliente";
            _clienteId = clienteId;
            return this;
        }

        public AtorBuilder ComoSistema()
        {
            _tipoAtor = "Sistema";
            _clienteId = null;
            return this;
        }

        public Ator Build()
        {
            if (_tipoAtor == "Cliente" && !_clienteId.HasValue)
                _clienteId = Guid.NewGuid();

            return _tipoAtor switch
            {
                "Administrador" => Ator.Administrador(_userId),
                "Cliente" => Ator.Cliente(_userId, _clienteId!.Value),
                "Sistema" => Ator.Sistema(),
                _ => throw new ArgumentException($"Tipo de ator não suportado: {_tipoAtor}")
            };
        }


    }
}