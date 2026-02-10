using Application.Contracts.Gateways;
using Application.OrdemServico.Interfaces.External;
using Moq;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Application.OrdemServico.Dtos.External;

namespace Tests.Application.SharedHelpers.Gateways
{
    public class OrdemServicoGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;
        private readonly Guid _id;

        public OrdemServicoGatewayObterPorIdSetupBuilder(Mock<IOrdemServicoGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(ordemServico);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((OrdemServicoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewayObterPorCodigoSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;
        private readonly string _codigo;

        public OrdemServicoGatewayObterPorCodigoSetupBuilder(Mock<IOrdemServicoGateway> mock, string codigo)
        {
            _mock = mock;
            _codigo = codigo;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.ObterPorCodigoAsync(_codigo)).ReturnsAsync(ordemServico);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorCodigoAsync(_codigo)).ReturnsAsync((OrdemServicoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorCodigoAsync(_codigo)).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewayObterTodosSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;

        public OrdemServicoGatewayObterTodosSetupBuilder(Mock<IOrdemServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(IEnumerable<OrdemServicoAggregate> ordensServico) => _mock.Setup(g => g.ObterTodosAsync()).ReturnsAsync(ordensServico);

        public void RetornaListaVazia() => _mock.Setup(g => g.ObterTodosAsync()).ReturnsAsync(new List<OrdemServicoAggregate>());

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterTodosAsync()).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewayAtualizarSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;

        public OrdemServicoGatewayAtualizarSetupBuilder(Mock<IOrdemServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).ReturnsAsync(ordemServico);

        public void ComCallback(Action<OrdemServicoAggregate> callback) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).Callback(callback).ReturnsAsync((OrdemServicoAggregate os) => os);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewaySalvarSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;

        public OrdemServicoGatewaySalvarSetupBuilder(Mock<IOrdemServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>())).ReturnsAsync(ordemServico);

        public void ComCallback(Action<OrdemServicoAggregate> callback) => _mock.Setup(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>())).Callback(callback).ReturnsAsync((OrdemServicoAggregate os) => os);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>())).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;

        public EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId)
        {
            _mock = mock;
            _itemId = itemId;
        }

        public void Retorna(ItemEstoqueExternalDto itemEstoque) => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ReturnsAsync(itemEstoque);

        public void NaoRetornaNada() => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ReturnsAsync((ItemEstoqueExternalDto?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;
        private readonly int _quantidade;

        public EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId, int quantidade)
        {
            _mock = mock;
            _itemId = itemId;
            _quantidade = quantidade;
        }

        public void Retorna(bool disponivel) => _mock.Setup(s => s.VerificarDisponibilidadeAsync(_itemId, _quantidade)).ReturnsAsync(disponivel);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.VerificarDisponibilidadeAsync(_itemId, _quantidade)).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceAtualizarQuantidadeSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;
        private readonly int _novaQuantidade;

        public EstoqueExternalServiceAtualizarQuantidadeSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade)
        {
            _mock = mock;
            _itemId = itemId;
            _novaQuantidade = novaQuantidade;
        }

        public void Completa() => _mock.Setup(s => s.AtualizarQuantidadeEstoqueAsync(_itemId, _novaQuantidade)).Returns(Task.CompletedTask);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.AtualizarQuantidadeEstoqueAsync(_itemId, _novaQuantidade)).ThrowsAsync(excecao);
    }

    public class ServicoExternalServiceObterServicoPorIdSetupBuilder
    {
        private readonly Mock<IServicoExternalService> _mock;
        private readonly Guid _servicoId;

        public ServicoExternalServiceObterServicoPorIdSetupBuilder(Mock<IServicoExternalService> mock, Guid servicoId)
        {
            _mock = mock;
            _servicoId = servicoId;
        }

        public void Retorna(ServicoExternalDto servico) => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ReturnsAsync(servico);

        public void NaoRetornaNada() => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ReturnsAsync((ServicoExternalDto?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewayObterEntreguesUltimosDiasSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;
        private readonly int _quantidadeDias;

        public OrdemServicoGatewayObterEntreguesUltimosDiasSetupBuilder(Mock<IOrdemServicoGateway> mock, int quantidadeDias)
        {
            _mock = mock;
            _quantidadeDias = quantidadeDias;
        }

        public void Retorna(IEnumerable<OrdemServicoAggregate> ordensEntregues) => _mock.Setup(g => g.ObterEntreguesUltimosDiasAsync(_quantidadeDias)).ReturnsAsync(ordensEntregues);

        public void RetornaListaVazia() => _mock.Setup(g => g.ObterEntreguesUltimosDiasAsync(_quantidadeDias)).ReturnsAsync(new List<OrdemServicoAggregate>());

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterEntreguesUltimosDiasAsync(_quantidadeDias)).ThrowsAsync(excecao);
    }

    public static class OrdemServicoGatewayMockExtensions
    {
        public static OrdemServicoGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IOrdemServicoGateway> mock, Guid id) => new OrdemServicoGatewayObterPorIdSetupBuilder(mock, id);

        public static OrdemServicoGatewayObterPorCodigoSetupBuilder AoObterPorCodigo(this Mock<IOrdemServicoGateway> mock, string codigo) => new OrdemServicoGatewayObterPorCodigoSetupBuilder(mock, codigo);

        public static OrdemServicoGatewayObterTodosSetupBuilder AoObterTodos(this Mock<IOrdemServicoGateway> mock) => new OrdemServicoGatewayObterTodosSetupBuilder(mock);

        public static OrdemServicoGatewayAtualizarSetupBuilder AoAtualizar(this Mock<IOrdemServicoGateway> mock) => new OrdemServicoGatewayAtualizarSetupBuilder(mock);

        public static OrdemServicoGatewaySalvarSetupBuilder AoSalvar(this Mock<IOrdemServicoGateway> mock) => new OrdemServicoGatewaySalvarSetupBuilder(mock);

        public static OrdemServicoGatewayObterEntreguesUltimosDiasSetupBuilder AoObterEntreguesUltimosDias(this Mock<IOrdemServicoGateway> mock, int quantidadeDias) => new OrdemServicoGatewayObterEntreguesUltimosDiasSetupBuilder(mock, quantidadeDias);

        public static EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder AoObterItemEstoquePorId(this Mock<IEstoqueExternalService> mock, Guid itemId) => new EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder(mock, itemId);

        public static EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder AoVerificarDisponibilidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int quantidade) => new EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder(mock, itemId, quantidade);

        public static EstoqueExternalServiceAtualizarQuantidadeSetupBuilder AoAtualizarQuantidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade) => new EstoqueExternalServiceAtualizarQuantidadeSetupBuilder(mock, itemId, novaQuantidade);

        public static ServicoExternalServiceObterServicoPorIdSetupBuilder AoObterServicoPorId(this Mock<IServicoExternalService> mock, Guid servicoId) => new ServicoExternalServiceObterServicoPorIdSetupBuilder(mock, servicoId);

        public static void DeveTerAtualizadoQuantidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade)
        {
            mock.Verify(s => s.AtualizarQuantidadeEstoqueAsync(itemId, novaQuantidade), Times.Once);
        }
    }

    public class ClienteExternalServiceObterClientePorVeiculoIdSetupBuilder
    {
        private readonly Mock<IClienteExternalService> _mock;
        private readonly Guid _veiculoId;

        public ClienteExternalServiceObterClientePorVeiculoIdSetupBuilder(Mock<IClienteExternalService> mock, Guid veiculoId)
        {
            _mock = mock;
            _veiculoId = veiculoId;
        }

        public void Retorna(ClienteExternalDto cliente) => _mock.Setup(s => s.ObterClientePorVeiculoIdAsync(_veiculoId)).ReturnsAsync(cliente);

        public void NaoRetornaNada() => _mock.Setup(s => s.ObterClientePorVeiculoIdAsync(_veiculoId)).ReturnsAsync((ClienteExternalDto?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.ObterClientePorVeiculoIdAsync(_veiculoId)).ThrowsAsync(excecao);
    }

    public static class ClienteExternalServiceMockExtensions
    {
        public static ClienteExternalServiceObterClientePorVeiculoIdSetupBuilder AoObterClientePorVeiculoId(this Mock<IClienteExternalService> mock, Guid veiculoId) => new ClienteExternalServiceObterClientePorVeiculoIdSetupBuilder(mock, veiculoId);
    }

    public class VeiculoExternalServiceVerificarExistenciaVeiculoSetupBuilder
    {
        private readonly Mock<IVeiculoExternalService> _mock;
        private readonly Guid _veiculoId;

        public VeiculoExternalServiceVerificarExistenciaVeiculoSetupBuilder(Mock<IVeiculoExternalService> mock, Guid veiculoId)
        {
            _mock = mock;
            _veiculoId = veiculoId;
        }

        public void Retorna(bool existe) => _mock.Setup(s => s.VerificarExistenciaVeiculo(_veiculoId)).ReturnsAsync(existe);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.VerificarExistenciaVeiculo(_veiculoId)).ThrowsAsync(excecao);
    }

    public static class VeiculoExternalServiceMockExtensions
    {
        public static VeiculoExternalServiceVerificarExistenciaVeiculoSetupBuilder AoVerificarExistenciaVeiculo(this Mock<IVeiculoExternalService> mock, Guid veiculoId) => new VeiculoExternalServiceVerificarExistenciaVeiculoSetupBuilder(mock, veiculoId);
    }

    public static class OrdemServicoGatewayMockVerifyExtensions
    {
        public static void DeveTerVerificadoExistenciaVeiculo(this Mock<IVeiculoExternalService> mock, Guid veiculoId)
        {
            mock.Verify(v => v.VerificarExistenciaVeiculo(veiculoId), Times.Once);
        }

        public static void DeveTerVerificadoCodigoExistente(this Mock<IOrdemServicoGateway> mock)
        {
            mock.Verify(g => g.ObterPorCodigoAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }

        public static void DeveTerSalvoOrdemServico(this Mock<IOrdemServicoGateway> mock)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        public static void DeveTerObtidoOrdemServicoPorCodigo(this Mock<IOrdemServicoGateway> mock, int vezes = 1)
        {
            mock.Verify(g => g.ObterPorCodigoAsync(It.IsAny<string>()), Times.Exactly(vezes));
        }

        public static void DeveTerSalvadoOrdemServico(this Mock<IOrdemServicoGateway> mock, int vezes = 1)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Exactly(vezes));
        }

        public static void NaoDeveTerSalvoOrdemServico(this Mock<IOrdemServicoGateway> mock)
        {
            mock.Verify(g => g.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }
    }
}