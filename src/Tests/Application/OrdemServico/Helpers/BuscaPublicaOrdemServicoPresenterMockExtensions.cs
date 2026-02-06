using Application.Contracts.Presenters;
using Moq;

namespace Tests.Application.OrdemServico.Helpers
{
    public static class BuscaPublicaOrdemServicoPresenterMockExtensions
    {
        public static void DeveTerApresentadoNaoEncontrado(this Mock<IBuscaPublicaOrdemServicoPresenter> mock)
        {
            mock.Verify(p => p.ApresentarNaoEncontrado(), Times.Once,
                "Era esperado que o método ApresentarNaoEncontrado fosse chamado exatamente uma vez.");
        }

        public static void NaoDeveTerApresentadoNaoEncontrado(this Mock<IBuscaPublicaOrdemServicoPresenter> mock)
        {
            mock.Verify(p => p.ApresentarNaoEncontrado(), Times.Never,
                "O método ApresentarNaoEncontrado não deveria ter sido chamado.");
        }
    }
}