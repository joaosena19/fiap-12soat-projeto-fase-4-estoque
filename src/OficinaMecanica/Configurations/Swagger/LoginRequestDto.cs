namespace API.Configurations.Swagger;

/// <summary>
/// DTO para requisição de login
/// </summary>
/// <param name="DocumentoIdentificadorUsuario">CPF ou CNPJ do usuário</param>
/// <param name="Senha">Senha do usuário para autenticação</param>
public record LoginRequestDto(string DocumentoIdentificadorUsuario, string Senha);