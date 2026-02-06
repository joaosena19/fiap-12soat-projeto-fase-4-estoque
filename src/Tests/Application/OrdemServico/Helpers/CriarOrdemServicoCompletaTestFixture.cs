using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.UseCases;
using Application.Contracts.Monitoramento;
using Moq;

namespace Tests.Application.OrdemServico.Helpers
{
    public class CriarOrdemServicoCompletaTestFixture
    {
        public Mock<IOrdemServicoGateway> OrdemServicoGatewayMock { get; }
        public Mock<IClienteGateway> ClienteGatewayMock { get; }
        public Mock<IVeiculoGateway> VeiculoGatewayMock { get; }
        public Mock<IServicoGateway> ServicoGatewayMock { get; }
        public Mock<IItemEstoqueGateway> ItemEstoqueGatewayMock { get; }
        public Mock<ICriarOrdemServicoCompletaPresenter> PresenterMock { get; }
        public Mock<IMetricsService> MetricsServiceMock { get; }

        public CriarOrdemServicoCompletaUseCase UseCase { get; }

        public CriarOrdemServicoCompletaTestFixture()
        {
            OrdemServicoGatewayMock = new Mock<IOrdemServicoGateway>();
            ClienteGatewayMock = new Mock<IClienteGateway>();
            VeiculoGatewayMock = new Mock<IVeiculoGateway>();
            ServicoGatewayMock = new Mock<IServicoGateway>();
            ItemEstoqueGatewayMock = new Mock<IItemEstoqueGateway>();
            PresenterMock = new Mock<ICriarOrdemServicoCompletaPresenter>();
            MetricsServiceMock = new Mock<IMetricsService>();

            UseCase = new CriarOrdemServicoCompletaUseCase();
        }
    }
}