namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para busca pública de ordem de serviço por código e documento do cliente
    /// </summary>
    public class BuscaPublicaOrdemServicoDto
    {
        /// <summary>
        /// Código da ordem de serviço
        /// </summary>
        /// <example>OS-20250125-ABC123</example>
        public string CodigoOrdemServico { get; set; } = string.Empty;

        /// <summary>
        /// Documento de identificação do cliente (CPF ou CNPJ)
        /// </summary>
        /// <example>12345678901</example>
        public string DocumentoIdentificadorCliente { get; set; } = string.Empty;
    }
}
