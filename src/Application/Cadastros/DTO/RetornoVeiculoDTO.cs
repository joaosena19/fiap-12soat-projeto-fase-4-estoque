namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para retorno de veículo
    /// </summary>
    public class RetornoVeiculoDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Id do cliente proprietário do veículo
        /// </summary>
        /// <example>123e4567-e89b-12d3-a456-426614174000</example>
        public Guid ClienteId { get; set; } = Guid.Empty;

        /// <summary>
        /// Placa do veículo
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
        /// <example>Carro</example>
        public string TipoVeiculo { get; set; } = string.Empty;
    }
}
