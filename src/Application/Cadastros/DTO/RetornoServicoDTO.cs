namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para retorno de servico
    /// </summary>
    public class RetornoServicoDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Nome do servico
        /// </summary>
        /// <example>Troca de óleo</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Preço do serviço
        /// </summary>
        /// <example>100.00</example>
        public decimal Preco { get; set; } = 0L;
    }
}
