namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para adicionar serviços à ordem de serviço
    /// </summary>
    public class AdicionarServicosDto
    {
        /// <summary>
        /// Lista de IDs dos serviços a serem adicionados
        /// </summary>
        /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6", "2fa85f64-5717-4562-b3fc-2c963f66afa7"]</example>
        public List<Guid> ServicosOriginaisIds { get; set; } = new();
    }
}
