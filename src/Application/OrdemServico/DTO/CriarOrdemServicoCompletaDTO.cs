using Domain.Cadastros.Enums;

namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para criação completa de ordem de serviço com cliente, veículo, serviços e itens
    /// </summary>
    public class CriarOrdemServicoCompletaDto
    {
        public ClienteDto Cliente { get; set; } = new();
        public VeiculoDto Veiculo { get; set; } = new();
        public List<Guid>? ServicosIds { get; set; }
        public List<ItemDto>? Itens { get; set; }
    }

    public class ClienteDto
    {
        public string Nome { get; set; } = string.Empty;
        public string DocumentoIdentificador { get; set; } = string.Empty;
    }

    public class VeiculoDto
    {
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Ano { get; set; }
        public TipoVeiculoEnum TipoVeiculo { get; set; }
    }

    public class ItemDto
    {
        public Guid ItemEstoqueId { get; set; } = Guid.Empty;
        public int Quantidade { get; set; }
    }
}