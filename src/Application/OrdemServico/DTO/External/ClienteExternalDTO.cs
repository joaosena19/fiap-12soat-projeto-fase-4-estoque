namespace Application.OrdemServico.Dtos.External
{
    /// <summary>
    /// DTO para dados de Cliente vindos do bounded context de Cadastros
    /// </summary>
    public class ClienteExternalDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string DocumentoIdentificador { get; set; } = string.Empty;
    }
}
