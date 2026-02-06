using Domain.Cadastros.Enums;

namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para criação de veículo
    /// </summary>
    public class CriarVeiculoDto
    {
        /// <summary>
        /// Id do cliente proprietário do veículo
        /// </summary>
        /// <example>123e4567-e89b-12d3-a456-426614174000</example>
        public Guid ClienteId { get; set; } = Guid.Empty;

        /// <summary>
        /// Placa do veículo (7 caracteres alfanuméricos)
        /// </summary>
        /// <example>ABC1234</example>
        public string Placa { get; set; } = string.Empty;

        /// <summary>
        /// Modelo do veículo
        /// </summary>
        /// <example>Civic</example>
        public string Modelo { get; set; } = string.Empty;

        /// <summary>
        /// Marca do veículo
        /// </summary>
        /// <example>Honda</example>
        public string Marca { get; set; } = string.Empty;

        /// <summary>
        /// Cor do veículo
        /// </summary>
        /// <example>Preto</example>
        public string Cor { get; set; } = string.Empty;

        /// <summary>
        /// Ano do veículo
        /// </summary>
        /// <example>2020</example>
        public int Ano { get; set; }

        /// <summary>
        /// Tipo do veículo
        /// </summary>
        /// <example>carro</example>
        public TipoVeiculoEnum TipoVeiculo { get; set; }
    }
}
