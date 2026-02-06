using Shared.Enums;

namespace Shared.Exceptions
{
    /// <summary>
    /// Exception customizada que carrega informações sobre o tipo de erro customizado
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Tipo de erro customizado
        /// </summary>
        public ErrorType ErrorType { get; }
        
        // Propriedades para o Log Estruturado
        public string LogTemplate { get; }
        public object[] LogArgs { get; }

        /// <summary>
        /// Construtor com valores padrão
        /// </summary>
        /// <param name="message">Mensagem de erro (padrão: "Invalid input")</param>
        /// <param name="errorType">Tipo de erro customizado (padrão: InvalidInput)</param>
        public DomainException(string message = "Invalid input", ErrorType errorType = ErrorType.InvalidInput) 
            : base(message)
        {
            ErrorType = errorType;
            LogTemplate = message; // Default: Log igual msg do usuário
            LogArgs = Array.Empty<object>();
        }

        public DomainException(string mensagemUsuario, ErrorType errorType, string logTemplate, params object[] logArgs) 
            : base(mensagemUsuario)
        {
            ErrorType = errorType;
            LogTemplate = logTemplate;
            LogArgs = logArgs;
        }
    }
}
