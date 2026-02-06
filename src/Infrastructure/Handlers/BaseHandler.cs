using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers
{
    public abstract class BaseHandler
    {
        private readonly ILoggerFactory _loggerFactory;

        protected BaseHandler(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Fábrica centralizada para criar loggers adaptados para Clean Architecture
        /// </summary>
        protected IAppLogger CriarLoggerPara<TUseCase>()
        {
            // 1. Cria o logger nativo do .NET (com a categoria correta TUseCase)
            var aspNetLogger = _loggerFactory.CreateLogger<TUseCase>();

            // 2. Encapsula no Adapter da Application
            return new LoggerAdapter<TUseCase>(aspNetLogger);
        }
    }
}
