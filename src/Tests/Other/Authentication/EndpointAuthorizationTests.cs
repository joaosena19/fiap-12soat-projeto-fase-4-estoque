using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Tests.Integration;

namespace Tests.Other.Authentication
{
    public class EndpointAuthorizationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EndpointAuthorizationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Usa client sem autenticação
        }

        #region Endpoints que precisam de Authorize

        [Theory]
        // EstoqueItemController endpoints
        [InlineData("GET", "/api/estoque/itens")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/estoque/itens")]
        [InlineData("PUT", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("PATCH", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/quantidade")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/disponibilidade?quantidadeRequisitada=1")]
        public async Task Endpoints_SemAutenticacao_DevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion
    }
}
