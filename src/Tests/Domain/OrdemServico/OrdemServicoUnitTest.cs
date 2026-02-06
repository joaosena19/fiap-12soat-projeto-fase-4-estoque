using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.OrdemServico;
using FluentAssertions;
using Shared.Exceptions;
using System.Reflection;

namespace Tests.Domain.OrdemServico
{
    public class OrdemServicoUnitTest
    {
        #region Testes ValueObject Codigo

        [Theory(DisplayName = "Não deve criar código se o formato for inválido")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("OS-123")]
        [InlineData("OS-20240101")]
        [InlineData("OS-20240101-ABC")]
        [InlineData("OS-2024-123456")]
        [InlineData("ABC-20240101-123456")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComFormatoInvalido_DeveLancarExcecao(string? codigoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Codigo(codigoInvalido!))
                .Should().Throw<DomainException>()
                .WithMessage("*Código*inválido. Formato esperado: OS-YYYYMMDD-ABC123");
        }

        [Fact(DisplayName = "Não deve criar código se for nulo")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComValorNulo_DeveLancarExcecao()
        {
            // Arrange
            string codigoNulo = null!;

            // Act & Assert
            FluentActions.Invoking(() => new Codigo(codigoNulo))
                .Should().Throw<DomainException>()
                .WithMessage("*Código*inválido. Formato esperado: OS-YYYYMMDD-ABC123");
        }

        [Theory(DisplayName = "Deve aceitar códigos com formato válido")]
        [InlineData("OS-20240101-ABC123")]
        [InlineData("OS-20241231-XYZ789")]
        [InlineData("OS-20240701-A1B2C3")]
        [InlineData("os-20240101-abc123")]
        [InlineData(" os-20240101-abc123 ")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComFormatoValido_DeveAceitarCodigo(string codigoValido)
        {
            // Act
            var codigo = new Codigo(codigoValido);

            // Assert
            codigo.Valor.Should().Be(codigoValido.Trim().ToUpper());
        }

        [Fact(DisplayName = "Deve gerar novo código com formato correto")]
        [Trait("ValueObject", "Codigo")]
        public void GerarNovo_DeveGerarCodigoComFormatoCorreto()
        {
            // Act
            var codigo = Codigo.GerarNovo();

            // Assert
            codigo.Valor.Should().StartWith("OS-");
            codigo.Valor.Should().MatchRegex(@"^OS-\d{8}-[A-Z0-9]{6}$");
        }

        #endregion

        #region Testes Metodo AlterarStatus

        [Fact(DisplayName = "AlterarStatus deve chamar Cancelar quando status for Cancelada")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComCancelada_DeveChamarCancelar()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.Cancelada);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);
        }

