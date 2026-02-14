using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Identidade.Services;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Helpers
{
    /// <summary>
    /// Builder para criação de tokens JWT em testes
    /// </summary>
    public class JwtTokenBuilder
    {
        private readonly List<Claim> _claims;
        private DateTime _expiracao;
        private string _key;
        private string _issuer;
        private string _audience;

        public JwtTokenBuilder()
        {
            _claims = new List<Claim>();
            _expiracao = DateTime.UtcNow.AddHours(1);
            _key = JwtTestConstants.Key;
            _issuer = JwtTestConstants.Issuer;
            _audience = JwtTestConstants.Audience;
        }

        public JwtTokenBuilder ComUsuarioId(Guid usuarioId)
        {
            _claims.Add(new Claim("userId", usuarioId.ToString()));
            return this;
        }

        public JwtTokenBuilder ComClienteId(Guid clienteId)
        {
            _claims.Add(new Claim("clienteId", clienteId.ToString()));
            return this;
        }

        public JwtTokenBuilder ComRole(RoleEnum role)
        {
            _claims.Add(new Claim("role", role.ToString()));
            return this;
        }

        public JwtTokenBuilder ComRoles(params RoleEnum[] roles)
        {
            foreach (var role in roles)
                _claims.Add(new Claim("role", role.ToString()));
            return this;
        }

        public JwtTokenBuilder ComClaimCustomizada(string tipo, string valor)
        {
            _claims.Add(new Claim(tipo, valor));
            return this;
        }

        public JwtTokenBuilder ComExpiracao(DateTime expiracao)
        {
            _expiracao = expiracao;
            return this;
        }

        public string Build()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                expires: _expiracao,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string BuildTokenInvalido() => "token-invalido-sem-formato-jwt";

        public string BuildTokenSemAssinatura()
        {
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                expires: _expiracao
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
