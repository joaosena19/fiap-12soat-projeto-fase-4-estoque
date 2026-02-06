using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Estoque.UseCases;
using Moq;
using Tests.Application.Estoque.Helpers;

namespace Tests.Application.Estoque.Helpers
{
    public class ItemEstoqueTestFixture
    {
        public Mock<IItemEstoqueGateway> ItemEstoqueGatewayMock { get; }
        public Mock<ICriarItemEstoquePresenter> CriarItemEstoquePresenterMock { get; }
        public Mock<IAtualizarItemEstoquePresenter> AtualizarItemEstoquePresenterMock { get; }
        public Mock<IBuscarItemEstoquePorIdPresenter> BuscarItemEstoquePorIdPresenterMock { get; }
        public Mock<IBuscarTodosItensEstoquePresenter> BuscarTodosItensEstoquePresenterMock { get; }
        public Mock<IAtualizarQuantidadePresenter> AtualizarQuantidadePresenterMock { get; }
        public Mock<IVerificarDisponibilidadePresenter> VerificarDisponibilidadePresenterMock { get; }

        public CriarItemEstoqueUseCase CriarItemEstoqueUseCase { get; }
        public AtualizarItemEstoqueUseCase AtualizarItemEstoqueUseCase { get; }
        public BuscarItemEstoquePorIdUseCase BuscarItemEstoquePorIdUseCase { get; }
        public BuscarTodosItensEstoqueUseCase BuscarTodosItensEstoqueUseCase { get; }
        public AtualizarQuantidadeUseCase AtualizarQuantidadeUseCase { get; }
        public VerificarDisponibilidadeUseCase VerificarDisponibilidadeUseCase { get; }

        public ItemEstoqueTestFixture()
        {
            ItemEstoqueGatewayMock = new Mock<IItemEstoqueGateway>();
            CriarItemEstoquePresenterMock = new Mock<ICriarItemEstoquePresenter>();
            AtualizarItemEstoquePresenterMock = new Mock<IAtualizarItemEstoquePresenter>();
            BuscarItemEstoquePorIdPresenterMock = new Mock<IBuscarItemEstoquePorIdPresenter>();
            BuscarTodosItensEstoquePresenterMock = new Mock<IBuscarTodosItensEstoquePresenter>();
            AtualizarQuantidadePresenterMock = new Mock<IAtualizarQuantidadePresenter>();
            VerificarDisponibilidadePresenterMock = new Mock<IVerificarDisponibilidadePresenter>();

            CriarItemEstoqueUseCase = new CriarItemEstoqueUseCase();
            AtualizarItemEstoqueUseCase = new AtualizarItemEstoqueUseCase();
            BuscarItemEstoquePorIdUseCase = new BuscarItemEstoquePorIdUseCase();
            BuscarTodosItensEstoqueUseCase = new BuscarTodosItensEstoqueUseCase();
            AtualizarQuantidadeUseCase = new AtualizarQuantidadeUseCase();
            VerificarDisponibilidadeUseCase = new VerificarDisponibilidadeUseCase();
        }
    }
}