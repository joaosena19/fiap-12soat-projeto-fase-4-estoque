namespace Application.Identidade.Dtos
{
    /// <summary>
    /// DTO para retorno de usuário
    /// </summary>
    public class RetornoUsuarioDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Documento de identificação do usuário (CPF ou CNPJ - apenas números)
        /// </summary>
        /// <example>12345678901</example>
        public string DocumentoIdentificador { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do documento de identifação (CPF ou CNPJ)
        /// </summary>
        /// <example>CPF</example>
        public string TipoDocumentoIdentificador { get; set; } = string.Empty;

        /// <summary>
        /// Lista de roles do usuário
        /// </summary>
        /// <example>["Administrador", "Cliente"]</example>
        public List<string> Roles { get; set; } = new();
    }
}