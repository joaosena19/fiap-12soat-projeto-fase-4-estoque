using Application.Contracts.Monitoramento;
using Application.Extensions.Enums;
using Application.Identidade.Services;
using Shared.Exceptions;

namespace Application.Extensions;

public static class LoggerExtensions
{
    public static IAppLogger ComUseCase(this IAppLogger logger, object useCase)
    {
        var useCaseName = useCase.GetType().Name;
        return logger.ComPropriedade("UseCase", useCaseName);
    }

    public static IAppLogger ComAtor(this IAppLogger logger, Ator ator)
    {
        return logger
            .ComPropriedade("Ator_UsuarioId", ator.UsuarioId)
            .ComPropriedade("Ator_ClienteId", ator.ClienteId)
            .ComPropriedade("Ator_UsuarioRoles", ator.Roles.Select(r => r.ToString()).ToArray());
    }

    public static IAppLogger ComDomainErrorType(this IAppLogger logger, DomainException ex)
    {
        return logger.ComPropriedade("DomainErrorType", ex.ErrorType);
    }

    public static IAppLogger ComMensageria(this IAppLogger logger, NomeMensagemEnum nomeMensagem, TipoMensagemEnum tipo)
    {
        return logger
            .ComPropriedade("Mensagem_Nome", nomeMensagem.ToString())
            .ComPropriedade("Mensagem_Tipo", tipo.ToString())
            .ComPropriedade("Eh_Mensageria", true);
    }
}