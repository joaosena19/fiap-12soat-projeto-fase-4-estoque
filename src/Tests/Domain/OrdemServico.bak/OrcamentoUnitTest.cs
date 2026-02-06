using Domain.OrdemServico.Aggregates.OrdemServico;
using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.Orcamento;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.OrdemServico
{
    public class OrcamentoUnitTest
    {
        #region Testes ValueObject DataCriacao

        [Fact(DisplayName = "Não deve criar data de criação se for vazia")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComValorVazio_DeveLancarExcecao()
        {
            // Arrange
            var dataInvalida = default(DateTime);

            // Act & Assert
            FluentActions.Invoking(() => new DataCriacao(dataInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Data de criação não pode ser vazia");
        }

        [Theory(DisplayName = "Deve aceitar datas de criação válidas")]
        [InlineData("2024-01-01")]
        [InlineData("2024-07-15")]
        [InlineData("2024-12-31")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComValorValido_DeveAceitarData(string dataString)
        {
            // Arrange
            var dataValida = DateTime.Parse(dataString);

            // Act
            var dataCriacao = new DataCriacao(dataValida);

            // Assert
            dataCriacao.Valor.Should().Be(dataValida);
        }

        [Fact(DisplayName = "Deve aceitar data de criação atual")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComDataAtual_DeveAceitarData()
        {
            // Arrange
            var dataAtual = DateTime.UtcNow;

            // Act
            var dataCriacao = new DataCriacao(dataAtual);

            // Assert
            dataCriacao.Valor.Should().Be(dataAtual);
        }

        #endregion

        #region Testes ValueObject PrecoOrcamento

        [Theory(DisplayName = "Não deve criar preço do orçamento se for negativo")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoOrcamento")]
        public void PrecoOrcamento_ComValorNegativo_DeveLancarExcecao(decimal precoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new PrecoOrcamento(precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço do orçamento não pode ser negativo");
        }

        [Theory(DisplayName = "Deve aceitar preços de orçamento válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(150.75)]
        [InlineData(5000)]
        [InlineData(999999.99)]
        [Trait("ValueObject", "PrecoOrcamento")]
        public void PrecoOrcamento_ComValorValido_DeveAceitarPreco(decimal precoValido)
        {
            // Act
            var preco = new PrecoOrcamento(precoValido);

            // Assert
            preco.Valor.Should().Be(precoValido);
        }

        #endregion

        #region Testes Metodo GerarOrcamento

        [Fact(DisplayName = "Deve gerar orçamento com sucesso")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ComServicosEItens_DeveGerarOrcamento()
        {
            // Arrange
            var servicos = new List<ServicoIncluido>
            {
                ServicoIncluido.Criar(Guid.NewGuid(), "Troca de óleo", 50.00m),
                ServicoIncluido.Criar(Guid.NewGuid(), "Revisão freios", 100.00m)
            };
            
            var itens = new List<ItemIncluido>
            {
                ItemIncluido.Criar(Guid.NewGuid(), "Filtro de óleo", 25.00m, 1, TipoItemIncluidoEnum.Peca),
                ItemIncluido.Criar(Guid.NewGuid(), "Óleo do motor", 30.00m, 2, TipoItemIncluidoEnum.Insumo)
            };

            var precoEsperado = 50.00m + 100.00m + (25.00m * 1) + (30.00m * 2); // Serviços + Item 1 (25*1) + Item 2 (30*2)

            // Act
            var orcamento = Orcamento.GerarOrcamento(servicos, itens);

            // Assert
            orcamento.Id.Should().NotBeEmpty();
            orcamento.DataCriacao.Valor.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            orcamento.Preco.Valor.Should().Be(precoEsperado);
        }

        [Fact(DisplayName = "Deve gerar orçamento apenas com serviços")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ApenasComServicos_DeveGerarOrcamento()
        {
            // Arrange
            var servicos = new List<ServicoIncluido>
            {
                ServicoIncluido.Criar(Guid.NewGuid(), "Diagnóstico", 80.00m)
            };
            var itens = new List<ItemIncluido>();

            // Act
            var orcamento = Orcamento.GerarOrcamento(servicos, itens);

            // Assert
            orcamento.Id.Should().NotBeEmpty();
            orcamento.Preco.Valor.Should().Be(80.00m);
        }

        [Fact(DisplayName = "Deve gerar orçamento apenas com itens")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_ApenasComItens_DeveGerarOrcamento()
        {
            // Arrange
            var servicos = new List<ServicoIncluido>();
            var itens = new List<ItemIncluido>
            {
                ItemIncluido.Criar(Guid.NewGuid(), "Pneu", 200.00m, 4, TipoItemIncluidoEnum.Peca)
            };

            // Act
            var orcamento = Orcamento.GerarOrcamento(servicos, itens);

            // Assert
            orcamento.Id.Should().NotBeEmpty();
            orcamento.Preco.Valor.Should().Be(800.00m); // 200 * 4
        }

        [Fact(DisplayName = "Deve gerar orçamento com valor zero se não houver serviços nem itens")]
        [Trait("Método", "GerarOrcamento")]
        public void GerarOrcamento_SemServicosNemItens_DeveGerarOrcamentoComValorZero()
        {
            // Arrange
            var servicos = new List<ServicoIncluido>();
            var itens = new List<ItemIncluido>();

            // Act
            var orcamento = Orcamento.GerarOrcamento(servicos, itens);

            // Assert
            orcamento.Id.Should().NotBeEmpty();
            orcamento.Preco.Valor.Should().Be(0.00m);
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar orçamento")]
        [Trait("Método", "GerarOrcamento")]
        public void OrcamentoGerar_Deve_GerarUuidVersao7_Quando_GerarOrcamento()
        {
            // Arrange
            var servicos = new List<ServicoIncluido>
            {
                ServicoIncluido.Criar(Guid.NewGuid(), "Troca de óleo", 50.00m)
            };
            var itens = new List<ItemIncluido>
            {
                ItemIncluido.Criar(Guid.NewGuid(), "Filtro", 25.00m, 1, TipoItemIncluidoEnum.Peca)
            };

            // Act
            var orcamento = Orcamento.GerarOrcamento(servicos, itens);

            // Assert
            orcamento.Id.Should().NotBe(Guid.Empty);
            var guidString = orcamento.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}
