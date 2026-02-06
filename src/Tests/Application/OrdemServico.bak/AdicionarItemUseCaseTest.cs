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
    public class AdicionarItemUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AdicionarItemUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve adicionar item com sucesso quando ordem de serviço existir e item de estoque for válido")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveAdicionarItemComSucesso_QuandoOrdemServicoExistirEItemEstoqueForValido()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidade = 2;
            var logger = MockLogger.CriarSimples();

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemEstoque.Id,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object,
                logger);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.ItensIncluidos.Should().HaveCount(1);

            var itemAdicionado = ordemServicoAtualizada.ItensIncluidos.First();
            itemAdicionado.ItemEstoqueOriginalId.Should().Be(itemEstoque.Id);
            itemAdicionado.Nome.Valor.Should().Be(itemEstoque.Nome);
            itemAdicionado.Preco.Valor.Should().Be(itemEstoque.Preco);
            itemAdicionado.Quantidade.Valor.Should().Be(quantidade);

            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>(ordemServicoAtualizada);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item de estoque não existir")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemEstoqueNaoExistir()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoqueId).NaoRetornaNada();

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>($"Item de estoque com ID {itemEstoqueId} não encontrado.", ErrorType.ReferenceNotFound);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidadeInvalida = -1; // Quantidade inválida para provocar DomainException

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemEstoque.Id,
                quantidadeInvalida,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("A quantidade deve ser maior que zero.", ErrorType.InvalidInput);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidade = 1;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemEstoque.Id,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta adicionar item em ordem de serviço")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAdicionarItem()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServico = new OrdemServicoBuilder().Build();
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidade = 1;

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                itemEstoque.Id,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Acesso negado. Apenas administradores podem adicionar itens.", ErrorType.NotAllowed);
            _fixture.AdicionarItemPresenterMock.NaoDeveTerApresentadoSucesso<IAdicionarItemPresenter, OrdemServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "AdicionarItem")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var itemEstoqueId = Guid.NewGuid();
            var quantidade = 1;
            var mockLogger = MockLogger.Criar();
            
            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AdicionarItemUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                itemEstoqueId,
                quantidade,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.AdicionarItemPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
            _fixture.AdicionarItemPresenterMock.DeveTerApresentadoErro<IAdicionarItemPresenter, OrdemServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}
