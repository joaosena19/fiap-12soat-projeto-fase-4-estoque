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
        // ClienteController endpoints
        [InlineData("GET", "/api/cadastros/clientes")]
        [InlineData("GET", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/clientes/documento/12345678901")]
        [InlineData("POST", "/api/cadastros/clientes")]
        [InlineData("PUT", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        // ServicoController endpoints
        [InlineData("GET", "/api/cadastros/servicos")]
        [InlineData("GET", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/servicos")]
        [InlineData("PUT", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        // VeiculoController endpoints
        [InlineData("GET", "/api/cadastros/veiculos")]
        [InlineData("GET", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/veiculos/placa/ABC1234")]
        [InlineData("GET", "/api/cadastros/veiculos/cliente/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/veiculos")]
        [InlineData("PUT", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        // UsuarioController endpoints
        [InlineData("GET", "/api/identidade/usuarios/documento/12345678901")]
        [InlineData("POST", "/api/identidade/usuarios")]
        // EstoqueItemController endpoints
        [InlineData("GET", "/api/estoque/itens")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/estoque/itens")]
        [InlineData("PUT", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("PATCH", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/quantidade")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/disponibilidade?quantidadeRequisitada=1")]
        // OrdemServicoController endpoints
        [InlineData("GET", "/api/ordens-servico")]
        [InlineData("GET", "/api/ordens-servico/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/ordens-servico/codigo/OS123")]
        [InlineData("POST", "/api/ordens-servico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/cancelar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/iniciar-diagnostico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/aprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/desaprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/finalizar-execucao")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/entregar")]
        [InlineData("GET", "/api/ordens-servico/tempo-medio")]
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

        #region Endpoints que não podem ter Authorize

        [Theory]
        [InlineData("POST", "/api/ordens-servico/busca-publica")]
        public async Task Endpoints_ComAllowAnonymous_NaoDevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion
    }
}
