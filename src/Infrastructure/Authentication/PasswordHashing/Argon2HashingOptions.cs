namespace Shared.Options
{
    /// <summary>
    /// Configurações para hash de senhas com Argon2id
    /// Essas configurações devem ser compartilhadas entre todas as aplicações
    /// que precisam verificar senhas criadas por este sistema
    /// </summary>
    public class Argon2HashingOptions
    {
        /// <summary>
        /// Tamanho do salt em bytes
        /// </summary>
        public int SaltSize { get; set; }

        /// <summary>
        /// Tamanho do hash resultante em bytes
        /// </summary>
        public int HashSize { get; set; }

        /// <summary>
        /// Número de iterações
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Tamanho da memória em KB
        /// </summary>
        public int MemorySize { get; set; }

        /// <summary>
        /// Grau de paralelismo
        /// </summary>
        public int DegreeOfParallelism { get; set; }
    }
}