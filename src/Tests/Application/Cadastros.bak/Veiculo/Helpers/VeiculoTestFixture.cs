using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Moq;

namespace Tests.Application.Cadastros.Veiculo.Helpers
{
    public class VeiculoTestFixture
    {
        public Mock<IVeiculoGateway> VeiculoGatewayMock { get; }
        public Mock<IClienteGateway> ClienteGatewayMock { get; }
        public Mock<ICriarVeiculoPresenter> CriarVeiculoPresenterMock { get; }
        public Mock<IBuscarVeiculosPresenter> BuscarVeiculosPresenterMock { get; }
        public Mock<IBuscarVeiculosPorClientePresenter> BuscarVeiculosPorClientePresenterMock { get; }
        public Mock<IBuscarVeiculoPorPlacaPresenter> BuscarVeiculoPorPlacaPresenterMock { get; }
        public Mock<IBuscarVeiculoPorIdPresenter> BuscarVeiculoPorIdPresenterMock { get; }
        public Mock<IAtualizarVeiculoPresenter> AtualizarVeiculoPresenterMock { get; }

        public CriarVeiculoUseCase CriarVeiculoUseCase { get; }
        public BuscarVeiculosUseCase BuscarVeiculosUseCase { get; }
        public BuscarVeiculosPorClienteUseCase BuscarVeiculosPorClienteUseCase { get; }
        public BuscarVeiculoPorPlacaUseCase BuscarVeiculoPorPlacaUseCase { get; }
        public BuscarVeiculoPorIdUseCase BuscarVeiculoPorIdUseCase { get; }
        public AtualizarVeiculoUseCase AtualizarVeiculoUseCase { get; }

        public VeiculoTestFixture()
        {
            VeiculoGatewayMock = new Mock<IVeiculoGateway>();
            ClienteGatewayMock = new Mock<IClienteGateway>();
            CriarVeiculoPresenterMock = new Mock<ICriarVeiculoPresenter>();
            BuscarVeiculosPresenterMock = new Mock<IBuscarVeiculosPresenter>();
            BuscarVeiculosPorClientePresenterMock = new Mock<IBuscarVeiculosPorClientePresenter>();
            BuscarVeiculoPorPlacaPresenterMock = new Mock<IBuscarVeiculoPorPlacaPresenter>();
            BuscarVeiculoPorIdPresenterMock = new Mock<IBuscarVeiculoPorIdPresenter>();
            AtualizarVeiculoPresenterMock = new Mock<IAtualizarVeiculoPresenter>();

            CriarVeiculoUseCase = new CriarVeiculoUseCase();
            BuscarVeiculosUseCase = new BuscarVeiculosUseCase();
            BuscarVeiculosPorClienteUseCase = new BuscarVeiculosPorClienteUseCase();
            BuscarVeiculoPorPlacaUseCase = new BuscarVeiculoPorPlacaUseCase();
            BuscarVeiculoPorIdUseCase = new BuscarVeiculoPorIdUseCase();
            AtualizarVeiculoUseCase = new AtualizarVeiculoUseCase();
        }
    }
}