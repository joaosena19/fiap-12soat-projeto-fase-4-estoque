using Application.Identidade.Dtos;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tests.Helpers;

namespace Tests.Integration.Identidade
{
    public class GetByDocumentoTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public GetByDocumentoTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "GET /documento/{documento} deve retornar 200 OK e usuário específico")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar200OK_E_UsuarioEspecifico()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var criarDto = new 
            { 
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create user first
            var createResponse = await _client.PostAsJsonAsync("/api/identidade/usuarios", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var usuarioCriado = await context.Usuarios.FirstOrDefaultAsync(u => u.DocumentoIdentificadorUsuario.Valor == cpf);
            usuarioCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/identidade/usuarios/documento/{cpf}");
            var usuario = await response.Content.ReadFromJsonAsync<RetornoUsuarioDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            usuario.Should().NotBeNull();
            usuario!.Id.Should().Be(usuarioCriado!.Id);
            usuario.DocumentoIdentificador.Should().Be(cpf);
            usuario.TipoDocumentoIdentificador.Should().Be("CPF");
            usuario.Roles.Should().HaveCount(1);
            usuario.Roles.Should().Contain("Cliente");
        }

        [Fact(DisplayName = "GET /documento/{documento} deve retornar 404 Not Found quando usuário não existe")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar404NotFound_QuandoUsuarioNaoExiste()
        {
            // Arrange
            var cpfInexistente = DocumentoHelper.GerarCpfValido();

            // Act
            var response = await _client.GetAsync($"/api/identidade/usuarios/documento/{cpfInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /documento/{documento} deve retornar 403 Forbidden quando cliente tenta buscar usuário por documento")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar403Forbidden_QuandoClienteTentaBuscarUsuarioPorDocumento()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var cpfInexistente = DocumentoHelper.GerarCpfValido();

            // Act - Cliente tenta buscar usuário por documento
            var response = await clienteAuthenticatedClient.GetAsync($"/api/identidade/usuarios/documento/{cpfInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}