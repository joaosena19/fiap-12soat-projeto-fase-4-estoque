using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBasePresenter<TObjetoSucesso>
    {
        void ApresentarErro(string mensagem, ErrorType errorType);
        void ApresentarSucesso(TObjetoSucesso objeto);
    }
}