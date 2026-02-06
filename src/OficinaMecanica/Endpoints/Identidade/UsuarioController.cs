using API.Dtos;
using API.Endpoints;
using API.Presenters.Identidade.Usuario;
using Application.Identidade.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers.Identidade;
using Infrastructure.Repositories.Identidade;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Options;
using Infrastructure.Authentication.PasswordHashing;
using Microsoft.Extensions.Logging;

namespace API.Endpoints.Identidade
{
    /// <summary>
    /// Controller para gerenciamento de usuários
    /// </summary>
    [Route("api/identidade/usuarios")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioController(AppDbContext context, IConfiguration configuration, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Buscar usuário por documento (CPF ou CNPJ)
        /// </summary>
        /// <param name="documento">CPF ou CNPJ, com ou sem formatação</param>
        /// <returns>Usuário encontrado</returns>
        /// <response code="200">Usuário encontrado com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("documento/{documento}")]
        [ProducesResponseType(typeof(RetornoUsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorDocumento(string documento)
        {
            //Necessário pois CNPJ tem / , então se mandarem com formatação, vai encodar
            var documentoUnencoded = Uri.UnescapeDataString(documento);

            var gateway = new UsuarioRepository(_context);
            var presenter = new BuscarUsuarioPorDocumentoPresenter();
            var handler = new UsuarioHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.BuscarUsuarioPorDocumentoAsync(ator, documentoUnencoded, gateway, presenter);
            return presenter.ObterResultado();
        }

        /// <summary>
        /// Criar um novo usuário
        /// </summary>
        /// <param name="dto">Dados do usuário a ser criado</param>
        /// <returns>Usuário criado com sucesso</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="409">Conflito - Usuário já existe</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RetornoUsuarioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CriarUsuarioDto dto)
        {
            // Configurar PasswordHasher com opções do appsettings
            var argon2Options = new Argon2HashingOptions();
            _configuration.GetSection("Argon2HashingOptions").Bind(argon2Options);
            var options = Options.Create(argon2Options);
            var passwordHasher = new PasswordHasher(options);

            var gateway = new UsuarioRepository(_context);
            var presenter = new CriarUsuarioPresenter();
            var handler = new UsuarioHandler(_loggerFactory);
            var ator = BuscarAtorAtual();
            
            await handler.CriarUsuarioAsync(ator, dto, gateway, presenter, passwordHasher);
            return presenter.ObterResultado();
        }
    }
}