using Application.Contracts.Monitoramento;
using Infrastructure.Database;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging
{
    /// <summary>
    /// Classe base para consumers de mensageria.
    /// Fornece acesso ao contexto do banco de dados e facilita a criação de loggers seguindo o padrão Pure DI.
    /// </summary>
    public abstract class BaseConsumer
    {
        protected readonly AppDbContext _context;
        private readonly ILoggerFactory _loggerFactory;

        protected BaseConsumer(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Fábrica centralizada para criar loggers adaptados para Clean Architecture.
        /// Permite criar um logger com a categoria apropriada para qualquer tipo.
        /// </summary>
        /// <typeparam name="TCategory">O tipo que será usado como categoria do logger</typeparam>
        /// <returns>Logger adaptado que implementa IAppLogger</returns>
        protected IAppLogger CriarLoggerPara<TCategory>()
        {
            // 1. Cria o logger nativo do .NET (com a categoria correta TCategory)
            var aspNetLogger = _loggerFactory.CreateLogger<TCategory>();

            // 2. Encapsula no Adapter da Application
            return new LoggerAdapter<TCategory>(aspNetLogger);
        }
    }
}
