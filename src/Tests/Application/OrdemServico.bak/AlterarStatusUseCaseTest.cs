using Application.Contracts.Presenters;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;

namespace Tests.Application.OrdemServico
{
    public class AlterarStatusUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AlterarStatusUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando alterar status da ordem de serviço")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoAlterarStatus()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServico = new OrdemServicoBuilder().ProntoParaOrcamento().ComServicos().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                StatusOrdemServicoEnum.AguardandoAprovacao,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não for encontrada")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoForEncontrada()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando DomainException ocorrer")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErroDeDominio_QuandoDomainExceptionOcorrer()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();
            var domainException = new DomainException(message: "Regra de domínio quebrada.", ErrorType.DomainRuleBroken);

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(domainException);

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro(domainException.Message, domainException.ErrorType);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var ordemServico = new OrdemServicoBuilder().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro inesperado"));

            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                StatusOrdemServicoEnum.Cancelada,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tenta alterar status de ordem de serviço")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentaAlterarStatus()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var status = StatusOrdemServicoEnum.EmExecucao;

            // Act
            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                status,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Acesso negado. Apenas administradores podem alterar status.", ErrorType.NotAllowed);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando sistema (webhook) altera status da ordem de serviço")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoSistemaAlteraStatus()
        {
            // Arrange
            var ator = new AtorBuilder().ComoSistema().Build();
            var ordemServico = new OrdemServicoBuilder().ComOrcamento().Build();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().Retorna(ordemServico);

            // Act
            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServico.Id,
                StatusOrdemServicoEnum.EmExecucao,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve logar information ao ocorrer DomainException")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveLogarInformation_AoOcorrerDomainException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoCliente(Guid.NewGuid()).Build();
            var ordemServicoId = Guid.NewGuid();
            var novoStatus = StatusOrdemServicoEnum.Finalizada;
            var mockLogger = MockLogger.Criar();

            // Act
            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                novoStatus,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error ao ocorrer Exception")]
        [Trait("UseCase", "AlterarStatus")]
        public async Task ExecutarAsync_DeveLogarError_AoOcorrerException()
        {
            // Arrange
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var ordemServicoId = Guid.NewGuid();
            var novoStatus = StatusOrdemServicoEnum.Finalizada;
            var mockLogger = MockLogger.Criar();
            
            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AlterarStatusUseCase.ExecutarAsync(
                ator,
                ordemServicoId,
                novoStatus,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object,
                mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }
    }
}
