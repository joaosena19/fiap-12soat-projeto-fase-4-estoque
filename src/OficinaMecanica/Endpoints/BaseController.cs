using Application.Identidade.Services;
using Infrastructure.Authentication.AtorFactories;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public abstract class BaseController : ControllerBase
{
    // Factory nativa do .NET
    protected readonly ILoggerFactory _loggerFactory;

    protected BaseController(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Busca o ator atual baseado no token JWT
    /// </summary>
    /// <returns>Ator atual autenticado</returns>
    protected Ator BuscarAtorAtual()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            throw new UnauthorizedAccessException("Token de autorização é obrigatório");

        var token = authHeader.Replace("Bearer ", "");
        return AtorJwtFactory.CriarPorTokenJwt(token);
    }
}