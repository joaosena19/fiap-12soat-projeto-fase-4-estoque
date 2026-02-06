namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para webhook para passagem de ID
    /// </summary>
    public class WebhookIdDto
    {
        /// <summary>
        /// ID do recurso
        /// </summary>
        /// <example>00000000-0000-0000-0000-000000000000</example>
        public Guid Id { get; set; }
    }
}