using Application.Cadastros.Dtos;
using Application.OrdemServico.Dtos;
using Domain.Cadastros.Enums;
using Domain.OrdemServico.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Tests.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration.OrdemServico
{
    public class WebhookOrdemServicoControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly HttpClient _hmacClient;

        public WebhookOrdemServicoControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient(); // Para criar dados de teste
            _hmacClient = _factory.CreateHmacClient(); // Para testar webhooks
        }

        #region Webhook AprovarOrcamento Tests

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/aprovar/webhook deve retornar status 204 com dados válidos e HMAC correto")]
        [Trait("Method", "WebhookAprovarOrcamento")]
        public async Task WebhookAprovarOrcamento_ComDadosValidosEHmacCorreto_DeveRetornarStatus204()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente de teste
            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente Webhook Aprovar",
                DocumentoIdentificador = "99477309052"
            };

            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Criar veículo de teste
            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "WHK1234",
                Modelo = "Civic Webhook",
                Marca = "Honda",
                Cor = "Azul",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };

            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Criar serviço de teste
            var servicoDto = new CriarServicoDto
            {
                Nome = "Serviço Webhook Test",
                Preco = 200.00m
            };

            var servicoResponse = await _client.PostAsJsonAsync("/api/cadastros/servicos", servicoDto);
            servicoResponse.EnsureSuccessStatusCode();
            var servico = await servicoResponse.Content.ReadFromJsonAsync<RetornoServicoDto>();

            // Criar ordem de serviço
            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            // Preparar ordem para aprovação (iniciar diagnóstico, adicionar serviço, gerar orçamento)
            await _client.PostAsync($"/api/ordens-servico/{ordem!.Id}/iniciar-diagnostico", null);

            var adicionarServicosDto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = new List<Guid> { servico!.Id }
            };

            await _client.PostAsJsonAsync($"/api/ordens-servico/{ordem.Id}/servicos", adicionarServicosDto);
            await _client.PostAsync($"/api/ordens-servico/{ordem.Id}/orcamento", null);

            // Preparar DTO do webhook
            var webhookDto = new WebhookIdDto { Id = ordem.Id };

            // Act
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/aprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar se o status foi atualizado no banco de dados
            var ordemNoDB = await context.OrdensServico.FirstOrDefaultAsync(o => o.Id == ordem.Id);
            ordemNoDB.Should().NotBeNull();
            ordemNoDB!.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/aprovar/webhook deve retornar status 401 sem assinatura HMAC")]
        [Trait("Method", "WebhookAprovarOrcamento")]
        public async Task WebhookAprovarOrcamento_SemAssinaturaHmac_DeveRetornarStatus401()
        {
            // Arrange
            var webhookDto = new WebhookIdDto { Id = Guid.NewGuid() };

            // Act - usando client normal sem HMAC
            var response = await _client.PostAsJsonAsync("/api/ordens-servico/orcamento/aprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/aprovar/webhook deve retornar status 401 com assinatura HMAC inválida")]
        [Trait("Method", "WebhookAprovarOrcamento")]
        public async Task WebhookAprovarOrcamento_ComAssinaturaHmacInvalida_DeveRetornarStatus401()
        {
            // Arrange
            var webhookDto = new WebhookIdDto { Id = Guid.NewGuid() };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/ordens-servico/orcamento/aprovar/webhook");
            request.Headers.Add("X-Signature", "sha256=assinatura_invalida");
            request.Content = JsonContent.Create(webhookDto);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/aprovar/webhook deve retornar status 404 quando ordem não existir")]
        [Trait("Method", "WebhookAprovarOrcamento")]
        public async Task WebhookAprovarOrcamento_ComOrdemInexistente_DeveRetornarStatus404()
        {
            // Arrange
            var webhookDto = new WebhookIdDto { Id = Guid.NewGuid() };

            // Act
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/aprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/aprovar/webhook deve retornar status 422 quando ordem em status inválido")]
        [Trait("Method", "WebhookAprovarOrcamento")]
        public async Task WebhookAprovarOrcamento_ComStatusInvalido_DeveRetornarStatus422()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente e veículo
            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente Status Inválido",
                DocumentoIdentificador = "29259308089"
            };

            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "INV1234",
                Modelo = "Modelo Invalid",
                Marca = "Marca",
                Cor = "Cor",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };

            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Criar ordem de serviço (que ficará em status Recebida - inválido para aprovação)
            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            var webhookDto = new WebhookIdDto { Id = ordem!.Id };

            // Act - Tentar aprovar orçamento em ordem sem orçamento (status Recebida)
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/aprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        #endregion

        #region Webhook DesaprovarOrcamento Tests

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/desaprovar/webhook deve retornar status 204 com dados válidos")]
        [Trait("Method", "WebhookDesaprovarOrcamento")]
        public async Task WebhookDesaprovarOrcamento_ComDadosValidos_DeveRetornarStatus204()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente de teste
            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente Webhook Desaprovar",
                DocumentoIdentificador = "68400084012"
            };

            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            // Criar veículo de teste
            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "DES1234",
                Modelo = "Civic Desaprovar",
                Marca = "Honda",
                Cor = "Vermelho",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };

            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Criar serviço de teste
            var servicoDto = new CriarServicoDto
            {
                Nome = "Serviço Desaprovar Test",
                Preco = 150.00m
            };

            var servicoResponse = await _client.PostAsJsonAsync("/api/cadastros/servicos", servicoDto);
            servicoResponse.EnsureSuccessStatusCode();
            var servico = await servicoResponse.Content.ReadFromJsonAsync<RetornoServicoDto>();

            // Criar ordem de serviço
            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            // Preparar ordem para desaprovação (iniciar diagnóstico, adicionar serviço, gerar orçamento)
            await _client.PostAsync($"/api/ordens-servico/{ordem!.Id}/iniciar-diagnostico", null);

            var adicionarServicosDto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = new List<Guid> { servico!.Id }
            };

            await _client.PostAsJsonAsync($"/api/ordens-servico/{ordem.Id}/servicos", adicionarServicosDto);
            await _client.PostAsync($"/api/ordens-servico/{ordem.Id}/orcamento", null);

            // Preparar DTO do webhook
            var webhookDto = new WebhookIdDto { Id = ordem.Id };

            // Act
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/desaprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar se o status foi atualizado no banco de dados
            var ordemNoDB = await context.OrdensServico.FirstOrDefaultAsync(o => o.Id == ordem.Id);
            ordemNoDB.Should().NotBeNull();
            ordemNoDB!.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/desaprovar/webhook deve retornar status 401 sem assinatura HMAC")]
        [Trait("Method", "WebhookDesaprovarOrcamento")]
        public async Task WebhookDesaprovarOrcamento_SemAssinaturaHmac_DeveRetornarStatus401()
        {
            // Arrange
            var webhookDto = new WebhookIdDto { Id = Guid.NewGuid() };

            // Act - usando client normal sem HMAC
            var response = await _client.PostAsJsonAsync("/api/ordens-servico/orcamento/desaprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/desaprovar/webhook deve retornar status 404 quando ordem não existir")]
        [Trait("Method", "WebhookDesaprovarOrcamento")]
        public async Task WebhookDesaprovarOrcamento_ComOrdemInexistente_DeveRetornarStatus404()
        {
            // Arrange
            var webhookDto = new WebhookIdDto { Id = Guid.NewGuid() };

            // Act
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/desaprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/orcamento/desaprovar/webhook deve retornar status 422 quando ordem em status inválido")]
        [Trait("Method", "WebhookDesaprovarOrcamento")]
        public async Task WebhookDesaprovarOrcamento_ComStatusInvalido_DeveRetornarStatus422()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Criar cliente e veículo
            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente Status Inválido Desaprovar",
                DocumentoIdentificador = "37462161095"
            };

            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "STV1234",
                Modelo = "Modelo Status Invalid",
                Marca = "Marca",
                Cor = "Cor",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };

            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            // Criar ordem de serviço (que ficará em status Recebida - inválido para desaprovação)
            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            var webhookDto = new WebhookIdDto { Id = ordem!.Id };

            // Act - Tentar desaprovar orçamento em ordem sem orçamento (status Recebida)
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/orcamento/desaprovar/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        #endregion

        #region Webhook AlterarStatus Tests

        [Fact(DisplayName = "POST /api/ordens-servico/status/webhook deve retornar status 422 quando transição de status for inválida")]
        [Trait("Method", "WebhookAlterarStatus")]
        public async Task WebhookAlterarStatus_ComTransicaoInvalida_DeveRetornarStatus422()
        {
            // Arrange: cria cliente, veículo e ordem no status Recebida
            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente AlterarStatus Inválido",
                DocumentoIdentificador = "45026788050"
            };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "ALT1234",
                Modelo = "Modelo Alter",
                Marca = "Marca",
                Cor = "Preto",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };
            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            // Tentar alterar diretamente para EmExecucao deve violar regra de domínio
            var webhookDto = new WebhookAlterarStatusDto { Id = ordem!.Id, Status = StatusOrdemServicoEnum.EmExecucao };

            // Act
            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/status/webhook", webhookDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/status/webhook deve retornar status 401 com assinatura HMAC inválida")]
        [Trait("Method", "WebhookAlterarStatus")]
        public async Task WebhookAlterarStatus_ComAssinaturaHmacInvalida_DeveRetornarStatus401()
        {
            var webhookDto = new WebhookAlterarStatusDto { Id = Guid.NewGuid(), Status = StatusOrdemServicoEnum.EmDiagnostico };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/ordens-servico/status/webhook");
            request.Headers.Add("X-Signature", "sha256=assinatura_invalida");
            request.Content = JsonContent.Create(webhookDto);

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/status/webhook deve retornar status 204 com dados válidos e HMAC correto")]
        [Trait("Method", "WebhookAlterarStatus")]
        public async Task WebhookAlterarStatus_ComDadosValidosEHmacCorreto_DeveRetornarStatus204()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var clienteDto = new CriarClienteDto
            {
                Nome = "Cliente AlterarStatus Sucesso",
                DocumentoIdentificador = "81248990021"
            };
            var clienteResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", clienteDto);
            clienteResponse.EnsureSuccessStatusCode();
            var cliente = await clienteResponse.Content.ReadFromJsonAsync<RetornoClienteDto>();

            var veiculoDto = new CriarVeiculoDto
            {
                ClienteId = cliente!.Id,
                Placa = "ALT2040",
                Modelo = "Modelo OK",
                Marca = "Marca",
                Cor = "Azul",
                Ano = 2023,
                TipoVeiculo = TipoVeiculoEnum.Carro
            };
            var veiculoResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculoDto);
            veiculoResponse.EnsureSuccessStatusCode();
            var veiculo = await veiculoResponse.Content.ReadFromJsonAsync<RetornoVeiculoDto>();

            var ordemDto = new CriarOrdemServicoDto { VeiculoId = veiculo!.Id };
            var ordemResponse = await _client.PostAsJsonAsync("/api/ordens-servico", ordemDto);
            ordemResponse.EnsureSuccessStatusCode();
            var ordem = await ordemResponse.Content.ReadFromJsonAsync<RetornoOrdemServicoDto>();

            var webhookDto = new WebhookAlterarStatusDto { Id = ordem!.Id, Status = StatusOrdemServicoEnum.Cancelada };

            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/status/webhook", webhookDto);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var ordemNoDB = await context.OrdensServico.FirstOrDefaultAsync(o => o.Id == ordem.Id);
            ordemNoDB.Should().NotBeNull();
            ordemNoDB!.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);
        }

        [Fact(DisplayName = "POST /api/ordens-servico/status/webhook deve retornar status 404 quando ordem não existir")]
        [Trait("Method", "WebhookAlterarStatus")]
        public async Task WebhookAlterarStatus_ComOrdemInexistente_DeveRetornarStatus404()
        {
            var webhookDto = new WebhookAlterarStatusDto { Id = Guid.NewGuid(), Status = StatusOrdemServicoEnum.EmDiagnostico };

            var response = await _hmacClient.PostAsJsonWithHmacAsync("/api/ordens-servico/status/webhook", webhookDto);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}