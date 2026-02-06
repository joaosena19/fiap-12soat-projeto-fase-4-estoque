namespace API.Configurations.Swagger;

/// <summary>
/// DTO para resposta de login
/// </summary>
/// <param name="Token">Token JWT gerado para autenticação</param>
/// <param name="TokenType">Tipo do token (padrão: Bearer)</param>
/// <param name="ExpiresIn">Tempo de expiração do token em segundos</param>
public record LoginResponseDto(string Token, string TokenType = "Bearer", int ExpiresIn = 3600);