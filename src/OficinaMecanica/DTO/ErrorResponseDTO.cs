namespace API.Dtos
{
    /// <summary>
    /// DTO para respostas de erro padronizadas da API
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// Mensagem de erro
        /// </summary>
        /// <example>Mensagem de erro</example>
        public string Message { get; set; }

        /// <summary>
        /// Código de status HTTP
        /// </summary>
        /// <example>400</example>
        public int StatusCode { get; set; }

        /// <summary>
        /// Construtor com parâmetros
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        /// <param name="statusCode">Código de status HTTP</param>
        public ErrorResponseDto(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
