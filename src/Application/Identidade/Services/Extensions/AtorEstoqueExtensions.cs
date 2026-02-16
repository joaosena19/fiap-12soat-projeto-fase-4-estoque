using Application.Identidade.Services;

namespace Application.Identidade.Services.Extensions
{
    public static class AtorEstoqueExtensions
    {
        public static bool PodeGerenciarEstoque(this Ator ator)
        {
            return ator.PodeGerenciarSistema();
        }
    }
}