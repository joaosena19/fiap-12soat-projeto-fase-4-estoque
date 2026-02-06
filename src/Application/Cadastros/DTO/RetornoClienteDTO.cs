namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para retorno de cliente
    /// </summary>
    public class RetornoClienteDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Documento de identificação do cliente (CPF ou CNPJ - apenas números)
        /// </summary>
        /// <example>12345678901</example>
        public string DocumentoIdentificador { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do documento de identifação (CPF ou CNPJ)
        /// </summary>
        /// <example>CPF</example>
        public string TipoDocumentoIdentificador { get; set; } = string.Empty;
    }
}
