using Infrastructure.Authentication.PasswordHashing;
using Microsoft.Extensions.Options;
using Shared.Exceptions;
using Shared.Options;
using Shared.Enums;

namespace Tests.Other.Authentication
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;
        private readonly Argon2HashingOptions _options;

        public PasswordHasherTests()
        {
            _options = new Argon2HashingOptions
            {
                SaltSize = 16,
                HashSize = 32,
                Iterations = 4,
                MemorySize = 65536,
                DegreeOfParallelism = 1
            };
            _passwordHasher = new PasswordHasher(Options.Create(_options));
        }

        [Fact(DisplayName = "Deve gerar hash válido para senha válida")]
        public void Hash_DeveGerarHashValido_ParaSenhaValida()
        {
            // Arrange
            var senha = "senha12345";

            // Act
            var hash = _passwordHasher.Hash(senha);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            
            // Verifica se o hash em base64 tem o tamanho esperado
            var hashBytes = Convert.FromBase64String(hash);
            Assert.Equal(_options.SaltSize + _options.HashSize, hashBytes.Length);
        }

        [Fact(DisplayName = "Deve gerar hashes diferentes para a mesma senha")]
        public void Hash_DeveGerarHashesDiferentes_ParaMesmaSenha()
        {
            // Arrange
            var senha = "senha12345";

            // Act
            var hash1 = _passwordHasher.Hash(senha);
            var hash2 = _passwordHasher.Hash(senha);

            // Assert
            Assert.NotEqual(hash1, hash2); // Salts diferentes devem gerar hashes diferentes
        }

        [Theory(DisplayName = "Deve lançar exceção para senhas inválidas")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1234567")] // Menos de 8 caracteres
        [InlineData("a234567890123456789012345678901234567890123456789012345678901234567890")] // Mais de 64 caracteres
        public void Hash_DeveLancarExcecao_ParaSenhasInvalidas(string senhaInvalida)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => _passwordHasher.Hash(senhaInvalida));
            Assert.Equal("Senha deve possuir entre 8 e 64 caracteres", exception.Message);
            Assert.Equal(ErrorType.InvalidInput, exception.ErrorType);
        }

        [Fact(DisplayName = "Deve funcionar com caracteres especiais e unicode")]
        public void Hash_DeveFuncionar_ComCaracteresEspeciais()
        {
            // Arrange
            var senha = "Minhä_S3nh@_Ç0mplëx4!";

            // Act
            var hash = _passwordHasher.Hash(senha);

            // Assert
            Assert.NotNull(hash);
        }
    }
}