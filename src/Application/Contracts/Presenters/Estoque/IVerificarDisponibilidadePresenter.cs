using Domain.Estoque.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IVerificarDisponibilidadePresenter
{
    void ApresentarSucesso(ItemEstoque itemEstoque, int quantidadeRequisitada, bool disponivel);
    void ApresentarErro(string mensagem, ErrorType errorType);
}