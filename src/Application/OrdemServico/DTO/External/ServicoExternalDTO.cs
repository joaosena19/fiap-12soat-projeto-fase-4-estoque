namespace Application.OrdemServico.Dtos.External
{
    /// <summary>
    /// DTO para dados de Serviço vindos do bounded context de Cadastros
    /// </summary>
    public class ServicoExternalDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
    }
}
