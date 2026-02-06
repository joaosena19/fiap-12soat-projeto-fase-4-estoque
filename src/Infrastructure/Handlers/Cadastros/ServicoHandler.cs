using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Infrastructure.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.Cadastros
{
    public class ServicoHandler : BaseHandler
    {
        public ServicoHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public async Task CriarServicoAsync(Ator ator, string nome, decimal preco, IServicoGateway gateway, ICriarServicoPresenter presenter)
        {
            var useCase = new CriarServicoUseCase();
            var logger = CriarLoggerPara<CriarServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, nome, preco, gateway, presenter, logger);
        }

        public async Task AtualizarServicoAsync(Ator ator, Guid id, string nome, decimal preco, IServicoGateway gateway, IAtualizarServicoPresenter presenter)
        {
            var useCase = new AtualizarServicoUseCase();
            var logger = CriarLoggerPara<AtualizarServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, id, nome, preco, gateway, presenter, logger);
        }

        public async Task BuscarServicosAsync(Ator ator, IServicoGateway gateway, IBuscarServicosPresenter presenter)
        {
            var useCase = new BuscarServicosUseCase();
            var logger = CriarLoggerPara<BuscarServicosUseCase>();
            
            await useCase.ExecutarAsync(ator, gateway, presenter, logger);
        }

        public async Task BuscarServicoPorIdAsync(Ator ator, Guid id, IServicoGateway gateway, IBuscarServicoPorIdPresenter presenter)
        {
            var useCase = new BuscarServicoPorIdUseCase();
            var logger = CriarLoggerPara<BuscarServicoPorIdUseCase>();
            
            await useCase.ExecutarAsync(ator, id, gateway, presenter, logger);
        }
    }
}