using Application.Contracts.Gateways;
using Moq;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.SharedHelpers.Gateways
{
    public class VeiculoGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;
        private readonly Guid _id;

        public VeiculoGatewayObterPorIdSetupBuilder(Mock<IVeiculoGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(VeiculoAggregate veiculo) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(veiculo);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((VeiculoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class VeiculoGatewayObterPorPlacaSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;
        private readonly string _placa;

        public VeiculoGatewayObterPorPlacaSetupBuilder(Mock<IVeiculoGateway> mock, string placa)
        {
            _mock = mock;
            _placa = placa;
        }

        public void Retorna(VeiculoAggregate veiculo) => _mock.Setup(g => g.ObterPorPlacaAsync(_placa)).ReturnsAsync(veiculo);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorPlacaAsync(_placa)).ReturnsAsync((VeiculoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorPlacaAsync(_placa)).ThrowsAsync(excecao);
    }

    public class VeiculoGatewayObterPorClienteIdSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;
        private readonly Guid _clienteId;

        public VeiculoGatewayObterPorClienteIdSetupBuilder(Mock<IVeiculoGateway> mock, Guid clienteId)
        {
            _mock = mock;
            _clienteId = clienteId;
        }

        public void Retorna(IEnumerable<VeiculoAggregate> veiculos) => _mock.Setup(g => g.ObterPorClienteIdAsync(_clienteId)).ReturnsAsync(veiculos);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorClienteIdAsync(_clienteId)).ThrowsAsync(excecao);
    }

    public class VeiculoGatewayAtualizarSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;

        public VeiculoGatewayAtualizarSetupBuilder(Mock<IVeiculoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(Func<VeiculoAggregate, VeiculoAggregate> func) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<VeiculoAggregate>())).ReturnsAsync(func);

        public void RetornaOMesmoVeiculo() => _mock.Setup(g => g.AtualizarAsync(It.IsAny<VeiculoAggregate>())).ReturnsAsync((VeiculoAggregate veiculo) => veiculo);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<VeiculoAggregate>())).ThrowsAsync(excecao);

        public void ComCallback(Action<VeiculoAggregate> callback)
        {
            _mock.Setup(g => g.AtualizarAsync(It.IsAny<VeiculoAggregate>()))
                .Callback(callback)
                .ReturnsAsync((VeiculoAggregate veiculo) => veiculo);
        }
    }

    public class VeiculoGatewaySalvarSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;

        public VeiculoGatewaySalvarSetupBuilder(Mock<IVeiculoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(VeiculoAggregate veiculo) => _mock.Setup(g => g.SalvarAsync(It.IsAny<VeiculoAggregate>())).ReturnsAsync(veiculo);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.SalvarAsync(It.IsAny<VeiculoAggregate>())).ThrowsAsync(excecao);
    }

    public class VeiculoGatewayObterTodosSetupBuilder
    {
        private readonly Mock<IVeiculoGateway> _mock;

        public VeiculoGatewayObterTodosSetupBuilder(Mock<IVeiculoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(IEnumerable<VeiculoAggregate> veiculos) => _mock.Setup(g => g.ObterTodosAsync()).ReturnsAsync(veiculos);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterTodosAsync()).ThrowsAsync(excecao);
    }

    public static class VeiculoGatewayMockExtensions
    {
        public static VeiculoGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IVeiculoGateway> mock, Guid id) => new VeiculoGatewayObterPorIdSetupBuilder(mock, id);

        public static VeiculoGatewayObterPorPlacaSetupBuilder AoObterPorPlaca(this Mock<IVeiculoGateway> mock, string placa) => new VeiculoGatewayObterPorPlacaSetupBuilder(mock, placa);

        public static VeiculoGatewayObterPorClienteIdSetupBuilder AoObterPorClienteId(this Mock<IVeiculoGateway> mock, Guid clienteId) => new VeiculoGatewayObterPorClienteIdSetupBuilder(mock, clienteId);

        public static VeiculoGatewayAtualizarSetupBuilder AoAtualizar(this Mock<IVeiculoGateway> mock) => new VeiculoGatewayAtualizarSetupBuilder(mock);

        public static VeiculoGatewaySalvarSetupBuilder AoSalvar(this Mock<IVeiculoGateway> mock) => new VeiculoGatewaySalvarSetupBuilder(mock);

        public static VeiculoGatewayObterTodosSetupBuilder AoObterTodos(this Mock<IVeiculoGateway> mock) => new VeiculoGatewayObterTodosSetupBuilder(mock);
    }

    public static class VeiculoGatewayMockVerifyExtensions
    {
        public static void DeveTerSalvadoVeiculo(this Mock<IVeiculoGateway> mock, int vezes = 1)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<VeiculoAggregate>()), Times.Exactly(vezes));
        }

        public static void DeveTerObtidoVeiculoPorPlaca(this Mock<IVeiculoGateway> mock, string placa, int vezes = 1)
        {
            mock.Verify(g => g.ObterPorPlacaAsync(placa), Times.Exactly(vezes));
        }

        public static void NaoDeveTerSalvadoVeiculo(this Mock<IVeiculoGateway> mock)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<VeiculoAggregate>()), Times.Never);
        }
    }
}