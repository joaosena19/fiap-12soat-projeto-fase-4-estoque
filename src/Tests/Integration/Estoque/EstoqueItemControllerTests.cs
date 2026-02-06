using Application.Estoque.Dtos;
using Domain.Estoque.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration.Estoque
{
    public class EstoqueItemControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EstoqueItemControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Item de Estoque no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirItemEstoque()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dto = new 
            { 
                Nome = "Filtro de Óleo Test",
                Quantidade = 50,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 25.50m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/estoque/itens", dto);
            var itemEstoqueEntity = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Filtro de Óleo Test");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            itemEstoqueEntity.Should().NotBeNull();
            itemEstoqueEntity!.Nome.Valor.Should().Be("Filtro de Óleo Test");
            itemEstoqueEntity.Quantidade.Valor.Should().Be(50);
            itemEstoqueEntity.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Peca);
            itemEstoqueEntity.Preco.Valor.Should().Be(25.50m);
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Item de Estoque existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarItemEstoque()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create item first
            var criarDto = new 
            { 
                Nome = "Filtro de Ar Original",
                Quantidade = 30,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 25.50m
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Filtro de Ar Original");
            itemCriado.Should().NotBeNull();

            var atualizarDto = new 
            { 
                Nome = "Filtro de Ar Premium Atualizado",
                Quantidade = 75,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Insumo,
                Preco = 35.75m
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/estoque/itens/{itemCriado!.Id}", atualizarDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var itemAtualizado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == itemCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            itemAtualizado.Should().NotBeNull();
            itemAtualizado!.Nome.Valor.Should().Be("Filtro de Ar Premium Atualizado");
            itemAtualizado.Quantidade.Valor.Should().Be(75);
            itemAtualizado.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Insumo);
            itemAtualizado.Preco.Valor.Should().Be(35.75m);
        }

        [Fact(DisplayName = "PATCH deve retornar 200 OK e atualizar apenas a quantidade do Item de Estoque.")]
        [Trait("Metodo", "UpdateQuantidade")]
        public async Task UpdateQuantidade_Deve_Retornar200OK_E_AtualizarQuantidade()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create item first
            var criarDto = new 
            { 
                Nome = "Óleo Motor para Quantidade",
                Quantidade = 20,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Insumo,
                Preco = 15.75m
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Óleo Motor para Quantidade");
            itemCriado.Should().NotBeNull();

            var atualizarQuantidadeDto = new { Quantidade = 100 };

            // Act
            var updateResponse = await _client.PatchAsJsonAsync($"/api/estoque/itens/{itemCriado!.Id}/quantidade", atualizarQuantidadeDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var itemAtualizado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == itemCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            itemAtualizado.Should().NotBeNull();
            itemAtualizado!.Nome.Valor.Should().Be("Óleo Motor para Quantidade"); // Nome não deve mudar
            itemAtualizado.Quantidade.Valor.Should().Be(100);
            itemAtualizado.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Insumo); // Tipo não deve mudar
            itemAtualizado.Preco.Valor.Should().Be(15.75m); // Preco não deve mudar
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de itens de estoque")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeItensEstoque()
        {
            // Arrange
            var item1 = new 
            { 
                Nome = "Pneu Novo",
                Quantidade = 4,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 150.00m
            };
            var item2 = new 
            { 
                Nome = "Aditivo Radiador",
                Quantidade = 10,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Insumo,
                Preco = 25.50m
            };

            // Create test items
            await _client.PostAsJsonAsync("/api/estoque/itens", item1);
            await _client.PostAsJsonAsync("/api/estoque/itens", item2);

            // Act
            var response = await _client.GetAsync("/api/estoque/itens");
            var itens = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoItemEstoqueDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            itens.Should().NotBeNull();
            itens.Should().HaveCountGreaterThanOrEqualTo(2);

            var itensLista = itens!.ToList();
            itensLista.Should().Contain(i => i.Nome == "Pneu Novo");
            itensLista.Should().Contain(i => i.Nome == "Aditivo Radiador");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há itens de estoque")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaItensEstoque()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database is clean for this test
            context.ItensEstoque.RemoveRange(context.ItensEstoque);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/estoque/itens");
            var itens = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoItemEstoqueDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            itens.Should().NotBeNull();
            itens.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e item de estoque específico")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_ItemEstoqueEspecifico()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var criarDto = new 
            { 
                Nome = "Bateria 60Ah",
                Quantidade = 5,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 250.00m
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Bateria 60Ah");
            itemCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/estoque/itens/{itemCriado!.Id}");
            var item = await response.Content.ReadFromJsonAsync<RetornoItemEstoqueDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            item.Should().NotBeNull();
            item!.Id.Should().Be(itemCriado.Id);
            item.Nome.Should().Be("Bateria 60Ah");
            item.Quantidade.Should().Be(5);
            item.TipoItemEstoque.Should().Be(TipoItemEstoqueEnum.Peca.ToString());
            item.Preco.Should().Be(250.00m);
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando item de estoque não existe")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoItemEstoqueNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/estoque/itens/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /{id}/disponibilidade deve retornar 200 OK e informações de disponibilidade")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_Deve_Retornar200OK_E_InformacoesDisponibilidade()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var criarDto = new 
            { 
                Nome = "Vela de Ignição",
                Quantidade = 20,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Vela de Ignição");
            itemCriado.Should().NotBeNull();

            var quantidadeRequisitada = 15;

            // Act
            var response = await _client.GetAsync($"/api/estoque/itens/{itemCriado!.Id}/disponibilidade?quantidadeRequisitada={quantidadeRequisitada}");
            var disponibilidade = await response.Content.ReadFromJsonAsync<RetornoDisponibilidadeDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            disponibilidade.Should().NotBeNull();
            disponibilidade!.Disponivel.Should().BeTrue();
            disponibilidade.QuantidadeEmEstoque.Should().Be(20);
            disponibilidade.QuantidadeSolicitada.Should().Be(15);
        }

        [Fact(DisplayName = "GET /{id}/disponibilidade deve retornar 200 OK e indisponível quando não há estoque suficiente")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_Deve_Retornar200OK_E_Indisponivel_QuandoNaoHaEstoqueSuficiente()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var criarDto = new 
            { 
                Nome = "Pastilha de Freio",
                Quantidade = 5,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Pastilha de Freio");
            itemCriado.Should().NotBeNull();

            var quantidadeRequisitada = 10; // Mais do que tem em estoque

            // Act
            var response = await _client.GetAsync($"/api/estoque/itens/{itemCriado!.Id}/disponibilidade?quantidadeRequisitada={quantidadeRequisitada}");
            var disponibilidade = await response.Content.ReadFromJsonAsync<RetornoDisponibilidadeDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            disponibilidade.Should().NotBeNull();
            disponibilidade!.Disponivel.Should().BeFalse();
            disponibilidade.QuantidadeEmEstoque.Should().Be(5);
            disponibilidade.QuantidadeSolicitada.Should().Be(10);
        }

        [Fact(DisplayName = "GET /{id}/disponibilidade deve retornar 404 NotFound quando item de estoque não existe")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_Deve_Retornar404NotFound_QuandoItemEstoqueNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var quantidadeRequisitada = 5;

            // Act
            var response = await _client.GetAsync($"/api/estoque/itens/{idInexistente}/disponibilidade?quantidadeRequisitada={quantidadeRequisitada}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "POST deve retornar 409 Conflict quando já existe item com mesmo nome")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar409Conflict_QuandoJaExisteItemComMesmoNome()
        {
            // Arrange
            var dto = new 
            { 
                Nome = "Item Duplicado Test",
                Quantidade = 10,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            // Create first item
            var firstResponse = await _client.PostAsJsonAsync("/api/estoque/itens", dto);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act - Try to create duplicate
            var duplicateResponse = await _client.PostAsJsonAsync("/api/estoque/itens", dto);

            // Assert
            duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact(DisplayName = "PUT deve retornar 404 NotFound quando item de estoque não existe")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar404NotFound_QuandoItemEstoqueNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var atualizarDto = new 
            { 
                Nome = "Item Inexistente",
                Quantidade = 5,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/estoque/itens/{idInexistente}", atualizarDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "PATCH deve retornar 404 NotFound quando item de estoque não existe")]
        [Trait("Metodo", "UpdateQuantidade")]
        public async Task UpdateQuantidade_Deve_Retornar404NotFound_QuandoItemEstoqueNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            var atualizarQuantidadeDto = new { Quantidade = 10 };

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/estoque/itens/{idInexistente}/quantidade", atualizarQuantidadeDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "POST deve salvar TipoItemEstoque sempre em lowercase independente do input")]
        [Trait("Lowercase", "TipoItemEstoque")]
        public async Task Post_DeveSalvarTipoItemEstoqueSempreEmLowercase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dto = new 
            { 
                Nome = "Filtro de Óleo Teste Casing",
                Quantidade = 50,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/estoque/itens", dto);
            var itemEstoqueEntity = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Filtro de Óleo Teste Casing");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            itemEstoqueEntity.Should().NotBeNull();
            itemEstoqueEntity!.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Peca);
        }

        [Fact(DisplayName = "PUT deve salvar TipoItemEstoque sempre em lowercase independente do input")]
        [Trait("Lowercase", "TipoItemEstoque")]
        public async Task Put_DeveSalvarTipoItemEstoqueSempreEmLowercase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create item first
            var criarDto = new 
            { 
                Nome = "Item para Update",
                Quantidade = 30,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca
            };

            var createResponse = await _client.PostAsJsonAsync("/api/estoque/itens", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var itemCriado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Nome.Valor == "Item para Update");
            itemCriado.Should().NotBeNull();

            var atualizarDto = new 
            { 
                Nome = "Item Atualizado",
                Quantidade = 75,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Insumo // Update para Insumo
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/estoque/itens/{itemCriado!.Id}", atualizarDto);
            
            // Clear EF tracking
            context.ChangeTracker.Clear();
            var itemAtualizado = await context.ItensEstoque.FirstOrDefaultAsync(i => i.Id == itemCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            itemAtualizado.Should().NotBeNull();
            itemAtualizado!.TipoItemEstoque.Valor.Should().Be(TipoItemEstoqueEnum.Insumo);
        }

        [Fact(DisplayName = "GET deve retornar 403 Forbidden quando cliente tenta listar itens de estoque")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar403Forbidden_QuandoClienteTentaListarItensEstoque()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);

            // Act - Cliente tenta listar itens de estoque
            var response = await clienteAuthenticatedClient.GetAsync("/api/estoque/itens");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "GET /{id} deve retornar 403 Forbidden quando cliente tenta buscar item de estoque por ID")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar403Forbidden_QuandoClienteTentaBuscarItemEstoquePorId()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var itemId = Guid.NewGuid();

            // Act - Cliente tenta buscar item de estoque por ID
            var response = await clienteAuthenticatedClient.GetAsync($"/api/estoque/itens/{itemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST deve retornar 403 Forbidden quando cliente tenta criar item de estoque")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar403Forbidden_QuandoClienteTentaCriarItemEstoque()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var dto = new 
            { 
                Nome = "Item não autorizado",
                Quantidade = 10,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 100.00m
            };

            // Act - Cliente tenta criar item de estoque
            var response = await clienteAuthenticatedClient.PostAsJsonAsync("/api/estoque/itens", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "PUT deve retornar 403 Forbidden quando cliente tenta atualizar item de estoque")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar403Forbidden_QuandoClienteTentaAtualizarItemEstoque()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var itemId = Guid.NewGuid();
            var dto = new 
            { 
                Nome = "Item atualizado não autorizado",
                Quantidade = 20,
                TipoItemEstoque = (int)TipoItemEstoqueEnum.Peca,
                Preco = 200.00m
            };

            // Act - Cliente tenta atualizar item de estoque
            var response = await clienteAuthenticatedClient.PutAsJsonAsync($"/api/estoque/itens/{itemId}", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "PATCH deve retornar 403 Forbidden quando cliente tenta atualizar quantidade de item de estoque")]
        [Trait("Metodo", "UpdateQuantidade")]
        public async Task UpdateQuantidade_Deve_Retornar403Forbidden_QuandoClienteTentaAtualizarQuantidade()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var itemId = Guid.NewGuid();
            var dto = new { Quantidade = 50 };

            // Act - Cliente tenta atualizar quantidade
            var response = await clienteAuthenticatedClient.PatchAsJsonAsync($"/api/estoque/itens/{itemId}/quantidade", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "GET /{id}/disponibilidade deve retornar 403 Forbidden quando cliente tenta verificar disponibilidade")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_Deve_Retornar403Forbidden_QuandoClienteTentaVerificarDisponibilidade()
        {
            // Arrange - Cliente autenticado (não admin)
            var clienteId = Guid.NewGuid();
            var clienteAuthenticatedClient = _factory.CreateAuthenticatedClient(isAdmin: false, clienteId: clienteId);
            var itemId = Guid.NewGuid();
            var quantidadeRequisitada = 5;

            // Act - Cliente tenta verificar disponibilidade
            var response = await clienteAuthenticatedClient.GetAsync($"/api/estoque/itens/{itemId}/disponibilidade?quantidadeRequisitada={quantidadeRequisitada}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
