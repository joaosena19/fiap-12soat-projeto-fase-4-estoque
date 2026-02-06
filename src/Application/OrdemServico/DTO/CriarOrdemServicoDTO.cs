namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para criação de ordem de serviço
    /// </summary>
    public class CriarOrdemServicoDto
    {
        /// <summary>
        /// ID do veículo para o qual a ordem de serviço está sendo criada
        /// </summary>
        /// <example>123e4567-e89b-12d3-a456-426614174000</example>
        public Guid VeiculoId { get; set; } = Guid.Empty;
    }
}
