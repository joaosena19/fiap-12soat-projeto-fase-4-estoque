using Application.Identidade.Services;

namespace Application.Identidade.Services.Extensions
{
    public static class AtorUsuarioExtensions
    {
        public static bool PodeGerenciarUsuarios(this Ator ator)
        {
            return ator.PodeGerenciarSistema();
        }
    }
}