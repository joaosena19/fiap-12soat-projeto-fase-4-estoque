namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para adicionar item à ordem de serviço
    /// </summary>
    public class AdicionarItemDto
    {
        /// <summary>
        /// ID do item de estoque
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid ItemEstoqueOriginalId { get; set; } = Guid.Empty;

        /// <summary>
        /// Quantidade do item a ser adicionado
        /// </summary>
        /// <example>2</example>
        public int Quantidade { get; set; } = 0;
    }
}
