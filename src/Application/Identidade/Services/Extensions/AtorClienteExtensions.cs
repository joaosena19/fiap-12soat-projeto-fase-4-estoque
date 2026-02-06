using Application.Contracts.Gateways;
using Domain.Cadastros.Aggregates;

namespace Application.Identidade.Services.Extensions;

public static class AtorClienteExtensions
{
    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeAcessarCliente(this Ator ator, Cliente cliente)
    {
        return ator.PodeGerenciarSistema() || cliente.Id == ator.ClienteId;
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeAcessarCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || clienteId == ator.ClienteId;
    }

    /// <summary>
    /// Administrador pode criar qualquer cliente, usuário pode criar cliente apenas com o mesmo documento
    /// </summary>
    public static async Task<bool> PodeCriarClienteAsync(this Ator ator, string documentoCliente, IUsuarioGateway usuarioGateway)
    {
        // Admin pode criar qualquer cliente
        if (ator.PodeGerenciarSistema()) 
            return true;
        
        // Se não é admin, busca o usuário pelo UsuarioId para comparar o documento
        var usuario = await usuarioGateway.ObterPorIdAsync(ator.UsuarioId);
        if (usuario == null) 
            return false;
        
        // Compara o documento do usuário com o documento do cliente a ser criado
        var documentoUsuarioLimpo = LimparFormatacao(usuario.DocumentoIdentificadorUsuario.Valor);
        var documentoNovoLimpo = LimparFormatacao(documentoCliente);
        
        return documentoUsuarioLimpo == documentoNovoLimpo;
    }

    /// <summary>
    /// Somente administrador
    /// </summary>
    public static bool PodeListarClientes(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeEditarCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || ator.ClienteId == clienteId;
    }
    
    /// <summary>
    /// Remove formatação de CPF/CNPJ para comparação
    /// </summary>
    private static string LimparFormatacao(string documento)
    {
        return documento?.Replace(".", "").Replace("/", "").Replace("-", "") ?? string.Empty;
    }
}