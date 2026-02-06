using Application.Contracts.Gateways;
using Moq;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.SharedHelpers.Gateways
{
    public class ServicoGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IServicoGateway> _mock;
        private readonly Guid _id;

        public ServicoGatewayObterPorIdSetupBuilder(Mock<IServicoGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(ServicoAggregate servico) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(servico);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((ServicoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class ServicoGatewayAtualizarSetupBuilder
    {
        private readonly Mock<IServicoGateway> _mock;

        public ServicoGatewayAtualizarSetupBuilder(Mock<IServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(Func<ServicoAggregate, ServicoAggregate> func) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ServicoAggregate>())).ReturnsAsync(func);

        public void RetornaOMesmoServico() => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ServicoAggregate>())).ReturnsAsync((ServicoAggregate servico) => servico);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ServicoAggregate>())).ThrowsAsync(excecao);

        public void ComCallback(Action<ServicoAggregate> callback)
        {
            _mock.Setup(g => g.AtualizarAsync(It.IsAny<ServicoAggregate>()))
                .Callback(callback)
                .ReturnsAsync((ServicoAggregate servico) => servico);
        }
    }

    public class ServicoGatewaySalvarSetupBuilder
    {
        private readonly Mock<IServicoGateway> _mock;

        public ServicoGatewaySalvarSetupBuilder(Mock<IServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(ServicoAggregate servico) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ServicoAggregate>())).ReturnsAsync(servico);

        public void ComCallback(Action<ServicoAggregate> callback) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ServicoAggregate>())).Callback(callback).ReturnsAsync((ServicoAggregate servico) => servico);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ServicoAggregate>())).ThrowsAsync(excecao);
    }

    public class ServicoGatewayObterPorNomeSetupBuilder
    {
        private readonly Mock<IServicoGateway> _mock;
        private readonly string _nome;

        public ServicoGatewayObterPorNomeSetupBuilder(Mock<IServicoGateway> mock, string nome)
        {
            _mock = mock;
            _nome = nome;
        }

        public void Retorna(ServicoAggregate servico) => _mock.Setup(g => g.ObterPorNomeAsync(_nome)).ReturnsAsync(servico);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorNomeAsync(_nome)).ReturnsAsync((ServicoAggregate?)null);
    }

    public class ServicoGatewayObterTodosSetupBuilder
    {
        private readonly Mock<IServicoGateway> _mock;

        public ServicoGatewayObterTodosSetupBuilder(Mock<IServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(IEnumerable<ServicoAggregate> servicos) => _mock.Setup(g => g.ObterTodosAsync()).ReturnsAsync(servicos);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterTodosAsync()).ThrowsAsync(excecao);
    }

    public static class ServicoGatewayMockExtensions
    {
        public static ServicoGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IServicoGateway> mock, Guid id) => new ServicoGatewayObterPorIdSetupBuilder(mock, id);

        public static ServicoGatewayObterPorNomeSetupBuilder AoObterPorNome(this Mock<IServicoGateway> mock, string nome) => new ServicoGatewayObterPorNomeSetupBuilder(mock, nome);

        public static ServicoGatewayAtualizarSetupBuilder AoAtualizar(this Mock<IServicoGateway> mock) => new ServicoGatewayAtualizarSetupBuilder(mock);

        public static ServicoGatewaySalvarSetupBuilder AoSalvar(this Mock<IServicoGateway> mock) => new ServicoGatewaySalvarSetupBuilder(mock);

        public static ServicoGatewayObterTodosSetupBuilder AoObterTodos(this Mock<IServicoGateway> mock) => new ServicoGatewayObterTodosSetupBuilder(mock);
    }

    public static class ServicoGatewayMockVerifyExtensions
    {
        public static void DeveTerObtidoServicoPorId(this Mock<IServicoGateway> mock, Guid servicoId, int vezes = 1)
        {
            mock.Verify(g => g.ObterPorIdAsync(servicoId), Times.Exactly(vezes));
        }
    }
}