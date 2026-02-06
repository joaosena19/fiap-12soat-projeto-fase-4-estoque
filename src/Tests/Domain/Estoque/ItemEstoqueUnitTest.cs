using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Estoque
{
    public class ItemEstoqueUnitTest
    {
        #region Testes Método Criar

        [Fact(DisplayName = "Deve criar item de estoque com dados válidos")]
        [Trait("Método", "Criar")]
        public void Criar_DeveCriarItemEstoqueComDadosValidos()
        {
            // Arrange
            var nome = "Filtro de Óleo";
            var quantidade = 50;
            var tipoItemEstoque = TipoItemEstoqueEnum.Peca;
            var preco = 25.50m;

            // Act
            var itemEstoque = ItemEstoque.Criar(nome, quantidade, tipoItemEstoque, preco);

            // Assert
            itemEstoque.Should().NotBeNull();
            itemEstoque.Id.Should().NotBeEmpty();
            itemEstoque.Nome.Valor.Should().Be(nome);
            itemEstoque.Quantidade.Valor.Should().Be(quantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoItemEstoque);
            itemEstoque.Preco.Valor.Should().Be(preco);
        }

        #endregion

        #region Testes Método Atualizar

        [Fact(DisplayName = "Deve atualizar item de estoque com dados válidos")]
        [Trait("Método", "Atualizar")]
        public void Atualizar_DeveAtualizarItemEstoqueComDadosValidos()
        {
            // Arrange
            var nomeOriginal = "Filtro de Óleo";
            var quantidadeOriginal = 50;
            var tipoOriginal = TipoItemEstoqueEnum.Peca;
            var precoOriginal = 25.50m;
            
            var novoNome = "Filtro de Óleo Premium";
            var novaQuantidade = 75;
            var novoTipo = TipoItemEstoqueEnum.Insumo;
            var novoPreco = 35.75m;

            var itemEstoque = ItemEstoque.Criar(nomeOriginal, quantidadeOriginal, tipoOriginal, precoOriginal);

            // Act
            itemEstoque.Atualizar(novoNome, novaQuantidade, novoTipo, novoPreco);

            // Assert
            itemEstoque.Nome.Valor.Should().Be(novoNome);
            itemEstoque.Quantidade.Valor.Should().Be(novaQuantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(novoTipo);
            itemEstoque.Preco.Valor.Should().Be(novoPreco);
        }

        #endregion

        #region Testes Método AtualizarQuantidade

        [Fact(DisplayName = "Deve atualizar apenas a quantidade do item de estoque")]
        [Trait("Método", "AtualizarQuantidade")]
        public void AtualizarQuantidade_DeveAtualizarApenasQuantidade()
        {
            // Arrange
            var nome = "Filtro de Óleo";
            var quantidadeOriginal = 50;
            var novaQuantidade = 100;
            var tipoItemEstoque = TipoItemEstoqueEnum.Peca;
            var preco = 25.50m;

            var itemEstoque = ItemEstoque.Criar(nome, quantidadeOriginal, tipoItemEstoque, preco);

            // Act
            itemEstoque.AtualizarQuantidade(novaQuantidade);

            // Assert
            itemEstoque.Nome.Valor.Should().Be(nome); // Nome não deve mudar
            itemEstoque.Quantidade.Valor.Should().Be(novaQuantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoItemEstoque); // Tipo não deve mudar
            itemEstoque.Preco.Valor.Should().Be(preco); // Preço não deve mudar
        }

        #endregion

        #region Testes Método VerificarDisponibilidade

        [Theory(DisplayName = "Deve verificar disponibilidade corretamente")]
        [InlineData(50, 30, true)]   // Estoque 50, solicita 30 - disponível
        [InlineData(50, 50, true)]   // Estoque 50, solicita 50 - disponível
        [InlineData(50, 51, false)]  // Estoque 50, solicita 51 - não disponível
        [InlineData(10, 15, false)]  // Estoque 10, solicita 15 - não disponível
        [Trait("Método", "VerificarDisponibilidade")]
        public void VerificarDisponibilidade_DeveRetornarStatusCorreto(int quantidadeEstoque, int quantidadeSolicitada, bool esperado)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", quantidadeEstoque, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act
            var disponivel = itemEstoque.VerificarDisponibilidade(quantidadeSolicitada);

            // Assert
            disponivel.Should().Be(esperado);
        }

        [Theory(DisplayName = "Não deve verificar disponibilidade com quantidade inválida")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("Método", "VerificarDisponibilidade")]
        public void VerificarDisponibilidade_DeveLancarExcecao_QuandoQuantidadeInvalida(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.VerificarDisponibilidade(quantidadeInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade requisitada deve ser maior que 0");
        }

        #endregion

        #region Testes ValueObject Nome

        [Theory(DisplayName = "Não deve criar item de estoque se o nome for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void Criar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeInvalido, quantidadeValida, tipoValido, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        [Fact(DisplayName = "Não deve criar item de estoque se o nome for nulo")]
        [Trait("ValueObject", "Nome")]
        public void Criar_ComNomeNulo_DeveLancarExcecao()
        {
            // Arrange
            string nomeNulo = null!;
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeNulo, quantidadeValida, tipoValido, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se o nome for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void Atualizar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar(nomeInvalido, 50, TipoItemEstoqueEnum.Peca, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        #endregion

        #region Testes ValueObject Quantidade

        [Theory(DisplayName = "Não deve criar item de estoque se a quantidade for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        [Trait("ValueObject", "Quantidade")]
        public void Criar_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeValido, quantidadeInvalida, tipoValido, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        [Theory(DisplayName = "Deve aceitar quantidades válidas")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        [Trait("ValueObject", "Quantidade")]
        public void Criar_ComQuantidadeValida_DeveAceitarQuantidade(int quantidadeValida)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act
            var itemEstoque = ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido, 25.50m);

            // Assert
            itemEstoque.Quantidade.Valor.Should().Be(quantidadeValida);
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se a quantidade for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("ValueObject", "Quantidade")]
        public void Atualizar_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar("Filtro de Óleo", quantidadeInvalida, TipoItemEstoqueEnum.Peca, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        [Theory(DisplayName = "Não deve atualizar quantidade se for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("ValueObject", "Quantidade")]
        public void AtualizarQuantidade_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.AtualizarQuantidade(quantidadeInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        #endregion

        #region Testes ValueObject TipoItemEstoque

        [Theory(DisplayName = "Não deve criar item de estoque se o tipo for inválido")]
        [InlineData((TipoItemEstoqueEnum)0)]
        [InlineData((TipoItemEstoqueEnum)3)]
        [InlineData((TipoItemEstoqueEnum)999)]
        [InlineData((TipoItemEstoqueEnum)(-1))]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Criar_ComTipoItemEstoqueInvalido_DeveLancarExcecao(TipoItemEstoqueEnum tipoInvalido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeValido, quantidadeValida, tipoInvalido, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("*Tipo de item de estoque*não é válido*");
        }

        [Theory(DisplayName = "Deve aceitar tipos de item de estoque válidos")]
        [InlineData(TipoItemEstoqueEnum.Peca)]
        [InlineData(TipoItemEstoqueEnum.Insumo)]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Criar_ComTipoItemEstoqueValido_DeveAceitarTipo(TipoItemEstoqueEnum tipoValido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;

            // Act
            var itemEstoque = ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido, 25.50m);

            // Assert
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoValido);
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se o tipo for inválido")]
        [InlineData((TipoItemEstoqueEnum)0)]
        [InlineData((TipoItemEstoqueEnum)3)]
        [InlineData((TipoItemEstoqueEnum)999)]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Atualizar_ComTipoItemEstoqueInvalido_DeveLancarExcecao(TipoItemEstoqueEnum tipoInvalido)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar("Filtro de Óleo", 50, tipoInvalido, 25.50m))
                .Should().Throw<DomainException>()
                .WithMessage("*Tipo de item de estoque*não é válido*");
        }

        #endregion

        #region Testes ValueObject PrecoItem

        [Theory(DisplayName = "Não deve criar item de estoque se o preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoItem")]
        public void Criar_ComPrecoInvalido_DeveLancarExcecao(decimal precoInvalido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço não pode ser negativo");
        }

        [Theory(DisplayName = "Deve aceitar preços válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(25.50)]
        [InlineData(1000)]
        [Trait("ValueObject", "PrecoItem")]
        public void Criar_ComPrecoValido_DeveAceitarPreco(decimal precoValido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act
            var itemEstoque = ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido, precoValido);

            // Assert
            itemEstoque.Preco.Valor.Should().Be(precoValido);
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se o preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoItem")]
        public void Atualizar_ComPrecoInvalido_DeveLancarExcecao(decimal precoInvalido)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço não pode ser negativo");
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar item de estoque")]
        [Trait("Método", "Criar")]
        public void ItemEstoqueCriar_Deve_GerarUuidVersao7_Quando_CriarItemEstoque()
        {
            // Arrange
            var nome = "Filtro de Óleo";
            var quantidade = 50;
            var tipo = TipoItemEstoqueEnum.Peca;
            var preco = 25.50m;

            // Act
            var itemEstoque = ItemEstoque.Criar(nome, quantidade, tipo, preco);

            // Assert
            itemEstoque.Id.Should().NotBe(Guid.Empty);
            var guidString = itemEstoque.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}
