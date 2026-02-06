using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Cadastros.Enums;
using Infrastructure.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.Cadastros
{
    public class VeiculoHandler : BaseHandler
    {
        public VeiculoHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public async Task CriarVeiculoAsync(Ator ator, Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo,
            IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, ICriarVeiculoPresenter presenter)
        {
            var useCase = new CriarVeiculoUseCase();
            var logger = CriarLoggerPara<CriarVeiculoUseCase>();
            
            await useCase.ExecutarAsync(ator, clienteId, placa, modelo, marca, cor, ano, tipoVeiculo, veiculoGateway, clienteGateway, presenter, logger);
        }

        public async Task AtualizarVeiculoAsync(Ator ator, Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo,
            IVeiculoGateway gateway, IAtualizarVeiculoPresenter presenter)
        {
            var useCase = new AtualizarVeiculoUseCase();
            var logger = CriarLoggerPara<AtualizarVeiculoUseCase>();

            await useCase.ExecutarAsync(ator, id, modelo, marca, cor, ano, tipoVeiculo, gateway, presenter, logger);
        }

        public async Task BuscarVeiculosAsync(Ator ator, IVeiculoGateway gateway, IBuscarVeiculosPresenter presenter)
        {
            var useCase = new BuscarVeiculosUseCase();
            var logger = CriarLoggerPara<BuscarVeiculosUseCase>();
            
            await useCase.ExecutarAsync(ator, gateway, presenter, logger);
        }

        public async Task BuscarVeiculoPorIdAsync(Ator ator, Guid id, IVeiculoGateway gateway, IBuscarVeiculoPorIdPresenter presenter)
        {
            var useCase = new BuscarVeiculoPorIdUseCase();
            var logger = CriarLoggerPara<BuscarVeiculoPorIdUseCase>();
            
            await useCase.ExecutarAsync(ator, id, gateway, presenter, logger);
        }

        public async Task BuscarVeiculoPorPlacaAsync(Ator ator, string placa, IVeiculoGateway gateway, IBuscarVeiculoPorPlacaPresenter presenter)
        {
            var useCase = new BuscarVeiculoPorPlacaUseCase();
            var logger = CriarLoggerPara<BuscarVeiculoPorPlacaUseCase>();
            
            await useCase.ExecutarAsync(ator, placa, gateway, presenter, logger);
        }

        public async Task BuscarVeiculosPorClienteAsync(Ator ator, Guid clienteId, IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, IBuscarVeiculosPorClientePresenter presenter)
        {
            var useCase = new BuscarVeiculosPorClienteUseCase();
            var logger = CriarLoggerPara<BuscarVeiculosPorClienteUseCase>();
            
            await useCase.ExecutarAsync(ator, clienteId, veiculoGateway, clienteGateway, presenter, logger);
        }
    }
}