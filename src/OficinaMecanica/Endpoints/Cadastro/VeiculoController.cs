using API.Dtos;
using API.Presenters.Cadastro.Veiculo;
using Application.Cadastros.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.Cadastros;
using Infrastructure.Repositories.Cadastros;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Cadastro
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de veículos
    /// </summary>
    [Route("api/cadastros/veiculos")]
    [ApiController]
    [Produces("application/json")]
    public class VeiculoController : BaseController
    {
        private readonly AppDbContext _context;

        public VeiculoController(AppDbContext context, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _context = context;
        }

        /// <summary>
        /// Buscar todos os veículos
        /// </summary>
        /// <returns>Lista de veículos</returns>
        /// <response code="200">Lista de veículos retornada com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoVeiculoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var presenter = new BuscarVeiculosPresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarVeiculosAsync(ator, veiculoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar veículo por ID
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <returns>Veículo encontrado</returns>
        /// <response code="200">Veículo encontrado com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var presenter = new BuscarVeiculoPorIdPresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarVeiculoPorIdAsync(ator, id, veiculoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar veículo por placa
        /// </summary>
        /// <param name="placa">Placa do veículo</param>
        /// <returns>Veículo encontrado</returns>
        /// <response code="200">Veículo encontrado com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("placa/{placa}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPlaca(string placa)
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var presenter = new BuscarVeiculoPorPlacaPresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarVeiculoPorPlacaAsync(ator, placa, veiculoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar veículos por ID do cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Lista de veículos do cliente</returns>
        /// <response code="200">Veículos encontrados com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="422">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("cliente/{clienteId}")]
        [ProducesResponseType(typeof(IEnumerable<RetornoVeiculoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByClienteId(Guid clienteId)
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var clienteGateway = new ClienteRepository(_context);
            var presenter = new BuscarVeiculosPorClientePresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarVeiculosPorClienteAsync(ator, clienteId, veiculoGateway, clienteGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar um novo veículo
        /// </summary>
        /// <param name="dto">Dados do veículo a ser criado</param>
        /// <returns>Veículo criado com sucesso</returns>
        /// <response code="201">Veículo criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="409">Placa já cadastrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarVeiculoDto dto)
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var clienteGateway = new ClienteRepository(_context);
            var presenter = new CriarVeiculoPresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.CriarVeiculoAsync(ator, dto.ClienteId, dto.Placa, dto.Modelo, dto.Marca, dto.Cor, dto.Ano, dto.TipoVeiculo, veiculoGateway, clienteGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Atualizar um veículo existente
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="dto">Dados do veículo para atualização</param>
        /// <returns>Veículo atualizado com sucesso</returns>
        /// <response code="200">Veículo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Veículo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoVeiculoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarVeiculoDto dto)
        {
            var veiculoGateway = new VeiculoRepository(_context);
            var presenter = new AtualizarVeiculoPresenter();
            var handler = new VeiculoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();

            await handler.AtualizarVeiculoAsync(ator, id, dto.Modelo, dto.Marca, dto.Cor, dto.Ano, dto.TipoVeiculo, veiculoGateway, presenter);
            return presenter.ObterResultado();
        }
    }
}
