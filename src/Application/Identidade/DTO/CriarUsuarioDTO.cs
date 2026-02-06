namespace Application.Identidade.Dtos
{
    /// <summary>
    /// DTO para criação de usuário
    /// </summary>
    public class CriarUsuarioDto
    {
        /// <summary>
        /// Documento de identificação do usuário (CPF ou CNPJ, com ou sem formatação)
        /// </summary>
        /// <example>12345678901</example>
        public string DocumentoIdentificador { get; set; } = string.Empty;

        /// <summary>
        /// Senha em texto claro (será criptografada pelo sistema)
        /// </summary>
        /// <example>MinhaSenh@123</example>
        public string SenhaNaoHasheada { get; set; } = string.Empty;

        /// <summary>
        /// Lista de roles para o usuário
        /// </summary>
        /// <example>["Administrador", "Cliente"]</example>
        public List<string> Roles { get; set; } = new();
    }
}