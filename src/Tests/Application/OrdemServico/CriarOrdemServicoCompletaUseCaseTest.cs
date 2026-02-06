using Domain.OrdemServico.Enums;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class CriarOrdemServicoCompletaUseCaseTest
    {
        private readonly CriarOrdemServicoCompletaTestFixture _fixture;

        public CriarOrdemServicoCompletaUseCaseTest()
        {
            _fixture = new CriarOrdemServicoCompletaTestFixture();
        }

        [Fact(DisplayName = "Deve retornar NotAllowed quando cliente tenta criar ordem de serviço completa")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveRetornarNotAllowed_QuandoClienteTentaCriarOrdemServicoCompleta()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, 
                MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.PresenterMock.Verify(p => p.ApresentarErro("Acesso negado. Apenas administradores podem criar ordens de serviço completas.", ErrorType.NotAllowed), Times.Once);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
            _fixture.ClienteGatewayMock.Verify(c => c.ObterPorDocumentoAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar ordem de serviço completa com cliente e veículo novos")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoCompleta_ComClienteEVeiculoNovos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteCriado = new ClienteBuilder().ComNome(dto.Cliente.Nome).Build();
            var veiculoCriado = new VeiculoBuilder().ComClienteId(clienteCriado.Id).ComPlaca(dto.Veiculo.Placa).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(clienteCriado);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculoCriado);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.VeiculoId.Should().Be(veiculoCriado.Id);
            ordemServicoSalva.Status.Valor.Should().Be(StatusOrdemServicoEnum.Recebida);
            _fixture.ClienteGatewayMock.DeveTerSalvadoCliente();
            _fixture.VeiculoGatewayMock.DeveTerSalvadoVeiculo();
            _fixture.OrdemServicoGatewayMock.DeveTerSalvoOrdemServico();
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve criar ordem de serviço completa com cliente e veículo existentes")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoCompleta_ComClienteEVeiculoExistentes()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteExistente = new ClienteBuilder().ComNome(dto.Cliente.Nome).Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).ComPlaca(dto.Veiculo.Placa).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.VeiculoId.Should().Be(veiculoExistente.Id);
            _fixture.ClienteGatewayMock.NaoDeveTerSalvadoCliente();
            _fixture.VeiculoGatewayMock.NaoDeveTerSalvadoVeiculo();
            _fixture.OrdemServicoGatewayMock.DeveTerSalvoOrdemServico();
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve adicionar serviços encontrados à ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveAdicionarServicos_QuandoEncontrados()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servico1 = new ServicoBuilder().Build();
            var servico2 = new ServicoBuilder().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder()
                .ComServicos(servico1.Id, servico2.Id)
                .Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.ServicoGatewayMock.AoObterPorId(servico1.Id).Retorna(servico1);
            _fixture.ServicoGatewayMock.AoObterPorId(servico2.Id).Retorna(servico2);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.ServicosIncluidos.Should().HaveCount(2);
            ordemServicoSalva.ServicosIncluidos.Should().Contain(s => s.ServicoOriginalId == servico1.Id);
            ordemServicoSalva.ServicosIncluidos.Should().Contain(s => s.ServicoOriginalId == servico2.Id);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve ignorar serviços não encontrados")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveIgnorarServicos_QuandoNaoEncontrados()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoInexistente1 = Guid.NewGuid();
            var servicoInexistente2 = Guid.NewGuid();
            var dto = new CriarOrdemServicoCompletaDtoBuilder()
                .ComServicos(servicoInexistente1, servicoInexistente2)
                .Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.ServicoGatewayMock.AoObterPorId(servicoInexistente1).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoObterPorId(servicoInexistente2).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.ServicosIncluidos.Should().BeEmpty();
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve adicionar itens encontrados à ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveAdicionarItens_QuandoEncontrados()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var item1 = new ItemEstoqueBuilder().Build();
            var item2 = new ItemEstoqueBuilder().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder()
                .ComItens((item1.Id, 2), (item2.Id, 3))
                .Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(item1.Id).Retorna(item1);
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(item2.Id).Retorna(item2);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.ItensIncluidos.Should().HaveCount(2);
            ordemServicoSalva.ItensIncluidos.Should().Contain(i => i.ItemEstoqueOriginalId == item1.Id && i.Quantidade.Valor == 2);
            ordemServicoSalva.ItensIncluidos.Should().Contain(i => i.ItemEstoqueOriginalId == item2.Id && i.Quantidade.Valor == 3);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve ignorar itens não encontrados")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveIgnorarItens_QuandoNaoEncontrados()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var itemInexistente1 = Guid.NewGuid();
            var itemInexistente2 = Guid.NewGuid();
            var dto = new CriarOrdemServicoCompletaDtoBuilder()
                .ComItens((itemInexistente1, 2), (itemInexistente2, 3))
                .Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemInexistente1).NaoRetornaNada();
            _fixture.ItemEstoqueGatewayMock.AoObterPorId(itemInexistente2).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.ItensIncluidos.Should().BeEmpty();
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve criar ordem de serviço sem serviços nem itens")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServico_SemServicosNemItens()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder()
                .SemServicos()
                .SemItens()
                .Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.ServicosIncluidos.Should().BeEmpty();
            ordemServicoSalva.ItensIncluidos.Should().BeEmpty();
            ordemServicoSalva.Status.Valor.Should().Be(StatusOrdemServicoEnum.Recebida);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve gerar código único para ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveGerarCodigoUnico_ParaOrdemServico()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            var ordemServicoExistente = new OrdemServicoBuilder().ComVeiculoId(Guid.NewGuid()).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            
            // Simula que na primeira tentativa o código já existe, na segunda não
            _fixture.OrdemServicoGatewayMock.SetupSequence(g => g.ObterPorCodigoAsync(It.IsAny<string>()))
                .ReturnsAsync(ordemServicoExistente)
                .ReturnsAsync((OrdemServicoAggregate?)null);
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            ordemServicoSalva!.Codigo.Valor.Should().NotBe(ordemServicoExistente.Codigo.Valor);
            ordemServicoSalva.Codigo.Valor.Should().StartWith("OS-");
            ordemServicoSalva.Codigo.Valor.Should().HaveLength(18);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(ordemServicoSalva), Times.Once);
        }

        [Fact(DisplayName = "Deve apresentar erro quando ocorrer exceção de domínio")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOcorrerExcecaoDominio()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var mensagemErro = "Erro de domínio personalizado";
            var tipoErro = ErrorType.DomainRuleBroken;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador)
                .LancaExcecao(new DomainException(mensagemErro, tipoErro));

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.PresenterMock.Verify(p => p.ApresentarErro(mensagemErro, tipoErro), Times.Once);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
            _fixture.OrdemServicoGatewayMock.NaoDeveTerSalvoOrdemServico();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.UseCase.ExecutarAsync(ator, dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object, MockLogger.CriarSimples(), _fixture.MetricsServiceMock.Object);

            // Assert
            _fixture.PresenterMock.Verify(p => p.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError), Times.Once);
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve logar Information quando ator não for administrador")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoAtorNaoForAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteExistente = new ClienteBuilder().Build();
            var veiculoExistente = new VeiculoBuilder().ComClienteId(clienteExistente.Id).Build();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).Retorna(clienteExistente);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).Retorna(veiculoExistente);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }

        [Fact(DisplayName = "Deve registrar métrica de ordem de serviço criada completa")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveRegistrarMetricaOrdemServicoCriada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteCriado = new ClienteBuilder().ComDocumento(dto.Cliente.DocumentoIdentificador).Build();
            var veiculoCriado = new VeiculoBuilder().ComClienteId(clienteCriado.Id).ComPlaca(dto.Veiculo.Placa).Build();
            OrdemServicoAggregate? ordemServicoSalva = null;

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(clienteCriado);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculoCriado);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => ordemServicoSalva = os);

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object,
                MockLogger.CriarSimples(),
                _fixture.MetricsServiceMock.Object);

            // Assert
            ordemServicoSalva.Should().NotBeNull();
            _fixture.MetricsServiceMock.DeveTerRegistradoOrdemServicoCriada(ordemServicoSalva!.Id, clienteCriado.Id, ator.UsuarioId);
        }

        [Fact(DisplayName = "Deve logar erro quando registro de métrica falhar em ordem de serviço completa")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveLogarErro_QuandoRegistroMetricaFalhar()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var dto = new CriarOrdemServicoCompletaDtoBuilder().Build();
            var clienteCriado = new ClienteBuilder().ComDocumento(dto.Cliente.DocumentoIdentificador).Build();
            var veiculoCriado = new VeiculoBuilder().ComClienteId(clienteCriado.Id).ComPlaca(dto.Veiculo.Placa).Build();
            var mockLogger = MockLogger.Criar();
            var excecaoMetrica = new Exception("Erro ao registrar métrica");

            _fixture.ClienteGatewayMock.AoObterPorDocumento(dto.Cliente.DocumentoIdentificador).NaoRetornaNada();
            _fixture.ClienteGatewayMock.AoSalvar().Retorna(clienteCriado);
            _fixture.VeiculoGatewayMock.AoObterPorPlaca(dto.Veiculo.Placa).NaoRetornaNada();
            _fixture.VeiculoGatewayMock.AoSalvar().Retorna(veiculoCriado);
            _fixture.OrdemServicoGatewayMock.AoObterPorCodigo(It.IsAny<string>()).NaoRetornaNada();
            _fixture.OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { });
            _fixture.MetricsServiceMock.AoRegistrarOrdemServicoCriada().LancaExcecao(excecaoMetrica);

            // Act
            await _fixture.UseCase.ExecutarAsync(
                ator,
                dto,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ClienteGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.ServicoGatewayMock.Object,
                _fixture.ItemEstoqueGatewayMock.Object,
                _fixture.PresenterMock.Object,
                mockLogger.Object,
                _fixture.MetricsServiceMock.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
            _fixture.PresenterMock.Verify(p => p.ApresentarSucesso(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }
    }
}
