namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para retorno do tempo médio de ordens de serviço
    /// </summary>
    public class RetornoTempoMedioDto
    {
        /// <summary>
        /// Quantidade de dias considerados no cálculo
        /// </summary>
        /// <example>30</example>
        public int QuantidadeDias { get; set; }

        /// <summary>
        /// Data inicial do período considerado
        /// </summary>
        /// <example>2024-12-28T00:00:00Z</example>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final do período considerado
        /// </summary>
        /// <example>2025-01-27T23:59:59Z</example>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Quantidade de ordens de serviço analisadas
        /// </summary>
        /// <example>15</example>
        public int QuantidadeOrdensAnalisadas { get; set; }

        /// <summary>
        /// Tempo médio completo em horas (da criação até a entrega)
        /// </summary>
        /// <example>72.5</example>
        public double TempoMedioCompletoHoras { get; set; }

        /// <summary>
        /// Tempo médio de execução em horas (do início da execução até a finalização)
        /// </summary>
        /// <example>24.3</example>
        public double TempoMedioExecucaoHoras { get; set; }
    }
}
