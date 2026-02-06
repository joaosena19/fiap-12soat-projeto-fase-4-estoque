using API.Dtos;
using API.Endpoints;
using API.Presenters.Cadastro.Servico;
using Application.Cadastros.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.Cadastros;
using Infrastructure.Repositories.Cadastros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Endpoints.Cadastro
{
    /// <summary>
    /// Controller para gerenciamento de cadastro de serviços
    /// </summary>
    [Route("api/cadastros/servicos")]
    [ApiController]
    [Produces("application/json")]
    public class ServicoController : BaseController
    {
        private readonly AppDbContext _context;

        public ServicoController(AppDbContext context, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _context = context;
        }

        /// <summary>
        /// Buscar todos os serviços
        /// </summary>
        /// <returns>Lista de serviços</returns>
        /// <response code="200">Lista de serviços retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoServicoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var servicoGateway = new ServicoRepository(_context);
            var presenter = new BuscarServicosPresenter();
            var handler = new ServicoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarServicosAsync(ator, servicoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar serviço por ID
        /// </summary>
        /// <param name="id">ID do serviço</param>
        /// <returns>Serviço encontrado</returns>
        /// <response code="200">Serviço encontrado com sucesso</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var servicoGateway = new ServicoRepository(_context);
            var presenter = new BuscarServicoPorIdPresenter();
            var handler = new ServicoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarServicoPorIdAsync(ator, id, servicoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar um novo serviço
        /// </summary>
        /// <param name="dto">Dados do serviço a ser criado</param>
        /// <returns>Serviço criado com sucesso</returns>
        /// <response code="201">Serviço criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="409">Conflito - Serviço já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarServicoDto dto)
        {
            var servicoGateway = new ServicoRepository(_context);
            var presenter = new CriarServicoPresenter();
            var handler = new ServicoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.CriarServicoAsync(ator, dto.Nome, dto.Preco, servicoGateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Atualizar um serviço existente
        /// </summary>
        /// <param name="id">ID do serviço a ser atualizado</param>
        /// <param name="dto">Dados do serviço a ser atualizado</param>
        /// <returns>Serviço atualizado com sucesso</returns>
        /// <response code="200">Serviço atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoServicoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarServicoDto dto)
        {
            var servicoGateway = new ServicoRepository(_context);
            var presenter = new AtualizarServicoPresenter();
            var handler = new ServicoHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.AtualizarServicoAsync(ator, id, dto.Nome, dto.Preco, servicoGateway, presenter);
            return presenter.ObterResultado();
        }
    }
}
