using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Cadastros.ValueObjects.Veiculo;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class VeiculoUnitTest
    {
        #region Testes Método Criar e Atualizar

        [Fact(DisplayName = "Deve criar veículo com dados válidos")]
        [Trait("Dados Válidos", "Criar")]
        public void Criar_DeveCriarVeiculoComDadosValidos()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placa = "ABC1234";
            var modelo = "Civic";
            var marca = "Honda";
            var cor = "Preto";
            var ano = 2020;
            var tipoVeiculo = TipoVeiculoEnum.Carro;

            // Act
            var veiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);

            // Assert
            veiculo.Should().NotBeNull();
            veiculo.Id.Should().NotBeEmpty();
            veiculo.ClienteId.Should().Be(clienteId);
            veiculo.Placa.Valor.Should().Be(placa);
            veiculo.Modelo.Valor.Should().Be(modelo);
            veiculo.Marca.Valor.Should().Be(marca);
            veiculo.Cor.Valor.Should().Be(cor);
            veiculo.Ano.Valor.Should().Be(ano);
            veiculo.TipoVeiculo.Valor.Should().Be(tipoVeiculo);
        }

        [Fact(DisplayName = "Deve atualizar veículo com dados válidos")]
        [Trait("Dados Válidos", "Atualizar")]
        public void Atualizar_DeveAtualizarVeiculoComDadosValidos()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            var novoModelo = "Corolla";
            var novaMarca = "Toyota";
            var novaCor = "Branco";
            var novoAno = 2021;
            var novoTipo = TipoVeiculoEnum.Carro;

            // Act
            veiculo.Atualizar(novoModelo, novaMarca, novaCor, novoAno, novoTipo);

            // Assert
            veiculo.ClienteId.Should().Be(clienteId);
            veiculo.Modelo.Valor.Should().Be(novoModelo);
            veiculo.Marca.Valor.Should().Be(novaMarca);
            veiculo.Cor.Valor.Should().Be(novaCor);
            veiculo.Ano.Valor.Should().Be(novoAno);
            veiculo.TipoVeiculo.Valor.Should().Be(novoTipo);
        }

        #endregion

        #region Testes ValueObject Placa

        [Theory(DisplayName = "Deve aceitar placas válidas")]
        [InlineData("ABC1234")] //Padrão antigo
        [InlineData("XYZ5678")]
        [InlineData("ABC1C34")] // Padrão mercosul
        [InlineData("XYZ5G78")]
        [InlineData("ABC-1234")] // Com hífen - deve ser removido
        [InlineData("abc1234")] // Lowercase - deve ser convertido para uppercase
        [InlineData(" DEF9876 ")] // Com espaços - devem ser removidos
        [Trait("ValueObject", "Placa")]
        public void Placa_ComPlacasValidas_DeveAceitarPlaca(string placaValida)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), placaValida, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            // Assert
            var placaEsperada = placaValida.Replace("-", "").ToUpper().Trim();
            veiculo.Placa.Valor.Should().Be(placaEsperada);
        }

        [Theory(DisplayName = "Não deve criar veículo se a placa for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC123")] // 6 caracteres
        [InlineData("ABC12345")] // 8 caracteres
        [InlineData("ABC-123")] // Com hífen mas não 7 caracteres após remoção
        [InlineData("ABC@123")] // Com caracteres especiais
        [InlineData("ABC 123")] // Com espaço no meio
        [Trait("ValueObject", "Placa")]
        public void Criar_ComPlacaInvalida_DeveLancarExcecao(string placaInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), placaInvalida, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Placa*");
        }

        #endregion

        #region Testes ValueObject Modelo

        [Theory(DisplayName = "Deve aceitar modelos válidos")]
        [InlineData("Civic")]
        [InlineData("Corolla")]
        [InlineData("A")] // Mínimo de 1 caractere
        [InlineData(" Modelo com espaços ")] // Com espaços - devem ser removidos
        [InlineData("ModeloComNomeMuitoLongoMasAindaDentroDoLimiteDe200CaracteresQueEhOLimiteMaximoParaOCampoModeloSegundoARegaDeNegocioDefinidaNaClasseValueObjectModeloQueValidaEsseCampoComUmaValidacaoMuitoEspecifica")] // 200 caracteres
        [Trait("ValueObject", "Modelo")]
        public void Modelo_ComModelosValidos_DeveAceitarModelo(string modeloValido)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", modeloValido, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            // Assert
            veiculo.Modelo.Valor.Should().Be(modeloValido.Trim());
        }

        [Theory(DisplayName = "Não deve criar veículo se o modelo for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Modelo")]
        public void Criar_ComModeloInvalido_DeveLancarExcecao(string modeloInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", modeloInvalido, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Modelo não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se o modelo for nulo")]
        [Trait("ValueObject", "Modelo")]
        public void Criar_ComModeloNulo_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", null!, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Modelo não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se o modelo for muito longo")]
        [Trait("ValueObject", "Modelo")]
        public void Criar_ComModeloMuitoLongo_DeveLancarExcecao()
        {
            // Arrange
            var modeloLongo = new string('A', 201); // 201 caracteres

            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", modeloLongo, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Modelo não pode ter mais de 200 caracteres*");
        }

        #endregion

        #region Testes ValueObject Marca

        [Theory(DisplayName = "Deve aceitar marcas válidas")]
        [InlineData("Honda")]
        [InlineData("Toyota")]
        [InlineData("A")] // Mínimo de 1 caractere
        [InlineData(" Marca com espaços ")] // Com espaços - devem ser removidos
        [InlineData("MarcaComNomeMuitoLongoMasAindaDentroDoLimiteDe200CaracteresQueEhOLimiteMaximoParaOCampoMarcaSegundoARegaDeNegocioDefinidaNaClasseValueObjectMarcaQueValidaEsseCampoComUmaValidacaoMuitoEspecifica")] // 200 caracteres
        [Trait("ValueObject", "Marca")]
        public void Marca_ComMarcasValidas_DeveAceitarMarca(string marcaValida)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", marcaValida, "Preto", 2020, TipoVeiculoEnum.Carro);

            // Assert
            veiculo.Marca.Valor.Should().Be(marcaValida.Trim());
        }

        [Theory(DisplayName = "Não deve criar veículo se a marca for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Marca")]
        public void Criar_ComMarcaInvalida_DeveLancarExcecao(string marcaInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", marcaInvalida, "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Marca não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a marca for nula")]
        [Trait("ValueObject", "Marca")]
        public void Criar_ComMarcaNula_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", null!, "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Marca não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a marca for muito longa")]
        [Trait("ValueObject", "Marca")]
        public void Criar_ComMarcaMuitoLonga_DeveLancarExcecao()
        {
            // Arrange
            var marcaLonga = new string('A', 201); // 201 caracteres

            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", marcaLonga, "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Marca não pode ter mais de 200 caracteres*");
        }

        #endregion

        #region Testes ValueObject Cor

        [Theory(DisplayName = "Deve aceitar cores válidas")]
        [InlineData("Preto")]
        [InlineData("Branco")]
        [InlineData("Azul")]
        [InlineData("A")] // Mínimo de 1 caractere
        [InlineData(" Cor com espaços ")] // Com espaços - devem ser removidos
        [InlineData("CorComNomeMuitoLongoMasAindaDentroDoLimiteDe100CaracteresQueEhOLimiteMaximoParaOCampoCor")] // 100 caracteres
        [Trait("ValueObject", "Cor")]
        public void Cor_ComCoresValidas_DeveAceitarCor(string corValida)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", corValida, 2020, TipoVeiculoEnum.Carro);

            // Assert
            veiculo.Cor.Valor.Should().Be(corValida.Trim());
        }

        [Theory(DisplayName = "Não deve criar veículo se a cor for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Cor")]
        public void Criar_ComCorInvalida_DeveLancarExcecao(string corInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", corInvalida, 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Cor não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a cor for nula")]
        [Trait("ValueObject", "Cor")]
        public void Criar_ComCorNula_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", null!, 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Cor não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a cor for muito longa")]
        [Trait("ValueObject", "Cor")]
        public void Criar_ComCorMuitoLonga_DeveLancarExcecao()
        {
            // Arrange
            var corLonga = new string('A', 101); // 101 caracteres

            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", corLonga, 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Cor não pode ter mais de 100 caracteres*");
        }

        #endregion

        #region Testes ValueObject Ano

        [Theory(DisplayName = "Deve aceitar anos válidos")]
        [InlineData(1885)] // Ano mínimo
        [InlineData(2020)]
        [InlineData(2024)]
        [Trait("ValueObject", "Ano")]
        public void Ano_ComAnosValidos_DeveAceitarAno(int anoValido)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", anoValido, TipoVeiculoEnum.Carro);

            // Assert
            veiculo.Ano.Valor.Should().Be(anoValido);
        }

        [Fact(DisplayName = "Deve aceitar ano futuro (próximo ano)")]
        [Trait("ValueObject", "Ano")]
        public void Ano_ComAnoFuturoValido_DeveAceitarAno()
        {
            // Arrange
            var anoProximo = DateTime.Now.Year + 1;

            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", anoProximo, TipoVeiculoEnum.Carro);

            // Assert
            veiculo.Ano.Valor.Should().Be(anoProximo);
        }

        [Theory(DisplayName = "Não deve criar veículo se o ano for inválido")]
        [InlineData(1884)]
        [Trait("ValueObject", "Ano")]
        public void Criar_ComAnoInvalido_DeveLancarExcecao(int anoInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", anoInvalido, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Ano deve estar entre*");
        }

        [Fact(DisplayName = "Não deve criar veículo se o ano for futuro demais")]
        [Trait("ValueObject", "Ano")]
        public void Criar_ComAnoFuturoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var anoInvalido = DateTime.Now.Year + 2;

            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", anoInvalido, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Ano deve estar entre*");
        }

        #endregion

        #region Testes ValueObject TipoVeiculo

        [Theory(DisplayName = "Deve aceitar todos os tipos de veículo válidos")]
        [InlineData(TipoVeiculoEnum.Carro)]
        [InlineData(TipoVeiculoEnum.Moto)]
        [Trait("ValueObject", "TipoVeiculo")]
        public void TipoVeiculo_ComTiposValidos_DeveAceitarTipo(TipoVeiculoEnum tipoValido)
        {
            // Act
            var veiculo = Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", 2020, tipoValido);

            // Assert
            veiculo.TipoVeiculo.Valor.Should().Be(tipoValido);
        }

        [Theory(DisplayName = "Não deve criar veículo se o tipo de veículo for inválido")]
        [InlineData((TipoVeiculoEnum)999)]
        [InlineData((TipoVeiculoEnum)0)]
        [InlineData((TipoVeiculoEnum)3)]
        [InlineData((TipoVeiculoEnum)(-1))]
        [Trait("ValueObject", "TipoVeiculo")]
        public void Criar_ComTipoVeiculoInvalido_DeveLancarExcecao(TipoVeiculoEnum tipoInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(Guid.NewGuid(), "ABC1234", "Civic", "Honda", "Preto", 2020, tipoInvalido);
            act.Should().Throw<DomainException>()
                .WithMessage("*Tipo de veículo*não é válido*");
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar veículo")]
        [Trait("Método", "Criar")]
        public void VeiculoCriar_Deve_GerarUuidVersao7_Quando_CriarVeiculo()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placa = "ABC1234";
            var modelo = "Civic";
            var marca = "Honda";
            var cor = "Preto";
            var ano = 2020;
            var tipoVeiculo = TipoVeiculoEnum.Carro;

            // Act
            var veiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);

            // Assert
            veiculo.Id.Should().NotBe(Guid.Empty);
            var guidString = veiculo.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }
}
