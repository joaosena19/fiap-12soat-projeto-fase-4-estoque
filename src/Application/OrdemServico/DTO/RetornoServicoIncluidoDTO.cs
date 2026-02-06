namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para retorno de serviço incluído na ordem de serviço
    /// </summary>
    public class RetornoServicoIncluidoDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// ID do serviço original no cadastro
        /// </summary>
        public Guid ServicoOriginalId { get; set; } = Guid.Empty;

        /// <summary>
        /// Nome do serviço
        /// </summary>
        /// <example>Troca de óleo</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Preço do serviço
        /// </summary>
        /// <example>100.00</example>
        public decimal Preco { get; set; } = 0M;
    }
}
