using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Identidade.Services;
using Shared.Exceptions;
using Shared.Enums;

namespace Infrastructure.Authentication.AtorFactories
{
    public static class AtorJwtFactory
    {
        public static Ator CriarPorTokenJwt(string tokenJwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            if (!tokenHandler.CanReadToken(tokenJwt))
                throw new DomainException("Token JWT inválido", ErrorType.Unauthorized);

            var jwt = tokenHandler.ReadJwtToken(tokenJwt);
            
            // Extrair UsuarioId
            var usuarioIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "userId" || x.Type == ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !Guid.TryParse(usuarioIdClaim.Value, out var usuarioId))
                throw new DomainException("Token deve conter userId válido", ErrorType.Unauthorized);

            // Extrair ClienteId (opcional)
            var clienteIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "clienteId");
            Guid? clienteId = null;
            if (clienteIdClaim != null && Guid.TryParse(clienteIdClaim.Value, out var parsedClienteId))
                clienteId = parsedClienteId;

            // Extrair Roles
            var rolesClaims = jwt.Claims.Where(x => x.Type == "role" || x.Type == ClaimTypes.Role).ToList();
            if (!rolesClaims.Any())
                throw new DomainException("Token deve conter pelo menos uma role", ErrorType.Unauthorized);

            var roles = new List<RoleEnum>();
            foreach (var roleClaim in rolesClaims)
            {
                if (Enum.TryParse<RoleEnum>(roleClaim.Value, true, out var role))
                    roles.Add(role);
                else
                    throw new DomainException($"Role '{roleClaim.Value}' não é válida. Roles permitidas: {string.Join(", ", Enum.GetNames<RoleEnum>())}", ErrorType.Unauthorized);
            }

            return Ator.ComRoles(usuarioId, clienteId, roles);
        }
    }
}