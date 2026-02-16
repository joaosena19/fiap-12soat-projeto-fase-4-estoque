using API.Extensions;
using FluentAssertions;
using Shared.Enums;
using System.Net;

namespace Tests.Other.Extensions
{
    public class ErrorTypeExtensionsTests
    {
        [Theory(DisplayName = "ToHttpStatusCode deve retornar o HttpStatusCode correto para cada ErrorType")]
        [Trait("Método", "ToHttpStatusCode")]
        [InlineData(ErrorType.InvalidInput, HttpStatusCode.BadRequest)]
        [InlineData(ErrorType.ResourceNotFound, HttpStatusCode.NotFound)]
        [InlineData(ErrorType.ReferenceNotFound, HttpStatusCode.UnprocessableEntity)]
        [InlineData(ErrorType.DomainRuleBroken, HttpStatusCode.UnprocessableEntity)]
        [InlineData(ErrorType.Conflict, HttpStatusCode.Conflict)]
        [InlineData(ErrorType.Unauthorized, HttpStatusCode.Unauthorized)]
        [InlineData(ErrorType.UnexpectedError, HttpStatusCode.InternalServerError)]
        public void ToHttpStatusCode_Deve_RetornarHttpStatusCodeCorreto_Para_CadaErrorType(ErrorType errorType, HttpStatusCode expectedStatusCode)
        {
            // Act
            var result = errorType.ToHttpStatusCode();

            // Assert
            result.Should().Be(expectedStatusCode);
        }

        [Fact(DisplayName = "ToHttpStatusCode deve retornar InternalServerError para valor não mapeado")]
        [Trait("Método", "ToHttpStatusCode")]
        public void ToHttpStatusCode_Deve_RetornarInternalServerError_Para_ValorNaoMapeado()
        {
            // Arrange
            var unmappedErrorType = (ErrorType)999; // Valor não existente no enum

            // Act
            var result = unmappedErrorType.ToHttpStatusCode();

            // Assert
            result.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact(DisplayName = "Todos os valores de ErrorType devem ter mapeamento para HttpStatusCode")]
        [Trait("Método", "ToHttpStatusCode")]
        public void ToHttpStatusCode_Deve_TerMapeamento_Para_TodosValoresErrorType()
        {
            // Arrange
            var allErrorTypes = Enum.GetValues<ErrorType>();

            // Act & Assert
            foreach (var errorType in allErrorTypes)
            {
                var result = errorType.ToHttpStatusCode();
                result.Should().NotBe(default(HttpStatusCode), $"ErrorType.{errorType} deve ter um mapeamento válido");
            }
        }
    }
}
