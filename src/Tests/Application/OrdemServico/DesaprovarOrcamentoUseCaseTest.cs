using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Domain.OrdemServico.Enums;

namespace Tests.Application.OrdemServico
{
    public class DesaprovarOrcamentoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public DesaprovarOrcamentoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando desaprovar orçamento")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoDesaprovarOrcamento()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando domain lançar DomainException")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainLancarDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().Build(); // Ordem sem orçamento

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErroComTipo(ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta desaprovar orçamento de outro cliente")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaDesaprovarOrcamentoDeOutroCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid(); // Cliente diferente do dono do veículo
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            var veiculo = new VeiculoBuilder().ComClienteId(outroClienteId).Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Acesso negado. Apenas administradores ou donos da ordem de serviço podem desaprovar orçamentos.", ErrorType.NotAllowed);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve logar Information quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var mockLogger = MockLogger.Criar();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar Error quando ocorrer exceção genérica")]
        [Trait("UseCase", "DesaprovarOrcamento")]
        public async Task ExecutarAsync_DeveLogarError_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().ComStatus(StatusOrdemServicoEnum.AguardandoAprovacao).Build();
            var veiculo = new VeiculoBuilder().ComClienteId(Guid.NewGuid()).Build();
            var mockLogger = MockLogger.Criar();
            var excecaoEsperada = new InvalidOperationException("Erro de banco de dados");

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.VeiculoGatewayMock.AoObterPorId(ordemServico.VeiculoId).Retorna(veiculo);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(excecaoEsperada);

            // Act
            await _fixture.DesaprovarOrcamentoUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.VeiculoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
