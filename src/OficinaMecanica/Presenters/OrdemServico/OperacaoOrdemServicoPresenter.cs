using API.Presenters;
using Application.Contracts.Presenters;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace API.Presenters.OrdemServico
{
    /// <summary>
    /// Presenter para ações sem response/NoContent
    /// </summary>
    public class OperacaoOrdemServicoPresenter : BasePresenter, IOperacaoOrdemServicoPresenter
    {
        public void ApresentarSucesso()
        {
            DefinirSucesso(new NoContentResult());
        }
    }
}