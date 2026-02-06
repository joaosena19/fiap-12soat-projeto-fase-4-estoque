namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para atualização de cliente
    /// </summary>
    public class AtualizarClienteDto
    {
        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;
    }
}
