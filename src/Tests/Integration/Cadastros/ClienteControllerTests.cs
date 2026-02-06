using Application.Cadastros.Dtos;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tests.Helpers;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Integration.Cadastros
{
    public class ClienteControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ClienteControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Cliente no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirCliente()
        {
            // Arrange
            var dto = new { Nome = "João", DocumentoIdentificador = "49622601030" };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/clientes", dto);
            var clientEntity = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "49622601030");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            clientEntity.Should().NotBeNull();
            clientEntity.Nome.Valor.Should().Be("João");
            clientEntity.DocumentoIdentificador.Valor.Should().Be("49622601030");
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Cliente existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarCliente()
        {
            // Arrange
            var criarDto = new { Nome = "João", DocumentoIdentificador = "42103574052" };
            var atualizarDto = new { Nome = "João Silva Atualizado" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "42103574052");
            clienteCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/clientes/{clienteCriado!.Id}", atualizarDto);

            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var clienteAtualizado = await context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be("João Silva Atualizado");
            clienteAtualizado.DocumentoIdentificador.Valor.Should().Be("42103574052"); // CPF não deve mudar
        }

        [Fact(DisplayName = "PUT deve retornar 403 Forbidden quando cliente tenta atualizar dados de outro cliente")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar403Forbidden_QuandoClienteTentaAtualizarDadosDeOutroCliente()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar primeiro cliente usando admin
            var criarDto1 = new { Nome = "Cliente 1", DocumentoIdentificador = DocumentoHelper.GerarCpfValido() };
            var adminClient = _factory.CreateAuthenticatedClient(); // cliente admin
            var createResponse1 = await adminClient.PostAsJsonAsync("/api/cadastros/clientes", criarDto1);
            createResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

            var cliente1Criado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == criarDto1.DocumentoIdentificador);
            cliente1Criado.Should().NotBeNull();

            // Criar segundo cliente usando admin
            var criarDto2 = new { Nome = "Cliente 2", DocumentoIdentificador = DocumentoHelper.GerarCpfValido() };
            var createResponse2 = await adminClient.PostAsJsonAsync("/api/cadastros/clientes", criarDto2);
            createResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

            var cliente2Criado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == criarDto2.DocumentoIdentificador);
            cliente2Criado.Should().NotBeNull();

            // Autenticar como segundo cliente
            var cliente2AuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: cliente2Criado!.Id);

            var atualizarDto = new { Nome = "Nome Modificado" };

            // Act - tentar atualizar dados do primeiro cliente autenticado como segundo cliente
            var updateResponse = await cliente2AuthenticatedClient.PutAsJsonAsync($"/api/cadastros/clientes/{cliente1Criado!.Id}", atualizarDto);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de clientes")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeClientes()
        {
            // Arrange
            var cliente1 = new { Nome = "João", DocumentoIdentificador = "12345678909" };
            var cliente2 = new { Nome = "Maria", DocumentoIdentificador = "84405205060" };

            // Create test clients
            await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente1);
            await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente2);

            // Act
            var response = await _client.GetAsync("/api/cadastros/clientes");
            var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoClienteDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            clientes.Should().NotBeNull();
            clientes.Should().HaveCountGreaterThanOrEqualTo(2);
            clientes.Should().Contain(c => c.Nome == "João" && c.DocumentoIdentificador == "12345678909" && c.TipoDocumentoIdentificador == "CPF");
            clientes.Should().Contain(c => c.Nome == "Maria" && c.DocumentoIdentificador == "84405205060" && c.TipoDocumentoIdentificador == "CPF");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há clientes")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaClientes()
        {
            // Arrange - Clear database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Clientes.RemoveRange(context.Clientes);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/cadastros/clientes");
            var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoClienteDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            clientes.Should().NotBeNull();
            clientes.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e cliente específico")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_ClienteEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "João", DocumentoIdentificador = "56227045020" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "56227045020");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{clienteCriado!.Id}");
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado.Id);
            cliente.Nome.Should().Be("João");
            cliente.DocumentoIdentificador.Should().Be("56227045020");
            cliente.TipoDocumentoIdentificador.Should().Be("CPF");
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando cliente não existe")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoClienteNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /cpf/{cpf} deve retornar 200 OK e cliente específico")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByCpf_Deve_Retornar200OK_E_ClienteEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "João", DocumentoIdentificador = "34806653063" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "34806653063");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/34806653063");
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("João");
            cliente.DocumentoIdentificador.Should().Be("34806653063");
            cliente.TipoDocumentoIdentificador.Should().Be("CPF");
        }

        [Fact(DisplayName = "GET /cpf/{cpf} deve retornar 404 NotFound quando cliente não existe")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByCpf_Deve_Retornar404NotFound_QuandoClienteNaoExiste()
        {
            // Arrange
            var cpfInexistente = "99999999999";

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/{cpfInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /documento/{cnpj} deve retornar 200 OK quando CNPJ existe e valor passado tem formatação")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar200OK_QuandoCnpjExisteEValorTemFormatacao()
        {
            // Arrange
            var criarDto = new { Nome = "Empresa CNPJ Formatado", DocumentoIdentificador = "03984051000193" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "03984051000193");
            clienteCriado.Should().NotBeNull();

            // Act - buscar com CNPJ formatado
            var cnpjFormatado = Uri.EscapeDataString("03.984.051/0001-93");
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/{cnpjFormatado}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("Empresa CNPJ Formatado");
            cliente.DocumentoIdentificador.Should().Be("03984051000193");
            cliente.TipoDocumentoIdentificador.Should().Be("CNPJ");
        }

        [Fact(DisplayName = "GET /documento/{cnpj} deve retornar 200 OK quando CNPJ existe e valor passado não tem formatação")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar200OK_QuandoCnpjExisteEValorSemFormatacao()
        {
            // Arrange
            var criarDto = new { Nome = "Empresa CNPJ Sem Formatação", DocumentoIdentificador = "38.689.954/0001-26" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "38689954000126");
            clienteCriado.Should().NotBeNull();

            // Act - buscar com CNPJ sem formatação
            var cnpjSemFormatacao = "38689954000126";
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/{cnpjSemFormatacao}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("Empresa CNPJ Sem Formatação");
            cliente.DocumentoIdentificador.Should().Be("38689954000126");
            cliente.TipoDocumentoIdentificador.Should().Be("CNPJ");
        }

        [Fact(DisplayName = "GET /documento/{cpf} deve retornar 200 OK quando CPF existe e valor passado tem formatação")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar200OK_QuandoCpfExisteEValorTemFormatacao()
        {
            // Arrange
            var criarDto = new { Nome = "Cliente CPF Formatado", DocumentoIdentificador = "79705026017" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "79705026017");
            clienteCriado.Should().NotBeNull();

            // Act - buscar com CPF formatado
            var cpfFormatado = "797.050.260-17";
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/{cpfFormatado}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("Cliente CPF Formatado");
            cliente.DocumentoIdentificador.Should().Be("79705026017");
            cliente.TipoDocumentoIdentificador.Should().Be("CPF");
        }

        [Fact(DisplayName = "GET /documento/{cpf} deve retornar 200 OK quando CPF existe e valor passado não tem formatação")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar200OK_QuandoCpfExisteEValorSemFormatacao()
        {
            // Arrange
            var criarDto = new { Nome = "Cliente CPF Sem Formatação", DocumentoIdentificador = "327.442.850-72" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "32744285072");
            clienteCriado.Should().NotBeNull();

            // Act - buscar com CPF sem formatação
            var cpfSemFormatacao = "32744285072";
            var response = await _client.GetAsync($"/api/cadastros/clientes/documento/{cpfSemFormatacao}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("Cliente CPF Sem Formatação");
            cliente.DocumentoIdentificador.Should().Be("32744285072");
            cliente.TipoDocumentoIdentificador.Should().Be("CPF");
        }

        [Fact(DisplayName = "POST deve salvar CNPJ sem formatação quando valor passado tem formatação")]
        [Trait("Metodo", "Post")]
        public async Task Post_DeveSalvarCnpjSemFormatacao_QuandoValorTemFormatacao()
        {
            // Arrange
            var dtoComFormatacao = new { Nome = "Empresa CNPJ Com Formatação", DocumentoIdentificador = "92.155.590/0001-40" };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/clientes", dtoComFormatacao);
            var clienteEntity = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "92155590000140");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            clienteEntity.Should().NotBeNull();

            // Verificar que o CNPJ foi salvo sem formatação
            clienteEntity.DocumentoIdentificador.Valor.Should().Be("92155590000140");
            clienteEntity.DocumentoIdentificador.Valor.Should().NotContain(".");
            clienteEntity.DocumentoIdentificador.Valor.Should().NotContain("/");
            clienteEntity.DocumentoIdentificador.Valor.Should().NotContain("-");
            clienteEntity.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CNPJ);
        }

        [Fact(DisplayName = "POST deve salvar CPF sem formatação quando valor passado tem formatação")]
        [Trait("Metodo", "Post")]
        public async Task Post_DeveSalvarCpfSemFormatacao_QuandoValorTemFormatacao()
        {
            // Arrange
            var dtoComFormatacao = new { Nome = "Cliente CPF Com Formatação", DocumentoIdentificador = "560.845.420-00" };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/clientes", dtoComFormatacao);
            var clienteEntity = await context.Clientes.FirstOrDefaultAsync(c => c.DocumentoIdentificador.Valor == "56084542000");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            clienteEntity.Should().NotBeNull();

            // Verificar que o CPF foi salvo sem formatação
            clienteEntity.DocumentoIdentificador.Valor.Should().Be("56084542000");
            clienteEntity.DocumentoIdentificador.Valor.Should().NotContain(".");
            clienteEntity.DocumentoIdentificador.Valor.Should().NotContain("-");
            clienteEntity.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CPF);
        }

        [Fact(DisplayName = "GET /documento/{documento} deve retornar 403 quando cliente tenta buscar dados de outro cliente")]
        [Trait("Metodo", "GetByDocumento")]
        public async Task GetByDocumento_Deve_Retornar403_QuandoClienteTentaBuscarDadosDeOutroCliente()
        {
            // Arrange - Criar um cliente no banco de dados
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cpfOutroCliente = DocumentoHelper.GerarCpfValido();
            var clienteEntity = ClienteAggregate.Criar("Cliente Outro", cpfOutroCliente);
            await context.Clientes.AddAsync(clienteEntity);
            await context.SaveChangesAsync();

            // Act - Cliente diferente tenta buscar os dados do primeiro cliente
            var clienteAtualId = Guid.NewGuid(); // ID de um cliente diferente
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteAtualId);

            var response = await clienteAuthenticatedClient.GetAsync($"/api/cadastros/clientes/documento/{cpfOutroCliente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "GET /{id} deve retornar 403 quando cliente tenta buscar dados de outro cliente")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar403_QuandoClienteTentaBuscarDadosDeOutroCliente()
        {
            // Arrange - Criar um cliente no banco de dados
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cpfOutroCliente = DocumentoHelper.GerarCpfValido();
            var clienteEntity = ClienteAggregate.Criar("Cliente Outro", cpfOutroCliente);
            await context.Clientes.AddAsync(clienteEntity);
            await context.SaveChangesAsync();

            // Act - Cliente diferente tenta buscar os dados do primeiro cliente pelo ID
            var clienteAtualId = Guid.NewGuid(); // ID de um cliente diferente
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteAtualId);

            var response = await clienteAuthenticatedClient.GetAsync($"/api/cadastros/clientes/{clienteEntity.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST deve retornar 403 quando usuário tenta criar cliente com documento diferente")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar403_QuandoUsuarioTentaCriarClienteComDocumentoDiferente()
        {
            // Arrange - Criar primeiro cliente e se autenticar como usuário associado
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cpfUsuarioExistente = DocumentoHelper.GerarCpfValido();
            var clienteExistenteEntity = ClienteAggregate.Criar("Cliente Existente", cpfUsuarioExistente);
            await context.Clientes.AddAsync(clienteExistenteEntity);
            await context.SaveChangesAsync();

            var usuarioAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteExistenteEntity.Id);
            var cpfDiferente = DocumentoHelper.GerarCpfValido();

            // Act - Usuário tenta criar novo cliente com CPF diferente do seu
            var dto = new { Nome = "Outro Cliente", DocumentoIdentificador = cpfDiferente };
            var response = await usuarioAuthenticatedClient.PostAsJsonAsync("/api/cadastros/clientes", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "GET deve retornar 403 quando cliente tenta listar clientes")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar403_QuandoClienteTentaListarClientes()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);

            // Act - Cliente tenta listar todos os clientes
            var response = await clienteAuthenticatedClient.GetAsync("/api/cadastros/clientes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
