using Application.Contracts.Services;
using Shared.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using System.Text;
using Shared.Exceptions;
using Shared.Enums;

namespace Infrastructure.Authentication.PasswordHashing
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly Argon2HashingOptions _options;

        public PasswordHasher(IOptions<Argon2HashingOptions> options)
        {
            _options = options.Value;
        }

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 64)
                throw new DomainException("Senha deve possuir entre 8 e 64 caracteres", ErrorType.InvalidInput);

            // Gerar salt aleatório
            var salt = new byte[_options.SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Criar hash Argon2id
            var hash = HashPassword(password, salt);

            // Combinar salt + hash em uma string base64
            var combinedBytes = new byte[_options.SaltSize + _options.HashSize];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, _options.SaltSize);
            Buffer.BlockCopy(hash, 0, combinedBytes, _options.SaltSize, _options.HashSize);

            return Convert.ToBase64String(combinedBytes);
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = _options.DegreeOfParallelism,
                Iterations = _options.Iterations,
                MemorySize = _options.MemorySize
            };

            return argon2.GetBytes(_options.HashSize);
        }
    }
}