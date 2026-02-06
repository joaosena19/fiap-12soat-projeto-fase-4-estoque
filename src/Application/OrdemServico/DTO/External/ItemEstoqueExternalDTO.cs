using Domain.OrdemServico.Enums;

namespace Application.OrdemServico.Dtos.External
{
    /// <summary>
    /// DTO para dados de Item de Estoque vindos do bounded context de Estoque
    /// </summary>
    public class ItemEstoqueExternalDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public TipoItemIncluidoEnum TipoItemIncluido { get; set; } 
    }
}
