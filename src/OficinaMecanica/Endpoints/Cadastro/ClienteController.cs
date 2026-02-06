using API.Dtos;
using API.Presenters.Cadastro.Cliente;
using Application.Cadastros.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.Cadastros;
using Infrastructure.Repositories.Cadastros;
using Infrastructure.Repositories.Identidade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Endpoints.Cadastro
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de clientes
    /// </summary>
    [Route("api/cadastros/clientes")]
    [ApiController]
    [Produces("application/json")]
    public class ClienteController : BaseController
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _context = context;
        }

        /// <summary>
        /// Buscar todos os clientes
        /// </summary>
        /// <returns>Lista de clientes</returns>
        /// <response code="200">Lista de clientes retornada com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoClienteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var gateway = new ClienteRepository(_context);
            var presenter = new BuscarClientesPresenter();
            var handler = new ClienteHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarClientesAsync(ator, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Cliente encontrado</returns>
        /// <response code="200">Cliente encontrado com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var gateway = new ClienteRepository(_context);
            var presenter = new BuscarClientePorIdPresenter();
            var handler = new ClienteHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarClientePorIdAsync(ator, id, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar cliente por documento (CPF ou CNPJ)
        /// </summary>
        /// <param name="documento">CPF ou CNPJ, com ou sem formatação</param>
        /// <returns>Cliente encontrado</returns>
        /// <response code="200">Cliente encontrado com sucesso</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("documento/{documento}")]
        [ProducesResponseType(typeof(RetornoClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDocumento(string documento)
        {
            //Necessário pois CNPJ tem / , então se mandarem com formatação, vai encodar
            var documentoUnencoded = Uri.UnescapeDataString(documento);

            var gateway = new ClienteRepository(_context);
            var presenter = new BuscarClientePorDocumentoPresenter();
            var handler = new ClienteHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarClientePorDocumentoAsync(ator, documentoUnencoded, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar um novo cliente
        /// </summary>
        /// <param name="dto">Dados do cliente a ser criado</param>
        /// <returns>Cliente criado com sucesso</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="403">Acesso negado - Usuário só pode criar cliente com o mesmo documento</response>
        /// <response code="409">Conflito - Cliente já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarClienteDto dto)
        {
            var clienteGateway = new ClienteRepository(_context);
            var usuarioGateway = new UsuarioRepository(_context);
            var presenter = new CriarClientePresenter();
            var handler = new ClienteHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.CriarClienteAsync(ator, dto.Nome, dto.DocumentoIdentificador, clienteGateway, usuarioGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Atualizar um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="dto">Dados do cliente a ser atualizado</param>
        /// <returns>Cliente atualizado com sucesso</returns>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="403">Acesso negado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarClienteDto dto)
        {
            var gateway = new ClienteRepository(_context);
            var presenter = new AtualizarClientePresenter();
            var handler = new ClienteHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.AtualizarClienteAsync(ator, id, dto.Nome, gateway, presenter);
            return presenter.ObterResultado();
        }
    }
}