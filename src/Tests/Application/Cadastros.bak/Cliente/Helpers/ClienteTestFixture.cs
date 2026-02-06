using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Moq;

namespace Tests.Application.Cadastros.Cliente.Helpers
{
    public class ClienteTestFixture
    {
        public Mock<IClienteGateway> ClienteGatewayMock { get; }
        public Mock<IUsuarioGateway> UsuarioGatewayMock { get; }
        public Mock<IAtualizarClientePresenter> AtualizarClientePresenterMock { get; }
        public Mock<IBuscarClientePorDocumentoPresenter> BuscarClientePorDocumentoPresenterMock { get; }
        public Mock<IBuscarClientePorIdPresenter> BuscarClientePorIdPresenterMock { get; }
        public Mock<IBuscarClientesPresenter> BuscarClientesPresenterMock { get; }
        public Mock<ICriarClientePresenter> CriarClientePresenterMock { get; }
        
        public AtualizarClienteUseCase AtualizarClienteUseCase { get; }
        public BuscarClientePorDocumentoUseCase BuscarClientePorDocumentoUseCase { get; }
        public BuscarClientePorIdUseCase BuscarClientePorIdUseCase { get; }
        public BuscarClientesUseCase BuscarClientesUseCase { get; }
        public CriarClienteUseCase CriarClienteUseCase { get; }

        public ClienteTestFixture()
        {
            ClienteGatewayMock = new Mock<IClienteGateway>();
            UsuarioGatewayMock = new Mock<IUsuarioGateway>();
            AtualizarClientePresenterMock = new Mock<IAtualizarClientePresenter>();
            BuscarClientePorDocumentoPresenterMock = new Mock<IBuscarClientePorDocumentoPresenter>();
            BuscarClientePorIdPresenterMock = new Mock<IBuscarClientePorIdPresenter>();
            BuscarClientesPresenterMock = new Mock<IBuscarClientesPresenter>();
            CriarClientePresenterMock = new Mock<ICriarClientePresenter>();
            
            AtualizarClienteUseCase = new AtualizarClienteUseCase();
            BuscarClientePorDocumentoUseCase = new BuscarClientePorDocumentoUseCase();
            BuscarClientePorIdUseCase = new BuscarClientePorIdUseCase();
            BuscarClientesUseCase = new BuscarClientesUseCase();
            CriarClienteUseCase = new CriarClienteUseCase();
        }
    }
}