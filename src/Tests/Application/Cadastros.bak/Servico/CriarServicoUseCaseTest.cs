using Application.Contracts.Presenters;
using Application.Identidade.Services;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Servico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class CriarServicoUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public CriarServicoUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve criar serviço com sucesso quando é administrador")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveCriarServicoComSucesso_QuandoEhAdministrador()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoEsperado = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150.00m).Build();
            var logger = MockLogger.CriarSimples();
            ServicoAggregate? servicoCriado = null;

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoEsperado.Nome.Valor).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoSalvar().ComCallback(servico => servicoCriado = servico);

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoEsperado.Nome.Valor, servicoEsperado.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, logger);

            // Assert
            servicoCriado.Should().NotBeNull();
            servicoCriado!.Nome.Valor.Should().Be(servicoEsperado.Nome.Valor);
            servicoCriado.Preco.Valor.Should().Be(servicoEsperado.Preco.Valor);

            _fixture.CriarServicoPresenterMock.DeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>(servicoCriado);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta criar serviço")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaCriarServico()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var servicoEsperado = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150.00m).Build();
            var logger = MockLogger.CriarSimples();

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoEsperado.Nome.Valor, servicoEsperado.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, logger);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Acesso negado. Apenas administradores podem criar serviços.", ErrorType.NotAllowed);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de conflito quando já existir serviço com mesmo nome")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroConflito_QuandoJaExistirServicoComMesmoNome()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoExistente = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(100m).Build();
            var servicoParaTentar = new ServicoBuilder().ComNome(servicoExistente.Nome.Valor).ComPreco(150m).Build();
            var logger = MockLogger.CriarSimples();

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoExistente.Nome.Valor).Retorna(servicoExistente);

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoParaTentar.Nome.Valor, servicoParaTentar.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, logger);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var nomeInvalido = "";
            var precoValido = 100m;
            var logger = MockLogger.CriarSimples();

            _fixture.ServicoGatewayMock.AoObterPorNome(nomeInvalido).NaoRetornaNada();

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, nomeInvalido, precoValido,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, logger);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Nome não pode ser vazio", ErrorType.InvalidInput);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoParaCriar = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150m).Build();
            var logger = MockLogger.CriarSimples();

            _fixture.ServicoGatewayMock.AoObterPorNome(servicoParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoParaCriar.Nome.Valor, servicoParaCriar.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, logger);

            // Assert
            _fixture.CriarServicoPresenterMock.DeveTerApresentadoErro<ICriarServicoPresenter, ServicoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.CriarServicoPresenterMock.NaoDeveTerApresentadoSucesso<ICriarServicoPresenter, ServicoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoExistente = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(100m).Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ServicoGatewayMock.AoObterPorNome(servicoExistente.Nome.Valor).Retorna(servicoExistente);

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoExistente.Nome.Valor, 150m,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "CriarServico")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var servicoParaCriar = new ServicoBuilder().ComNome("Troca de Óleo").ComPreco(150m).Build();
            var mockLogger = MockLogger.Criar();
            _fixture.ServicoGatewayMock.AoObterPorNome(servicoParaCriar.Nome.Valor).NaoRetornaNada();
            _fixture.ServicoGatewayMock.AoSalvar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.CriarServicoUseCase.ExecutarAsync(
                ator, servicoParaCriar.Nome.Valor, servicoParaCriar.Preco.Valor,
                _fixture.ServicoGatewayMock.Object, _fixture.CriarServicoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}