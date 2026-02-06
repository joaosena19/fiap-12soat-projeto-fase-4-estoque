namespace Application.Estoque.Dtos
{
    /// <summary>
    /// DTO para retorno de verificação de disponibilidade
    /// </summary>
    public class RetornoDisponibilidadeDto
    {
        /// <summary>
        /// Indica se o item está disponível na quantidade solicitada
        /// </summary>
        /// <example>true</example>
        public bool Disponivel { get; set; }

        /// <summary>
        /// Quantidade atual em estoque
        /// </summary>
        /// <example>50</example>
        public int QuantidadeEmEstoque { get; set; }

        /// <summary>
        /// Quantidade solicitada
        /// </summary>
        /// <example>10</example>
        public int QuantidadeSolicitada { get; set; }
    }
}
