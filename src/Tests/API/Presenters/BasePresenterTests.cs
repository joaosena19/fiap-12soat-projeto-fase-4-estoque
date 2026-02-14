using API.Presenters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Xunit;

namespace Tests.API.Presenters
{
    public class BasePresenterTests
    {
        private readonly ConcretePresenterParaTestes _presenter;

        public BasePresenterTests()
        {
            _presenter = new ConcretePresenterParaTestes();
        }

        #region ObterResultado

        [Fact(DisplayName = "Deve retornar Internal Server Error quando nenhum resultado foi definido")]
        [Trait("Presenter", "BasePresenter")]
        public void ObterResultado_DeveRetornarInternalServerError_QuandoNenhumResultadoFoiDefinido()
        {
            // Act
            var resultado = _presenter.ObterResultado();

            // Assert
            resultado.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = resultado as StatusCodeResult;
            statusCodeResult!.StatusCode.Should().Be(500);
            _presenter.FoiSucesso.Should().BeFalse();
        }

        #endregion

        #region DefinirSucesso

        [Fact(DisplayName = "Deve marcar foi sucesso e armazenar resultado quando definir sucesso com IActionResult")]
        [Trait("Presenter", "BasePresenter")]
        public void DefinirSucesso_DeveMarcarFoiSucessoEArmazenarResultado_QuandoDefinirSucessoComActionResult()
        {
            // Arrange
            var resultadoEsperado = new OkObjectResult(new { id = 1 });

            // Act
            _presenter.ExporDefinirSucesso(resultadoEsperado);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeTrue();
            resultado.Should().Be(resultadoEsperado);
        }

        [Fact(DisplayName = "Deve marcar foi sucesso e armazenar resultado quando definir sucesso com objeto")]
        [Trait("Presenter", "BasePresenter")]
        public void DefinirSucesso_DeveMarcarFoiSucessoEArmazenarResultado_QuandoDefinirSucessoComObjeto()
        {
            // Arrange
            var dadosEsperados = new { id = 1, nome = "Teste" };

            // Act
            _presenter.ExporDefinirSucesso(dadosEsperados);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeTrue();
            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult!.Value.Should().Be(dadosEsperados);
        }

        [Fact(DisplayName = "Deve marcar foi sucesso e armazenar resultado quando definir sucesso com localização")]
        [Trait("Presenter", "BasePresenter")]
        public void DefinirSucesso_DeveMarcarFoiSucessoEArmazenarResultado_QuandoDefinirSucessoComLocalizacao()
        {
            // Arrange
            var action = "BuscarPorId";
            var controller = "Produto";
            var routeValues = new { id = 1 };
            var dados = new { nome = "Teste" };

            // Act
            _presenter.ExporDefinirSucessoComLocalizacao(action, controller, routeValues, dados);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeTrue();
            resultado.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = resultado as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(action);
            createdResult.ControllerName.Should().Be(controller);
            createdResult.RouteValues.Should().ContainKey("id");
            createdResult.RouteValues!["id"].Should().Be(1);
            createdResult.Value.Should().Be(dados);
        }

        #endregion

        #region ApresentarErro

        [Fact(DisplayName = "Deve mapear Conflict para ConflictObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaConflictObjectResult_QuandoErrorTypeForConflict()
        {
            // Arrange
            var mensagem = "Recurso já existe";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.Conflict);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<ConflictObjectResult>();
            var conflictResult = resultado as ConflictObjectResult;
            var errorResponse = conflictResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear InvalidInput para BadRequestObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaBadRequestObjectResult_QuandoErrorTypeForInvalidInput()
        {
            // Arrange
            var mensagem = "Dados de entrada inválidos";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.InvalidInput);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = resultado as BadRequestObjectResult;
            var errorResponse = badRequestResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear ResourceNotFound para NotFoundObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaNotFoundObjectResult_QuandoErrorTypeForResourceNotFound()
        {
            // Arrange
            var mensagem = "Recurso não encontrado";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.ResourceNotFound);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = resultado as NotFoundObjectResult;
            var errorResponse = notFoundResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear ReferenceNotFound para UnprocessableEntityObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaUnprocessableEntityObjectResult_QuandoErrorTypeForReferenceNotFound()
        {
            // Arrange
            var mensagem = "Referência não encontrada";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.ReferenceNotFound);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<UnprocessableEntityObjectResult>();
            var unprocessableResult = resultado as UnprocessableEntityObjectResult;
            var errorResponse = unprocessableResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear DomainRuleBroken para UnprocessableEntityObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaUnprocessableEntityObjectResult_QuandoErrorTypeForDomainRuleBroken()
        {
            // Arrange
            var mensagem = "Regra de domínio violada";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.DomainRuleBroken);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<UnprocessableEntityObjectResult>();
            var unprocessableResult = resultado as UnprocessableEntityObjectResult;
            var errorResponse = unprocessableResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear Unauthorized para UnauthorizedObjectResult")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaUnauthorizedObjectResult_QuandoErrorTypeForUnauthorized()
        {
            // Arrange
            var mensagem = "Não autorizado";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.Unauthorized);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = resultado as UnauthorizedObjectResult;
            var errorResponse = unauthorizedResult!.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear NotAllowed para ObjectResult com status 403")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaObjectResultCom403_QuandoErrorTypeForNotAllowed()
        {
            // Arrange
            var mensagem = "Acesso negado";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.NotAllowed);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(403);
            var errorResponse = objectResult.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        [Fact(DisplayName = "Deve mapear UnexpectedError para ObjectResult com status 500")]
        [Trait("Presenter", "BasePresenter")]
        public void ApresentarErro_DeveMapearParaObjectResultCom500_QuandoErrorTypeForUnexpectedError()
        {
            // Arrange
            var mensagem = "Erro inesperado";

            // Act
            _presenter.ApresentarErro(mensagem, ErrorType.UnexpectedError);
            var resultado = _presenter.ObterResultado();

            // Assert
            _presenter.FoiSucesso.Should().BeFalse();
            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            var errorResponse = objectResult.Value;
            errorResponse.Should().NotBeNull();
            errorResponse!.GetType().GetProperty("message")!.GetValue(errorResponse).Should().Be(mensagem);
        }

        #endregion

        #region Presenter Concreto para Testes

        private class ConcretePresenterParaTestes : BasePresenter
        {
            public void ExporDefinirSucesso(IActionResult resultado)
            {
                DefinirSucesso(resultado);
            }

            public void ExporDefinirSucesso(object dados)
            {
                DefinirSucesso(dados);
            }

            public void ExporDefinirSucessoComLocalizacao(string action, string controller, object routeValues, object dados)
            {
                DefinirSucessoComLocalizacao(action, controller, routeValues, dados);
            }
        }

        #endregion
    }
}
