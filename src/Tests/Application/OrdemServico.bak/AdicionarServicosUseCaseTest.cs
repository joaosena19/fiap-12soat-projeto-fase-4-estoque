using Application.Contracts.Presenters;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class AdicionarServicosUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AdicionarServicosUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve adicionar serviços com sucesso quando ordem de serviço existir e serviços forem válidos")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveAdicionarServicosComSucesso_QuandoOrdemServicoExistirEServicosForemValidos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servico1 = new ServicoExternalDtoBuilder().Build();
            var servico2 = new ServicoExternalDtoBuilder().Build();
            var servicosIds = new List<Guid> { servico1.Id, servico2.Id };

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servico1.Id).Retorna(servico1);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servico2.Id).Retorna(servico2);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.ServicosIncluidos.Should().HaveCount(2);

            var primeiroServico = ordemServicoAtualizada.ServicosIncluidos.First(s => s.ServicoOriginalId == servico1.Id);
            primeiroServico.ServicoOriginalId.Should().Be(servico1.Id);
            primeiroServico.Nome.Valor.Should().Be(servico1.Nome);
            primeiroServico.Preco.Valor.Should().Be(servico1.Preco);

            var segundoServico = ordemServicoAtualizada.ServicosIncluidos.First(s => s.ServicoOriginalId == servico2.Id);
            segundoServico.ServicoOriginalId.Should().Be(servico2.Id);
            segundoServico.Nome.Valor.Should().Be(servico2.Nome);
            segundoServico.Preco.Valor.Should().Be(servico2.Preco);

            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>(ordemServicoAtualizada);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();
            var servicosIds = new List<Guid> { servicoId };

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando serviço não existir")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servicoId = Guid.NewGuid();
            var servicosIds = new List<Guid> { servicoId };

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servicoId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>($"Serviço com ID {servicoId} não encontrado.", ErrorType.ReferenceNotFound);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando lista de serviços for nula")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoListaServicosForNula()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            List<Guid>? servicosIds = null;

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                servicosIds!,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("É necessário informar ao menos um serviço para adicionar na Ordem de Serviço", ErrorType.InvalidInput);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando lista de serviços for vazia")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoListaServicosForVazia()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var servicosIds = new List<Guid>();

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("É necessário informar ao menos um serviço para adicionar na Ordem de Serviço", ErrorType.InvalidInput);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servico = new ServicoExternalDtoBuilder().Build();
            var servicosIds = new List<Guid> { servico.Id };

            // Primeiro adiciona o serviço
            ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servico.Id).Retorna(servico);

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("Este serviço já foi incluído nesta OS.", ErrorType.DomainRuleBroken);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servico = new ServicoExternalDtoBuilder().Build();
            var servicosIds = new List<Guid> { servico.Id };

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servico.Id).Retorna(servico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve adicionar um único serviço com sucesso")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveAdicionarUmServicoComSucesso()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servico = new ServicoExternalDtoBuilder().Build();
            var servicosIds = new List<Guid> { servico.Id };

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.ServicoExternalServiceMock.AoObterServicoPorId(servico.Id).Retorna(servico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.ServicosIncluidos.Should().HaveCount(1);

            var servicoAdicionado = ordemServicoAtualizada.ServicosIncluidos.First();
            servicoAdicionado.ServicoOriginalId.Should().Be(servico.Id);
            servicoAdicionado.Nome.Valor.Should().Be(servico.Nome);
            servicoAdicionado.Preco.Valor.Should().Be(servico.Preco);

            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>(ordemServicoAtualizada);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta adicionar serviços em ordem de serviço")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAdicionarServicos()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var servicosIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarServicosPresenterMock.DeveTerApresentadoErro<IAdicionarServicosPresenter, OrdemServicoAggregate>("Acesso negado. Apenas administradores podem adicionar serviços.", ErrorType.NotAllowed);
            _fixture.AdicionarServicosPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarServicosPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var servicosIds = new List<Guid> { Guid.NewGuid() };
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "AdicionarServicos")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var servicosIds = new List<Guid> { Guid.NewGuid() };
            var mockLogger = MockLogger.Criar();
            
            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AdicionarServicosUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                servicosIds,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.ServicoExternalServiceMock.Object,
                _fixture.AdicionarServicosPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
