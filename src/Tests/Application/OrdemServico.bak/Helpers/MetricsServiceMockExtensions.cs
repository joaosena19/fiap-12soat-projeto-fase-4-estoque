using Application.Contracts.Monitoramento;
using Moq;

namespace Tests.Application.OrdemServico.Helpers
{
    public class MetricsServiceRegistrarOrdemServicoCriadaSetupBuilder
    {
        private readonly Mock<IMetricsService> _mock;

        public MetricsServiceRegistrarOrdemServicoCriadaSetupBuilder(Mock<IMetricsService> mock)
        {
            _mock = mock;
        }

        public void LancaExcecao(Exception excecao) => _mock.Setup(m => m.RegistrarOrdemServicoCriada(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Throws(excecao);
    }

    public class MetricsServiceRegistrarMudancaOrdemServicoStatusSetupBuilder
    {
        private readonly Mock<IMetricsService> _mock;

        public MetricsServiceRegistrarMudancaOrdemServicoStatusSetupBuilder(Mock<IMetricsService> mock)
        {
            _mock = mock;
        }

        public void LancaExcecao(Exception excecao) => _mock.Setup(m => m.RegistrarMudancaOrdemServicoStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>())).Throws(excecao);
    }

    public static class MetricsServiceMockExtensions
    {
        public static MetricsServiceRegistrarOrdemServicoCriadaSetupBuilder AoRegistrarOrdemServicoCriada(this Mock<IMetricsService> mock)
        {
            return new MetricsServiceRegistrarOrdemServicoCriadaSetupBuilder(mock);
        }

        public static MetricsServiceRegistrarMudancaOrdemServicoStatusSetupBuilder AoRegistrarMudancaOrdemServicoStatus(this Mock<IMetricsService> mock)
        {
            return new MetricsServiceRegistrarMudancaOrdemServicoStatusSetupBuilder(mock);
        }
        public static void DeveTerRegistradoOrdemServicoCriada(this Mock<IMetricsService> mock, Guid ordemServicoId, Guid clienteId, Guid usuarioId)
        {
            mock.Verify(m => m.RegistrarOrdemServicoCriada(ordemServicoId, clienteId, usuarioId), Times.Once,
                $"Era esperado que o método RegistrarOrdemServicoCriada fosse chamado exatamente uma vez com OrdemServicoId '{ordemServicoId}', ClienteId '{clienteId}' e UsuarioId '{usuarioId}'.");
        }

        public static void DeveTerRegistradoOrdemServicoCriada(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarOrdemServicoCriada(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once,
                "Era esperado que o método RegistrarOrdemServicoCriada fosse chamado exatamente uma vez.");
        }

        public static void NaoDeveTerRegistradoOrdemServicoCriada(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarOrdemServicoCriada(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never,
                "O método RegistrarOrdemServicoCriada não deveria ter sido chamado.");
        }

        public static void DeveTerRegistradoMudancaOrdemServicoStatus(this Mock<IMetricsService> mock, Guid ordemServicoId, string statusAnterior, string statusNovo, double duracaoMs)
        {
            mock.Verify(m => m.RegistrarMudancaOrdemServicoStatus(ordemServicoId, statusAnterior, statusNovo, duracaoMs), Times.Once,
                $"Era esperado que o método RegistrarMudancaOrdemServicoStatus fosse chamado exatamente uma vez com OrdemServicoId '{ordemServicoId}', StatusAnterior '{statusAnterior}', StatusNovo '{statusNovo}' e DuracaoMs '{duracaoMs}'.");
        }

        public static void DeveTerRegistradoMudancaOrdemServicoStatus(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarMudancaOrdemServicoStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Once,
                "Era esperado que o método RegistrarMudancaOrdemServicoStatus fosse chamado exatamente uma vez.");
        }

        public static void NaoDeveTerRegistradoMudancaOrdemServicoStatus(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarMudancaOrdemServicoStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never,
                "O método RegistrarMudancaOrdemServicoStatus não deveria ter sido chamado.");
        }

        public static void DeveTerRegistradoAlgumaMetrica(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarOrdemServicoCriada(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.AtLeastOnce,
                "Era esperado que pelo menos um método de métrica fosse chamado.");
        }

        public static void NaoDeveTerRegistradoNenhumaMetrica(this Mock<IMetricsService> mock)
        {
            mock.Verify(m => m.RegistrarOrdemServicoCriada(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never,
                "Nenhum método de métrica deveria ter sido chamado.");
            mock.Verify(m => m.RegistrarMudancaOrdemServicoStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Never,
                "Nenhum método de métrica deveria ter sido chamado.");
        }
    }
}