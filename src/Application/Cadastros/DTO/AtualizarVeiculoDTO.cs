using Domain.Cadastros.Enums;

namespace Application.Cadastros.Dtos
{
    /// <summary>
    /// DTO para atualização de veículo
    /// </summary>
    public class AtualizarVeiculoDto
    {
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
