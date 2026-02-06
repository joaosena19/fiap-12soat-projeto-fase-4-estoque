using Domain.Identidade.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tests.Helpers;

namespace Tests.Integration.Identidade
{
    public class PostEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PostEndpointTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo usuário no banco de dados")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirUsuario()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new
            {
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);
            var usuarioEntity = await context.Usuarios.Include(u => u.Roles).FirstOrDefaultAsync(u => u.DocumentoIdentificadorUsuario.Valor == cpf);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            usuarioEntity.Should().NotBeNull();
            usuarioEntity!.DocumentoIdentificadorUsuario.Valor.Should().Be(cpf);
            usuarioEntity.Roles.Should().HaveCount(1);
            usuarioEntity.Roles.First().Nome.Valor.Should().Be("Cliente");
        }

        [Fact(DisplayName = "POST deve retornar 409 Conflict quando usuário já existe")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar409Conflict_QuandoUsuarioJaExiste()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new
            {
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };

            // Create user first
            var createResponse = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act - Try to create again
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact(DisplayName = "POST deve retornar 400 Bad Request para senha inválida")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar400BadRequest_ParaSenhaInvalida()
        {
            // Arrange
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new
            {
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "123", // Senha muito curta
                Roles = new[] { "Cliente" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/identidade/usuarios", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = "POST múltiplas criações não devem duplicar roles na tabela")]
        [Trait("Metodo", "Post")]
        public async Task Post_MultiplascriacoesNaoDevemDuplicarRoles()
        {
            // Arrange - Preparar dados para múltiplos usuários
            var cpf1 = DocumentoHelper.GerarCpfValido();
            var cpf2 = DocumentoHelper.GerarCpfValido();
            var cpf3 = DocumentoHelper.GerarCpfValido();

            var usuario1 = new
            {
                DocumentoIdentificador = cpf1,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };

            var usuario2 = new
            {
                DocumentoIdentificador = cpf2,
                SenhaNaoHasheada = "senhaSegura456",
                Roles = new[] { "Cliente", "Administrador" }
            };

            var usuario3 = new
            {
                DocumentoIdentificador = cpf3,
                SenhaNaoHasheada = "senhaSegura789",
                Roles = new[] { "Administrador" }
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Contar roles antes da criação (devem existir 2 já criadas no setup)
            var rolesIniciais = await context.Roles.CountAsync();

            // Act - Criar os três usuários
            var response1 = await _client.PostAsJsonAsync("/api/identidade/usuarios", usuario1);
            var response2 = await _client.PostAsJsonAsync("/api/identidade/usuarios", usuario2);
            var response3 = await _client.PostAsJsonAsync("/api/identidade/usuarios", usuario3);

            // Assert - Todos devem ter sido criados com sucesso
            response1.StatusCode.Should().Be(HttpStatusCode.Created);
            response2.StatusCode.Should().Be(HttpStatusCode.Created);
            response3.StatusCode.Should().Be(HttpStatusCode.Created);

            // Verificar que as roles não foram duplicadas na base
            var rolesFinais = await context.Roles.CountAsync();
            rolesFinais.Should().Be(rolesIniciais, "não devem ter sido criadas roles adicionais");

            // Deve ter exatamente uma role Cliente
            var rolesCliente = await context.Roles.CountAsync(r => r.Id == RoleEnum.Cliente);
            rolesCliente.Should().Be(1, "deve existir apenas uma role Cliente");

            // Deve ter exatamente uma role Administrador  
            var rolesAdministrador = await context.Roles.CountAsync(r => r.Id == RoleEnum.Administrador);
            rolesAdministrador.Should().Be(1, "deve existir apenas uma role Administrador");

            // Verificar que os usuários foram criados com as associações corretas
            var usuarios = await context.Usuarios.Include(u => u.Roles).ToListAsync();

            var usuarioEntity1 = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == cpf1);
            usuarioEntity1.Should().NotBeNull();
            usuarioEntity1!.Roles.Should().HaveCount(1).And.Contain(r => r.Id == RoleEnum.Cliente);

            var usuarioEntity2 = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == cpf2);
            usuarioEntity2.Should().NotBeNull();
            usuarioEntity2!.Roles.Should().HaveCount(2)
                .And.Contain(r => r.Id == RoleEnum.Cliente)
                .And.Contain(r => r.Id == RoleEnum.Administrador);

            var usuarioEntity3 = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == cpf3);
            usuarioEntity3.Should().NotBeNull();
            usuarioEntity3!.Roles.Should().HaveCount(1).And.Contain(r => r.Id == RoleEnum.Administrador);
        }

        [Fact(DisplayName = "POST deve retornar 403 Forbidden quando cliente tenta criar usuário")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar403Forbidden_QuandoClienteTentaCriarUsuario()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var cpf = DocumentoHelper.GerarCpfValido();
            var dto = new
            {
                DocumentoIdentificador = cpf,
                SenhaNaoHasheada = "senhaSegura123",
                Roles = new[] { "Cliente" }
            };

            // Act - Cliente tenta criar usuário
            var response = await clienteAuthenticatedClient.PostAsJsonAsync("/api/identidade/usuarios", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}