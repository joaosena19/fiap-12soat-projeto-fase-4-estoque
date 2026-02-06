namespace Application.Identidade.Services.Extensions;

public static class AtorServicoExtensions
{
    /// <summary>
    /// Somente administrador pode gerenciar serviços
    /// </summary>
    public static bool PodeGerenciarServicos(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }
}