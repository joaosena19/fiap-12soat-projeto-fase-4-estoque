using Application.Identidade.UseCases.Usuario;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;
using Moq;

namespace Tests.Application.Identidade.Helpers
{
    public class IdentidadeTestFixture
    {
        public Mock<IUsuarioGateway> UsuarioGatewayMock { get; }
        public Mock<ICriarUsuarioPresenter> CriarUsuarioPresenterMock { get; }
        public Mock<IBuscarUsuarioPorDocumentoPresenter> BuscarUsuarioPorDocumentoPresenterMock { get; }
        public Mock<IPasswordHasher> PasswordHasherMock { get; }
        
        public CriarUsuarioUseCase CriarUsuarioUseCase { get; }
        public BuscarUsuarioPorDocumentoUseCase BuscarUsuarioPorDocumentoUseCase { get; }

        public IdentidadeTestFixture()
        {
            UsuarioGatewayMock = new Mock<IUsuarioGateway>();
            CriarUsuarioPresenterMock = new Mock<ICriarUsuarioPresenter>();
            BuscarUsuarioPorDocumentoPresenterMock = new Mock<IBuscarUsuarioPorDocumentoPresenter>();
            PasswordHasherMock = new Mock<IPasswordHasher>();
            
            CriarUsuarioUseCase = new CriarUsuarioUseCase();
            BuscarUsuarioPorDocumentoUseCase = new BuscarUsuarioPorDocumentoUseCase();
        }
    }
}