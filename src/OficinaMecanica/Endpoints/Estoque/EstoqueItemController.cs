using API.Dtos;
using API.Endpoints;
using API.Presenters.Estoque;
using Application.Estoque.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.Estoque;
using Infrastructure.Repositories.Estoque;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Endpoints.Estoque
{
    /// <summary>
    /// Controller para gerenciamento de itens de estoque
    /// </summary>
    [Route("api/estoque/itens")]
    [ApiController]
    [Produces("application/json")]
    public class EstoqueItemController : BaseController
    {
        private readonly AppDbContext _context;

        public EstoqueItemController(AppDbContext context, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _context = context;
        }

        /// <summary>
        /// Buscar todos os itens de estoque
        /// </summary>
        /// <returns>Lista de itens de estoque</returns>
        /// <response code="200">Lista de itens de estoque retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RetornoItemEstoqueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new BuscarTodosItensEstoquePresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarTodosItensEstoqueAsync(ator, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Buscar item de estoque por ID
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <returns>Item de estoque encontrado</returns>
        /// <response code="200">Item de estoque encontrado com sucesso</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new BuscarItemEstoquePorIdPresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarItemEstoquePorIdAsync(ator, id, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar um novo item de estoque
        /// </summary>
        /// <param name="dto">Dados do item de estoque a ser criado</param>
        /// <returns>Item de estoque criado com sucesso</returns>
        /// <response code="201">Item de estoque criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoItemEstoqueDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarItemEstoqueDto dto)
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new CriarItemEstoquePresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.CriarItemEstoqueAsync(ator, dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Atualizar um item de estoque existente
        /// </summary>
        /// <param name="id">ID do item de estoque a ser atualizado</param>
        /// <param name="dto">Dados do item de estoque a ser atualizado</param>
        /// <returns>Item de estoque atualizado com sucesso</returns>
        /// <response code="200">Item de estoque atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarItemEstoqueDto dto)
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new AtualizarItemEstoquePresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.AtualizarItemEstoqueAsync(ator, id, dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Atualizar apenas a quantidade de um item de estoque
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <param name="dto">Nova quantidade do item de estoque</param>
        /// <returns>Item de estoque com quantidade atualizada</returns>
        /// <response code="200">Quantidade atualizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPatch("{id}/quantidade")]
        [ProducesResponseType(typeof(RetornoItemEstoqueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateQuantidade(Guid id, [FromBody] AtualizarQuantidadeDto dto)
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new AtualizarQuantidadePresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.AtualizarQuantidadeAsync(ator, id, dto.Quantidade, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Verificar disponibilidade de um item de estoque
        /// </summary>
        /// <param name="id">ID do item de estoque</param>
        /// <param name="quantidadeRequisitada">Quantidade necessária para verificação</param>
        /// <returns>Informações sobre a disponibilidade do item</returns>
        /// <response code="200">Verificação realizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Item de estoque não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}/disponibilidade")]
        [ProducesResponseType(typeof(RetornoDisponibilidadeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerificarDisponibilidade(Guid id, int quantidadeRequisitada)
        {
            var gateway = new ItemEstoqueRepository(_context);
            var presenter = new VerificarDisponibilidadePresenter();
            var handler = new ItemEstoqueHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.VerificarDisponibilidadeAsync(ator, id, quantidadeRequisitada, gateway, presenter);
            return presenter.ObterResultado();
        }
    }
}
