namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para retorno de item incluído na ordem de serviço
    /// </summary>
    public class RetornoItemIncluidoDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// ID do item de estoque original
        /// </summary>
        public Guid ItemEstoqueOriginalId { get; set; } = Guid.Empty;

        /// <summary>
        /// Nome do item
        /// </summary>
        /// <example>Filtro de Óleo</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade do item
        /// </summary>
        /// <example>2</example>
        public int Quantidade { get; set; } = 0;

        /// <summary>
        /// Tipo do item incluído
        /// </summary>
        /// <example>peca</example>
        public string TipoItemIncluido { get; set; } = string.Empty;

        /// <summary>
        /// Preço unitário do item
        /// </summary>
        /// <example>25.50</example>
        public decimal Preco { get; set; } = 0M;
    }
}
