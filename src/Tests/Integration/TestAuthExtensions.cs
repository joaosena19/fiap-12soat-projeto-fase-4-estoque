using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tests.Helpers;

namespace Tests.Integration
{
    public static class TestAuthExtensions
    {
        /// <summary>
        /// Cria um cliente HTTP com token JWT válido para administrador
        /// </summary>
        public static HttpClient CreateAuthenticatedClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
            where TEntryPoint : class
        {
            return factory.CreateAuthenticatedClient(isAdmin: true);
        }

        /// <summary>
        /// Cria um cliente HTTP com token JWT válido
        /// </summary>
        public static HttpClient CreateAuthenticatedClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory, 
            bool isAdmin = true, Guid? clienteId = null, Guid? usuarioId = null)
            where TEntryPoint : class
        {
            var client = factory.CreateClient();
            var token = CreateJwtToken(isAdmin, clienteId, usuarioId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        /// <summary>
        /// Cria um cliente HTTP sem autenticação (para testar 401)
        /// </summary>
        public static HttpClient CreateUnauthenticatedClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
            where TEntryPoint : class
        {
            return factory.CreateClient();
        }

        /// <summary>
        /// Gera um token JWT válido para os testes
        /// </summary>
        private static string CreateJwtToken(bool isAdmin = true, Guid? clienteId = null, Guid? usuarioId = null)
        {
            var user = usuarioId ?? Guid.NewGuid();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTestConstants.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new("userId", user.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (clienteId.HasValue)
            {
                claims.Add(new("clienteId", clienteId.Value.ToString()));
            }

            // Adicionar roles
            if (isAdmin)
            {
                claims.Add(new(ClaimTypes.Role, "Administrador"));
            }
            else
            {
                claims.Add(new(ClaimTypes.Role, "Cliente"));
            }

            var token = new JwtSecurityToken(
                issuer: JwtTestConstants.Issuer,
                audience: JwtTestConstants.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
