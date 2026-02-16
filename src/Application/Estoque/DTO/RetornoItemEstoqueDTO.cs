namespace Application.Estoque.Dtos
{
    /// <summary>
    /// DTO para retorno de item de estoque
    /// </summary>
    public class RetornoItemEstoqueDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Nome do item de estoque
        /// </summary>
        /// <example>Filtro de Óleo</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade em estoque
        /// </summary>
        /// <example>50</example>
        public int Quantidade { get; set; }

        /// <summary>
        /// Tipo do item de estoque
        /// </summary>
        /// <example>Peca</example>
        public string TipoItemEstoque { get; set; } = string.Empty;

        /// <summary>
        /// Preço unitário do item
        /// </summary>
        /// <example>25.50</example>
        public decimal Preco { get; set; }
    }
}
