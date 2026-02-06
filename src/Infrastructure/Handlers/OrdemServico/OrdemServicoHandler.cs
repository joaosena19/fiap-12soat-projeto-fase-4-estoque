using Application.OrdemServico.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.Identidade.Services;
using Application.Contracts.Monitoramento;
using Domain.OrdemServico.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.OrdemServico
{
    public class OrdemServicoHandler : BaseHandler
    {
        public OrdemServicoHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }
        public async Task BuscarOrdensServicoAsync(Ator ator, IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter)
        {
            var useCase = new BuscarOrdensServicoUseCase();
            var logger = CriarLoggerPara<BuscarOrdensServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, gateway, presenter, logger);
        }

        public async Task BuscarOrdemServicoPorIdAsync(Ator ator, Guid id, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorIdPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorIdUseCase();
            var logger = CriarLoggerPara<BuscarOrdemServicoPorIdUseCase>();
            
            await useCase.ExecutarAsync(ator, id, gateway, veiculoGateway, presenter, logger);
        }

        public async Task BuscarOrdemServicoPorCodigoAsync(Ator ator, string codigo, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IBuscarOrdemServicoPorCodigoPresenter presenter)
        {
            var useCase = new BuscarOrdemServicoPorCodigoUseCase();
            var logger = CriarLoggerPara<BuscarOrdemServicoPorCodigoUseCase>();
            
            await useCase.ExecutarAsync(ator, codigo, gateway, veiculoGateway, presenter, logger);
        }

        public async Task CriarOrdemServicoAsync(Ator ator, Guid veiculoId, IOrdemServicoGateway gateway, IVeiculoExternalService veiculoExternalService, IClienteExternalService clienteExternalService, ICriarOrdemServicoPresenter presenter, IMetricsService metricsService)
        {
            var useCase = new CriarOrdemServicoUseCase();
            var logger = CriarLoggerPara<CriarOrdemServicoUseCase>();

            await useCase.ExecutarAsync(ator, veiculoId, gateway, veiculoExternalService, presenter, logger, clienteExternalService, metricsService);
        }

        public async Task CriarOrdemServicoCompletaAsync(Ator ator, Application.OrdemServico.Dtos.CriarOrdemServicoCompletaDto dto, IOrdemServicoGateway ordemServicoGateway, IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway, IServicoGateway servicoGateway, IItemEstoqueGateway itemEstoqueGateway, ICriarOrdemServicoCompletaPresenter presenter, IMetricsService metricsService)
        {
            var useCase = new CriarOrdemServicoCompletaUseCase();
            var logger = CriarLoggerPara<CriarOrdemServicoCompletaUseCase>();
            
            await useCase.ExecutarAsync(ator, dto, ordemServicoGateway, clienteGateway, veiculoGateway, servicoGateway, itemEstoqueGateway, presenter, logger, metricsService);
        }

        public async Task AdicionarServicosAsync(Ator ator, Guid ordemServicoId, List<Guid> servicosOriginaisIds, IOrdemServicoGateway gateway, IServicoExternalService servicoExternalService, IAdicionarServicosPresenter presenter)
        {
            var useCase = new AdicionarServicosUseCase();
            var logger = CriarLoggerPara<AdicionarServicosUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, servicosOriginaisIds, gateway, servicoExternalService, presenter, logger);
        }

        public async Task AdicionarItemAsync(Ator ator, Guid ordemServicoId, Guid itemEstoqueOriginalId, int quantidade, IOrdemServicoGateway gateway, IEstoqueExternalService estoqueExternalService, IAdicionarItemPresenter presenter)
        {
            var useCase = new AdicionarItemUseCase();
            var logger = CriarLoggerPara<AdicionarItemUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, itemEstoqueOriginalId, quantidade, gateway, estoqueExternalService, presenter, logger);
        }

        public async Task RemoverServicoAsync(Ator ator, Guid ordemServicoId, Guid servicoIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverServicoUseCase();
            var logger = CriarLoggerPara<RemoverServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, servicoIncluidoId, gateway, presenter, logger);
        }

        public async Task RemoverItemAsync(Ator ator, Guid ordemServicoId, Guid itemIncluidoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new RemoverItemUseCase();
            var logger = CriarLoggerPara<RemoverItemUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, itemIncluidoId, gateway, presenter, logger);
        }

        public async Task CancelarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new CancelarOrdemServicoUseCase();
            var logger = CriarLoggerPara<CancelarOrdemServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter, logger);
        }

        public async Task IniciarDiagnosticoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IMetricsService metricsService)
        {
            var useCase = new IniciarDiagnosticoUseCase();
            var logger = CriarLoggerPara<IniciarDiagnosticoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter, logger, metricsService);
        }

        public async Task GerarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IGerarOrcamentoPresenter presenter)
        {
            var useCase = new GerarOrcamentoUseCase();
            var logger = CriarLoggerPara<GerarOrcamentoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter, logger);
        }

        public async Task AprovarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IEstoqueExternalService estoqueExternalService, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AprovarOrcamentoUseCase();
            var logger = CriarLoggerPara<AprovarOrcamentoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, veiculoGateway, estoqueExternalService, presenter, logger);
        }

        public async Task DesaprovarOrcamentoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IVeiculoGateway veiculoGateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new DesaprovarOrcamentoUseCase();
            var logger = CriarLoggerPara<DesaprovarOrcamentoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, veiculoGateway, presenter, logger);
        }

        public async Task FinalizarExecucaoAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IMetricsService metricsService)
        {
            var useCase = new FinalizarExecucaoUseCase();
            var logger = CriarLoggerPara<FinalizarExecucaoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter, logger, metricsService);
        }

        public async Task EntregarAsync(Ator ator, Guid ordemServicoId, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter, IMetricsService metricsService)
        {
            var useCase = new EntregarOrdemServicoUseCase();
            var logger = CriarLoggerPara<EntregarOrdemServicoUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, gateway, presenter, logger, metricsService);
        }

        public async Task ObterTempoMedioAsync(Ator ator, int quantidadeDias, IOrdemServicoGateway gateway, IObterTempoMedioPresenter presenter)
        {
            var useCase = new ObterTempoMedioUseCase();
            var logger = CriarLoggerPara<ObterTempoMedioUseCase>();
            
            await useCase.ExecutarAsync(ator, quantidadeDias, gateway, presenter, logger);
        }

        public async Task BuscaPublicaAsync(string codigoOrdemServico, string documentoIdentificadorCliente, IOrdemServicoGateway gateway, IClienteExternalService clienteExternalService, IBuscaPublicaOrdemServicoPresenter presenter)
        {
            var useCase = new BuscaPublicaOrdemServicoUseCase();
            var logger = CriarLoggerPara<BuscaPublicaOrdemServicoUseCase>();
            
            await useCase.ExecutarAsync(codigoOrdemServico, documentoIdentificadorCliente, gateway, clienteExternalService, presenter, logger);
        }

        public async Task AlterarStatusAsync(Ator ator, Guid ordemServicoId, StatusOrdemServicoEnum status, IOrdemServicoGateway gateway, IOperacaoOrdemServicoPresenter presenter)
        {
            var useCase = new AlterarStatusUseCase();
            var logger = CriarLoggerPara<AlterarStatusUseCase>();
            
            await useCase.ExecutarAsync(ator, ordemServicoId, status, gateway, presenter, logger);
        }
    }
}