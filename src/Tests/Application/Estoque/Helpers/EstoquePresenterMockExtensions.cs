using Application.Contracts.Presenters;
using Moq;
using Shared.Enums;
using ItemEstoqueAggregate = Domain.Estoque.Aggregates.ItemEstoque;

namespace Tests.Application.Estoque.Helpers
{
    public static class EstoquePresenterMockExtensions
    {
        // Abstrações específicas para IVerificarDisponibilidadePresenter, que usa ApresentarSucesso diferente do IBasePresenter
        public static void DeveTerApresentadoSucessoVerificacaoDisponibilidade(this Mock<IVerificarDisponibilidadePresenter> mock, ItemEstoqueAggregate item, int quantidadeRequisitada, bool disponivel)
        {
            mock.Verify(p => p.ApresentarSucesso(It.Is<ItemEstoqueAggregate>(i => Equals(i, item)), quantidadeRequisitada, disponivel), Times.Once,
                $"Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com o item fornecido, quantidade {quantidadeRequisitada} e disponibilidade {disponivel}.");
        }

        public static void DeveTerApresentadoErroVerificacaoDisponibilidade(this Mock<IVerificarDisponibilidadePresenter> mock, string mensagem, ErrorType errorType)
        {
            mock.Verify(p => p.ApresentarErro(mensagem, errorType), Times.Once,
                $"Era esperado que o método ApresentarErro fosse chamado exatamente uma vez com a mensagem '{mensagem}' e tipo '{errorType}'.");
        }

        public static void NaoDeveTerApresentadoSucessoVerificacaoDisponibilidade(this Mock<IVerificarDisponibilidadePresenter> mock)
        {
            mock.Verify(p => p.ApresentarSucesso(It.IsAny<ItemEstoqueAggregate>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never,
                "O método ApresentarSucesso não deveria ter sido chamado.");
        }

        public static void NaoDeveTerApresentadoErroVerificacaoDisponibilidade(this Mock<IVerificarDisponibilidadePresenter> mock)
        {
            mock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never,
                "O método ApresentarErro não deveria ter sido chamado.");
        }
    }
}