using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.OrdemServico.UseCases;
using Application.Contracts.Monitoramento;
using Moq;

namespace Tests.Application.OrdemServico.Helpers
{
    public class OrdemServicoTestFixture
    {
        public Mock<IOrdemServicoGateway> OrdemServicoGatewayMock { get; }
        public Mock<IVeiculoGateway> VeiculoGatewayMock { get; }
        public Mock<IEstoqueExternalService> EstoqueExternalServiceMock { get; }
        public Mock<IServicoExternalService> ServicoExternalServiceMock { get; }
        public Mock<IClienteExternalService> ClienteExternalServiceMock { get; }
        public Mock<IAdicionarItemPresenter> AdicionarItemPresenterMock { get; }
        public Mock<IAdicionarServicosPresenter> AdicionarServicosPresenterMock { get; }
        public Mock<IOperacaoOrdemServicoPresenter> OperacaoOrdemServicoPresenterMock { get; }
        public Mock<IGerarOrcamentoPresenter> GerarOrcamentoPresenterMock { get; }
        public Mock<IBuscaPublicaOrdemServicoPresenter> BuscaPublicaOrdemServicoPresenterMock { get; }
        public Mock<IBuscarOrdemServicoPorCodigoPresenter> BuscarOrdemServicoPorCodigoPresenterMock { get; }
        public Mock<IBuscarOrdemServicoPorIdPresenter> BuscarOrdemServicoPorIdPresenterMock { get; }
        public Mock<IBuscarOrdensServicoPresenter> BuscarOrdensServicoPresenterMock { get; }
        public Mock<IObterTempoMedioPresenter> ObterTempoMedioPresenterMock { get; }
        public Mock<IVeiculoExternalService> VeiculoExternalServiceMock { get; }
        public Mock<ICriarOrdemServicoPresenter> CriarOrdemServicoPresenterMock { get; }
        public Mock<IMetricsService> MetricsServiceMock { get; }

        public AdicionarItemUseCase AdicionarItemUseCase { get; }
        public AdicionarServicosUseCase AdicionarServicosUseCase { get; }
        public AprovarOrcamentoUseCase AprovarOrcamentoUseCase { get; }
        public BuscaPublicaOrdemServicoUseCase BuscaPublicaOrdemServicoUseCase { get; }
        public BuscarOrdemServicoPorCodigoUseCase BuscarOrdemServicoPorCodigoUseCase { get; }
        public BuscarOrdemServicoPorIdUseCase BuscarOrdemServicoPorIdUseCase { get; }
        public BuscarOrdensServicoUseCase BuscarOrdensServicoUseCase { get; }
        public CancelarOrdemServicoUseCase CancelarOrdemServicoUseCase { get; }
        public DesaprovarOrcamentoUseCase DesaprovarOrcamentoUseCase { get; }
        public EntregarOrdemServicoUseCase EntregarOrdemServicoUseCase { get; }
        public FinalizarExecucaoUseCase FinalizarExecucaoUseCase { get; }
        public GerarOrcamentoUseCase GerarOrcamentoUseCase { get; }
        public IniciarDiagnosticoUseCase IniciarDiagnosticoUseCase { get; }
        public RemoverServicoUseCase RemoverServicoUseCase { get; }
        public RemoverItemUseCase RemoverItemUseCase { get; }
        public ObterTempoMedioUseCase ObterTempoMedioUseCase { get; }
        public CriarOrdemServicoUseCase CriarOrdemServicoUseCase { get; }
        public AlterarStatusUseCase AlterarStatusUseCase { get; }

        public OrdemServicoTestFixture()
        {
            OrdemServicoGatewayMock = new Mock<IOrdemServicoGateway>();
            VeiculoGatewayMock = new Mock<IVeiculoGateway>();
            EstoqueExternalServiceMock = new Mock<IEstoqueExternalService>();
            ServicoExternalServiceMock = new Mock<IServicoExternalService>();
            ClienteExternalServiceMock = new Mock<IClienteExternalService>();
            AdicionarItemPresenterMock = new Mock<IAdicionarItemPresenter>();
            AdicionarServicosPresenterMock = new Mock<IAdicionarServicosPresenter>();
            OperacaoOrdemServicoPresenterMock = new Mock<IOperacaoOrdemServicoPresenter>();
            GerarOrcamentoPresenterMock = new Mock<IGerarOrcamentoPresenter>();
            BuscaPublicaOrdemServicoPresenterMock = new Mock<IBuscaPublicaOrdemServicoPresenter>();
            BuscarOrdemServicoPorCodigoPresenterMock = new Mock<IBuscarOrdemServicoPorCodigoPresenter>();
            BuscarOrdemServicoPorIdPresenterMock = new Mock<IBuscarOrdemServicoPorIdPresenter>();
            BuscarOrdensServicoPresenterMock = new Mock<IBuscarOrdensServicoPresenter>();
            ObterTempoMedioPresenterMock = new Mock<IObterTempoMedioPresenter>();
            VeiculoExternalServiceMock = new Mock<IVeiculoExternalService>();
            CriarOrdemServicoPresenterMock = new Mock<ICriarOrdemServicoPresenter>();
            MetricsServiceMock = new Mock<IMetricsService>();

            AdicionarItemUseCase = new AdicionarItemUseCase();
            AdicionarServicosUseCase = new AdicionarServicosUseCase();
            AprovarOrcamentoUseCase = new AprovarOrcamentoUseCase();
            BuscaPublicaOrdemServicoUseCase = new BuscaPublicaOrdemServicoUseCase();
            BuscarOrdemServicoPorCodigoUseCase = new BuscarOrdemServicoPorCodigoUseCase();
            BuscarOrdemServicoPorIdUseCase = new BuscarOrdemServicoPorIdUseCase();
            BuscarOrdensServicoUseCase = new BuscarOrdensServicoUseCase();
            CancelarOrdemServicoUseCase = new CancelarOrdemServicoUseCase();
            DesaprovarOrcamentoUseCase = new DesaprovarOrcamentoUseCase();
            EntregarOrdemServicoUseCase = new EntregarOrdemServicoUseCase();
            FinalizarExecucaoUseCase = new FinalizarExecucaoUseCase();
            GerarOrcamentoUseCase = new GerarOrcamentoUseCase();
            IniciarDiagnosticoUseCase = new IniciarDiagnosticoUseCase();
            RemoverServicoUseCase = new RemoverServicoUseCase();
            RemoverItemUseCase = new RemoverItemUseCase();
            ObterTempoMedioUseCase = new ObterTempoMedioUseCase();
            CriarOrdemServicoUseCase = new CriarOrdemServicoUseCase();
            AlterarStatusUseCase = new AlterarStatusUseCase();
        }
    }
}