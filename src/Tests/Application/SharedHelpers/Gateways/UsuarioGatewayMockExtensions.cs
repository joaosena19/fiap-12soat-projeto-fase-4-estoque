using Application.Contracts.Gateways;
using Domain.Identidade.Aggregates;
using Moq;

namespace Tests.Application.SharedHelpers.Gateways
{
    public class UsuarioGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IUsuarioGateway> _mock;
        private readonly Guid _id;

        public UsuarioGatewayObterPorIdSetupBuilder(Mock<IUsuarioGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(Usuario? usuario) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(usuario);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((Usuario?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class UsuarioGatewayObterPorDocumentoSetupBuilder
    {
        private readonly Mock<IUsuarioGateway> _mock;
        private readonly string _documento;

        public UsuarioGatewayObterPorDocumentoSetupBuilder(Mock<IUsuarioGateway> mock, string documento)
        {
            _mock = mock;
            _documento = documento;
        }

        public void Retorna(Usuario? usuario) => _mock.Setup(g => g.ObterPorDocumentoAsync(_documento)).ReturnsAsync(usuario);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorDocumentoAsync(_documento)).ReturnsAsync((Usuario?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorDocumentoAsync(_documento)).ThrowsAsync(excecao);
    }

    public class UsuarioGatewayObterRolesSetupBuilder
    {
        private readonly Mock<IUsuarioGateway> _mock;
        private readonly IEnumerable<string> _roleStrings;

        public UsuarioGatewayObterRolesSetupBuilder(Mock<IUsuarioGateway> mock, IEnumerable<string> roleStrings)
        {
            _mock = mock;
            _roleStrings = roleStrings;
        }

        public void Retorna(List<Role> roles) => _mock.Setup(g => g.ObterRolesAsync(_roleStrings)).ReturnsAsync(roles);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterRolesAsync(_roleStrings)).ThrowsAsync(excecao);
    }

    public class UsuarioGatewaySalvarSetupBuilder
    {
        private readonly Mock<IUsuarioGateway> _mock;

        public UsuarioGatewaySalvarSetupBuilder(Mock<IUsuarioGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(Usuario usuario) => _mock.Setup(g => g.SalvarAsync(It.IsAny<Usuario>())).ReturnsAsync(usuario);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.SalvarAsync(It.IsAny<Usuario>())).ThrowsAsync(excecao);
    }

    public static class UsuarioGatewayMockExtensions
    {
        public static UsuarioGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IUsuarioGateway> mock, Guid id) => new UsuarioGatewayObterPorIdSetupBuilder(mock, id);

        public static UsuarioGatewayObterPorDocumentoSetupBuilder AoObterPorDocumento(this Mock<IUsuarioGateway> mock, string documento) => new UsuarioGatewayObterPorDocumentoSetupBuilder(mock, documento);

        public static UsuarioGatewayObterRolesSetupBuilder AoObterRoles(this Mock<IUsuarioGateway> mock, IEnumerable<string> roleStrings) => new UsuarioGatewayObterRolesSetupBuilder(mock, roleStrings);

        public static UsuarioGatewaySalvarSetupBuilder AoSalvar(this Mock<IUsuarioGateway> mock) => new UsuarioGatewaySalvarSetupBuilder(mock);
    }

    public static class UsuarioGatewayMockVerifyExtensions
    {
        public static void DeveTerSalvadoUsuario(this Mock<IUsuarioGateway> mock, int vezes = 1)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<Usuario>()), Times.Exactly(vezes));
        }

        public static void DeveTerObtidoUsuarioPorDocumento(this Mock<IUsuarioGateway> mock, string documento, int vezes = 1)
        {
            mock.Verify(g => g.ObterPorDocumentoAsync(documento), Times.Exactly(vezes));
        }

        public static void NaoDeveTerSalvadoUsuario(this Mock<IUsuarioGateway> mock)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }
}