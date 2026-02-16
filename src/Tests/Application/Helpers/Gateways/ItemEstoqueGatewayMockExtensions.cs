using Application.Contracts.Gateways;
using Moq;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.SharedHelpers.Gateways
{
    public class ItemEstoqueGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IItemEstoqueGateway> _mock;
        private readonly Guid _id;

        public ItemEstoqueGatewayObterPorIdSetupBuilder(Mock<IItemEstoqueGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(ItemEstoqueAggregate itemEstoque) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(itemEstoque);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((ItemEstoqueAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class ItemEstoqueGatewayObterPorNomeSetupBuilder
    {
        private readonly Mock<IItemEstoqueGateway> _mock;
        private readonly string _nome;

        public ItemEstoqueGatewayObterPorNomeSetupBuilder(Mock<IItemEstoqueGateway> mock, string nome)
        {
            _mock = mock;
            _nome = nome;
        }

        public void Retorna(ItemEstoqueAggregate itemEstoque) => _mock.Setup(g => g.ObterPorNomeAsync(_nome)).ReturnsAsync(itemEstoque);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorNomeAsync(_nome)).ReturnsAsync((ItemEstoqueAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorNomeAsync(_nome)).ThrowsAsync(excecao);
    }

    public class ItemEstoqueGatewayAtualizarSetupBuilder
    {
        private readonly Mock<IItemEstoqueGateway> _mock;

        public ItemEstoqueGatewayAtualizarSetupBuilder(Mock<IItemEstoqueGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(Func<ItemEstoqueAggregate, ItemEstoqueAggregate> func) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ItemEstoqueAggregate>())).ReturnsAsync(func);

        public void RetornaOMesmoItemEstoque() => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ItemEstoqueAggregate>())).ReturnsAsync((ItemEstoqueAggregate itemEstoque) => itemEstoque);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ItemEstoqueAggregate>())).ThrowsAsync(excecao);

        public void ComCallback(Action<ItemEstoqueAggregate> callback) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<ItemEstoqueAggregate>())).Callback(callback).ReturnsAsync((ItemEstoqueAggregate itemEstoque) => itemEstoque);
    }

    public class ItemEstoqueGatewaySalvarSetupBuilder
    {
        private readonly Mock<IItemEstoqueGateway> _mock;

        public ItemEstoqueGatewaySalvarSetupBuilder(Mock<IItemEstoqueGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(ItemEstoqueAggregate itemEstoque) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ItemEstoqueAggregate>())).ReturnsAsync(itemEstoque);

        public void ComCallback(Action<ItemEstoqueAggregate> callback) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ItemEstoqueAggregate>())).Callback(callback).ReturnsAsync((ItemEstoqueAggregate itemEstoque) => itemEstoque);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.SalvarAsync(It.IsAny<ItemEstoqueAggregate>())).ThrowsAsync(excecao);
    }

    public class ItemEstoqueGatewayObterTodosSetupBuilder
    {
        private readonly Mock<IItemEstoqueGateway> _mock;

        public ItemEstoqueGatewayObterTodosSetupBuilder(Mock<IItemEstoqueGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(IEnumerable<ItemEstoqueAggregate> itensEstoque) => _mock.Setup(g => g.ObterTodosAsync()).ReturnsAsync(itensEstoque);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterTodosAsync()).ThrowsAsync(excecao);
    }

    public static class ItemEstoqueGatewayMockExtensions
    {
        public static ItemEstoqueGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IItemEstoqueGateway> mock, Guid id) => new ItemEstoqueGatewayObterPorIdSetupBuilder(mock, id);

        public static ItemEstoqueGatewayObterPorNomeSetupBuilder AoObterPorNome(this Mock<IItemEstoqueGateway> mock, string nome) => new ItemEstoqueGatewayObterPorNomeSetupBuilder(mock, nome);

        public static ItemEstoqueGatewayAtualizarSetupBuilder AoAtualizar(this Mock<IItemEstoqueGateway> mock) => new ItemEstoqueGatewayAtualizarSetupBuilder(mock);

        public static ItemEstoqueGatewaySalvarSetupBuilder AoSalvar(this Mock<IItemEstoqueGateway> mock) => new ItemEstoqueGatewaySalvarSetupBuilder(mock);

        public static ItemEstoqueGatewayObterTodosSetupBuilder AoObterTodos(this Mock<IItemEstoqueGateway> mock) => new ItemEstoqueGatewayObterTodosSetupBuilder(mock);
    }

    public static class ItemEstoqueGatewayMockVerifyExtensions
    {
        public static void DeveTerObtidoItemPorId(this Mock<IItemEstoqueGateway> mock, Guid itemId, int vezes = 1)
        {
            mock.Verify(g => g.ObterPorIdAsync(itemId), Times.Exactly(vezes));
        }
    }
}