        [Fact(DisplayName = "AlterarStatus deve chamar IniciarDiagnostico quando status for EmDiagnostico")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComEmDiagnostico_DeveChamarIniciarDiagnostico()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.EmDiagnostico);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmDiagnostico);
        }

        [Fact(DisplayName = "AlterarStatus deve chamar GerarOrcamento quando status for AguardandoAprovacao")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComAguardandoAprovacao_DeveChamarGerarOrcamento()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10m);

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.AguardandoAprovacao);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.AguardandoAprovacao);
            ordemServico.Orcamento.Should().NotBeNull();
        }

        [Fact(DisplayName = "AlterarStatus deve chamar IniciarExecucao quando status for EmExecucao")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComEmExecucao_DeveChamarIniciarExecucao()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10m);
            ordemServico.GerarOrcamento();

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.EmExecucao);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);
            ordemServico.Historico.DataInicioExecucao.Should().NotBeNull();
        }

        [Fact(DisplayName = "AlterarStatus deve chamar FinalizarExecucao quando status for Finalizada")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComFinalizada_DeveChamarFinalizarExecucao()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10m);
            ordemServico.GerarOrcamento();
            ordemServico.IniciarExecucao();

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.Finalizada);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Finalizada);
            ordemServico.Historico.DataFinalizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "AlterarStatus deve chamar Entregar quando status for Entregue")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComEntregue_DeveChamarEntregar()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10m);
            ordemServico.GerarOrcamento();
            ordemServico.IniciarExecucao();
            ordemServico.FinalizarExecucao();

            ordemServico.AlterarStatus(StatusOrdemServicoEnum.Entregue);

            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Entregue);
            ordemServico.Historico.DataEntrega.Should().NotBeNull();
        }

        [Fact(DisplayName = "AlterarStatus não deve permitir alterar para Recebida")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComRecebida_DeveLancarExcecao()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            FluentActions.Invoking(() => ordemServico.AlterarStatus(StatusOrdemServicoEnum.Recebida))
                .Should().Throw<DomainException>()
                .WithMessage("*Não é possível alterar o status para 'Recebida'.*");
        }

        [Fact(DisplayName = "AlterarStatus com status inválido deve lançar exceção")]
        [Trait("Método", "AlterarStatus")]
        public void AlterarStatus_ComStatusInvalido_DeveLancarExcecao()
        {
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var statusInvalido = (StatusOrdemServicoEnum)999;

            FluentActions.Invoking(() => ordemServico.AlterarStatus(statusInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Status inválido.*");
        }

        #endregion

        #region Testes ValueObject HistoricoTemporal

        [Fact(DisplayName = "Não deve criar histórico temporal se data de criação for vazia")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataCriacaoVazia_DeveLancarExcecao()
        {
            // Arrange
            var dataInvalida = default(DateTime);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("A data de criação é obrigatória.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de início de execução for anterior à data de criação")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataInicioExecucaoAnteriorACriacao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddDays(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao))
                .Should().Throw<DomainException>()
                .WithMessage("A data de início de execução não pode ser anterior à data de criação.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de finalização for anterior à data de início de execução")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataFinalizacaoAnteriorAInicioExecucao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao))
                .Should().Throw<DomainException>()
                .WithMessage("A data de finalização não pode ser anterior à data de início de execução.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de entrega for anterior à data de finalização")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataEntregaAnteriorAFinalizacao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(1);
            var dataEntrega = dataFinalizacao.AddHours(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao, dataEntrega))
                .Should().Throw<DomainException>()
                .WithMessage("A data de entrega não pode ser anterior à data de finalização.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de finalização for informada sem data de início de execução")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataFinalizacaoSemDataInicio_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            DateTime? dataInicioExecucao = null;
            var dataFinalizacao = dataCriacao.AddHours(2);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao))
                .Should().Throw<DomainException>()
                .WithMessage("A data de finalização não pode ser anterior à data de início de execução.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de entrega for informada sem data de finalização")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataEntregaSemDataFinalizacao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            DateTime? dataFinalizacao = null;
            var dataEntrega = dataInicioExecucao.AddHours(2);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao, dataEntrega))
                .Should().Throw<DomainException>()
                .WithMessage("A data de entrega não pode ser anterior à data de finalização.");
        }

        [Fact(DisplayName = "Deve aceitar histórico temporal com apenas data de criação")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComApenasDataCriacao_DeveAceitarHistorico()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;

            // Act
            var historico = new HistoricoTemporal(dataCriacao);

            // Assert
            historico.DataCriacao.Should().Be(dataCriacao);
            historico.DataInicioExecucao.Should().BeNull();
            historico.DataFinalizacao.Should().BeNull();
            historico.DataEntrega.Should().BeNull();
        }

        [Fact(DisplayName = "Deve aceitar histórico temporal com datas válidas")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDatasValidas_DeveAceitarHistorico()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);
            var dataEntrega = dataFinalizacao.AddHours(1);

            // Act
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao, dataEntrega);

            // Assert
            historico.DataCriacao.Should().Be(dataCriacao);
            historico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            historico.DataFinalizacao.Should().Be(dataFinalizacao);
            historico.DataEntrega.Should().Be(dataEntrega);
        }

        [Fact(DisplayName = "Deve marcar data de início de execução")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataInicioExecucao_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var historico = new HistoricoTemporal(dataCriacao);
            var dataInicioExecucao = dataCriacao.AddHours(1);

            // Act
            var novoHistorico = historico.MarcarDataInicioExecucao(dataInicioExecucao);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
        }

        [Fact(DisplayName = "Deve marcar data de finalização")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataFinalizadaExecucao_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);

            // Act
            var novoHistorico = historico.MarcarDataFinalizadaExecucao(dataFinalizacao);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            novoHistorico.DataFinalizacao.Should().Be(dataFinalizacao);
        }

        [Fact(DisplayName = "Deve marcar data de entrega")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataEntrega_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao);
            var dataEntrega = dataFinalizacao.AddHours(1);

            // Act
            var novoHistorico = historico.MarcarDataEntrega(dataEntrega);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            novoHistorico.DataFinalizacao.Should().Be(dataFinalizacao);
            novoHistorico.DataEntrega.Should().Be(dataEntrega);
        }

        #endregion

        #region Testes ValueObject Status

        [Theory(DisplayName = "Não deve criar status se enum for inválido")]
        [InlineData((StatusOrdemServicoEnum)(-1))]
        [InlineData((StatusOrdemServicoEnum)7)]
        [InlineData((StatusOrdemServicoEnum)999)]
        [Trait("ValueObject", "Status")]
        public void Status_ComEnumInvalido_DeveLancarExcecao(StatusOrdemServicoEnum statusInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Status(statusInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Status da Ordem de Serviço*não é válido*");
        }

        [Fact(DisplayName = "Deve aceitar todos os status válidos")]
        [Trait("ValueObject", "Status")]
        public void Status_ComTodosEnumsValidos_DeveAceitarStatus()
        {
            // Arrange & Act & Assert
            foreach (StatusOrdemServicoEnum statusValido in Enum.GetValues<StatusOrdemServicoEnum>())
            {
                var status = new Status(statusValido);
                status.Valor.Should().Be(statusValido);
            }
        }

        #endregion

        #region Testes Metodo Criar

        [Fact(DisplayName = "Deve criar ordem de serviço com sucesso")]
        [Trait("Método", "Criar")]
        public void Criar_ComVeiculoId_DeveCriarOrdemServico()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            // Act
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            // Assert
            ordemServico.Id.Should().NotBeEmpty();
            ordemServico.VeiculoId.Should().Be(veiculoId);
            ordemServico.Codigo.Valor.Should().StartWith("OS-");
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Recebida);
            ordemServico.Historico.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            ordemServico.ServicosIncluidos.Should().BeEmpty();
            ordemServico.ItensIncluidos.Should().BeEmpty();
            ordemServico.Orcamento.Should().BeNull();
        }

        #endregion

        #region Testes Metodo PermiteAlterarServicosItens

        [Theory(DisplayName = "Deve permitir alterar serviços/itens nos status permitidos")]
        [InlineData(StatusOrdemServicoEnum.Recebida)]
        [InlineData(StatusOrdemServicoEnum.EmDiagnostico)]
        [Trait("Método", "PermiteAlterarServicosItens")]
        public void PermiteAlterarServicosItens_ComStatusPermitido_DeveRetornarTrue(StatusOrdemServicoEnum statusPermitido)
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            if (statusPermitido == StatusOrdemServicoEnum.EmDiagnostico)
            {
                ordemServico.IniciarDiagnostico();
            }

            // Act
            var resultado = ordemServico.PermiteAlterarServicosItens();

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact(DisplayName = "Não deve permitir alterar serviços/itens nos status não permitidos")]
        [Trait("Método", "PermiteAlterarServicosItens")]
        public void PermiteAlterarServicosItens_ComStatusNaoPermitido_DeveRetornarFalse()
        {
            // Arrange
            var statusNaoPermitidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusNaoPermitido in statusNaoPermitidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Usar helper method para definir o status diretamente
                DefinirStatusPorReflection(ordemServico, statusNaoPermitido);

                // Act
                var resultado = ordemServico.PermiteAlterarServicosItens();

                // Assert
                resultado.Should().BeFalse($"Status {statusNaoPermitido} não deveria permitir alterar serviços/itens");
            }
        }

        #endregion

        #region Testes Metodo AdicionarServico

        [Fact(DisplayName = "Deve adicionar serviço com sucesso")]
        [Trait("Método", "AdicionarServico")]
        public void AdicionarServico_ComParametrosValidos_DeveAdicionarServico()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var servicoId = Guid.NewGuid();
            var nome = "Troca de óleo";
            var preco = 75.50m;

            // Act
            ordemServico.AdicionarServico(servicoId, nome, preco);

            // Assert
            ordemServico.ServicosIncluidos.Should().HaveCount(1);
            var servicoAdicionado = ordemServico.ServicosIncluidos.First();
            servicoAdicionado.ServicoOriginalId.Should().Be(servicoId);
            servicoAdicionado.Nome.Valor.Should().Be(nome);
            servicoAdicionado.Preco.Valor.Should().Be(preco);
        }

        [Fact(DisplayName = "Não deve adicionar serviço se status não permitir")]
        [Trait("Método", "AdicionarServico")]
        public void AdicionarServico_ComStatusNaoPermitido_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10.00m))
                    .Should().Throw<DomainException>()
                    .WithMessage($"Não é possível adicionar serviços a uma Ordem de Serviço com o status '{statusInvalido}'.",
                        $"Status {statusInvalido} não deveria permitir adicionar serviços");
            }
        }

        [Fact(DisplayName = "Não deve adicionar serviço duplicado")]
        [Trait("Método", "AdicionarServico")]
        public void AdicionarServico_ComServicoJaIncluido_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var servicoId = Guid.NewGuid();
            ordemServico.AdicionarServico(servicoId, "Teste", 10.00m);

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.AdicionarServico(servicoId, "Teste 2", 20.00m))
                .Should().Throw<DomainException>()
                .WithMessage("Este serviço já foi incluído nesta OS.");
        }

        #endregion

        #region Testes Metodo AdicionarItem

        [Fact(DisplayName = "Deve adicionar item com sucesso")]
        [Trait("Método", "AdicionarItem")]
        public void AdicionarItem_ComParametrosValidos_DeveAdicionarItem()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var nome = "Filtro de óleo";
            var precoUnitario = 25.00m;
            var quantidade = 2;
            var tipo = TipoItemIncluidoEnum.Peca;

            // Act
            ordemServico.AdicionarItem(itemId, nome, precoUnitario, quantidade, tipo);

            // Assert
            ordemServico.ItensIncluidos.Should().HaveCount(1);
            var itemAdicionado = ordemServico.ItensIncluidos.First();
            itemAdicionado.ItemEstoqueOriginalId.Should().Be(itemId);
            itemAdicionado.Nome.Valor.Should().Be(nome);
            itemAdicionado.Preco.Valor.Should().Be(precoUnitario);
            itemAdicionado.Quantidade.Valor.Should().Be(quantidade);
            itemAdicionado.TipoItemIncluido.Valor.Should().Be(tipo);
        }

        [Fact(DisplayName = "Deve incrementar quantidade se item já existir")]
        [Trait("Método", "AdicionarItem")]
        public void AdicionarItem_ComItemJaExistente_DeveIncrementarQuantidade()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            ordemServico.AdicionarItem(itemId, "Filtro", 10.00m, 2, TipoItemIncluidoEnum.Peca);

            // Act
            ordemServico.AdicionarItem(itemId, "Filtro", 10.00m, 3, TipoItemIncluidoEnum.Peca);

            // Assert
            ordemServico.ItensIncluidos.Should().HaveCount(1);
            var item = ordemServico.ItensIncluidos.First();
            item.Quantidade.Valor.Should().Be(5); // 2 + 3
        }

        [Fact(DisplayName = "Não deve adicionar item se status não permitir")]
        [Trait("Método", "AdicionarItem")]
        public void AdicionarItem_ComStatusNaoPermitido_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.AdicionarItem(Guid.NewGuid(), "Teste", 10.00m, 1, TipoItemIncluidoEnum.Peca))
                    .Should().Throw<DomainException>()
                    .WithMessage($"Não é possível adicionar itens a uma Ordem de Serviço com o status '{statusInvalido}'.",
                        $"Status {statusInvalido} não deveria permitir adicionar itens");
            }
        }

        [Theory(DisplayName = "Não deve adicionar item com quantidade inválida")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("Método", "AdicionarItem")]
        public void AdicionarItem_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.AdicionarItem(Guid.NewGuid(), "Teste", 10.00m, quantidadeInvalida, TipoItemIncluidoEnum.Peca))
                .Should().Throw<DomainException>()
                .WithMessage("A quantidade deve ser maior que zero.");
        }

        #endregion

        #region Testes Metodo RemoverServico

        [Fact(DisplayName = "Deve remover serviço com sucesso")]
        [Trait("Método", "RemoverServico")]
        public void RemoverServico_ComServicoExistente_DeveRemoverServico()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10.00m);
            var servicoId = ordemServico.ServicosIncluidos.First().Id;

            // Act
            ordemServico.RemoverServico(servicoId);

            // Assert
            ordemServico.ServicosIncluidos.Should().BeEmpty();
        }

        [Fact(DisplayName = "Não deve remover serviço se status não permitir")]
        [Trait("Método", "RemoverServico")]
        public void RemoverServico_ComStatusNaoPermitido_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 10.00m);
                var servicoId = ordemServico.ServicosIncluidos.First().Id;
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.RemoverServico(servicoId))
                    .Should().Throw<DomainException>()
                    .WithMessage($"Não é possível remover serviços de uma Ordem de Serviço com o status '{statusInvalido}'.",
                        $"Status {statusInvalido} não deveria permitir remover serviços");
            }
        }

        [Fact(DisplayName = "Não deve remover serviço inexistente")]
        [Trait("Método", "RemoverServico")]
        public void RemoverServico_ComServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var servicoIdInexistente = Guid.NewGuid();

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.RemoverServico(servicoIdInexistente))
                .Should().Throw<DomainException>()
                .WithMessage("Serviço não encontrado nesta ordem de serviço.");
        }

        #endregion

        #region Testes Metodo RemoverItem

        [Fact(DisplayName = "Deve remover item com sucesso")]
        [Trait("Método", "RemoverItem")]
        public void RemoverItem_ComItemExistente_DeveRemoverItem()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.AdicionarItem(Guid.NewGuid(), "Teste", 10.00m, 1, TipoItemIncluidoEnum.Peca);
            var itemId = ordemServico.ItensIncluidos.First().Id;

            // Act
            ordemServico.RemoverItem(itemId);

            // Assert
            ordemServico.ItensIncluidos.Should().BeEmpty();
        }

        [Fact(DisplayName = "Não deve remover item se status não permitir")]
        [Trait("Método", "RemoverItem")]
        public void RemoverItem_ComStatusNaoPermitido_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                ordemServico.AdicionarItem(Guid.NewGuid(), "Teste", 10.00m, 1, TipoItemIncluidoEnum.Peca);
                var itemId = ordemServico.ItensIncluidos.First().Id;
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.RemoverItem(itemId))
                    .Should().Throw<DomainException>()
                    .WithMessage($"Não é possível remover itens de uma Ordem de Serviço com o status '{statusInvalido}'.",
                        $"Status {statusInvalido} não deveria permitir remover itens");
            }
        }

        [Fact(DisplayName = "Não deve remover item inexistente")]
        [Trait("Método", "RemoverItem")]
        public void RemoverItem_ComItemInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            var itemIdInexistente = Guid.NewGuid();

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.RemoverItem(itemIdInexistente))
                .Should().Throw<DomainException>()
                .WithMessage("Item não encontrado nesta ordem de serviço.");
        }

        #endregion

        #region Testes Metodo Cancelar

        [Fact(DisplayName = "Deve cancelar ordem de serviço com sucesso")]
        [Trait("Método", "Cancelar")]
        public void Cancelar_DeveAlterarStatusParaCancelada()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            // Act
            ordemServico.Cancelar();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);
        }

        [Fact(DisplayName = "Deve permitir cancelar ordem de serviço em qualquer status")]
        [Trait("Método", "Cancelar")]
        public void Cancelar_ComQualquerStatus_DeveAlterarStatusParaCancelada()
        {
            // Arrange & Act & Assert
            foreach (StatusOrdemServicoEnum statusAtual in Enum.GetValues<StatusOrdemServicoEnum>())
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection se não for o status inicial
                if (statusAtual != StatusOrdemServicoEnum.Recebida)
                {
                    DefinirStatusPorReflection(ordemServico, statusAtual);
                }

                // Act
                ordemServico.Cancelar();

                // Assert
                ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada, 
                    $"Deveria ser possível cancelar uma OS do status '{statusAtual}' para 'Cancelada'");
            }
        }

        #endregion

        #region Testes Metodo IniciarDiagnostico

        [Fact(DisplayName = "Deve iniciar diagnóstico com sucesso")]
        [Trait("Método", "IniciarDiagnostico")]
        public void IniciarDiagnostico_ComStatusRecebida_DeveAlterarStatusParaEmDiagnostico()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            // Act
            ordemServico.IniciarDiagnostico();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmDiagnostico);
        }

        [Fact(DisplayName = "Não deve iniciar diagnóstico se status não for Recebida")]
        [Trait("Método", "IniciarDiagnostico")]
        public void IniciarDiagnostico_ComTodosStatusDiferentesDeRecebida_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Recebida });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.IniciarDiagnostico())
                    .Should().Throw<DomainException>()
                    .WithMessage($"Só é possível iniciar diagnóstico para um ordem de serviço com o status 'Recebida'", 
                        $"Status {statusInvalido} não deveria permitir iniciar diagnóstico");
            }
        }

        #endregion

        #region Testes Metodo GerarOrcamento

        [Fact(DisplayName = "Deve gerar orçamento com sucesso")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ComServicosIncluidos_DeveGerarOrcamento()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);

            // Act
            ordemServico.GerarOrcamento();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.AguardandoAprovacao);
            ordemServico.Orcamento.Should().NotBeNull();
            ordemServico.Orcamento!.Preco.Valor.Should().Be(50.00m);
        }

        [Fact(DisplayName = "Não deve gerar orçamento se já existir")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ComOrcamentoJaExistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.GerarOrcamento())
                .Should().Throw<DomainException>()
                .WithMessage("Já existe um orçamento gerado para esta ordem de serviço.");
        }

        [Fact(DisplayName = "Não deve gerar orçamento se status não for EmDiagnostico")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ComStatusDiferenteDeEmDiagnostico_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.EmDiagnostico });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.GerarOrcamento())
                    .Should().Throw<DomainException>()
                    .WithMessage("Só é possível gerar orçamento para uma ordem de serviço com o status 'EmDiagnostico'",
                        $"Status {statusInvalido} não deveria permitir gerar orçamento");
            }
        }

        [Fact(DisplayName = "Não deve gerar orçamento sem serviços ou itens")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_SemServicosOuItens_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.GerarOrcamento())
                .Should().Throw<DomainException>()
                .WithMessage("Não é possível gerar orçamento sem pelo menos um serviço ou item incluído.");
        }

        #endregion

        #region Testes Metodo AprovarOrcamento

        [Fact(DisplayName = "Deve aprovar orçamento e iniciar execução")]
        [Trait("Método", "AprovarOrcamento")]
        public void AprovarOrcamento_ComOrcamentoExistente_DeveIniciarExecucao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();

            // Act
            ordemServico.AprovarOrcamento();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);
            ordemServico.Historico.DataInicioExecucao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve aprovar orçamento se não existir")]
        [Trait("Método", "AprovarOrcamento")]
        public void AprovarOrcamento_SemOrcamento_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.AprovarOrcamento())
                .Should().Throw<DomainException>()
                .WithMessage("Não existe orçamento para aprovar. É necessário gerar o orçamento primeiro.");
        }

        #endregion

        #region Testes Metodo DesaprovarOrcamento

        [Fact(DisplayName = "Deve desaprovar orçamento e cancelar ordem")]
        [Trait("Método", "DesaprovarOrcamento")]
        public void DesaprovarOrcamento_ComOrcamentoExistente_DeveCancelarOrdem()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();

            // Act
            ordemServico.DesaprovarOrcamento();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);
        }

        [Fact(DisplayName = "Não deve desaprovar orçamento se não existir")]
        [Trait("Método", "DesaprovarOrcamento")]
        public void DesaprovarOrcamento_SemOrcamento_DeveLancarExcecao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());

            // Act & Assert
            FluentActions.Invoking(() => ordemServico.DesaprovarOrcamento())
                .Should().Throw<DomainException>()
                .WithMessage("Não existe orçamento para desaprovar. É necessário gerar o orçamento primeiro.");
        }

        #endregion

        #region Testes Metodo IniciarExecucao

        [Fact(DisplayName = "Deve iniciar execução com sucesso")]
        [Trait("Método", "IniciarExecucao")]
        public void IniciarExecucao_ComStatusAguardandoAprovacao_DeveIniciarExecucao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();

            // Act
            ordemServico.IniciarExecucao();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);
            ordemServico.Historico.DataInicioExecucao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve iniciar execução se status não for AguardandoAprovacao")]
        [Trait("Método", "IniciarExecucao")]
        public void IniciarExecucao_ComStatusDiferenteDeAguardandoAprovacao_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.AguardandoAprovacao });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.IniciarExecucao())
                    .Should().Throw<DomainException>()
                    .WithMessage("Só é possível iniciar execução para uma ordem de serviço com o status 'AguardandoAprovacao'",
                        $"Status {statusInvalido} não deveria permitir iniciar execução");
            }
        }

        #endregion

        #region Testes Metodo FinalizarExecucao

        [Fact(DisplayName = "Deve finalizar execução com sucesso")]
        [Trait("Método", "FinalizarExecucao")]
        public void FinalizarExecucao_ComStatusEmExecucao_DeveFinalizarExecucao()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();

            // Act
            ordemServico.FinalizarExecucao();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Finalizada);
            ordemServico.Historico.DataFinalizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve finalizar execução se status não for EmExecucao")]
        [Trait("Método", "FinalizarExecucao")]
        public void FinalizarExecucao_ComStatusDiferenteDeEmExecucao_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.EmExecucao });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.FinalizarExecucao())
                    .Should().Throw<DomainException>()
                    .WithMessage("Só é possível finalizar execução para uma ordem de serviço com o status 'EmExecucao'",
                        $"Status {statusInvalido} não deveria permitir finalizar execução");
            }
        }

        #endregion

        #region Testes Metodo Entregar

        [Fact(DisplayName = "Deve entregar ordem de serviço com sucesso")]
        [Trait("Método", "Entregar")]
        public void Entregar_ComStatusFinalizada_DeveEntregarOrdemServico()
        {
            // Arrange
            var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Teste", 50.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();
            ordemServico.FinalizarExecucao();

            // Act
            ordemServico.Entregar();

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Entregue);
            ordemServico.Historico.DataEntrega.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve entregar se status não for Finalizada")]
        [Trait("Método", "Entregar")]
        public void Entregar_ComStatusDiferenteDeFinalizada_DeveLancarExcecao()
        {
            // Arrange
            var statusInvalidos = Enum.GetValues<StatusOrdemServicoEnum>()
                .Except(new[] { StatusOrdemServicoEnum.Finalizada });

            foreach (var statusInvalido in statusInvalidos)
            {
                var ordemServico = OrdemServicoAggregate.Criar(Guid.NewGuid());
                
                // Definir status usando reflection
                DefinirStatusPorReflection(ordemServico, statusInvalido);

                // Act & Assert
                FluentActions.Invoking(() => ordemServico.Entregar())
                    .Should().Throw<DomainException>()
                    .WithMessage("Só é possível entregar uma ordem de serviço com o status 'Finalizada'",
                        $"Status {statusInvalido} não deveria permitir entrega");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Define o status de uma ordem de serviço usando reflection
        /// </summary>
        /// <param name="ordemServico">A ordem de serviço</param>
        /// <param name="status">O status a ser definido</param>
        private static void DefinirStatusPorReflection(OrdemServicoAggregate ordemServico, StatusOrdemServicoEnum status)
        {
            var statusField = typeof(OrdemServicoAggregate).GetField("_status", BindingFlags.NonPublic | BindingFlags.Instance);
            if (statusField == null)
            {
                // Tentar buscar por property backing field
                statusField = typeof(OrdemServicoAggregate).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(f => f.Name.Contains("Status") || f.Name.Contains("status"));
            }

            if (statusField != null)
            {
                statusField.SetValue(ordemServico, new Status(status));
            }
            else
            {
                // Fallback: usar a property Status se o field não for encontrado
                var statusProperty = typeof(OrdemServicoAggregate).GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                var setMethod = statusProperty?.GetSetMethod(true);
                setMethod?.Invoke(ordemServico, new object[] { new Status(status) });
            }
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar ordem de serviço")]
        [Trait("Método", "Criar")]
        public void OrdemServicoCriar_Deve_GerarUuidVersao7_Quando_CriarOrdemServico()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            // Act
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            // Assert
            ordemServico.Id.Should().NotBe(Guid.Empty);
            var guidString = ordemServico.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}
