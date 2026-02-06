using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Cadastros.ValueObjects.Cliente;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class ClienteTests
    {
        #region Testes Método Criar e Atualizar

        [Fact(DisplayName = "Deve criar novo Cliente com dados válidos")]
        [Trait("Método", "Criar")]
        public void ClienteCriar_Deve_CriarCliente_Quando_DadosValidos()
        {
            // Arrange
            var nome = "João da Silva";
            var cpf = "36050793000";

            // Act
            var cliente = Cliente.Criar(nome, cpf);

            // Assert
            cliente.Should().NotBeNull();
            cliente.Id.Should().NotBe(Guid.Empty);
            cliente.Nome.Valor.Should().Be(nome);
            cliente.DocumentoIdentificador.Valor.Should().Be(cpf);
        }

        [Fact(DisplayName = "Deve atualizar cliente com dados válidos")]
        [Trait("Método", "Atualizar")]
        public void ClienteAtualizar_Deve_AtualizarCliente_Quando_DadosValidos()
        {
            // Arrange
            var nomeOriginal = "João da Silva";
            var cpfOriginal = "36050793000";
            var novoNome = "João Silva Santos";

            var cliente = Cliente.Criar(nomeOriginal, cpfOriginal);

            // Act
            cliente.Atualizar(novoNome);

            // Assert
            cliente.Nome.Valor.Should().Be(novoNome);
            cliente.DocumentoIdentificador.Valor.Should().Be(cpfOriginal); // CPF não deve ter mudado
        }

        #endregion

        #region Testes ValueObject Nome

        [Theory(DisplayName = "Não deve criar novo Cliente se o Nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ClienteCriar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var cpfValido = "36050793000";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeInvalido, cpfValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve atualizar cliente se o nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ClienteAtualizar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var cliente = Cliente.Criar("João da Silva", "36050793000");

            // Act & Assert
            FluentActions.Invoking(() => cliente.Atualizar(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        #endregion

        #region Testes ValueObject DocumentoIdentificador

        [Theory(DisplayName = "Não deve criar novo Cliente se o documento for inválido")]
        [InlineData("")]
        [InlineData("01234567891")] // CPF inválido
        [InlineData("11111111111")] // CPF com todos os dígitos iguais
        [InlineData("12345678901234")] // CNPJ inválido
        [InlineData("11111111111111")] // CNPJ com todos os dígitos iguais
        [InlineData("abc123")] // Contém letras
        [Trait("ValueObject", "DocumentoIdentificador")]
        public void ClienteCriar_Deve_ThrowException_Quando_DocumentoInvalido(string documentoInvalido)
        {
            // Arrange
            var nomeValido = "João";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeValido, documentoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Documento de identificação inválido*");
        }

        [Theory(DisplayName = "Deve criar Cliente com CPFs válidos formatados")]
        [InlineData("360.507.930-00")]
        [InlineData("111.444.777-35")]
        [Trait("Dados Válidos", "CpfFormatado")]
        public void ClienteCriar_Deve_CriarCliente_QuandoCpfFormatadoValido(string cpfFormatado)
        {
            // Arrange
            var nome = "João da Silva";

            // Act
            var cliente = Cliente.Criar(nome, cpfFormatado);

            // Assert
            cliente.Should().NotBeNull();
            cliente.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CPF);
            cliente.DocumentoIdentificador.Valor.Should().MatchRegex(@"^\d{11}$"); // Deve conter apenas 11 dígitos
        }

        [Theory(DisplayName = "Deve criar Cliente com CNPJs válidos formatados")]
        [InlineData("11.222.333/0001-81")]
        [InlineData("47.960.950/0001-21")]
        [Trait("Dados Válidos", "CnpjFormatado")]
        public void ClienteCriar_Deve_CriarCliente_QuandoCnpjFormatadoValido(string cnpjFormatado)
        {
            // Arrange
            var nome = "Empresa ABC Ltda";

            // Act
            var cliente = Cliente.Criar(nome, cnpjFormatado);

            // Assert
            cliente.Should().NotBeNull();
            cliente.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CNPJ);
            cliente.DocumentoIdentificador.Valor.Should().MatchRegex(@"^\d{14}$"); // Deve conter apenas 14 dígitos
        }

        [Fact(DisplayName = "Deve manter tipo de documento após atualização do nome")]
        [Trait("Dados Válidos", "AtualizarComCpf")]
        public void ClienteAtualizar_DeveManterTipoDocumento_AposAtualizacaoNome()
        {
            // Arrange
            var nomeOriginal = "João da Silva";
            var cpf = "36050793000";
            var novoNome = "João Silva Santos";

            var cliente = Cliente.Criar(nomeOriginal, cpf);

            // Act
            cliente.Atualizar(novoNome);

            // Assert
            cliente.Nome.Valor.Should().Be(novoNome);
            cliente.DocumentoIdentificador.Valor.Should().Be(cpf);
            cliente.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CPF);
        }

        [Fact(DisplayName = "Deve manter tipo de documento CNPJ após atualização do nome")]
        [Trait("Dados Válidos", "AtualizarComCnpj")]
        public void ClienteAtualizar_DeveManterTipoDocumentoCnpj_AposAtualizacaoNome()
        {
            // Arrange
            var nomeOriginal = "Empresa ABC Ltda";
            var cnpj = "11222333000181";
            var novoNome = "Empresa ABC Sociedade Ltda";

            var cliente = Cliente.Criar(nomeOriginal, cnpj);

            // Act
            cliente.Atualizar(novoNome);

            // Assert
            cliente.Nome.Valor.Should().Be(novoNome);
            cliente.DocumentoIdentificador.Valor.Should().Be(cnpj);
            cliente.DocumentoIdentificador.TipoDocumento.Should().Be(TipoDocumentoEnum.CNPJ);
        }

        [Theory(DisplayName = "Deve criar DocumentoIdentificador válido para CPF")]
        [InlineData("36050793000")]
        [InlineData("360.507.930-00")]
        [InlineData("111.444.777-35")]
        [InlineData("11144477735")]
        [Trait("ValueObject", "DocumentoIdentificador")]
        public void DocumentoIdentificador_DeveCriar_QuandoCpfValido(string cpfValido)
        {
            // Act
            var documento = new DocumentoIdentificador(cpfValido);

            // Assert
            documento.Should().NotBeNull();
            documento.Valor.Should().NotBeEmpty();
            documento.TipoDocumento.Should().Be(TipoDocumentoEnum.CPF);
        }

        [Theory(DisplayName = "Deve criar DocumentoIdentificador válido para CNPJ")]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        [InlineData("47960950000121")]
        [InlineData("47.960.950/0001-21")]
        [Trait("ValueObject", "DocumentoIdentificador")]
        public void DocumentoIdentificador_DeveCriar_QuandoCnpjValido(string cnpjValido)
        {
            // Act
            var documento = new DocumentoIdentificador(cnpjValido);

            // Assert
            documento.Should().NotBeNull();
            documento.Valor.Should().NotBeEmpty();
            documento.TipoDocumento.Should().Be(TipoDocumentoEnum.CNPJ);
        }

        [Theory(DisplayName = "Não deve criar DocumentoIdentificador com documento inválido")]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("12345678901")] // CPF inválido
        [InlineData("11111111111")] // CPF com todos os dígitos iguais
        [InlineData("123456789012345")] // CNPJ inválido - muito longo
        [InlineData("11111111111111")] // CNPJ com todos os dígitos iguais
        [InlineData("abc")]
        [InlineData("12345")]
        [Trait("ValueObject", "DocumentoIdentificador")]
        public void DocumentoIdentificador_DeveLancarExcecao_QuandoDocumentoInvalido(string documentoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new DocumentoIdentificador(documentoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Documento de identificação inválido");
        }

        [Theory(DisplayName = "Deve limpar formatação do documento")]
        [InlineData("925.072.620-10", "92507262010")]
        [InlineData("66.096.909/0001-01", "66096909000101")]
        [Trait("ValueObject", "DocumentoIdentificador")]
        public void DocumentoIdentificador_DeveLimparFormatacao(string documentoFormatado, string documentoLimpo)
        {
            // Arrange & Act
            var documento = new DocumentoIdentificador(documentoFormatado);

            // Assert
            documento.Valor.Should().Be(documentoLimpo);
        }

        #endregion

        #region Testes UUID Version 7

        [Fact(DisplayName = "Deve gerar UUID versão 7 ao criar cliente")]
        [Trait("Método", "Criar")]
        public void ClienteCriar_Deve_GerarUuidVersao7_Quando_CriarCliente()
        {
            // Arrange
            var nome = "João da Silva";
            var cpf = "36050793000";

            // Act
            var cliente = Cliente.Criar(nome, cpf);

            // Assert
            cliente.Id.Should().NotBe(Guid.Empty);
            var guidString = cliente.Id.ToString();
            var thirdGroup = guidString.Split('-')[2];
            thirdGroup[0].Should().Be('7', "O UUID deve ser versão 7");
        }

        #endregion
    }

}
