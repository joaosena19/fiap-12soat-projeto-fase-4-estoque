using Application.Identidade.Dtos;
using Application.Identidade.Services;
using Application.Identidade.UseCases.Usuario;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;
using Infrastructure.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.Identidade
{
    public class UsuarioHandler : BaseHandler
    {
        public UsuarioHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public async Task CriarUsuarioAsync(Ator ator, CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter, IPasswordHasher passwordHasher)
        {
            var useCase = new CriarUsuarioUseCase();
            var logger = CriarLoggerPara<CriarUsuarioUseCase>();
            
            await useCase.ExecutarAsync(ator, dto, gateway, presenter, passwordHasher, logger);
        }

        public async Task BuscarUsuarioPorDocumentoAsync(Ator ator, string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            var useCase = new BuscarUsuarioPorDocumentoUseCase();
            var logger = CriarLoggerPara<BuscarUsuarioPorDocumentoUseCase>();
            
            await useCase.ExecutarAsync(ator, documento, gateway, presenter, logger);
        }
    }
}