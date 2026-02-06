using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Moq;

namespace Tests.Application.Cadastros.Servico.Helpers
{
    public class ServicoTestFixture
    {
        public Mock<IServicoGateway> ServicoGatewayMock { get; }
        public Mock<ICriarServicoPresenter> CriarServicoPresenterMock { get; }
        public Mock<IBuscarServicoPorIdPresenter> BuscarServicoPorIdPresenterMock { get; }
        public Mock<IBuscarServicosPresenter> BuscarServicosPresenterMock { get; }
        public Mock<IAtualizarServicoPresenter> AtualizarServicoPresenterMock { get; }

        public CriarServicoUseCase CriarServicoUseCase { get; }
        public BuscarServicoPorIdUseCase BuscarServicoPorIdUseCase { get; }
        public BuscarServicosUseCase BuscarServicosUseCase { get; }
        public AtualizarServicoUseCase AtualizarServicoUseCase { get; }

        public ServicoTestFixture()
        {
            ServicoGatewayMock = new Mock<IServicoGateway>();
            CriarServicoPresenterMock = new Mock<ICriarServicoPresenter>();
            BuscarServicoPorIdPresenterMock = new Mock<IBuscarServicoPorIdPresenter>();
            BuscarServicosPresenterMock = new Mock<IBuscarServicosPresenter>();
            AtualizarServicoPresenterMock = new Mock<IAtualizarServicoPresenter>();

            CriarServicoUseCase = new CriarServicoUseCase();
            BuscarServicoPorIdUseCase = new BuscarServicoPorIdUseCase();
            BuscarServicosUseCase = new BuscarServicosUseCase();
            AtualizarServicoUseCase = new AtualizarServicoUseCase();
        }
    }
}