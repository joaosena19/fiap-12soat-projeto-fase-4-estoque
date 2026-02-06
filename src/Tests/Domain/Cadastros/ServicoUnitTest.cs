using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class ServicoTests
    {
        #region Testes Método Criar e Atualizar

        [Fact(DisplayName = "Deve criar novo Serviço com dados válidos")]
        [Trait("Método", "Criar")]
        public void ServicoCriar_Deve_CriarServico_Quando_DadosValidos()
        {
            // Arrange
            var nome = "Troca de óleo";
            var preco = 150.00M;

            // Act
            var servico = Servico.Criar(nome, preco);

            // Assert
            servico.Should().NotBeNull();
            servico.Id.Should().NotBe(Guid.Empty);
            servico.Nome.Valor.Should().Be(nome);
            servico.Preco.Valor.Should().Be(preco);
        }

        [Fact(DisplayName = "Deve atualizar serviço com dados válidos")]
        [Trait("Método", "Atualizar")]
        public void ServicoAtualizar_Deve_AtualizarServico_Quando_DadosValidos()
        {
            // Arrange
            var nomeOriginal = "Troca de óleo";
            var precoOriginal = 100.00M;
            var novoNome = "Troca de óleo premium";
            var novoPreco = 200.00M;

            var servico = Servico.Criar(nomeOriginal, precoOriginal);

            // Act
            servico.Atualizar(novoNome, novoPreco);

            // Assert
            servico.Nome.Valor.Should().Be(novoNome);
            servico.Preco.Valor.Should().Be(novoPreco);
        }

        #endregion

        #region Testes ValueObject Nome

        [Theory(DisplayName = "Não deve criar novo Serviço se o Nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_500_caracteres__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ServicoCriar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var precoValido = 100.00M;

            // Act & Assert
            FluentActions.Invoking(() => Servico.Criar(nomeInvalido, precoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve atualizar serviço se o nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_500_caracteres__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ServicoAtualizar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var servico = Servico.Criar("Troca de óleo", 100.00M);
            var precoValido = 150.00M;

            // Act & Assert
            FluentActions.Invoking(() => servico.Atualizar(nomeInvalido, precoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        #endregion

        #region Testes ValueObject Preco

        [Theory(DisplayName = "Não deve criar novo Serviço se o Preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-100.00)]
        [InlineData(-1)]
        [Trait("ValueObject", "Preco")]
        public void ServicoCriar_Deve_ThrowException_Quando_PrecoInvalido(decimal precoInvalido)
        {
            // Arrange
            var nomeValido = "Troca de óleo";

            // Act & Assert
            FluentActions.Invoking(() => Servico.Criar(nomeValido, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Preço não pode ser negativo*");
        }

        [Theory(DisplayName = "Deve aceitar preços válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        [Trait("ValueObject", "Preco")]
        public void ServicoCriar_Deve_AceitarPrecos_Quando_PrecoValido(decimal precoValido)
        {
            // Arrange
            var nome = "Serviço de teste";

            // Act
            var servico = Servico.Criar(nome, precoValido);

            // Assert
            servico.Should().NotBeNull();
            servico.Preco.Valor.Should().Be(precoValido);
        }

        [Theory(DisplayName = "Não deve atualizar serviço se o preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-100.00)]
        [Trait("ValueObject", "Preco")]
        public void ServicoAtualizar_Deve_ThrowException_Quando_PrecoInvalido(decimal precoInvalido)
        {
            // Arrange
            var servico = Servico.Criar("Troca de óleo", 100.00M);
            var nomeValido = "Troca de óleo premium";

            // Act & Assert
            FluentActions.Invoking(() => servico.Atualizar(nomeValido, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Preço não pode ser negativo*");
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar serviço")]
        [Trait("Método", "Criar")]
        public void ServicoCriar_Deve_GerarUuidVersao7_Quando_CriarServico()
        {
            // Arrange
            var nome = "Troca de óleo";
            var preco = 150.00M;

            // Act
            var servico = Servico.Criar(nome, preco);

            // Assert
            servico.Id.Should().NotBe(Guid.Empty);
            var guidString = servico.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}
