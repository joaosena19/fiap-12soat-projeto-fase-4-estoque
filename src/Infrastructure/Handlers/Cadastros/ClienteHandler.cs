using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Infrastructure.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.Cadastros
{
    public class ClienteHandler : BaseHandler
    {
        public ClienteHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public async Task CriarClienteAsync(Ator ator, string nome, string documento, IClienteGateway clienteGateway, IUsuarioGateway usuarioGateway, ICriarClientePresenter presenter)
        {
            var useCase = new CriarClienteUseCase();
            var logger = CriarLoggerPara<CriarClienteUseCase>();
            
            await useCase.ExecutarAsync(ator, nome, documento, clienteGateway, usuarioGateway, presenter, logger);
        }

        public async Task AtualizarClienteAsync(Ator ator, Guid id, string nome, IClienteGateway gateway, IAtualizarClientePresenter presenter)
        {
            var useCase = new AtualizarClienteUseCase();
            var logger = CriarLoggerPara<AtualizarClienteUseCase>();
            
            await useCase.ExecutarAsync(ator, id, nome, gateway, presenter, logger);
        }

        public async Task BuscarClientesAsync(Ator ator, IClienteGateway gateway, IBuscarClientesPresenter presenter)
        {
            var useCase = new BuscarClientesUseCase();
            var logger = CriarLoggerPara<BuscarClientesUseCase>();
            
            await useCase.ExecutarAsync(ator, gateway, presenter, logger);
        }

        public async Task BuscarClientePorIdAsync(Ator ator, Guid id, IClienteGateway gateway, IBuscarClientePorIdPresenter presenter)
        {
            var useCase = new BuscarClientePorIdUseCase();
            var logger = CriarLoggerPara<BuscarClientePorIdUseCase>();
            
            await useCase.ExecutarAsync(ator, id, gateway, presenter, logger);
        }

        public async Task BuscarClientePorDocumentoAsync(Ator ator, string documento, IClienteGateway gateway, IBuscarClientePorDocumentoPresenter presenter)
        {
            var useCase = new BuscarClientePorDocumentoUseCase();
            var logger = CriarLoggerPara<BuscarClientePorDocumentoUseCase>();
            
            await useCase.ExecutarAsync(ator, documento, gateway, presenter, logger);
        }
    }
}