using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace API.Presenters
{
    public abstract class BasePresenter
    {
        protected IActionResult? _resultado;
        protected bool _foiSucesso;

        protected BasePresenter()
        {
            _foiSucesso = false;
        }

        public void ApresentarErro(string mensagem, ErrorType errorType)
        {
            var errorResponse = new { message = mensagem };
            
            _resultado = errorType switch
            {
                ErrorType.Conflict => new ConflictObjectResult(errorResponse),
                ErrorType.InvalidInput => new BadRequestObjectResult(errorResponse),
                ErrorType.ResourceNotFound => new NotFoundObjectResult(errorResponse),
                ErrorType.ReferenceNotFound => new UnprocessableEntityObjectResult(errorResponse),
                ErrorType.DomainRuleBroken => new UnprocessableEntityObjectResult(errorResponse),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(errorResponse),
                ErrorType.NotAllowed => new ObjectResult(errorResponse) { StatusCode = 403 },
                _ => new ObjectResult(errorResponse) { StatusCode = 500 }
            };
            _foiSucesso = false;
        }

        public IActionResult ObterResultado()
        {
            return _resultado ?? new StatusCodeResult(500);
        }

        public bool FoiSucesso => _foiSucesso;

        protected void DefinirSucesso(IActionResult resultado)
        {
            _resultado = resultado;
            _foiSucesso = true;
        }

        protected void DefinirSucesso(object dados)
        {
            _resultado = new OkObjectResult(dados);
            _foiSucesso = true;
        }

        protected void DefinirSucessoComLocalizacao(string action, string controller, object routeValues, object dados)
        {
            _resultado = new CreatedAtActionResult(action, controller, routeValues, dados);
            _foiSucesso = true;
        }
    }
}