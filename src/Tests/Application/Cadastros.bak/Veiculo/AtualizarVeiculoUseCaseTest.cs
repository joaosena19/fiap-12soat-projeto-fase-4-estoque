using Application.Contracts.Presenters;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Application.SharedHelpers.AggregateBuilders;
using Tests.Application.SharedHelpers.Gateways;
using Tests.Application.SharedHelpers;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class AtualizarVeiculoUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public AtualizarVeiculoUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve atualizar veículo com sucesso quando veículo existir e ator for administrador")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveAtualizarVeiculoComSucesso_QuandoVeiculoExistirEAtorForAdministrador()
        {
            // Arrange
            var veiculoExistente = new VeiculoBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var novoModelo = "Novo Modelo";
            var novaMarca = "Nova Marca";
            var novaCor = "Nova Cor";
            var novoAno = veiculoExistente.Ano.Valor + 1;

            VeiculoAggregate? veiculoAtualizado = null;

            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoExistente.Id).Retorna(veiculoExistente);
            _fixture.VeiculoGatewayMock.AoAtualizar().ComCallback(veiculo => veiculoAtualizado = veiculo);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculoExistente.Id, novoModelo, novaMarca, novaCor, novoAno, veiculoExistente.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            veiculoAtualizado.Should().NotBeNull();
            veiculoAtualizado!.Modelo.Valor.Should().Be(novoModelo);
            veiculoAtualizado!.Marca.Valor.Should().Be(novaMarca);
            veiculoAtualizado!.Ano.Valor.Should().Be(novoAno);
            veiculoAtualizado!.Cor.Valor.Should().Be(novaCor);

            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarVeiculoPresenter, VeiculoAggregate>();
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve atualizar veículo com sucesso quando cliente for dono do veículo")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveAtualizarVeiculoComSucesso_QuandoClienteForDonoDoVeiculo()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoExistente = new VeiculoBuilder().ComCliente(clienteId).Build();
            var ator = new AtorBuilder().ComoCliente(clienteId).Build();
            var novoModelo = "Novo Modelo";

            VeiculoAggregate? veiculoAtualizado = null;

            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoExistente.Id).Retorna(veiculoExistente);
            _fixture.VeiculoGatewayMock.AoAtualizar().ComCallback(veiculo => veiculoAtualizado = veiculo);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculoExistente.Id, novoModelo, veiculoExistente.Marca.Valor, veiculoExistente.Cor.Valor, 
                veiculoExistente.Ano.Valor, veiculoExistente.TipoVeiculo.Valor,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            veiculoAtualizado.Should().NotBeNull();
            veiculoAtualizado!.Modelo.Valor.Should().Be(novoModelo);

            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoSucessoComQualquerObjeto<IAtualizarVeiculoPresenter, VeiculoAggregate>();
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando veículo não existir")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoId).NaoRetornaNada();

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculoId, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Veículo não encontrado.", ErrorType.ResourceNotFound);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro quando cliente tentar atualizar veículo de outro cliente")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoClienteTentarAtualizarVeiculoDeOutroCliente()
        {
            // Arrange
            var clienteDonoId = Guid.NewGuid();
            var clienteAtorId = Guid.NewGuid();
            var veiculoExistente = new VeiculoBuilder().ComCliente(clienteDonoId).Build();
            var ator = new AtorBuilder().ComoCliente(clienteAtorId).Build();

            _fixture.VeiculoGatewayMock.AoObterPorId(veiculoExistente.Id).Retorna(veiculoExistente);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculoExistente.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Acesso negado ao veículo.", ErrorType.NotAllowed);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var erroDominio = new Shared.Exceptions.DomainException("Erro de domínio", ErrorType.DomainRuleBroken);
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(erroDominio);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Erro de domínio", ErrorType.DomainRuleBroken);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }

        [Fact(DisplayName = "Deve logar information quando DomainException for lançada")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveLogarInformation_QuandoDomainExceptionForLancada()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var erroDominio = new Shared.Exceptions.DomainException("Erro de domínio", ErrorType.DomainRuleBroken);
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(erroDominio);

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoInformation();
        }

        [Fact(DisplayName = "Deve logar error quando Exception genérica for lançada")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveLogarError_QuandoExceptionGenericaForLancada()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            var mockLogger = MockLogger.Criar();
            
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, mockLogger.Object);

            // Assert
            mockLogger.DeveTerLogadoErrorComException();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AtualizarVeiculo")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var veiculo = new VeiculoBuilder().Build();
            var ator = new AtorBuilder().ComoAdministrador().Build();
            _fixture.VeiculoGatewayMock.AoObterPorId(veiculo.Id).Retorna(veiculo);
            _fixture.VeiculoGatewayMock.AoAtualizar().LancaExcecao(new Exception("Falha inesperada"));

            // Act
            await _fixture.AtualizarVeiculoUseCase.ExecutarAsync(
                ator, veiculo.Id, "Modelo", "Marca", "Cor", 2023, TipoVeiculoEnum.Carro,
                _fixture.VeiculoGatewayMock.Object, _fixture.AtualizarVeiculoPresenterMock.Object, MockLogger.CriarSimples());

            // Assert
            _fixture.AtualizarVeiculoPresenterMock.DeveTerApresentadoErro<IAtualizarVeiculoPresenter, VeiculoAggregate>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.AtualizarVeiculoPresenterMock.NaoDeveTerApresentadoSucesso<IAtualizarVeiculoPresenter, VeiculoAggregate>();
        }
    }
}
