using Application.Contracts.Monitoramento;
using Moq;

namespace Tests.Application.SharedHelpers
{
    public class MockLogger
    {
        public Mock<IAppLogger> Mock { get; private set; }
        public IAppLogger Object => Mock.Object;

        public MockLogger()
        {
            Mock = new Mock<IAppLogger>();
            ConfigurarComportamentoBasico();
        }

        private void ConfigurarComportamentoBasico()
        {
            // Setup básico que permite verificação depois
            Mock.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
            Mock.Setup(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()));
            Mock.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()));
            Mock.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));

            // ComPropriedade retorna ele mesmo para manter fluency
            Mock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Mock.Object);
        }

        private void ConfigurarMockRecursivo(Mock<IAppLogger> mockToSetup)
        {
            // Configura os métodos de log no mock da cadeia
            mockToSetup.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
            mockToSetup.Setup(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()));
            mockToSetup.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()));
            mockToSetup.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));
            
            // ComPropriedade também pode ser chamado no mock da cadeia
            var nextMockChain = new Mock<IAppLogger>();
            ConfigurarMockRecursivo(nextMockChain);
            mockToSetup.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(nextMockChain.Object);
        }

        // Métodos estáticos para compatibilidade com código existente
        public static IAppLogger CriarSimples() => new MockLogger().Object;
        
        public static MockLogger Criar() => new MockLogger();

        // Métodos de verificação fluentes - apenas verificam se foram chamados
        public void DeveTerLogadoInformation()
        {
            Mock.Verify(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        public void DeveTerLogadoWarning()
        {
            Mock.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        public void DeveTerLogadoError()
        {
            // Verifica se foi chamado LogError com string ou com exception
            try
            {
                Mock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
            }
            catch (Moq.MockException)
            {
                // Se não foi chamado com string, verifica se foi chamado com exception
                Mock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
            }
        }

        public void DeveTerLogadoErrorComException()
        {
            Mock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        public void DeveTerChamadoComPropriedade(string chaveEsperada, object valorEsperado)
        {
            Mock.Verify(x => x.ComPropriedade(chaveEsperada, valorEsperado), Times.Once);
        }

        public void NaoDeveTerLogadoNenhumError()
        {
            Mock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
            Mock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }

        public void NaoDeveTerLogadoNenhumWarning()
        {
            Mock.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }
    }
}