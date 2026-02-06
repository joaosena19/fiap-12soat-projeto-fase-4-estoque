using Shared.Enums;

namespace Application.Contracts.Presenters;

/// <summary>
/// Presenter para ações sem response/NoContent
/// </summary>
public interface IOperacaoOrdemServicoPresenter
{
    void ApresentarSucesso();
    void ApresentarErro(string mensagem, ErrorType errorType);
}